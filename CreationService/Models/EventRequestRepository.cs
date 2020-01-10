using System;
using System.Collections.Generic;

namespace CreationService.Models
{
    public class EventRequestRepository
    {
        Dictionary<Guid, EventRequest> requests;

        private static EventRequestRepository _instance;

        public static EventRequestRepository Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new EventRequestRepository();

                return _instance;
            }
        }

        private EventRequestRepository()
        {
            requests = new Dictionary<Guid, EventRequest>();
        }

        public void AddRequest(EventRequest r)
        {
            if (requests.ContainsKey(r.RequestId))
                return;

            requests.Add(r.RequestId, r);
        }

        public EventRequest GetRequest(Guid id)
        {
            return requests.ContainsKey(id) ? requests[id] : null; 
        }
    }
}