using QTS.Core.Diagram;
using QTS.Core.Graphics;
using QTS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QTS.Core
{
    /// <summary>
    /// Реализация основных возможностей временной диаграммы (заполнение и анализ)
    /// Управление отображением частично должно быть реализовано в производном классе.
    /// </summary>
    public abstract class TimeDiagram : ITimeDiagram, IDiagramData, IDiagramViewController
    {
        /// <summary>
        /// Простая линия
        /// </summary>
        struct Line
        {
            public double start, end;

            public Line(double start, double end)
            {
                this.start = start;
                this.end = end;
            }
        }

        protected int ChannelCount { get; }
        protected int QueueCapacity { get; }

        protected int SummaryClientCount { get; private set; }

        protected double FirstClientArrivalTime { get; private set; }
        protected double LastClientDepartureTime { get; private set; }

        protected bool Finished { get; private set; }

        protected bool ShowPreviousLines { get; private set; }

        protected double SummaryServiceTime { get; private set; }

        protected int ServedClientCount { get; private set; }
        protected int LostClientCount { get; private set; }
        protected int QueuedClientCount { get; private set; }

        protected int TopY => 2 + ChannelCount + QueueCapacity;

        readonly List<Line>[] queueLines;
        readonly double[] QueueBusyTimes;

        double currentClientX;
        int currentClientIndex;

        readonly List<Line>[] channelLines;

        /* Массив "обратных "линий (т.е. пустых промежутков между реальными линиями).
         * Используется при вычислении одновременной занятости n каналов:
         * для этого берется одна из возможных комбинаций (сочетание) n каналов, находится пересечение ее составляющих.
         * После этого вычитаются любые промежутки, где есть пересечение с другими каналами, не входящими в комбинацию - 
         * то есть, просто находится пересечение с линиями из этого массива.
         */
        readonly Line[][] inverseChannelLines;

        int currentVisibleIndex;
        readonly ParametersContainer readonlyParameters;

        event Action viewUpdated;

        /// <summary>
        /// Создает пустую диаграмму.
        /// </summary>
        /// <param name="channelCount">Количество каналов</param>
        /// <param name="queueCapacity">Количество мест обслуживания</param>
        protected TimeDiagram(ParametersContainer parameters)
        {
            readonlyParameters = parameters;
            ChannelCount = parameters.ChannelCount;
            QueueCapacity = parameters.QueueCapacity;

            channelLines = new List<Line>[ChannelCount];
            for (int i = 0; i < ChannelCount; i++)
                channelLines[i] = new List<Line>();

            inverseChannelLines = new Line[ChannelCount][];

            queueLines = new List<Line>[QueueCapacity];
            for (int i = 0; i < QueueCapacity; i++)
                queueLines[i] = new List<Line>();

            QueueBusyTimes = new double[QueueCapacity];

            ShowPreviousLines = true;
        }

        /// <summary>
        /// Добавляет точку на текущую линию.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        protected abstract void AddPathPoint(double y, double x);

        protected abstract void AddChannelRndMetadata(double realRndValue, double rndValue);

        /// <summary>
        /// Вызывается при создании нового пути заявки
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        protected abstract void OnPathStarted(double y, double x, double realRndValue, double rndValue);

        /// <summary>
        /// Вызывается по окончании создания пути заявки
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        protected abstract void OnPathFinished(double y, double x);

        /// <summary>
        /// Вызывается по окончании создания диаграммы
        /// </summary>
        protected abstract void PostProcessDiagram();

        /// <summary>
        /// Вызывается при изменении отображения диаграммы
        /// </summary>
        /// <param name="visibleLineIndex"></param>
        protected abstract void UpdateView(int visibleLineIndex);

        /// <summary>
        /// Устанавливает количество отображаемых линий
        /// </summary>
        /// <param name="lineIndex">Индекс последней отображаемой линии</param>
        protected void SetVisibleLinesCount(int lineIndex)
        {
            currentVisibleIndex = lineIndex;
            UpdateView(currentVisibleIndex);

            viewUpdated?.Invoke();
        }

        /// <summary>
        /// Заканчивает создание пути
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        void FinishPath(double y, double x)
        {
            if (LastClientDepartureTime < x)
                LastClientDepartureTime = x;

            AddPathPoint(y, x);
            OnPathFinished(y, x);
            SummaryClientCount++;
        }

        /// <summary>
        /// Возвращает массив "обратных" линий.
        /// Вызывается по окончании создания диаграммы.
        /// </summary>
        void CaluclateInverseChannelLines()
        {
            for (int i = 0; i < ChannelCount; i++)
            {
                var lines = channelLines[i];

                if (lines.Count == 0)
                {
                    inverseChannelLines[i] = new []{ new Line(FirstClientArrivalTime, LastClientDepartureTime) };
                    continue;
                }

                List<Line> result = new List<Line>();

                result.Add(new Line(FirstClientArrivalTime, lines[0].start));

                for (int k = 0; k < lines.Count - 1; k++)
                    result.Add(new Line(lines[k].end, lines[k + 1].start));

                result.Add(new Line(lines[lines.Count - 1].end, LastClientDepartureTime));

                inverseChannelLines[i] = result.ToArray();
            }
        }

        /// <summary>
        /// Возвращает пересечение двух множеств линий.
        /// </summary>
        /// <param name="lines1"></param>
        /// <param name="lines2"></param>
        /// <returns></returns>
        Line[] LineIntersect(IEnumerable<Line> lines1, IEnumerable<Line> lines2)
        {
            List<Line> result = new List<Line>();

            foreach (Line a in lines1)
            {
                foreach (Line b in lines2)
                {
                    double start = Math.Max(a.start, b.start);
                    double end = Math.Min(a.end, b.end);

                    if (end - start <= 0)
                        continue;

                    result.Add(new Line(start, end));
                }
            }

            return result.ToArray();
        }

        #region ITimeDiagram
        void ITimeDiagram.PushStartPoint(double arrivalTime, double realRndValue, double rndValue)
        {
            currentClientX = arrivalTime;
            currentClientIndex = SummaryClientCount;

            if (SummaryClientCount == 0)
                FirstClientArrivalTime = arrivalTime;

            int y = TopY;

            OnPathStarted(y, arrivalTime, realRndValue, rndValue);
            AddPathPoint(y, arrivalTime);
        }

        void ITimeDiagram.PushChannelLine(int channelIndex, double serviceTime, double realRndValue)
        {
            double departureTime = currentClientX + serviceTime;
            channelLines[channelIndex].Add(new Line(currentClientX, departureTime));

            SummaryServiceTime += serviceTime;

            int y = TopY - 1 - channelIndex;
            AddPathPoint(y, currentClientX);
            AddPathPoint(y, departureTime);
            AddChannelRndMetadata(realRndValue, serviceTime);
            currentClientX = departureTime;
        }

        void ITimeDiagram.PushQueueLine(int queuePlaceIndex, double queueTime)
        {
            if (currentClientIndex == SummaryClientCount)
            {
                QueuedClientCount++;
                currentClientIndex++;
            }

            double departureTime = currentClientX + queueTime;

            queueLines[queuePlaceIndex].Add(new Line(currentClientX, departureTime));

            QueueBusyTimes[queuePlaceIndex] += queueTime;

            if (queuePlaceIndex != 0)
                QueueBusyTimes[queuePlaceIndex - 1] -= queueTime;

            int y = TopY - 1 - ChannelCount - queuePlaceIndex;
            AddPathPoint(y, currentClientX);
            AddPathPoint(y, departureTime);
            currentClientX = departureTime;
        }

        void ITimeDiagram.PushServedPoint()
        {
            ServedClientCount++;
            int y = 1;
            FinishPath(y, currentClientX);
        }

        void ITimeDiagram.PushRefusedPoint()
        {
            LostClientCount++;
            int y = 0;
            FinishPath(y, currentClientX);
        }

        void ITimeDiagram.FinishDiagram()
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

            Finished = true;
            CaluclateInverseChannelLines();
            PostProcessDiagram();
        }

        bool ITimeDiagram.Finished => Finished;
        #endregion

        #region IDiagramData

        ParametersContainer IDiagramData.ReadonlyParameters => readonlyParameters;

        int IDiagramData.ChannelCount => ChannelCount;

        int IDiagramData.QueueCapacity => QueueCapacity;

        int IDiagramData.SummaryClientCount => SummaryClientCount;

        int IDiagramData.ServedClientCount => ServedClientCount;

        int IDiagramData.LostClientCount => LostClientCount;

        double IDiagramData.FirstClientArrivalTime => FirstClientArrivalTime;

        double IDiagramData.LastClientDepartureTime => LastClientDepartureTime;

        double IDiagramData.SystemWorkTime => LastClientDepartureTime - FirstClientArrivalTime;

        double[] IDiagramData.QueueBusyTimes => QueueBusyTimes;

        double IDiagramData.SummaryServiceTime => SummaryServiceTime;

        int IDiagramData.QueuedClientCount => QueuedClientCount;

        double IDiagramData.GetChannelsIntersectionLength(int count)
        {
            double intersectionLength = 0;

            foreach (var usingChannels in Permuter.Permute(ChannelCount, count))
            {
                var intersection = new Line[]
                {
                    new Line(0, double.MaxValue)
                };

                for (int i = 0; i < count; i++)
                    intersection = LineIntersect(intersection, channelLines[usingChannels[i]]);

                for (int i = 0; i < ChannelCount; i++)
                    if (!usingChannels.Contains(i))
                        intersection = LineIntersect(intersection, inverseChannelLines[i]);

                foreach (var line in intersection)
                    intersectionLength += line.end - line.start;

            }

            return intersectionLength;
        }

        int IDiagramData.GetClientCountAtTime(double point, double step)
        {
            int count = 0;

            foreach (var line in queueLines)
            {
                var result = LineIntersect(line, new[] { new Line(point - step / 2, point + step / 2) });

                if (result.Length != 0)
                    count++;
            }

            foreach (var line in channelLines)
            {
                var result = LineIntersect
                    (
                    line,
                    new Line[]
                    {
                        new Line(point - step / 2, point + step / 2)
                    }
                    );

                if (result.Length != 0)
                    count++;
            }

            return count;
        }
        #endregion

        #region IDiagramViewController
        bool IDiagramViewController.ShowPreviousLines
        {
            get
            {
                return ShowPreviousLines;
            }
            set
            {
                ShowPreviousLines = value;
                SetVisibleLinesCount(currentVisibleIndex);
            }
        }

        void IDiagramViewController.GoToStart()
        {
            SetVisibleLinesCount(0);
        }

        void IDiagramViewController.GoToEnd()
        {
            SetVisibleLinesCount(SummaryClientCount - 1);
        }

        void IDiagramViewController.StepForward()
        {
            if (currentVisibleIndex == SummaryClientCount - 1)
                return;

            SetVisibleLinesCount(currentVisibleIndex + 1);
        }

        void IDiagramViewController.StepBack()
        {
            if (currentVisibleIndex == 0)
                return;

            SetVisibleLinesCount(currentVisibleIndex - 1);
        }

        event Action IDiagramViewController.ViewUpdated
        {
            add
            {
                viewUpdated += value;
            }

            remove
            {
                viewUpdated -= value;
            }
        }
        #endregion
    }
}
