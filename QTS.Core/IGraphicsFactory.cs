using System.Collections.Generic;

namespace QTS.Core
{
    /// <summary>
    /// Фабрика графических объектов.
    /// </summary>
    /// <typeparam name="T">Используемая реализация временной диаграммы.</typeparam>
    /// <typeparam name="G">Используемая реализация графика.</typeparam>
    public interface IGraphicsFactory<out T, out G> where T : InteractiveDiagram where G : IGraph
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
        G CreateEmptyGraph(string XAxis = "", string YAxis = "");


        /// <summary>
        /// Создает новый экземпяр графика и равномерно заполняет его значениями.
        /// </summary>
        /// <param name="minX">Начальное значение по X</param>
        /// <param name="yValues">Массив значений Y</param>
        /// <param name="XAxis">Название оси абцисс</param>
        /// <param name="YAxis">Название оси ординат</param>
        /// <returns></returns>
        G CreateEmptyGraph(int minX, IEnumerable<double> yValues, string XAxis = "", string YAxis = "");

    }
}
