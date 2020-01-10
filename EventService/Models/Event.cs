using System;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace EventService.Models
{
    public class Event : IJSONConvertable
    {
        public int EventId { get; set; }
        public string Location { get; set; }
        public string DriverName { get; set; }
        public string DriveFrom { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int NumberOfPeople { get; set; }
        public EventType EventType { get; set; }
        public string ResponsibleName { get; set; }
        public string ResponsiblePhoneNr { get; set; }
        public string Notes { get; set; }

        internal AppDb Db { get; set; }

        internal Event(AppDb db)
        {
            Db = db;
        }

        public Event() { }

        public async Task InsertAsync()
        {
            using var applicationCmd = Db.Connection.CreateCommand();
            applicationCmd.CommandText = @"INSERT INTO `Event` (`Location`, `DriverName`, `DriveFrom`, `DateFrom`, `DateTo`, `NumberOfPeople`, `EventTypeId`, `ResponsibleName`, `ResponsiblePhoneNr`, `Notes`) 
                                            VALUES (@location, @drivername, @drivefrom, @datefrom, @dateto, @numberofpeople, @eventtypeid, @responsiblename, @responsiblephonenr, @notes);";
            BindParams(applicationCmd);
            await applicationCmd.ExecuteNonQueryAsync();

            EventId = (int) applicationCmd.LastInsertedId;
        }

        public async Task UpdateAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `Event` 
                SET `Location` = @location, `DriverName` = @drivername, `DriveFrom` = @drivefrom, `DateFrom` = @datefrom, `DateTo` = @dateto, `NumberOfPeople` = @numberofpeople, `EventTypeId` = @eventtypeid, `ResponsibleName` = @responsiblename, `ResponsiblePhoneNr` = @responsiblephonenr, `Notes` = @notes 
                WHERE `EventId` = @eventid;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@eventid",
                DbType = DbType.Int32,
                Value = EventId,
            });
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@location",
                DbType = DbType.String,
                Value = Location,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@drivername",
                DbType = DbType.String,
                Value = DriverName,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@drivefrom",
                DbType = DbType.String,
                Value = DriveFrom,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@datefrom",
                DbType = DbType.Date,
                Value = DateFrom,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@dateto",
                DbType = DbType.Date,
                Value = DateTo,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@numberofpeople",
                DbType = DbType.Int32,
                Value = NumberOfPeople,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@eventtypeid",
                DbType = DbType.Int32,
                Value = EventType.EventTypeId,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@responsiblename",
                DbType = DbType.String,
                Value = ResponsibleName,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@responsiblephonenr",
                DbType = DbType.String,
                Value = ResponsiblePhoneNr,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@notes",
                DbType = DbType.String,
                Value = Notes,
            });
        }

        public string ConvertToJson(string command)
        {
            JObject eventObj = JObject.FromObject(this);
            eventObj["Command"] = command;

            return eventObj.ToString();
        }
    }
}