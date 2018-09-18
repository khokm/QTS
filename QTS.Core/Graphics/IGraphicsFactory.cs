using System.Collections.Generic;

namespace QTS.Core.Graphics
{
    /// <summary>
    /// Определяет функции фабрики графических объектов.
    /// </summary>
    /// <typeparam name="T">Используемая реализация интерактивной диаграммы.</typeparam>
    /// <typeparam name="G">Используемая реализация графика.</typeparam>
    public interface IGraphicsFactory<out T, out G> where T : InteractiveDiagram where G : IGraph
    {
        /// <summary>
        /// Создает новый экземпляр пустой диграммы.
        /// </summary>
        /// <param name="yLabels">Подписи на оси Y.</param>
        /// <returns>Экземпляр интерактивной диаграммы.</returns>
        T CreateEmptyDiagram(IEnumerable<string> yLabels);

        T CreateInteractiveGraph(string graphTitle);

        /// <summary>
        /// Создает новый экземпяр графика и равномерно заполняет его значениями из <paramref name="yValues"/> с шагом 1, начиная с <paramref name="startX"/>
        /// </summary>
        /// <param name="startX">Начальное значение по X</param>
        /// <param name="yValues">Массив значений по Y.</param>
        /// <param name="XAxis">Название оси абцисс</param>
        /// <param name="YAxis">Название оси ординат</param>
        /// <returns>Экземпляр графика.</returns>
        G CreateGraphByPoints(int startX, IEnumerable<double> yValues, string XAxis, string YAxis);

    }
}
