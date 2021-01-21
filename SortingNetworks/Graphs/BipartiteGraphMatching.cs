using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingNetworks.Graphs
{
    public static class BipartiteGraphMatching
    {
        public static IReadOnlyList<IReadOnlyList<int>> GetAllPerfectMatchings(IReadOnlyList<int> adjacency)
        {
            var match = GetHopcroftKarpMatching(adjacency);

            if (match == null)
            {
                return null;
            }

            throw new NotImplementedException();
        }

        public static IReadOnlyList<int> GetHopcroftKarpMatching(IReadOnlyList<int> adjacency)
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

        public static IReadOnlyList<int> GetCycle(IReadOnlyList<int> adjacency)
        {
            for (int i = 0; i < adjacency.Count; i++)
            {
                var visited = new HashSet<int>(adjacency.Count);
                var path = new List<int>();

                if (Visit(i, adjacency, visited, ref path))
                {
                    return path;
                }
            }

            return null;
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

        private static void EnumMaximumMatchingIter(IReadOnlyList<int> adjacency, IReadOnlyList<int> matchAdjacency,
            IReadOnlyList<IReadOnlyList<int>> allMatches, IReadOnlyList<Tuple<int, int>> subProblems, bool checkCycle=true)
        {
            if (checkCycle)
            {
                
            }
        }

        private static bool Visit(int v, IReadOnlyList<int> adjacency, HashSet<int> visited, ref List<int> path)
        {
            if (visited.Contains(v))
            {
                return false;
            }

            visited.Add(v);
            path.Add(v);

            for (var i = 0; i < adjacency.Count; i++)
            {
                if (adjacency[v].GetBitValue(i))
                {
                    var graph2Position = i + adjacency.Count;
                    if (path.Contains(graph2Position) || Visit(graph2Position, adjacency, visited, ref path))
                    {
                        return true;
                    }
                }
            }

            path.Remove(v);

            return false;
        }
    }
}
