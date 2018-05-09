using System;

namespace QTS.Core.Helpers
{
    /// <summary>
    /// Генератор случайных чисел
    /// </summary>
    class RandomGenerator
    {
        Random rnd;

        double minValue;

        public RandomGenerator(double minValue)
        {
            rnd = new Random();
            this.minValue = minValue;
        }


        /// <summary>
        /// Возвращает -1/intencity*ln(rnd);
        /// </summary>
        /// <param name="intencity"></param>
        /// <returns></returns>
        public double Next(int intencity)
        {
            /*Эту строку можно раскомментить, и тогда эффект случайности пропадет,
             * а точность результатов анализа будет 100%-ная.
             */

            //return 1.0 / intencity;

            int stackMaximum = 0;

            double value = 0;
            do
            {
                if (stackMaximum > 1000)
                    throw new Exception(
                        "Минмальный интервал: слишком большое значение."
                        );

                value = -Math.Log(rnd.NextDouble()) / intencity;
                stackMaximum++;
            }
            while (value < minValue);

            return value;
        }
    }
}
