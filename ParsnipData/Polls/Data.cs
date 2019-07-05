using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ParsnipData.Logs;

namespace PollApi
{
    class Data
    {
        private static string sqlConnectionString = "Server=198.38.83.33;Database=vamon112_parsnipdb;Uid=vamon112_ben;Password=ccjO07JT;";
        public static List<Poll> Polls = new List<Poll>();
        public static List<PollOption> PollOptions = new List<PollOption>();





        public static bool LoadPolls()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();
                    SqlCommand selectPolls = new SqlCommand("SELECT * FROM poll", conn);

                    using (SqlDataReader reader = selectPolls.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Poll temp = new Poll(new Guid(reader[0].ToString()))
                            {
                                createdByUserId = new Guid(reader[1].ToString()),
                                dateCreated = Convert.ToDateTime(reader[2]),
                                name = Convert.ToString(reader[3])
                            };

                            if (reader[4] != DBNull.Value) temp.description = reader[4].ToString();

                            Polls.Add(temp);
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

        internal static bool InsertPoll(Poll pPoll)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();

                    SqlCommand insertPoll = new SqlCommand("INSERT INTO poll (id, createdByUserId, dateCreated, name) VALUES(@id, @createdByUserId, @dateCreated, @name)", conn);
                    insertPoll.Parameters.Add(new SqlParameter("id", pPoll.id));
                    insertPoll.Parameters.Add(new SqlParameter("createdByUserId", pPoll.createdByUserId));
                    insertPoll.Parameters.Add(new SqlParameter("dateCreated", pPoll.dateCreated));
                    insertPoll.Parameters.Add(new SqlParameter("name", pPoll.name));

                    insertPoll.ExecuteNonQuery();

                    if (pPoll.description != null && pPoll.description != "")
                    {
                        SqlCommand insertPoll_updateDescription = new SqlCommand("UPDATE poll SET description = @description WHERE id = @id", conn);
                        insertPoll_updateDescription.Parameters.Add(new SqlParameter("description", pPoll.description));
                        insertPoll_updateDescription.Parameters.Add(new SqlParameter("id", pPoll.id));

                        insertPoll_updateDescription.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool LoadPollOptions()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();
                    SqlCommand selectPollOptions = new SqlCommand("SELECT * FROM poll_option", conn);

                    using (SqlDataReader reader = selectPollOptions.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PollOptions.Add(new PollOption(new Guid(reader[0].ToString()))
                            {
                                pollId = new Guid(reader[1].ToString()),
                                createdByUserId = new Guid(reader[2].ToString()),
                                dateCreated = Convert.ToDateTime(reader[3]),
                                value = Convert.ToString(reader[3])
                            });
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

        internal static bool InsertPollOption(PollOption pPollOption)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();

                    SqlCommand insertPollOption = new SqlCommand("INSERT INTO poll_option (id, pollId, createdByUserId, dateCreated, value) VALUES(@id, @pollId, createdByUserId, @dateCreated, @value)", conn);
                    insertPollOption.Parameters.Add(new SqlParameter("id", pPollOption.id));
                    insertPollOption.Parameters.Add(new SqlParameter("pollId", pPollOption.pollId));
                    insertPollOption.Parameters.Add(new SqlParameter("createdByUserId", pPollOption.createdByUserId));
                    insertPollOption.Parameters.Add(new SqlParameter("dateCreated", pPollOption.dateCreated));
                    insertPollOption.Parameters.Add(new SqlParameter("value", pPollOption.value));

                    insertPollOption.ExecuteNonQuery();
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
