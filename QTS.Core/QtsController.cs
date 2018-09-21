using QTS.Core.Tools;
using QTS.Core.Graphics;
using QTS.Core.Diagram;

using System.Collections.Generic;
using System.Linq;
using System;

namespace QTS.Core
{
    /// <summary>
    /// Класс, управляющий работой программы.
    /// Действия пользователя нужно перенаправлять в этот класс,
    /// а обратная связь осуществляется через IUserInterface.
    /// </summary>
    public class QtsController
    {
        string analyzeText = "";

        IUserInterface CallbackUi { get; }
        IGraphicsFactory<InteractiveDiagram, IGraph> GraphicsFactory { get; }

        static IEnumerable<Metric> clientMetrics = new[]
        {
            new Metric("1. Количество заявок", "шт", Formulas.SummaryClientCount, MetricType.Integer),
            new Metric(" 1.1 Из них обслужено", "шт", Formulas.ServedClientCount, MetricType.Integer),
            new Metric(" 1.2 Из них отказано", "шт", Formulas.LostClientCount, MetricType.Integer),
            new Metric("2. Время работы системы", "часов", Formulas.SystemWorkTime, MetricType.Float)
        };

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
        public QtsController(IUserInterface callbackUi, IGraphicsFactory<InteractiveDiagram, IGraph> graphicsFactory)
        {
            CallbackUi = callbackUi;
            GraphicsFactory = graphicsFactory;
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

        bool CheckParametersValid(GraphImprovementParamsData paramsData, int metricsCount)
        {
            if (paramsData == null)
                return false;

            if(paramsData.MetricIndex < 0 || paramsData.MetricIndex > metricsCount - 1)
            {
                CallbackUi.ShowError("Улучшение графика", "Такого графика не существует.");
                return false;
            }

            if (paramsData.ExperimentCount <= 0)
            {
                CallbackUi.ShowError("Улучшение графика", "Количество повторов экспериментов должно быть больше 0.");
                return false;
            }

            return true;
        }

        static string MakeReport(ParametersContainer diagramParameters, TimeDiagram timeDiagram)
        {
            return diagramParameters.ToString() + "\r\n" + ReportTool.MakeReport(timeDiagram, clientMetrics, GetMetrics(timeDiagram.ChannelCount, timeDiagram.QueueCapacity));
        }

        #region Вызовы построения и анализа диаграммы и синтеза СМО.
        /// <summary>
        /// Вызывает создание диаграммы по заданным значениям. Результатом является отображение диграммы на главной панели.
        /// </summary>
        /// <param name="parameters">Параметры диграммы, заданные пользователем</param>
        public void MakeDiagram()
        {
            ParametersContainer parameters = CallbackUi.GetDiagramParameters();

            if (!CheckParametersValid(parameters))
                return;

            TimeDiagram timeDiagram;

            bool useGraphics = true;

            var timeToCl = (int)parameters.TimeLimit * parameters.ThreadIntencity;

            int min;

            if (!parameters.HasTimeLimit)
                min = parameters.ClientLimit;
            else if (!parameters.HasClientLimit)
                min = timeToCl;
            else
                min = Math.Min(timeToCl, parameters.ClientLimit);

            if (min > 200)
            {
                useGraphics = CallbackUi.YesNoDialog("Предупреждение", string.Format(@"Временная диаграмма будет содержать около {0} линий.
Ее отрисовка может вызвать замедление работы компьютера.
Отрисовать диаграмму?
(анализ диаграммы возможен при любом выборе)", min));
            }

            ProcessModeller modeller = new ProcessModeller(parameters);

            InteractiveDiagram intDiag = null;

            try
            {
                timeDiagram = useGraphics ? modeller.CreateDiagram(GraphicsFactory, out intDiag) : modeller.CreateDiagram();
            }
            catch
            {
                CallbackUi.ShowError("Создать диаграмму", "При моделировании процесса возникло исключение."/* + ex.StackTrace*/);
                return;
            }

            CallbackUi.InteractiveDiagram = intDiag;

            analyzeText = MakeReport(parameters, timeDiagram);

            if (intDiag != null)
            {
                CallbackUi.HideText();
                intDiag.ViewUpdated += CallbackUi.InvalidateDiagramView;
                intDiag.GoToEnd();
            }
            else
            {
                CallbackUi.ShowText("Отображение диграммы отключено.\nДля ее анализа, используйте Действия - Анализ диаграммы.");
            }
        }

        /// <summary>
        /// Вызывает показ отчета об анализе диграммы. Результатом является отображение диграммы в текстовом окне.
        /// </summary>
        public void MakeDiagramAnalyze()
        {
            if (analyzeText == "")
            {
                CallbackUi.ShowError("Анализ диаграммы", "Диаграмма еще не создана.");
                return;
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

            if (graphsFolder == "" && reportsFolder == "")
            {
                CallbackUi.ShowWarning("Синтез СМО", "Отмена синтеза СМО");
                return;
            }

            List<TimeDiagram> diagrams = new List<TimeDiagram>(gradient.MaxQueueCapacity - gradient.MinQueueCapacity + 1);

            CallbackUi.LockInterface();

            //Меняем параметр "КМО".
            for (parameters.QueueCapacity = gradient.MinQueueCapacity; parameters.QueueCapacity <= gradient.MaxQueueCapacity; parameters.QueueCapacity++)
            {
                CallbackUi.ShowText($"Моделирование процесса: {parameters.QueueCapacity - gradient.MinQueueCapacity + 1} из {gradient.MaxQueueCapacity - gradient.MinQueueCapacity + 1}...");

                ProcessModeller modeller = new ProcessModeller(parameters);

                try
                {
                    diagrams.Add(modeller.CreateDiagram());
                }
                catch/* (Exception ex)*/
                {
                    CallbackUi.HideText();
                    CallbackUi.UnlockInterface();
                    CallbackUi.ShowError("Синтез СМО", "При моделировании процесса возникло исключение."/* + ex.Message*/);
                    return;
                }
            }

            CallbackUi.HideText();
            CallbackUi.UnlockInterface();

            try
            {
                if (graphsFolder != "")
                {
                    foreach (var metric in GetMetrics(parameters.ChannelCount, gradient.MaxQueueCapacity))
                    {
                        var graph = GraphicsFactory.CreateGraphByPoints(gradient.MinQueueCapacity, ReportTool.GetHeights(diagrams, metric), "Кол-во мест в очереди", metric.Name);
                        graph.ExportToBitmap(true, graphsFolder + "/" + graph.Title + ".png");
                    }

                    CallbackUi.StartExplorer(graphsFolder);
                }
            }
            catch
            {
                CallbackUi.ShowError("Синтез СМО", "Недостаточно прав для записи в " + graphsFolder);
            }

            try
            {
                if (reportsFolder != "")
                {
                    foreach (var diagram in diagrams)
                        CallbackUi.CreateTextFile(reportsFolder + "/Отчет для кол-ва мест " + diagram.QueueCapacity + ".txt", MakeReport(parameters, diagram));

                    if (reportsFolder != graphsFolder)
                        CallbackUi.StartExplorer(reportsFolder);
                }
            }
            catch
            {
                CallbackUi.ShowError("Синтез СМО", "Недостаточно прав для записи в " + reportsFolder);
            }

        }

        public void MakeGraphImprovement()
        {
            ParametersContainer parameters;
            QueuePlaceGradientData gradient;

            if (!CheckParametersValid(parameters = CallbackUi.GetDiagramParameters()) ||
                !CheckParametersValid(gradient = CallbackUi.GetQueuePlaceGradientData()))
                return;

            var metrics = GetMetrics(parameters.ChannelCount, gradient.MaxQueueCapacity).ToArray();

            GraphImprovementParamsData improvementData = CallbackUi.GetGraphImprovementParams(metrics.Select(metric => metric.Name).ToArray());

            if (!CheckParametersValid(improvementData, metrics.Length))
                return;

            Metric usedMetric = metrics[improvementData.MetricIndex];

            InteractiveDiagram intDiag = GraphicsFactory.CreateInteractiveGraph(usedMetric.Name);

            double[] heightsSum = null;

            CallbackUi.LockInterface();

            for (int i = 0; i < improvementData.ExperimentCount; i++)
            {
                List<TimeDiagram> diagrams = new List<TimeDiagram>(gradient.MaxQueueCapacity - gradient.MinQueueCapacity + 1);

                for (parameters.QueueCapacity = gradient.MinQueueCapacity; parameters.QueueCapacity <= gradient.MaxQueueCapacity; parameters.QueueCapacity++)
                {
                    CallbackUi.ShowText($"Эксперимент { i + 1} из {improvementData.ExperimentCount}.\nМоделирование процесса: {parameters.QueueCapacity - gradient.MinQueueCapacity + 1} из {gradient.MaxQueueCapacity - gradient.MinQueueCapacity + 1}...");

                    ProcessModeller modeller = new ProcessModeller(parameters);
                    try
                    {
                        diagrams.Add(modeller.CreateDiagram());
                    }
                    catch
                    {
                        CallbackUi.HideText();
                        CallbackUi.UnlockInterface();
                        CallbackUi.ShowError("Улучшение графика", "При моделировании процесса возникло исключение."/* + ex.Message*/);
                    }

                }

                var heights = ReportTool.GetHeights(diagrams, usedMetric);

                if (heightsSum == null)
                    heightsSum = heights.ToArray();
                else
                    heightsSum = heightsSum.Zip(heights, (a, b) => a + b).ToArray();

                intDiag.BeginInteractiveLine(i);
                intDiag.AddPoints(heights, gradient.MinQueueCapacity);
                intDiag.AddLineMetadata($"Эксперимент { i + 1 }");
                intDiag.CompleteLine();
            }

            CallbackUi.HideText();
            CallbackUi.UnlockInterface();

            intDiag.BeginLine(1);
            intDiag.AddPoints(heightsSum.Select(height => height / improvementData.ExperimentCount), gradient.MinQueueCapacity);
            intDiag.AddLineMetadata($"Сумма { improvementData.ExperimentCount } графиков");
            intDiag.CompleteLine();

            intDiag.SetLayer(0);
            CallbackUi.InteractiveDiagram = intDiag;
            intDiag.ViewUpdated += CallbackUi.InvalidateDiagramView;
            intDiag.GoToEnd();
        }
        #endregion
    }
}