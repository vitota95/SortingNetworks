using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingNetworks.Graphs
{
    public class BipartiteGraphMatching
    {
        private List<int[]> matchings;

        private int[] dist;

        private int[] pairU;

        private int[] pairV;

        private int NIL;

        public IReadOnlyList<IReadOnlyList<int>> GetAllPerfectMatchings(IReadOnlyList<int> adjacency)
        {
            matchings = new List<int[]>();

            var match = GetHopcroftKarpMatching(adjacency);

            if (match == null)
            {
                return null;
            }


            if (match == null)
            {
                return null;
            }

            var cycle = GetDirectedGraphCycle(adjacency, match);

            var newMatch = new int[adjacency.Count];
            for (var i = 0; i < adjacency.Count; i++)
            {
                newMatch[i] = match[i] & ~cycle[i];
            }
            
            throw new NotImplementedException();
        }

        public IReadOnlyList<int> GetHopcroftKarpMatching(IReadOnlyList<int> adjacency)
        {
            matchings = new List<int[]>();

            var vertexInMatching = 0;
            var dimension = adjacency.Count + 1;
            NIL = adjacency.Count;
            pairU = new int[dimension];
            pairV = new int[dimension];
            pairU.Populate(NIL);
            pairV.Populate(NIL);

            dist = new int[dimension];

            while (BFS(dimension, adjacency.ToArray()))
            {
                for (var u = 0; u < adjacency.Count; u++)
                {
                    if (pairU[u] == NIL && DFS(u, adjacency.ToArray()))
                    {
                        vertexInMatching++;
                    }
                }
            }

            if (vertexInMatching == adjacency.Count)
            {
                var matching = pairU.Take(adjacency.Count).ToArray();
                matchings.Add(matching);
                return matching;
            }

            return null;
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
                if (Visit(new Tuple<bool, int>(false, i), adjacency2, adjacency1, visited, ref path))
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
                for (var j = 0; j < adj1.Count; j++)
                {
                    if ((adj2[i] & (1 << j)) == 1)
                    {
                        result[j] |= 1 << i;
                    }
                }
            }

            return result;
        }
         
        private bool BFS(int dimension, int[] adjacency)
        {
            var queue = new Queue<int>();

            for(var i = 0; i < adjacency.Length; i++)
            {
                if (pairU[i] == NIL)
                {
                    dist[i] = 0;
                    queue.Enqueue(i);
                }
                else
                {
                    dist[i] = int.MaxValue;
                }
            }

            dist[NIL] = int.MaxValue;

            while (queue.TryDequeue(out var u))
            {
                if (dist[u] < dist[NIL])
                {
                    for (int i = 0; i < IComparatorNetwork.Inputs; i++)
                    {
                        // Get all adjacent vertices of dequeued vertex u
                        if ((adjacency[u] & (1 << i)) == 0) continue;
                        var v = i;

                        if (dist[pairV[v]] == int.MaxValue)
                        {
                            dist[pairV[v]] = dist[u] + 1;
                            queue.Enqueue(pairV[v]);
                        }
                    }
                }
            }

            return dist[NIL] != int.MaxValue;
        } 
        
        private bool DFS(int u, int[] adjacency)
        {
            if (u == NIL) return true;

            for (var i = 0; i < IComparatorNetwork.Inputs; i++)
            {
                if ((adjacency[u] & (1 << i)) == 0) continue;
                var v = i;

                if (dist[pairV[v]] == dist[u] + 1)
                {
                    if (DFS(pairV[v], adjacency))
                    {
                        pairV[v] = u;
                        pairU[u] = v;
                        return true;
                    }
                }
            }

            dist[u] = int.MaxValue;
            return false;
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

                // edges in E (all edges) - pairU directed to V
                uAdj[i] = bipartitePosition;
            }

            // find cycle in directed graph
            return GetCycle(uAdj, matching);
        }
    }
}
