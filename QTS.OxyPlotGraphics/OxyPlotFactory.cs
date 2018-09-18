using QTS.Core.Graphics;
using System.Collections.Generic;

namespace QTS.OxyPlotGraphics
{
    /// <summary>
    /// Простая реализация фабрики графических элементов.
    /// </summary>
    public class OxyPlotFactory : IGraphicsFactory<OxyPlotGraph, OxyPlotGraph>
    {
        public OxyPlotGraph CreateGraphByPoints(int startX, IEnumerable<double> yValues, string XAxis, string YAxis)
        {
            var graph = new OxyPlotGraph(XAxis, YAxis, 0, 0, 1, 1, 1, null);
            graph.CreateLineByPoints(yValues, startX);
            return graph;
        }

        public OxyPlotGraph CreateInteractiveGraph(string graphTitle)
        {
            return new OxyPlotGraph("Количество мест в очереди", graphTitle, double.MinValue, double.MinValue, 1, 1, 1, null);
        }

        public OxyPlotGraph CreateEmptyDiagram(IEnumerable<string> yLabels)
        {
            return new OxyPlotGraph("Время (ч)", "", double.MinValue, double.MinValue, 0, 0, 0, yLabels);
        }
    }
}
