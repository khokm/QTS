using System.Drawing;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using QTS.Core;

namespace QTS.OxyPlotGraphics
{
    public class OxyPlotGraph : IGraph
    {
        public PlotModel plotModel { get; }
        public LineSeries currentLine { get; private set; }

        public OxyPlotGraph()
        {
            plotModel = new PlotModel()
            {
                IsLegendVisible = false
            };
        }

        public void AddPoint(double y, double x)
        {
            currentLine.Points.Add(new DataPoint(x, y));
        }

        public void SetTitle(string name)
        {
            plotModel.Title = name;
        }

        public void AddLine(string name)
        {
            currentLine = new LineSeries()
            {
                Title = name
            };
            currentLine.LineStyle = LineStyle.Solid;
        }

        public void CompleteLine()
        {
            plotModel.Series.Add(currentLine);
        }

        public Bitmap ExportToBitmap()
        {
            PngExporter exp = new PngExporter();
            return exp.ExportToBitmap(plotModel);
        }
    }
}
