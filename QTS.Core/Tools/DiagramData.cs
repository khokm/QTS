using System;
using System.Linq;

namespace QTS.Core
{
    class DiagramData
    {
        public bool Finished { get; private set; }
        //protected double FirstClientArrivalTime { get; private set; }
        public double SystemWorkTime { get; private set; }

        public double SummaryServiceTime { get; private set; }

        public int ServedClientCount { get; private set; }
        public int LostClientCount { get; private set; }
        public int SummaryClientCount => ServedClientCount + LostClientCount;

        public int QueuedClientCount { get; set; }

        public double AverageClientCount => (double)clientsAtTimeSum / checksSum;

        public double[] ChannelBusyTimes { get; }

        public double GetSummaryChannelBusyTime(int channelCount) => (channelCount > ChannelCount || channelCount < 0) ? 0 : ChannelBusyTimes[channelCount];

        public double GetQueueBusyTime(int queueIndex) => (queueIndex >= QueueCapacity || queueIndex < 0) ? 0 : QueueBusyTimes[queueIndex];

        public InteractiveDiagram InteractiveDiagram { get; }

        public ParametersContainer ReadonlyParameters { get; }

        public double[] QueueBusyTimes;

        double currentClientPosition;
        int currentClientIndex;

        double[] ChannelLeaveTime;
        double[] QueueLeaveTime;

        int clientsAtTimeSum;
        int checksSum;

        public int ChannelCount { get; }

        public int QueueCapacity { get; }

        //{
        //    if (channelCount == ChannelCount)
        //        return ChannelBusyTimes[channelCount];
        //    if (channelCount == 0)
        //        return SystemWorkTime - ChannelBusyTimes[1];

        //    return ChannelBusyTimes[channelCount] - ChannelBusyTimes[channelCount + 1];
        //}

        /// <summary>
        /// Создает пустую диаграмму.
        /// </summary>
        /// <param name="channelCount">Количество каналов</param>
        /// <param name="queueCapacity">Количество мест обслуживания</param>
        public DiagramData(ParametersContainer parameters, InteractiveDiagram interactiveDiagram)
        {
            //2 + ChannelCount + QueueCapacity
            ReadonlyParameters = parameters;
            ChannelCount = parameters.ChannelCount;
            QueueCapacity = parameters.QueueCapacity;

            ChannelLeaveTime = new double[ChannelCount];
            QueueLeaveTime = new double[QueueCapacity];

            ChannelBusyTimes = new double[ChannelCount + 1];//+ время занятости 0 каналов
            QueueBusyTimes = new double[QueueCapacity];

            InteractiveDiagram = interactiveDiagram;
        }

        public void AddToClientSum(double checkTime)
        {
            foreach (var time in ChannelLeaveTime)
                if (time >= checkTime)
                    clientsAtTimeSum++;

            foreach (var time in QueueLeaveTime)
                if (time >= checkTime)
                    clientsAtTimeSum++;

            checksSum++;
        }

        public void PushStartPoint(double arrivalTime, double realRndValue, double rndValue)
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

            currentClientPosition = arrivalTime;
            currentClientIndex = SummaryClientCount;

            //if (SummaryClientCount == 0)
            //    FirstClientArrivalTime = arrivalTime;

            if(InteractiveDiagram != null)
            {
                int y = 2 + ChannelCount + QueueCapacity;
                string metadata = $"Заявка { SummaryClientCount + 1 }.\nt (интервал момента прихода): { (float)rndValue } ч (rnd: { (float)realRndValue });\n";
                InteractiveDiagram.OnPathStarted(y, arrivalTime);
                InteractiveDiagram.AddPathPoint(y, arrivalTime);
                InteractiveDiagram.AddPathMetadata(metadata);
            }
        }

        public void PushChannelLine(int channelIndex, double serviceTime, double realRndValue)
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

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

            if(InteractiveDiagram != null)
            {
                int y = 1 + ChannelCount - channelIndex + QueueCapacity;
                InteractiveDiagram.AddPathPoint(y, currentClientPosition);
                InteractiveDiagram.AddPathPoint(y, departureTime);

                string metadata = $"t (время обслуживания): { (float)serviceTime } ч (rnd: { (float)realRndValue }).";

                InteractiveDiagram.AddPathMetadata(metadata);
            }

            currentClientPosition = departureTime;
        }

        public void PushQueueLine(int queuePlaceIndex, double queueTime)
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

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

            if(InteractiveDiagram != null)
            {
                int y = QueueCapacity - queuePlaceIndex + 1;

                InteractiveDiagram.AddPathPoint(y, currentClientPosition);
                InteractiveDiagram.AddPathPoint(y, departureTime);
            }

            currentClientPosition = departureTime;
        }

        public void PushServedPoint()
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

            ServedClientCount++;
            int y = 1;
            FinishPath(y, currentClientPosition);
        }

        public void PushRefusedPoint()
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

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
            if (Finished)
                throw new Exception("Диаграмма уже построена");

            if (SystemWorkTime < x)
                SystemWorkTime = x;

            if(InteractiveDiagram != null)
            {
                InteractiveDiagram.AddPathPoint(y, x);
                InteractiveDiagram.OnPathFinished(y, x, SummaryClientCount);
            }
        }

        public void FinishDiagram()
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

            //{
            //    if (channelCount == ChannelCount)
            //        return ChannelBusyTimes[channelCount];
            //    if (channelCount == 0)
            //        return SystemWorkTime - ChannelBusyTimes[1];

            //    return ChannelBusyTimes[channelCount] - ChannelBusyTimes[channelCount + 1];
            //}

            ChannelBusyTimes[0] = SystemWorkTime - (ChannelCount == 0 ? 0 : ChannelBusyTimes[1]);

            for (int i = 1; i < ChannelCount; i++)
                ChannelBusyTimes[i] -= ChannelBusyTimes[i + 1];

            Finished = true;
            if(InteractiveDiagram != null)
                InteractiveDiagram.PostProcessDiagram();
        }
    }
}
