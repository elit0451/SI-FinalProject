
using System;
using System.Collections;
using System.IO;
using System.Text;
using CsvHelper;
using LocationService.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LocationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly string dataFolder;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public LocationController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            dataFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Data");
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable records;
            string recordsJSON = "";

            string fullPath = Path.Combine(dataFolder, "locations.csv");

            using (var reader = new StreamReader(fullPath, Encoding.UTF8))
            using (var csv = new CsvReader(reader))
            {
                records = csv.GetRecords<Location>();
                recordsJSON = JsonConvert.SerializeObject(records).ToString();
            }

            return new OkObjectResult(recordsJSON);
        }
    }
}
