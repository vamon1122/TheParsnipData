using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ParsnipData;
using ParsnipData.Logs;

namespace ParsnipData.AnonymousApi
{
    public static class Data
    {
        
        public static List<Post> posts { get; set; }

        public static bool LoadPosts()
        {
            try
            {
                posts.Clear();
                using (SqlConnection openConn = Parsnip.GetOpenDbConnection())
                {
                    SqlCommand selectLogEntries = new SqlCommand("SELECT * FROM t_Posts", openConn);

                    using (SqlDataReader reader = selectLogEntries.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Post temp = new Post(new Guid(reader[0].ToString()))
                            {
                                date = Convert.ToDateTime(reader[2].ToString()),
                                text = reader[4].ToString()
                            };

                            if (reader[1] != DBNull.Value) temp.userId = new Guid(reader[1].ToString());
                            if (reader[3] != DBNull.Value) temp.type = reader[3].ToString();

                            posts.Add(temp);
                        }


                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        internal static bool InsertLogEntry(LogEntry pLogEntry)
        {
            try
            {
                using (SqlConnection openConn = Parsnip.GetOpenDbConnection())
                {
                
                    SqlCommand insertLogEntry = new SqlCommand("INSERT INTO t_LogEntries (id, datetime, value) VALUES(@id, @datetime, @value)", openConn);
                    insertLogEntry.Parameters.Add(new SqlParameter("id", pLogEntry.id));
                    insertLogEntry.Parameters.Add(new SqlParameter("datetime", pLogEntry.date));
                    insertLogEntry.Parameters.Add(new SqlParameter("value", pLogEntry.text));

                    insertLogEntry.ExecuteNonQuery();

                    if (pLogEntry.userId != null && pLogEntry.userId != Guid.Empty)
                    {
                        SqlCommand insertLogEntry_updateUserId = new SqlCommand("UPDATE t_LogEntries SET userId = @userId WHERE id = @id", openConn);
                        insertLogEntry_updateUserId.Parameters.Add(new SqlParameter("userId", pLogEntry.userId));
                        insertLogEntry_updateUserId.Parameters.Add(new SqlParameter("id", pLogEntry.id));

                        insertLogEntry_updateUserId.ExecuteNonQuery();
                    }

                    if (pLogEntry.type != null)
                    {
                        SqlCommand insertLogEntry_updateType = new SqlCommand("UPDATE t_LogEntries SET type = @type WHERE id = @id", openConn);
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
