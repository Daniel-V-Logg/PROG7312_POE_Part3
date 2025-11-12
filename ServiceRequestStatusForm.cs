using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using MunicipalServiceApp.Models;
using MunicipalServiceApp.Repositories;
using MunicipalServiceApp.DataStructures;
using MunicipalServiceApp.Scheduling;

namespace MunicipalServiceApp
{
    /// <summary>
    /// Form for viewing and managing service requests.
    /// Displays service requests in a list with filtering capabilities.
    /// </summary>
    public partial class ServiceRequestStatusForm : Form
    {
        private readonly IServiceRequestRepository _repository;
        private BinarySearchTree<ServiceRequest, string> _requestIdBST;
        private List<ServiceRequest> _currentDisplayedRequests;
        private List<Edge> _mstEdges;
        private List<ServiceRequest> _mstRequests;

        public ServiceRequestStatusForm()
        {
            InitializeComponent();
            _repository = new InMemoryServiceRequestRepository();
            _requestIdBST = new BinarySearchTree<ServiceRequest, string>(req => req.RequestId);
            _currentDisplayedRequests = new List<ServiceRequest>();
            _mstEdges = new List<Edge>();
            _mstRequests = new List<ServiceRequest>();
            InitializeForm();
        }

        /// <summary>
        /// Sets up the form when it loads - populates status filter and loads requests.
        /// </summary>
        private void InitializeForm()
        {
            // Populate status filter dropdown
            PopulateStatusFilter();

            // Load all service requests into the list
            LoadAllRequests();

            // Set default priority filter to show all
            cmbPriorityFilter.SelectedIndex = 0;
        }

        /// <summary>
        /// Fills the status combobox with status options.
        /// </summary>
        private void PopulateStatusFilter()
        {
            cmbStatusFilter.Items.Clear();
            cmbStatusFilter.Items.Add("All Statuses");
            cmbStatusFilter.Items.Add("Submitted");
            cmbStatusFilter.Items.Add("In Progress");
            cmbStatusFilter.Items.Add("Completed");
            cmbStatusFilter.SelectedIndex = 0;
        }

        /// <summary>
        /// Loads all service requests into the ListView and builds BST index.
        /// </summary>
        private void LoadAllRequests()
        {
            var allRequests = _repository.GetAll().ToList();
            _currentDisplayedRequests = allRequests;
            
            // Build BST index for fast RequestId lookup
            _requestIdBST = new BinarySearchTree<ServiceRequest, string>(req => req.RequestId);
            foreach (var request in allRequests)
            {
                _requestIdBST.Insert(request);
            }
            
            DisplayRequests(allRequests);
            UpdateStatusLabel($"Showing {allRequests.Count} service request(s)");
        }

        /// <summary>
        /// Displays a list of service requests in the ListView.
        /// </summary>
        private void DisplayRequests(List<ServiceRequest> requests)
        {
            lvRequests.Items.Clear();

            // Sort by priority (1 = high, 5 = low) and then by date submitted (newest first)
            var sortedRequests = requests
                .OrderBy(r => r.Priority)
                .ThenByDescending(r => r.DateSubmitted)
                .ToList();

            foreach (var request in sortedRequests)
            {
                var item = new ListViewItem(request.Title);
                item.SubItems.Add(request.RequestId);
                item.SubItems.Add(request.Status);
                item.SubItems.Add(request.Priority.ToString());
                item.SubItems.Add(request.DateSubmitted.ToString("dd/MM/yyyy HH:mm"));
                item.SubItems.Add(request.AssignedTeamId ?? "Unassigned");
                item.SubItems.Add($"({request.Latitude:F4}, {request.Longitude:F4})");
                item.SubItems.Add(request.Description);
                item.Tag = request; // Store the request object for later use

                // Color code by priority (1 = high priority = red tint, 5 = low priority = green tint)
                if (request.Priority == 1)
                    item.BackColor = System.Drawing.Color.LightPink;
                else if (request.Priority == 2)
                    item.BackColor = System.Drawing.Color.LightYellow;
                else if (request.Priority >= 4)
                    item.BackColor = System.Drawing.Color.LightGreen;

                lvRequests.Items.Add(item);
            }
        }

        /// <summary>
        /// Updates the status label with current operation info.
        /// </summary>
        private void UpdateStatusLabel(string message)
        {
            lblStatus.Text = message;
        }

        // --- Button Click Handlers ---

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var allRequests = _repository.GetAll().ToList();
                var filteredRequests = allRequests.AsEnumerable();

                // Search by RequestId using BST if the search text looks like a RequestId (GUID format)
                string keyword = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    // Check if it looks like a RequestId (contains hyphens, typical GUID format)
                    if (keyword.Contains("-") && keyword.Length > 20)
                    {
                        // Try BST search by RequestId
                        ServiceRequest foundRequest = _requestIdBST.Search(keyword);
                        if (foundRequest != null)
                        {
                            filteredRequests = new List<ServiceRequest> { foundRequest }.AsEnumerable();
                            UpdateStatusLabel($"Found request by RequestId: {foundRequest.Title}");
                        }
                        else
                        {
                            // Fall back to keyword search
                            filteredRequests = allRequests.Where(r =>
                                r.Title.ToLower().Contains(keyword.ToLower()) ||
                                r.Description.ToLower().Contains(keyword.ToLower()) ||
                                r.RequestId.ToLower().Contains(keyword.ToLower())
                            );
                        }
                    }
                    else
                    {
                        // Regular keyword search
                        filteredRequests = filteredRequests.Where(r =>
                            r.Title.ToLower().Contains(keyword.ToLower()) ||
                            r.Description.ToLower().Contains(keyword.ToLower()) ||
                            r.RequestId.ToLower().Contains(keyword.ToLower())
                        );
                    }
                }

                // Filter by status
                string statusFilter = cmbStatusFilter.SelectedItem?.ToString();
                if (statusFilter != null && statusFilter != "All Statuses")
                {
                    filteredRequests = filteredRequests.Where(r => r.Status == statusFilter);
                }

                // Filter by priority
                string priorityFilter = cmbPriorityFilter.SelectedItem?.ToString();
                if (priorityFilter != null && priorityFilter != "All Priorities")
                {
                    // Extract priority number from "Priority 1 (High)" or "Priority 2"
                    var parts = priorityFilter.Split(' ');
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int priority))
                    {
                        filteredRequests = filteredRequests.Where(r => r.Priority == priority);
                    }
                }

                var results = filteredRequests.ToList();
                _currentDisplayedRequests = results;
                DisplayRequests(results);
                UpdateStatusLabel($"Found {results.Count} service request(s) matching your criteria");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while searching: {ex.Message}", "Search Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbStatusFilter.SelectedIndex = 0;
            cmbPriorityFilter.SelectedIndex = 0;
            LoadAllRequests();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _repository.Load(); // Reload from JSON file
            LoadAllRequests();
        }

        private void lvRequests_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvRequests.SelectedItems.Count > 0)
            {
                var selectedRequest = lvRequests.SelectedItems[0].Tag as ServiceRequest;
                if (selectedRequest != null)
                {
                    // Show details in the details textbox
                    txtDetails.Text = $"Request ID: {selectedRequest.RequestId}\r\n" +
                                     $"Title: {selectedRequest.Title}\r\n" +
                                     $"Status: {selectedRequest.Status}\r\n" +
                                     $"Priority: {selectedRequest.Priority} (1=High, 5=Low)\r\n" +
                                     $"Date Submitted: {selectedRequest.DateSubmitted:yyyy-MM-dd HH:mm:ss}\r\n" +
                                     $"Assigned Team: {selectedRequest.AssignedTeamId ?? "Unassigned"}\r\n" +
                                     $"Location: Latitude {selectedRequest.Latitude:F6}, Longitude {selectedRequest.Longitude:F6}\r\n" +
                                     $"\r\nDescription:\r\n{selectedRequest.Description}";
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            _repository.Save(); // Save any changes before closing
            this.Close();
        }

        /// <summary>
        /// Handles the Schedule Team button click - uses heap scheduler to show top requests.
        /// </summary>
        private async void btnScheduleTeam_Click(object sender, EventArgs e)
        {
            try
            {
                btnScheduleTeam.Enabled = false;
                UpdateStatusLabel("Scheduling requests...");

                await Task.Run(() =>
                {
                    // Use the heap scheduler to get top requests
                    RequestScheduler scheduler = new RequestScheduler();
                    
                    // Add all current displayed requests to scheduler
                    foreach (var request in _currentDisplayedRequests)
                    {
                        scheduler.AddRequest(request);
                    }

                    // Get top 5 requests
                    var topRequests = scheduler.GetTopRequests(5);

                    // Update UI on main thread
                    this.Invoke((MethodInvoker)delegate
                    {
                        DisplayRequests(topRequests);
                        UpdateStatusLabel($"Scheduled top {topRequests.Count} priority requests");
                        btnScheduleTeam.Enabled = true;
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while scheduling: {ex.Message}", "Scheduling Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnScheduleTeam.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Show Routing (MST) button click - builds graph and visualizes MST.
        /// </summary>
        private async void btnShowRouting_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentDisplayedRequests.Count < 2)
                {
                    MessageBox.Show("Need at least 2 requests to show routing.", "Routing Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                btnShowRouting.Enabled = false;
                UpdateStatusLabel("Computing MST routing...");

                await Task.Run(() =>
                {
                    // Build graph from current displayed requests
                    Graph graph = GraphUtilities.BuildGraphFromRequests(_currentDisplayedRequests, maxDistanceKm: 50.0);
                    
                    // Compute MST using Kruskal's algorithm
                    List<Edge> mst = graph.MinimumSpanningTreeKruskal();

                    // Update UI on main thread
                    this.Invoke((MethodInvoker)delegate
                    {
                        _mstEdges = mst;
                        _mstRequests = _currentDisplayedRequests;
                        
                        // Show MST visualization panel
                        grpActions.Visible = false;
                        grpMSTVisualization.Visible = true;
                        pnlMST.Invalidate(); // Trigger paint event
                        
                        double totalDistance = GraphUtilities.CalculateTotalWeight(mst);
                        UpdateStatusLabel($"MST routing computed. Total distance: {totalDistance:F2} km");
                        btnShowRouting.Enabled = true;
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while computing routing: {ex.Message}", "Routing Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnShowRouting.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the MST panel paint event - draws nodes and edges.
        /// </summary>
        private void pnlMST_Paint(object sender, PaintEventArgs e)
        {
            if (_mstRequests == null || _mstRequests.Count == 0 || _mstEdges == null)
            {
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Calculate bounds for normalization
            double minLat = _mstRequests.Min(r => r.Latitude);
            double maxLat = _mstRequests.Max(r => r.Latitude);
            double minLon = _mstRequests.Min(r => r.Longitude);
            double maxLon = _mstRequests.Max(r => r.Longitude);

            double latRange = maxLat - minLat;
            double lonRange = maxLon - minLon;

            // Add padding
            int padding = 30;
            int panelWidth = pnlMST.Width - padding * 2;
            int panelHeight = pnlMST.Height - padding * 2;

            // Normalize coordinates to panel size
            Dictionary<int, PointF> nodePositions = new Dictionary<int, PointF>();
            for (int i = 0; i < _mstRequests.Count; i++)
            {
                double normalizedLat = latRange > 0 ? (_mstRequests[i].Latitude - minLat) / latRange : 0.5;
                double normalizedLon = lonRange > 0 ? (_mstRequests[i].Longitude - minLon) / lonRange : 0.5;

                float x = padding + (float)(normalizedLon * panelWidth);
                float y = padding + (float)((1 - normalizedLat) * panelHeight); // Flip Y axis

                nodePositions[i] = new PointF(x, y);
            }

            // Draw edges (lines)
            using (Pen edgePen = new Pen(Color.Blue, 2))
            {
                foreach (var edge in _mstEdges)
                {
                    if (nodePositions.ContainsKey(edge.From) && nodePositions.ContainsKey(edge.To))
                    {
                        PointF from = nodePositions[edge.From];
                        PointF to = nodePositions[edge.To];
                        g.DrawLine(edgePen, from, to);
                    }
                }
            }

            // Draw nodes (circles)
            using (Brush nodeBrush = new SolidBrush(Color.Red))
            using (Pen nodePen = new Pen(Color.DarkRed, 2))
            {
                foreach (var kvp in nodePositions)
                {
                    float radius = 8;
                    RectangleF nodeRect = new RectangleF(
                        kvp.Value.X - radius,
                        kvp.Value.Y - radius,
                        radius * 2,
                        radius * 2
                    );
                    g.FillEllipse(nodeBrush, nodeRect);
                    g.DrawEllipse(nodePen, nodeRect);
                }
            }

            // Draw labels
            using (Brush textBrush = new SolidBrush(Color.Black))
            using (Font labelFont = new Font("Arial", 7))
            {
                for (int i = 0; i < _mstRequests.Count; i++)
                {
                    if (nodePositions.ContainsKey(i))
                    {
                        PointF pos = nodePositions[i];
                        string label = $"R{i + 1}";
                        g.DrawString(label, labelFont, textBrush, pos.X + 10, pos.Y - 5);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Close MST button click - hides MST visualization and shows actions panel.
        /// </summary>
        private void btnCloseMST_Click(object sender, EventArgs e)
        {
            grpMSTVisualization.Visible = false;
            grpActions.Visible = true;
        }
    }
}

