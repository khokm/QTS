namespace QTS.Core
{
    public class QueuePlaceGradientData
    {
        public int MinQueueCapacity { get; }
        public int MaxQueueCapacity { get; }

        public QueuePlaceGradientData(int minPlaceCount, int maxPlaceCount)
        {
            MinQueueCapacity = minPlaceCount;
            MaxQueueCapacity = maxPlaceCount;
        }
    }
}
