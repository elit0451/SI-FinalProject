using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoggerService
{
    public class CommandRouter
    {
        internal static void Route(string routingKey, string message)
        {
            JObject receivedObj = JsonConvert.DeserializeObject<JObject>(message);

            switch (routingKey)
            {
                case "info":
                   
                    break;

                case "warning":
                    
                    break;

                case "critical":
                   
                    break;
                default:
                    Console.WriteLine("No such route");
                    break;
            }
        }
    }
}