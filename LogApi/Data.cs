using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace LogApi
{
    public static class Data
    {
        //Live
        //public static readonly string sqlConnectionString = "Server=198.38.83.33;Database=vamon112_parsnipdb;Uid=vamon112_ben;Password=ccjO07JT";
        //public static readonly string baseAddress = "http://api.theparsnip.co.uk";
        //public static readonly LogWriter AsyncLog = new LogWriter("Async_Login.txt", HttpContext.Current.Server.MapPath("~"));

        //Home
        public static readonly string sqlConnectionString = @"Data Source=BEN-PC\SQLEXPRESS;Initial Catalog=ParsnipTestDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static readonly string baseAddress = "http://localhost:59622";

        //Branson
        //public static readonly string sqlConnectionString = @"Data Source=branson\sqlexpress;Initial Catalog=ParsnipTestDb;Integrated Security=True";
        //public static readonly string baseAddress = "http://localhost:59622";
        //public static readonly LogWriter AsyncLog = new LogWriter("Async_Login.txt", @"C:\");

        public static DateTime adjustedTime { get { return DateTime.Now.AddHours(8); } }

        public static SqlConnection GetOpenDbConnection()
        {
            SqlConnection conn = new SqlConnection(sqlConnectionString);
            conn.Open();
            return conn;
        }

        public static List<LogEntry> logEntries { get; internal set; }

        public static bool ClearLogs()
        {
            
            try
            {
                using(SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();

                    SqlCommand deleteLogs = new SqlCommand("DELETE FROM t_LogEntries", conn);
                    deleteLogs.ExecuteNonQuery();
                }
                System.Diagnostics.Debug.WriteLine("Logs were cleared");
                logEntries.Clear();
                
                return true;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("There was an exception whilst deleteing log entries: {0}", e);
                return false;
            }
        }

        public static List<LogEntry> GetAllLogEntries()
        {
            try
            {
                logEntries = new List<LogEntry>();
                
                using(SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();
                    SqlCommand selectLogEntries = new SqlCommand("SELECT * FROM t_LogEntries ORDER BY dateTime DESC", conn);

                    using(SqlDataReader reader = selectLogEntries.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logEntries.Add(new LogEntry(reader));
                        }
                    }
                }
                return logEntries;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("There was an exception whilst loading log entries: {0}", e);
                throw new Exception(string.Format("Error whilst getting all log entries: {0}", e));
            }
        }
    }
}
