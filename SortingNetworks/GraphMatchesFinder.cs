using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SortingNetworks
{
    public class GraphMatchesFinder
    {
        private static int V { get; set; }

        public IEnumerable<int[]> FindAllPerfectMatches(int[] edges)
        {
            V = edges.Length;
            var bpGraph = new bool[edges.Length, edges.Length];
            for (var i = 0; i < edges.Length; i++)
            {
                for (var j = 0; j < edges.Length; j++)
                {
                    bpGraph[i, j] = ((edges[i] >> j) & 1) == 1;
                }
            }

            var permutation = this.FindPerfectMatch(bpGraph);

            if (permutation.Contains(-1))
            {
                return null;
            }

            return this.FindPerfectMatchesRecursive(bpGraph, new List<int[]>{permutation});
        }

        private bool BPM(bool[,] bpGraph, int u, bool[] seen, int[] matchR)
        {
            for (var v = 0; v < bpGraph.GetLength(0); v++)
            {
                if (!bpGraph[u, v] || seen[v]) continue;

                seen[v] = true;

                if (matchR[v] < 0 || BPM(bpGraph, matchR[v], seen, matchR))
                {
                    matchR[v] = u;
                    return true;
                }
            }
            return false;
        }

        // Returns maximum number of  

        // matching from M to N 

        public int[] FindPerfectMatch(bool[,] bpGraph)
        {
            var dimension = bpGraph.GetLength(0);
            var matchR = new int[dimension];

            for (var i = 0; i < dimension; ++i) matchR[i] = -1;

            for (var u = 0; u < dimension; u++)
            {
                var seen = new bool[dimension];
                for (var i = 0; i < dimension; ++i)
                    seen[i] = false;

                BPM(bpGraph, u, seen, matchR);
            }

            return matchR;
        }

        private IReadOnlyList<int[]> FindPerfectMatchesRecursive(bool[,] bpGraph, List<int[]> perfectMatches)
        {
            var hasEdges = false;
            int d1;
            int d2;
            for (var i = 0; i < bpGraph.GetLength(0); i++)
            {
                for (var j = 0; j < bpGraph.GetLength(1); j++)
                {
                    if (bpGraph[i,j])
                    {
                        hasEdges = true;
                        d1 = i;
                        d2 = j;
                        break;
                    }
                }

                if (hasEdges)
                {
                    break;
                }
            }

            if (!hasEdges)
            {
                return perfectMatches;
            }



        }
        // A recursive function that uses visited[]  
        // and parent to detect cycle in subgraph 
        // reachable from vertex v. 
        Boolean isCyclicUtil(int v, Boolean[] visited, int parent)
        {
            // Mark the current node as visited 
            visited[v] = true;

            // Recur for all the vertices  
            // adjacent to this vertex 
            foreach (int i in adj[v])
            {
                // If an adjacent is not visited,  
                // then recur for that adjacent 
                if (!visited[i])
                {
                    if (isCyclicUtil(i, visited, v))
                        return true;
                }

                // If an adjacent is visited and  
                // not parent of current vertex, 
                // then there is a cycle. 
                else if (i != parent)
                    return true;
            }
            return false;
        }

        // Returns true if the graph contains  
        // a cycle, else false. 
        Boolean isCyclic()
        {
            // Mark all the vertices as not visited  
            // and not part of recursion stack 
            Boolean[] visited = new Boolean[V];
            for (int i = 0; i < V; i++)
                visited[i] = false;

            // Call the recursive helper function  
            // to detect cycle in different DFS trees 
            for (int u = 0; u < V; u++)

                // Don't recur for u if already visited 
                if (!visited[u])
                    if (isCyclicUtil(u, visited, -1))
                        return true;

            return false;
        }
    }
}
