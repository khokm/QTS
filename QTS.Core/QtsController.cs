using System;
using System.IO;
using QTS.Core.Tools;
using System.Collections.Generic;
using System.Linq;

namespace QTS.Core
{
    /// <summary>
    /// Класс, управляющий работой программы.
    /// Действия пользователя нужно перенаправлять в этот класс,
    /// а обратная связь осуществляется через IUserInterface.
    /// </summary>
    public class QtsController
    {
        InteractiveDiagram diagramViewController = null;
        DiagramData diagram = null;
        string analyzeText = "";

        ICallbackUi CallbackUi { get; }
        IGraphicsFactory<InteractiveDiagram, IGraph> GraphicsFactory { get; }


        static IEnumerable<Metric> GetMetrics(int channelCount, int queueCapacity)
        {
            List<Metric> metrics = new List<Metric>(9 + channelCount * 2 + queueCapacity);

            metrics.Add(new Metric("1. Пропускная способность системы", "шт/ч", Formulas.SystemThroughput, MetricType.Float));
            metrics.Add(new Metric("2. Вероятность обслуживания", "", Formulas.ServedProbality, MetricType.Probality));
            metrics.Add(new Metric("3. Вероятность отказа", "", Formulas.RefuseProbality, MetricType.Probality));

            for (int i = 1; i < channelCount + 1; i++)
            {
                int b = i;
                metrics.Add(new Metric($"4.{ i }. Вероятность занятости только { i } каналов", "", diagram => Formulas.ChannelBusyProbality(diagram, b), MetricType.Probality));
            }

            metrics.Add(new Metric("5. Среднее количество занятых каналов", "", Formulas.AverageBusyChannelCount, MetricType.Float));

            for (int i = 1; i < channelCount + 1; i++)
            {
                int b = i;
                metrics.Add(new Metric($"6.{ i }. Вероятность простоя { i } и более каналов", "", diagram => Formulas.ChannelIdleProbality(diagram, b), MetricType.Probality));
            }

            for (int i = 1; i < queueCapacity + 1; i++)
            {
                int b = i;
                metrics.Add(new Metric($"7.{ i }. Вероятность того, что в очереди будет одновременно { i } заявок", "", diagram => Formulas.QueueBusyProbality(diagram, b), MetricType.Probality));
            }

            metrics.Add(new Metric("8. Среднее количество заявок в очереди", "шт", Formulas.AverageClientCountInQueue, MetricType.Float));
            metrics.Add(new Metric("9. Среднее время ожидания заявки в очереди", "", Formulas.AverageClientQueueWaitingTime, MetricType.Float));
            metrics.Add(new Metric("10 .Среднее время обслуживания заявки", "часов", Formulas.AverageClientServiceTime, MetricType.Float));
            metrics.Add(new Metric("11. Среднее время нахождения заявки в системе", "часов", Formulas.SummaryAverageClientTime, MetricType.Float));
            metrics.Add(new Metric("12. Среднее количество заявок в системе", "шт", Formulas.AverageClientCount, MetricType.Float));

            return metrics;
        }

        /// <summary>
        /// Создает новый экземпляр приложения для построения и анализа СМО.
        /// </summary>
        /// <param name="callbackUi">Используемый для обратной связи пользовательский интерфейс</param>
        /// <param name="graphicsFactory">Фабрика для создания графических элементов.</param>
        public QtsController(ICallbackUi callbackUi, IGraphicsFactory<InteractiveDiagram, IGraph> graphicsFactory)
        {
            CallbackUi = callbackUi;
            GraphicsFactory = graphicsFactory;
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
        bool CheckParametersValid(ParametersContainer parameters)
        {
            if (parameters == null)
                return false;

            if (!parameters.HasTimeLimit && !parameters.HasClientLimit)
            {
                CallbackUi.ShowError("Построить временную диаграмму", "Система не имеет ограничений ни по времени, ни по заявкам.");
                return false;
            }

            if (parameters.ChannelCount == 0)
                CallbackUi.ShowWarning("Построить временную диаграмму", "Система не имеет мест обслуживания.\nСледовательно, все заявки будут отклонены.");

            if (parameters.ChannelCount < 0 || parameters.QueueCapacity < 0)
            {
                CallbackUi.ShowError("Эта ошибка никогда не вылезет", "Надо было юзать uint");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверяет корректность введенных параметров.
        /// </summary>
        /// <param name="gradient">Параметры для синтеза СМО.</param>
        /// <returns></returns>
        bool CheckParametersValid(QueuePlaceGradientData gradient)
        {
            if (gradient == null)
                return false;

            if (gradient.MaxQueueCapacity <= gradient.MinQueueCapacity || gradient.MinQueueCapacity < 0 || gradient.MaxQueueCapacity < 0)
            {
                CallbackUi.ShowError("Синтез СМО", "Некорректные значения градиента КМО.");
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
        public void MakeDiagram()
        {
            ParametersContainer parameters = CallbackUi.GetDiagramParameters();


            //QueuePlaceGradientData gradient = new QueuePlaceGradientData(2, 30);

            //List<DiagramData> diagrams = new List<DiagramData>(gradient.MaxQueueCapacity - gradient.MinQueueCapacity + 1);

            //for (parameters.QueueCapacity = gradient.MinQueueCapacity; parameters.QueueCapacity <= gradient.MaxQueueCapacity; parameters.QueueCapacity++)
            //{
            //    ProcessModeller mdl = new ProcessModeller(parameters);
            //    diagrams.Add(mdl.CreateDiagram(GraphicsFactory));
            //}

            //var metrics = new Metric[]
            //{
            //    new Metric(Formulas.AverageBusyChannelCount, "Среднее колво занятых каналов", "", MetricType.Float)
            //};

            //var graphs = ReportTool.CreateGraphs(diagrams.ToArray(), metrics, gradient, GraphicsFactory);

            //foreach (var graph in graphs)
            //    graph.ExportToBitmap(true).Save(graph.Title + ".png");


            if (!CheckParametersValid(parameters))
                return;

            RemoveCurrentDiagram();

            DiagramData timeDiagram;

            bool useGraphics = true;

            if (parameters.HasClientLimit && parameters.ClientLimit > 200 || parameters.TimeLimit * parameters.ThreadIntencity > 200)
            {
                string text = parameters.HasClientLimit ? string.Format("содержит {0}", parameters.ClientLimit) : string.Format("будет содержать около {0}", parameters.ThreadIntencity * parameters.TimeLimit);
                useGraphics = CallbackUi.YesNoDialog("Предупреждение", "Временная диаграмма " + text + " линий.\nЕе отрисовка может вызвать замедление работы компьютера.\n Отрисовать диаграмму ?\n(анализ диаграммы возможен при любом выборе)");
            }

            ProcessModeller modeller = new ProcessModeller(parameters);

            try
            {
                timeDiagram = useGraphics ? modeller.CreateDiagram(GraphicsFactory) : modeller.CreateDiagram();
            }
            catch (Exception ex)
            {
                CallbackUi.ShowError("Ошибка вычислений", "При моделировании процесса возникло исключение:\n" + ex.Message);
                return;
            }

            //Сохраним ссылку на диаграмму, чтобы позже по команде пользователя провести ее анализ.
            diagram = timeDiagram;

            if (useGraphics)
            {
                CallbackUi.SetDiagramView(timeDiagram.InteractiveDiagram);
                diagramViewController = timeDiagram.InteractiveDiagram;
                diagramViewController.ViewUpdated += CallbackUi.InvalidateDiagramView;
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
            {
                analyzeText = diagram.ReadonlyParameters.ToString() + "\r\n";

                analyzeText += "Показатели системы:\r\n";

                if (diagram.SystemWorkTime == 0 || diagram.SummaryClientCount == 0)
                {
                    analyzeText += "В процессе работы системы не поступило ни одной заявки! Анализ невозможен.";
                }
                else
                {
                    analyzeText += " 1. Количество заявок: " + diagram.SummaryClientCount + " шт\r\n";
                    analyzeText += "   1.1 Из них обслужено: " + diagram.ServedClientCount + " шт\r\n";
                    analyzeText += "   1.2 Из них утеряно: " + diagram.LostClientCount + " шт\r\n";

                    analyzeText += " 2. Время работы системы: " + (float)diagram.SystemWorkTime + " часов\r\n\r\n";

                    analyzeText += ReportTool.MakeReport(diagram, GetMetrics(diagram.ChannelCount, diagram.QueueCapacity));
                }
            }

            CallbackUi.ShowTextWindow("Анализ диаграммы", analyzeText);
        }

        /// <summary>
        /// Вызывает синтез СМО. Результатом является набор графиков и отчетов, сохраненные в указанные папки.
        /// </summary>
        /// <param name="parameters">Параметры, задаваемые пользователем.</param>
        /// <param name="minQueuePlaceCount">Минимальное количество мест в очереди</param>
        /// <param name="maxQueuePlaceCount">Максимальное количество мест в очереди</param>
        public void MakeSynthesis()
        {
            ParametersContainer parameters;
            QueuePlaceGradientData gradient;
            string graphsFolder;
            string reportsFolder;

            if (!CheckParametersValid(parameters = CallbackUi.GetDiagramParameters()) ||
                !CheckParametersValid(gradient = CallbackUi.GetQueuePlaceGradientData()))
                return;

            graphsFolder = CallbackUi.GetFolderPath("Выберите папку для сохранения графиков");

            if (graphsFolder == "")
                CallbackUi.ShowWarning("Синтез СМО", "Графики не будут сохранены");

            reportsFolder = CallbackUi.GetFolderPath("Выберите папку для сохранения отчетов", graphsFolder);

            if(graphsFolder == "" && reportsFolder == "")
            {
                CallbackUi.ShowWarning("Синтез СМО", "Отмена синтеза СМО");
                return;
            }

            List<DiagramData> diagrams = new List<DiagramData>(gradient.MaxQueueCapacity - gradient.MinQueueCapacity + 1);

            //Меняем параметр "КМО".
            for (parameters.QueueCapacity = gradient.MinQueueCapacity; parameters.QueueCapacity <= gradient.MaxQueueCapacity; parameters.QueueCapacity++)
            {
                CallbackUi.ShowSynthesisStats(parameters.QueueCapacity - gradient.MinQueueCapacity + 1, gradient.MaxQueueCapacity - gradient.MinQueueCapacity + 1);

                ProcessModeller modeller = new ProcessModeller(parameters);

                try
                {
                    diagrams.Add(modeller.CreateDiagram(GraphicsFactory));
                }
                catch (Exception ex)
                {
                    CallbackUi.CloseSynthesisStats();
                    CallbackUi.ShowError("Ошибка вычислений", "При моделировании процесса возникло исключение:\n" + ex.Message);
                    return;
                }
            }

            CallbackUi.CloseSynthesisStats();

            var graphs = ReportTool.CreateGraphs(diagrams, GetMetrics(parameters.ChannelCount, gradient.MaxQueueCapacity), gradient, GraphicsFactory);

            if(graphsFolder != "")
                foreach(var graph in graphs)
                    graph.ExportToBitmap(true).Save(graphsFolder + "/" + graph.Title + ".png");

            if(reportsFolder != "")
                foreach(var diagram in diagrams)
                    File.WriteAllText(reportsFolder + "/Отчет для кол-ва мест " + diagram.QueueCapacity + ".txt", ReportTool.MakeReport(diagram, GetMetrics(diagram.ChannelCount, diagram.QueueCapacity)));

            if(graphsFolder != "")
                CallbackUi.StartExplorer(graphsFolder);
            if (reportsFolder != "" && reportsFolder != graphsFolder)
                CallbackUi.StartExplorer(reportsFolder);

        }
        #endregion
    }
}