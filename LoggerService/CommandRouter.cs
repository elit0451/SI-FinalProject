using System;
using LoggerService.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoggerService
{
    public class CommandRouter
    {
        internal static void Route(string routingKey, string message)
        {
            JObject receivedObj = JsonConvert.DeserializeObject<JObject>(message);
            string logMessage = receivedObj["Message"].Value<string>();
            string logService = receivedObj["Service"].Value<string>();
            Log log = new Log();
            log.Message = logMessage;
            log.Service = logService;

            switch (routingKey)
            {
                case "info":
                    log.Level = LogLevel.INFO;
                    break;

                case "warning":
                    log.Level = LogLevel.WARNING;
                    break;

                case "critical":
                    log.Level = LogLevel.CRITICAL;
                    break;
                    
                default:
                    Console.WriteLine("No such route");
                    break;
            }

            LogProcessor.Process(log);
        }
    }
}