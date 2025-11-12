using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServiceApp.DataStructures;
using MunicipalServiceApp.Models;
using MunicipalServiceApp.Scheduling;

namespace MunicipalServiceApp.Tests
{
    /// <summary>
    /// Unit tests for binary heap, priority queue, and scheduler implementations.
    /// Validates correct ordering and scheduling of service requests.
    /// </summary>
    public class HeapAndSchedulerTests
    {
        /// <summary>
        /// Tests the BinaryHeap with integer values (min-heap)
        /// </summary>
        public static void TestBinaryHeapMin()
        {
            Console.WriteLine("=== Testing BinaryHeap (Min-Heap) ===");
            BinaryHeap<int> heap = new BinaryHeap<int>(HeapType.MinHeap);

            heap.Insert(5);
            heap.Insert(2);
            heap.Insert(8);
            heap.Insert(1);
            heap.Insert(3);

            Console.WriteLine($"Heap count: {heap.Count}");
            Console.WriteLine($"Peek (should be 1): {heap.Peek()}");

            List<int> extracted = new List<int>();
            while (!heap.IsEmpty)
            {
                extracted.Add(heap.Extract());
            }

            Console.WriteLine($"Extracted order: {string.Join(", ", extracted)}");
            bool isCorrect = extracted.SequenceEqual(new[] { 1, 2, 3, 5, 8 });
            Console.WriteLine($"Order is correct: {isCorrect}");
            Console.WriteLine("BinaryHeap (Min-Heap) test completed.\n");
        }

        /// <summary>
        /// Tests the BinaryHeap with integer values (max-heap)
        /// </summary>
        public static void TestBinaryHeapMax()
        {
            Console.WriteLine("=== Testing BinaryHeap (Max-Heap) ===");
            BinaryHeap<int> heap = new BinaryHeap<int>(HeapType.MaxHeap);

            heap.Insert(5);
            heap.Insert(2);
            heap.Insert(8);
            heap.Insert(1);
            heap.Insert(3);

            Console.WriteLine($"Heap count: {heap.Count}");
            Console.WriteLine($"Peek (should be 8): {heap.Peek()}");

            List<int> extracted = new List<int>();
            while (!heap.IsEmpty)
            {
                extracted.Add(heap.Extract());
            }

            Console.WriteLine($"Extracted order: {string.Join(", ", extracted)}");
            bool isCorrect = extracted.SequenceEqual(new[] { 8, 5, 3, 2, 1 });
            Console.WriteLine($"Order is correct: {isCorrect}");
            Console.WriteLine("BinaryHeap (Max-Heap) test completed.\n");
        }

        /// <summary>
        /// Tests PriorityQueue with ServiceRequest objects
        /// Validates that requests are processed in priority order (1 = highest)
        /// </summary>
        public static void TestPriorityQueue()
        {
            Console.WriteLine("=== Testing PriorityQueue with ServiceRequest ===");
            PriorityQueue queue = new PriorityQueue();

            ServiceRequest req1 = new ServiceRequest("Low Priority", "Low priority task", 5, -25.7479, 28.2293);
            ServiceRequest req2 = new ServiceRequest("High Priority", "High priority task", 1, -25.7480, 28.2294);
            ServiceRequest req3 = new ServiceRequest("Medium Priority", "Medium priority task", 3, -25.7481, 28.2295);
            ServiceRequest req4 = new ServiceRequest("Critical", "Critical task", 1, -25.7482, 28.2296);
            ServiceRequest req5 = new ServiceRequest("Very Low", "Very low priority", 4, -25.7483, 28.2297);

            queue.Enqueue(req1);
            queue.Enqueue(req2);
            queue.Enqueue(req3);
            queue.Enqueue(req4);
            queue.Enqueue(req5);

            Console.WriteLine($"Queue count: {queue.Count}");

            List<ServiceRequest> processed = new List<ServiceRequest>();
            while (!queue.IsEmpty)
            {
                ServiceRequest next = queue.Dequeue();
                processed.Add(next);
                Console.WriteLine($"Dequeued: Priority {next.Priority} - {next.Title}");
            }

            bool isCorrect = processed[0].Priority <= processed[1].Priority &&
                            processed[1].Priority <= processed[2].Priority &&
                            processed[2].Priority <= processed[3].Priority &&
                            processed[3].Priority <= processed[4].Priority;

            Console.WriteLine($"Order is correct (ascending priority): {isCorrect}");
            Console.WriteLine("PriorityQueue test completed.\n");
        }

        /// <summary>
        /// Tests the RequestScheduler with team-specific requests
        /// </summary>
        public static void TestScheduler()
        {
            Console.WriteLine("=== Testing RequestScheduler ===");
            IScheduler scheduler = new RequestScheduler();

            ServiceRequest req1 = new ServiceRequest("Team A - High", "High priority for Team A", 1, -25.7479, 28.2293, "Team-A");
            ServiceRequest req2 = new ServiceRequest("Team A - Low", "Low priority for Team A", 5, -25.7480, 28.2294, "Team-A");
            ServiceRequest req3 = new ServiceRequest("Team B - Medium", "Medium priority for Team B", 3, -25.7481, 28.2295, "Team-B");
            ServiceRequest req4 = new ServiceRequest("Team A - Critical", "Critical for Team A", 1, -25.7482, 28.2296, "Team-A");
            ServiceRequest req5 = new ServiceRequest("Unassigned", "Unassigned request", 2, -25.7483, 28.2297);
            ServiceRequest req6 = new ServiceRequest("Team B - High", "High priority for Team B", 2, -25.7484, 28.2298, "Team-B");

            scheduler.AddRequest(req1);
            scheduler.AddRequest(req2);
            scheduler.AddRequest(req3);
            scheduler.AddRequest(req4);
            scheduler.AddRequest(req5);
            scheduler.AddRequest(req6);

            Console.WriteLine($"Total requests: {scheduler.GetTotalRequestCount()}");

            List<ServiceRequest> teamARequests = scheduler.GetTopRequestsForTeam("Team-A", 3);
            Console.WriteLine($"\nTop 3 requests for Team-A:");
            foreach (var req in teamARequests)
            {
                Console.WriteLine($"  Priority {req.Priority}: {req.Title}");
            }

            bool teamAOrderCorrect = teamARequests.Count >= 2 &&
                                    teamARequests[0].Priority <= teamARequests[1].Priority;
            Console.WriteLine($"Team-A order is correct: {teamAOrderCorrect}");

            List<ServiceRequest> topGlobal = scheduler.GetTopRequests(4);
            Console.WriteLine($"\nTop 4 requests globally:");
            foreach (var req in topGlobal)
            {
                Console.WriteLine($"  Priority {req.Priority}: {req.Title} (Team: {req.AssignedTeamId ?? "Unassigned"})");
            }

            bool globalOrderCorrect = topGlobal.Count >= 2 &&
                                    topGlobal[0].Priority <= topGlobal[1].Priority &&
                                    topGlobal[1].Priority <= topGlobal[2].Priority;
            Console.WriteLine($"Global order is correct: {globalOrderCorrect}");

            Console.WriteLine("RequestScheduler test completed.\n");
        }

        /// <summary>
        /// Tests scheduler with multiple teams and validates correct prioritization
        /// </summary>
        public static void TestSchedulerMultipleTeams()
        {
            Console.WriteLine("=== Testing RequestScheduler with Multiple Teams ===");
            IScheduler scheduler = new RequestScheduler();

            ServiceRequest[] requests = new ServiceRequest[]
            {
                new ServiceRequest("A1", "Team A priority 1", 1, -25.7479, 28.2293, "Team-A"),
                new ServiceRequest("A2", "Team A priority 2", 2, -25.7480, 28.2294, "Team-A"),
                new ServiceRequest("A3", "Team A priority 3", 3, -25.7481, 28.2295, "Team-A"),
                new ServiceRequest("B1", "Team B priority 1", 1, -25.7482, 28.2296, "Team-B"),
                new ServiceRequest("B2", "Team B priority 2", 2, -25.7483, 28.2297, "Team-B"),
                new ServiceRequest("C1", "Team C priority 1", 1, -25.7484, 28.2298, "Team-C"),
            };

            foreach (var req in requests)
            {
                scheduler.AddRequest(req);
            }

            List<ServiceRequest> teamATop2 = scheduler.GetTopRequestsForTeam("Team-A", 2);
            Console.WriteLine($"Team-A top 2 requests:");
            foreach (var req in teamATop2)
            {
                Console.WriteLine($"  Priority {req.Priority}: {req.Title}");
            }

            bool teamACorrect = teamATop2.Count == 2 && 
                               teamATop2[0].Priority == 1 && 
                               teamATop2[1].Priority == 2;
            Console.WriteLine($"Team-A top 2 are correct: {teamACorrect}");

            List<ServiceRequest> teamBTop2 = scheduler.GetTopRequestsForTeam("Team-B", 2);
            Console.WriteLine($"\nTeam-B top 2 requests:");
            foreach (var req in teamBTop2)
            {
                Console.WriteLine($"  Priority {req.Priority}: {req.Title}");
            }

            bool teamBCorrect = teamBTop2.Count == 2 && 
                               teamBTop2[0].Priority == 1 && 
                               teamBTop2[1].Priority == 2;
            Console.WriteLine($"Team-B top 2 are correct: {teamBCorrect}");

            Console.WriteLine("Multiple teams test completed.\n");
        }

        /// <summary>
        /// Runs all heap and scheduler tests
        /// </summary>
        public static void RunAllTests()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("Heap and Scheduler Tests");
            Console.WriteLine("==========================================\n");

            TestBinaryHeapMin();
            TestBinaryHeapMax();
            TestPriorityQueue();
            TestScheduler();
            TestSchedulerMultipleTeams();

            Console.WriteLine("==========================================");
            Console.WriteLine("All heap and scheduler tests completed!");
            Console.WriteLine("==========================================");
        }
    }
}

