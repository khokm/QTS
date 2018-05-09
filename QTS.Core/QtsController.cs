using System;
using System.Collections.Generic;
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
        string diagramAnalyze = "";

        ICallbackUi CallbackUi { get; }

        /// <summary>
        /// Создает новый экземпляр приложения для построения и анализа СМО.
        /// </summary>
        /// <param name="callbackUi">Используемый для обратной связи пользовательский интерфейс</param>
        public QtsController(ICallbackUi callbackUi)
        {
            this.CallbackUi = callbackUi;
        }

        /// <summary>
        /// Удаляет текущую используемую диаграмму.
        /// </summary>
        void RemoveCurrentDiagram()
        {
            diagramViewController = null;
            diagram = null;
            diagramAnalyze = "";
            CallbackUi.RemoveDiagramView();
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
            RemoveCurrentDiagram();

            var timeDiagram = CallbackUi.CreateNewDiagram(parameters.channelCount, parameters.QueueCapacity);

            try
            {
                Solver.FillDiagram(parameters, timeDiagram);
            }
            catch(Exception ex)
            {
                CallbackUi.ShowError("Ошибка вычислений", ex.Message);
                return;
            }

            diagram = timeDiagram;

            int clientCount = timeDiagram.SummaryClientCount;

            bool showDiagram = clientCount < 200 || CallbackUi.YesNoDialog("Предупреждение", "Временная диаграмма содержит" + clientCount + " линий.\nЕе отрисовка может вызвать замедление работы компьютера.\n Отрисовать диаграмму?\n(анализ диаграммы возможен при любом выборе)");

            if (showDiagram)
            {
                diagramViewController = timeDiagram;
                CallbackUi.SetDiagramView(timeDiagram);
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
                return;

            if (diagramAnalyze.Length == 0)
                diagramAnalyze = Solver.GetDiagramAnalyzeText(diagram);

            CallbackUi.ShowTextWindow("Анализ диаграммы", diagramAnalyze);
        }

        /// <summary>
        /// Вызывает синтез СМО. Результатом является набор графиков и отчетов, сохраненные в указанные папки.
        /// </summary>
        /// <param name="parameters">Параметры, задаваемые пользователем.</param>
        /// <param name="minQueuePlaceCount">Минимальное количество мест в очереди</param>
        /// <param name="maxQueuePlaceCount">Максимальное количество мест в очереди</param>
        public void MakeSynthesis(ParametersContainer parameters, int minQueuePlaceCount, int maxQueuePlaceCount)
        {
            string folder = CallbackUi.GetImagePathFolder("Выберите папку для сохранения графиков");

            if (folder == "")
                return;

            string textFolder = CallbackUi.GetImagePathFolder("Выберите папку для сохранения отчетов");

            if (textFolder == "")
                return;

            var graphNames = Solver.GenerateGraphNames(parameters.channelCount, maxQueuePlaceCount);

            int totalGraphCount = graphNames.Length;

            var analyzers = new List<DiagramAnalyzer>();
            var graphs = new IGraph[totalGraphCount];

            for (int i = 0; i < totalGraphCount; i++)
            {
                var oxyGraph = CallbackUi.CreateGraph();

                oxyGraph.Title = graphNames[i];
                oxyGraph.StartLine("");

                graphs[i] = oxyGraph;
            }

            try
            {
                for(int k = minQueuePlaceCount; k <= maxQueuePlaceCount; k++)
                {
                    parameters.QueueCapacity = k;
                    var diagram = CallbackUi.CreateNewDiagram(parameters.channelCount, parameters.QueueCapacity);

                    Solver.FillDiagram(parameters, diagram);
                    Solver.AddPointsToGraph(graphs, maxQueuePlaceCount, k, diagram);

                    var analyzeText = Solver.GetDiagramAnalyzeText(diagram);
                    File.WriteAllText(textFolder + "/Отчет для кол-ва мест " + k + ".txt", analyzeText);
                }
            }
            catch(Exception ex)
            {
                CallbackUi.ShowError("Ошибка вычислений", ex.Message);
                return;
            }

            for (int i = 0; i < totalGraphCount; i++)
            {
                graphs[i].CompleteLine();

                var bitmap = graphs[i].ExportToBitmap();

                bitmap.Save(folder + "/" + graphNames[i] + ".png");
            }
        }
        #endregion
    }
}