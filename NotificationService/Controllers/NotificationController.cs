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
        
        /*
        [HttpGet]
        public async Task<IActionResult> GetRating()
        {
            await Db.Connection.OpenAsync();
            var query = new ApplicationQuery(Db);
            var result = await query.GetAllAsync();
            return new OkObjectResult(result);
        } */
    }
}