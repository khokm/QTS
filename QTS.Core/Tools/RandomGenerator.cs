using System;

namespace QTS.Core.Tools
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
        /// <param name="realValue">Реальное значение гсч.</param>
        /// <returns></returns>
        public double Next(int intencity, out double realValue)
        {
            int stackMaximum = 0;

            double value;
            double realVal;
            do
            {
                if (stackMaximum > 1000)
                    throw new Exception(
                        "Минмальный интервал: слишком большое значение."
                        );

                realVal = Rnd.NextDouble();
                value = -Math.Log(realVal) / intencity;
                stackMaximum++;
            }
            while (value < MinValue);

            realValue = realVal;
            return value;
        }
    }
}
