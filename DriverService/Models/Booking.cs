using System;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DriverService.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int ApplicationId { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        internal AppDb Db { get; set; }

        internal Booking(AppDb db)
        {
            Db = db;
        }

        public Booking() { }

        public async Task InsertAsync()
        {
            using var bookingCmd = Db.Connection.CreateCommand();
            bookingCmd.CommandText = @"INSERT INTO `Booking` (`ApplicationId`, `FromTime`,`ToTime`) VALUES (@applicationid, @fromtime, @totime);";
            BindParams(bookingCmd);
            await bookingCmd.ExecuteNonQueryAsync();
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@applicationid",
                DbType = DbType.Int32,
                Value = ApplicationId,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@fromtime",
                DbType = DbType.DateTime,
                Value = FromTime,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@totime",
                DbType = DbType.DateTime,
                Value = ToTime,
            });
        }
    }
}