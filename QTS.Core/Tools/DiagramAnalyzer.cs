using QTS.Core.Diagram;

namespace QTS.Core.Tools
{
    class DiagramAnalyzer
    {
        IAnalyzable diagram { get; }
        public DiagramAnalyzer(IAnalyzable diagram)
        {
            this.diagram = diagram;
        }

        public int totalValues
        {
            get
            {
                return 9 + diagram.channelCount * 2 + diagram.queueCapacity;
            }
        }

        public float servedProbality
        {
            get
            {
                if (diagram.clientsCount == 0)
                    return 0;

                return (float)diagram.clientsServed / diagram.clientsCount;
            }
        }

        public float systemThroughput
        {
            get
            {
                if (diagram.systemWorkTime == 0)
                    return 0;

                return (float)(diagram.clientsServed / diagram.systemWorkTime);
            }
        }

        public float lostProbality
        {
            get
            {
                if (diagram.clientsCount == 0)
                    return 0;

                return (float)diagram.clientsLost / diagram.clientsCount;
            }
        }

        //Вероятность P занятости только n каналов, где P = arr[n - 1]

        //_cbp - кеш объекта, он нужен, чтобы не вызывать
        //GetChannelintersectionLength, требующую много ресурсов
        float[] _cbp;
        //Используется для определения, не изменялась ли диаграмма
        int _clientsCount = -1;
        public float[] channelBusyProbality
        {
            get
            {
                if (_cbp != null && diagram.clientsCount == _clientsCount)
                    return _cbp;

                _clientsCount = diagram.clientsCount;
                _cbp = new float[diagram.channelCount];

                if (diagram.systemWorkTime == 0)
                    return _cbp;

                for (int i = 0; i < _cbp.Length; i++)
                {
                    _cbp[i] = (float)
                        (
                        diagram.GetChannelIntersectionLength(i + 1) / 
                        diagram.systemWorkTime
                        );
                }

                return _cbp;
            }
        }

        public float averageBusyChannelCount
        {
            get
            {
                float value = 0;

                for(int i = 0; i < diagram.channelCount; i++)
                    value += (i + 1) * channelBusyProbality[i];

                return value;
            }
        }

        //Вероятность P простоя хотя бы n каналов, где P = arr[n - 1]
        public float[] channelIdleProbality
        {
            get
            {
                float[] cip = new float[diagram.channelCount];

                if (diagram.systemWorkTime == 0)
                    return cip;

                float totalProb = (float)
                    (
                    diagram.GetChannelIntersectionLength(0) /
                    diagram.systemWorkTime
                    );

                cip[diagram.channelCount - 1] = totalProb;

                for (int i = diagram.channelCount - 2; i >= 0 ; i--)
                {
                    float channelBusyProb = 
                        channelBusyProbality[diagram.channelCount - 2 - i];

                    cip[i] = channelBusyProb + totalProb;

                    totalProb += channelBusyProb;
                }

                return cip;
            }
        }

        public float averageClientCountInQueue
        {
            get
            {
                float value = 0;

                var probs = queueBusyProbality;

                for(int i = 0; i < diagram.queueCapacity; i++)
                    value += (i + 1) * probs[i];

                return value;
            }
        }

        //Вероятность P того, что в очереди будет только n заявок,
        //где P = arr[n - 1]
        public float[] queueBusyProbality
        {
            get
            {
                float[] probs = new float[diagram.queueCapacity];

                if (diagram.systemWorkTime == 0)
                    return probs;

                for (int i = 0; i < diagram.queueCapacity; i++)
                    probs[i] = (float)
                        (
                        diagram.queueBusyTime[i] / 
                        diagram.systemWorkTime
                        );

                return probs;
            }
        }

        public float averageClientQueueWaitingTime
        {
            get
            {
                if (diagram.queueClientCount == 0)
                    return 0;

                return 
                    (float)
                    (diagram.queueWaitingTime / diagram.queueClientCount);
            }
        }

        public float averageClientServiceTime
        {
            get
            {
                if (diagram.clientsServed == 0)
                    return 0;

                return (float)diagram.serviceTime / diagram.clientsServed;
            }
        }

        public float averageClientTime
        {
            get
            {
                return 
                    averageClientQueueWaitingTime + 
                    averageClientServiceTime;
            }
        }

        public float averageClientCountInSystem
        {
            get
            {
                double step = diagram.systemWorkTime / 100;

                double currentTime = diagram.systemStartTime - step / 2;

                int clientCount = 0;

                for (int i = 0; i < 100; i++)
                {
                    currentTime += step;

                    int c = diagram.GetClientCountAtTime(currentTime, step);
                    clientCount += c;
                }

                return (float)clientCount / 100;
            }
        }
    }
}
