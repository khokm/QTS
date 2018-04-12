using QTS.Core.Diagram;
using QTS.Core.Helpers;

namespace QTS.Core.Tools
{
    class Solver
    {
        public static void FillDiagram(Parameters parameters, IDiagram timeDiagram)
        {
            RandomGenerator rnd = new RandomGenerator(parameters.minRandomValue);
            double workTime = 0;

            DiagramCreator model = new DiagramCreator(parameters.queueCapacity, parameters.channelsIntencity, parameters.preferFirstChannel, timeDiagram);

            for (int i = 0; !parameters.hasClientLimit || i < parameters.maxClients; i++)
            {
                workTime += rnd.Next(parameters.threadIntencity);

                if (parameters.hasTimeLimit && workTime > parameters.maxTime)
                    break;

                model.PushClient(workTime, rnd);
            }

            timeDiagram.CompleteDiagram();
        }


        public static void AddPointsToGraph(IGraph[] graphs, int queueMaximumCapacity, double currentX, IAnalyzable timeDiagram)
        {
            var info = new DiagramAnalyzer(timeDiagram);

            int k = 0;

            graphs[k++].AddPoint(info.systemThroughput, currentX);
            graphs[k++].AddPoint(info.servedProbality, currentX);
            graphs[k++].AddPoint(info.lostProbality, currentX);

            var cbps = info.channelBusyProbality;

            for (int i = 0; i < cbps.Length; i++)
                graphs[k++].AddPoint(cbps[i], currentX);

            graphs[k++].AddPoint(info.averageBusyChannelCount, currentX);

            var cips = info.channelIdleProbality;

            for (int i = 0; i < cips.Length; i++)
                graphs[k++].AddPoint(cips[i], currentX);

            var qbps = info.queueBusyProbality;

            for (int i = 0; i < qbps.Length; i++)
                graphs[k++].AddPoint(qbps[i], currentX);

            int queueDiff = queueMaximumCapacity - qbps.Length;

            for(int i = 0; i < queueDiff; i++)
                graphs[k++].AddPoint(0, currentX);

            graphs[k++].AddPoint(info.averageClientCountInQueue, currentX);
            graphs[k++].AddPoint(info.averageClientQueueWaitingTime, currentX);
            graphs[k++].AddPoint(info.averageClientServiceTime, currentX);
            graphs[k++].AddPoint(info.averageClientTime, currentX);
            graphs[k++].AddPoint(info.averageClientCountInSystem, currentX);
        }

        public static string DiagramAnalyze(IAnalyzable timeDiagram)
        {
            string analyzeText = "";

            if (timeDiagram.systemWorkTime == 0)
                return "В процессе работы системы не поступило ни одной заявки! Анализ невозможен.";

            analyzeText += "Результаты работы системы:\r\n";

            analyzeText += " 1. Количество заявок: " + timeDiagram.clientsCount + " шт\r\n";
            analyzeText += " 2. Время работы: " + timeDiagram.systemWorkTime + " часов\r\n\r\n";

            var info = new DiagramAnalyzer(timeDiagram);

            analyzeText += "Анализ временной диаграммы:\r\n";

            var names = GenerateGraphNames(timeDiagram.channelCount, timeDiagram.queueCapacity);

            int k = 0;

            analyzeText += " " + names[k++] + ": " + info.systemThroughput + " шт/ч\r\n";

            analyzeText += " " + names[k++] + ": " + info.servedProbality + "\r\n";

            analyzeText += " " + names[k++] + ": " + info.lostProbality + "\r\n";

            var cbps = info.channelBusyProbality;

            for (int i = 0; i < cbps.Length; i++)
                analyzeText += " " + names[k++] + ": " + cbps[i] + "\r\n";

            analyzeText += " " + names[k++] + ": " + info.averageBusyChannelCount + "\r\n";

            var cips = info.channelIdleProbality;

            for (int i = 0; i < cips.Length; i++)
                analyzeText += " " + names[k++] + ": " + cips[i] + "\r\n";

            var qbps = info.queueBusyProbality;

            for (int i = 0; i < qbps.Length; i++)
                analyzeText += " " + names[k++] + ": " + qbps[i] + "\r\n";

            analyzeText += " " + names[k++] + ": " + info.averageClientCountInQueue + " шт\r\n";

            analyzeText += " " + names[k++] + ": " + info.averageClientQueueWaitingTime + " часов\r\n";

            analyzeText += " " + names[k++] + ": " + info.averageClientServiceTime + " часов\r\n";

            analyzeText += " " + names[k++] + ": " + info.averageClientTime + " часов\r\n";

            analyzeText += " " + names[k++] + ": " + info.averageClientCountInSystem + " шт";

            return analyzeText;
        }

        public static string[] GenerateGraphNames(int channelCount, int queueCapacity)
        {
            int totalGraphCount = 9 + channelCount * 2 + queueCapacity;
            string[] names = new string[totalGraphCount];

            int k = 0;

            names[k++] = "1. Пропускная способность системы";
            names[k++] = "2. Вероятность обслуживания";
            names[k++] = "3. Вероятность отказа";

            for(int i = 0; i < channelCount; i++)
                names[k++] = string.Format("4.{0}. Вероятность занятости {0} каналов", i + 1);

            names[k++] = "5. Среднее количество занятых каналов";

            for (int i = 0; i < channelCount; i++)
                names[k++] = string.Format("6.{0}. Вероятность простоя {0} каналов", i + 1);

            for(int i = 0; i < queueCapacity; i++)
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
