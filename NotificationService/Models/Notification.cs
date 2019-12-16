using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace NotificationService.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int EventId { get; set; }
        public string Content { get; set; }
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
        }
    }
}