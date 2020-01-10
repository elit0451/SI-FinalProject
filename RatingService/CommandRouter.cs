using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RatingService.Database;
using RatingService.Models;

namespace RatingService
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
                    int feedback = receivedObj["Feedback"].Value<int>();
                    Rating rating = new Rating(AppDb.Instance)
                    {
                        EventId = eventId,
                        Feedback = feedback
                    };

                    await AppDb.Instance.Connection.OpenAsync();
                    await rating.InsertAsync();
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