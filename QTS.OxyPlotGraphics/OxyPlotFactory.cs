using System;
using QTS.Core;
using System.Collections.Generic;

namespace QTS.OxyPlotGraphics
{
    /// <summary>
    /// Простая реализация фабрики графических элементов.
    /// </summary>
    public class OxyPlotFactory : IGraphicsFactory<OxyPlotDiagram, OxyPlotGraph>
    {
        public OxyPlotGraph CreateEmptyGraph(string XAxis = "", string YAxis = "")
        {
            return new OxyPlotGraph(XAxis, YAxis);
        }

        public OxyPlotGraph CreateEmptyGraph(int minX, IEnumerable<double> yValues, string XAxis = "", string YAxis = "")
        {
            return new OxyPlotGraph(minX, yValues, XAxis, YAxis);
        }

        public OxyPlotDiagram CreateEmptyDiagram(int channelCount, int queueCapacity)
        {
            return new OxyPlotDiagram(channelCount, queueCapacity);
        }

    }
}
