using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DriverService.Models
{
    public enum WeekDay
    {
        SUNDAY = 0,
        MONDAY = 1,
        TUESDAY = 2,
        WEDNESDAY = 3,
        THURSDAY = 4,
        FRIDAY = 5,
        SATURDAY = 6
    }
    public class Application
    {
        public int ApplicationID { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string LicenseNumber { get; set; }
        public List<WeekDay> WeekDays { get; set; }
        public bool Accepted { get; set; }

        internal AppDb Db { get; set; }

        internal Application(AppDb db)
        {
            Db = db;
        }

        public Application(){}

        public async Task InsertAsync()
        {
            using var applicationCmd = Db.Connection.CreateCommand();
            applicationCmd.CommandText = @"INSERT INTO `Application` (`Name`, `DateOfBirth`,`LicenseNumber`) VALUES (@name, @dateofbirth, @licensenumber);";
            BindParams(applicationCmd);
            await applicationCmd.ExecuteNonQueryAsync();
            

            ApplicationID = (int) applicationCmd.LastInsertedId;

            foreach(WeekDay weekDay in WeekDays){
                using var workdayCmd = Db.Connection.CreateCommand();
                workdayCmd.CommandText = @"INSERT INTO `Application_Workday` (`ApplicationId`, `WorkdayId`) VALUES (@applicationid, @workdayid);";
                workdayCmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@applicationid",
                    DbType = DbType.Int32,
                    Value = ApplicationID,
                });
                workdayCmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@workdayid",
                    DbType = DbType.Int32,
                    Value = (int)weekDay,
                });
                await workdayCmd.ExecuteNonQueryAsync();    
            }
        }

        public async Task UpdateAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `Application` SET `Accepted` = @accepted WHERE `ApplicationId` = @applicationid;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@applicationid",
                DbType = DbType.Int32,
                Value = ApplicationID,
            });
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@name",
                DbType = DbType.String,
                Value = Name,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@dateofbirth",
                DbType = DbType.DateTime,
                Value = DateOfBirth,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@licensenumber",
                DbType = DbType.String,
                Value = LicenseNumber,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@accepted",
                DbType = DbType.Boolean,
                Value = Accepted,
            });
        }
    }


}