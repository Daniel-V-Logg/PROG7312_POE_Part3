using System;
using System.Collections.Generic;

namespace MunicipalServiceApp.DataStructures
{
    /// <summary>
    /// An AVL (Adelson-Velsky and Landis) tree implementation.
    /// A self-balancing binary search tree that maintains O(log n) height through rotations.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the tree</typeparam>
    /// <typeparam name="TKey">The type of the key used for comparison (must implement IComparable)</typeparam>
    public class AVLTree<T, TKey> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// Represents a node in the AVL tree
        /// </summary>
        public class AVLNode
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
            /// Reference to the left child node
            /// </summary>
            public AVLNode Left { get; set; }

            /// <summary>
            /// Reference to the right child node
            /// </summary>
            public AVLNode Right { get; set; }

            /// <summary>
            /// The height of this node in the tree
            /// </summary>
            public int Height { get; set; }

            /// <summary>
            /// Initializes a new instance of the AVLNode class
            /// </summary>
            /// <param name="data">The data to store in this node</param>
            /// <param name="key">The key used for comparison</param>
            public AVLNode(T data, TKey key)
            {
                Data = data;
                Key = key;
                Left = null;
                Right = null;
                Height = 1;
            }
        }

        /// <summary>
        /// Function to extract the key from a data item
        /// </summary>
        private readonly Func<T, TKey> _keySelector;

        /// <summary>
        /// The root node of the tree
        /// </summary>
        public AVLNode Root { get; private set; }

        /// <summary>
        /// Initializes a new instance of the AVLTree class
        /// </summary>
        /// <param name="keySelector">A function that extracts the key from a data item</param>
        public AVLTree(Func<T, TKey> keySelector)
        {
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            Root = null;
        }

        /// <summary>
        /// Gets the height of a node (returns 0 for null nodes)
        /// </summary>
        private int GetHeight(AVLNode node)
        {
            return node == null ? 0 : node.Height;
        }

        /// <summary>
        /// Calculates the balance factor of a node (difference between left and right subtree heights)
        /// </summary>
        private int GetBalanceFactor(AVLNode node)
        {
            if (node == null)
                return 0;
            return GetHeight(node.Left) - GetHeight(node.Right);
        }

        /// <summary>
        /// Updates the height of a node based on its children
        /// </summary>
        private void UpdateHeight(AVLNode node)
        {
            if (node != null)
            {
                node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
            }
        }

        /// <summary>
        /// Performs a right rotation to balance the tree
        /// </summary>
        private AVLNode RotateRight(AVLNode y)
        {
            AVLNode x = y.Left;
            AVLNode T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            UpdateHeight(y);
            UpdateHeight(x);

            return x;
        }

        /// <summary>
        /// Performs a left rotation to balance the tree
        /// </summary>
        private AVLNode RotateLeft(AVLNode x)
        {
            AVLNode y = x.Right;
            AVLNode T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            UpdateHeight(x);
            UpdateHeight(y);

            return y;
        }

        /// <summary>
        /// Inserts a new item into the tree and rebalances if necessary
        /// </summary>
        /// <param name="data">The data to insert</param>
        public void Insert(T data)
        {
            TKey key = _keySelector(data);
            Root = Insert(Root, data, key);
        }

        /// <summary>
        /// Recursive helper method for insertion with balancing
        /// </summary>
        private AVLNode Insert(AVLNode node, T data, TKey key)
        {
            if (node == null)
            {
                return new AVLNode(data, key);
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
            else
            {
                return node;
            }

            UpdateHeight(node);

            int balance = GetBalanceFactor(node);

            if (balance > 1 && key.CompareTo(node.Left.Key) < 0)
            {
                return RotateRight(node);
            }

            if (balance < -1 && key.CompareTo(node.Right.Key) > 0)
            {
                return RotateLeft(node);
            }

            if (balance > 1 && key.CompareTo(node.Left.Key) > 0)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            if (balance < -1 && key.CompareTo(node.Right.Key) < 0)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
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
            AVLNode node = SearchNode(Root, key);
            return node != null ? node.Data : default(T);
        }

        /// <summary>
        /// Searches for a node with the specified key
        /// </summary>
        /// <param name="key">The key to search for</param>
        /// <returns>The node containing the key, or null if not found</returns>
        public AVLNode SearchNode(TKey key)
        {
            return SearchNode(Root, key);
        }

        /// <summary>
        /// Recursive helper method for search
        /// </summary>
        private AVLNode SearchNode(AVLNode node, TKey key)
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
        /// Performs an in-order traversal of the tree
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
        private void InOrderTraverse(AVLNode node, List<T> result)
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

