using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotificationService.DTOs;
using NotificationService.Models;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        public AppDb Db { get; }

        public NotificationController(AppDb db)
        {
            Db = db;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetNotifications(int eventId)
        {
            List<NotificationDTO> resultOfDTOs = new List<NotificationDTO>();

            await Db.Connection.OpenAsync();
            var query = new NotificationQuery(Db);
            var result = await query.GetAllAsync(eventId);
            if (!(result is null))
                foreach (Notification notification in result)
                    resultOfDTOs.Add(notification.ConvertToDTO());

            return new OkObjectResult(resultOfDTOs);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AcceptApplication(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new NotificationQuery(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            result.MarkedRead = true;
            await result.UpdateAsync();
            return new OkObjectResult(result);
        }
    }
}