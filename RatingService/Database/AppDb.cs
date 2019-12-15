using System;
using MySql.Data.MySqlClient;

namespace RatingService.Database
{
    public class AppDb : IDisposable
    {
        private static AppDb _instance = null;
        public static AppDb Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new AppDb();
                return _instance;
            }
        }
        public MySqlConnection Connection { get; }

        private AppDb()
        {
            Connection = new MySqlConnection("server=localhost;user id=root;password=soft2019Backend;port=3306;database=RatingService;");
        }

        public void Dispose() => Connection.Dispose();
    }
}