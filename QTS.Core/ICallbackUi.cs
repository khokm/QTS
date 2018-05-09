namespace QTS.Core
{
    /// <summary>
    /// Пользовательский интерфейс (например, CLI или GUI) для обратной связи.
    /// </summary>
    public interface ICallbackUi
    {
        /// <summary>
        /// Перерисовывает диаграмму.
        /// </summary>
        void InvalidateDiagramView();

        /// <summary>
        /// Устанавливает текущую отображаемую диаграмму.
        /// </summary>
        /// <param name="diagram">Отображаемая диаграмма</param>
        void SetDiagramView(TimeDiagram diagram);

        /// <summary>
        /// Выключает отображение диаграммы.
        /// </summary>
        void RemoveDiagramView();

        /// <summary>
        /// Вызывает диалог "Да/Нет".
        /// </summary>
        /// <param name="title">Заголовок окна</param>
        /// <param name="message">Содержимое окна</param>
        /// <returns>Ответ пользователя</returns>
        bool YesNoDialog(string title, string message);

        /// <summary>
        /// Создает новый экземпляр диграммы.
        /// </summary>
        /// <param name="channelCount"></param>
        /// <param name="queueCapacity"></param>
        /// <returns></returns>
        TimeDiagram CreateNewDiagram(int channelCount, int queueCapacity);

        /// <summary>
        /// Создает новый экземпляр графика.
        /// </summary>
        /// <returns></returns>
        IGraph CreateGraph();

        /// <summary>
        /// Возвращает путь к папке для сохранения графиков.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        string GetImagePathFolder(string description);

        /// <summary>
        /// Создает текстовое окно.
        /// </summary>
        /// <param name="title">Заголовок окна</param>
        /// <param name="text">Содержимое окна</param>
        void ShowTextWindow(string title, string text);

        /// <summary>
        /// Уведомляет пользователя о недопустимых действиях.
        /// </summary>
        /// <param name="title">Заголовок окна</param>
        /// <param name="message">Содержимое окна</param>
        void ShowError(string title, string message);
    }
}
