﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using ParsnipData;
using ParsnipData.Cookies;

namespace ParsnipData.Logs
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
            DateTimeCreated = Parsnip.AdjustedTime;

        }

        Log()
        {
            SessionId = Cookie.Read("sessionId");
        }

        public Log(string pName) : this()
        {
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();

                Debug.WriteLine("Creating new Log object with name = " + pName);
                Name = pName;

                if (NameExists(conn))
                {
                    SelectByName(conn);
                }
                else
                {
                    Id = Guid.NewGuid();
                    DateTimeCreated = Parsnip.AdjustedTime;
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

                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();
                    SqlCommand selectLogEntries = new SqlCommand("SELECT * FROM log_entry WHERE log_id = @log_id  ORDER BY date_time_created DESC", conn);
                    selectLogEntries.Parameters.Add(new SqlParameter("log_id", Id));

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
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();

                SqlCommand GetLogs = new SqlCommand("SELECT * FROM log", conn);
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
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();

                if (Id != null && Id != Guid.Empty)
                {
                    if (IdExists(conn))
                        return SelectById(conn);
                    else
                        return false;

                }

                if (Name != null && Name != "")
                {
                    if (NameExists(conn))
                        return SelectByName(conn);
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
                SqlCommand selectLog = new SqlCommand("SELECT * FROM log WHERE log_id = @log_id", pOpenConn);
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
                SqlCommand selectLog = new SqlCommand("SELECT * FROM log WHERE name = @name", pOpenConn);
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
            using (SqlConnection openConn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                openConn.Open();

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
                SqlCommand doesLogIdExist = new SqlCommand("SELECT COUNT(*) FROM log WHERE log_id = @log_id", pOpenConn);
                doesLogIdExist.Parameters.Add(new SqlParameter("log_id", Id));

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
                SqlCommand doesLogNameExist = new SqlCommand("SELECT COUNT(*) FROM log WHERE name = @name", pOpenConn);
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
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();

                    SqlCommand insertLog = new SqlCommand("INSERT INTO log (log_id, date_time_created, name) VALUES(@log_id, @date_time_created, @name)", conn);
                    insertLog.Parameters.Add(new SqlParameter("log_id", Id));
                    insertLog.Parameters.Add(new SqlParameter("date_time_created", DateTimeCreated));
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
