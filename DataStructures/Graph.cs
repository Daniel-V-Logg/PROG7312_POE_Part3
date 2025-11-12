using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServiceApp.DataStructures
{
    /// <summary>
    /// Represents an edge in the graph with a weight
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// The source vertex
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// The destination vertex
        /// </summary>
        public int To { get; set; }

        /// <summary>
        /// The weight of the edge
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Initializes a new instance of the Edge class
        /// </summary>
        public Edge(int from, int to, double weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }
    }

    /// <summary>
    /// A graph data structure using adjacency list representation.
    /// Supports directed and undirected graphs with weighted edges.
    /// </summary>
    public class Graph
    {
        private readonly List<List<Edge>> _adjacencyList;
        private readonly bool _directed;
        private readonly int _vertexCount;

        /// <summary>
        /// Gets the number of vertices in the graph
        /// </summary>
        public int VertexCount => _vertexCount;

        /// <summary>
        /// Gets the number of edges in the graph
        /// </summary>
        public int EdgeCount { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Graph class
        /// </summary>
        /// <param name="vertexCount">The number of vertices in the graph</param>
        /// <param name="directed">Whether the graph is directed (default: false)</param>
        public Graph(int vertexCount, bool directed = false)
        {
            if (vertexCount < 0)
            {
                throw new ArgumentException("Vertex count must be non-negative", nameof(vertexCount));
            }

            _vertexCount = vertexCount;
            _directed = directed;
            _adjacencyList = new List<List<Edge>>();
            EdgeCount = 0;

            for (int i = 0; i < vertexCount; i++)
            {
                _adjacencyList.Add(new List<Edge>());
            }
        }

        /// <summary>
        /// Adds an edge to the graph
        /// </summary>
        /// <param name="from">The source vertex</param>
        /// <param name="to">The destination vertex</param>
        /// <param name="weight">The weight of the edge</param>
        public void AddEdge(int from, int to, double weight = 1.0)
        {
            if (from < 0 || from >= _vertexCount || to < 0 || to >= _vertexCount)
            {
                throw new ArgumentException("Vertex indices must be within valid range");
            }

            _adjacencyList[from].Add(new Edge(from, to, weight));
            EdgeCount++;

            if (!_directed)
            {
                _adjacencyList[to].Add(new Edge(to, from, weight));
            }
        }

        /// <summary>
        /// Gets all edges connected to a vertex
        /// </summary>
        /// <param name="vertex">The vertex index</param>
        /// <returns>A list of edges connected to the vertex</returns>
        public List<Edge> GetEdges(int vertex)
        {
            if (vertex < 0 || vertex >= _vertexCount)
            {
                throw new ArgumentException("Vertex index must be within valid range");
            }
            return new List<Edge>(_adjacencyList[vertex]);
        }

        /// <summary>
        /// Gets all edges in the graph
        /// </summary>
        /// <returns>A list of all edges</returns>
        public List<Edge> GetAllEdges()
        {
            List<Edge> edges = new List<Edge>();
            for (int i = 0; i < _vertexCount; i++)
            {
                foreach (var edge in _adjacencyList[i])
                {
                    if (_directed || edge.From <= edge.To)
                    {
                        edges.Add(edge);
                    }
                }
            }
            return edges;
        }

        /// <summary>
        /// Performs Depth-First Search (DFS) traversal starting from a given vertex
        /// </summary>
        /// <param name="startVertex">The starting vertex</param>
        /// <returns>A list of vertices in the order they were visited</returns>
        public List<int> DepthFirstSearch(int startVertex)
        {
            if (startVertex < 0 || startVertex >= _vertexCount)
            {
                throw new ArgumentException("Start vertex must be within valid range");
            }

            List<int> visitedOrder = new List<int>();
            bool[] visited = new bool[_vertexCount];
            DFS(startVertex, visited, visitedOrder);
            return visitedOrder;
        }

        /// <summary>
        /// Recursive helper method for DFS
        /// </summary>
        private void DFS(int vertex, bool[] visited, List<int> visitedOrder)
        {
            visited[vertex] = true;
            visitedOrder.Add(vertex);

            foreach (var edge in _adjacencyList[vertex])
            {
                if (!visited[edge.To])
                {
                    DFS(edge.To, visited, visitedOrder);
                }
            }
        }

        /// <summary>
        /// Performs Breadth-First Search (BFS) traversal starting from a given vertex
        /// </summary>
        /// <param name="startVertex">The starting vertex</param>
        /// <returns>A list of vertices in the order they were visited</returns>
        public List<int> BreadthFirstSearch(int startVertex)
        {
            if (startVertex < 0 || startVertex >= _vertexCount)
            {
                throw new ArgumentException("Start vertex must be within valid range");
            }

            List<int> visitedOrder = new List<int>();
            bool[] visited = new bool[_vertexCount];
            Queue<int> queue = new Queue<int>();

            visited[startVertex] = true;
            queue.Enqueue(startVertex);

            while (queue.Count > 0)
            {
                int vertex = queue.Dequeue();
                visitedOrder.Add(vertex);

                foreach (var edge in _adjacencyList[vertex])
                {
                    if (!visited[edge.To])
                    {
                        visited[edge.To] = true;
                        queue.Enqueue(edge.To);
                    }
                }
            }

            return visitedOrder;
        }

        /// <summary>
        /// Computes the Minimum Spanning Tree (MST) using Kruskal's algorithm
        /// </summary>
        /// <returns>A list of edges that form the MST</returns>
        public List<Edge> MinimumSpanningTreeKruskal()
        {
            List<Edge> mst = new List<Edge>();
            List<Edge> allEdges = GetAllEdges();

            allEdges.Sort((e1, e2) => e1.Weight.CompareTo(e2.Weight));

            int[] parent = new int[_vertexCount];
            for (int i = 0; i < _vertexCount; i++)
            {
                parent[i] = i;
            }

            foreach (var edge in allEdges)
            {
                int rootFrom = Find(parent, edge.From);
                int rootTo = Find(parent, edge.To);

                if (rootFrom != rootTo)
                {
                    mst.Add(edge);
                    Union(parent, rootFrom, rootTo);
                }
            }

            return mst;
        }

        /// <summary>
        /// Finds the root of a vertex using path compression
        /// </summary>
        private int Find(int[] parent, int vertex)
        {
            if (parent[vertex] != vertex)
            {
                parent[vertex] = Find(parent, parent[vertex]);
            }
            return parent[vertex];
        }

        /// <summary>
        /// Unions two sets
        /// </summary>
        private void Union(int[] parent, int x, int y)
        {
            parent[x] = y;
        }
    }
}

