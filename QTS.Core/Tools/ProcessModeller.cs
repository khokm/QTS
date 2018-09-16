using QTS.Core.Graphics;
using QTS.Core.Diagram;
using System.Collections.Generic;

namespace QTS.Core.Tools
{
    /// <summary>
    /// Моделирует процесс СМО.
    /// </summary>
    class ProcessModeller
    {
        ParametersContainer Parameters { get; }

        double[] channelIdleTimes { get; }
        double[] queueIdleTimes { get; }

        public ProcessModeller(ParametersContainer parameters)
        {
            Parameters = parameters;
            channelIdleTimes = new double[Parameters.ChannelCount];
            queueIdleTimes = new double[parameters.QueueCapacity];

            for (int i = 0; i < Parameters.ChannelCount; i++)
                channelIdleTimes[i] = -1;

            for (int i = 0; i < Parameters.QueueCapacity; i++)
                queueIdleTimes[i] = -1;
        }

        /// <summary>
        /// Возвращает индекс доступного свободного места в указанный момент времени. Если такого нет, возвращает -1.
        /// </summary>
        /// <param name="arrivalTime">Момент времени</param>
        /// <returns>Индекс свободного стояночного места</returns>
        int GetQueuePlaceIndex(double arrivalTime)
        {
            for (int i = 0; i < Parameters.QueueCapacity; i++)
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
            if (Parameters.ChannelCount == 0)
                return -1;

            int minIndex = 0;

            double minimumValue = channelIdleTimes[0];

            for (int i = 0; i < Parameters.ChannelCount; i++)
            {
                if (Parameters.PreferFirstChannel && channelIdleTimes[i] < arrivalTime)
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
        void CreateChannelLine(int channelIndex, double arrivalTime, RandomGenerator rnd, TimeDiagram timeDiagram)
        {
            double realRndValue;
            double clientSerivceTime = rnd.Next(Parameters.ChannelsIntencites[channelIndex], out realRndValue);

            double departureTime = arrivalTime + clientSerivceTime;

            channelIdleTimes[channelIndex] = departureTime;

            timeDiagram.PushChannelLine(channelIndex, clientSerivceTime, realRndValue);
            timeDiagram.PushServedPoint();
        }

        /// <summary>
        /// Добавляет в систему новую заявку и строит ее путь.
        /// </summary>
        /// <param name="arrivalTime">Время прибытия заявки</param>
        /// <param name="rnd">Импользуемый ГСЧ</param>
        void PushClient(double arrivalTime, RandomGenerator rnd, double realRndValue, double interval, TimeDiagram timeDiagram)
        {
            timeDiagram.PushStartPoint(arrivalTime, realRndValue, interval);

            int usingChannel = GetNextPossibleChannel(arrivalTime);

            if (usingChannel == -1)
            {
                timeDiagram.PushRefusedPoint();
                return;
            }

            if (channelIdleTimes[usingChannel] < arrivalTime)
            {
                CreateChannelLine(usingChannel, arrivalTime, rnd, timeDiagram);
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

            CreateChannelLine(usingChannel, queueIdleTimes[0], rnd, timeDiagram);
        }

        void FillDiagram(TimeDiagram timeDiagram)
        {
            double step = 0.5 / Parameters.ThreadIntencity;


            RandomGenerator rnd = new RandomGenerator(Parameters.MinRandomValue);

            double realTime = 0;
            double fixedTime = 0;

            for (int i = 0; !Parameters.HasClientLimit || i < Parameters.ClientLimit; i++)
            {
                double realRndValue;
                double rndValue = rnd.Next(Parameters.ThreadIntencity, out realRndValue);
                realTime += rndValue;

                while (fixedTime < realTime)
                {
                    timeDiagram.CheckClientCountAtTime(fixedTime);
                    fixedTime += step;
                }

                if (Parameters.HasTimeLimit && realTime > Parameters.TimeLimit)
                    break;

                PushClient(realTime, rnd, realRndValue, rndValue, timeDiagram);
            }

            timeDiagram.FinishDiagram();
        }

        public TimeDiagram CreateDiagram()
        {
            var diagram = new TimeDiagram(Parameters.ChannelCount, Parameters.QueueCapacity);

            FillDiagram(diagram);

            return diagram;
        }

        public TimeDiagram CreateDiagram<T>(IGraphicsFactory<T, IGraph> factory, out InteractiveDiagram interactiveDiagram) where T : InteractiveDiagram
        {
            List<string> labels = new List<string>(Parameters.ChannelCount + Parameters.QueueCapacity + 3);

            labels.Add("Заявки");

            for (int i = 0; i < Parameters.ChannelCount; i++)
                labels.Add($"Канал { Parameters.ChannelCount - i }");

            for (int i = 0; i < Parameters.QueueCapacity; i++)
                labels.Add($"Место { Parameters.QueueCapacity - i }");

            labels.Add("Обслужено");
            labels.Add("Отказ");

            T idg = factory.CreateEmptyDiagram(labels);

            TimeDiagram diagram = new TimeDiagram(Parameters.ChannelCount, Parameters.QueueCapacity, idg);

            FillDiagram(diagram);

            interactiveDiagram = idg;

            return diagram;
        }
    }
}