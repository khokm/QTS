using System;
using System.Collections.Generic;
using System.IO;
using QTS.Core.Diagram;
using QTS.Core.Graphics;
using QTS.Core.Tools;

namespace QTS.Core
{
    public class QtsController
    {
        IController diagramController = null;
        IAnalyzable diagram = null;
        string diagramAnalyze = "";

        IForm callbackForm;

        public QtsController(IForm callbackForm)
        {
            this.callbackForm = callbackForm;
        }

        void ResetApplicationData()
        {
            diagramController = null;
            diagram = null;
            diagramAnalyze = "";
            callbackForm.RemoveModel();
        }

        //Diagram Control Buttons
        //
        public void GoToDiagramStart()
        {
            if (diagramController == null)
                return;

            diagramController.GoToStart();
        }

        public void GoToDiagramEnd()
        {
            if (diagramController == null)
                return;

            diagramController.GoToEnd();
        }

        public void GoToDiagramNext()
        {
            if (diagramController == null)
                return;

            diagramController.StepForward();
        }

        public void GoToDiagramPrev()
        {
            if (diagramController == null)
                return;

            diagramController.StepBack();
        }

        public void ShowPreviousLines(bool show)
        {
            if (diagramController == null)
                return;

            diagramController.showPreviousLines = show;
        }

        //Methods
        //
        public void CreateTimeDiagram(Parameters parameters)
        {
            ResetApplicationData();

            var timeDiagram = callbackForm.CreateDiagram(parameters.channelCount, parameters.queueCapacity);

            try
            {
                Solver.FillDiagram(parameters, timeDiagram);
            }
            catch(Exception ex)
            {
                callbackForm.ShowError("Ошибка вычислений", ex.Message);
                return;
            }

            diagram = timeDiagram;

            int clientCount = timeDiagram.clientsCount;

            bool showDiagram = clientCount < 200 || callbackForm.YesNoDialog("Предупреждение", "Временная диаграмма содержит" + clientCount + " линий.\nЕе отрисовка может вызвать замедление работы компьютера.\n Отрисовать диаграмму?\n(анализ диаграммы возможен при любом выборе)");

            if (showDiagram)
            {
                diagramController = timeDiagram;
                callbackForm.SetModel(timeDiagram);
                diagramController.OnVisualUpdated += callbackForm.GetFormUpdateAction();
                diagramController.GoToEnd();
            }
        }

        public void ShowDiagramAnalyze()
        {
            if (diagram == null)
                return;

            if (diagramAnalyze.Length == 0)
                diagramAnalyze = Solver.DiagramAnalyze(diagram);

            callbackForm.ShowTextWindow("Анализ диаграммы", diagramAnalyze);
        }

        public void CreateGraphs(Parameters parameters, int minPlaceCount, int maxPlaceCount)
        {
            string folder = callbackForm.GetImagePathFolder("Выберите папку для сохранения графиков");

            if (folder == "")
                return;

            string textFolder = callbackForm.GetImagePathFolder("Выберите папку для сохранения отчетов");

            if (textFolder == "")
                return;

            var graphNames = Solver.GenerateGraphNames(parameters.channelCount, maxPlaceCount);

            int totalGraphCount = graphNames.Length;

            var analyzers = new List<DiagramAnalyzer>();
            var graphs = new IGraph[totalGraphCount];

            for (int i = 0; i < totalGraphCount; i++)
            {
                var oxyGraph = callbackForm.CreateGraph();

                oxyGraph.SetTitle(graphNames[i]);
                oxyGraph.AddLine("");

                graphs[i] = oxyGraph;
            }

            try
            {
                for(int k = minPlaceCount; k <= maxPlaceCount; k++)
                {
                    parameters.queueCapacity = k;
                    var diagram = callbackForm.CreateDiagram(parameters.channelCount, parameters.queueCapacity);

                    Solver.FillDiagram(parameters, diagram);
                    Solver.AddPointsToGraph(graphs, maxPlaceCount, k, diagram);

                    var analyzeText = Solver.DiagramAnalyze(diagram);
                    File.WriteAllText(textFolder + "/Отчет для кол-ва мест " + k + ".txt", analyzeText);
                }
            }
            catch(Exception ex)
            {
                callbackForm.ShowError("Ошибка вычислений", ex.Message);
                return;
            }

            for (int i = 0; i < totalGraphCount; i++)
            {
                graphs[i].CompleteLine();

                var bitmap = graphs[i].ExportToBitmap();

                bitmap.Save(folder + "/" + graphNames[i] + ".png");
            }
        }
    }
}