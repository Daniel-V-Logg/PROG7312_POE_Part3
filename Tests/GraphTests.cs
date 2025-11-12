using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServiceApp.DataStructures;
using MunicipalServiceApp.Models;

namespace MunicipalServiceApp.Tests
{
    /// <summary>
    /// Unit tests for Graph data structure, traversal algorithms, and MST computation.
    /// Demonstrates building graphs from ServiceRequest coordinates and routing optimization.
    /// </summary>
    public class GraphTests
    {
        /// <summary>
        /// Tests basic graph operations (add edge, get edges)
        /// </summary>
        public static void TestBasicGraphOperations()
        {
            Console.WriteLine("=== Testing Basic Graph Operations ===");
            Graph graph = new Graph(5, directed: false);

            graph.AddEdge(0, 1, 2.5);
            graph.AddEdge(0, 2, 1.0);
            graph.AddEdge(1, 3, 3.0);
            graph.AddEdge(2, 4, 2.0);

            Console.WriteLine($"Graph has {graph.VertexCount} vertices and {graph.EdgeCount} edges");

            List<Edge> edgesFrom0 = graph.GetEdges(0);
            Console.WriteLine($"Edges from vertex 0: {edgesFrom0.Count}");
            foreach (var edge in edgesFrom0)
            {
                Console.WriteLine($"  {edge.From} -> {edge.To} (weight: {edge.Weight})");
            }

            Console.WriteLine("Basic graph operations test completed.\n");
        }

        /// <summary>
        /// Tests Depth-First Search traversal
        /// </summary>
        public static void TestDepthFirstSearch()
        {
            Console.WriteLine("=== Testing Depth-First Search (DFS) ===");
            Graph graph = new Graph(6, directed: false);

            graph.AddEdge(0, 1);
            graph.AddEdge(0, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(2, 4);
            graph.AddEdge(3, 5);

            List<int> dfsResult = graph.DepthFirstSearch(0);
            Console.WriteLine($"DFS traversal from vertex 0: {string.Join(" -> ", dfsResult)}");

            bool isCorrect = dfsResult.Count == 6 && dfsResult[0] == 0;
            Console.WriteLine($"DFS order is correct: {isCorrect}");
            Console.WriteLine("DFS test completed.\n");
        }

        /// <summary>
        /// Tests Breadth-First Search traversal
        /// </summary>
        public static void TestBreadthFirstSearch()
        {
            Console.WriteLine("=== Testing Breadth-First Search (BFS) ===");
            Graph graph = new Graph(6, directed: false);

            graph.AddEdge(0, 1);
            graph.AddEdge(0, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(2, 4);
            graph.AddEdge(3, 5);

            List<int> bfsResult = graph.BreadthFirstSearch(0);
            Console.WriteLine($"BFS traversal from vertex 0: {string.Join(" -> ", bfsResult)}");

            bool isCorrect = bfsResult.Count == 6 && bfsResult[0] == 0;
            Console.WriteLine($"BFS order is correct: {isCorrect}");
            Console.WriteLine("BFS test completed.\n");
        }

        /// <summary>
        /// Tests Minimum Spanning Tree using Kruskal's algorithm
        /// </summary>
        public static void TestMinimumSpanningTree()
        {
            Console.WriteLine("=== Testing Minimum Spanning Tree (Kruskal) ===");
            Graph graph = new Graph(5, directed: false);

            graph.AddEdge(0, 1, 2.0);
            graph.AddEdge(0, 2, 1.0);
            graph.AddEdge(1, 2, 3.0);
            graph.AddEdge(1, 3, 4.0);
            graph.AddEdge(2, 3, 2.5);
            graph.AddEdge(2, 4, 1.5);
            graph.AddEdge(3, 4, 3.5);

            List<Edge> mst = graph.MinimumSpanningTreeKruskal();
            Console.WriteLine($"MST contains {mst.Count} edges:");

            double totalWeight = 0.0;
            foreach (var edge in mst)
            {
                Console.WriteLine($"  {edge.From} -> {edge.To} (weight: {edge.Weight:F2})");
                totalWeight += edge.Weight;
            }

            Console.WriteLine($"Total MST weight: {totalWeight:F2}");
            Console.WriteLine("MST test completed.\n");
        }

        /// <summary>
        /// Tests building a graph from ServiceRequest coordinates
        /// </summary>
        public static void TestGraphFromServiceRequests()
        {
            Console.WriteLine("=== Testing Graph from ServiceRequest Coordinates ===");

            List<ServiceRequest> requests = new List<ServiceRequest>
            {
                new ServiceRequest("Request 1", "First request", 1, -25.7479, 28.2293),
                new ServiceRequest("Request 2", "Second request", 2, -25.7480, 28.2294),
                new ServiceRequest("Request 3", "Third request", 3, -25.7481, 28.2295),
                new ServiceRequest("Request 4", "Fourth request", 4, -25.7482, 28.2296),
                new ServiceRequest("Request 5", "Fifth request", 5, -25.7483, 28.2297)
            };

            Graph graph = GraphUtilities.BuildGraphFromRequests(requests, maxDistanceKm: 50.0);

            Console.WriteLine($"Graph built from {requests.Count} service requests");
            Console.WriteLine($"Graph has {graph.VertexCount} vertices and {graph.EdgeCount} edges");

            List<Edge> allEdges = graph.GetAllEdges();
            Console.WriteLine($"\nEdges in graph (distance in km):");
            foreach (var edge in allEdges)
            {
                double distance = edge.Weight;
                Console.WriteLine($"  Request {edge.From + 1} <-> Request {edge.To + 1}: {distance:F4} km");
            }

            Console.WriteLine("Graph from ServiceRequests test completed.\n");
        }

        /// <summary>
        /// Tests MST computation for routing optimization
        /// </summary>
        public static void TestMSTRouting()
        {
            Console.WriteLine("=== Testing MST for Routing Optimization ===");

            List<ServiceRequest> requests = new List<ServiceRequest>
            {
                new ServiceRequest("Location A", "Service at location A", 1, -25.7479, 28.2293, "Team-1"),
                new ServiceRequest("Location B", "Service at location B", 2, -25.7480, 28.2294, "Team-1"),
                new ServiceRequest("Location C", "Service at location C", 3, -25.7481, 28.2295, "Team-1"),
                new ServiceRequest("Location D", "Service at location D", 4, -25.7482, 28.2296, "Team-1"),
                new ServiceRequest("Location E", "Service at location E", 5, -25.7483, 28.2297, "Team-1")
            };

            Graph graph = GraphUtilities.BuildGraphFromRequests(requests, maxDistanceKm: 50.0);
            List<Edge> mst = graph.MinimumSpanningTreeKruskal();

            Console.WriteLine($"MST for routing {requests.Count} service requests:");
            Console.WriteLine($"MST contains {mst.Count} edges (optimal route connections):");

            double totalDistance = 0.0;
            foreach (var edge in mst)
            {
                double distance = edge.Weight;
                totalDistance += distance;
                Console.WriteLine($"  {requests[edge.From].Title} <-> {requests[edge.To].Title}: {distance:F4} km");
            }

            Console.WriteLine($"\nTotal routing distance: {totalDistance:F4} km");
            Console.WriteLine("This MST represents the optimal route to visit all locations with minimum travel distance.");

            List<int> dfsRoute = graph.DepthFirstSearch(0);
            Console.WriteLine($"\nDFS route starting from '{requests[0].Title}':");
            foreach (int vertex in dfsRoute)
            {
                Console.WriteLine($"  -> {requests[vertex].Title}");
            }

            Console.WriteLine("MST routing test completed.\n");
        }

        /// <summary>
        /// Tests building a graph by team assignment
        /// </summary>
        public static void TestGraphByTeam()
        {
            Console.WriteLine("=== Testing Graph by Team Assignment ===");

            List<ServiceRequest> requests = new List<ServiceRequest>
            {
                new ServiceRequest("Team A - 1", "First Team A request", 1, -25.7479, 28.2293, "Team-A"),
                new ServiceRequest("Team A - 2", "Second Team A request", 2, -25.7480, 28.2294, "Team-A"),
                new ServiceRequest("Team B - 1", "First Team B request", 1, -25.7481, 28.2295, "Team-B"),
                new ServiceRequest("Team B - 2", "Second Team B request", 2, -25.7482, 28.2296, "Team-B"),
                new ServiceRequest("Team A - 3", "Third Team A request", 3, -25.7483, 28.2297, "Team-A")
            };

            Graph graph = GraphUtilities.BuildGraphByTeam(requests);

            Console.WriteLine($"Graph built by team assignment from {requests.Count} requests");
            Console.WriteLine($"Graph has {graph.VertexCount} vertices and {graph.EdgeCount} edges");

            List<Edge> allEdges = graph.GetAllEdges();
            Console.WriteLine($"\nEdges (connecting requests from same team):");
            foreach (var edge in allEdges)
            {
                Console.WriteLine($"  {requests[edge.From].Title} <-> {requests[edge.To].Title} " +
                                 $"(Team: {requests[edge.From].AssignedTeamId}, Distance: {edge.Weight:F4} km)");
            }

            Console.WriteLine("Graph by team test completed.\n");
        }

        /// <summary>
        /// Runs all graph tests
        /// </summary>
        public static void RunAllTests()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("Graph Data Structure Tests");
            Console.WriteLine("==========================================\n");

            TestBasicGraphOperations();
            TestDepthFirstSearch();
            TestBreadthFirstSearch();
            TestMinimumSpanningTree();
            TestGraphFromServiceRequests();
            TestMSTRouting();
            TestGraphByTeam();

            Console.WriteLine("==========================================");
            Console.WriteLine("All graph tests completed!");
            Console.WriteLine("==========================================");
        }
    }
}

