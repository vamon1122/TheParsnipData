using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using ParsnipApi;
using CookieApi;

namespace LogApi
{
    public class Log
    {
        private bool isNew;
        public Guid Id { get; private set; }
        public DateTime DateTimeCreated { get; private set; }
        public string Name { get; set;  }
        public string SessionId { get; private set; }


        public Log(SqlDataReader pReader) : this()
        {
            AddValues(pReader);
        }

        public Log(Guid pLogId) : this()
        {
            isNew = true;
            Id = pLogId;
            DateTimeCreated = Parsnip.adjustedTime;

        }

        Log()
        {
            SessionId = Cookie.Read("sessionId");
        }

        public Log(string pName) : this()
        {
            using (SqlConnection openConn = Parsnip.GetOpenDbConnection())
            {
                Debug.WriteLine("Creating new Log object with name = " + pName);
                Name = pName;

                if (NameExists(openConn))
                {
                    SelectByName(openConn);
                }
                else
                {
                    Id = Guid.NewGuid();
                    DateTimeCreated = Parsnip.adjustedTime;
                    Insert();
                }
            }
        }

        public static Log Default { get { return new Log("general"); } }

        public List<LogEntry> GetLogEntries()
        {
            try
            {
                var logEntries = new List<LogEntry>();

                using (SqlConnection conn = new SqlConnection(Parsnip.sqlConnectionString))
                {
                    conn.Open();
                    SqlCommand selectLogEntries = new SqlCommand("SELECT * FROM t_LogEntries WHERE logId = @logId  ORDER BY dateTime DESC", conn);
                    selectLogEntries.Parameters.Add(new SqlParameter("logId", Id));

                    using (SqlDataReader reader = selectLogEntries.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logEntries.Add(new LogEntry(reader));
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
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all logs...");

            var logs = new List<Log>();
            using (SqlConnection conn = Parsnip.GetOpenDbConnection())
            {
                SqlCommand GetLogs = new SqlCommand("SELECT * FROM t_Logs", conn);
                using (SqlDataReader reader = GetLogs.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(new Log(reader));
                    }
                }
            }

            foreach (Log temp in logs)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found log {0} with id {1}", temp.Name, temp.Id));
            }

            return logs;
        }

        internal bool AddValues(SqlDataReader pReader)
        {
            try
            {
                    isNew = false;
                    Id = new Guid(pReader[0].ToString());
                    DateTimeCreated = Convert.ToDateTime(pReader[1].ToString());
                    Name = pReader[2].ToString();
                
            }
            catch(Exception e)
            {
                Debug.WriteLine("Exception whilst adding values to a log: " + e);
                return false;
            }
            return true;
        }

        public bool Select()
        {
            using (SqlConnection openConnection = Parsnip.GetOpenDbConnection())
            {
                if (Id != null && Id != Guid.Empty)
                {
                    if (IdExists(openConnection))
                        return SelectById(openConnection);
                    else
                        return false;

                }

                if (Name != null && Name != "")
                {
                    if (NameExists(openConnection))
                        return SelectByName(openConnection);
                    else
                        return false;
                }

                return false;

                
            }
        }

        bool SelectById(SqlConnection pOpenConn)
        {
            try
            {
                SqlCommand selectLog = new SqlCommand("SELECT * FROM t_Logs WHERE id = @id", pOpenConn);
                selectLog.Parameters.Add(new SqlParameter("id", Id));

                using (SqlDataReader reader = selectLog.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AddValues(reader);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception whilst selecting log by id: " + e);
                return false;
            }
            return true;
        }

        bool SelectByName(SqlConnection pOpenConn)
        {
            try
            {
                SqlCommand selectLog = new SqlCommand("SELECT * FROM t_Logs WHERE name = @name", pOpenConn);
                selectLog.Parameters.Add(new SqlParameter("name", Name));
                
                using(SqlDataReader reader = selectLog.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AddValues(reader);
                    }
                }
                
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception whilst selecting log by name: " + e);
                return false;
            }
            return true;
        }

        public bool Exists()
        {
            using (SqlConnection openConn = Parsnip.GetOpenDbConnection())
            {
                if (IdExists(openConn))
                {
                    Debug.WriteLine("The log Id already existed!");
                }

                if (NameExists(openConn))
                {
                    Debug.WriteLine("The log name already existed!");
                }

                return true;
            }
        }

        private bool IdExists(SqlConnection pOpenConn)
        {
            try
            {
                SqlCommand doesLogIdExist = new SqlCommand("SELECT COUNT(*) FROM t_Logs WHERE id = @id", pOpenConn);
                doesLogIdExist.Parameters.Add(new SqlParameter("id", Id));

                int logsFound;
                using (SqlDataReader reader = doesLogIdExist.ExecuteReader())
                {
                    reader.Read();
                    logsFound = Convert.ToInt16(reader[0]);
                }

                if (logsFound > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception whilst checking if log exists " + e);
                return false;
            }
        }

        private bool NameExists(SqlConnection pOpenConn)
        {
            try
            {
                SqlCommand doesLogNameExist = new SqlCommand("SELECT COUNT(*) FROM t_Logs WHERE name = @name", pOpenConn);
                doesLogNameExist.Parameters.Add(new SqlParameter("name", Name));

                int logsFound;
                using (SqlDataReader reader = doesLogNameExist.ExecuteReader())
                {
                    reader.Read();
                    logsFound = Convert.ToInt16(reader[0]);
                }

                if (logsFound > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception whilst checking if log exists " + e);
                return false;
            }
        }

        private bool Insert()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.sqlConnectionString))
                {
                    conn.Open();

                    SqlCommand insertLog = new SqlCommand("INSERT INTO t_Logs (id, dateTime, name) VALUES(@id, @dateTime, @name)", conn);
                    insertLog.Parameters.Add(new SqlParameter("id", Id));
                    insertLog.Parameters.Add(new SqlParameter("dateTime", DateTimeCreated));
                    insertLog.Parameters.Add(new SqlParameter("name", Name));

                    insertLog.ExecuteNonQuery();
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
