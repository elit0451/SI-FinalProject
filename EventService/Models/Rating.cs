using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace EventService.Models
{
    public class Rating : IValidatableObject, IJSONConvertable
    {
        public int EventId { get; set; }
        public int Feedback { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EventId <= 0)
                yield return new ValidationResult("Invalid EventID");

            if (Feedback < 0 || Feedback > 5)
                yield return new ValidationResult("Invalid Feedback amount. Enter a number between 0 and 5");
        }

        public string ConvertToJson(string command)
        {
            JObject rating = JObject.FromObject(this);
            rating["Command"] = command;

            return rating.ToString();
        }
    }
}