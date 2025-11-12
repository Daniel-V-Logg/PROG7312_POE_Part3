using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MunicipalServiceApp.Models;
using Newtonsoft.Json;

namespace MunicipalServiceApp.Repositories
{
    /// <summary>
    /// In-memory implementation of IServiceRequestRepository using a List for storage.
    /// Provides save/load functionality to JSON file.
    /// </summary>
    public class InMemoryServiceRequestRepository : IServiceRequestRepository
    {
        private readonly List<ServiceRequest> _requests;
        private readonly string _dataFilePath;

        /// <summary>
        /// Initializes a new instance of InMemoryServiceRequestRepository
        /// </summary>
        /// <param name="dataFilePath">Path to the JSON file for persistence (defaults to Data/serviceRequests.json)</param>
        public InMemoryServiceRequestRepository(string dataFilePath = null)
        {
            _requests = new List<ServiceRequest>();
            
            if (string.IsNullOrEmpty(dataFilePath))
            {
                // Default to Data folder in the application directory
                var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }
                _dataFilePath = Path.Combine(dataDirectory, "serviceRequests.json");
            }
            else
            {
                _dataFilePath = dataFilePath;
            }

            // Automatically load existing data on construction
            Load();
        }

        /// <summary>
        /// Gets all service requests
        /// </summary>
        public IEnumerable<ServiceRequest> GetAll()
        {
            return _requests.AsReadOnly();
        }

        /// <summary>
        /// Gets a service request by its ID
        /// </summary>
        public ServiceRequest GetById(string requestId)
        {
            return _requests.FirstOrDefault(r => r.RequestId == requestId);
        }

        /// <summary>
        /// Adds a new service request
        /// </summary>
        public void Add(ServiceRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrEmpty(request.RequestId))
            {
                request.RequestId = Guid.NewGuid().ToString();
            }

            // Check if ID already exists
            if (_requests.Any(r => r.RequestId == request.RequestId))
            {
                throw new InvalidOperationException($"Service request with ID {request.RequestId} already exists.");
            }

            _requests.Add(request);
        }

        /// <summary>
        /// Updates an existing service request
        /// </summary>
        public void Update(ServiceRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var existingRequest = _requests.FirstOrDefault(r => r.RequestId == request.RequestId);
            if (existingRequest == null)
            {
                throw new InvalidOperationException($"Service request with ID {request.RequestId} not found.");
            }

            // Update all properties
            var index = _requests.IndexOf(existingRequest);
            _requests[index] = request;
        }

        /// <summary>
        /// Deletes a service request by ID
        /// </summary>
        public bool Delete(string requestId)
        {
            if (string.IsNullOrEmpty(requestId))
            {
                return false;
            }

            var request = _requests.FirstOrDefault(r => r.RequestId == requestId);
            if (request == null)
            {
                return false;
            }

            return _requests.Remove(request);
        }

        /// <summary>
        /// Saves all requests to persistent storage (JSON file)
        /// </summary>
        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(_dataFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Ensure we have data to save
                if (_requests == null)
                {
                    throw new InvalidOperationException("Request list is null");
                }

                var json = JsonConvert.SerializeObject(_requests, Formatting.Indented);
                
                // Write to a temporary file first, then move it (atomic operation)
                string tempFile = _dataFilePath + ".tmp";
                File.WriteAllText(tempFile, json);
                
                // Replace the original file
                if (File.Exists(_dataFilePath))
                {
                    File.Delete(_dataFilePath);
                }
                File.Move(tempFile, _dataFilePath);
                
                // Verify the file was written and has content
                if (!File.Exists(_dataFilePath))
                {
                    throw new IOException($"File was not created at {_dataFilePath}");
                }
                
                string savedContent = File.ReadAllText(_dataFilePath);
                if (string.IsNullOrWhiteSpace(savedContent) || savedContent == "[]")
                {
                    throw new IOException($"File was created but is empty. Request count: {_requests.Count}");
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to save service requests to {_dataFilePath}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads all requests from persistent storage (JSON file)
        /// If file doesn't exist or is empty, generates sample data
        /// </summary>
        public void Load()
        {
            if (!File.Exists(_dataFilePath))
            {
                // File doesn't exist yet, generate sample data
                GenerateSampleDataIfNeeded();
                return;
            }

            try
            {
                var json = File.ReadAllText(_dataFilePath);
                if (string.IsNullOrWhiteSpace(json) || json.Trim() == "[]")
                {
                    // Empty file, generate sample data
                    GenerateSampleDataIfNeeded();
                    return;
                }

                var loadedRequests = JsonConvert.DeserializeObject<List<ServiceRequest>>(json);
                if (loadedRequests != null && loadedRequests.Count > 0)
                {
                    _requests.Clear();
                    _requests.AddRange(loadedRequests);
                }
                else
                {
                    // No valid data, generate sample data
                    GenerateSampleDataIfNeeded();
                }
            }
            catch (JsonException ex)
            {
                // If JSON is corrupted, generate sample data
                System.Diagnostics.Debug.WriteLine($"JSON parse error, generating sample data: {ex.Message}");
                GenerateSampleDataIfNeeded();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load error, generating sample data: {ex.Message}");
                GenerateSampleDataIfNeeded();
            }
        }

        /// <summary>
        /// Generates sample data if the repository is empty
        /// </summary>
        private void GenerateSampleDataIfNeeded()
        {
            if (_requests.Count == 0)
            {
                var sampleRequests = ServiceRequestDataGenerator.GenerateSampleRequests(10);
                _requests.AddRange(sampleRequests);
                Save(); // Save the generated sample data
            }
        }
    }
}

