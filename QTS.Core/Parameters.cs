namespace QTS.Core
{
    public class Parameters
    {
        public int[] channelsIntencity { get; set; }
        public int maxClients { get; set; }
        public double maxTime { get; set; }
        public double minRandomValue { get; set; }
        public int queueCapacity { get; set; }
        public bool preferFirstChannel { get; set; }
        public int threadIntencity { get; set; }

        public bool hasClientLimit { get; }
        public bool hasTimeLimit { get; }

        public int channelCount
        {
            get
            {
                return channelsIntencity.Length;
            }
        }

        public Parameters(int threadIntencity, int parkCount,
            int[] channelsIntencity, double maxTime,
            int maxClients, double minRandomValue,
            bool preferFirstChannel, bool clientsLimit, bool timeLimit)
        {
            this.channelsIntencity = channelsIntencity;
            this.maxClients = maxClients;
            this.maxTime = maxTime;
            this.minRandomValue = minRandomValue;
            this.queueCapacity = parkCount;
            this.preferFirstChannel = preferFirstChannel;
            this.threadIntencity = threadIntencity;

            this.hasClientLimit = clientsLimit;
            this.hasTimeLimit = timeLimit;
        }

    }
}
