using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using RatingService.Database;

namespace RatingService.Models
{
    public class Rating: IJSONConvertable
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
            var applicationCmd = Db.Connection.CreateCommand();
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

        public string ConvertToJson(string command)
        {
            JObject rating = JObject.FromObject(this);
            
            if(command != string.Empty)
                rating["command"] = command;

            return rating.ToString();
        }
    }
}