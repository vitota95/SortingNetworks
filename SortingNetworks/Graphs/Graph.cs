﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortingNetworks.Graphs
{
    public class Graph
    {
        private static readonly IReadOnlyList<int> _vertices = Enumerable.Range(0, IComparatorNetwork.Inputs).ToList();

        private List<Edge> _edges;

        private List<int> _adjacency;

        public IReadOnlyList<Edge> Edges => _edges;

        public IReadOnlyList<int> Vertices => _vertices;

        public IReadOnlyList<int> Adjacency => _adjacency;

        public Graph(IReadOnlyList<int> positions, bool inverse = false)
        {
            var edges = new List<Edge>();

            for (var i = 0; i < positions.Count; i++)
            {
                for (var j = 0; j < positions.Count; j++)
                {
                    if ((positions[i] & (1 << j)) == 0) continue;
                    edges.Add(inverse
                        ? new Edge(this.Vertices[j], this.Vertices[i])
                        : new Edge(this.Vertices[i], this.Vertices[j]));
                }
            }

            this._edges = edges;
            this._adjacency = positions.ToList();
        }

        public IReadOnlyList<int> GetCycle()
        {
            var visited = new HashSet<int>(this.Vertices.Count);
            var cycle = new HashSet<int>(this.Vertices.Count);

            foreach (var vertex in this.Vertices)
            {
                if (this.GetCycleRecursive(vertex, visited, ref cycle)) return cycle.ToArray();
            }

            return null;
        }

        public void RemoveEdges(IReadOnlyList<Edge> edges)
        {
            foreach (var e in edges)
            {
                this._edges.Remove(e);
                this._adjacency[e.V1] &= ~(1 << e.V2);
            }
        }  
        
        public void RemoveEdge(Edge edge)
        {
            this._edges.Remove(edge);
            this._adjacency[edge.V1] &= ~(1 << edge.V2);
        }        
        
        public void RemoveEdgesAndAdjacent(IReadOnlyList<Edge> edges)
        {
            foreach (var e in edges)
            {
                this._edges.Remove(e);
                this._adjacency[e.V1] = 0;
            }
        }

        public void Merge(Graph g)
        {
            this._edges.AddRange(g.Edges);
        }

        public static IReadOnlyList<int> GetSymmetricDifference(IReadOnlyList<int> a, IReadOnlyList<int> b)
        {
            var x2 = b.ToList();
            //var differences = a.Except(b).Union(b.Except(a));

            foreach (var e in a)
            {
                var i = x2.IndexOf(e);
                if (i != -1)
                {
                    x2[i] = i;
                    x2[e] = e;
                }
            }

            return x2;
        }

        private bool GetCycleRecursive(int vertex,  HashSet<int> visited, ref HashSet<int> recStack)
        {
            if (!recStack.Add(vertex)) return true;

            if (!visited.Add(vertex)) return false;

            var children = this.Adjacency[vertex];

            for (var i = 0; i < this.Vertices.Count; i++)
            {
                if ((children & (1 << i)) == 0) continue;
                if (GetCycleRecursive(i, visited, ref recStack)) return true;
            }

            recStack.Remove(vertex);

            return false;
        }

        private IEnumerable<Edge> Neighbors(Edge source)
        {
            return this.Edges.Where(x => x.V1 == source.V1 || x.V2 == source.V2);
        }
    }

    public struct Edge : IEquatable<Edge>
    {
        public readonly int V1;
        public readonly int V2;

        public Edge(int v1, int v2)
        {
            this.V1 = v1;
            this.V2 = v2;
        }

        public override int GetHashCode() => (V1, V2).GetHashCode();
        public override bool Equals(object other) => other is Edge l && Equals(l);
        public bool Equals(Edge other) => V1 == other.V1 && V2 == other.V2;
    }
}
