using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace UacApi
{
    public class Account
    {
        public Account()
        {
            sqlConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"M:\\Users\\benba\\My Documents\\Production\\Coding\\TheParsnipWeb\\Program\\TheParsnipWeb\\App_Data\\ParsnipDb.mdf\";Integrated Security=True";
        }

        private string sqlConnectionString;

        public string email { get; set; }
        public string username { get; set; }
        public string fname { get; set; }
        public string sname { get; set; }
        public DateTime dob { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string postcode { get; set; }
        public string mobilephone { get; set; }
        public string homephone { get; set; }
        public string workphone { get; set; }

        private bool HasBeenInserted;

        public bool LogIn(string pUsername, string pPwd)
        {
            string dbPwd = null;
            username = pUsername;

            using (SqlConnection conn = new SqlConnection(sqlConnectionString))
            {
                conn.Open();

                if (GetPwdFromDb() && pPwd == dbPwd)
                { 
                    if (GetUserDetails())
                    {
                        return true;
                    }
                }
                return false;
                
                bool GetPwdFromDb()
                {
                    try
                    {
                        SqlCommand GetLogInDetails = new SqlCommand("SELECT Pwd FROM t_Users WHERE username = @username", conn);
                        GetLogInDetails.Parameters.Add(new SqlParameter("username", username));

                        using (SqlDataReader reader = GetLogInDetails.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dbPwd = reader[0].ToString();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("There was an exception whilst getting da password: " + e);
                        return false;
                    }
                    return true;
                }

                bool GetUserDetails()
                {
                    try
                    {
                        SqlCommand GetLogInDetails = new SqlCommand("SELECT email, fname, sname, dob, address1, address2, address3, postcode, mobilephone, homephone, workphone FROM t_Users WHERE username = @username", conn);
                        GetLogInDetails.Parameters.Add(new SqlParameter("username", username));

                        using (SqlDataReader reader = GetLogInDetails.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if(reader[0] != DBNull.Value)
                                {
                                    email = reader[0].ToString();
                                }
                                
                                fname = reader[1].ToString();
                                sname = reader[2].ToString();

                                if(reader[3] != DBNull.Value)
                                {
                                    dob = Convert.ToDateTime(reader[3]);
                                }
                                
                                if(reader[4] != DBNull.Value)
                                {
                                    address1 = reader[4].ToString();
                                }

                                if (reader[5] != DBNull.Value)
                                {
                                    address2 = reader[5].ToString();
                                }

                                if (reader[6] != DBNull.Value)
                                {
                                    address3 = reader[6].ToString();
                                }

                                if (reader[7] != DBNull.Value)
                                {
                                    postcode = reader[7].ToString();
                                }

                                if (reader[8] != DBNull.Value)
                                {
                                    mobilephone = reader[8].ToString();
                                }

                                if (reader[9] != DBNull.Value)
                                {
                                    homephone = reader[9].ToString();
                                }

                                if (reader[10] != DBNull.Value)
                                {
                                    workphone = reader[10].ToString();
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("There was an exception whilst getting da user data: " + e);
                        return false;
                    }
                    return true;
                }
            }
        }

        public bool DbUpdate()
        {
            if (HasBeenInserted)
            {
                return true;
            }
            else
            {
                throw new System.InvalidOperationException("Account cannot be updated. Account must be inserted into the database before it can be updated!");
            }
        }

        public bool DbInsert()
        {
            if(username != null && fname != null && sname != null)
            {
                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();

                    bool UsernameExists()
                    {
                        SqlCommand FindUsername = new SqlCommand("SELECT count(1) FROM t_Users WHERE username = @username");
                        FindUsername.Parameters.Add(new SqlParameter("username", username));



                        using(SqlDataReader reader = FindUsername.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if(reader[0].ToString() == "1")
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        throw new Exception("Some exception");
                    }

                    SqlCommand InsertIntoDb = new SqlCommand("INSERT INTO t_Users (username, fname, sname) VALUES(@username, @fname, @sname)", conn);
                    InsertIntoDb.Parameters.Add(new SqlParameter("username", username));
                    InsertIntoDb.Parameters.Add(new SqlParameter("fname", fname));
                    InsertIntoDb.Parameters.Add(new SqlParameter("sname", sname));

                    InsertIntoDb.ExecuteNonQuery();
                }
                    return true;
            }
            else
            {
                throw new InvalidOperationException("Account cannot be inserted. The account's properties: username, fname & sname, must be initialised before it can be inserted!");
            }
        }
    }
}
