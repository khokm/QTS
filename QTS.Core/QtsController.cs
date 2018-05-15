using System;
using System.IO;
using QTS.Core.Diagram;
using QTS.Core.Graphics;
using QTS.Core.Tools;
using QTS.Core.Helpers;

namespace QTS.Core
{
    /// <summary>
    /// Класс, управляющий работой программы.
    /// Действия пользователя нужно перенаправлять в этот класс,
    /// а обратная связь осуществляется через IUserInterface.
    /// </summary>
    /// <typeparam name="T">Используемая реализация временной диаграммы.</typeparam>
    public class QtsController
    {
        IDiagramViewController diagramViewController = null;
        IDiagramData diagram = null;
        string analyzeText = "";

        ICallbackUi CallbackUi { get; }
        IGraphicsFactory<TimeDiagram, IGraph> GraphicsFactory { get; }

        /// <summary>
        /// Создает новый экземпляр приложения для построения и анализа СМО.
        /// </summary>
        /// <param name="callbackUi">Используемый для обратной связи пользовательский интерфейс</param>
        /// <param name="graphicsFactory">Фабрика для создания графических элементов.</param>
        public QtsController(ICallbackUi callbackUi, IGraphicsFactory<TimeDiagram, IGraph> graphicsFactory)
        {
            CallbackUi = callbackUi;
            GraphicsFactory = graphicsFactory;
        }

        /// <summary>
        /// Заполняет диаграмму значениями (моделирует процесс работы системы)
        /// </summary>
        /// <param name="parameters">Параметры системы</param>
        /// <param name="timeDiagram">Заполняемая временная диаграмма</param>
        void FillDiagram(ParametersContainer parameters, ITimeDiagram timeDiagram)
        {
            RandomGenerator rnd = new RandomGenerator(parameters.MinRandomValue);
            double workTime = 0;

            DiagramCreator model = new DiagramCreator(parameters.QueueCapacity, parameters.ChannelsIntencites, parameters.PreferFirstChannel, timeDiagram);

            for (int i = 0; !parameters.HasClientLimit || i < parameters.ClientLimit; i++)
            {
                workTime += rnd.Next(parameters.ThreadIntencity);

                if (parameters.HasTimeLimit && workTime > parameters.TimeLimit)
                    break;

                model.PushClient(workTime, rnd);
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
        void AddPointsToGraph(ParametersContainer parameters, IGraph[] graphs, int queueMaximumCapacity, double currentX)
        {
            var timeDiagram = GraphicsFactory.CreateEmptyDiagram(parameters.ChannelCount, parameters.QueueCapacity);

            FillDiagram(parameters, timeDiagram);

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
        string GetDiagramAnalyzeText(IDiagramData diagramData)
        {
            if (diagramData.SystemWorkTime == 0)
                return "В процессе работы системы не поступило ни одной заявки! Анализ невозможен.";

            string text = "";

            text += "Результаты работы системы:\r\n";

            text += " 1. Количество заявок: " + diagramData.SummaryClientCount + " шт\r\n";
            text += " 2. Время работы: " + diagramData.SystemWorkTime + " часов\r\n\r\n";

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
        string[] GenerateGraphNames(int channelCount, int queueCapacity)
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

        /// <summary>
        /// Удаляет текущую используемую диаграмму.
        /// </summary>
        void RemoveCurrentDiagram()
        {
            diagramViewController = null;
            diagram = null;
            analyzeText = "";
            CallbackUi.RemoveDiagramView();
        }

        /// <summary>
        /// Проверяет корректность введенных параметров.
        /// </summary>
        /// <param name="parameters">Параметры для построения диаграммы, которые нужно проверить на правильность.</param>
        /// <returns></returns>
        public bool CheckParametersValid(ParametersContainer parameters)
        {
            if (!parameters.HasTimeLimit && !parameters.HasClientLimit)
            {
                CallbackUi.ShowError("Построить временную диаграмму", "Система не имеет ограничений ни по времени, ни по заявкам.");
                return false;
            }
            
            if (parameters.ChannelCount == 0)
                CallbackUi.ShowWarning("Построить временную диаграмму", "Система не имеет мест обслуживания.\nСледовательно, все заявки будут отклонены.");

            if (parameters.ChannelCount < 0 || parameters.QueueCapacity < 0)
            {
                CallbackUi.ShowError("Эта ошибка никогда не вылезет", "Надо было юзать unit");
                return false;
            }

            return true;
        }

        #region Вызовы управления отображением диаграммы
        public void GoToDiagramStart()
        {
            if (diagramViewController == null)
                return;

            diagramViewController.GoToStart();
        }

        public void GoToDiagramEnd()
        {
            if (diagramViewController == null)
                return;

            diagramViewController.GoToEnd();
        }

        public void GoToDiagramNext()
        {
            if (diagramViewController == null)
                return;

            diagramViewController.StepForward();
        }

        public void GoToDiagramPrev()
        {
            if (diagramViewController == null)
                return;

            diagramViewController.StepBack();
        }

        public void ShowPreviousLines(bool show)
        {
            if (diagramViewController == null)
                return;

            diagramViewController.ShowPreviousLines = show;
        }
        #endregion

        #region Вызовы построения и анализа диаграммы и синтеза СМО.
        /// <summary>
        /// Вызывает создание диаграммы по заданным значениям. Результатом является отображение диграммы на главной панели.
        /// </summary>
        /// <param name="parameters">Параметры диграммы, заданные пользователем</param>
        public void MakeDiagram(ParametersContainer parameters)
        {
            if (!CheckParametersValid(parameters))
                return;

            RemoveCurrentDiagram();

            var timeDiagram = GraphicsFactory.CreateEmptyDiagram(parameters.ChannelCount, parameters.QueueCapacity);

            try
            {
                FillDiagram(parameters, timeDiagram);
            }
            catch (Exception ex)
            {
                CallbackUi.ShowError("Ошибка вычислений", "При моделировании процесса возникло исключение:\n" + ex.Message);
                return;
            }

            //Сохраним ссылку на диаграмму, чтобы позже по команде пользователя провести ее анализ.
            diagram = timeDiagram;

            int clientCount = diagram.SummaryClientCount;

            bool showDiagram = clientCount < 200 || CallbackUi.YesNoDialog("Предупреждение", "Временная диаграмма содержит" + clientCount + " линий.\nЕе отрисовка может вызвать замедление работы компьютера.\n Отрисовать диаграмму?\n(анализ диаграммы возможен при любом выборе)");

            if (showDiagram)
            {
                CallbackUi.SetDiagramView(timeDiagram);
                diagramViewController = timeDiagram;
                diagramViewController.OnViewUpdated += CallbackUi.InvalidateDiagramView;
                diagramViewController.GoToEnd();
            }
        }

        /// <summary>
        /// Вызывает показ отчета об анализе диграммы. Результатом является отображение диграммы в текстовом окне.
        /// </summary>
        public void MakeDiagramAnalyze()
        {
            if (diagram == null)
            {
                CallbackUi.ShowError("Анализ диаграммы", "Диаграмма еще не создана.");
                return;
            }

            if (analyzeText.Length == 0)
                analyzeText = GetDiagramAnalyzeText(diagram);

            CallbackUi.ShowTextWindow("Анализ диаграммы", analyzeText);
        }

        /// <summary>
        /// Вызывает синтез СМО. Результатом является набор графиков и отчетов, сохраненные в указанные папки.
        /// </summary>
        /// <param name="parameters">Параметры, задаваемые пользователем.</param>
        /// <param name="minQueuePlaceCount">Минимальное количество мест в очереди</param>
        /// <param name="maxQueuePlaceCount">Максимальное количество мест в очереди</param>
        public void MakeSynthesis(ParametersContainer parameters, int minQueuePlaceCount, int maxQueuePlaceCount)
        {
            if (!CheckParametersValid(parameters))
                return;

            if (maxQueuePlaceCount <= minQueuePlaceCount || minQueuePlaceCount < 0 || maxQueuePlaceCount < 0)
            {
                CallbackUi.ShowError("Синтез СМО", "Некорректные значения градиента КМО");
                return;
            }

            string graphsFolder, reportsFolder;

            if ((graphsFolder = CallbackUi.GetFolderPath("Выберите папку для сохранения графиков")) == "" || (reportsFolder = CallbackUi.GetFolderPath("Выберите папку для сохранения отчетов", graphsFolder)) == "")
                return;

            var graphNames = GenerateGraphNames(parameters.ChannelCount, maxQueuePlaceCount);

            int totalGraphCount = graphNames.Length;

            var graphs = new IGraph[totalGraphCount];

            for (int i = 0; i < totalGraphCount; i++)
            {
                var oxyGraph = GraphicsFactory.CreateEmptyGraph();

                oxyGraph.Title = graphNames[i];
                oxyGraph.BeginLine();

                graphs[i] = oxyGraph;
            }

            bool noReportRights = false;

            for (int i = minQueuePlaceCount; i <= maxQueuePlaceCount; i++)
            {
                //Меняем параметр "КМО".
                parameters.QueueCapacity = i;

                try
                {
                    AddPointsToGraph(parameters, graphs, maxQueuePlaceCount, i);
                }
                catch (Exception ex)
                {
                    CallbackUi.ShowError("Ошибка вычислений", "При моделировании процесса возникло исключение:\n" + ex.Message);
                    return;
                }


                try
                {
                    File.WriteAllText(reportsFolder + "/Отчет для кол-ва мест " + i + ".txt", GetDiagramAnalyzeText(diagram));
                }
                catch
                {
                    noReportRights = true;
                }
            }

            bool noGraphRights = false;

            for (int i = 0; i < totalGraphCount; i++)
            {
                graphs[i].CompleteLine();

                var bitmap = graphs[i].ExportToBitmap();

                try
                {
                    bitmap.Save(graphsFolder + "/" + graphNames[i] + ".png");
                }
                catch
                {
                    noGraphRights = true;
                }
            }

            if (noGraphRights)
                CallbackUi.ShowWarning("", "Не удалось сохранить некоторые изображения графиков.\nВозможно, не достаточно прав для записи в выбранную папку.");

            if (noReportRights)
                CallbackUi.ShowWarning("", "Не удалось создать файлы отчетов.\nВозможно, не достаточно прав для записи в выбранную папку.");

            if(!noGraphRights)
                CallbackUi.StartExplorer(graphsFolder);

            if (!noReportRights && reportsFolder != graphsFolder)
                CallbackUi.StartExplorer(reportsFolder);

        }
        #endregion
    }
}