using System;

namespace QTS.Core.Diagram
{
    /// <summary>
    /// Представляет набор параметров системы.
    /// </summary>
    [Serializable]
    public class ParametersContainer
    {
        /// <summary>
        /// Интенсивность потока заявок.
        /// </summary>
        public int ThreadIntencity { get; set; }
        /// <summary>
        /// Количество стояночных мест.
        /// </summary>
        public int QueueCapacity { get; set; }
        /// <summary>
        /// Минимальное значение, выдаваемое ГСЧ.
        /// </summary>
        public double MinRandomValue { get; set; }
        /// <summary>
        /// Использует ли диаграмма ограничение по времени?
        /// </summary>
        public bool HasTimeLimit { get; set; }
        /// <summary>
        /// Ограничение по времени.
        /// </summary>
        public double TimeLimit { get; set; }
        /// <summary>
        /// Использует ли диаграмма ограничение по количеству заявок?
        /// </summary>
        public bool HasClientLimit { get; set; }
        /// <summary>
        /// Ограничение по количеству заявок.
        /// </summary>
        public int ClientLimit { get; set; }
        /// <summary>
        /// Предпочитать первый канал?
        /// </summary>
        public bool PreferFirstChannel { get; set; }
        /// <summary>
        /// Пропускная способность (I) канала, где I = <see cref="ChannelsIntencites"/>[номер канала - 1].
        /// </summary>
        public int[] ChannelsIntencites { get; set; }

        /// <summary>
        /// Количество каналов в системе.
        /// </summary>
        public int ChannelCount => ChannelsIntencites.Length;

        ParametersContainer()
        {

        }

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
        /// <param name="channelsIntencies">Пропускная способность (I) канала, где I = <see cref="channelsIntencies"/>[номер канала - 1].</param>
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

        /// <summary>
        /// Переводит набор параметров в строковый вид, пригодный для чтения.
        /// </summary>
        /// <returns></returns>
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
