using QTS.Core;

namespace QTS.OxyPlotGraphics
{
    /// <summary>
    /// Простая реализация фабрики графических элементов.
    /// </summary>
    public class OxyPlotFactory : IGraphicsFactory<OxyPlotDiagram>
    {
        public IGraph CreateEmptyGraph()
        {
            return new OxyPlotGraph();
        }

        public OxyPlotDiagram CreateEmptyDiagram(int channelCount, int queueCapacity)
        {
            return new OxyPlotDiagram(channelCount, queueCapacity);
        }
    }
}
