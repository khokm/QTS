using System.Drawing;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using QTS.Core;
using OxyPlot.Axes;
using System.Collections.Generic;
using System;

namespace QTS.OxyPlotGraphics
{
    /// <summary>
    /// Реализация графика с использованием библиотеки OxyPlot.
    /// </summary>
    public class OxyPlotGraph : IGraph
    {
        /// <summary>
        /// 
        /// </summary>
        public PlotModel PlotModel { get; }
        public LineSeries CurrentLine { get; private set; }

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

        public string Title
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

        public OxyPlotGraph(string XAxis = "", string YAxis = "")
        {
            PlotModel = new PlotModel()
            {
                IsLegendVisible = false,
            };

            if (XAxis != "")
            {
                var xAxis = new LinearAxis()
                {
                    Position = AxisPosition.Bottom,
                    AbsoluteMinimum = 0,
                    Minimum = 0,
                    MinimumRange = 1,
                    MinimumMinorStep = 1,
                    MinimumMajorStep = 1,
                    MajorGridlineStyle = LineStyle.Solid,
                    //MaximumRange = 1,
                    Title = XAxis
                };

                PlotModel.Axes.Insert(0, xAxis);
            }

            if (YAxis != "")
            {
                var yAxis = new LinearAxis()
                {
                    Position = AxisPosition.Left,
                    AbsoluteMinimum = 0,
                    Minimum = 0,
                    MinimumRange = 1,
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dash,
                    //Title = YAxis
                };

                PlotModel.Axes.Insert(1, yAxis);
            }

        }

        public void AddPoint(double y, double x)
        {
            CurrentLine.Points.Add(new DataPoint(x, y));
        }

        public void BeginLine()
        {
            CurrentLine = new LineSeries()
            {
                LineStyle = LineStyle.Solid
            };
            CurrentLine.TrackerFormatString = "";
        }

        public void CompleteLine(bool randomColor = true)
        {
            if(randomColor)
                CurrentLine.Color = DefaultColors[_currentColorIndex++ % DefaultColors.Count];
            PlotModel.Series.Add(CurrentLine);
        }

        public Bitmap ExportToBitmap(bool betterHeights)
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

            PngExporter exp = new PngExporter();
            return exp.ExportToBitmap(PlotModel);
        }
    }
}
