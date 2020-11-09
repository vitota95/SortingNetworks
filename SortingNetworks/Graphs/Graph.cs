using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortingNetworks.Graphs
{
    public class Graph
    {
        public IReadOnlyList<Edge> Edges { get; }

        public IReadOnlyList<int> Vertices { get; }

        public Graph(IReadOnlyList<int> positions)
        {
            var vertices = new int[positions.Count];
            var edges = new List<Edge>();

            for (var i = 0; i < positions.Count; i++)
            {
                vertices[i] = i;
                for (var j = 0; j < positions.Count; j++)
                {
                    if ((positions[i] & (1 << j)) == 0) continue;
                    edges.Add(new Edge(i, j));
                }
            }

            this.Edges = edges;
        }

        public IReadOnlyList<Edge> GetCycle(Edge edge)
        {

        }

        private bool GetCycleRecursive(Edge source, ref HashSet<Edge> visited)
        {
            if (!visited.Contains(source))
            {
                // Mark the current node as visited
                visited.Add(source);

                // Recur for all the vertices adjacent to this vertex
                foreach (var adjacent in this.Neighbors(source))
                {
                    // If an adjacent node was not visited, then check the DFS forest of the adjacent for UNdirected cycles.
                    if (!visited.Contains(adjacent) && GetCycleRecursive(adjacent, source, ref visited))
                        return true;

                    // If an adjacent is visited and NOT parent of current vertex, then there is a cycle.
                    if (parent != (object)null && !adjacent.IsEqualTo((T)parent))
                        return true;
                }
            }

            return false;
        }

        private IEnumerable<Edge> Neighbors(Edge source)
        {
            return this.Edges.Where(x => x.V1 == source.V1 || x.V2 == source.V2);
        }
    }

    public struct Edge
    {
        public int V1;
        public int V2;

        public Edge(int v1, int v2)
        {
            this.V1 = v1;
            this.V2 = v2;
        }
    }
}
