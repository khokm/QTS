using System;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using QTS.Core;

namespace QTS.OxyPlotGraphics
{
    public class OxyPlotDiagram : TimeDiagram
    {
        OxyPlotGraph graph;

        public PlotModel plotModel
        {
            get
            {
                if (!diagramCompleted)
                    throw new Exception("Диаграмма не завершена!");

                return graph.plotModel;
            }
        }

        public OxyPlotDiagram(int channelCount, int queueCapacity) : 
            base(channelCount, queueCapacity)
        {
            graph = new OxyPlotGraph();
        }

        protected override void AddVisualPoint(double y, double x)
        {
            graph.AddPoint(y, x);
        }

        protected override void OnLineStarted(double y, double x)
        {
            graph.AddLine("Клиент " + (clientsCount + 1));

            graph.currentLine.MouseDown += LineMouseDown;

            AddAnnotation(y, x, VerticalAlignment.Bottom);
        }

        protected override void OnLineFinished(double y, double x)
        {
            AddAnnotation(y, x, VerticalAlignment.Top);
            graph.CompleteLine();
        }

        protected override void FinishVisualDiagram()
        {
            var categoryAxis = new CategoryAxis()
            {
                Position = AxisPosition.Left,

                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,

                IsTickCentered = true,

                MinimumRange = Math.Min(10, topY * 2),
                MaximumRange = topY * 2,
                AbsoluteMinimum = -5,
                AbsoluteMaximum = topY * 2,
            };

            categoryAxis.ActualLabels.Add("Отказ");
            categoryAxis.ActualLabels.Add("Обслужено");

            for (int i = 0; i < queueCapacity; i++)
                categoryAxis.ActualLabels.Add
                    (
                    "Место " + (queueCapacity - i)
                    );

            for (int i = 0; i < channelCount; i++)
                categoryAxis.ActualLabels.Add
                    (
                    "Канал " + (channelCount - i)
                    );

            categoryAxis.ActualLabels.Add("Заявки");

            var xAxe = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                MinimumRange = 0.001,
                MaximumRange = 100000
            };

            graph.plotModel.Axes.Add(categoryAxis);
            graph.plotModel.Axes.Add(xAxe);
        }

        protected override void UpdateVisualContent(int visibleLineIndex)
        {
            var series = plotModel.Series;

            if (showPreviousLines)
            {
                for (int i = 0; i < series.Count; i++)
                    series[i].IsVisible = i <= visibleLineIndex;
            }
            else
            {
                for (int i = 0; i < series.Count; i++)
                    series[i].IsVisible = i == visibleLineIndex;
            }
        }

        void AddAnnotation(double y, double x, VerticalAlignment textAlligment)
        {
            var pointAnnotation1 = new PointAnnotation()
            {
                X = x,
                Y = y,
                Text = (graph.plotModel.Series.Count + 1).ToString(),
                TextVerticalAlignment = textAlligment
            };

            pointAnnotation1.MouseDown += LineMouseDown;

            graph.plotModel.Annotations.Add(pointAnnotation1);
        }

        void LineMouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.IsControlDown)
            {
                if (sender is LineSeries)
                {
                    int lineIndex = int.Parse
                        (
                        ((LineSeries)sender).Title.Remove(0, 7)
                        ) - 1;

                    SetDiagramVisibility(lineIndex);
                }
                else
                {
                    int lineIndex = int.Parse
                        (
                        ((PointAnnotation)sender).Text
                        ) - 1;

                    SetDiagramVisibility(lineIndex);
                }
            }
        }
    }
}
