using System.Drawing;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using QTS.Core;

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

        public OxyPlotGraph()
        {
            PlotModel = new PlotModel()
            {
                IsLegendVisible = false
            };
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
