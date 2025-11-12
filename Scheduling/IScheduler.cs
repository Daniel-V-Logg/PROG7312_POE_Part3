using System.Collections.Generic;
using MunicipalServiceApp.Models;

namespace MunicipalServiceApp.Scheduling
{
    /// <summary>
    /// Interface for scheduling service requests for teams.
    /// Provides methods to schedule and retrieve top priority requests.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Adds a service request to the scheduler
        /// </summary>
        /// <param name="request">The service request to add</param>
        void AddRequest(ServiceRequest request);

        /// <summary>
        /// Gets the top N highest priority requests for a specific team
        /// </summary>
        /// <param name="teamId">The ID of the team</param>
        /// <param name="count">The number of requests to retrieve</param>
        /// <returns>A list of the top N highest priority requests for the team</returns>
        List<ServiceRequest> GetTopRequestsForTeam(string teamId, int count);

        /// <summary>
        /// Gets the top N highest priority requests across all teams
        /// </summary>
        /// <param name="count">The number of requests to retrieve</param>
        /// <returns>A list of the top N highest priority requests</returns>
        List<ServiceRequest> GetTopRequests(int count);

        /// <summary>
        /// Gets the total number of requests in the scheduler
        /// </summary>
        /// <returns>The total count of requests</returns>
        int GetTotalRequestCount();
    }
}

