using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QTS.Core
{

    /// <summary>
    /// Фабрика графических объектов.
    /// </summary>
    /// <typeparam name="T">Используемая реализация временной диаграммы.</typeparam>
    public interface IGraphicsFactory<T> where T : TimeDiagram
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
        IGraph CreateEmptyGraph();
    }
}
