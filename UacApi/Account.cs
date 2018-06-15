using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Web;
using BenLog;

namespace UacApi
{
    public class Account
    {
        private LogWriter AccountLog;

        public Account()
        {
            AccountLog = new LogWriter("Account Object.txt", AppDomain.CurrentDomain.BaseDirectory);
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

        

        private string[] GetCookies()
        {
            AccountLog.Info("Getting user details from cookies...");
            HttpCookie userName;
            HttpCookie userPwd;
            string[] UserDetails = new string[2];

            if (HttpContext.Current.Request.Cookies["userName"] != null)
            {
                userName = HttpContext.Current.Request.Cookies["userName"];
                AccountLog.Debug("Found a username cookie! Username = " + userName.Value);
                username = userName.Value;
                UserDetails[0] = userName.Value;
            }
            else
            {
                AccountLog.Debug("No username cookie was found.");
                UserDetails[0] = "";
            }

            if (HttpContext.Current.Request.Cookies["userPwd"] != null)
            {
                userPwd = HttpContext.Current.Request.Cookies["userPwd"];
                AccountLog.Debug("Found a password cookie! Password = " + userPwd.Value);
                UserDetails[1] = userPwd.Value;
            }
            else
            {
                AccountLog.Debug("No password cookie was found.");
                UserDetails[1] = "";
            }

            AccountLog.Info("Returning user details from cookies.");
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
            if (WriteSessionCookie("userPwd", pPwd))
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
            AccountLog.Info(String.Format("Attempting to write permanant {0} cookie...", pName));
            try
            {
                HttpContext.Current.Response.Cookies[pName].Value = pVal;
                HttpContext.Current.Response.Cookies[pName].Expires = DateTime.Now.AddDays(365);
                AccountLog.Info(String.Format("Permanent {0} cookie ({0} = {1}) was written successfully!", pName, pVal));
                return true;
            }
            catch
            {
                AccountLog.Error(String.Format("Failed to write permanent {0} cookie. {0} != {1}", pName, pVal));
                return false;
            }
        }

        private bool WriteSessionCookie(string pName, string pVal)
        {
            AccountLog.Info(String.Format("Attempting to write session {0} cookie...", pName));
            try
            {
                HttpContext.Current.Response.Cookies[pName].Value = pVal;
                AccountLog.Info(String.Format("Session {0} cookie ({0} = {1}) was written successfully!", pName, pVal));
                return true;
            }
            catch
            {
                AccountLog.Error(String.Format("Failed to write session {0} cookie. {0} != {1}", pName, pVal));
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


        public bool LogIn()
        {
            string[] Cookies = GetCookies();
            string CookieUsername = Cookies[0];
            username = Cookies[0];
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

        public bool LogIn(string pUsername, bool pRememberUsername, string pPwd, bool pRememberPwd)
        {
            AccountLog.Info(String.Format("[LogIn] Logging in with Username = {0} & Pwd = {1}...",pUsername, pPwd));

            string dbPwd = null;
            username = pUsername;

            using (SqlConnection conn = new SqlConnection(sqlConnectionString))
            {
                conn.Open();
                AccountLog.Debug("[LogIn] Sql connection opened succesfully!");



                if (GetPwdFromDb() && pPwd == dbPwd)
                {
                    AccountLog.Debug(String.Format("[LogIn] DbPwd == Pwd ({0} == {1})", dbPwd, pPwd));
                    if (GetUserDetails())
                    {
                        AccountLog.Debug(String.Format("[LogIn] Got user's details successfully!", dbPwd, pPwd));
                        if (pRememberUsername)
                        {
                            AccountLog.Debug(String.Format("[LogIn] RememberUsername = true. Writing permanent username cookie (userName = {0})", pUsername));
                            WritePermUserCookie(pUsername);
                        }
                        if (pRememberPwd)
                        {
                            AccountLog.Debug(String.Format("[LogIn] RememberPassword = true. Writing permanent password cookie (userPwd = {0})", pPwd));
                            WritePermCookie("userPwd", pPwd);
                        }
                        else
                        {
                            AccountLog.Debug(String.Format("[LogIn] RememberPassword = false. Writing session password cookie (userPwd = {0})", pUsername));
                            WriteSessionPwdCookie(pPwd);
                        }
                        AccountLog.Info("[LogIn] Logged in successfully!");
                        return true;
                    }
                }
                else
                {
                    AccountLog.Debug(String.Format("[LogIn] DbPwd != Pwd ({0} != {1}", dbPwd, pPwd));
                }
                AccountLog.Error("[LogIn] Failed to log in.");
                return false;
                
                bool GetPwdFromDb()
                {
                    try
                    {
                        SqlCommand GetLogInDetails = new SqlCommand("SELECT Pwd FROM t_Users WHERE username = @username", conn);
                        GetLogInDetails.Parameters.Add(new SqlParameter("username", pUsername));

                        using (SqlDataReader reader = GetLogInDetails.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dbPwd = reader[0].ToString().Trim();
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
                                    email = reader[0].ToString().Trim();
                                }
                                
                                fname = reader[1].ToString().Trim();
                                sname = reader[2].ToString().Trim();

                                if(reader[3] != DBNull.Value)
                                {
                                    dob = Convert.ToDateTime(reader[3]);
                                }
                                
                                if(reader[4] != DBNull.Value)
                                {
                                    address1 = reader[4].ToString().Trim();
                                }

                                if (reader[5] != DBNull.Value)
                                {
                                    address2 = reader[5].ToString().Trim();
                                }

                                if (reader[6] != DBNull.Value)
                                {
                                    address3 = reader[6].ToString().Trim();
                                }

                                if (reader[7] != DBNull.Value)
                                {
                                    postcode = reader[7].ToString().Trim();
                                }

                                if (reader[8] != DBNull.Value)
                                {
                                    mobilephone = reader[8].ToString().Trim();
                                }

                                if (reader[9] != DBNull.Value)
                                {
                                    homephone = reader[9].ToString().Trim();
                                }

                                if (reader[10] != DBNull.Value)
                                {
                                    workphone = reader[10].ToString().Trim();
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
