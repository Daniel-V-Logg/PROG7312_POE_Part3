using System;
using System.Collections.Generic;

namespace MunicipalServiceApp.DataStructures
{
    /// <summary>
    /// A binary search tree implementation that maintains sorted order based on a comparable key.
    /// Supports efficient insertion and search operations.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the tree</typeparam>
    /// <typeparam name="TKey">The type of the key used for comparison (must implement IComparable)</typeparam>
    public class BinarySearchTree<T, TKey> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// Represents a node in the binary search tree
        /// </summary>
        public class TreeNode
        {
            /// <summary>
            /// The data stored in this node
            /// </summary>
            public T Data { get; set; }

            /// <summary>
            /// The key used for comparison and ordering
            /// </summary>
            public TKey Key { get; set; }

            /// <summary>
            /// Reference to the left child node (contains keys less than this node's key)
            /// </summary>
            public TreeNode Left { get; set; }

            /// <summary>
            /// Reference to the right child node (contains keys greater than this node's key)
            /// </summary>
            public TreeNode Right { get; set; }

            /// <summary>
            /// Initializes a new instance of the TreeNode class
            /// </summary>
            /// <param name="data">The data to store in this node</param>
            /// <param name="key">The key used for comparison</param>
            public TreeNode(T data, TKey key)
            {
                Data = data;
                Key = key;
                Left = null;
                Right = null;
            }
        }

        /// <summary>
        /// Function to extract the key from a data item
        /// </summary>
        private readonly Func<T, TKey> _keySelector;

        /// <summary>
        /// The root node of the tree
        /// </summary>
        public TreeNode Root { get; private set; }

        /// <summary>
        /// Initializes a new instance of the BinarySearchTree class
        /// </summary>
        /// <param name="keySelector">A function that extracts the key from a data item</param>
        public BinarySearchTree(Func<T, TKey> keySelector)
        {
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            Root = null;
        }

        /// <summary>
        /// Inserts a new item into the tree, maintaining the binary search tree property
        /// </summary>
        /// <param name="data">The data to insert</param>
        public void Insert(T data)
        {
            TKey key = _keySelector(data);
            Root = Insert(Root, data, key);
        }

        /// <summary>
        /// Recursive helper method for insertion
        /// </summary>
        private TreeNode Insert(TreeNode node, T data, TKey key)
        {
            if (node == null)
            {
                return new TreeNode(data, key);
            }

            int comparison = key.CompareTo(node.Key);
            if (comparison < 0)
            {
                node.Left = Insert(node.Left, data, key);
            }
            else if (comparison > 0)
            {
                node.Right = Insert(node.Right, data, key);
            }

            return node;
        }

        /// <summary>
        /// Searches for an item with the specified key
        /// </summary>
        /// <param name="key">The key to search for</param>
        /// <returns>The data associated with the key, or default(T) if not found</returns>
        public T Search(TKey key)
        {
            TreeNode node = SearchNode(Root, key);
            return node != null ? node.Data : default(T);
        }

        /// <summary>
        /// Searches for a node with the specified key
        /// </summary>
        /// <param name="key">The key to search for</param>
        /// <returns>The node containing the key, or null if not found</returns>
        public TreeNode SearchNode(TKey key)
        {
            return SearchNode(Root, key);
        }

        /// <summary>
        /// Recursive helper method for search
        /// </summary>
        private TreeNode SearchNode(TreeNode node, TKey key)
        {
            if (node == null)
            {
                return null;
            }

            int comparison = key.CompareTo(node.Key);
            if (comparison == 0)
            {
                return node;
            }
            else if (comparison < 0)
            {
                return SearchNode(node.Left, key);
            }
            else
            {
                return SearchNode(node.Right, key);
            }
        }

        /// <summary>
        /// Performs an in-order traversal of the tree, which returns items in sorted order
        /// </summary>
        /// <returns>A list containing all items in sorted order by key</returns>
        public List<T> InOrderTraverse()
        {
            List<T> result = new List<T>();
            InOrderTraverse(Root, result);
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
    }
}

