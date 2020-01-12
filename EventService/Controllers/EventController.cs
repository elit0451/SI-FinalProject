using System.Threading.Tasks;
using EventService.Models;
using EventService.RabbitMQ;
using Microsoft.AspNetCore.Mvc;

namespace EventService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        public AppDb Db { get; }
        private IRabbitMQPersistentConnection rabbitMQ;

        public EventController(AppDb db, IRabbitMQPersistentConnection rabbitmq)
        {
            Db = db;
            rabbitMQ = rabbitmq;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewEvent(Event eventObj)
        {
            await Db.Connection.OpenAsync();
            eventObj.Db = Db;
            await eventObj.InsertAsync();

            rabbitMQ.PublishToChannel("event", "add", eventObj.ConvertToJson("add"));
            
            return new OkObjectResult(eventObj.ConvertToDTO());
        }
    }
}
