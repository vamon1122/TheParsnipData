using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParsnipApi;
using System.Data.SqlClient;

namespace LogApi
{
    public class LogEntry
    {
        private bool isNew;
        public Guid id { get; }
        public Guid userId { get; } 
        public DateTime date { get; }

        private string _type;
        public string type { get { return _type; } set{ if (value.Length < 11)  _type = value;  else  throw new FormatException(String.Format("The value for type \"{0}\" is too long!", value)); } }

        private string _text;

        

        public string text { get { return _text; } set {
                if (value.Length < 8001)
                {
                    _text = value;
                    System.Diagnostics.Debug.WriteLine(text);
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


        public LogEntry(SqlDataReader pReader)
        {
            isNew = false;
            id = new Guid(pReader[0].ToString());
            date = Convert.ToDateTime(pReader[2].ToString());
            text = pReader[4].ToString();

            if (pReader[1] != DBNull.Value) userId = new Guid(pReader[1].ToString());
            if (pReader[3] != DBNull.Value) type = pReader[3].ToString();
        }

        public LogEntry(Guid pUserId)
        {
            isNew = true;
            id = Guid.NewGuid();
            userId = pUserId;
            date = ParsnipApi.Data.adjustedTime;

        }

        private bool Insert()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ParsnipApi.Data.sqlConnectionString))
                {
                    conn.Open();

                    SqlCommand insertLogEntry = new SqlCommand("INSERT INTO t_LogEntries (id, dateTime, value) VALUES(@id, @dateTime, @value)", conn);
                    insertLogEntry.Parameters.Add(new SqlParameter("id", id));
                    insertLogEntry.Parameters.Add(new SqlParameter("dateTime", date));
                    insertLogEntry.Parameters.Add(new SqlParameter("value", text));

                    insertLogEntry.ExecuteNonQuery();

                    if (userId != null && userId != Guid.Empty)
                    {
                        SqlCommand insertLogEntry_updateUserId = new SqlCommand("UPDATE t_LogEntries SET userId = @userId WHERE id = @id", conn);
                        insertLogEntry_updateUserId.Parameters.Add(new SqlParameter("userId", userId));
                        insertLogEntry_updateUserId.Parameters.Add(new SqlParameter("id", id));

                        insertLogEntry_updateUserId.ExecuteNonQuery();
                    }

                    if (type != null)
                    {
                        SqlCommand insertLogEntry_updateType = new SqlCommand("UPDATE t_LogEntries SET type = @type WHERE id = @id", conn);
                        insertLogEntry_updateType.Parameters.Add(new SqlParameter("type", type));
                        insertLogEntry_updateType.Parameters.Add(new SqlParameter("id", id));

                        insertLogEntry_updateType.ExecuteNonQuery();
                    }

                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
