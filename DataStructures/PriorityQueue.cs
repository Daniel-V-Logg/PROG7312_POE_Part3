using MunicipalServiceApp.Models;

namespace MunicipalServiceApp.DataStructures
{
    /// <summary>
    /// A priority queue implementation for ServiceRequest objects.
    /// Uses a binary heap to efficiently manage requests by priority.
    /// Lower priority numbers (1 = high priority) are processed first.
    /// </summary>
    public class PriorityQueue
    {
        private readonly BinaryHeap<ServiceRequest> _heap;

        /// <summary>
        /// Gets the number of requests in the queue
        /// </summary>
        public int Count => _heap.Count;

        /// <summary>
        /// Gets a value indicating whether the queue is empty
        /// </summary>
        public bool IsEmpty => _heap.IsEmpty;

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class
        /// </summary>
        public PriorityQueue()
        {
            _heap = new BinaryHeap<ServiceRequest>(HeapType.MinHeap, CompareByPriority);
        }

        /// <summary>
        /// Comparison function for ServiceRequest by priority (1 = highest priority)
        /// </summary>
        private static int CompareByPriority(ServiceRequest x, ServiceRequest y)
        {
            return x.Priority.CompareTo(y.Priority);
        }

        /// <summary>
        /// Adds a service request to the priority queue
        /// </summary>
        /// <param name="request">The service request to add</param>
        public void Enqueue(ServiceRequest request)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            _heap.Insert(request);
        }

        /// <summary>
        /// Removes and returns the highest priority service request from the queue
        /// </summary>
        /// <returns>The highest priority service request</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when the queue is empty</exception>
        public ServiceRequest Dequeue()
        {
            return _heap.Extract();
        }

        /// <summary>
        /// Returns the highest priority service request without removing it
        /// </summary>
        /// <returns>The highest priority service request</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when the queue is empty</exception>
        public ServiceRequest Peek()
        {
            return _heap.Peek();
        }
    }
}

