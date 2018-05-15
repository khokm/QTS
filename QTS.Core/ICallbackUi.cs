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

        /// <summary>
        /// Устанавливает диаграмму, которая должна быть отображена в главном окне.
        /// </summary>
        /// <param name="diagram">Отображаемая диаграмма</param>
        void SetDiagramView(TimeDiagram diagram);

        /// <summary>
        /// Выключает отображение диаграммы в главном окне.
        /// </summary>
        void RemoveDiagramView();

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
    }
}
