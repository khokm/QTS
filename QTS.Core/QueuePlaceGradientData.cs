namespace QTS.Core
{
    public class QueuePlaceGradientData
    {
        public int MinPlaceCount { get; }
        public int MaxPlaceCount { get; }

        public QueuePlaceGradientData(int minPlaceCount, int maxPlaceCount)
        {
            MinPlaceCount = minPlaceCount;
            MaxPlaceCount = maxPlaceCount;
        }
    }
}
