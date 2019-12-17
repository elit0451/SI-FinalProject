using System;
using System.IO;
using LoggerService.Model;

namespace LoggerService
{
    public static class LogProcessor
    {
        public static void Process(Log log)
        {
            if (log.Level == LogLevel.CRITICAL)
                Console.WriteLine("[{0}] - {1} - {2}", log.Level, log.Service, log.Message);
                
            using(StreamWriter writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "log.txt", true))
                writer.WriteLine("[{0}] - {1} - {2}", log.Level, log.Service, log.Message);
        }
    }
}