using System;
using System.Collections.Generic;
using EventService.Models;

namespace EventService.DTOs
{
    public class EventDTO
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
        public List<LinkDTO> RelatedLinks { get; set; }

        public EventDTO() { }
    }
}