using System;
using System.IO;
using QTS.Core.Diagram;
using QTS.Core.Graphics;
using QTS.Core.Tools;

namespace QTS.Core
{
    /// <summary>
    /// Класс, управляющий работой программы.
    /// Действия пользователя нужно перенаправлять в этот класс,
    /// а обратная связь осуществляется через IUserInterface.
    /// </summary>
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
                CallbackUi.ShowError("Эта ошибка никогда не вылезет", "Надо было юзать unit");
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

            if (gradient.MaxPlaceCount <= gradient.MinPlaceCount || gradient.MinPlaceCount < 0 || gradient.MaxPlaceCount < 0)
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

            if (!CheckParametersValid(parameters))
                return;

            RemoveCurrentDiagram();

            TimeDiagram timeDiagram;

            try
            {
                ProcessModeller modeller = new ProcessModeller(parameters);
                timeDiagram = modeller.CreateDiagram(GraphicsFactory);
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
                analyzeText = Solver.GetDiagramAnalyzeText(diagram);

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
                !CheckParametersValid(gradient = CallbackUi.GetQueuePlaceGradientData()) ||
                (graphsFolder = CallbackUi.GetFolderPath("Выберите папку для сохранения графиков")) == "" ||
                (reportsFolder = CallbackUi.GetFolderPath("Выберите папку для сохранения отчетов", graphsFolder)) == "")
                return;

            var graphNames = Solver.GenerateGraphNames(parameters.ChannelCount, gradient.MaxPlaceCount);

            int totalGraphCount = graphNames.Length;

            var graphs = new IGraph[totalGraphCount];

            for (int i = 0; i < totalGraphCount; i++)
            {
                var oxyGraph = GraphicsFactory.CreateEmptyGraph("Кол-во мест в очереди", graphNames[i]);

                oxyGraph.Title = graphNames[i];
                oxyGraph.BeginLine();

                graphs[i] = oxyGraph;
            }

            bool noReportRights = false;

            //Меняем параметр "КМО".
            for (parameters.QueueCapacity = gradient.MinPlaceCount; parameters.QueueCapacity <= gradient.MaxPlaceCount; parameters.QueueCapacity++)
            {
                CallbackUi.ShowSynthesisStats(parameters.QueueCapacity - gradient.MinPlaceCount + 1, gradient.MaxPlaceCount - gradient.MinPlaceCount + 1);
                TimeDiagram timeDiagram;

                try
                {
                    ProcessModeller modeller = new ProcessModeller(parameters);
                    timeDiagram = modeller.CreateDiagram(GraphicsFactory);
                }
                catch (Exception ex)
                {
                    CallbackUi.CloseSynthesisStats();
                    CallbackUi.ShowError("Ошибка вычислений", "При моделировании процесса возникло исключение:\n" + ex.Message);
                    return;
                }

                Solver.AddPointsToGraph(timeDiagram, parameters, graphs, gradient.MaxPlaceCount, parameters.QueueCapacity);

                try
                {
                    File.WriteAllText(reportsFolder + "/Отчет для кол-ва мест " + parameters.QueueCapacity + ".txt", Solver.GetDiagramAnalyzeText(timeDiagram));
                }
                catch
                {
                    noReportRights = true;
                }
            }

            CallbackUi.CloseSynthesisStats();

            bool noGraphRights = false;

            for (int i = 0; i < totalGraphCount; i++)
            {
                graphs[i].CompleteLine(false);

                var bitmap = graphs[i].ExportToBitmap(true);

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

            if (!noGraphRights)
                CallbackUi.StartExplorer(graphsFolder);

            if (!noReportRights && reportsFolder != graphsFolder)
                CallbackUi.StartExplorer(reportsFolder);

        }
        #endregion
    }
}