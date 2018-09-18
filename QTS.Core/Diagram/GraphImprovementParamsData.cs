namespace QTS.Core.Diagram
{
    public class GraphImprovementParamsData
    {
        public int MetricIndex { get; }

        public int ExperimentCount { get; }

        public GraphImprovementParamsData(int metricIndex, int experimentCount)
        {
            MetricIndex = metricIndex;
            ExperimentCount = experimentCount;
        }
    }
}
