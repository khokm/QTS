using QTS.Core.Diagram;
using QTS.Core.Helpers;

namespace QTS.Core.Tools
{
    class DiagramCreator
    {
        int[] channelsIntencity { get; }

        int channelCount{ get { return channelsIntencity.Length; } }
        int parksCount { get { return parkFreeTime.Length; } }

        double[] channelFreeTime { get; }
        double[] parkFreeTime { get; }

        IDiagram timeDiagram { get; }

        bool preferFirstChannel { get; }

        public DiagramCreator(int parkCount, int[] channelsIntencity, 
            bool preferFirstChannel, IDiagram diagram)
        {
            this.preferFirstChannel = preferFirstChannel;
            this.channelsIntencity = channelsIntencity;

            channelFreeTime = new double[channelCount];
            parkFreeTime = new double[parkCount];

            for (int i = 0; i < channelCount; i++)
                channelFreeTime[i] = -1;

            for (int i = 0; i < parkCount; i++)
                parkFreeTime[i] = -1;

            timeDiagram = diagram;
        }

        int GetParkingPlaceIndex(double arrivalTime)
        {
            for (int i = 0; i < parksCount; i++)
            {
                if (parkFreeTime[i] < arrivalTime)
                    return i;
            }

            return -1;
        }

        int GetNextPossibleChannel(double arrivalTime)
        {
            if (channelCount == 0)
                return -1;

            int minIndex = 0;
            double minimumValue = channelFreeTime[0];
            for (int i = 0; i < channelCount; i++)
            {
                if (preferFirstChannel && channelFreeTime[i] < arrivalTime)
                    return i;

                if (channelFreeTime[i] < minimumValue)
                {
                    minimumValue = channelFreeTime[i];
                    minIndex = i;
                }
            }

            return minIndex;
        }

        void GoToChannel(int channelIndex, double arrivalTime, 
            RandomGenerator rnd)
        {
            double clientSerivceTime = rnd.Next
                (
                channelsIntencity[channelIndex]
                );

            double serviceStopTime = arrivalTime + clientSerivceTime;
            channelFreeTime[channelIndex] = serviceStopTime;

            timeDiagram.PushChannelLine
                (
                channelIndex, arrivalTime, serviceStopTime
                );
            timeDiagram.PushServedPoint(serviceStopTime);
        }

        public void PushClient(double arrivalTime, RandomGenerator rnd)
        {
            timeDiagram.PushStartPoint(arrivalTime);

            int usingChannel = GetNextPossibleChannel(arrivalTime);

            if (usingChannel == -1)
            {
                timeDiagram.PushBreakPoint(arrivalTime);
                return;
            }

            if (channelFreeTime[usingChannel] < arrivalTime)
            {
                GoToChannel(usingChannel, arrivalTime, rnd);
                return;
            }

            int parkIndex = GetParkingPlaceIndex(arrivalTime);

            if (parkIndex == -1)
            {
                timeDiagram.PushBreakPoint(arrivalTime);
                return;
            }

            double parkStart = arrivalTime;

            timeDiagram.AddQueueClient();

            for (int i = parkIndex; i >= 0; i--)
            {
                double parkEnd;
                if (i != 0)
                    parkEnd = parkFreeTime[i - 1];
                else
                    parkEnd = channelFreeTime[usingChannel];

                parkFreeTime[i] = parkEnd;

                timeDiagram.PushParkLine(i, parkStart, parkEnd);
                parkStart = parkEnd;
            }

            GoToChannel(usingChannel, parkFreeTime[0], rnd);
        }
    }
}