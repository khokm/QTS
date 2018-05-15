using System;

namespace QTS.Core.Helpers
{
    /// <summary>
    /// Генератор случайных чисел
    /// </summary>
    class RandomGenerator
    {
        Random Rnd { get; }

        double MinValue { get; }

        public RandomGenerator(double minValue)
        {
            Rnd = new Random();
            MinValue = minValue;
        }

        /// <summary>
        /// Возвращает -1/intencity*ln(rnd);
        /// </summary>
        /// <param name="intencity"></param>
        /// <returns></returns>
        public double Next(int intencity)
        {
            int stackMaximum = 0;

            double value = 0;
            do
            {
                if (stackMaximum > 1000)
                    throw new Exception(
                        "Минмальный интервал: слишком большое значение."
                        );

                value = -Math.Log(Rnd.NextDouble()) / intencity;
                stackMaximum++;
            }
            while (value < MinValue);

            return value;
        }
    }
}
