using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NotificationService.DTOs;

namespace NotificationService.Models
{
    public class NotificationQuery
    {
        private AppDb Db;
        public NotificationQuery(AppDb db)
        {
            Db = db;
        }

        

        public async Task<List<NotificationDTO>> GetAllAsync(int id)
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

        private async Task<List<NotificationDTO>> ReallAllNotificationsAsync(DbDataReader reader)
        {
            List<NotificationDTO> returnList = new List<NotificationDTO>();
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

                    if(!notification.MarkedRead)
                    {
                        var notificationDTO = new NotificationDTO(Db)
                        {
                            NotificationId = notification.NotificationId,
                            Content = notification.Content
                        };
                        returnList.Add(notificationDTO);
                    }
                }
            }

            return returnList;
        }
    }
}