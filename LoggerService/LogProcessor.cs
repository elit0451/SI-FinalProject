using System;
using System.IO;
using LoggerService.Model;

namespace LoggerService
{
    public static class LogProcessor
    {
        private static StreamWriter writer;

        static LogProcessor()
        {
            writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "log.txt", true);
        }


        public static void Process(Log log)
        {
            if (log.Level == LogLevel.CRITICAL)
                Console.WriteLine("[{0}] - {1} - {2}", log.Level, log.Service, log.Message);

                writer.WriteLine("[{0}] - {1} - {2}", log.Level, log.Service, log.Message);
        }
    }
}