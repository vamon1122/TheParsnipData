using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Web;

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

        public bool LogIn()
        {
            string[] Cookies = GetCookies();
            string CookieUsername = Cookies[0];
            string CookiePwd = Cookies[1];

            if (String.IsNullOrEmpty(CookieUsername) || String.IsNullOrWhiteSpace(CookieUsername) || String.IsNullOrEmpty(CookiePwd) || String.IsNullOrWhiteSpace(CookiePwd))
            {
                return false;
            }
            else
            {
                if (LogIn(CookieUsername, false, CookiePwd, false))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        private string[] GetCookies()
        {
            HttpCookie userName;
            HttpCookie userPwd;
            string[] UserDetails = new string[2];

            if (HttpContext.Current.Request.Cookies["userName"] != null)
            {
                userName = HttpContext.Current.Request.Cookies["userName"];
                UserDetails[0] = userName.Value;
            }
            else
            {
                UserDetails[0] = "";
            }

            if (HttpContext.Current.Request.Cookies["userPwd"] != null)
            {
                userPwd = HttpContext.Current.Request.Cookies["userPwd"];
                UserDetails[1] = userPwd.Value;
            }
            else
            {
                UserDetails[1] = "";
            }

            return UserDetails;
        }
         
        private bool WritePermUserCookie(string pUsername)
        {
            if(WritePermCookie("userName", pUsername))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool WriteSessionPwdCookie(string pPwd)
        {
            if(WriteSessionCookie("userPwd", pPwd))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool WritePermCookie(string pName, string pVal)
        {
            try
            {
                HttpContext.Current.Response.Cookies[pName].Value = pVal;
                HttpContext.Current.Response.Cookies[pName].Expires = DateTime.Now.AddDays(365);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool WriteSessionCookie(string pName, string pVal)
        {
            try
            {
                HttpContext.Current.Response.Cookies[pName].Value = pVal;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool WritePermUserTempPwdCookies(string pUsername, string pPwd)
        {
            if(WritePermUserCookie(pUsername) && WriteSessionPwdCookie(pPwd))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool WritePermCookies(string pUsername, string pPwd)
        {
            if(WritePermUserCookie(pUsername) &&
            WritePermCookie("userPwd", pPwd))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool LogIn(string pUsername, bool pRememberUsername, string pPwd, bool pRememberPwd)
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
