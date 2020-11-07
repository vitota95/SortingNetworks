using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortingNetworks
{
    public class GraphMatchesFinder
    {
        private static bool BFS(Queue<int> queue, int[] toMatchedLeft, int[] toMatchedRight, int[] distances, int[]edges)
        {
            var distancesNil = edges.Length;

            for (var i = 0; i < toMatchedLeft.Length; i++)
            {
                if (distances[i] == 0)
                {
                    queue.Enqueue(i);
                }
                else
                {
                    distances[i] = 0;
                }
            }

            // distances[NIL] hay que iniciar to matched a este valor
            distances[distancesNil] = int.MaxValue;

            while (queue.Count > 0)
            {
                var left = queue.Dequeue();

                if (distances[left] < distances[distancesNil])
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

            return distances[distancesNil] != int.MaxValue;
        }

        private static bool DFS(int left, int[] edges, int[] toMatchedLeft, int[] toMatchedRight, int[] distances)
        {
            var distancesNil = edges.Length;
            if (left == distancesNil)
            {
                return true;
            }

            for (var right = 0; right<edges.Length; right++)
            {
                for (var i = 0; i < edges.Length; i++)
                {
                    if ((edges[right] & (1 << i)) == 0) continue;

                    var nextLeft = toMatchedLeft[right];
                    if (distances[nextLeft] == distances[left] + 1)
                    {
                        if (DFS(nextLeft, edges, toMatchedRight, toMatchedLeft, distances))
                        {
                            toMatchedLeft[right] = left;
                            toMatchedRight[left] = right;
                            return true;
                        }
                    }
                }
            }

            // The left could not match any right.
            distances[left] = int.MaxValue;
            return false;
        }

        public static int[] FindPerfectMatchs(int[] edges)
        {
            var defaultvalue = edges.Length + 1;
            var distances = new int[defaultvalue];
            var queue = new Queue<int>();
            var toMatchedRight = new int[edges.Length];
            var toMatchedLeft = new int[edges.Length];

            toMatchedRight.Populate(defaultvalue - 1);
            toMatchedLeft.Populate(defaultvalue - 1);

            while (BFS(queue, toMatchedLeft, toMatchedRight, distances, edges))
            {
                for (int i = 0; i < edges.Length; i++)
                {
                    if (toMatchedRight[i] == defaultvalue - 1)
                    {
                        DFS(i, edges, toMatchedLeft, toMatchedRight, distances);
                    }
                }
            }

            // REMOVE UNMATCHES??

            return toMatchedRight;
        }
    }
}
