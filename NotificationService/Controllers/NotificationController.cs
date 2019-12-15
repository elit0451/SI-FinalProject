using Microsoft.AspNetCore.Mvc;

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

        /* [HttpPost]
        public async Task<IActionResult> AddNewRating(Rating rating)
        {
            await Db.Connection.OpenAsync();
            rating.Db = Db;
            await rating.InsertAsync();
            return new OkObjectResult(rating);
        }

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