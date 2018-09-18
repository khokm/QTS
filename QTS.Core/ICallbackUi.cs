using QTS.Core.Graphics;
using QTS.Core.Diagram;
using System.Collections.Generic;

namespace QTS.Core
{
    /// <summary>
    /// Пользовательский интерфейс (например, CLI или GUI) для обратной связи.
    /// </summary>
    /// <typeparam name="T">Используемая реализация временной диаграммы.</typeparam>
    public interface ICallbackUi
    {
        /// <summary>
        /// Перерисовывает диаграмму, отбражаемую в главном окне.
        /// </summary>
        void InvalidateDiagramView();

        InteractiveDiagram InteractiveDiagram { get; set; }

        /// <summary>
        /// Получает параметры для построения диаграммы.
        /// </summary>
        /// <returns></returns>
        ParametersContainer GetDiagramParameters();

        /// <summary>
        /// Получает параметры градиента для синтеза СМО.
        /// </summary>
        /// <returns></returns>
        QueuePlaceGradientData GetQueuePlaceGradientData();

        GraphImprovementParamsData GetGraphImprovementParams(string[] metricNames);
        /// <summary>
        /// Запрашивает подтверждение/отрицание пользователя о каких-либо действиях программы.
        /// </summary>
        /// <param name="title">Совершаемое действие.</param>
        /// <param name="message">Описание совершаемого действия.</param>
        /// <returns>Ответ пользователя</returns>
        bool YesNoDialog(string title, string message);

        /// <summary>
        /// Просит пользователя выбрать папку для сохранения данных.
        /// </summary>
        /// <param name="description">Описание сохраняемого содержимого.</param>
        /// <param name="defaultPath">Путь к папке, выбранный по умолчанию.</param>
        /// <returns>Путь к выбранной папке.</returns>
        string GetFolderPath(string description, string defaultPath="");

        /// <summary>
        /// Открывает папку в новом окне.
        /// </summary>
        /// <param name="path">Путь к папке.</param>
        void StartExplorer(string path);
        /// <summary>
        /// Показывает пользователю текстовое окно.
        /// </summary>
        /// <param name="title">Заголовок окна</param>
        /// <param name="text">Содержимое окна</param>
        void ShowTextWindow(string title, string text);

        /// <summary>
        /// Уведомляет пользователя о совершаемых недопустимых действиях.
        /// </summary>
        /// <param name="title">Заголовок окна</param>
        /// <param name="message">Содержимое окна</param>
        void ShowError(string title, string message);

        /// <summary>
        /// Показывает пользователю предупреждение.
        /// </summary>
        /// <param name="title">Заголовок окна</param>
        /// <param name="message">Содержимое окна</param>
        void ShowWarning(string title, string message);

        void CreateTextFile(string path, string text);

        void ShowProgressWindow(string description);

        void CloseProgressWindow();

    }
}
