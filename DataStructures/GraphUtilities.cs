using System;
using System.Collections.Generic;
using MunicipalServiceApp.Models;

namespace MunicipalServiceApp.DataStructures
{
    /// <summary>
    /// Utility functions for working with graphs and ServiceRequest objects.
    /// Provides methods for building graphs from service requests and calculating distances.
    /// </summary>
    public static class GraphUtilities
    {
        /// <summary>
        /// Calculates the Euclidean distance between two geographic coordinates (in kilometers).
        /// Uses the Haversine formula for accurate distance calculation on Earth's surface.
        /// </summary>
        /// <param name="lat1">Latitude of the first point</param>
        /// <param name="lon1">Longitude of the first point</param>
        /// <param name="lat2">Latitude of the second point</param>
        /// <param name="lon2">Longitude of the second point</param>
        /// <returns>The distance in kilometers</returns>
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0;

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                      Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        /// <summary>
        /// Builds a graph from a list of ServiceRequest objects.
        /// Creates edges between requests based on distance threshold.
        /// </summary>
        /// <param name="requests">The list of service requests</param>
        /// <param name="maxDistanceKm">Maximum distance in kilometers to create an edge (default: 10km)</param>
        /// <returns>A graph where vertices represent service requests and edges represent connections</returns>
        public static Graph BuildGraphFromRequests(List<ServiceRequest> requests, double maxDistanceKm = 10.0)
        {
            if (requests == null || requests.Count == 0)
            {
                throw new ArgumentException("Request list cannot be null or empty", nameof(requests));
            }

            Graph graph = new Graph(requests.Count, directed: false);

            for (int i = 0; i < requests.Count; i++)
            {
                for (int j = i + 1; j < requests.Count; j++)
                {
                    double distance = CalculateDistance(
                        requests[i].Latitude, requests[i].Longitude,
                        requests[j].Latitude, requests[j].Longitude
                    );

                    if (distance <= maxDistanceKm)
                    {
                        graph.AddEdge(i, j, distance);
                    }
                }
            }

            return graph;
        }

        /// <summary>
        /// Builds a graph from ServiceRequest objects where edges represent shared team assignments.
        /// </summary>
        /// <param name="requests">The list of service requests</param>
        /// <returns>A graph where edges connect requests assigned to the same team</returns>
        public static Graph BuildGraphByTeam(List<ServiceRequest> requests)
        {
            if (requests == null || requests.Count == 0)
            {
                throw new ArgumentException("Request list cannot be null or empty", nameof(requests));
            }

            Graph graph = new Graph(requests.Count, directed: false);

            for (int i = 0; i < requests.Count; i++)
            {
                for (int j = i + 1; j < requests.Count; j++)
                {
                    if (!string.IsNullOrEmpty(requests[i].AssignedTeamId) &&
                        requests[i].AssignedTeamId == requests[j].AssignedTeamId)
                    {
                        double distance = CalculateDistance(
                            requests[i].Latitude, requests[i].Longitude,
                            requests[j].Latitude, requests[j].Longitude
                        );
                        graph.AddEdge(i, j, distance);
                    }
                }
            }

            return graph;
        }

        /// <summary>
        /// Calculates the total weight (distance) of edges in a list
        /// </summary>
        /// <param name="edges">The list of edges</param>
        /// <returns>The total weight of all edges</returns>
        public static double CalculateTotalWeight(List<Edge> edges)
        {
            double total = 0.0;
            foreach (var edge in edges)
            {
                total += edge.Weight;
            }
            return total;
        }
    }
}

