using System.Collections.Generic;
using System.Linq;
using QTS.Core.Graphics;
using QTS.Core.Diagram;

namespace QTS.Core.Tools
{
    static class ReportTool
    {
        public static string MakeReport(IAnalyzable diagram, IEnumerable<Metric> clientMetrics, IEnumerable<Metric> metrics) =>
            "Показатели системы:\r\n" + string.Join("\r\n", clientMetrics.Select(metric => string.Format(" {0}: {1} {2}", metric.Name, metric.Formula(diagram), metric.Units))) +
            "\r\n\r\nАнализ диаграммы:\r\n" + string.Join("\r\n", metrics.Select(metric => string.Format(" {0}: {1} {2}", metric.Name, metric.Formula(diagram), metric.Units)));

        public static IEnumerable<T> CreateGraphs<T>(IEnumerable<IAnalyzable> diagrams, IEnumerable<Metric> metrics, QueuePlaceGradientData gradient, IGraphicsFactory<InteractiveDiagram, T> factory) where T : IGraph => 
            metrics.Select(metric => 
            factory.CreateEmptyGraph(gradient.MinQueueCapacity, diagrams.Select(diagram => 
            (double)metric.Formula(diagram)), "Кол-во мест в очереди", metric.Name));
       
    }
}
