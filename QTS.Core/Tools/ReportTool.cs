using System.Collections.Generic;
using System.Linq;

namespace QTS.Core.Tools
{
    static class ReportTool
    {
        public static string MakeReport(DiagramData diagram, IEnumerable<Metric> metrics) =>
            "Анализ диаграммы:" + string.Join("\r\n", metrics.Select(metric => string.Format("{0}: {1} {2}", metric.Name, metric.Formula(diagram), metric.Units)));

        public static IEnumerable<T> CreateGraphs<T>(IEnumerable<DiagramData> diagrams, IEnumerable<Metric> metrics, QueuePlaceGradientData gradient, IGraphicsFactory<InteractiveDiagram, T> factory) where T : IGraph => 
            metrics.Select(metric => 
            factory.CreateEmptyGraph(gradient.MinQueueCapacity, diagrams.Select(diagram => 
            (double)metric.Formula(diagram)), "Кол-во мест в очереди", metric.Name));
       
    }
}
