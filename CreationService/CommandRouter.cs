using System;
using System.Threading.Tasks;
using CreationService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CreationService
{
    internal static class CommandRouter
    {
        internal static async Task RouteAsync(string message)
        {
            JObject receivedObj = JsonConvert.DeserializeObject<JObject>(message);
            string command = receivedObj["command"].Value<string>();
            int eventId = receivedObj["EventId"].Value<int>();
            string correlationId = receivedObj["correlationId"].Value<string>();
            string replyTo = receivedObj["replyTo"].Value<string>();

            switch (command)
            {
                case "add":
                    string location = receivedObj["Location"].Value<string>();
                    string driveFrom = receivedObj["DriveFrom"].Value<string>();
                    DateTime dateFrom = receivedObj["DateFrom"].Value<DateTime>();
                    DateTime dateTo = receivedObj["DateTo"].Value<DateTime>();

                    Event rating = new Event()
                    {
                        EventId = eventId,
                        Location = location,
                        DriveFrom = driveFrom,
                        DateFrom  = dateFrom,
                        DateTo = dateTo
                    };

                    MessageGateway.PublishRPC()
                    break;
                case "get":
                    await AppDb.Instance.Connection.OpenAsync();
                    RatingQuery query = new RatingQuery(AppDb.Instance);
                    Rating foundRating = await query.FindOneAsync(eventId);

                    if(!(foundRating is null))
                        // Convert to JSON without a command
                        MessageGateway.PublishRPC(replyTo, correlationId, foundRating.ConvertToJson(""));
                    break;
                default:
                    Console.WriteLine("No such command");
                    break;
            }
        }
    }
}