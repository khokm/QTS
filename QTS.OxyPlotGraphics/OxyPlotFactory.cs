using QTS.Core;

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

        public OxyPlotDiagram CreateEmptyDiagram(int channelCount, int queueCapacity)
        {
            return new OxyPlotDiagram(channelCount, queueCapacity);
        }
    }
}
