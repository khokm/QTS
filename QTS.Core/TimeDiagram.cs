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

        double m_systemStartTime;
        double m_systemStopTime;
        bool m_diagramFinished;

        double m_serviceTime;
        int m_clientsServed;
        int m_clientsLost;
        int m_queueClientCount;
        int m_clientsCount;

        int m_channelCount;
        int m_queueCapacity;

        List<Line>[] m_queueLines;
        double[] m_queueBusyTime;

        List<Line>[] m_channelLines;

        /* Массив "обратных "линий (т.е. пустых промежутков между реальными линиями).
         * Используется при вычислении одновременной занятости n каналов:
         * для этого берется одна из возможных комбинаций (сочетание) n каналов, находится пересечение ее составляющих.
         * После этого вычитаются любые промежутки, где есть пересечение с другими каналами, не входящими в комбинацию - 
         * то есть, просто находится пересечение с линиями из этого массива.
         */
        Line[][] m_inverseChannelLines;

        int currentVisibleIndex;

        bool m_showPreviousLines;

        protected int TopY => 2 + ChannelCount + QueueCapacity;

        /// <summary>
        /// Создает пустую диаграмму.
        /// </summary>
        /// <param name="channelCount">Количество каналов</param>
        /// <param name="queueCapacity">Количество мест обслуживания</param>
        protected TimeDiagram(int channelCount, int queueCapacity)
        {
            m_channelCount = channelCount;
            m_queueCapacity = queueCapacity;

            m_channelLines = new List<Line>[channelCount];
            for (int i = 0; i < channelCount; i++)
                m_channelLines[i] = new List<Line>();

            m_queueLines = new List<Line>[queueCapacity];
            for (int i = 0; i < queueCapacity; i++)
                m_queueLines[i] = new List<Line>();

            m_queueBusyTime = new double[queueCapacity];
            
            m_showPreviousLines = true;
        }

        /// <summary>
        /// Добавляет точку на текущую линию.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        protected abstract void AddVisualPoint(double y, double x);

        /// <summary>
        /// Вызывается при создании нового пути заявки
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        protected abstract void OnPathStarted(double y, double x);

        /// <summary>
        /// Вызывается по окончании создания пути заявки
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        protected abstract void OnPathFinished(double y, double x);

        /// <summary>
        /// Вызывается по окончании создания диаграммы
        /// </summary>
        protected abstract void OnDiagramFinished();

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

            OnViewUpdated?.Invoke();
        }

        /// <summary>
        /// Заканчивает создание пути
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        void FinishPath(double y, double x)
        {
            if (LastClientDepartureTime < x)
                m_systemStopTime = x;

            OnPathFinished(y, x);
            m_clientsCount++;
        }

        /// <summary>
        /// Возвращает массив "обратных" линий.
        /// Вызывается по окончании создания диаграммы.
        /// </summary>
        Line[][] CaluclateInverseChannelLines()
        {
            Line[][] inverseLines = new Line[ChannelCount][];

            for (int i = 0; i < ChannelCount; i++)
            {
                var lines = m_channelLines[i];

                if (lines.Count == 0)
                {
                    inverseLines[i] = new []{ new Line(FirstClientArrivalTime, LastClientDepartureTime) };
                    continue;
                }

                List<Line> result = new List<Line>();

                result.Add(new Line(FirstClientArrivalTime, lines[0].start));

                for (int k = 0; k < lines.Count - 1; k++)
                    result.Add(new Line(lines[k].end, lines[k + 1].start));

                result.Add(new Line(lines[lines.Count - 1].end, LastClientDepartureTime));

                inverseLines[i] = result.ToArray();
            }

            return inverseLines;
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
        public void PushStartPoint(double arrivalTime)
        {
            if (SummaryClientCount == 0)
                m_systemStartTime = arrivalTime;

            int y = TopY;

            OnPathStarted(y, arrivalTime);
            AddVisualPoint(y, arrivalTime);
        }

        public void PushChannelLine(int channelIndex, double start, double end)
        {
            m_channelLines[channelIndex].Add(new Line(start, end));

            m_serviceTime += end - start;

            int y = TopY - 1 - channelIndex;
            AddVisualPoint(y, start);
            AddVisualPoint(y, end);
        }

        public void IncrementQueueClientCount()
        {
            m_queueClientCount++;
        }

        public void PushQueueLine(int queuePlaceIndex, double start, double end)
        {
            m_queueLines[queuePlaceIndex].Add(new Line(start, end));

            double currentQueuedTime = end - start;
            m_queueBusyTime[queuePlaceIndex] += currentQueuedTime;

            if (queuePlaceIndex != 0)
                m_queueBusyTime[queuePlaceIndex - 1] -= currentQueuedTime;

            int y = TopY - 1 - ChannelCount - queuePlaceIndex;
            AddVisualPoint(y, start);
            AddVisualPoint(y, end);
        }

        public void PushServedPoint(double departureTime)
        {
            m_clientsServed++;
            int y = 1;
            AddVisualPoint(y, departureTime);
            FinishPath(y, departureTime);
        }

        public void PushRefusedPoint(double departureTime)
        {
            m_clientsLost++;
            int y = 0;
            AddVisualPoint(y, departureTime);
            FinishPath(y, departureTime);
        }

        public void FinishDiagram()
        {
            if (m_diagramFinished)
                throw new Exception("Диаграмма уже построена");

            m_diagramFinished = true;
            m_inverseChannelLines = CaluclateInverseChannelLines();
            OnDiagramFinished();
        }

        public bool Finished => m_diagramFinished;
        #endregion

        #region IDiagramData
        public int ChannelCount => m_channelCount;

        public int QueueCapacity => m_queueCapacity;

        public int SummaryClientCount => m_clientsCount;

        public int ServedClientCount => m_clientsServed;

        public int LostClientCount => m_clientsLost;

        public double FirstClientArrivalTime => m_systemStartTime;

        public double LastClientDepartureTime => m_systemStopTime;

        public double SystemWorkTime => LastClientDepartureTime - FirstClientArrivalTime;

        public double[] QueueBusyTimes => m_queueBusyTime;

        public double SummaryServiceTime => m_serviceTime;

        public int QueuedClientCount => m_queueClientCount;

        public double GetChannelsIntersectionLength(int count)
        {
            double intersectionLength = 0;

            foreach (var usingChannels in Permuter.Permute(ChannelCount, count))
            {
                var intersection = new Line[]
                {
                    new Line(0, double.MaxValue)
                };

                for (int i = 0; i < count; i++)
                    intersection = LineIntersect(intersection, m_channelLines[usingChannels[i]]);

                for (int i = 0; i < ChannelCount; i++)
                    if (!usingChannels.Contains(i))
                        intersection = LineIntersect(intersection, m_inverseChannelLines[i]);

                foreach (var line in intersection)
                    intersectionLength += line.end - line.start;

            }

            return intersectionLength;
        }

        public int GetClientCountAtTime(double point, double step)
        {
            int count = 0;

            foreach (var line in m_queueLines)
            {
                var result = LineIntersect(line, new[] { new Line(point - step / 2, point + step / 2) });

                if (result.Length != 0)
                    count++;
            }

            foreach (var line in m_channelLines)
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
        public bool ShowPreviousLines
        {
            get
            {
                return m_showPreviousLines;
            }
            set
            {
                m_showPreviousLines = value;
                SetVisibleLinesCount(currentVisibleIndex);
            }
        }

        public void GoToStart()
        {
            SetVisibleLinesCount(0);
        }

        public void GoToEnd()
        {
            SetVisibleLinesCount(SummaryClientCount - 1);
        }

        public void StepForward()
        {
            if (currentVisibleIndex == SummaryClientCount - 1)
                return;

            SetVisibleLinesCount(currentVisibleIndex + 1);
        }

        public void StepBack()
        {
            if (currentVisibleIndex == 0)
                return;

            SetVisibleLinesCount(currentVisibleIndex - 1);
        }

        public event Action OnViewUpdated;
        #endregion
    }
}
