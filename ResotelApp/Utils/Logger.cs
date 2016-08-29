using System;
using System.Configuration;
using System.IO;

namespace ResotelApp.Utils
{
    static class Logger
    {
        private static string _fileName;

        static Logger()
        {
            string logDir = ConfigurationManager.AppSettings["LogDir"];
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            _fileName = $"{Path.Combine(logDir, $"resotle_{DateTime.Now}:ddMMyyyy.log")}";
        }

        public static void Log(string message)
        {
            using (FileStream fs = File.OpenWrite(_fileName))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(message);
                }
            }
        }

        public static void Log(Exception ex)
        {
            using (FileStream fs = File.OpenWrite(_fileName))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(ex.StackTrace);
                }
            }
        }
    }
}
