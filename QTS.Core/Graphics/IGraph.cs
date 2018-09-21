using System.Collections.Generic;

namespace QTS.Core.Graphics
{
    /// <summary>
    /// Определяет функции графика.
    /// </summary>
    public interface IGraph
    {
        /// <summary>
        /// Название графика.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Начинает новую линию.
        /// </summary>
        void BeginLine();

        /// <summary>
        /// Добавляет аннотацию в указанных координатах.
        /// </summary>
        /// <param name="y">Позиция аннотации по оси Y.</param>
        /// <param name="x">Позиция аннотации по оси X.</param>
        /// <param name="annotation">Текст аннотации.</param>
        /// <param name="atTop"></param>
        void AddAnnotation(double y, double x, string annotation, bool atTop);

        /// <summary>
        /// Добавляет описание к создаваемой линии.
        /// </summary>
        /// <param name="metadata">Текст описания.</param>
        void AddLineMetadata(string metadata);

        /// <summary>
        /// Заканчивает создание линии.
        /// </summary>
        /// <param name="randomColor">Использовать случайный цвет для линии?</param>
        void CompleteLine(bool randomColor = true);

        /// <summary>
        /// Добавляет точку на линию.
        /// </summary>
        /// <param name="y">Позиция точки по оси Y</param>
        /// <param name="x">Позиция точки по оси x</param>
        void AddPoint(double y, double x);

        /// <summary>
        /// Создает новую линию, равномерно распределяя значения <paramref name="yValues"/> по оси X с шагом 1, начиная с <paramref name="startX"/>
        /// </summary>
        /// <param name="yValues">Набор значений по оси Y.</param>
        /// <param name="startX">Позиция по оси X, с которой начинается линия.</param>
        void AddPoints(IEnumerable<double> yValues, double startX);

        /// <summary>
        /// Сохраняет график в файл как png изображение.
        /// </summary>
        /// <param name="betterHieghts">Автоматически подкорректировать отображение графика?</param>
        /// <param name="path">Путь к файлу.</param>
        void ExportToBitmap(bool betterHieghts, string path);
    }
}
