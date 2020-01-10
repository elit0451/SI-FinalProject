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

        internal AppDb Db { get; set; }

        internal Rating(AppDb db)
        {
            Db = db;
        }

        public Rating() { }

        public async Task InsertAsync()
        {
            using var applicationCmd = Db.Connection.CreateCommand();
            applicationCmd.CommandText = @"INSERT INTO `Rating` (`EventId`, `Feedback`) VALUES (@eventid, @feedback);";
            BindParams(applicationCmd);
            await applicationCmd.ExecuteNonQueryAsync();
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@eventid",
                DbType = DbType.Int32,
                Value = EventId,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@feedback",
                DbType = DbType.Int32,
                Value = Feedback,
            });
        }

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
            rating["command"] = command;

            return rating.ToString();
        }
    }
}