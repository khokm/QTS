namespace QTS.Core.Diagram
{
    /// <summary>
    /// Определяет статистические данные анализируемого объекта.
    /// </summary>
    interface IAnalyzable
    {
        /// <summary>
        /// Параметр "Количество каналов в системе".
        /// </summary>
        int ChannelCount { get; }

        /// <summary>
        /// Параметр "Количество мест в очереди".
        /// </summary>
        int QueueCapacity { get; }

        /// <summary>
        /// Время работы системы.
        /// </summary>
        double SystemWorkTime { get; }

        /// <summary>
        /// Общее время обслуживания.
        /// </summary>
        double SummaryServiceTime { get; }

        /// <summary>
        /// Количество обслуженных заявок.
        /// </summary>
        int ServedClientCount { get;}

        /// <summary>
        /// Количество отказанных заявок.
        /// </summary>
        int LostClientCount { get; }

        /// <summary>
        /// Общее количество заявок.
        /// </summary>
        int SummaryClientCount { get; }

        /// <summary>
        /// Количество заявок, побывавших в очереди.
        /// </summary>
        int QueuedClientCount { get; }
        
        /// <summary>
        /// Среднее количество заявок в системе.
        /// </summary>
        double AverageClientCount { get; }

        /// <summary>
        /// Время занятости (P) канала, где P = <see cref="ChannelBusyTimes"/>[номер канала - 1].
        /// </summary>
        double[] ChannelBusyTimes { get; }

        /// <summary>
        /// Время занятости (P) места в очереди, где P = <see cref="QueueBusyTimes"/>[номер канала - 1].
        /// </summary>
        double[] QueueBusyTimes { get; }
    }
}
