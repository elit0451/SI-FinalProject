using System;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CreationService.Models
{
    public class Event : IJSONConvertable
    {
        public int EventId { get; set; }
        public string Location { get; set; }
        public string DriverName { get; set; }
        public string DriveFrom { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int NumberOfPeople { get; set; }
        public EventType EventType { get; set; }
        public string ResponsibleName { get; set; }
        public string ResponsiblePhoneNr { get; set; }
        public string Notes { get; set; }

        public string ConvertToJson(string command)
        {
            JObject eventObj = JObject.FromObject(this);
            eventObj["command"] = command;

            return eventObj.ToString();
        }
    }


}