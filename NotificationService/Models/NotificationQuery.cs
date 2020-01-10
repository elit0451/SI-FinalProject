using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace NotificationService.Models
{
    public class NotificationQuery
    {
        private AppDb Db;
        public NotificationQuery(AppDb db)
        {
            Db = db;
        }

        public async Task<Notification> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Notification WHERE NotificationId = @notificationid;";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@notificationid",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReallAllNotificationsAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<List<Notification>> GetAllAsync(int id)
        {
            var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Notification WHERE EventId = @eventid;";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@eventid",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReallAllNotificationsAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result : null;
        }

        private async Task<List<Notification>> ReallAllNotificationsAsync(DbDataReader reader)
        {
            List<Notification> returnList = new List<Notification>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var notification = new Notification(Db)
                    {
                        NotificationId = reader.GetInt32(0),
                        EventId = reader.GetInt32(1),
                        Content = reader.GetString(2),
                        MarkedRead = reader.GetBoolean(3)
                    };

                    if (!notification.MarkedRead)
                        returnList.Add(notification);
                }
            }

            return returnList;
        }
    }
}