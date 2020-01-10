using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NotificationService.DTOs;

namespace NotificationService.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int EventId { get; set; }
        public string Content { get; set; }
        public bool MarkedRead { get; set; }
        internal AppDb Db { get; set; }

        public Notification() { }
        public Notification(AppDb db)
        {
            Db = db;
        }

        public async Task InsertAsync()
        {
            var applicationCmd = Db.Connection.CreateCommand();
            applicationCmd.CommandText = @"INSERT INTO `Notification` (`EventId`, `Content`) 
                                            VALUES (@eventid, @content);";
            BindParams(applicationCmd);
            await applicationCmd.ExecuteNonQueryAsync();

            NotificationId = (int) applicationCmd.LastInsertedId;
        }
        public async Task UpdateAsync()
        {
            var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `Notification` SET `MarkedRead` = @markedread WHERE `NotificationId` = @notificationid;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@notificationid",
                DbType = DbType.Int32,
                Value = NotificationId,
            });
        }
        private void BindParams(MySqlCommand command)
        {
            command.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@eventid",
                DbType = DbType.Int32,
                Value = EventId,
            });

            command.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@content",
                DbType = DbType.String,
                Value = Content,
            });

            command.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@markedread",
                DbType = DbType.Boolean,
                Value = MarkedRead,
            });
        }

        public NotificationDTO ConvertToDTO()
        {
            return new NotificationDTO()
            {
                NotificationId = this.NotificationId,
                Content = this.Content
            };
        }
    }
}