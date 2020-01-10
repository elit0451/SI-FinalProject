using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace EventService.Models
{
    public class EventQuery
    {
        private AppDb Db;
        public EventQuery(AppDb db)
        {
            Db = db;
        }

        public async Task<Event> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Event WHERE EventId = @eventid;";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@eventid",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReallAllEventsAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        private async Task<List<Event>> ReallAllEventsAsync(DbDataReader reader)
        {
            List<Event> returnList = new List<Event>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    Event e = new Event(Db)
                    {
                        EventId = reader.GetInt32(0),
                        Location = reader.GetString(1),
                        DriverName = reader.GetString(2),
                        DriveFrom = reader.GetString(3),
                        DateFrom = reader.GetDateTime(4),
                        DateTo = reader.GetDateTime(5),
                        NumberOfPeople = reader.GetInt32(6),
                        EventType = new EventType() { EventTypeId = reader.GetInt32(7)},
                        ResponsibleName = reader.GetString(8),
                        ResponsiblePhoneNr = reader.GetString(9),
                        Notes = reader.GetString(10),
                    };

                    returnList.Add(e);
                }
            }
            return returnList;
        }
    }
}