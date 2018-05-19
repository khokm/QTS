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
        /// <param name="realRndValue">Значение гсч.</param>
        /// <param name="rndValue">Промежуток между заявками.</param>
        void PushStartPoint(double arrivalTime, double realRndValue, double rndValue);

        /// <summary>
        /// Добавляет линию заявки, находящейся на обслуживании
        /// </summary>
        /// <param name="channelIndex">Номер места обслуживания</param>
        /// <param name="serviceTime">Время обслуживания.</param>
        /// <param name="realRndValue">Значение гсч.</param>
        void PushChannelLine(int channelIndex, double serviceTime, double realRndValue);

        /// <summary>
        /// Добавляет линию заявки, находящейся на указанном стояночном месте
        /// </summary>
        /// <param name="queuePlaceIndex">Номер стояночного места</param>
        /// <param name="queueTime">Время нахождения на стояночном месте.</param>
        void PushQueueLine(int queuePlaceIndex, double queueTime);

        /// <summary>
        /// Добавляет точку на линии "Обслужено" и заканчивает линию заявки.
        /// </summary>
        void PushServedPoint();

        /// <summary>
        /// Добавляет точку на линии "Отказ" и заканчивает линию заявки.
        /// </summary>
        void PushRefusedPoint();

        /// <summary>
        /// Заканчивает создание диаграммы и вызывает ее постообработку (добавление надписей и т.д.)
        /// </summary>
        void FinishDiagram();

        /// <summary>
        /// Завершено ли создание диаграммы?
        /// </summary>
        bool Finished { get; }
    }
}
