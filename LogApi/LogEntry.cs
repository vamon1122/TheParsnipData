using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using CookieApi;

namespace LogApi
{
    public class LogEntry
    {
        private bool isNew;
        public Guid id { get; private set; }
        public Guid logId { get; set; }
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
            logId = new Guid(pReader[1].ToString());
            date = Convert.ToDateTime(pReader[3].ToString());
            _text = pReader[5].ToString();

            if (pReader[2] != DBNull.Value) userId = new Guid(pReader[1].ToString());
            if (pReader[4] != DBNull.Value) type = pReader[3].ToString();

            return true;
        }

        public LogEntry(Log pLog)
        {
            isNew = true;
            SessionId = SessionId = CookieApi.Cookie.Read("ASP.NET_sessionId");
            id = Guid.NewGuid();
            if (pLog.Id == Guid.Empty)
                throw new Exception("LogId was empty!");
            Debug.WriteLine("----------Creating new log entry. Logid = " + logId);
            logId = pLog.Id;
            //userId = pUserId;
            date = Data.adjustedTime;

        }

        private bool Insert()
        {
            string stage = "";
            try
            {
                using (SqlConnection openConn = Data.GetOpenDbConnection())
                {
                    stage = "inserting LogEntry...";
                    
                    Debug.WriteLine("---------- BEN! - sessionId = " + SessionId);
                    

                    SqlCommand insertLogEntry = new SqlCommand("INSERT INTO t_LogEntries (id, logId, sessionId, dateTime, text) VALUES(@id, @logId, @sessionId, @dateTime, @text)", openConn);
                    insertLogEntry.Parameters.Add(new SqlParameter("id", id));
                    insertLogEntry.Parameters.Add(new SqlParameter("logId", logId));
                    insertLogEntry.Parameters.Add(new SqlParameter("sessionId", SessionId));
                    insertLogEntry.Parameters.Add(new SqlParameter("text", text));
                    insertLogEntry.Parameters.Add(new SqlParameter("dateTime", date));


                    

                    Debug.WriteLine("text = " + text);
                    Debug.WriteLine("_text = " + _text);

                    
                    

                    insertLogEntry.ExecuteNonQuery();

                    if (userId != null && userId != Guid.Empty)
                    {
                        stage = "UserId was null. Updating LogEntry...";
                        SqlCommand insertLogEntry_updateUserId = new SqlCommand("UPDATE t_LogEntries SET userId = @userId WHERE id = @id", openConn);
                        insertLogEntry_updateUserId.Parameters.Add(new SqlParameter("userId", userId));
                        insertLogEntry_updateUserId.Parameters.Add(new SqlParameter("id", id));

                        insertLogEntry_updateUserId.ExecuteNonQuery();
                    }

                    if (type != null)
                    {
                        stage = "type was null. Updating LogEntry...";
                        SqlCommand insertLogEntry_updateType = new SqlCommand("UPDATE t_LogEntries SET type = @type WHERE id = @id", openConn);
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
