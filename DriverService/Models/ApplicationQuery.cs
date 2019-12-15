using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DriverService.Models
{
    internal class ApplicationQuery
    {
        private AppDb Db;

        public ApplicationQuery(AppDb db)
        {
            Db = db;
        }

        public async Task<List<Application>> FindAvailableDrivers(DateTime from, DateTime to)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT a.*
                                FROM Application AS a, Application_Workday AS aw
                                WHERE a.ApplicationId = aw.ApplicationId 
                                    AND Accepted = 1
                                    AND FIND_IN_SET (WorkdayId, @workdays)  
                                    AND a.ApplicationId NOT IN 
                                        (SELECT ApplicationId 
                                        FROM Booking 
                                        WHERE COALESCE(FromTime BETWEEN @from AND @to, FALSE) 
                                            OR COALESCE(ToTime BETWEEN @from AND @to, FALSE))
                                GROUP BY a.ApplicationId
                                HAVING COUNT(DISTINCT WorkdayId) = @workdaycount";

            // TODO: Convert dates to days of the week
            var intArr = new List<int>(){0,1,2,3};
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@workdays",
                DbType = DbType.String,
                Value = string.Join(",",intArr),
            });

            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@from",
                DbType = DbType.DateTime,
                Value = from,
            });

            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@to",
                DbType = DbType.DateTime,
                Value = to,
            });

            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@workdaycount",
                DbType = DbType.Int32,
                Value = intArr.Count,
            });
            var result = await ReadAllDriversAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result : null;
        }

        public async Task<Application> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Application, Application_Workday WHERE Application.ApplicationId = Application_Workday.ApplicationId AND Application.ApplicationId = @applicationid;";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@applicationid",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReadAllDriverAndWorkDaysAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<List<Application>> GetAllAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Application, Application_Workday WHERE Application.ApplicationId = Application_Workday.ApplicationId AND Accepted = 0;";
            var result = await ReadAllDriverAndWorkDaysAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result : null;
        }

        private async Task<List<Application>> ReadAllDriverAndWorkDaysAsync(DbDataReader reader)
        {
            var applications = new Dictionary<int, Application>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var listWeekDays = new List<WeekDay>();
                    listWeekDays.Add((WeekDay)reader.GetInt32(6));

                    var application = new Application(Db)
                    {
                        ApplicationID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        DateOfBirth = reader.GetDateTime(2),
                        LicenseNumber = reader.GetString(3),
                        Accepted = reader.GetBoolean(4),
                        WeekDays = listWeekDays
                    };

                    if (applications.ContainsKey(application.ApplicationID))
                        applications[application.ApplicationID].WeekDays.AddRange(application.WeekDays);
                    else
                        applications.Add(application.ApplicationID, application);
                }
            }

            List<Application> returnList = new List<Application>();
            returnList.AddRange(applications.Values);

            return returnList;
        }

        private async Task<List<Application>> ReadAllDriversAsync(DbDataReader reader)
        {
            var applications = new List<Application>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {

                    var application = new Application(Db)
                    {
                        ApplicationID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        DateOfBirth = reader.GetDateTime(2),
                        LicenseNumber = reader.GetString(3),
                        Accepted = reader.GetBoolean(4),
                    };

                    applications.Add(application);
                }
            }
            
            return applications;
        }
    }
}