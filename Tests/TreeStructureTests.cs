using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServiceApp.DataStructures;
using MunicipalServiceApp.Models;

namespace MunicipalServiceApp.Tests
{
    /// <summary>
    /// Unit tests for tree data structures (BinaryTree, BinarySearchTree, AVLTree, RedBlackTree)
    /// Demonstrates insert and search operations with ServiceRequest objects.
    /// </summary>
    public class TreeStructureTests
    {
        /// <summary>
        /// Tests the basic BinaryTree implementation
        /// </summary>
        public static void TestBinaryTree()
        {
            Console.WriteLine("=== Testing BinaryTree ===");
            BinaryTree<int> tree = new BinaryTree<int>();

            tree.Insert(10);
            tree.Insert(20);
            tree.Insert(30);
            tree.Insert(40);
            tree.Insert(50);

            List<int> inOrder = tree.InOrderTraverse();
            Console.WriteLine($"In-order traversal: {string.Join(", ", inOrder)}");

            List<int> preOrder = tree.PreOrderTraverse();
            Console.WriteLine($"Pre-order traversal: {string.Join(", ", preOrder)}");

            List<int> postOrder = tree.PostOrderTraverse();
            Console.WriteLine($"Post-order traversal: {string.Join(", ", postOrder)}");

            Console.WriteLine($"Tree contains {inOrder.Count} nodes");
            Console.WriteLine("BinaryTree test completed.\n");
        }

        /// <summary>
        /// Tests BinarySearchTree with ServiceRequest keyed by RequestId
        /// </summary>
        public static void TestBinarySearchTreeByRequestId()
        {
            Console.WriteLine("=== Testing BinarySearchTree (keyed by RequestId) ===");
            BinarySearchTree<ServiceRequest, string> bst = new BinarySearchTree<ServiceRequest, string>(
                req => req.RequestId
            );

            ServiceRequest req1 = new ServiceRequest("Pothole Repair", "Large pothole on Main St", 1, -25.7479, 28.2293);
            ServiceRequest req2 = new ServiceRequest("Street Light Out", "Light not working on Oak Ave", 2, -25.7480, 28.2294);
            ServiceRequest req3 = new ServiceRequest("Garbage Collection", "Missed pickup on Elm St", 3, -25.7481, 28.2295);

            bst.Insert(req1);
            bst.Insert(req2);
            bst.Insert(req3);

            ServiceRequest found = bst.Search(req1.RequestId);
            Console.WriteLine($"Search for RequestId '{req1.RequestId}': {(found != null ? "FOUND - " + found.Title : "NOT FOUND")}");

            found = bst.Search(req2.RequestId);
            Console.WriteLine($"Search for RequestId '{req2.RequestId}': {(found != null ? "FOUND - " + found.Title : "NOT FOUND")}");

            found = bst.Search("non-existent-id");
            Console.WriteLine($"Search for non-existent RequestId: {(found != null ? "FOUND" : "NOT FOUND")}");

            List<ServiceRequest> sorted = bst.InOrderTraverse();
            Console.WriteLine($"In-order traversal (sorted by RequestId): {sorted.Count} items");
            foreach (var req in sorted)
            {
                Console.WriteLine($"  - {req.RequestId}: {req.Title}");
            }

            Console.WriteLine("BinarySearchTree (RequestId) test completed.\n");
        }

        /// <summary>
        /// Tests BinarySearchTree with ServiceRequest keyed by DateSubmitted
        /// </summary>
        public static void TestBinarySearchTreeByDate()
        {
            Console.WriteLine("=== Testing BinarySearchTree (keyed by DateSubmitted) ===");
            BinarySearchTree<ServiceRequest, DateTime> bst = new BinarySearchTree<ServiceRequest, DateTime>(
                req => req.DateSubmitted
            );

            ServiceRequest req1 = new ServiceRequest("Issue 1", "First issue", 1, -25.7479, 28.2293);
            System.Threading.Thread.Sleep(10);
            ServiceRequest req2 = new ServiceRequest("Issue 2", "Second issue", 2, -25.7480, 28.2294);
            System.Threading.Thread.Sleep(10);
            ServiceRequest req3 = new ServiceRequest("Issue 3", "Third issue", 3, -25.7481, 28.2295);

            bst.Insert(req1);
            bst.Insert(req2);
            bst.Insert(req3);

            ServiceRequest found = bst.Search(req2.DateSubmitted);
            Console.WriteLine($"Search for DateSubmitted '{req2.DateSubmitted}': {(found != null ? "FOUND - " + found.Title : "NOT FOUND")}");

            List<ServiceRequest> sorted = bst.InOrderTraverse();
            Console.WriteLine($"In-order traversal (sorted by DateSubmitted): {sorted.Count} items");
            foreach (var req in sorted)
            {
                Console.WriteLine($"  - {req.DateSubmitted:yyyy-MM-dd HH:mm:ss}: {req.Title}");
            }

            Console.WriteLine("BinarySearchTree (DateSubmitted) test completed.\n");
        }

        /// <summary>
        /// Tests AVLTree with ServiceRequest keyed by RequestId
        /// </summary>
        public static void TestAVLTreeByRequestId()
        {
            Console.WriteLine("=== Testing AVLTree (keyed by RequestId) ===");
            AVLTree<ServiceRequest, string> avl = new AVLTree<ServiceRequest, string>(
                req => req.RequestId
            );

            ServiceRequest req1 = new ServiceRequest("AVL Test 1", "First AVL test", 1, -25.7479, 28.2293);
            ServiceRequest req2 = new ServiceRequest("AVL Test 2", "Second AVL test", 2, -25.7480, 28.2294);
            ServiceRequest req3 = new ServiceRequest("AVL Test 3", "Third AVL test", 3, -25.7481, 28.2295);
            ServiceRequest req4 = new ServiceRequest("AVL Test 4", "Fourth AVL test", 4, -25.7482, 28.2296);
            ServiceRequest req5 = new ServiceRequest("AVL Test 5", "Fifth AVL test", 5, -25.7483, 28.2297);

            avl.Insert(req1);
            avl.Insert(req2);
            avl.Insert(req3);
            avl.Insert(req4);
            avl.Insert(req5);

            ServiceRequest found = avl.Search(req3.RequestId);
            Console.WriteLine($"Search for RequestId '{req3.RequestId}': {(found != null ? "FOUND - " + found.Title : "NOT FOUND")}");

            found = avl.Search(req5.RequestId);
            Console.WriteLine($"Search for RequestId '{req5.RequestId}': {(found != null ? "FOUND - " + found.Title : "NOT FOUND")}");

            List<ServiceRequest> sorted = avl.InOrderTraverse();
            Console.WriteLine($"In-order traversal (sorted by RequestId): {sorted.Count} items");
            foreach (var req in sorted)
            {
                Console.WriteLine($"  - {req.RequestId}: {req.Title}");
            }

            Console.WriteLine("AVLTree (RequestId) test completed.\n");
        }

        /// <summary>
        /// Tests AVLTree with ServiceRequest keyed by DateSubmitted
        /// </summary>
        public static void TestAVLTreeByDate()
        {
            Console.WriteLine("=== Testing AVLTree (keyed by DateSubmitted) ===");
            AVLTree<ServiceRequest, DateTime> avl = new AVLTree<ServiceRequest, DateTime>(
                req => req.DateSubmitted
            );

            ServiceRequest req1 = new ServiceRequest("Date Test 1", "First date test", 1, -25.7479, 28.2293);
            System.Threading.Thread.Sleep(10);
            ServiceRequest req2 = new ServiceRequest("Date Test 2", "Second date test", 2, -25.7480, 28.2294);
            System.Threading.Thread.Sleep(10);
            ServiceRequest req3 = new ServiceRequest("Date Test 3", "Third date test", 3, -25.7481, 28.2295);

            avl.Insert(req1);
            avl.Insert(req2);
            avl.Insert(req3);

            ServiceRequest found = avl.Search(req2.DateSubmitted);
            Console.WriteLine($"Search for DateSubmitted '{req2.DateSubmitted}': {(found != null ? "FOUND - " + found.Title : "NOT FOUND")}");

            List<ServiceRequest> sorted = avl.InOrderTraverse();
            Console.WriteLine($"In-order traversal (sorted by DateSubmitted): {sorted.Count} items");
            foreach (var req in sorted)
            {
                Console.WriteLine($"  - {req.DateSubmitted:yyyy-MM-dd HH:mm:ss}: {req.Title}");
            }

            Console.WriteLine("AVLTree (DateSubmitted) test completed.\n");
        }

        /// <summary>
        /// Tests RedBlackTree with ServiceRequest keyed by RequestId
        /// </summary>
        public static void TestRedBlackTreeByRequestId()
        {
            Console.WriteLine("=== Testing RedBlackTree (keyed by RequestId) ===");
            RedBlackTree<ServiceRequest, string> rbt = new RedBlackTree<ServiceRequest, string>(
                req => req.RequestId
            );

            ServiceRequest req1 = new ServiceRequest("RB Test 1", "First RB test", 1, -25.7479, 28.2293);
            ServiceRequest req2 = new ServiceRequest("RB Test 2", "Second RB test", 2, -25.7480, 28.2294);
            ServiceRequest req3 = new ServiceRequest("RB Test 3", "Third RB test", 3, -25.7481, 28.2295);
            ServiceRequest req4 = new ServiceRequest("RB Test 4", "Fourth RB test", 4, -25.7482, 28.2296);

            rbt.Insert(req1);
            rbt.Insert(req2);
            rbt.Insert(req3);
            rbt.Insert(req4);

            ServiceRequest found = rbt.Search(req2.RequestId);
            Console.WriteLine($"Search for RequestId '{req2.RequestId}': {(found != null ? "FOUND - " + found.Title : "NOT FOUND")}");

            found = rbt.Search(req4.RequestId);
            Console.WriteLine($"Search for RequestId '{req4.RequestId}': {(found != null ? "FOUND - " + found.Title : "NOT FOUND")}");

            found = rbt.Search("non-existent-id");
            Console.WriteLine($"Search for non-existent RequestId: {(found != null ? "FOUND" : "NOT FOUND")}");

            List<ServiceRequest> sorted = rbt.InOrderTraverse();
            Console.WriteLine($"In-order traversal (sorted by RequestId): {sorted.Count} items");
            foreach (var req in sorted)
            {
                Console.WriteLine($"  - {req.RequestId}: {req.Title}");
            }

            Console.WriteLine("RedBlackTree (RequestId) test completed.\n");
        }

        /// <summary>
        /// Tests RedBlackTree with ServiceRequest keyed by DateSubmitted
        /// </summary>
        public static void TestRedBlackTreeByDate()
        {
            Console.WriteLine("=== Testing RedBlackTree (keyed by DateSubmitted) ===");
            RedBlackTree<ServiceRequest, DateTime> rbt = new RedBlackTree<ServiceRequest, DateTime>(
                req => req.DateSubmitted
            );

            ServiceRequest req1 = new ServiceRequest("RB Date 1", "First RB date test", 1, -25.7479, 28.2293);
            System.Threading.Thread.Sleep(10);
            ServiceRequest req2 = new ServiceRequest("RB Date 2", "Second RB date test", 2, -25.7480, 28.2294);
            System.Threading.Thread.Sleep(10);
            ServiceRequest req3 = new ServiceRequest("RB Date 3", "Third RB date test", 3, -25.7481, 28.2295);

            rbt.Insert(req1);
            rbt.Insert(req2);
            rbt.Insert(req3);

            ServiceRequest found = rbt.Search(req2.DateSubmitted);
            Console.WriteLine($"Search for DateSubmitted '{req2.DateSubmitted}': {(found != null ? "FOUND - " + found.Title : "NOT FOUND")}");

            List<ServiceRequest> sorted = rbt.InOrderTraverse();
            Console.WriteLine($"In-order traversal (sorted by DateSubmitted): {sorted.Count} items");
            foreach (var req in sorted)
            {
                Console.WriteLine($"  - {req.DateSubmitted:yyyy-MM-dd HH:mm:ss}: {req.Title}");
            }

            Console.WriteLine("RedBlackTree (DateSubmitted) test completed.\n");
        }

        /// <summary>
        /// Runs all tree structure tests
        /// </summary>
        public static void RunAllTests()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("Tree Data Structure Tests");
            Console.WriteLine("==========================================\n");

            TestBinaryTree();
            TestBinarySearchTreeByRequestId();
            TestBinarySearchTreeByDate();
            TestAVLTreeByRequestId();
            TestAVLTreeByDate();
            TestRedBlackTreeByRequestId();
            TestRedBlackTreeByDate();

            Console.WriteLine("==========================================");
            Console.WriteLine("All tree structure tests completed!");
            Console.WriteLine("==========================================");
        }
    }
}

