using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingNetworks
{
    using System.Collections;

    public static class IEnumerableExtensions
    {
        public static void SetBit(this int[] array, int position)
        {
            array[position / 32] |= 1 << (position % 32);
        } 
        
        public static void UnsetBit(this int[] array, int position)
        {
            array[position / 32] &= ~(1 << (position % 32));
        }
        
        public static bool GetBitValue(this int[] array, int position)
        {
            return (array[position / 32] & (1 << (position % 32))) != 0;
        }
        
        public static bool GetBitValue(this int num, int position)
        {
            return (num & (1 << (position % 32))) != 0;
        }

        public static IEnumerable<IReadOnlyList<T>> SplitList<T>(this IEnumerable<T> source, int nSize = 1000)
        {
            var result = source.ToList();
            for (var i = 0; i < result.Count; i += nSize)
            {
                yield return result.GetRange(i, Math.Min(nSize, result.Count - i));
            }
        }

        public static T AggregateInParallel<T>(this IEnumerable<T> values, Func<T, T, T> function, int numTasks)
        {
            Queue<T> queue = new Queue<T>();
            var netsPerTask = queue.Count / numTasks;
            foreach (var value in values)
                queue.Enqueue(value);
            if (!queue.Any())
                return default(T);  //Consider throwing or doing something else here if the sequence is empty

            var tasks = Enumerable.Range(0, numTasks)
                .Select(_ => Task.Run(() =>
                {
                    (T, T)? GetFromQueue()
                    {
                        lock (queue)
                        {
                            if (queue.Count > 2)
                            {
                                return (queue.Dequeue(), queue.Dequeue());
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    var pair = GetFromQueue();
                    while (pair != null)
                    {
                        var result = function(pair.Value.Item1, pair.Value.Item2);
                        lock (queue)
                        {
                            queue.Enqueue(result);
                        }
                        pair = GetFromQueue();
                    }
                }))
                .ToArray();
            Task.WaitAll(tasks);
            return queue.Dequeue();
        }


        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = value;
            }
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> enumerable)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();

            var factorials = Enumerable.Range(0, array.Length + 1)
                .Select(Factorial)
                .ToArray();

            for (var i = 0L; i < factorials[array.Length]; i++)
            {
                var sequence = GenerateSequence(i, array.Length - 1, factorials);
                yield return GeneratePermutation(array, sequence);
            }
        } 
        
        public static IEnumerable<IEnumerable<int>> GetPermutationsWithSkip(this IEnumerable<int> enumerable, int[] positions)
        {
            var array = enumerable.ToArray();

            var factorials = Enumerable.Range(0, array.Length + 1)
                .Select(Factorial)
                .ToArray();

            for (var i = 0L; i < factorials[array.Length]; i++)
            {
                var sequence = GenerateSequence(i, array.Length - 1, factorials);
                var permutation = GeneratePermutation(array, sequence).ToArray();
                var add = true;
                for (int j = 0; j < permutation.Length; j++)
                {
                    var arr = new BitArray(new int[] { positions[permutation[j]] }) { Length = positions.Length };
                    if (!arr.Get(j))
                    {
                        add = false;
                        break;
                    }
                }

                if (add) yield return permutation;
            }
        }

        private static IEnumerable<T> GeneratePermutation<T>(T[] array, IReadOnlyList<int> sequence)
        {
            var clone = (T[])array.Clone();

            for (int i = 0; i < clone.Length - 1; i++)
            {
                Swap(ref clone[i], ref clone[i + sequence[i]]);
            }

            return clone;
        }

        private static int[] GenerateSequence(long number, int size, IReadOnlyList<long> factorials)
        {
            var sequence = new int[size];

            for (var j = 0; j < sequence.Length; j++)
            {
                var facto = factorials[sequence.Length - j];

                sequence[j] = (int)(number / facto);
                number = (int)(number % facto);
            }

            return sequence;
        }

        static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        private static long Factorial(int n)
        {
            long result = n;

            for (int i = 1; i < n; i++)
            {
                result *= i;
            }

            return result;
        }
    }
}
