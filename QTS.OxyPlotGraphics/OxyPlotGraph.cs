using System.Drawing;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using QTS.Core;
using OxyPlot.Axes;

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
        }

        public void CompleteLine()
        {
            PlotModel.Series.Add(CurrentLine);
        }

        public Bitmap ExportToBitmap()
        {
            PngExporter exp = new PngExporter();
            return exp.ExportToBitmap(PlotModel);
        }
    }
}
