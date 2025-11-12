using System;
using System.Collections.Generic;

namespace MunicipalServiceApp.DataStructures
{
    /// <summary>
    /// A basic binary tree data structure with generic type support.
    /// Provides insert and traverse operations.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the tree nodes</typeparam>
    public class BinaryTree<T>
    {
        /// <summary>
        /// Represents a node in the binary tree
        /// </summary>
        public class TreeNode
        {
            /// <summary>
            /// The data stored in this node
            /// </summary>
            public T Data { get; set; }

            /// <summary>
            /// Reference to the left child node
            /// </summary>
            public TreeNode Left { get; set; }

            /// <summary>
            /// Reference to the right child node
            /// </summary>
            public TreeNode Right { get; set; }

            /// <summary>
            /// Initializes a new instance of the TreeNode class
            /// </summary>
            /// <param name="data">The data to store in this node</param>
            public TreeNode(T data)
            {
                Data = data;
                Left = null;
                Right = null;
            }
        }

        /// <summary>
        /// The root node of the tree
        /// </summary>
        public TreeNode Root { get; private set; }

        /// <summary>
        /// Initializes a new instance of the BinaryTree class
        /// </summary>
        public BinaryTree()
        {
            Root = null;
        }

        /// <summary>
        /// Inserts a new value into the tree.
        /// Uses level-order insertion to maintain a complete binary tree structure.
        /// </summary>
        /// <param name="data">The data to insert</param>
        public void Insert(T data)
        {
            if (Root == null)
            {
                Root = new TreeNode(data);
                return;
            }

            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(Root);

            while (queue.Count > 0)
            {
                TreeNode current = queue.Dequeue();

                if (current.Left == null)
                {
                    current.Left = new TreeNode(data);
                    return;
                }
                else if (current.Right == null)
                {
                    current.Right = new TreeNode(data);
                    return;
                }
                else
                {
                    queue.Enqueue(current.Left);
                    queue.Enqueue(current.Right);
                }
            }
        }

        /// <summary>
        /// Performs an in-order traversal of the tree and returns the results as a list
        /// </summary>
        /// <returns>A list containing all nodes in in-order sequence</returns>
        public List<T> InOrderTraverse()
        {
            List<T> result = new List<T>();
            InOrderTraverse(Root, result);
            return result;
        }

        /// <summary>
        /// Performs a pre-order traversal of the tree and returns the results as a list
        /// </summary>
        /// <returns>A list containing all nodes in pre-order sequence</returns>
        public List<T> PreOrderTraverse()
        {
            List<T> result = new List<T>();
            PreOrderTraverse(Root, result);
            return result;
        }

        /// <summary>
        /// Performs a post-order traversal of the tree and returns the results as a list
        /// </summary>
        /// <returns>A list containing all nodes in post-order sequence</returns>
        public List<T> PostOrderTraverse()
        {
            List<T> result = new List<T>();
            PostOrderTraverse(Root, result);
            return result;
        }

        /// <summary>
        /// Helper method for in-order traversal
        /// </summary>
        private void InOrderTraverse(TreeNode node, List<T> result)
        {
            if (node != null)
            {
                InOrderTraverse(node.Left, result);
                result.Add(node.Data);
                InOrderTraverse(node.Right, result);
            }
        }

        /// <summary>
        /// Helper method for pre-order traversal
        /// </summary>
        private void PreOrderTraverse(TreeNode node, List<T> result)
        {
            if (node != null)
            {
                result.Add(node.Data);
                PreOrderTraverse(node.Left, result);
                PreOrderTraverse(node.Right, result);
            }
        }

        /// <summary>
        /// Helper method for post-order traversal
        /// </summary>
        private void PostOrderTraverse(TreeNode node, List<T> result)
        {
            if (node != null)
            {
                PostOrderTraverse(node.Left, result);
                PostOrderTraverse(node.Right, result);
                result.Add(node.Data);
            }
        }
    }
}

