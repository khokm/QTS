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

        public PlotModel plotModel => graph.PlotModel;

        public OxyPlotDiagram(int channelCount, int queueCapacity) : base(channelCount, queueCapacity)
        {
            graph = new OxyPlotGraph();
        }

        protected override void AddPathPoint(double y, double x)
        {
            graph.AddPoint(y, x);
        }


        protected override void AddChannelRndMetadata(double realRndValue, double rndValue)
        {
            graph.CurrentLine.TrackerFormatString += $"t (время обслуживания): { (float)rndValue } (реал. знач. ГСЧ: { (float)realRndValue });";
        }

        protected override void OnPathStarted(double y, double x, double realRndValue, double rndValue)
        {
            graph.BeginLine();

            graph.CurrentLine.TrackerFormatString = $"t (время, прошедшее с появления предыдущей заявки): { (float)rndValue } (реал. знач. ГСЧ: { (float)realRndValue });\n";

            AddAnnotation(y, x, VerticalAlignment.Bottom);
        }

        protected override void OnPathFinished(double y, double x)
        {
            int clientNum = SummaryClientCount + 1;

            graph.CurrentLine.Title = clientNum.ToString();
            graph.CurrentLine.TrackerFormatString = $"Заявка { clientNum }.\n{ graph.CurrentLine.TrackerFormatString }";
            graph.CurrentLine.MouseDown += OnMouseDown;

            AddAnnotation(y, x, VerticalAlignment.Top);
            graph.CompleteLine();
        }

        protected override void PostProcessDiagram()
        {
            var xAxis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                MinimumRange = 0.001,
                MaximumRange = 100000,

                //AbsoluteMinimum = 0
            };

            var yAxis = new CategoryAxis()
            {
                Position = AxisPosition.Left,
                MinimumRange = Math.Min(10, TopY * 2),
                MaximumRange =  TopY * 2,

                AbsoluteMinimum = -5,
                AbsoluteMaximum = TopY * 2,

                //Делаем горизонтальные линии пунктирными
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,

                //Делаем так, чтобы боковые подписи были напротив линий
                IsTickCentered = true
            };

            //Расставляем боковые подписи
            yAxis.ActualLabels.Add("Отказ");
            yAxis.ActualLabels.Add("Обслужено");

            for (int i = 0; i < QueueCapacity; i++)
                yAxis.ActualLabels.Add($"Место { QueueCapacity - i }");

            for (int i = 0; i < ChannelCount; i++)
                yAxis.ActualLabels.Add($"Канал { ChannelCount - i }");

            yAxis.ActualLabels.Add("Заявки");

            graph.PlotModel.Axes.Insert(0, xAxis);
            graph.PlotModel.Axes.Insert(1, yAxis);
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

        /// <summary>
        /// Создает новую подпись над точкой.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <param name="textAlligment">Положение надписи относительно точки</param>
        void AddAnnotation(double y, double x, VerticalAlignment textAlligment)
        {
            var pointAnnotation1 = new PointAnnotation()
            {
                X = x,
                Y = y,
                Text = (graph.PlotModel.Series.Count + 1).ToString(),
                TextVerticalAlignment = textAlligment
            };

            pointAnnotation1.MouseDown += OnMouseDown;

            graph.PlotModel.Annotations.Add(pointAnnotation1);
        }

        /// <summary>
        /// Вызывается при нажатии на точку или линию диграммы.
        /// Используется для передвижения по диаграмме.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.IsControlDown)
            {
                if (sender is LineSeries)
                {
                    int lineIndex = int.Parse(((LineSeries)sender).Title) - 1;

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
