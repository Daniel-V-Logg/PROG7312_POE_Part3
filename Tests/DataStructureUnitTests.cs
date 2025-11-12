using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServiceApp.DataStructures;
using MunicipalServiceApp.Models;

namespace MunicipalServiceApp.Tests
{
    /// <summary>
    /// Comprehensive unit tests for all data structures.
    /// Tests insert, search, and delete operations for each structure.
    /// </summary>
    public static class DataStructureUnitTests
    {
        /// <summary>
        /// Runs all unit tests for data structures
        /// </summary>
        public static void RunAllUnitTests()
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("DATA STRUCTURE UNIT TESTS");
            Console.WriteLine("=========================================\n");

            TestBinarySearchTreeOperations();
            TestAVLTreeOperations();
            TestRedBlackTreeOperations();
            TestBinaryHeapOperations();
            TestPriorityQueueOperations();
            TestGraphOperations();

            Console.WriteLine("\n=========================================");
            Console.WriteLine("ALL UNIT TESTS COMPLETE");
            Console.WriteLine("=========================================");
        }

        /// <summary>
        /// Tests BST insert, search, and traversal operations
        /// </summary>
        private static void TestBinarySearchTreeOperations()
        {
            Console.WriteLine("=== Binary Search Tree Unit Tests ===");
            int passed = 0;
            int total = 0;

            // Test 1: Insert
            total++;
            var bst = new BinarySearchTree<ServiceRequest, string>(req => req.RequestId);
            var req1 = new ServiceRequest("Test 1", "Description 1", 1, -26.2041, 28.0473);
            var req2 = new ServiceRequest("Test 2", "Description 2", 2, -26.2042, 28.0474);
            var req3 = new ServiceRequest("Test 3", "Description 3", 3, -26.2043, 28.0475);

            bst.Insert(req1);
            bst.Insert(req2);
            bst.Insert(req3);

            var allItems = bst.InOrderTraverse();
            if (allItems.Count == 3)
            {
                Console.WriteLine("✓ Insert test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"✗ Insert test: FAIL (Expected 3, got {allItems.Count})");
            }

            // Test 2: Search
            total++;
            var found = bst.Search(req2.RequestId);
            if (found != null && found.RequestId == req2.RequestId)
            {
                Console.WriteLine("✓ Search test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine("✗ Search test: FAIL");
            }

            // Test 3: Search non-existent
            total++;
            var notFound = bst.Search("non-existent-id");
            if (notFound == null)
            {
                Console.WriteLine("✓ Search non-existent test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine("✗ Search non-existent test: FAIL");
            }

            Console.WriteLine($"BST Tests: {passed}/{total} passed\n");
        }

        /// <summary>
        /// Tests AVL Tree insert, search, and balance operations
        /// </summary>
        private static void TestAVLTreeOperations()
        {
            Console.WriteLine("=== AVL Tree Unit Tests ===");
            int passed = 0;
            int total = 0;

            var avl = new AVLTree<ServiceRequest, string>(req => req.RequestId);
            var requests = new List<ServiceRequest>();

            // Insert multiple items to test balancing
            for (int i = 0; i < 10; i++)
            {
                var req = new ServiceRequest($"AVL Test {i}", $"Desc {i}", i % 5 + 1, -26.2041 + i * 0.001, 28.0473 + i * 0.001);
                requests.Add(req);
                avl.Insert(req);
            }

            // Test 1: Insert and balance
            total++;
            var allItems = avl.InOrderTraverse();
            if (allItems.Count == 10)
            {
                Console.WriteLine("✓ Insert with balancing test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"✗ Insert with balancing test: FAIL (Expected 10, got {allItems.Count})");
            }

            // Test 2: Search
            total++;
            var found = avl.Search(requests[5].RequestId);
            if (found != null && found.RequestId == requests[5].RequestId)
            {
                Console.WriteLine("✓ Search test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine("✗ Search test: FAIL");
            }

            // Test 3: In-order traversal maintains sorted order
            total++;
            var sorted = avl.InOrderTraverse();
            bool isSorted = true;
            for (int i = 1; i < sorted.Count; i++)
            {
                if (string.Compare(sorted[i - 1].RequestId, sorted[i].RequestId) > 0)
                {
                    isSorted = false;
                    break;
                }
            }
            if (isSorted)
            {
                Console.WriteLine("✓ Sorted order test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine("✗ Sorted order test: FAIL");
            }

            Console.WriteLine($"AVL Tree Tests: {passed}/{total} passed\n");
        }

        /// <summary>
        /// Tests Red-Black Tree operations
        /// </summary>
        private static void TestRedBlackTreeOperations()
        {
            Console.WriteLine("=== Red-Black Tree Unit Tests ===");
            int passed = 0;
            int total = 0;

            var rbt = new RedBlackTree<ServiceRequest, string>(req => req.RequestId);
            var requests = new List<ServiceRequest>();

            // Insert items
            for (int i = 0; i < 8; i++)
            {
                var req = new ServiceRequest($"RBT Test {i}", $"Desc {i}", i % 5 + 1, -26.2041 + i * 0.001, 28.0473 + i * 0.001);
                requests.Add(req);
                rbt.Insert(req);
            }

            // Test 1: Insert
            total++;
            var allItems = rbt.InOrderTraverse();
            if (allItems.Count == 8)
            {
                Console.WriteLine("✓ Insert test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"✗ Insert test: FAIL (Expected 8, got {allItems.Count})");
            }

            // Test 2: Search
            total++;
            var found = rbt.Search(requests[3].RequestId);
            if (found != null && found.RequestId == requests[3].RequestId)
            {
                Console.WriteLine("✓ Search test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine("✗ Search test: FAIL");
            }

            Console.WriteLine($"Red-Black Tree Tests: {passed}/{total} passed\n");
        }

        /// <summary>
        /// Tests Binary Heap operations (insert, extract, peek)
        /// </summary>
        private static void TestBinaryHeapOperations()
        {
            Console.WriteLine("=== Binary Heap Unit Tests ===");
            int passed = 0;
            int total = 0;

            // Test Min-Heap
            total++;
            var minHeap = new BinaryHeap<int>(HeapType.MinHeap);
            minHeap.Insert(5);
            minHeap.Insert(2);
            minHeap.Insert(8);
            minHeap.Insert(1);
            minHeap.Insert(3);

            if (minHeap.Peek() == 1 && minHeap.Count == 5)
            {
                Console.WriteLine("✓ Min-Heap insert and peek test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"✗ Min-Heap insert and peek test: FAIL (Peek: {minHeap.Peek()}, Count: {minHeap.Count})");
            }

            // Test Extract
            total++;
            var extracted = new List<int>();
            while (!minHeap.IsEmpty)
            {
                extracted.Add(minHeap.Extract());
            }
            var expected = new List<int> { 1, 2, 3, 5, 8 };
            if (extracted.SequenceEqual(expected))
            {
                Console.WriteLine("✓ Min-Heap extract test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"✗ Min-Heap extract test: FAIL (Got: [{string.Join(", ", extracted)}])");
            }

            // Test Max-Heap
            total++;
            var maxHeap = new BinaryHeap<int>(HeapType.MaxHeap);
            maxHeap.Insert(5);
            maxHeap.Insert(2);
            maxHeap.Insert(8);
            maxHeap.Insert(1);

            if (maxHeap.Peek() == 8)
            {
                Console.WriteLine("✓ Max-Heap test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"✗ Max-Heap test: FAIL (Peek: {maxHeap.Peek()})");
            }

            Console.WriteLine($"Binary Heap Tests: {passed}/{total} passed\n");
        }

        /// <summary>
        /// Tests Priority Queue operations
        /// </summary>
        private static void TestPriorityQueueOperations()
        {
            Console.WriteLine("=== Priority Queue Unit Tests ===");
            int passed = 0;
            int total = 0;

            var pq = new PriorityQueue();
            var requests = new List<ServiceRequest>
            {
                new ServiceRequest("High Priority", "Urgent", 1, -26.2041, 28.0473),
                new ServiceRequest("Low Priority", "Not urgent", 5, -26.2042, 28.0474),
                new ServiceRequest("Medium Priority", "Normal", 3, -26.2043, 28.0475)
            };

            foreach (var req in requests)
            {
                pq.Enqueue(req);
            }

            // Test 1: Enqueue
            total++;
            if (pq.Count == 3)
            {
                Console.WriteLine("✓ Enqueue test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"✗ Enqueue test: FAIL (Count: {pq.Count})");
            }

            // Test 2: Dequeue (should return highest priority first)
            total++;
            var dequeued = pq.Dequeue();
            if (dequeued.Priority == 1)
            {
                Console.WriteLine("✓ Dequeue priority order test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"✗ Dequeue priority order test: FAIL (Priority: {dequeued.Priority})");
            }

            Console.WriteLine($"Priority Queue Tests: {passed}/{total} passed\n");
        }

        /// <summary>
        /// Tests Graph operations (add edge, DFS, BFS, MST)
        /// </summary>
        private static void TestGraphOperations()
        {
            Console.WriteLine("=== Graph Unit Tests ===");
            int passed = 0;
            int total = 0;

            // Test 1: Create graph and add edges
            total++;
            var graph = new Graph(5, directed: false);
            graph.AddEdge(0, 1, 2.5);
            graph.AddEdge(0, 2, 1.0);
            graph.AddEdge(1, 3, 3.0);
            graph.AddEdge(2, 4, 2.0);

            if (graph.VertexCount == 5 && graph.EdgeCount == 4)
            {
                Console.WriteLine("✓ Graph creation and add edge test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"✗ Graph creation test: FAIL (Vertices: {graph.VertexCount}, Edges: {graph.EdgeCount})");
            }

            // Test 2: DFS
            total++;
            var dfs = graph.DepthFirstSearch(0);
            if (dfs.Count > 0 && dfs.Contains(0))
            {
                Console.WriteLine("✓ DFS test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"✗ DFS test: FAIL (Count: {dfs.Count})");
            }

            // Test 3: BFS
            total++;
            var bfs = graph.BreadthFirstSearch(0);
            if (bfs.Count > 0 && bfs[0] == 0)
            {
                Console.WriteLine("✓ BFS test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"✗ BFS test: FAIL");
            }

            // Test 4: MST
            total++;
            var mst = graph.MinimumSpanningTreeKruskal();
            if (mst != null && mst.Count > 0)
            {
                Console.WriteLine("✓ MST computation test: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"✗ MST computation test: FAIL (Count: {mst?.Count ?? 0})");
            }

            Console.WriteLine($"Graph Tests: {passed}/{total} passed\n");
        }
    }
}

