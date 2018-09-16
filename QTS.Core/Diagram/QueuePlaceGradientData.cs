namespace QTS.Core.Diagram
{
    /// <summary>
    /// Представляет контейнер для хранения параметров синтеза СМО по количеству мест в очереди (КМО).
    /// </summary>
    public class QueuePlaceGradientData
    {
        /// <summary>
        /// Минимальное количество мест в очереди.
        /// </summary>
        public int MinQueueCapacity { get; }
        /// <summary>
        /// Максимальное количество мест в очереди.
        /// </summary>
        public int MaxQueueCapacity { get; }

        /// <summary>
        /// Создает новый экземпляр класса <see cref="QueuePlaceGradientData"/> с указанными параметрами синтеза СМО.
        /// </summary>
        /// <param name="minPlaceCount">Минимальное количество мест в очереди.</param>
        /// <param name="maxPlaceCount">Максимальное количество мест в очереди.</param>
        public QueuePlaceGradientData(int minPlaceCount, int maxPlaceCount)
        {
            MinQueueCapacity = minPlaceCount;
            MaxQueueCapacity = maxPlaceCount;
        }
    }
}
