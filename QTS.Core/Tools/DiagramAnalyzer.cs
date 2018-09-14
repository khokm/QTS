using QTS.Core.Diagram;

namespace QTS.Core.Tools
{
    /// <summary>
    /// Анализатор диаграммы. Вычисляет значения показателей.
    /// </summary>
    class DiagramAnalyzer
    {
        IDiagramData diagram { get; }

        /*
         * GetChannelsIntersectionLength использует очень много ресурсов,
         * поэтому мы закешируем некоторые произодные от нее результаты,
         * чтобы не пересчитывать их.
         */
        float[] channelBusyProbalitiesCache;

        /*
         * Здесь подобная ситуация, хоть и не столь критично по производительности.
         */
        float[] queueBusyProbalityCache;
        float averageClientServiceTimeCache;
        float averageClientQueueWaitingTimeCache;

        /// <summary>
        /// Создает новый анализатор.
        /// </summary>
        /// <param name="diagram"></param>
        public DiagramAnalyzer(IDiagramData diagram)
        {
            this.diagram = diagram;

            channelBusyProbalitiesCache = new float[diagram.ChannelCount];
            queueBusyProbalityCache = new float[diagram.QueueCapacity];

            if (diagram.SystemWorkTime != 0)
            {
                for (int i = 0; i < diagram.ChannelCount; i++)
                    channelBusyProbalitiesCache[i] = (float)(diagram.GetChannelsIntersectionLength(i + 1) / diagram.SystemWorkTime);

                for (int i = 0; i < diagram.QueueCapacity; i++)
                    queueBusyProbalityCache[i] = (float)(diagram.QueueBusyTimes[i] / diagram.SystemWorkTime);

            }
            averageClientServiceTimeCache = diagram.ServedClientCount == 0 ? 0 : (float)diagram.SummaryServiceTime / diagram.ServedClientCount;

            averageClientQueueWaitingTimeCache = 0;

            if (diagram.QueuedClientCount != 0)
            {
                double summaryTime = 0;

                foreach (var time in diagram.QueueBusyTimes)
                    summaryTime += time;

                averageClientQueueWaitingTimeCache = (float)(summaryTime / diagram.QueuedClientCount);
            }
        }

        /// <summary>
        /// Количество показателей.
        /// </summary>
        public int TotalIndexCount => 9 + diagram.ChannelCount * 2 + diagram.QueueCapacity;

        /// <summary>
        /// Вероятность обслуживания.
        /// </summary>
        public float ServedProbality => diagram.SummaryClientCount == 0 ? 0 : (float)diagram.ServedClientCount / diagram.SummaryClientCount;

        /// <summary>
        /// Вероятность отказа.
        /// </summary>
        public float RefuseProbality => diagram.SummaryClientCount == 0 ? 0 : (float)diagram.LostClientCount / diagram.SummaryClientCount;

        /// <summary>
        /// Пропускная способность системы.
        /// </summary>
        public float SystemThroughput => diagram.SystemWorkTime == 0 ? 0 : (float)(diagram.ServedClientCount / diagram.SystemWorkTime);

        /// <summary>
        /// Вероятность P занятости n и только n каналов,
        /// где P = arr[n - 1]
        /// </summary>
        public float[] ChannelBusyProbalies => channelBusyProbalitiesCache;

        /// <summary>
        /// Среднее количество занятых каналов.
        /// </summary>
        public float AverageBusyChannelCount
        {
            get
            {
                float value = 0;

                for(int i = 0; i < diagram.ChannelCount; i++)
                    value += (i + 1) * channelBusyProbalitiesCache[i];

                return value;
            }
        }

        /// <summary>
        /// Вероятность P простоя хотя бы n каналов,
        /// где P = arr[n - 1]
        /// </summary>
        public float[] ChannelIdleProbality
        {
            get
            {
                float[] cip = new float[diagram.ChannelCount];

                //if (diagram.SystemWorkTime == 0) //Так прога крашилась, если каналов было 0...
                if (diagram.SystemWorkTime == 0 || diagram.ChannelCount == 0) //поэтому пришлось добавить второе условие
                    return cip;

                float totalProb = (float)(diagram.GetChannelsIntersectionLength(0) /diagram.SystemWorkTime);

                cip[diagram.ChannelCount - 1] = totalProb; //Собственно, здесь и крашилась. Наверное, стоило бы переписать всю функцию.

                for (int i = diagram.ChannelCount - 2; i >= 0 ; i--)
                {
                    float channelBusyProb = channelBusyProbalitiesCache[diagram.ChannelCount - 2 - i];

                    cip[i] = channelBusyProb + totalProb;

                    totalProb += channelBusyProb;
                }

                return cip;
            }
        }

        /// <summary>
        /// Вероятность P того, что в очереди будет только n заявок,
        /// где P = arr[n - 1]
        /// </summary>
        public float[] QueueBusyProbality => queueBusyProbalityCache;

        /// <summary>
        /// Среднее количество заявок в очереди.
        /// </summary>
        public float AverageClientCountInQueue
        {
            get
            {
                float value = 0;

                var probs = queueBusyProbalityCache;

                for(int i = 0; i < diagram.QueueCapacity; i++)
                    value += (i + 1) * probs[i];

                return value;
            }
        }

        /// <summary>
        /// Среднее время ожидания заявки в очереди.
        /// </summary>
        public float AverageClientQueueWaitingTime => averageClientQueueWaitingTimeCache;

        /// <summary>
        /// Среднее время обслуживания заявки.
        /// </summary>
        public float AverageClientServiceTime => averageClientServiceTimeCache;

        /// <summary>
        /// Среднее время нахождения заявки в системе.
        /// </summary>
        public float SummaryAverageClientTime => averageClientQueueWaitingTimeCache + averageClientServiceTimeCache;

        /// <summary>
        /// Среднее количество заявок, находящихся в системе.
        /// </summary>
        public float AverageClientCount => (float)diagram.AverageClientCount;
    }
}
