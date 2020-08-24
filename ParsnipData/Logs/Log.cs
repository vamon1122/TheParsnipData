using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using ParsnipData;
using ParsnipData.Cookies;

namespace ParsnipData.Logging
{
    public class Log
    {
        public static readonly Log General;
        public static readonly Log Debug;
        public static readonly Log Session;
        public static readonly Log LogInOut;
        public static readonly Log Access;
        public static readonly Log AccessJustification;
        public static readonly Log Test;

        public enum Ids
        {
            General = 1,
            Debug = 2,
            Session = 3,
            LogInOut = 4,
            Access = 5,
            AccessJustification = 6,
            Test = 7
        }

        static Log()
        {
            General = Select(Ids.General);
            Debug = Select(Ids.Debug);
            Session = Select(Ids.Session);
            LogInOut = Select(Ids.LogInOut);
            Access = Select(Ids.Access);
            AccessJustification = Select(Ids.AccessJustification);
            Test = Select(Ids.Test);
        }

        public DateTime DateTimeCreated { get; private set; }

        public int Id { get; set; }

        public string Name { get; set;  }
        public string SessionId { get; private set; }


        public Log(SqlDataReader pReader) : this()
        {
            AddValues(pReader);
        }

        Log()
        {
            SessionId = Cookie.Read("sessionId");
        }

        /*
        public Log(string pName) : this()
        {
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                Name = pName;


                if (!SelectByName(conn))
                {
                    DateTimeCreated = Parsnip.AdjustedTime;
                    Insert();
                }
            }
        }
        */

        public static Log Default { get { return Log.Select(Ids.General); } }

        public List<LogEntry> GetLogEntries()
        {
            try
            {
                var logEntries = new List<LogEntry>();

                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();
                    using (SqlCommand selectLogEntriesByLogId = new SqlCommand("log_entry_select_where_log_id", conn))
                    {
                        selectLogEntriesByLogId.CommandType = System.Data.CommandType.StoredProcedure;
                        selectLogEntriesByLogId.Parameters.Add(new SqlParameter("log_id", Id));

                        using (SqlDataReader reader = selectLogEntriesByLogId.ExecuteReader())
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
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("There was an exception whilst loading log entries: {0}", e);
                throw new Exception(string.Format("Error whilst getting log entries for log {0}: {1}",Name, e));
            }
        }

        public static List<Log> GetAllLogs()
        {
            var Log = new List<Log>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();

                using (SqlCommand getAllLog = new SqlCommand("log_select", conn))
                {
                    getAllLog.CommandType = System.Data.CommandType.StoredProcedure;
                    using (SqlDataReader reader = getAllLog.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Log.Add(new Log(reader));
                        }
                    }
                }
            }

            return Log;
        }

        internal bool AddValues(SqlDataReader reader)
        {
            try
            {
                Id = (int)reader[0];
                Name = reader[1].ToString();
                DateTimeCreated = Convert.ToDateTime(reader[2].ToString());
                
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception whilst adding values to a log: " + e);
                return false;
            }
            return true;
        }

        public static Log Select(Ids id)
        {
            Log log = null;
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                try
                {
                    using (SqlCommand selectLogByName = new SqlCommand("log_select_where_id", conn))
                    {
                        selectLogByName.CommandType = System.Data.CommandType.StoredProcedure;
                        selectLogByName.Parameters.Add(new SqlParameter("id", id));

                        conn.Open();
                        using (SqlDataReader reader = selectLogByName.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                log = new Log();
                                log.AddValues(reader);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception whilst selecting log by name: " + e);
                }
            }
            return log;
        }

        private bool Insert()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (SqlCommand insertLog = new SqlCommand("log_insert", conn))
                    
                    {
                        insertLog.CommandType = System.Data.CommandType.StoredProcedure;
                        insertLog.Parameters.Add(new SqlParameter("datetime_created", DateTimeCreated));
                        insertLog.Parameters.Add(new SqlParameter("name", Name));

                        conn.Open();
                        insertLog.ExecuteNonQuery();
                    }
                }
            }    
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }
    }
}
