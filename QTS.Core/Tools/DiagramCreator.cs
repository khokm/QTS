using QTS.Core.Diagram;
using QTS.Core.Helpers;
using System;

namespace QTS.Core.Tools
{
    /// <summary>
    /// Прокладывает пути заявок на диграмме.
    /// </summary>
    class DiagramCreator
    {
        int[] channelsIntencity { get; }

        int channelCount => channelsIntencity.Length;
        int queueCount => queueIdleTimes.Length;

        double[] channelIdleTimes { get; }
        double[] queueIdleTimes { get; }

        ITimeDiagram timeDiagram { get; }

        bool preferFirstChannel { get; }

        /// <summary>
        /// Создает новый заполнитель диаграммы.
        /// </summary>
        /// <param name="queueCapacity">Количество мест в очереди</param>
        /// <param name="channelsIntencity">Массив пропускных способность каналов</param>
        /// <param name="preferFirstChannel">Предпочитать первый канал?</param>
        /// <param name="diagram">Заполняемая диаграмма</param>
        public DiagramCreator(int queueCapacity, int[] channelsIntencity, bool preferFirstChannel, ITimeDiagram diagram)
        {
            if (diagram.Finished)
                throw new Exception("DiagramCreator::Диаграмма уже построена!");

            this.preferFirstChannel = preferFirstChannel;
            this.channelsIntencity = channelsIntencity;

            channelIdleTimes = new double[channelCount];
            queueIdleTimes = new double[queueCapacity];

            for (int i = 0; i < channelCount; i++)
                channelIdleTimes[i] = -1;

            for (int i = 0; i < queueCapacity; i++)
                queueIdleTimes[i] = -1;

            timeDiagram = diagram;
        }

        /// <summary>
        /// Возвращает индекс доступного свободного места в указанный момент времени. Если такого нет, возвращает -1.
        /// </summary>
        /// <param name="arrivalTime">Момент времени</param>
        /// <returns>Индекс свободного стояночного места</returns>
        int GetQueuePlaceIndex(double arrivalTime)
        {
            for (int i = 0; i < queueCount; i++)
            {
                if (queueIdleTimes[i] < arrivalTime)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Возвращает индекс доступного места обслуживания в указанный момент времени. Если такого нет, возвращает -1.
        /// </summary>
        /// <param name="arrivalTime">Момент времени</param>
        /// <returns>Индекс свободного места обслуживания</returns>
        int GetNextPossibleChannel(double arrivalTime)
        {
            if (channelCount == 0)
                return -1;

            int minIndex = 0;

            double minimumValue = channelIdleTimes[0];

            for (int i = 0; i < channelCount; i++)
            {
                if (preferFirstChannel && channelIdleTimes[i] < arrivalTime)
                    return i;

                if (channelIdleTimes[i] < minimumValue)
                {
                    minimumValue = channelIdleTimes[i];
                    minIndex = i;
                }
            }

            return minIndex;
        }

        /// <summary>
        /// Создает новую линию в пути заявки, обслуженной на указанном месте обслуживания.
        /// </summary>
        /// <param name="channelIndex">Индекс используемого места обслуживания.</param>
        /// <param name="arrivalTime">Время начала обслуживания.</param>
        /// <param name="rnd">Используемый ГСЧ</param>
        void CreateChannelLine(int channelIndex, double arrivalTime, RandomGenerator rnd)
        {
            double clientSerivceTime = rnd.Next(channelsIntencity[channelIndex]);

            double departureTime = arrivalTime + clientSerivceTime;

            channelIdleTimes[channelIndex] = departureTime;

            timeDiagram.PushChannelLine(channelIndex, clientSerivceTime);
            timeDiagram.PushServedPoint();
        }

        /// <summary>
        /// Добавляет в систему новую заявку и строит ее путь.
        /// </summary>
        /// <param name="arrivalTime">Время прибытия заявки</param>
        /// <param name="rnd">Импользуемый ГСЧ</param>
        public void PushClient(double arrivalTime, RandomGenerator rnd)
        {
            timeDiagram.PushStartPoint(arrivalTime);

            int usingChannel = GetNextPossibleChannel(arrivalTime);

            if (usingChannel == -1)
            {
                timeDiagram.PushRefusedPoint();
                return;
            }

            if (channelIdleTimes[usingChannel] < arrivalTime)
            {
                CreateChannelLine(usingChannel, arrivalTime, rnd);
                return;
            }

            int queuePlaceIndex = GetQueuePlaceIndex(arrivalTime);

            if (queuePlaceIndex == -1)
            {
                timeDiagram.PushRefusedPoint();
                return;
            }

            double queuedStart = arrivalTime;

            for (int i = queuePlaceIndex; i >= 0; i--)
            {
                double queuedEndTime = i != 0 ? queueIdleTimes[i - 1] : channelIdleTimes[usingChannel];

                queueIdleTimes[i] = queuedEndTime;

                timeDiagram.PushQueueLine(i, queuedEndTime - queuedStart);
                queuedStart = queuedEndTime;
            }

            CreateChannelLine(usingChannel, queueIdleTimes[0], rnd);
        }
    }
}