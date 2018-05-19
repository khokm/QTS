using System;

namespace QTS.Core.Graphics
{
    /// <summary>
    /// Интерфейс для объекта, чьим отображением можно управлять.
    /// </summary>
    interface IDiagramViewController
    {
        /// <summary>
        /// Переход к началу диграммы (отображение только первой линии)
        /// </summary>
        void GoToStart();

        /// <summary>
        /// Переход в конец диграммы (полное отображение всех линий)
        /// </summary>
        void GoToEnd();

        /// <summary>
        /// Включение отображения следующей линии
        /// </summary>
        void StepForward();

        /// <summary>
        /// Отключение отображения следующей линии
        /// </summary>
        void StepBack();

        /// <summary>
        /// Показывать все отображаемые линии (или только одну)?
        /// </summary>
        bool ShowPreviousLines { get; set; }

        /// <summary>
        /// Вызывается при изменении отображения диаграммы
        /// </summary>
        event Action ViewUpdated;
    }
}
