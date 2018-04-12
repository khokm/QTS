namespace QTS.Core.Diagram
{
    interface IDiagram
    {
        void PushStartPoint(double arrivalTime);

        void PushChannelLine(int channelIndex, double start, double end);

        void AddQueueClient();

        void PushParkLine(int parkIndex, double start, double end);

        void PushServedPoint(double completeTime);

        void PushBreakPoint(double breakTime);

        void CompleteDiagram();
    }
}
