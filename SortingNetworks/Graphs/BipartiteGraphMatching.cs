using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SortingNetworks.Graphs
{
    public class BipartiteGraphMatching
    {
        private int[] dist;

        private int[] pairU;

        private int[] pairV;

        private int NIL;

        private int[] match1Adj;

        private int[] match2Adj;

        public bool GetAllPerfectMatchings(IReadOnlyList<int> adjacency, int[] output1, int[] output2, int[] output2Dual)
        {
            var match = GetHopcroftKarpMatching(adjacency);

            if (match == null)
            {
                //Trace.WriteLine($"Result: False");
                return false;
            }

            //if (ComparatorNetwork.OutputIsSubset(match, output1, output2) ||
            //    ComparatorNetwork.OutputIsSubset(match, output2Dual, output1)) 
            //Trace.WriteLine($"Found match {string.Join(", ", match)}");
            if (ComparatorNetwork.OutputIsSubsetBipartite(match, output1, output2))
            {
                //Trace.WriteLine($"Result: True");
                return true;
            }

            var result = this.GetAllPerfectMatchingsIter(adjacency, match, output1, output2, output2Dual);
            //Trace.WriteLine($"Result: {result}");
            return result;
        }

        public int[] GetHopcroftKarpMatching(IReadOnlyList<int> adjacency)
        {
            var vertexInMatching = 0;
            var dimension = adjacency.Count + 1;
            NIL = adjacency.Count;
            pairU = new int[dimension];
            pairV = new int[dimension];
            pairU.Populate(NIL);
            pairV.Populate(NIL);

            dist = new int[dimension];

            while (BFS(adjacency.ToArray()))
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
                return matching;
            }

            return null;
        }

        private bool GetAllPerfectMatchingsIter(IReadOnlyList<int> adjacency, IReadOnlyList<int> match, int[] output1, int[] output2, int[] output2Dual)
        {
            var newMatch = GetNextMatching(adjacency, match);

            if (newMatch == null)
            {
                return false;
            }

            var queue = new Queue<Tuple<IReadOnlyList<int>, IReadOnlyList<int>>>();
            AddSubproblemsToQueue(adjacency, match, newMatch, ref queue);

            while (queue.TryDequeue(out Tuple<IReadOnlyList<int>, IReadOnlyList<int>> subproblem))
            {
                var subAdj = subproblem.Item1;
                var subMatch = subproblem.Item2;
                newMatch = this.GetNextMatching(subAdj, subMatch);

                if (newMatch != null)
                {
                    //if (ComparatorNetwork.OutputIsSubset(newMatch, output1, output2) ||
                    //    ComparatorNetwork.OutputIsSubset(newMatch, output2Dual, output1))
                    //Trace.WriteLine($"Found match {string.Join(", ", newMatch)}");
                    if (ComparatorNetwork.OutputIsSubsetBipartite( newMatch, output1, output2))
                    {
                        return true;
                    }

                    AddSubproblemsToQueue(subAdj, subMatch, newMatch, ref queue);
                }
            }

            return false;
        }

        private void AddSubproblemsToQueue(IReadOnlyList<int> adjacency, IReadOnlyList<int> match, IReadOnlyList<int> newMatch,
            ref Queue<Tuple<IReadOnlyList<int>, IReadOnlyList<int>>> queue)
        {
            Tuple<int, int> edge = null;
            var i = 0;
            var j = 0;
            for (i = 0; i < adjacency.Count; i++)
            {
                for (j = 0; j < adjacency.Count; j++)
                {
                    // take an edge in M1 - M2
                    if ((this.match1Adj[i] & (1 << j)) != 0 && (this.match2Adj[i] & (1 << j)) == 0)
                    {
                        edge = new Tuple<int, int>(i, 1 << j);
                        break;
                    }
                }

                if (edge != null)
                {
                    break;
                }
            }

            var gPlus = adjacency.ToArray();
            var gMinus = adjacency.ToArray();

            if (edge == null)
            {
                return;
            }

            // remove all edges of vertex contained in E
            gPlus[i] = 0;

            for (var k = 0; k < gPlus.Length; k++)
            {
                //gPlus[i] &= ~(edge.Item2);
                gPlus[k] &= ~(1<<j);
            }

            // add E again
            gPlus[i] = 1<<j;

            // remove (i,j) edge from gMinus
            gMinus[i] &= ~(1<<j);

            queue.Enqueue(new Tuple<IReadOnlyList<int>, IReadOnlyList<int>>(gPlus, match));
            queue.Enqueue(new Tuple<IReadOnlyList<int>, IReadOnlyList<int>>(gMinus, match));
        }

        private int[] GetNextMatching(IReadOnlyList<int> bipartite, IReadOnlyList<int> matching)
        {
            var uAdj = new int[bipartite.Count];
            var vAdj = new int[bipartite.Count];
            var matchingAdj = new int[bipartite.Count];

            for (var i = 0; i < matching.Count; i++)
            {
                var bipartitePosition = bipartite[i];

                // set difference
                bipartitePosition &= ~(1 << matching[i]);

                // edges in E (all edges) - match directed to V
                uAdj[i] = bipartitePosition;

                // direct edges in the match from V to U
                vAdj[matching[i]] = 1 << i;
                matchingAdj[i] = 1 << matching[i];
            }

            this.match1Adj = vAdj;

            // find cycle in directed graph
            return GetMatchingFromCycle(uAdj, vAdj, matchingAdj);
        }

        public int[] GetMatchingFromCycle(IReadOnlyList<int> uAdj, IReadOnlyList<int> vAdj, IReadOnlyList<int> matchingAdj)
        {
            for (var i = 0; i < uAdj.Count; i++)
            {
                var visited = new HashSet<Tuple<bool, int>>();
                var path = new List<Tuple<bool, int>>();
                var next = new Tuple<bool, int>(true, i);
                if (Visit(ref next, uAdj, vAdj, visited, ref path))
                {
                    path.Add(next);
                    return GetMatching(matchingAdj, path);
                }

                visited = new HashSet<Tuple<bool, int>>();
                path = new List<Tuple<bool, int>>();
                next = new Tuple<bool, int>(false, i);

                if (Visit(ref next, uAdj, vAdj, visited, ref path))
                {
                    path.Add(next);
                    return GetMatching(matchingAdj, path);
                }
            }

            return null;
        }

        private int[] GetMatching(IReadOnlyList<int> vAdj, List<Tuple<bool, int>> path)
        {
            var cycleAdj = new int[vAdj.Count];

            // Remove edges in the cycle
            for (var i = 0; i < path.Count - 1; i++)
            {
                var v1 = path[i].Item2;
                var v2 = path[i + 1].Item2;

                if (path[i].Item1)
                {
                    cycleAdj[v1] |= 1 << v2;
                }
                else
                {
                    cycleAdj[v2] |= 1 << v1;
                }
            }

            var newMatchAdj = SymmetricDifference(vAdj, cycleAdj);

            this.match2Adj = new int[newMatchAdj.Length];
            Array.Copy(newMatchAdj, this.match2Adj, newMatchAdj.Length);

            // convert adj matrix to match
            var newMatch = new int[vAdj.Count];
            for (var i = 0; i < newMatchAdj.Length; i++)
            {
                var exponent = 0;
                while ((newMatchAdj[i] >>= 1) != 0)
                {
                    exponent++;
                }

                newMatch[i] = exponent;
            }

            return newMatch;
        }

        private static int[] SymmetricDifference(IReadOnlyList<int> adj1, int[] adj2)
        {
            var newMatchAdj = new int[adj1.Count];
            for (var i = 0; i < adj1.Count; i++)
            {
                for (var j = 0; j < adj2.Length; j++)
                {
                    if ((adj2[i] & (1 << j)) != 0 && (adj1[i] & (1 << j)) == 0)
                    {
                        newMatchAdj[i] |= 1 << j;
                    }
                    else if ((adj2[i] & (1 << j)) == 0 && (adj1[i] & (1 << j)) != 0)
                    {
                        newMatchAdj[i] |= 1 << j;
                    }
                }
            }

            return newMatchAdj;
        }

        private bool BFS(int[] adjacency)
        {
            var queue = new Queue<int>();

            for (var i = 0; i < adjacency.Length; i++)
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

        private bool Visit(ref Tuple<bool, int> v, IReadOnlyList<int> adjacency1, IReadOnlyList<int> adjacency2, HashSet<Tuple<bool, int>> visited, ref List<Tuple<bool, int>> path)
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
                if (path.Contains(next) || Visit(ref next, adjacency1, adjacency2, visited, ref path))
                {
                    return true;
                }
            }

            path.Remove(v);

            return false;
        }
    }
}
