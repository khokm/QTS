using System;

namespace QTS.Core.Helpers
{
    class RandomGenerator
    {
        Random rnd;
        public RandomGenerator(double minValue)
        {
            rnd = new Random();
            this.minValue = minValue;
        }

        double minValue;

        public double Next(int intencity)
        {
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
