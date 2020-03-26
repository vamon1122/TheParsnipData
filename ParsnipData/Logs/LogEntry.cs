using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParsnipData;
using System.Data.SqlClient;
using System.Diagnostics;
using ParsnipData.Cookies;

namespace ParsnipData.Logging
{
    public class LogEntry
    {
        private bool isNew;
        public int Id { get; private set; }
        public int logId { get; set; }
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
            Id = (int)pReader[0];
            logId = (int)pReader[1];
            date = Convert.ToDateTime(pReader[3].ToString());
            _text = pReader[4].ToString();

            return true;
        }

        public LogEntry(Log pLog)
        {  
            isNew = true;
            SessionId = SessionId = Cookies.Cookie.Read("ASP.NET_sessionId");
            logId = pLog.Id;
            //userId = pUserId;
            date = Parsnip.AdjustedTime;

        }

        public LogEntry(Log.Ids id)
        {
            logId = (int)id;
        }

        private bool Insert()
        {
            string stage = "";
            try
            {
                using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    stage = "inserting LogEntry...";

                    using(SqlCommand insertLogEntry = new SqlCommand("log_entry_insert", conn))
                    {
                        insertLogEntry.CommandType = System.Data.CommandType.StoredProcedure;
                        insertLogEntry.Parameters.Add(new SqlParameter("log_id", logId));
                        insertLogEntry.Parameters.Add(new SqlParameter("session_id", SessionId));
                        insertLogEntry.Parameters.Add(new SqlParameter("text", text));
                        insertLogEntry.Parameters.Add(new SqlParameter("datetime_created", date));

                        conn.Open();
                        insertLogEntry.ExecuteNonQuery();
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
