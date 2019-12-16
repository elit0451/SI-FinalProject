using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<IActionResult> AddNewNotification(Notification notification)
        {
            await Db.Connection.OpenAsync();
            notification.Db = Db;
            await notification.InsertAsync();
            return new OkObjectResult(notification);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotifications(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new NotificationQuery(Db);
            var result = await query.GetAllAsync(id);
            return new OkObjectResult(result);
        }
    }
}