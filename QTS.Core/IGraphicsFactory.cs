namespace QTS.Core
{
    /// <summary>
    /// Фабрика графических объектов.
    /// </summary>
    /// <typeparam name="T">Используемая реализация временной диаграммы.</typeparam>
    /// <typeparam name="G">Используемая реализация графика.</typeparam>
    public interface IGraphicsFactory<out T, out G> where T : TimeDiagram where G : IGraph
    {
        /// <summary>
        /// Создает новый экземпляр пустой диграммы.
        /// </summary>
        /// <param name="channelCount">Количество каналов. Используется для вычисления количества горизонтальных линий диаграммы.</param>
        /// <param name="channelCount">Количество мест обслуживания. Используется для вычисления количества горизонтальных линий диаграммы.</param>
        /// <returns></returns>
        T CreateEmptyDiagram(int channelCount, int queueCapacity);

        /// <summary>
        /// Создает новый экземпляр пустого графика.
        /// </summary>
        /// <returns></returns>
        G CreateEmptyGraph();
    }
}
