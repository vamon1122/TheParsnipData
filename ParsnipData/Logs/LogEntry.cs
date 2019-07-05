using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParsnipData;
using System.Data.SqlClient;
using System.Diagnostics;
using ParsnipData.Cookies;

namespace ParsnipData.Logs
{
    public class LogEntry
    {
        private bool isNew;
        public Guid id { get; private set; }
        public Guid log_id { get; set; }
        public Guid userId { get; private set;  } 
        public DateTime date { get; private set; }
        private string SessionId;

        private string _type;
        public string type { get { return _type; } set{ if (value.Length < 11)  _type = value;  else  throw new FormatException(String.Format("The value for type \"{0}\" is too long!", value)); } }

        private string _text;

        public string text { get { return _text; } set {
                if (value.Length < 4001)
                {
                    _text = value;
                    Debug.WriteLine("----------[LOG ENTRY] - " + text);
                    if(isNew) Insert();
                }
                else
                {
                    throw new FormatException(
                        String.Format("The value for type \"{0}\" is too long!",
                        value));
                }
            }
        }

        private static DateTime lastEntry;

        public LogEntry(SqlDataReader pReader)
        {
            isNew = false;
            AddValues(pReader);
        }

        internal bool AddValues(SqlDataReader pReader)
        {
            isNew = false;
            id = new Guid(pReader[0].ToString());
            log_id = new Guid(pReader[1].ToString());
            date = Convert.ToDateTime(pReader[3].ToString());
            _text = pReader[5].ToString();

            if (pReader[2] != DBNull.Value) userId = new Guid(pReader[1].ToString());
            if (pReader[4] != DBNull.Value) type = pReader[3].ToString();

            return true;
        }

        public LogEntry(Log pLog)
        {
            isNew = true;
            SessionId = SessionId = Cookies.Cookie.Read("ASP.NET_sessionId");
            id = Guid.NewGuid();
            if (pLog.Id == Guid.Empty)
                throw new Exception("LogId was empty!");
            Debug.WriteLine("----------Creating new log entry. Logid = " + log_id);
            log_id = pLog.Id;
            //userId = pUserId;
            date = Parsnip.adjustedTime;

        }

        private bool Insert()
        {
            string stage = "";
            try
            {
                using (SqlConnection openConn = Parsnip.GetOpenDbConnection())
                {
                    stage = "inserting LogEntry...";
                    
                    Debug.WriteLine("---------- BEN! - session_id = " + SessionId);
                    

                    SqlCommand insertLogEntry = new SqlCommand("INSERT INTO log_entry (log_entry_id, log_id, session_id, date_time_created, text) VALUES(@log_entry_id, @log_id, @session_id, @date_time_created, @text)", openConn);
                    insertLogEntry.Parameters.Add(new SqlParameter("log_entry_id", id));
                    insertLogEntry.Parameters.Add(new SqlParameter("log_id", log_id));
                    insertLogEntry.Parameters.Add(new SqlParameter("session_id", SessionId));
                    insertLogEntry.Parameters.Add(new SqlParameter("text", text));
                    insertLogEntry.Parameters.Add(new SqlParameter("date_time_created", date));


                    

                    Debug.WriteLine("text = " + text);
                    Debug.WriteLine("_text = " + _text);

                    
                    

                    insertLogEntry.ExecuteNonQuery();

                    /*
                    if (userId != null && userId != Guid.Empty)
                    {
                        stage = "UserId was null. Updating LogEntry...";
                        SqlCommand insertLogEntry_updateUserId = new SqlCommand("UPDATE log_entry SET userId = @userId WHERE log_entry_id = @id", openConn);
                        insertLogEntry_updateUserId.Parameters.Add(new SqlParameter("userId", userId));
                        insertLogEntry_updateUserId.Parameters.Add(new SqlParameter("id", id));

                        insertLogEntry_updateUserId.ExecuteNonQuery();
                    }
                    */

                    if (type != null)
                    {
                        stage = "type was null. Updating LogEntry...";
                        SqlCommand insertLogEntry_updateType = new SqlCommand("UPDATE log_entry SET type = @type WHERE log_entry_id = @id", openConn);
                        insertLogEntry_updateType.Parameters.Add(new SqlParameter("type", type));
                        insertLogEntry_updateType.Parameters.Add(new SqlParameter("id", id));

                        insertLogEntry_updateType.ExecuteNonQuery();
                    }

                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("There was an exception whilst inserting the log entry. I was {0} : {1}", stage, e));
                return false;
            }
        }
    }
}
