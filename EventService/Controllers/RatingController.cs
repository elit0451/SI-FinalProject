using System;
using System.Threading.Tasks;
using EventService.Models;
using EventService.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EventService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RatingController : ControllerBase
    {
        private IRabbitMQPersistentConnection rabbitMQ;

        public RatingController(IRabbitMQPersistentConnection rabbitmq)
        {
            rabbitMQ = rabbitmq;
        }


        [HttpPost]
        public IActionResult AddNewRating(Rating rating)
        {
            if (ModelState.IsValid)
            {
                rabbitMQ.PublishToChannel("event.feedback", rating.ConvertToJson("add"));
                return new OkObjectResult(rating);
            }
            else
                return new BadRequestResult();
        }

        [HttpGet("{id}")]
        public IActionResult GetSpecificRating(int id)
        {
            JObject request = new JObject();
            request["command"] = "get";
            request["EventId"] = id;

            string result = rabbitMQ.RPCRequest("event.feedback", request.ToString());

            Rating rating = JsonConvert.DeserializeObject<Rating>(result);

            return new OkObjectResult(rating);
        }
    }
}
