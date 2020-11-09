using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortingNetworks
{
    public class GraphMatchesFinder
    {
        private static bool HasAugmentingPath(Queue<int> queue, int[] toMatchedLeft, int[] toMatchedRight, int[] distances, int[]edges)
        {
            var maxValue = edges.Length;

            for (var i = 0; i < edges.Length; i++)
            {
                if (toMatchedRight[i] == maxValue)
                {
                    distances[i] = 0;
                    queue.Enqueue(i);
                }
                else
                {
                    distances[i] = int.MaxValue;
                }
            }

            // distances[NIL] hay que iniciar to matched a este valor
            distances[maxValue] = int.MaxValue;

            while (0 < queue.Count)
            {
                var left = queue.Dequeue();

                if (distances[left] < distances[maxValue])
                {
                    for (var right = 0; right < edges.Length; right++)
                    {
                        if ((edges[right] & (1 << right)) == 0) continue;

                        var nextLeft = toMatchedLeft[right];
                        if (distances[nextLeft] == int.MaxValue)
                        {
                            // The nextLeft has not been visited and is being visited.
                            distances[nextLeft] = distances[left] + 1;
                            queue.Enqueue(nextLeft);
                        }
                    }
                }
            }

            return distances[maxValue] != int.MaxValue;
        }

        private static bool TryMatching(int left, int[] edges, int[] toMatchedLeft, int[] toMatchedRight, int[] distances)
        {
            var distancesNil = edges.Length;
            if (left == distancesNil)
            {
                return true;
            }

            
            for (var right = 0; right < edges.Length; right++)
            {
                if ((edges[left] & (1 << right)) == 0) continue;

                var nextLeft = toMatchedLeft[right];
                if (distances[nextLeft] == distances[left] + 1)
                {
                    if (TryMatching(nextLeft, edges, toMatchedLeft, toMatchedRight, distances))
                    {
                        toMatchedLeft[right] = left;
                        toMatchedRight[left] = right;
                        return true;
                    }
                }
            }
            

            // The left could not match any right.
            distances[left] = int.MaxValue;
            return false;
        }

        public static int[] FindPerfectMatches(int[] edges)
        {
            var length = edges.Length + 1;
            var maxValue = edges.Length;
            var distances = new int[length];
            var queue = new Queue<int>();
            var toMatchedRight = new int[edges.Length];
            var toMatchedLeft = new int[edges.Length];

            // set o maximum array
            toMatchedRight.Populate(maxValue);
            toMatchedLeft.Populate(maxValue);

            while (HasAugmentingPath(queue, toMatchedLeft, toMatchedRight, distances, edges))
            {
                for (var i = 0; i < edges.Length; i++)
                {
                    if (toMatchedRight[i] == maxValue)
                    {
                        TryMatching(i, edges, toMatchedLeft, toMatchedRight, distances);
                    }
                }
            }

            // If there is any unmatch no subsumption
            return toMatchedRight;
        }
    }
}
