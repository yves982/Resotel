using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace ResotelApp.Utils
{
    static class Logger
    {
        private static string _fileName;
        private static string _errorFileName;

        static Logger()
        {
            string logDir = ConfigurationManager.AppSettings["LogDir"];
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            _fileName = $"{Path.Combine(logDir, $"resotel_{DateTime.Now:ddMMyyyy}.log")}";
            _errorFileName = $"{Path.Combine(logDir, $"resotel_{DateTime.Now:ddMMyyyy}.err.log")}";
        }

        public static void Log(string message)
        {
            using (FileStream fs = File.Open(_fileName, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.WriteLine($"##{DateTime.Now:HH:mm:ss}## {message}");
                }
            }
        }

        public static void Log(Exception ex)
        {
            using (FileStream fs = File.Open(_errorFileName, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.WriteLine($"##{DateTime.Now:HH:mm:ss}##");
                    writer.WriteLine(ex.StackTrace);
                }
            }
        }
    }
}
