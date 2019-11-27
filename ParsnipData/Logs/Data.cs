using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ParsnipData;

namespace ParsnipData.Logs
{
    public static class Data
    {
        public static List<LogEntry> logEntries { get; internal set; }

        public static bool ClearLogs()
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();

                    SqlCommand deleteAllLogs = new SqlCommand("log_entry_delete", conn);
                    deleteAllLogs.ExecuteNonQuery();
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
                
                using(SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (SqlCommand selectLogEntries = new SqlCommand("log_entry_select", conn))
                    {
                        selectLogEntries.CommandType = System.Data.CommandType.StoredProcedure;
                        
                        conn.Open();
                        using (SqlDataReader reader = selectLogEntries.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logEntries.Add(new LogEntry(reader));
                            }
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
