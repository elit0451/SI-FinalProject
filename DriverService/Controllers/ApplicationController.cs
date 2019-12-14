using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriverService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DriverService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : ControllerBase
    {
        public AppDb Db { get; }

        public ApplicationController(AppDb db)
        {
            Db = db;
        }


        [HttpPost]
        public async Task<IActionResult> AddNewApplication(Application application)
        {
            await Db.Connection.OpenAsync();
            application.Db = Db;
            await application.InsertAsync();
            return new OkObjectResult(application);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllApplications()
        {
            await Db.Connection.OpenAsync();
            var query = new ApplicationQuery(Db);
            var result = await query.GetAllAsync();
            return new OkObjectResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AcceptApplication(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new ApplicationQuery(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            result.Accepted = true;
            await result.UpdateAsync();
            return new OkObjectResult(result);
        }
    }
}
