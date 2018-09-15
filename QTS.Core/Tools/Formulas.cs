using System.Linq;

namespace QTS.Core.Tools
{
    static class Formulas
    {

        /// <summary>
        /// Количество показателей.
        /// </summary>
        public static int TotalMetricsCount(DiagramData diagram) => 9 + diagram.ChannelCount * 2 + diagram.QueueCapacity;

        /// <summary>
        /// Вероятность обслуживания.
        /// </summary>
        public static float ServedProbality(DiagramData diagram) => (float)diagram.ServedClientCount / diagram.SummaryClientCount;

        /// <summary>
        /// Вероятность отказа.
        /// </summary>
        public static float RefuseProbality(DiagramData diagram) => (float)diagram.LostClientCount / diagram.SummaryClientCount;

        /// <summary>
        /// Пропускная способность системы.
        /// </summary>
        public static float SystemThroughput(DiagramData diagram) => (float)(diagram.ServedClientCount / diagram.SystemWorkTime);

        /// <summary>
        /// Вероятность P занятости только channelCount каналов.
        /// </summary>
        public static float ChannelBusyProbality(DiagramData diagram, int channelCount) => 
            (channelCount < 0 || channelCount > diagram.ChannelCount) ? 0 : (float)(diagram.ChannelBusyTimes[channelCount] / diagram.SystemWorkTime);

        /// <summary>
        /// Среднее количество занятых каналов.
        /// </summary>
        public static float AverageBusyChannelCount(DiagramData diagram) => (float)diagram.ChannelBusyTimes.Select((value, index) => index * (value / diagram.SystemWorkTime)).Sum();

        /// <summary>
        /// Вероятность P простоя хотя бы channelCount каналов.
        /// </summary>
        public static float ChannelIdleProbality(DiagramData diagram, int channelCount) =>
            (channelCount < 0 || channelCount > diagram.ChannelCount) ? 0 : (float)(diagram.ChannelBusyTimes.Reverse().Skip(channelCount).Sum() / diagram.SystemWorkTime);
       
        /// <summary>
        /// Вероятность P того, что в очереди будет только n заявок.
        /// </summary>
        public static float QueueBusyProbality(DiagramData diagram, int clientCount) =>
             (clientCount < 1 || clientCount > diagram.QueueCapacity) ? 0 : (float)(diagram.QueueBusyTimes[clientCount - 1] / diagram.SystemWorkTime);

        /// <summary>
        /// Среднее количество заявок в очереди.
        /// </summary>
        public static float AverageClientCountInQueue(DiagramData diagram) => (float)diagram.QueueBusyTimes.Select((value, index) => (index + 1) * (value / diagram.SystemWorkTime)).Sum();

        /// <summary>
        /// Среднее время ожидания заявки в очереди.
        /// </summary>
        public static float AverageClientQueueWaitingTime(DiagramData diagram) => diagram.QueuedClientCount == 0 ? 0 : (float)(diagram.QueueBusyTimes.Sum() / diagram.QueuedClientCount);

        /// <summary>
        /// Среднее время обслуживания заявки.
        /// </summary>
        public static float AverageClientServiceTime(DiagramData diagram) => diagram.ServedClientCount == 0 ? 0 : (float)diagram.SummaryServiceTime / diagram.ServedClientCount;

        /// <summary>
        /// Среднее время нахождения заявки в системе.
        /// </summary>
        public static float SummaryAverageClientTime(DiagramData diagram) => AverageClientQueueWaitingTime(diagram) + AverageClientServiceTime(diagram);

        /// <summary>
        /// Среднее количество заявок, находящихся в системе.
        /// </summary>
        public static float AverageClientCount(DiagramData diagram) => (float)diagram.AverageClientCount;
    }
}