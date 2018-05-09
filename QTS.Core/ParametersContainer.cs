namespace QTS.Core
{
    /// <summary>
    /// Контейнер параметров для построения временной диграммы.
    /// </summary>
    public class ParametersContainer
    {
        public int ThreadIntencity { get; set; }
        public int QueueCapacity { get; set; }
        public int[] ChannelsIntencity { get; set; }
        public double MinRandomValue { get; set; }
        public bool HasTimeLimit { get; }
        public double TimeLimit { get; set; }
        public bool HasClientLimit { get; }
        public int ClientLimit { get; set; }
        public bool PreferFirstChannel { get; set; }

        public int channelCount => ChannelsIntencity.Length;

        /// <summary>
        /// Создает новый контейнер параметров.
        /// </summary>
        /// <param name="threadIntencity">Интенсивность потока</param>
        /// <param name="queueCapacity">Количество стояночных мест</param>
        /// <param name="channelsIntencies">Массив пропускных способностей каналов</param>
        /// <param name="minRandomValue">Минимальное значение, выдаваемое ГСЧ</param>
        /// <param name="hasTimeLimit">Использует ли диаграмма ограничение по времени?</param>
        /// <param name="timeLimit">Ограничение по времени</param>
        /// <param name="hasClientLimit">Использует ли диаграмма ограничение по количеству заявок?</param>
        /// <param name="clientLimit">Ограничение по количеству заявок</param>
        /// <param name="preferFirstChannel">Предпочитать первый канал?</param>
        public ParametersContainer(int threadIntencity, int queueCapacity,
            int[] channelsIntencies, double minRandomValue,
            bool hasTimeLimit, double timeLimit,
            bool hasClientLimit, int clientLimit, bool preferFirstChannel)
        {
            ThreadIntencity = threadIntencity;
            QueueCapacity = queueCapacity;
            ChannelsIntencity = channelsIntencies;
            MinRandomValue = minRandomValue;
            HasTimeLimit = hasTimeLimit;
            TimeLimit = timeLimit;
            HasClientLimit = hasClientLimit;
            ClientLimit = clientLimit;
            PreferFirstChannel = preferFirstChannel;
        }
    }
}
