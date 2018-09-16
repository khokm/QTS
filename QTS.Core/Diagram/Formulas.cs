using System.Linq;

namespace QTS.Core.Diagram
{
    static class Formulas
    {
        /// <summary>
        /// Общее количество заявок.
        /// </summary>
        public static float SummaryClientCount(IAnalyzable diagram) => diagram.SummaryClientCount;

        /// <summary>
        /// Количество обслуженных заявок.
        /// </summary>
        public static float ServedClientCount(IAnalyzable diagram) => diagram.ServedClientCount;

        /// <summary>
        /// Количество отказанных заявок.
        /// </summary>
        public static float LostClientCount(IAnalyzable diagram) => diagram.LostClientCount;

        /// <summary>
        /// Время работы системы.
        /// </summary>
        public static float SystemWorkTime(IAnalyzable diagram) => (float)diagram.SystemWorkTime;

        /// <summary>
        /// Количество показателей.
        /// </summary>
        public static int TotalMetricsCount(IAnalyzable diagram) => 9 + diagram.ChannelCount * 2 + diagram.QueueCapacity;

        /// <summary>
        /// Вероятность обслуживания.
        /// </summary>
        public static float ServedProbality(IAnalyzable diagram) => (float)diagram.ServedClientCount / diagram.SummaryClientCount;

        /// <summary>
        /// Вероятность отказа.
        /// </summary>
        public static float RefuseProbality(IAnalyzable diagram) => (float)diagram.LostClientCount / diagram.SummaryClientCount;

        /// <summary>
        /// Пропускная способность системы.
        /// </summary>
        public static float SystemThroughput(IAnalyzable diagram) => (float)(diagram.ServedClientCount / diagram.SystemWorkTime);

        /// <summary>
        /// Вероятность занятости только <paramref name="channelCount"/> каналов.
        /// 0, если <paramref name="channelCount"/> больше кол-ва каналов;
        /// </summary>
        public static float ChannelBusyProbality(IAnalyzable diagram, int channelCount) => 
            (channelCount > diagram.ChannelCount) ? 0 : (float)(diagram.ChannelBusyTimes[channelCount] / diagram.SystemWorkTime);

        /// <summary>
        /// Среднее количество занятых каналов.
        /// </summary>
        public static float AverageBusyChannelCount(IAnalyzable diagram) => (float)diagram.ChannelBusyTimes.Select((value, index) => index * (value / diagram.SystemWorkTime)).Sum();

        /// <summary>
        /// Вероятность простоя хотя бы <paramref name="channelCount"/> каналов.
        /// 0, если <paramref name="channelCount"/> больше кол-ва каналов;
        /// </summary>
        public static float ChannelIdleProbality(IAnalyzable diagram, int channelCount) =>
            (channelCount > diagram.ChannelCount) ? 0 : (float)(diagram.ChannelBusyTimes.Reverse().Skip(channelCount).Sum() / diagram.SystemWorkTime);

        /// <summary>
        /// Вероятность того, что в очереди будет только <paramref name="clientCount"/> заявок.
        /// 0, если <paramref name="clientCount"/> больше кол-ва каналов;
        /// </summary>
        public static float QueueBusyProbality(IAnalyzable diagram, int clientCount) =>
             (clientCount > diagram.QueueCapacity) ? 0 : (float)(diagram.QueueBusyTimes[clientCount - 1] / diagram.SystemWorkTime);

        /// <summary>
        /// Среднее количество заявок в очереди.
        /// </summary>
        public static float AverageClientCountInQueue(IAnalyzable diagram) => (float)diagram.QueueBusyTimes.Select((value, index) => (index + 1) * (value / diagram.SystemWorkTime)).Sum();

        /// <summary>
        /// Среднее время ожидания заявки в очереди.
        /// 0, если нет заявок, ожидавших в очереди.
        /// </summary>
        public static float AverageClientQueueWaitingTime(IAnalyzable diagram) => diagram.QueuedClientCount == 0 ? 0 : (float)(diagram.QueueBusyTimes.Sum() / diagram.QueuedClientCount);

        /// <summary>
        /// Среднее время обслуживания заявки.
        /// 0, если нет обсуженных заявок.
        /// </summary>
        public static float AverageClientServiceTime(IAnalyzable diagram) => diagram.ServedClientCount == 0 ? 0 : (float)diagram.SummaryServiceTime / diagram.ServedClientCount;

        /// <summary>
        /// Среднее время нахождения заявки в системе.
        /// </summary>
        public static float SummaryAverageClientTime(IAnalyzable diagram) => AverageClientQueueWaitingTime(diagram) + AverageClientServiceTime(diagram);

        /// <summary>
        /// Среднее количество заявок, находящихся в системе.
        /// </summary>
        public static float AverageClientCount(IAnalyzable diagram) => (float)diagram.AverageClientCount;
    }
}