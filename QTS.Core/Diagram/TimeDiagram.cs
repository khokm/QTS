using System;
using System.Linq;
using QTS.Core.Graphics;

namespace QTS.Core.Diagram
{
    /// <summary>
    /// Представляет временную диаграмму, которую можно заполнять значениями и анализировать.
    /// </summary>
    class TimeDiagram : IAnalyzable
    {
        /// <summary>
        /// Завершено ли построение диаграммы?
        /// </summary>
        public bool Finished { get; private set; }
        //protected double FirstClientArrivalTime { get; private set; }

        /// <summary>
        /// <see cref="IAnalyzable.ChannelCount"/>
        /// </summary>
        public int ChannelCount { get; }

        /// <summary>
        /// <see cref="IAnalyzable.QueueCapacity"/>
        /// </summary>
        public int QueueCapacity { get; }

        /// <summary>
        /// <see cref="IAnalyzable.SystemWorkTime"/>
        /// </summary>
        public double SystemWorkTime { get; private set; }

        /// <summary>
        /// <see cref="IAnalyzable.SummaryServiceTime"/>
        /// </summary>
        public double SummaryServiceTime { get; private set; }

        /// <summary>
        /// <see cref="IAnalyzable.ServedClientCount"/>
        /// </summary>
        public int ServedClientCount { get; private set; }

        /// <summary>
        /// <see cref="IAnalyzable.LostClientCount"/>
        /// </summary>
        public int LostClientCount { get; private set; }

        /// <summary>
        /// <see cref="IAnalyzable.SummaryClientCount"/>
        /// </summary>
        public int SummaryClientCount => ServedClientCount + LostClientCount;

        /// <summary>
        /// <see cref="IAnalyzable.QueuedClientCount"/>
        /// </summary>
        public int QueuedClientCount { get; private set; }

        /// <summary>
        /// <see cref="IAnalyzable.AverageClientCount"/>
        /// </summary>
        public double AverageClientCount => (double)clientsAtTimeSum / checksSum;

        /// <summary>
        /// <see cref="IAnalyzable.ChannelBusyTimes"/>
        /// </summary>
        public double[] ChannelBusyTimes { get; }

        /// <summary>
        /// <see cref="IAnalyzable.QueueBusyTimes"/>
        /// </summary>
        public double[] QueueBusyTimes { get; }

        InteractiveDiagram interactiveDiagram { get; } = null;

        double currentClientPosition;
        int currentClientIndex;

        double[] ChannelLeaveTime;
        double[] QueueLeaveTime;

        int clientsAtTimeSum;
        int checksSum;

        /// <summary>
        /// Создает новый экземпляр временной диаграммы.
        /// </summary>
        /// <param name="channelCount">Количество каналов обслуживания.</param>
        /// <param name="queueCapacity">Количество мест в очереди.</param>
        public TimeDiagram(int channelCount, int queueCapacity)
        {
            ChannelCount = channelCount;
            QueueCapacity = queueCapacity;

            ChannelLeaveTime = new double[ChannelCount];
            QueueLeaveTime = new double[QueueCapacity];

            ChannelBusyTimes = new double[ChannelCount + 1];//+ время занятости 0 каналов
            QueueBusyTimes = new double[QueueCapacity];
        }

        /// <summary>
        /// Создает новый экземпляр временной диаграммы с графическими данными.
        /// </summary>
        /// <param name="channelCount">Количество каналов обслуживания.</param>
        /// <param name="queueCapacity">Количество мест в очереди.</param>
        /// <param name="interactiveDiagram">Экземпляр <see cref="InteractiveDiagram"/> для создания графических данных.</param>
        public TimeDiagram(int channelCount, int queueCapacity, InteractiveDiagram interactiveDiagram) : this(channelCount, queueCapacity)
        {
            if (interactiveDiagram == null)
                throw new NullReferenceException("InteractiveDiagram is null");

            this.interactiveDiagram = interactiveDiagram;
        }

        /// <summary>
        /// Считает количество заявок в системе в указанный момент времени. Результат прибавляется к статистике.
        /// </summary>
        /// <param name="checkTime"></param>
        public void CheckClientCountAtTime(double checkTime)
        {
            foreach (var time in ChannelLeaveTime)
                if (time >= checkTime)
                    clientsAtTimeSum++;

            foreach (var time in QueueLeaveTime)
                if (time >= checkTime)
                    clientsAtTimeSum++;

            checksSum++;
        }

        /// <summary>
        /// Создает новый путь заявки.
        /// Если используются графические данные, добавяет точку на линии "Заявки".
        /// </summary>
        /// <param name="arrivalTime">Время прибытия заявки</param>
        /// <param name="realRndValue">Значение ГСЧ, которое было использовано в формуле расчета пуассоновского потока. Используется как метаданные о заявке.</param>
        /// <param name="interval">Интервал момента прихода.</param>
        public void PushStartPoint(double arrivalTime, double realRndValue, double interval)
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

            currentClientPosition = arrivalTime;
            currentClientIndex = SummaryClientCount;

            //if (SummaryClientCount == 0)
            //    FirstClientArrivalTime = arrivalTime;

            if (interactiveDiagram != null)
            {
                int y = 2 + ChannelCount + QueueCapacity;
                string metadata = $"Заявка { SummaryClientCount + 1 }.\nt (интервал момента прихода): { (float)interval } ч (rnd: { (float)realRndValue });\n";

                interactiveDiagram.BeginInteractiveLine(SummaryClientCount);
                interactiveDiagram.AddInteractiveAnnotation(y, arrivalTime, (SummaryClientCount + 1).ToString(), true, SummaryClientCount);
                interactiveDiagram.AddPoint(y, arrivalTime);
                interactiveDiagram.AddLineMetadata(metadata);
            }
        }

        /// <summary>
        /// Создает отрезок пути заявки на указанном месте обслуживания.
        /// Если используются графические данные, добавляет отрезок на линии "Канал &lt;<paramref name="channelIndex"/> + 1&gt;".
        /// </summary>
        /// <param name="channelIndex">&lt;Номер канала - 1&gt;.</param>
        /// <param name="serviceTime">Время обслуживания.</param>
        /// <param name="realRndValue">Значение ГСЧ, которое было использовано в формуле расчета пуассоновского потока. Используется как метаданные о заявке.</param>
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

            if (interactiveDiagram != null)
            {
                int y = 1 + ChannelCount - channelIndex + QueueCapacity;

                interactiveDiagram.AddPoint(y, currentClientPosition);
                interactiveDiagram.AddPoint(y, departureTime);

                string metadata = $"t (время обслуживания): { (float)serviceTime } ч (rnd: { (float)realRndValue }).";

                interactiveDiagram.AddLineMetadata(metadata);
            }

            currentClientPosition = departureTime;
        }

        /// <summary>
        /// Создает отрезок пути заявки на указанном месте в очереди.
        /// Если используются графические данные, добавляет отрезок на линии "Место &lt;<paramref name="queuePlaceIndex"/> + 1&gt;".
        /// </summary>
        /// <param name="queuePlaceIndex">&lt;Номер места обслуживания - 1&gt;.</param>
        /// <param name="queueTime">Время ожидания на данном месте обслуживания.</param>
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

            if (interactiveDiagram != null)
            {
                int y = QueueCapacity - queuePlaceIndex + 1;

                interactiveDiagram.AddPoint(y, currentClientPosition);
                interactiveDiagram.AddPoint(y, departureTime);
            }

            currentClientPosition = departureTime;
        }

        /// <summary>
        /// Инкрементирует количество обслуженных заявок.
        /// Если используются графические данные, добавляет отрезок на линии "Обслужено".
        /// </summary>
        public void PushServedPoint()
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

            ServedClientCount++;
            int y = 1;
            FinishPath(y, currentClientPosition);
        }

        /// <summary>
        /// Инкрементирует количество отказанных заявок.
        /// Если используются графические данные, добавляет отрезок на линии "Отказано".
        /// </summary>
        public void PushRefusedPoint()
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

            LostClientCount++;
            int y = 0;
            FinishPath(y, currentClientPosition);
        }

        /// <summary>
        /// Завершает путь заявки.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        void FinishPath(double y, double x)
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

            if (SystemWorkTime < x)
                SystemWorkTime = x;

            if (interactiveDiagram != null)
            {
                interactiveDiagram.AddPoint(y, x);
                interactiveDiagram.AddInteractiveAnnotation(y, x, SummaryClientCount.ToString(), false, SummaryClientCount - 1);
                interactiveDiagram.CompleteLine();
            }
        }

        /// <summary>
        /// Завершает создание диаграммы и подготавливает статистические данные для анализа.
        /// </summary>
        public void FinishDiagram()
        {
            if (Finished)
                throw new Exception("Диаграмма уже построена");

            ChannelBusyTimes[0] = SystemWorkTime - (ChannelCount == 0 ? 0 : ChannelBusyTimes[1]);

            for (int i = 1; i < ChannelCount; i++)
                ChannelBusyTimes[i] -= ChannelBusyTimes[i + 1];

            if (interactiveDiagram != null)
                interactiveDiagram.SetLayer(0);

            Finished = true;          
        }
    }
}
