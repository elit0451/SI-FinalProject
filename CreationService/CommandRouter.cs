using System;
using CreationService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CreationService
{
    internal static class CommandRouter
    {
        internal static void Route(string message)
        {
            JObject receivedObj = JsonConvert.DeserializeObject<JObject>(message);
            string command = receivedObj["Command"].Value<string>();

            switch (command)
            {
                case "add":
                    int eventId = receivedObj["EventId"].Value<int>();
                    string location = receivedObj["Location"].Value<string>();
                    string driveFrom = receivedObj["DriveFrom"].Value<string>();
                    DateTime dateFrom = receivedObj["DateFrom"].Value<DateTime>();
                    DateTime dateTo = receivedObj["DateTo"].Value<DateTime>();

                    Event eventObj = new Event()
                    {
                        EventId = eventId,
                        Location = location,
                        DriveFrom = driveFrom,
                        DateFrom = dateFrom,
                        DateTo = dateTo
                    };

                    EventRequest r = new EventRequest(Guid.NewGuid(), eventObj);
                    EventRequestRepository.Instance.AddRequest(r);

                    JObject requestCar = new JObject();
                    requestCar["From"] = dateFrom;
                    requestCar["To"] = dateTo;
                    requestCar["StationId"] = driveFrom;
                    requestCar["Command"] = "getAvailableCars";
                    requestCar["RequestId"] = r.RequestId;

                    JObject requestDriver = new JObject();
                    requestDriver["From"] = dateFrom;
                    requestDriver["To"] = dateTo;
                    requestDriver["RequestId"] = r.RequestId;


                    MessageGateway.PublishMessage("connector", "requests", requestCar.ToString());
                    MessageGateway.PublishMessage("driver", "find", requestDriver.ToString());
                    break;

                case "carFound":
                    {
                        string requestId = receivedObj["RequestId"].Value<string>();
                        JArray cars = receivedObj["Cars"].Value<JArray>();
                        string license = cars[0]["License"].Value<string>();


                        EventRequest eventRequest = EventRequestRepository.Instance.GetRequest(Guid.Parse(requestId));

                        JObject response = new JObject();
                        response["EventId"] = eventRequest.Event.EventId;
                        response["Command"] = "car";
                        response["License"] = license;

                        MessageGateway.PublishMessage("event", "update", response.ToString());
                    }
                    break;

                case "driverFound":
                    {
                        string requestId = receivedObj["RequestId"].Value<string>();
                        JArray drivers = receivedObj["Drivers"].Value<JArray>();
                        string driverName = drivers[0]["Name"].Value<string>();


                        EventRequest eventRequest = EventRequestRepository.Instance.GetRequest(Guid.Parse(requestId));

                        JObject response = new JObject();
                        response["EventId"] = eventRequest.Event.EventId;
                        response["Command"] = "driver";
                        response["DriverName"] = driverName;

                        MessageGateway.PublishMessage("event", "update", response.ToString());
                    }
                    break;
                default:
                    Console.WriteLine("No such command");
                    break;
            }
        }
    }
}