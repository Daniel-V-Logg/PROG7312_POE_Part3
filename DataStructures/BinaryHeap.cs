using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MunicipalServiceApp.DataStructures
{
    /// <summary>
    /// Enumeration for heap type (min-heap or max-heap)
    /// </summary>
    public enum HeapType
    {
        /// <summary>
        /// Min-heap: root contains the minimum value
        /// </summary>
        MinHeap,
        /// <summary>
        /// Max-heap: root contains the maximum value
        /// </summary>
        MaxHeap
    }

    /// <summary>
    /// A binary heap implementation that can function as either a min-heap or max-heap.
    /// Provides efficient insertion and extraction of the root element.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the heap</typeparam>
    public class BinaryHeap<T> where T : IComparable<T>
    {
        private readonly List<T> _heap;
        private readonly HeapType _heapType;
        private readonly Comparison<T> _comparison;

        /// <summary>
        /// Gets the number of elements in the heap
        /// </summary>
        public int Count => _heap.Count;

        /// <summary>
        /// Gets a value indicating whether the heap is empty
        /// </summary>
        public bool IsEmpty => _heap.Count == 0;

        /// <summary>
        /// Initializes a new instance of the BinaryHeap class with a comparison function
        /// </summary>
        /// <param name="heapType">The type of heap (MinHeap or MaxHeap)</param>
        /// <param name="comparison">A comparison function to determine element ordering</param>
        public BinaryHeap(HeapType heapType, Comparison<T> comparison)
        {
            _heap = new List<T>();
            _heapType = heapType;
            _comparison = comparison ?? throw new ArgumentNullException(nameof(comparison));
        }

        /// <summary>
        /// Initializes a new instance of the BinaryHeap class with IComparable elements
        /// </summary>
        /// <param name="heapType">The type of heap (MinHeap or MaxHeap)</param>
        
        
     
        public BinaryHeap(HeapType heapType)
        {
                _heap = new List<T>();
                _heapType = heapType;
                _comparison = (x, y) => x.CompareTo(y);
        }
        


        /// <summary>
        /// Inserts a new element into the heap and maintains heap property
        /// </summary>
        /// <param name="item">The element to insert</param>
        public void Insert(T item)
        {
            _heap.Add(item);
            HeapifyUp(_heap.Count - 1);
        }

        /// <summary>
        /// Removes and returns the root element (minimum for min-heap, maximum for max-heap)
        /// </summary>
        /// <returns>The root element</returns>
        /// <exception cref="InvalidOperationException">Thrown when the heap is empty</exception>
        public T Extract()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Cannot extract from an empty heap");
            }

            T root = _heap[0];
            T last = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            if (_heap.Count > 0)
            {
                _heap[0] = last;
                HeapifyDown(0);
            }

            return root;
        }

        /// <summary>
        /// Returns the root element without removing it
        /// </summary>
        /// <returns>The root element</returns>
        /// <exception cref="InvalidOperationException">Thrown when the heap is empty</exception>
        public T Peek()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Cannot peek at an empty heap");
            }
            return _heap[0];
        }

        /// <summary>
        /// Moves an element up the heap to maintain heap property
        /// </summary>
        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (ShouldSwap(parentIndex, index))
                {
                    Swap(parentIndex, index);
                    index = parentIndex;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Moves an element down the heap to maintain heap property
        /// </summary>
        private void HeapifyDown(int index)
        {
            while (true)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int targetIndex = index;

                if (leftChild < _heap.Count && ShouldSwap(targetIndex, leftChild))
                {
                    targetIndex = leftChild;
                }

                if (rightChild < _heap.Count && ShouldSwap(targetIndex, rightChild))
                {
                    targetIndex = rightChild;
                }

                if (targetIndex != index)
                {
                    Swap(index, targetIndex);
                    index = targetIndex;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Determines if two elements should be swapped based on heap type
        /// </summary>
        private bool ShouldSwap(int parentIndex, int childIndex)
        {
            int comparison = _comparison(_heap[parentIndex], _heap[childIndex]);
            if (_heapType == HeapType.MinHeap)
            {
                return comparison > 0;
            }
            else
            {
                return comparison < 0;
            }
        }

        /// <summary>
        /// Swaps two elements in the heap
        /// </summary>
        private void Swap(int index1, int index2)
        {
            T temp = _heap[index1];
            _heap[index1] = _heap[index2];
            _heap[index2] = temp;
        }
    }
}

