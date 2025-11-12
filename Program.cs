using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MunicipalServiceApp.Tests;

namespace MunicipalServiceApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Check if running in test mode
            if (args.Length > 0 && (args[0].ToLower() == "--test" || args[0].ToLower() == "/test" || args[0].ToLower() == "-test"))
            {
                RunAllTests();
                Console.WriteLine("\nPress any key to exit...");
                try
                {
                    Console.ReadKey();
                }
                catch (InvalidOperationException)
                {
                    // Console input not available (e.g., when running from PowerShell with redirected input)
                    // Just exit without waiting for key press
                }
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        /// <summary>
        /// Runs all test suites
        /// </summary>
        static void RunAllTests()
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("RUNNING ALL TESTS");
            Console.WriteLine("=========================================\n");

            try
            {
                DataStructureTests.RunAllTests();
                Console.WriteLine();
                
                ServiceRequestTests.RunAllTests();
                Console.WriteLine();
                
                TreeStructureTests.RunAllTests();
                Console.WriteLine();
                
                HeapAndSchedulerTests.RunAllTests();
                Console.WriteLine();
                
                GraphTests.RunAllTests();
                Console.WriteLine();
                
                // New unit and integration tests
                DataStructureUnitTests.RunAllUnitTests();
                Console.WriteLine();
                
                RepositoryIntegrationTests.RunAllIntegrationTests();
                Console.WriteLine();
                
                Console.WriteLine("=========================================");
                Console.WriteLine("ALL TEST SUITES COMPLETE");
                Console.WriteLine("=========================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR RUNNING TESTS: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}
