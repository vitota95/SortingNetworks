using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortingNetworks.Graphs
{
    public class Graph
    {
        private Vertex[][] _adjacency;

        private static readonly IReadOnlyList<Vertex> _vertices = Enumerable.Range(0, IComparatorNetwork.Inputs - 1).Select(x => new Vertex(x)).ToList(); 

        public IReadOnlyList<Edge> Edges { get; }

        public IReadOnlyList<Vertex> Vertices => _vertices;

        public IReadOnlyList<IReadOnlyList<Vertex>> Adjacency => _adjacency;

        public Graph(IReadOnlyList<int> positions)
        {
            var edges = new List<Edge>();

            for (var i = 0; i < positions.Count; i++)
            {
                for (var j = 0; j < positions.Count; j++)
                {
                    if ((positions[i] & (1 << j)) == 0) continue;
                    edges.Add(new Edge(this.Vertices[i], this.Vertices[j]));
                }
            }

            this.Edges = edges;
            this._adjacency = GetAdjacencyMatrix(edges, positions.Count);
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

        private bool GetCycleRecursive(Vertex vertex, ref HashSet<Edge> visited)
        {


            return false;
        }

        private IEnumerable<Edge> Neighbors(Edge source)
        {
            return this.Edges.Where(x => x.V1.Id == source.V1.Id || x.V2.Id == source.V2.Id);
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
