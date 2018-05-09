using System;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using QTS.Core;

namespace QTS.OxyPlotGraphics
{
    /// <summary>
    /// Реализация временной диаграммы с использованием библиотеки OxyPlot.
    /// </summary>
    public class OxyPlotDiagram : TimeDiagram
    {
        OxyPlotGraph graph;

        public PlotModel plotModel
        {
            get
            {
                if (!Completed)
                    throw new Exception("Диаграмма не завершена!");

                return graph.PlotModel;
            }
        }

        public OxyPlotDiagram(int channelCount, int queueCapacity) : base(channelCount, queueCapacity)
        {
            graph = new OxyPlotGraph();
        }

        protected override void AddVisualPoint(double y, double x)
        {
            graph.AddPoint(y, x);
        }

        protected override void OnPathStarted(double y, double x)
        {
            graph.StartLine("Заявка " + (SummaryClientCount + 1));

            graph.CurrentLine.MouseDown += LineMouseDown;

            AddAnnotation(y, x, VerticalAlignment.Bottom);
        }

        protected override void OnPathFinished(double y, double x)
        {
            AddAnnotation(y, x, VerticalAlignment.Top);
            graph.CompleteLine();
        }

        protected override void OnDiagramFinished()
        {
            var categoryAxis = new CategoryAxis()
            {
                Position = AxisPosition.Left,

                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,

                IsTickCentered = true,

                MinimumRange = Math.Min(10, TopY * 2),
                MaximumRange = TopY * 2,
                AbsoluteMinimum = -5,
                AbsoluteMaximum = TopY * 2,
            };

            categoryAxis.ActualLabels.Add("Отказ");
            categoryAxis.ActualLabels.Add("Обслужено");

            for (int i = 0; i < QueueCapacity; i++)
                categoryAxis.ActualLabels.Add("Место " + (QueueCapacity - i));

            for (int i = 0; i < ChannelCount; i++)
                categoryAxis.ActualLabels.Add("Канал " + (ChannelCount - i));

            categoryAxis.ActualLabels.Add("Заявки");

            var xAxe = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                MinimumRange = 0.001,
                MaximumRange = 100000
            };

            graph.PlotModel.Axes.Add(categoryAxis);
            graph.PlotModel.Axes.Add(xAxe);
        }

        protected override void UpdateView(int visibleLineIndex)
        {
            var series = plotModel.Series;

            if (ShowPreviousLines)
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
                Text = (graph.PlotModel.Series.Count + 1).ToString(),
                TextVerticalAlignment = textAlligment
            };

            pointAnnotation1.MouseDown += LineMouseDown;

            graph.PlotModel.Annotations.Add(pointAnnotation1);
        }

        void LineMouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.IsControlDown)
            {
                if (sender is LineSeries)
                {
                    int lineIndex = int.Parse(((LineSeries)sender).Title.Remove(0, 7)) - 1;

                    SetVisibleLinesCount(lineIndex);
                }
                else
                {
                    int lineIndex = int.Parse(((PointAnnotation)sender).Text) - 1;

                    SetVisibleLinesCount(lineIndex);
                }
            }
        }
    }
}
