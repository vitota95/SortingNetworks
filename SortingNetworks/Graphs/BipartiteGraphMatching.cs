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
            var match = GetHopcroftKarpMatching(adjacency).ToArray();

            if (match == null)
            {
                return null;
            }

            match = new [] { 1, 0, 2, 3, 4 };

            var cycle = GetDirectedGraphCycle(adjacency, match);

            var newMatch = new int[match.Length];
            for (var i = 0; i < match.Length; i++)
            {
                newMatch[i] = match[i] & ~cycle[i];
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

        public static IReadOnlyList<int> GetCycle(IReadOnlyList<int> adjacency1, IReadOnlyList<int> adjacency2)
        {
            for (var i = 0; i < adjacency1.Count; i++)
            {
                var visited = new HashSet<Tuple<bool, int>>();
                var path = new List<Tuple<bool, int>>();

                if (Visit(new Tuple<bool, int>(true, i), adjacency1, adjacency2, visited, ref path))
                {
                    // here I should create a new adjacency matrix by fusion of this 2 with a bipartite graph????
                    return GetBipartiteAdjacency(adjacency1, adjacency2);
                }
                
                path = new List<Tuple<bool, int>>();
                if (Visit(new Tuple<bool, int>(false, i), adjacency1, adjacency2, visited, ref path))
                {
                    return GetBipartiteAdjacency(adjacency2, adjacency1);
                }
            }

            return null;
        }

        private static IReadOnlyList<int> GetBipartiteAdjacency(IReadOnlyList<int> adj1, IReadOnlyList<int> adj2)
        {
            var result = adj1.ToArray();

            for (var i = 0; i < adj1.Count; i++)
            {
                for (int j = 0; j < adj1.Count; j++)
                {
                    if ((adj2[i] & (1 << j)) == 1)
                    {
                        result[j] |= 1 << i;
                    }
                }
            }

            return result;
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

        private static bool Visit(Tuple<bool, int> v, IReadOnlyList<int> adjacency1, IReadOnlyList<int> adjacency2, HashSet<Tuple<bool, int>> visited, ref List<Tuple<bool, int>> path)
        {
            if (visited.Contains(v))
            {
                return false;
            }

            visited.Add(v);
            path.Add(v);

            var adjacency = v.Item1 ? adjacency1 : adjacency2;

            var position = adjacency[v.Item2];
            for (var i = 0; i < adjacency.Count; i++)
            {
                if (!position.GetBitValue(i)) continue;

                var next = new Tuple<bool, int>(!v.Item1, i);
                if (path.Contains(next) || Visit(next, adjacency1, adjacency2, visited, ref path))
                {
                    return true;
                }
            }

            path.Remove(v);

            return false;
        }

        private static IReadOnlyList<int> GetDirectedGraphCycle(IReadOnlyList<int> bipartite, IReadOnlyList<int> matching)
        {
            var uAdj = new int[bipartite.Count];

            for (var i = 0; i < matching.Count;  i++)
            {
                var bipartitePosition = bipartite[i];

                // set difference
                bipartitePosition &= ~(1 << matching[i]);

                // edges in E (all edges) - matching directed to V
                uAdj[i] = bipartitePosition;
            }

            // find cycle in directed graph
            return GetCycle(uAdj, matching);
        }
    }
}
