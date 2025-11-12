using System;
using System.IO;

namespace MunicipalServiceApp.Utilities
{
    /// <summary>
    /// Simple logging utility for the application.
    /// Logs to both debug output and a log file.
    /// </summary>
    public static class ApplicationLogger
    {
        private static readonly string LogFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "application.log");

        /// <summary>
        /// Logs an information message
        /// </summary>
        public static void LogInfo(string message)
        {
            Log("INFO", message);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        public static void LogWarning(string message)
        {
            Log("WARN", message);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        public static void LogError(string message, Exception ex = null)
        {
            string errorMessage = message;
            if (ex != null)
            {
                errorMessage += $": {ex.Message}\nStack Trace: {ex.StackTrace}";
            }
            Log("ERROR", errorMessage);
        }

        /// <summary>
        /// Internal logging method
        /// </summary>
        private static void Log(string level, string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
                
                // Write to debug output
                System.Diagnostics.Debug.WriteLine(logEntry);
                
                // Write to log file
                var logDirectory = Path.GetDirectoryName(LogFilePath);
                if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
                
                File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
            }
            catch
            {
                // Silently fail if logging fails to avoid breaking the application
            }
        }
    }
}

