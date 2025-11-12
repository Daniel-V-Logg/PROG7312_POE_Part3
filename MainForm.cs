using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MunicipalServiceApp.Repositories;
using MunicipalServiceApp.Utilities;

namespace MunicipalServiceApp
{
    public partial class MainForm : Form
    {
        private static IServiceRequestRepository _sharedRepository;

        public MainForm()
        {
            InitializeComponent();
            btnServiceStatus.Enabled = true;
            btnServiceStatus.Text = "Service Request Status";
            
            // Initialize shared repository and load data at application start
            InitializeApplicationData();
        }

        /// <summary>
        /// Initializes application data structures at startup.
        /// Loads sample data if needed and populates data structures.
        /// </summary>
        private void InitializeApplicationData()
        {
            try
            {
                ApplicationLogger.LogInfo("Initializing application data structures...");
                
                // Create shared repository instance
                _sharedRepository = new InMemoryServiceRequestRepository();
                
                // Load data (will generate sample data if file doesn't exist)
                _sharedRepository.Load();
                
                var requestCount = _sharedRepository.GetAll().Count();
                ApplicationLogger.LogInfo($"Loaded {requestCount} service requests from repository.");
                
                // Data structures will be populated when ServiceRequestStatusForm is opened
                ApplicationLogger.LogInfo("Application data initialization complete.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("Failed to initialize application data", ex);
                MessageBox.Show($"Error initializing application: {ex.Message}", 
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Gets the shared repository instance
        /// </summary>
        public static IServiceRequestRepository GetSharedRepository()
        {
            if (_sharedRepository == null)
            {
                _sharedRepository = new InMemoryServiceRequestRepository();
                _sharedRepository.Load();
            }
            return _sharedRepository;
        }


        private void btnReportIssues_Click(object sender, EventArgs e)
        {
            var reportForm = new ReportIssuesForm();
            reportForm.ShowDialog();
        }

        private void btnLocalEvents_Click(object sender, EventArgs e)
        {
            var eventsForm = new LocalEventsForm();
            eventsForm.ShowDialog();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            btnLocalEvents.Enabled = true;
            btnServiceStatus.Enabled = true;
            btnServiceStatus.Text = "Service Request Status";
        }

        private void btnServiceStatus_Click(object sender, EventArgs e)
        {
            var serviceStatusForm = new ServiceRequestStatusForm();
            serviceStatusForm.ShowDialog();
        }
    }
}
