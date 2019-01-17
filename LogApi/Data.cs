using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ParsnipApi;

namespace LogApi
{
    public static class Data
    {
        public static List<LogEntry> logEntries { get; internal set; }

        public static bool ClearLogs()
        {
            
            try
            {
                using(SqlConnection conn = new SqlConnection(ParsnipApi.Data.sqlConnectionString))
                {
                    conn.Open();

                    SqlCommand deleteLogs = new SqlCommand("DELETE FROM t_LogEntries", conn);
                    deleteLogs.ExecuteNonQuery();
                }
                System.Diagnostics.Debug.WriteLine("Logs were cleared");
                logEntries.Clear();
                new LogEntry(Log.Default) { text = "Logs were cleared!" };
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
                
                using(SqlConnection conn = new SqlConnection(ParsnipApi.Data.sqlConnectionString))
                {
                    conn.Open();
                    SqlCommand selectLogEntries = new SqlCommand("SELECT * FROM t_LogEntries", conn);

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
