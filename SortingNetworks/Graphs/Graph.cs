using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortingNetworks.Graphs
{
    public class Graph
    {
        private Vertex[][] _adjacency;
        public IReadOnlyList<Edge> Edges { get; }

        public IReadOnlyList<Vertex> Vertices { get; }

        public IReadOnlyList<IReadOnlyList<Vertex>> Adjacency => _adjacency;

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
            var cycle = new HashSet<Edge>();
            var vertex = new Vertex(0);
            this.GetCycleRecursive(vertex, ref cycle);

            return cycle.ToArray();
        }

        private static Vertex[][] GetAdjacencyMatrix(IReadOnlyList<Edge> edges, int size)
        {
            var matrix = new Vertex[size][];
            for (var j = 0; j < size; j++)
            {
                matrix[j] = new Vertex[size];
            }

            foreach (var edge in edges)
            {
               matrix[edge.V1.Id][edge.V2.Id] = edge.V2;
               matrix[edge.V2.Id][edge.V1.Id] = edge.V1;
            }

            return matrix;
        }

        public IReadOnlyList<Vertex> GetAdjacency(Vertex v)
        {

        }

        private bool GetCycleRecursive(Vertex vertex, ref HashSet<Edge> visited)
        {
            

            return false;
        }

        private IEnumerable<Edge> Neighbors(Edge source)
        {
            return this.Edges.Where(x => x.V1 == source.V1 || x.V2 == source.V2);
        }
    }

    public struct Edge
    {
        public Vertex V1;
        public Vertex V2;

        public Edge(Vertex v1, Vertex v2)
        {
            this.V1 = v1;
            this.V2 = v2;
        }
    }

    public struct Vertex
    {
        public bool Color;
        public int Id;

        public Vertex(int id)
        {
            this.Color = false;
            this.Id = id;
        }
    }
}
