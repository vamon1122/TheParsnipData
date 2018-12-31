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

        public static bool LoadLogEntries()
        {
            try
            {
                logEntries.Clear();
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
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
