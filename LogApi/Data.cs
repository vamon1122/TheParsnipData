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
        private static string sqlConnectionString = ParsnipApi.Data.sqlConnectionString;
        public static List<LogEntry> LogEntries = new List<LogEntry>();

        



        public static bool LoadLogEntries()
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();
                    SqlCommand selectLogEntries = new SqlCommand("SELECT * FROM t_LogEntries", conn);

                    using(SqlDataReader reader = selectLogEntries.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LogEntry temp = new LogEntry(new Guid(reader[0].ToString()))
                            {
                                date = Convert.ToDateTime(reader[2].ToString()),
                                text = reader[4].ToString()
                            };

                            if (reader[1] != DBNull.Value) temp.userId = new Guid(reader[1].ToString());
                            if (reader[3] != DBNull.Value) temp.type = reader[3].ToString();

                            LogEntries.Add(temp);
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

        internal static bool InsertLogEntry(LogEntry pLogEntry)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();

                    SqlCommand insertLogEntry = new SqlCommand("INSERT INTO t_LogEntries (id, datetime, value) VALUES(@id, @datetime, @value)", conn);
                    insertLogEntry.Parameters.Add(new SqlParameter("id", pLogEntry.id));
                    insertLogEntry.Parameters.Add(new SqlParameter("datetime", pLogEntry.date));
                    insertLogEntry.Parameters.Add(new SqlParameter("value", pLogEntry.text));

                    insertLogEntry.ExecuteNonQuery();

                    if(pLogEntry.userId != null && pLogEntry.userId != Guid.Empty)
                    {
                        SqlCommand insertLogEntry_updateUserId = new SqlCommand("UPDATE t_LogEntries SET userId = @userId WHERE id = @id", conn);
                        insertLogEntry_updateUserId.Parameters.Add(new SqlParameter("userId", pLogEntry.userId));
                        insertLogEntry_updateUserId.Parameters.Add(new SqlParameter("id", pLogEntry.id));

                        insertLogEntry_updateUserId.ExecuteNonQuery();
                    }

                    if (pLogEntry.type != null)
                    {
                        SqlCommand insertLogEntry_updateType = new SqlCommand("UPDATE t_LogEntries SET type = @type WHERE id = @id", conn);
                        insertLogEntry_updateType.Parameters.Add(new SqlParameter("type", pLogEntry.type));
                        insertLogEntry_updateType.Parameters.Add(new SqlParameter("id", pLogEntry.id));

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
