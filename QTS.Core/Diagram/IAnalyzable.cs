namespace QTS.Core.Diagram
{
    interface IAnalyzable
    {
        int channelCount { get; }
        int queueCapacity { get; }

        int clientsCount { get; }
        int clientsServed { get; }
        int clientsLost { get; }

        double systemStartTime { get; }
        double systemStopTime { get; }
        double systemWorkTime { get; }

        double[] queueBusyTime { get; }

        double queueWaitingTime { get; }
        double serviceTime { get; }

        int queueClientCount { get; }

        double GetChannelIntersectionLength(int channelCount);

        int GetClientCountAtTime(double point, double step);
    }
}