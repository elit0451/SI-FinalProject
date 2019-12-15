using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace EventService.Models
{
    internal class RatingQuery
    {
        private AppDb Db;

        public RatingQuery(AppDb db)
        {
            Db = db;
        }

        public async Task<Rating> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Rating WHERE EventId = @eventid";

            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@eventid",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        private async Task<List<Rating>> ReadAllAsync(DbDataReader reader)
        {
            var ratings = new List<Rating>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var rating = new Rating(Db)
                    {
                        EventId = reader.GetInt32(0),
                        Feedback = reader.GetInt32(1),
                    };
                    ratings.Add(rating);
                }
            }

            return ratings;
        }
        
    }
}