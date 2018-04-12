using System.Collections.Generic;
using System.Linq;

namespace QTS.Core.Helpers
{
    class Permuter
    {
        public static int[][] Permute(int n, int k)
        {
            var list = new int[n];

            for (int i = 0; i < n; i++)
                list[i] = i;

            var combos = _Permute(list, k);

            List<int[]> result = new List<int[]>();

            foreach (var a in combos)
            {
                bool itsOk = true;

                int prev = -1;
                foreach (var b in a)
                {
                    if (b > prev)
                    {
                        prev = b;
                    }
                    else
                    {
                        itsOk = false;
                        break;
                    }
                }

                if (!itsOk)
                    continue;

                int[] res = new int[k];
                int index = 0;

                foreach (var b in a)
                {
                    res[index++] = b;
                }

                result.Add(res.ToArray());
            }

            return result.ToArray();
        }

        static IEnumerable<IEnumerable<T>> _Permute<T>(
            IEnumerable<T> list, int count)
        {
            if (count == 0)
            {
                yield return new T[0];
            }
            else
            {
                int startingElementIndex = 0;
                foreach (T startingElement in list)
                {
                    IEnumerable<T> remainingItems = AllExcept
                        (
                        list, startingElementIndex
                        );

                    foreach (IEnumerable<T> permutationOfRemainder in 
                        _Permute(remainingItems, count - 1))
                    {
                        yield return Concat<T>(
                            new T[] { startingElement },
                            permutationOfRemainder);
                    }
                    startingElementIndex += 1;
                }
            }
        }

        static IEnumerable<T> Concat<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            foreach (T item in a) { yield return item; }
            foreach (T item in b) { yield return item; }
        }

        static IEnumerable<T> AllExcept<T>(
            IEnumerable<T> input, int indexToSkip)
        {
            int index = 0;
            foreach (T item in input)
            {
                if (index != indexToSkip) yield return item;
                index += 1;
            }
        }
    }
}