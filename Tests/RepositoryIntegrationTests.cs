using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MunicipalServiceApp.Models;
using MunicipalServiceApp.Repositories;

namespace MunicipalServiceApp.Tests
{
    /// <summary>
    /// Integration tests for repository -> UI data flow (non-UI tests).
    /// Tests the complete data flow from repository to data structures.
    /// </summary>
    public static class RepositoryIntegrationTests
    {
        /// <summary>
        /// Runs all integration tests
        /// </summary>
        public static void RunAllIntegrationTests()
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("REPOSITORY INTEGRATION TESTS");
            Console.WriteLine("=========================================\n");

            TestRepositoryToBSTIntegration();
            TestRepositoryToHeapIntegration();
            TestRepositoryToGraphIntegration();
            TestDataPersistenceIntegration();
            TestSampleDataGeneration();

            Console.WriteLine("\n=========================================");
            Console.WriteLine("ALL INTEGRATION TESTS COMPLETE");
            Console.WriteLine("=========================================");
        }

        /// <summary>
        /// Tests integration between repository and BST
        /// </summary>
        private static void TestRepositoryToBSTIntegration()
        {
            Console.WriteLine("=== Repository -> BST Integration Test ===");

            // Create temporary repository
            string tempFile = Path.Combine(Path.GetTempPath(), $"test_requests_{Guid.NewGuid()}.json");
            var repository = new InMemoryServiceRequestRepository(tempFile);

            // Add test data
            var requests = new List<ServiceRequest>
            {
                new ServiceRequest("Test 1", "Description 1", 1, -26.2041, 28.0473),
                new ServiceRequest("Test 2", "Description 2", 2, -26.2042, 28.0474),
                new ServiceRequest("Test 3", "Description 3", 3, -26.2043, 28.0475)
            };

            foreach (var req in requests)
            {
                repository.Add(req);
            }
            repository.Save();

            // Load into BST
            var bst = new DataStructures.BinarySearchTree<ServiceRequest, string>(req => req.RequestId);
            var loadedRequests = repository.GetAll().ToList();
            foreach (var req in loadedRequests)
            {
                bst.Insert(req);
            }

            // Verify BST can find all requests
            bool allFound = true;
            foreach (var req in requests)
            {
                var found = bst.Search(req.RequestId);
                if (found == null || found.RequestId != req.RequestId)
                {
                    allFound = false;
                    break;
                }
            }

            Console.WriteLine($"Repository -> BST Integration: {(allFound ? "PASS" : "FAIL")}");
            Console.WriteLine($"  Requests added: {requests.Count}");
            Console.WriteLine($"  BST searches successful: {allFound}\n");

            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }

        /// <summary>
        /// Tests integration between repository and Heap (via Scheduler)
        /// </summary>
        private static void TestRepositoryToHeapIntegration()
        {
            Console.WriteLine("=== Repository -> Heap Integration Test ===");

            string tempFile = Path.Combine(Path.GetTempPath(), $"test_requests_{Guid.NewGuid()}.json");
            var repository = new InMemoryServiceRequestRepository(tempFile);

            // Add requests with different priorities
            var requests = new List<ServiceRequest>
            {
                new ServiceRequest("Low Priority", "Desc", 5, -26.2041, 28.0473),
                new ServiceRequest("High Priority", "Desc", 1, -26.2042, 28.0474),
                new ServiceRequest("Medium Priority", "Desc", 3, -26.2043, 28.0475)
            };

            foreach (var req in requests)
            {
                repository.Add(req);
            }

            // Load into scheduler (uses heap)
            var scheduler = new Scheduling.RequestScheduler();
            var loadedRequests = repository.GetAll().ToList();
            foreach (var req in loadedRequests)
            {
                scheduler.AddRequest(req);
            }

            // Get top requests (should be highest priority first)
            var topRequests = scheduler.GetTopRequests(2);
            bool correctOrder = topRequests.Count == 2 && topRequests[0].Priority == 1;

            Console.WriteLine($"Repository -> Heap Integration: {(correctOrder ? "PASS" : "FAIL")}");
            Console.WriteLine($"  Requests added: {requests.Count}");
            Console.WriteLine($"  Top priority extracted: {(topRequests.Count > 0 ? topRequests[0].Priority.ToString() : "N/A")}\n");

            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }

        /// <summary>
        /// Tests integration between repository and Graph
        /// </summary>
        private static void TestRepositoryToGraphIntegration()
        {
            Console.WriteLine("=== Repository -> Graph Integration Test ===");

            string tempFile = Path.Combine(Path.GetTempPath(), $"test_requests_{Guid.NewGuid()}.json");
            var repository = new InMemoryServiceRequestRepository(tempFile);

            // Add requests with coordinates
            var requests = new List<ServiceRequest>
            {
                new ServiceRequest("Location 1", "Desc", 1, -26.2041, 28.0473),
                new ServiceRequest("Location 2", "Desc", 2, -26.2042, 28.0474),
                new ServiceRequest("Location 3", "Desc", 3, -26.2043, 28.0475)
            };

            foreach (var req in requests)
            {
                repository.Add(req);
            }

            // Build graph from repository data
            var loadedRequests = repository.GetAll().ToList();
            var graph = DataStructures.GraphUtilities.BuildGraphFromRequests(loadedRequests, maxDistanceKm: 50.0);

            bool graphBuilt = graph.VertexCount == loadedRequests.Count && graph.EdgeCount > 0;

            Console.WriteLine($"Repository -> Graph Integration: {(graphBuilt ? "PASS" : "FAIL")}");
            Console.WriteLine($"  Requests: {loadedRequests.Count}");
            Console.WriteLine($"  Graph vertices: {graph.VertexCount}");
            Console.WriteLine($"  Graph edges: {graph.EdgeCount}\n");

            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }

        /// <summary>
        /// Tests data persistence (save and load)
        /// </summary>
        private static void TestDataPersistenceIntegration()
        {
            Console.WriteLine("=== Data Persistence Integration Test ===");

            string tempFile = Path.Combine(Path.GetTempPath(), $"test_requests_{Guid.NewGuid()}.json");
            var repository1 = new InMemoryServiceRequestRepository(tempFile);

            // Add and save
            var request = new ServiceRequest("Persistence Test", "Test description", 2, -26.2041, 28.0473);
            repository1.Add(request);
            repository1.Save();

            // Create new repository instance and load
            var repository2 = new InMemoryServiceRequestRepository(tempFile);
            repository2.Load();
            var loaded = repository2.GetById(request.RequestId);

            bool persistenceWorks = loaded != null && loaded.RequestId == request.RequestId && loaded.Title == request.Title;

            Console.WriteLine($"Data Persistence: {(persistenceWorks ? "PASS" : "FAIL")}");
            Console.WriteLine($"  Request saved and loaded: {persistenceWorks}\n");

            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }

        /// <summary>
        /// Tests sample data generation
        /// </summary>
        private static void TestSampleDataGeneration()
        {
            Console.WriteLine("=== Sample Data Generation Test ===");

            string tempFile = Path.Combine(Path.GetTempPath(), $"test_requests_{Guid.NewGuid()}.json");
            
            // Delete file to trigger sample data generation
            if (File.Exists(tempFile))
                File.Delete(tempFile);

            var repository = new InMemoryServiceRequestRepository(tempFile);
            repository.Load(); // Should generate sample data

            var requests = repository.GetAll().ToList();
            bool sampleGenerated = requests.Count > 0;

            Console.WriteLine($"Sample Data Generation: {(sampleGenerated ? "PASS" : "FAIL")}");
            Console.WriteLine($"  Sample requests generated: {requests.Count}\n");

            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}

