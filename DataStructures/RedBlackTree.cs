using System;
using System.Collections.Generic;

namespace MunicipalServiceApp.DataStructures
{
    /// <summary>
    /// A Red-Black tree implementation.
    /// A self-balancing binary search tree that maintains balance through color properties and rotations.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the tree</typeparam>
    /// <typeparam name="TKey">The type of the key used for comparison (must implement IComparable)</typeparam>
    public class RedBlackTree<T, TKey> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// Enumeration for node colors in the Red-Black tree
        /// </summary>
        public enum NodeColor
        {
            Red,
            Black
        }

        /// <summary>
        /// Represents a node in the Red-Black tree
        /// </summary>
        public class RBNode
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
            public RBNode Left { get; set; }

            /// <summary>
            /// Reference to the right child node
            /// </summary>
            public RBNode Right { get; set; }

            /// <summary>
            /// Reference to the parent node
            /// </summary>
            public RBNode Parent { get; set; }

            /// <summary>
            /// The color of this node (Red or Black)
            /// </summary>
            public NodeColor Color { get; set; }

            /// <summary>
            /// Initializes a new instance of the RBNode class
            /// </summary>
            /// <param name="data">The data to store in this node</param>
            /// <param name="key">The key used for comparison</param>
            /// <param name="color">The initial color of the node</param>
            public RBNode(T data, TKey key, NodeColor color)
            {
                Data = data;
                Key = key;
                Color = color;
                Left = null;
                Right = null;
                Parent = null;
            }
        }

        /// <summary>
        /// Function to extract the key from a data item
        /// </summary>
        private readonly Func<T, TKey> _keySelector;

        /// <summary>
        /// The root node of the tree
        /// </summary>
        public RBNode Root { get; private set; }

        /// <summary>
        /// A sentinel node representing null (NIL) nodes
        /// </summary>
        private readonly RBNode NIL;

        /// <summary>
        /// Initializes a new instance of the RedBlackTree class
        /// </summary>
        /// <param name="keySelector">A function that extracts the key from a data item</param>
        public RedBlackTree(Func<T, TKey> keySelector)
        {
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            NIL = new RBNode(default(T), default(TKey), NodeColor.Black);
            Root = NIL;
        }

        /// <summary>
        /// Performs a left rotation around the given node
        /// </summary>
        private void RotateLeft(RBNode x)
        {
            RBNode y = x.Right;
            x.Right = y.Left;

            if (y.Left != NIL)
            {
                y.Left.Parent = x;
            }

            y.Parent = x.Parent;

            if (x.Parent == NIL)
            {
                Root = y;
            }
            else if (x == x.Parent.Left)
            {
                x.Parent.Left = y;
            }
            else
            {
                x.Parent.Right = y;
            }

            y.Left = x;
            x.Parent = y;
        }

        /// <summary>
        /// Performs a right rotation around the given node
        /// </summary>
        private void RotateRight(RBNode y)
        {
            RBNode x = y.Left;
            y.Left = x.Right;

            if (x.Right != NIL)
            {
                x.Right.Parent = y;
            }

            x.Parent = y.Parent;

            if (y.Parent == NIL)
            {
                Root = x;
            }
            else if (y == y.Parent.Right)
            {
                y.Parent.Right = x;
            }
            else
            {
                y.Parent.Left = x;
            }

            x.Right = y;
            y.Parent = x;
        }

        /// <summary>
        /// Fixes the Red-Black tree properties after insertion
        /// </summary>
        private void InsertFixup(RBNode z)
        {
            while (z.Parent.Color == NodeColor.Red)
            {
                if (z.Parent == z.Parent.Parent.Left)
                {
                    RBNode y = z.Parent.Parent.Right;
                    if (y.Color == NodeColor.Red)
                    {
                        z.Parent.Color = NodeColor.Black;
                        y.Color = NodeColor.Black;
                        z.Parent.Parent.Color = NodeColor.Red;
                        z = z.Parent.Parent;
                    }
                    else
                    {
                        if (z == z.Parent.Right)
                        {
                            z = z.Parent;
                            RotateLeft(z);
                        }
                        z.Parent.Color = NodeColor.Black;
                        z.Parent.Parent.Color = NodeColor.Red;
                        RotateRight(z.Parent.Parent);
                    }
                }
                else
                {
                    RBNode y = z.Parent.Parent.Left;
                    if (y.Color == NodeColor.Red)
                    {
                        z.Parent.Color = NodeColor.Black;
                        y.Color = NodeColor.Black;
                        z.Parent.Parent.Color = NodeColor.Red;
                        z = z.Parent.Parent;
                    }
                    else
                    {
                        if (z == z.Parent.Left)
                        {
                            z = z.Parent;
                            RotateRight(z);
                        }
                        z.Parent.Color = NodeColor.Black;
                        z.Parent.Parent.Color = NodeColor.Red;
                        RotateLeft(z.Parent.Parent);
                    }
                }
            }
            Root.Color = NodeColor.Black;
        }

        /// <summary>
        /// Inserts a new item into the tree and maintains Red-Black properties
        /// </summary>
        /// <param name="data">The data to insert</param>
        public void Insert(T data)
        {
            TKey key = _keySelector(data);
            RBNode z = new RBNode(data, key, NodeColor.Red);
            RBNode y = NIL;
            RBNode x = Root;

            while (x != NIL)
            {
                y = x;
                if (z.Key.CompareTo(x.Key) < 0)
                {
                    x = x.Left;
                }
                else
                {
                    x = x.Right;
                }
            }

            z.Parent = y;
            if (y == NIL)
            {
                Root = z;
            }
            else if (z.Key.CompareTo(y.Key) < 0)
            {
                y.Left = z;
            }
            else
            {
                y.Right = z;
            }

            z.Left = NIL;
            z.Right = NIL;
            z.Color = NodeColor.Red;
            InsertFixup(z);
        }

        /// <summary>
        /// Searches for an item with the specified key
        /// </summary>
        /// <param name="key">The key to search for</param>
        /// <returns>The data associated with the key, or default(T) if not found</returns>
        public T Search(TKey key)
        {
            RBNode node = SearchNode(key);
            return node != NIL ? node.Data : default(T);
        }

        /// <summary>
        /// Searches for a node with the specified key
        /// </summary>
        /// <param name="key">The key to search for</param>
        /// <returns>The node containing the key, or NIL if not found</returns>
        public RBNode SearchNode(TKey key)
        {
            RBNode x = Root;
            while (x != NIL)
            {
                int comparison = key.CompareTo(x.Key);
                if (comparison == 0)
                {
                    return x;
                }
                else if (comparison < 0)
                {
                    x = x.Left;
                }
                else
                {
                    x = x.Right;
                }
            }
            return NIL;
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
        private void InOrderTraverse(RBNode node, List<T> result)
        {
            if (node != NIL)
            {
                InOrderTraverse(node.Left, result);
                result.Add(node.Data);
                InOrderTraverse(node.Right, result);
            }
        }
    }
}

