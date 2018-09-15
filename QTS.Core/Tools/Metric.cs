namespace QTS.Core.Tools
{
    enum MetricType
    {
        Integer,
        Float,
        Probality
    }
    delegate float Formula(DiagramData diagram);

    struct Metric
    {
        public Formula Formula { get; }
        public string Name { get; }
        public string Units { get; }
        public MetricType Type { get; }

        public Metric(string name, string units, Formula formula, MetricType type)
        {
            Formula = formula;
            Name = name;
            Units = units;
            Type = type;
        }
    }
}
