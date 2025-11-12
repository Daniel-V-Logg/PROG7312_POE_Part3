using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServiceApp.DataStructures;
using MunicipalServiceApp.Models;

namespace MunicipalServiceApp.Scheduling
{
    /// <summary>
    /// Implementation of IScheduler that uses a priority heap to schedule service requests.
    /// Maintains separate priority queues for each team and a global queue for all requests.
    /// </summary>
    public class RequestScheduler : IScheduler
    {
        private readonly PriorityQueue _globalQueue;
        private readonly Dictionary<string, PriorityQueue> _teamQueues;

        /// <summary>
        /// Initializes a new instance of the RequestScheduler class
        /// </summary>
        public RequestScheduler()
        {
            _globalQueue = new PriorityQueue();
            _teamQueues = new Dictionary<string, PriorityQueue>();
        }

        /// <summary>
        /// Adds a service request to the scheduler.
        /// The request is added to both the global queue and the team-specific queue (if assigned).
        /// </summary>
        /// <param name="request">The service request to add</param>
        public void AddRequest(ServiceRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _globalQueue.Enqueue(request);

            string teamId = request.AssignedTeamId;
            if (!string.IsNullOrEmpty(teamId))
            {
                if (!_teamQueues.ContainsKey(teamId))
                {
                    _teamQueues[teamId] = new PriorityQueue();
                }
                _teamQueues[teamId].Enqueue(request);
            }
        }

        /// <summary>
        /// Gets the top N highest priority requests for a specific team
        /// </summary>
        /// <param name="teamId">The ID of the team</param>
        /// <param name="count">The number of requests to retrieve</param>
        /// <returns>A list of the top N highest priority requests for the team</returns>
        public List<ServiceRequest> GetTopRequestsForTeam(string teamId, int count)
        {
            if (string.IsNullOrEmpty(teamId))
            {
                throw new ArgumentException("Team ID cannot be null or empty", nameof(teamId));
            }

            if (count <= 0)
            {
                throw new ArgumentException("Count must be greater than zero", nameof(count));
            }

            List<ServiceRequest> results = new List<ServiceRequest>();
            PriorityQueue tempQueue = new PriorityQueue();

            if (!_teamQueues.ContainsKey(teamId))
            {
                return results;
            }

            PriorityQueue teamQueue = _teamQueues[teamId];

            while (!teamQueue.IsEmpty && results.Count < count)
            {
                ServiceRequest request = teamQueue.Dequeue();
                results.Add(request);
                tempQueue.Enqueue(request);
            }

            while (!tempQueue.IsEmpty)
            {
                teamQueue.Enqueue(tempQueue.Dequeue());
            }

            return results;
        }

        /// <summary>
        /// Gets the top N highest priority requests across all teams
        /// </summary>
        /// <param name="count">The number of requests to retrieve</param>
        /// <returns>A list of the top N highest priority requests</returns>
        public List<ServiceRequest> GetTopRequests(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Count must be greater than zero", nameof(count));
            }

            List<ServiceRequest> results = new List<ServiceRequest>();
            PriorityQueue tempQueue = new PriorityQueue();

            while (!_globalQueue.IsEmpty && results.Count < count)
            {
                ServiceRequest request = _globalQueue.Dequeue();
                results.Add(request);
                tempQueue.Enqueue(request);
            }

            while (!tempQueue.IsEmpty)
            {
                _globalQueue.Enqueue(tempQueue.Dequeue());
            }

            return results;
        }

        /// <summary>
        /// Gets the total number of requests in the scheduler
        /// </summary>
        /// <returns>The total count of requests</returns>
        public int GetTotalRequestCount()
        {
            return _globalQueue.Count;
        }
    }
}

