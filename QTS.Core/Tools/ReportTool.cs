using System.Collections.Generic;
using System.Linq;
using QTS.Core.Diagram;

namespace QTS.Core.Tools
{
    static class ReportTool
    {
        public static string MakeReport(IAnalyzable diagram, IEnumerable<Metric> clientMetrics, IEnumerable<Metric> metrics) =>
            "Показатели системы:\r\n" + string.Join("\r\n", clientMetrics.Select(metric => string.Format(" {0}: {1} {2}", metric.Name, metric.Formula(diagram), metric.Units))) +
            "\r\n\r\nАнализ диаграммы:\r\n" + string.Join("\r\n", metrics.Select(metric => string.Format(" {0}: {1} {2}", metric.Name, metric.Formula(diagram), metric.Units)));

        public static IEnumerable<double> GetHeights(IEnumerable<IAnalyzable> diagrams, Metric metric) =>
            diagrams.Select(diagram => (double)metric.Formula(diagram));
       
    }
}
