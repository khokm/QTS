using QTS.Core.Diagram;
using QTS.Core.Helpers;

namespace QTS.Core.Tools
{
    /// <summary>
    /// Просто удобные функции.
    /// </summary>
    static class Solver
    {
        /// <summary>
        /// Заполняет диаграмму значениями (моделирует процесс работы системы)
        /// </summary>
        /// <param name="parameters">Параметры системы</param>
        /// <param name="timeDiagram">Заполняемая временная диаграмма</param>
        public static void FillDiagram(ParametersContainer parameters, ITimeDiagram timeDiagram)
        {
            RandomGenerator rnd = new RandomGenerator(parameters.MinRandomValue);
            double workTime = 0;

            DiagramCreator model = new DiagramCreator(parameters.QueueCapacity, parameters.ChannelsIntencites, parameters.PreferFirstChannel, timeDiagram);

            for (int i = 0; !parameters.HasClientLimit || i < parameters.ClientLimit; i++)
            {
                double realRndValue;
                double rndValue = rnd.Next(parameters.ThreadIntencity, out realRndValue);
                workTime += rndValue;

                if (parameters.HasTimeLimit && workTime > parameters.TimeLimit)
                    break;

                model.PushClient(workTime, rnd, realRndValue, rndValue);
            }

            timeDiagram.FinishDiagram();
        }

        /// <summary>
        /// Создает новые точки на каждом из графиков показателей. Используется для синтеза СМО.
        /// </summary>
        /// <param name="parameters">Параметры построения графика.</param>
        /// <param name="graphs">Массив графиков показателей.</param>
        /// <param name="queueMaximumCapacity">Максимальное количество мест в очереди из всех анализируемых систем.</param>
        /// <param name="currentX">Текущая координата графика по X, куда будет внесена точка.</param>
        public static void AddPointsToGraph(TimeDiagram timeDiagram, ParametersContainer parameters, IGraph[] graphs, int queueMaximumCapacity, double currentX)
        {
            var info = new DiagramAnalyzer(timeDiagram);

            int k = 0;

            graphs[k++].AddPoint(info.SystemThroughput, currentX);
            graphs[k++].AddPoint(info.ServedProbality, currentX);
            graphs[k++].AddPoint(info.RefuseProbality, currentX);

            var cbps = info.ChannelBusyProbalies;

            for (int i = 0; i < cbps.Length; i++)
                graphs[k++].AddPoint(cbps[i], currentX);

            graphs[k++].AddPoint(info.AverageBusyChannelCount, currentX);

            var cips = info.ChannelIdleProbality;

            for (int i = 0; i < cips.Length; i++)
                graphs[k++].AddPoint(cips[i], currentX);

            var qbps = info.QueueBusyProbality;

            for (int i = 0; i < qbps.Length; i++)
                graphs[k++].AddPoint(qbps[i], currentX);

            int queueDiff = queueMaximumCapacity - qbps.Length;

            for (int i = 0; i < queueDiff; i++)
                graphs[k++].AddPoint(0, currentX);

            graphs[k++].AddPoint(info.AverageClientCountInQueue, currentX);
            graphs[k++].AddPoint(info.AverageClientQueueWaitingTime, currentX);
            graphs[k++].AddPoint(info.AverageClientServiceTime, currentX);
            graphs[k++].AddPoint(info.SummaryAverageClientTime, currentX);
            graphs[k++].AddPoint(info.AverageClientCount, currentX);
        }

        /// <summary>
        /// Анализирует диаграмму и генерирует текстовый отчет.
        /// </summary>
        /// <param name="diagramData">Анализируемая диаграмма.</param>
        /// <returns></returns>
        public static string GetDiagramAnalyzeText(IDiagramData diagramData)
        {
            if (diagramData.SystemWorkTime == 0)
                return "В процессе работы системы не поступило ни одной заявки! Анализ невозможен.";

            string text = "";

            text += "Результаты работы системы:\r\n";

            text += " 1. Количество заявок: " + diagramData.SummaryClientCount + " шт\r\n";
            text += " 2. Время работы (разница между временем появления первой и отъездом последней заявки): " + diagramData.SystemWorkTime + " часов\r\n\r\n";

            var info = new DiagramAnalyzer(diagramData);

            text += "Анализ временной диаграммы:\r\n";

            var names = GenerateGraphNames(diagramData.ChannelCount, diagramData.QueueCapacity);

            int k = 0;

            text += " " + names[k++] + ": " + info.SystemThroughput + " шт/ч\r\n";

            text += " " + names[k++] + ": " + info.ServedProbality + "\r\n";

            text += " " + names[k++] + ": " + info.RefuseProbality + "\r\n";

            var cbps = info.ChannelBusyProbalies;

            for (int i = 0; i < cbps.Length; i++)
                text += " " + names[k++] + ": " + cbps[i] + "\r\n";

            text += " " + names[k++] + ": " + info.AverageBusyChannelCount + "\r\n";

            var cips = info.ChannelIdleProbality;

            for (int i = 0; i < cips.Length; i++)
                text += " " + names[k++] + ": " + cips[i] + "\r\n";

            var qbps = info.QueueBusyProbality;

            for (int i = 0; i < qbps.Length; i++)
                text += " " + names[k++] + ": " + qbps[i] + "\r\n";

            text += " " + names[k++] + ": " + info.AverageClientCountInQueue + " шт\r\n";

            text += " " + names[k++] + ": " + info.AverageClientQueueWaitingTime + " часов\r\n";

            text += " " + names[k++] + ": " + info.AverageClientServiceTime + " часов\r\n";

            text += " " + names[k++] + ": " + info.SummaryAverageClientTime + " часов\r\n";

            text += " " + names[k++] + ": " + info.AverageClientCount + " шт";

            return text;
        }

        /// <summary>
        /// Генерирует массив названий показателей (т.е. пропускная способность системы, вероятность обслуживания, отказа и т.д.)
        /// </summary>
        /// <param name="channelCount">Количество мест обслуживания в системе</param>
        /// <param name="queueCapacity">Количество стояночных мест в системе</param>
        /// <returns></returns>
        public static string[] GenerateGraphNames(int channelCount, int queueCapacity)
        {
            int totalGraphCount = 9 + channelCount * 2 + queueCapacity;
            string[] names = new string[totalGraphCount];

            int k = 0;

            names[k++] = "1. Пропускная способность системы";
            names[k++] = "2. Вероятность обслуживания";
            names[k++] = "3. Вероятность отказа";

            for (int i = 0; i < channelCount; i++)
                names[k++] = string.Format("4.{0}. Вероятность занятости {0} каналов", i + 1);

            names[k++] = "5. Среднее количество занятых каналов";

            for (int i = 0; i < channelCount; i++)
                names[k++] = string.Format("6.{0}. Вероятность простоя {0} каналов", i + 1);

            for (int i = 0; i < queueCapacity; i++)
                names[k++] = string.Format("7.{0}. Вероятность того, что в очереди будет {0} заявок", i + 1);

            names[k++] = "8. Среднее количество заявок в очереди";
            names[k++] = "9. Среднее время ожидания заявки в очереди";
            names[k++] = "10 .Среднее время обслуживания заявки";
            names[k++] = "11. Среднее время нахождения заявки в системе";
            names[k++] = "12. Среднее количество заявок в системе";

            return names;
        }
    }
}
