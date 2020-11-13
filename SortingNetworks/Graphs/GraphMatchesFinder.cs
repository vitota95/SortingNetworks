using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SortingNetworks.Graphs;

namespace SortingNetworks
{
    public class GraphMatchesFinder
    {
        public bool TryPerfectMatches(IReadOnlyList<int> positions, HashSet<ushort> o1, HashSet<ushort> o2)
        {
            var perfectMatch = this.FindPerfectMatch(positions);
            if (perfectMatch == null) return false;

            if (OutputIsSubset(perfectMatch, o1, o2))
            {
                return true;
            }

            var problems = new Queue<Tuple<IReadOnlyList<int>, IReadOnlyList<int>>>();
            problems.Enqueue(new Tuple<IReadOnlyList<int>, IReadOnlyList<int>>(positions, perfectMatch));

            while (problems.Count > 0)
            {
                var problem = problems.Dequeue();
                var previousMatch = problem.Item2.ToList();
                positions = problem.Item1.ToArray();
                var g1 = new Graph(positions.ToArray());
                var g2 = new Graph(previousMatch);

                // TODO no need to create new positions
                g1.RemoveEdges(new Graph(problem.Item2).Edges);
                g1.Merge(g2);
                var cycle = g1.GetCycle();

                if (cycle == null) return false;

                perfectMatch = Graph.GetSymmetricDifference(cycle, previousMatch);
                if (perfectMatch.Count == 0) continue;
                
                if (OutputIsSubset(perfectMatch, o1, o2))
                {
                    return true;
                }
            }

            return false;
        }

        private IReadOnlyList<int> FindPerfectMatch(IReadOnlyList<int> adjacency)
        {
            var dimension = adjacency.Count;
            var matchR = new int[dimension];

            for (var i = 0; i < dimension; ++i) matchR[i] = -1;

            for (var u = 0; u < dimension; u++)
            {
                var seen = new bool[dimension];
                for (var i = 0; i < dimension; ++i)
                    seen[i] = false;

                BPM(adjacency, u, seen, matchR);
            }

            return matchR.Contains(-1) ? null : matchR;
        }

        private static bool BPM(IReadOnlyList<int> bpGraph, int u, bool[] seen, int[] matchR)
        {
            for (var v = 0; v < bpGraph.Count; v++)
            {
                if ((bpGraph[u] & (1 << v)) == 0 || seen[v]) continue;

                seen[v] = true;
                
                if (matchR[v] < 0 || BPM(bpGraph, matchR[v], seen, matchR))
                {
                    matchR[v] = u;
                    return true;
                }
            }
            return false;
        }

        private bool OutputIsSubset(IReadOnlyList<int> permutation, HashSet<ushort> o1, HashSet<ushort> o2)
        {
            using (var enumerator = o2.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
#if DEBUG
                    IComparatorNetwork.PermutationsNumber++;
#endif
                    var output = enumerator.Current;
                    var newOutput = 0;

                    // permute bits
                    for (var j = 0; j < permutation.Count; j++)
                    {
                        if ((output & (1 << permutation[j])) > 0) newOutput |= 1 << j;
                    }

                    if (!o1.Contains((ushort)newOutput))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
