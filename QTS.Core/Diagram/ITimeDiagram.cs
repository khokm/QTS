namespace QTS.Core.Diagram
{
    /// <summary>
    /// Интерфейс, для объекта, который можно заполнять, внося пути заявок.
    /// </summary>
    interface ITimeDiagram
    {
        /// <summary>
        /// Добавляет точку на линии "Заявки" и начинает путь заявки
        /// </summary>
        /// <param name="arrivalTime">Время появления заявки</param>
        void PushStartPoint(double arrivalTime);

        /// <summary>
        /// Добавляет линию заявки, находящейся на обслуживании
        /// </summary>
        /// <param name="channelIndex">Номер места обслуживания</param>
        /// <param name="start">Время начала обслуживания</param>
        /// <param name="end">Время окончания обслуживания</param>
        void PushChannelLine(int channelIndex, double start, double end);

        /// <summary>
        /// Увеличивает количество заявок, попавших в очередь, на 1
        /// </summary>
        void IncrementQueueClientCount();

        /// <summary>
        /// Добавляет линию заявки, находящейся на указанном стояночном месте
        /// </summary>
        /// <param name="queuePlaceIndex">Номер стояночного места</param>
        /// <param name="start">Время начала нахождения на стояночном месте</param>
        /// <param name="end">Время окончания нахождения на стояночном месте</param>
        void PushQueueLine(int queuePlaceIndex, double start, double end);

        /// <summary>
        /// Добавляет точку на линии "Обслужено" и заканчивает линию заявки.
        /// </summary>
        /// <param name="departureTime">Время отъезда заявки</param>
        void PushServedPoint(double departureTime);

        /// <summary>
        /// Добавляет точку на линии "Отказ" и заканчивает линию заявки.
        /// </summary>
        /// <param name="departureTime">Время отъезда</param>
        void PushRefusedPoint(double departureTime);

        /// <summary>
        /// Заканчивает создание диаграммы и вызывает ее постообработку (добавление надписей и т.д.)
        /// </summary>
        void FinishDiagram();

        /// <summary>
        /// Завершено ли создание диаграммы?
        /// </summary>
        bool Completed { get; }
    }
}
