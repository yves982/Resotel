using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace ResotelApp.Utils
{
    /// <summary>
    /// Write logs (error and normal) on demand. 
    /// It produces at most two log files per day. Log files are horodated and Log directory comes from config file.
    /// </summary>
    static class Logger
    {
        private static string _fileName;
        private static string _errorFileName;
        private static string _logDir = null;

        private static void _createsLogDirIfNeeded()
        {
            _logDir = ConfigurationManager.AppSettings["LogDir"];
            if (!Directory.Exists(_logDir))
            {
                Directory.CreateDirectory(_logDir);
            }
        }

        static Logger()
        {
            _createsLogDirIfNeeded();
            _fileName = $"{Path.Combine(_logDir, $"resotel_{DateTime.Now:ddMMyyyy}.log")}";
            _errorFileName = $"{Path.Combine(_logDir, $"resotel_{DateTime.Now:ddMMyyyy}.err.log")}";
            _createsLogDirIfNeeded();
        }

        /// <summary>
        /// Logs a message (horodated within log file)
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            _createsLogDirIfNeeded();
            using (FileStream fs = File.Open(_fileName, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.WriteLine($"##{DateTime.Now:HH:mm:ss}## {message}");
                }
            }
        }

        /// <summary>
        /// Logs an Exception's StackTrace
        /// </summary>
        /// <param name="ex"></param>
        public static void Log(Exception ex)
        {
            _createsLogDirIfNeeded();
            using (FileStream fs = File.Open(_errorFileName, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.WriteLine($"##{DateTime.Now:HH:mm:ss}##");
                    writer.WriteLine(ex.Message);
                    writer.WriteLine(ex.StackTrace);
                }
            }
        }
    }
}
