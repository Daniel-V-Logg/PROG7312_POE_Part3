using System;
using System.Collections.Generic;
using MunicipalServiceApp.Models;

namespace MunicipalServiceApp.Repositories
{
    /// <summary>
    /// Generates sample service request data for testing and demonstration purposes.
    /// </summary>
    public static class ServiceRequestDataGenerator
    {
        /// <summary>
        /// Generates a list of sample service requests with varied priorities, statuses, and locations.
        /// </summary>
        /// <param name="count">Number of sample requests to generate (default: 10)</param>
        /// <returns>List of sample ServiceRequest objects</returns>
        public static List<ServiceRequest> GenerateSampleRequests(int count = 10)
        {
            var requests = new List<ServiceRequest>();
            var random = new Random();
            
            // Sample locations in South Africa (Johannesburg area)
            var locations = new[]
            {
                new { Lat = -26.2041, Lon = 28.0473, Name = "Sandton" },
                new { Lat = -26.1076, Lon = 28.0567, Name = "Rosebank" },
                new { Lat = -26.2389, Lon = 28.0473, Name = "Johannesburg CBD" },
                new { Lat = -26.1467, Lon = 28.0311, Name = "Parktown" },
                new { Lat = -26.1715, Lon = 28.0444, Name = "Braamfontein" },
                new { Lat = -26.2041, Lon = 28.0473, Name = "Melrose" },
                new { Lat = -26.1076, Lon = 28.0567, Name = "Houghton" },
                new { Lat = -26.2389, Lon = 28.0473, Name = "Newtown" }
            };

            var titles = new[]
            {
                "Pothole Repair on Main Street",
                "Broken Street Light on Elm Drive",
                "Garbage Collection Missed",
                "Water Leak at Community Center",
                "Overgrown Tree Branches",
                "Damaged Sidewalk Tiles",
                "Noise Complaint - Construction After Hours",
                "Parks Playground Equipment Needs Repair",
                "Traffic Sign Vandalized",
                "Request for New Benches in Park",
                "Sewer Blockage on Oak Avenue",
                "Street Cleaning Required",
                "Public Toilet Maintenance",
                "Road Markings Faded",
                "Drainage Issue in Residential Area"
            };

            var descriptions = new[]
            {
                "Large pothole causing vehicle damage. Needs urgent attention.",
                "Street light has been out for 3 days. Safety concern.",
                "Garbage was not collected on scheduled day. Bin is overflowing.",
                "Water leak detected near the main entrance. Wasting water.",
                "Tree branches hanging over power lines. Potential hazard.",
                "Several sidewalk tiles are cracked and pose tripping risk.",
                "Construction work continuing past 10 PM. Disturbing residents.",
                "Swing set is broken and slide has sharp edges. Children at risk.",
                "Stop sign has been vandalized and is not visible.",
                "Park needs more seating areas for elderly visitors.",
                "Sewer backup causing unpleasant odors in the area.",
                "Street needs cleaning after weekend market.",
                "Public restroom facilities require maintenance and cleaning.",
                "Road markings are barely visible, especially at night.",
                "Drainage system not working properly after heavy rain."
            };

            var teams = new[]
            {
                "Road-Maintenance-01",
                "Electrical-Team-02",
                "Sanitation-Team-01",
                "Plumbing-Team-01",
                "Parks-Maintenance-01",
                "Infrastructure-Team-01",
                "Traffic-Control-01",
                null // Some unassigned
            };

            var statuses = new[] { "Submitted", "In Progress", "Completed" };

            for (int i = 0; i < count; i++)
            {
                var location = locations[random.Next(locations.Length)];
                int priority = random.Next(1, 6); // 1-5
                string status = statuses[random.Next(statuses.Length)];
                string team = teams[random.Next(teams.Length)];
                
                // Adjust priority based on status (completed items tend to be lower priority)
                if (status == "Completed")
                {
                    priority = random.Next(3, 6); // Lower priority for completed
                }
                else if (status == "In Progress")
                {
                    priority = random.Next(1, 4); // Medium-high priority
                }

                var request = new ServiceRequest(
                    title: titles[random.Next(titles.Length)],
                    description: descriptions[random.Next(descriptions.Length)],
                    priority: priority,
                    latitude: location.Lat + (random.NextDouble() - 0.5) * 0.01, // Add small random offset
                    longitude: location.Lon + (random.NextDouble() - 0.5) * 0.01,
                    assignedTeamId: team
                )
                {
                    Status = status,
                    DateSubmitted = DateTime.Now.AddDays(-random.Next(0, 30)) // Random date in last 30 days
                };

                requests.Add(request);
            }

            return requests;
        }
    }
}

