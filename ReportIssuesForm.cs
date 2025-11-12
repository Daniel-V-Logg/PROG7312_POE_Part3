using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MunicipalServiceApp.Models;
using MunicipalServiceApp.Repositories;
using MunicipalServiceApp.Utilities;

namespace MunicipalServiceApp
{
    public partial class ReportIssuesForm : Form
    {
        private string attachmentPath = string.Empty; // field to hold attached file path
        private readonly IServiceRequestRepository _repository;

        public ReportIssuesForm()
        {
            InitializeComponent();
            // Use shared repository from MainForm
            _repository = MainForm.GetSharedRepository();
            engagementProgressBar.Value = 30; // initial encouragement level
            lblEncouragement.Text = "You're almost there. A few details go a long way.";

            txtLocation.TextChanged += UpdateEngagement;
            cmbCategory.SelectedIndexChanged += UpdateEngagement;
            rtbDescription.TextChanged += UpdateEngagement;
        }

        private void UpdateEngagement(object sender, EventArgs e)
        {
            int score = 0;
            if (!string.IsNullOrWhiteSpace(txtLocation.Text)) score += 30;
            if (!string.IsNullOrWhiteSpace(cmbCategory.Text)) score += 30;
            if (!string.IsNullOrWhiteSpace(rtbDescription.Text)) score += 30;
            if (!string.IsNullOrEmpty(attachmentPath)) score += 10;

            engagementProgressBar.Value = Math.Min(100, Math.Max(10, score));

            if (score < 30)
                lblEncouragement.Text = "Great start. Add a location to help us pinpoint it.";
            else if (score < 60)
                lblEncouragement.Text = "Nice! Pick a category so it reaches the right team.";
            else if (score < 90)
                lblEncouragement.Text = "Looking good. A short description makes it actionable.";
            else
                lblEncouragement.Text = "Perfect! You’re ready to submit.";
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Images or Documents|*.jpg;*.jpeg;*.png;*.pdf;*.docx;*.xlsx";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string destFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Attachments");
                    if (!Directory.Exists(destFolder))
                        Directory.CreateDirectory(destFolder);

                    string fileName = Path.GetFileName(ofd.FileName);
                    string destPath = Path.Combine(destFolder, fileName);
                    File.Copy(ofd.FileName, destPath, true);

                    attachmentPath = destPath;
                    lblAttachment.Text = $"Attached: {fileName}";
                    engagementProgressBar.Value = 70; // boost engagement when attaching
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(txtLocation.Text))
                {
                    MessageBox.Show("Please enter the location of the issue.", "Location Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLocation.Focus();
                    return;
                }

                if (cmbCategory.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a category for the issue.", "Category Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbCategory.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(rtbDescription.Text))
                {
                    MessageBox.Show("Please provide a description of the issue.", "Description Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rtbDescription.Focus();
                    return;
                }

                if (rtbDescription.Text.Length < 10)
                {
                    MessageBox.Show("Please provide a more detailed description (at least 10 characters).", "Description Too Short", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rtbDescription.Focus();
                    return;
                }

                // Create and save issue (legacy Issue for DataStore)
                var issue = new Issue(
                    txtLocation.Text.Trim(),
                    cmbCategory.Text,
                    rtbDescription.Text.Trim(),
                    attachmentPath
                );

                DataStore.AddIssue(issue);

                // Also create and save as ServiceRequest for the repository
                // Parse location to extract coordinates if in format "lat,lon" or use defaults
                double latitude = -26.2041; // Default Johannesburg coordinates
                double longitude = 28.0473;
                
                string locationText = txtLocation.Text.Trim();
                if (locationText.Contains(","))
                {
                    var parts = locationText.Split(',');
                    if (parts.Length >= 2)
                    {
                        if (double.TryParse(parts[0].Trim(), out double lat) && 
                            double.TryParse(parts[1].Trim(), out double lon))
                        {
                            latitude = lat;
                            longitude = lon;
                        }
                    }
                }

                // Determine priority based on category
                int priority = 3; // Default medium priority
                string category = cmbCategory.Text.ToLower();
                if (category.Contains("emergency") || category.Contains("urgent") || category.Contains("safety"))
                    priority = 1; // High priority
                else if (category.Contains("maintenance") || category.Contains("repair"))
                    priority = 2;
                else if (category.Contains("request") || category.Contains("suggestion"))
                    priority = 5; // Low priority

                // Create ServiceRequest
                var serviceRequest = new ServiceRequest(
                    title: $"{cmbCategory.Text} - {txtLocation.Text.Trim()}",
                    description: rtbDescription.Text.Trim(),
                    priority: priority,
                    latitude: latitude,
                    longitude: longitude,
                    assignedTeamId: null // Unassigned initially
                );

                ApplicationLogger.LogInfo($"Submitting new service request: {serviceRequest.Title}");
                
                // Add to repository
                _repository.Add(serviceRequest);
                
                // Verify it was added to in-memory list
                int countAfterAdd = _repository.GetAll().Count();
                var verifyInMemory = _repository.GetById(serviceRequest.RequestId);
                if (verifyInMemory == null || countAfterAdd == 0)
                {
                    ApplicationLogger.LogError($"Request was not added to repository. Count: {countAfterAdd}");
                    throw new Exception($"Request was not added to repository. Count: {countAfterAdd}");
                }
                
                ApplicationLogger.LogInfo($"Request added to repository. Total requests: {countAfterAdd}");
                
                // Save to file
                _repository.Save();
                ApplicationLogger.LogInfo($"Request saved to persistent storage. Request ID: {serviceRequest.RequestId}");

                // Success feedback
                MessageBox.Show($"Issue reported successfully!\n\nRequest ID: {serviceRequest.RequestId}\n" +
                    $"Title: {serviceRequest.Title}\n" +
                    $"Priority: {serviceRequest.Priority}\n" +
                    $"Reference: {issue.ReportedAt:yyyy-MM-dd HH:mm}\n" +
                    $"\nThank you for helping improve our community.\n\n" +
                    $"Note: The request has been saved and will appear in the Service Request Status form.", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset form
                ResetForm();
            }
            catch (Exception ex)
            {
                string errorDetails = $"Error: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorDetails += $"\n\nInner Exception: {ex.InnerException.Message}";
                }
                errorDetails += $"\n\nStack Trace: {ex.StackTrace}";
                
                MessageBox.Show("An error occurred while submitting your report. Please try again.\n\n" + errorDetails, 
                    "Submission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetForm()
        {
            txtLocation.Clear();
            cmbCategory.SelectedIndex = -1;
            rtbDescription.Clear();
            lblAttachment.Text = "No file attached";
            attachmentPath = string.Empty;
            engagementProgressBar.Value = 30;
            lblEncouragement.Text = "You're almost there. A few details go a long way.";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
