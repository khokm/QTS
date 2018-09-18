using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using OxyPlot.Axes;
using System.Collections.Generic;
using System;
using System.Linq;
using QTS.Core.Graphics;
using OxyPlot.Annotations;

namespace QTS.OxyPlotGraphics
{
    /// <summary>
    /// Реализация графика с использованием библиотеки OxyPlot.
    /// </summary>
    public class OxyPlotGraph : InteractiveDiagram
    {
        public PlotModel PlotModel { get; }
        LineSeries currentLine;

        static int _currentColorIndex = 0;
        static List<OxyColor> DefaultColors = new List<OxyColor>()
        {
                 OxyColor.FromRgb(0x4E, 0x9A, 0x06),
                 OxyColor.FromRgb(0xC8, 0x8D, 0x00),
                 OxyColor.FromRgb(0xCC, 0x00, 0x00),
                 OxyColor.FromRgb(0x20, 0x4A, 0x87),
                 OxyColors.Red,
                 OxyColors.Orange,
                 OxyColors.Yellow,
                 OxyColors.Green,
                 OxyColors.Blue,
                 OxyColors.Indigo,
                 OxyColors.Violet
        };

        public override string Title
        {
            get
            {
                return PlotModel.Title;
            }

            set
            {
                PlotModel.Title = value;
            }
        }

        public OxyPlotGraph(string XAxisTitle, string YAxisTitle, double minimumX, double minumumY, double minimumXRange, double minimumMajorStep, double minimumMinorStep, IEnumerable<string> yLabels)
        {
            PlotModel = new PlotModel()
            {
                IsLegendVisible = false,
            };

            var xAxis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = minimumX,
                Minimum = 0,
                MinimumRange = minimumXRange,
                MinimumMinorStep = minimumMinorStep,
                MinimumMajorStep = minimumMajorStep,
                MajorGridlineStyle = LineStyle.Solid,
                //MaximumRange = 1,
                Title = XAxisTitle
            };

            Axis yAxis;

            if (yLabels != null)
            {
                var ax = new CategoryAxis()
                {
                    Position = AxisPosition.Left,
                    //Делаем горизонтальные линии пунктирными
                    MajorGridlineStyle = LineStyle.Dot,
                    MinorGridlineStyle = LineStyle.Dot,

                    //Делаем так, чтобы боковые подписи были напротив линий
                    IsTickCentered = true,
                };

                foreach (var label in yLabels.Reverse())
                    ax.ActualLabels.Add(label);

                ax.MaximumRange = ax.ActualLabels.Count * 2;
                ax.MinimumRange = Math.Min(10, ax.MaximumRange);

                yAxis = ax;
            }
            else
            {
                yAxis = new LinearAxis()
                {
                    Position = AxisPosition.Left,
                    AbsoluteMinimum = minumumY,
                    Minimum = 0,
                    MinimumRange = 1,
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dash,
                    //Title = YAxis
                };

                Title = YAxisTitle;

            }

            PlotModel.Axes.Insert(0, xAxis);
            PlotModel.Axes.Insert(1, yAxis);
        }

        public override void CreateLineByPoints(IEnumerable<double> yValues, double startX)
        {
            BeginLine();
            currentLine.Points.AddRange(yValues.Select(value => new DataPoint(startX++, value)));
            CompleteLine();
        }

        public override void AddPoint(double y, double x)
        {
            currentLine.Points.Add(new DataPoint(x, y));
        }

        protected override void CreateLine()
        {
            currentLine = new LineSeries()
            {
                LineStyle = LineStyle.Solid
            };
            currentLine.TrackerFormatString = "";
        }

        protected override void MakeLineInteractive(object lineKey, Action<object> onMouseDown)
        {
            currentLine.Tag = lineKey;
            currentLine.MouseDown += (object sender, OxyMouseDownEventArgs e) => { if(e.IsControlDown) onMouseDown(((LineSeries)sender).Tag); };
        }

        PointAnnotation CreateAnnotation(double y, double x, string annotation, bool atTop)
        {
            var pointAnnotation1 = new PointAnnotation()
            {
                X = x,
                Y = y,
                Text = annotation,
                TextVerticalAlignment = atTop ? VerticalAlignment.Bottom : VerticalAlignment.Top
            };
            PlotModel.Annotations.Add(pointAnnotation1);

            return pointAnnotation1;
        }

        public override void AddAnnotation(double y, double x, string annotation, bool atTop) => CreateAnnotation(y, x, annotation, atTop);

        protected override void CreateInteractiveAnnotation(double y, double x, string annotation, bool atTop, object annotationKey, Action<object> onMouseDown)
        {
            var point = CreateAnnotation(y, x, annotation, atTop);

            point.Tag = annotationKey;
            point.MouseDown += (object sender, OxyMouseDownEventArgs e) => { if (e.IsControlDown) onMouseDown(((PointAnnotation)sender).Tag); };
        }

        public override void CompleteLine(bool randomColor = false)
        {
            if(randomColor)
                currentLine.Color = DefaultColors[_currentColorIndex++ % DefaultColors.Count];
            PlotModel.Series.Add(currentLine);
        }

        public override void ExportToBitmap(bool betterHeights, string path)
        {
            if(betterHeights)
            {
                int max = 1;
                foreach (var line in PlotModel.Series)
                    foreach (var point in ((LineSeries)line).Points)
                        if (point.Y > max)
                            max = (int)Math.Ceiling(point.Y);

                PlotModel.Axes[1].MinimumRange = max;
            }

            PlotModel.Axes[0].Minimum = PlotModel.Series.Select(line => ((LineSeries)line).Points.Select(point => point.X)).Min().First();

            PngExporter exp = new PngExporter();
            exp.ExportToBitmap(PlotModel).Save(path);
        }

        public override void AddLineMetadata(string metadata) => currentLine.TrackerFormatString += metadata;

        public override void UpdateView(int visibleLineIndex)
        {
            var series = PlotModel.Series;

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
    }
}
