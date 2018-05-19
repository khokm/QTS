namespace QTS.Core.Diagram
{
    /// <summary>
    /// Интерфейс для объекта, который возможно анализировать.
    /// Какие-либо отбраковки и средние значения недопустимы - 
    /// здесь содержатся лишь точные данные.
    /// </summary>
    interface IDiagramData
    {
        /// <summary>
        /// Параметры диаграммы.
        /// </summary>
        ParametersContainer ReadonlyParameters { get; }

        /// <summary>
        /// Количество каналов (стояночных мест)
        /// </summary>
        int ChannelCount { get; }

        /// <summary>
        /// Количество мест в очереди
        /// </summary>
        int QueueCapacity { get; }

        /// <summary>
        /// Суммарное количество всех заявок (обслуженных и утерянных)
        /// </summary>
        int SummaryClientCount { get; }

        /// <summary>
        /// Количество обслуженных заявок
        /// </summary>
        int ServedClientCount { get; }

        /// <summary>
        /// Количество утерянных (не обслуженных) заявок
        /// </summary>
        int LostClientCount { get; }

        /// <summary>
        /// Время появления первой заявки
        /// </summary>
        double FirstClientArrivalTime { get; }

        /// <summary>
        /// Время выбывания из системы последней заявки
        /// </summary>
        double LastClientDepartureTime { get; }

        /// <summary>
        /// Полное время работы системы
        /// </summary>
        double SystemWorkTime { get; }

        /// <summary>
        /// Времена занятости каждой из стоянок
        /// </summary>
        double[] QueueBusyTimes { get; }

        /// <summary>
        /// Суммарное время обслуживания заявок
        /// </summary>
        double SummaryServiceTime { get; }

        /// <summary>
        /// Количество заявок, побывавших в очереди
        /// </summary>
        int QueuedClientCount { get; }

        /// <summary>
        /// Возвращает время одновременной работы n и только n каналов.
        /// </summary>
        /// <param name="channelCount">Количество каналов</param>
        /// <returns>Время</returns>
        double GetChannelsIntersectionLength(int channelCount);

        /// <summary>
        /// Возвращает количество заявок, находящееся в системе в указанный момент времени.
        /// </summary>
        /// <param name="point">Момент времени, в который нужно зарегистрировать количество заявок</param>
        /// <param name="step">'Ширина' момента времени</param>
        /// <returns>Количество заявок</returns>
        int GetClientCountAtTime(double point, double step);
    }
}