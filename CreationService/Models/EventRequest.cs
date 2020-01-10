using System;

namespace CreationService.Models
{
    public class EventRequest
    {
        public Guid RequestId {get;set;}
        public Event Event {get;set;}

        public EventRequest(Guid requestId, Event @event)
        {
            RequestId = requestId;
            Event = @event;
        }
    }
}