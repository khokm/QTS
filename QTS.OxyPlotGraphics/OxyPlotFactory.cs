using QTS.Core.Graphics;
using System.Collections.Generic;

namespace QTS.OxyPlotGraphics
{
    /// <summary>
    /// Простая реализация фабрики графических элементов.
    /// </summary>
    public class OxyPlotFactory : IGraphicsFactory<OxyPlotGraph, OxyPlotGraph>
    {
        public OxyPlotGraph CreateEmptyGraph(int minX, IEnumerable<double> yValues, string XAxis, string YAxis)
        {
            return new OxyPlotGraph(minX, yValues, XAxis, YAxis, 0, 1, 1, 1, null);
        }

        //public OxyPlotDiagram CreateEmptyDiagram(int channelCount, int queueCapacity)
        //{
        //    return new OxyPlotDiagram(channelCount, queueCapacity);
        //}

        public OxyPlotGraph CreateEmptyDiagram(IEnumerable<string> yLabels)
        {
            return new OxyPlotGraph("Время (ч)", "", double.MinValue, 0, 0, 0, yLabels);
        }
    }
}
