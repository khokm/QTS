namespace QTS.Core.Diagram
{
    /// <summary>
    /// Простая линия
    /// </summary>
    struct Line
    {
        public double start, end;

        public Line(double start, double end)
        {
            this.start = start;
            this.end = end;
        }
    }
}
