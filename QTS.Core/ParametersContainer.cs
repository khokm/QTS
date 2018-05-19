namespace QTS.Core
{
    /// <summary>
    /// Контейнер параметров для построения временной диграммы.
    /// </summary>
    public class ParametersContainer
    {
        public int ThreadIntencity { get; set; }
        public int QueueCapacity { get; set; }
        public double MinRandomValue { get; set; }
        public bool HasTimeLimit { get; }
        public double TimeLimit { get; set; }
        public bool HasClientLimit { get; }
        public int ClientLimit { get; set; }
        public bool PreferFirstChannel { get; set; }
        public int[] ChannelsIntencites { get; set; }

        public int ChannelCount => ChannelsIntencites.Length;

        /// <summary>
        /// Создает новый контейнер параметров.
        /// </summary>
        /// <param name="threadIntencity">Интенсивность потока</param>
        /// <param name="queueCapacity">Количество стояночных мест</param>
        /// <param name="minRandomValue">Минимальное значение, выдаваемое ГСЧ</param>
        /// <param name="hasTimeLimit">Использует ли диаграмма ограничение по времени?</param>
        /// <param name="timeLimit">Ограничение по времени</param>
        /// <param name="hasClientLimit">Использует ли диаграмма ограничение по количеству заявок?</param>
        /// <param name="clientLimit">Ограничение по количеству заявок</param>
        /// <param name="preferFirstChannel">Предпочитать первый канал?</param>
        /// <param name="channelsIntencies">Массив пропускных способностей каналов</param>
        public ParametersContainer(int threadIntencity, int queueCapacity,
            double minRandomValue, bool hasTimeLimit,
            double timeLimit, bool hasClientLimit,
            int clientLimit, bool preferFirstChannel, int[] channelsIntencies)
        {
            ThreadIntencity = threadIntencity;
            QueueCapacity = queueCapacity;
            MinRandomValue = minRandomValue;
            HasTimeLimit = hasTimeLimit;
            TimeLimit = timeLimit;
            HasClientLimit = hasClientLimit;
            ClientLimit = clientLimit;
            PreferFirstChannel = preferFirstChannel;
            ChannelsIntencites = channelsIntencies;
        }

        public override string ToString()
        {
            string text = "Параметры системы:\r\n";

            text += $" Интенсивность потока: {ThreadIntencity} шт/ч\r\n";

            text += $" Количество стояночных мест: {QueueCapacity}\r\n";

            if (MinRandomValue != 0)
                text += $" Минимальное значение ГСЧ: {MinRandomValue}\r\n";

            if (HasTimeLimit)
                text += $" Ограничение по времени: {TimeLimit} ч\r\n";

            if (HasClientLimit)
                text += $" Ограничение по количеству заявок: {ClientLimit} шт\r\n";

            if (!PreferFirstChannel)
                text += " Не предпочитать первый канал\r\n";

            for (int i = 0; i < ChannelsIntencites.Length; i++)
                text += string.Format(" Интенсивность {0} канала: {1} шт/ч\r\n", i + 1, ChannelsIntencites[i]);

            return text;
        }
    }
}
