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
            string command = receivedObj["command"].Value<string>();

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
                    requestCar["from"] = dateFrom;
                    requestCar["to"] = dateTo;
                    requestCar["stationId"] = driveFrom;
                    requestCar["command"] = "getAvailableCars";
                    requestCar["requestId"] = r.RequestId;

                    JObject requestDriver = new JObject();
                    requestDriver["from"] = dateFrom;
                    requestDriver["to"] = dateTo;
                    requestDriver["requestId"] = r.RequestId;


                    MessageGateway.PublishMessage("connector.requests", requestCar.ToString());
                    MessageGateway.PublishMessage("driver.find", requestDriver.ToString());
                    break;

                case "carFound":
                    {
                        string requestId = receivedObj["requestId"].Value<string>();
                        JArray cars = receivedObj["cars"].Value<JArray>();
                        string license = cars[0]["license"].Value<string>();


                        EventRequest eventRequest = EventRequestRepository.Instance.GetRequest(Guid.Parse(requestId));

                        JObject response = new JObject();
                        response["EventId"] = eventRequest.Event.EventId;
                        response["command"] = "car";
                        response["license"] = license;

                        MessageGateway.PublishMessage("event.update", response.ToString());
                    }
                    break;

                case "driverFound":
                    {
                        string requestId = receivedObj["requestId"].Value<string>();
                        JArray drivers = receivedObj["drivers"].Value<JArray>();
                        string driverName = drivers[0]["Name"].Value<string>();


                        EventRequest eventRequest = EventRequestRepository.Instance.GetRequest(Guid.Parse(requestId));

                        JObject response = new JObject();
                        response["EventId"] = eventRequest.Event.EventId;
                        response["command"] = "car";
                        response["driverName"] = driverName;

                        MessageGateway.PublishMessage("event.update", response.ToString());
                    }
                    break;
                default:
                    Console.WriteLine("No such command");
                    break;
            }
        }
    }
}