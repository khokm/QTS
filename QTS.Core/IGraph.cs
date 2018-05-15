using System.Drawing;

namespace QTS.Core
{
    /// <summary>
    /// Интерфейс для объекта, который можно рассматривать как график.
    /// </summary>
    public interface IGraph
    {
        /// <summary>
        /// Название графика
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Начинает создание новой линии
        /// </summary>
        void BeginLine();

        /// <summary>
        /// Заканчивает создание линии
        /// </summary>
        void CompleteLine();

        /// <summary>
        /// Добавляет точку в текущую линию
        /// </summary>
        void AddPoint(double y, double x);

        /// <summary>
        /// Конвертирует график в PNG-изображение
        /// </summary>
        /// <returns></returns>
        Bitmap ExportToBitmap();
    }
}
