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
        protected int ChannelCount { get; }
        protected int QueueCapacity { get; }

        protected double FirstClientArrivalTime { get; private set; }
        protected double LastClientDepartureTime { get; private set; }

        protected bool Finished { get; private set; }

        protected bool ShowPreviousLines { get; private set; }

        protected double SummaryServiceTime { get; private set; }

        protected int ServedClientCount { get; private set; }
        protected int LostClientCount { get; private set; }
        protected int SummaryClientCount => ServedClientCount + LostClientCount;

        protected int QueuedClientCount { get; private set; }

        protected int TopY => 2 + ChannelCount + QueueCapacity;

        readonly double[] QueueBusyTimes;

        double currentClientPosition;
        int currentClientIndex;

        string currentLineMetadata = "";

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

            ChannelLeaveTime = new double[ChannelCount];
            QueueLeaveTime = new double[QueueCapacity];

            ChannelBusyTimes = new double[ChannelCount + 1];//+ время занятости 0 каналов
            QueueBusyTimes = new double[QueueCapacity];

            ShowPreviousLines = true;
        }

        /// <summary>
        /// Добавляет точку на текущую линию.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        protected abstract void AddPathPoint(double y, double x);

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
        protected abstract void OnPathFinished(double y, double x, int clientNumber, string metadata);

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

        #region ITimeDiagram
        void ITimeDiagram.PushStartPoint(double arrivalTime, double realRndValue, double rndValue)
        {
            currentClientPosition = arrivalTime;
            currentClientIndex = SummaryClientCount;

            if (SummaryClientCount == 0)
                FirstClientArrivalTime = arrivalTime;

            int y = TopY;

            OnPathStarted(y, arrivalTime);
            AddPathPoint(y, arrivalTime);

            currentLineMetadata = $"Заявка { SummaryClientCount + 1 }.\nt (интервал момента прихода): { (float)rndValue } ч (rnd: { (float)realRndValue });\n";

            foreach (var time in ChannelLeaveTime)
                if (time >= arrivalTime)
                    ClientAtTimeSum++;

            foreach (var time in QueueLeaveTime)
                if (time >= arrivalTime)
                    ClientAtTimeSum++;
        }

        double[] ChannelLeaveTime;
        double[] QueueLeaveTime;
        public double[] ChannelBusyTimes;

        int ClientAtTimeSum;

        void ITimeDiagram.PushChannelLine(int channelIndex, double serviceTime, double realRndValue)
        {
            double departureTime = currentClientPosition + serviceTime;

            ChannelLeaveTime[channelIndex] = departureTime;

            int busyChannels = 1;

            foreach (var maxDepTime in ChannelLeaveTime.OrderByDescending(x => x))
            {
                if (maxDepTime >= departureTime)
                {
                    ChannelBusyTimes[busyChannels] += serviceTime;
                    ChannelBusyTimes[busyChannels - 1] -= serviceTime;
                }
                else
                {
                    double timeDiff = maxDepTime - currentClientPosition;

                    if (timeDiff <= 0)
                        break;

                    ChannelBusyTimes[busyChannels] += timeDiff;
                    ChannelBusyTimes[busyChannels - 1] -= timeDiff;
                }

                busyChannels++;
            }

            SummaryServiceTime += serviceTime;

            int y = TopY - 1 - channelIndex;
            AddPathPoint(y, currentClientPosition);
            AddPathPoint(y, departureTime);

            currentLineMetadata += $"t (время обслуживания): { (float)serviceTime } ч (rnd: { (float)realRndValue }).";
            currentClientPosition = departureTime;
        }

        void ITimeDiagram.PushQueueLine(int queuePlaceIndex, double queueTime)
        {
            if (currentClientIndex == SummaryClientCount)
            {
                QueuedClientCount++;
                currentClientIndex++;
            }

            double departureTime = currentClientPosition + queueTime;

            QueueLeaveTime[queuePlaceIndex] = departureTime;

            QueueBusyTimes[queuePlaceIndex] += queueTime;

            if (queuePlaceIndex != 0)
                QueueBusyTimes[queuePlaceIndex - 1] -= queueTime;

            int y = TopY - 1 - ChannelCount - queuePlaceIndex;

            AddPathPoint(y, currentClientPosition);
            AddPathPoint(y, departureTime);
            currentClientPosition = departureTime;
        }

        void ITimeDiagram.PushServedPoint()
        {
            ServedClientCount++;
            int y = 1;
            FinishPath(y, currentClientPosition);
        }

        void ITimeDiagram.PushRefusedPoint()
        {
            LostClientCount++;
            int y = 0;
            FinishPath(y, currentClientPosition);
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
            OnPathFinished(y, x, SummaryClientCount, currentLineMetadata);
        }

        void ITimeDiagram.FinishDiagram()
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

            Finished = true;
            PostProcessDiagram();
        }

        bool ITimeDiagram.Finished => Finished;
        #endregion

        #region IDiagramData

        ParametersContainer IDiagramData.ReadonlyParameters => readonlyParameters;

        double IDiagramData.AverageClientCount => (double)ClientAtTimeSum / SummaryClientCount;

        int IDiagramData.ChannelCount => ChannelCount;

        int IDiagramData.QueueCapacity => QueueCapacity;

        int IDiagramData.SummaryClientCount => SummaryClientCount;

        int IDiagramData.ServedClientCount => ServedClientCount;

        int IDiagramData.LostClientCount => LostClientCount;

        double IDiagramData.FirstClientArrivalTime => FirstClientArrivalTime;

        double IDiagramData.LastClientDepartureTime => LastClientDepartureTime;

        double IDiagramData.SystemWorkTime => LastClientDepartureTime;

        double[] IDiagramData.QueueBusyTimes => QueueBusyTimes;

        double IDiagramData.SummaryServiceTime => SummaryServiceTime;

        int IDiagramData.QueuedClientCount => QueuedClientCount;

        double IDiagramData.GetChannelsIntersectionLength(int channelCount)
        {
            if (channelCount == 0)
                return LastClientDepartureTime - ChannelBusyTimes[1];
            if (channelCount == ChannelCount)
                return ChannelBusyTimes[channelCount];

            return ChannelBusyTimes[channelCount] - ChannelBusyTimes[channelCount + 1];
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
