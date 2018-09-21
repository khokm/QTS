namespace QTS.Core.Diagram
{
    /// <summary>
    /// Представляет контейнер для хранения параметров улучшения графика.
    /// </summary>

    public class GraphImprovementParamsData
    {
        /// <summary>
        /// Индекс используемого показателя в массиве исходных показателей.
        /// </summary>
        public int MetricIndex { get; }

        /// <summary>
        /// Количество экспериментов.
        /// </summary>
        public int ExperimentCount { get; }

        /// <summary>
        /// Создает новый экземпляр класса <see cref="GraphImprovementParamsData"/>.
        /// </summary>
        /// <param name="metricIndex">Индекс используемого показателя в массиве исходных показателей.</param>
        /// <param name="experimentCount">Количество экспериментов.</param>
        public GraphImprovementParamsData(int metricIndex, int experimentCount)
        {
            MetricIndex = metricIndex;
            ExperimentCount = experimentCount;
        }
    }
}
