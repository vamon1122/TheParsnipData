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
            //sqlConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"M:\\Users\\benba\\My Documents\\Production\\Coding\\TheParsnipWeb\\Program\\TheParsnipWeb\\App_Data\\ParsnipDb.mdf\";Integrated Security=True";
            sqlConnectionString = "Server=198.38.83.33;Database=vamon112_parsnipdb;Uid=vamon112_ben;Password=ccjO07JT;";
        }

        private string sqlConnectionString;

        public string Email { get; set; }
        public string Username { get; set; }
        public string Fname { get; set; }
        public string Sname { get; set; }
        public DateTime Dob { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Postcode { get; set; }
        public string MobilePhone { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime LastLogIn { get; set; }
        public string AccountType { get; set; }
        public string AccountStatus { get; set; }

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
                Username = userName.Value;
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
         
        /*
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
        */

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

        /*
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
        */

        public bool LogIn()
        {
            string[] Cookies = GetCookies();
            string CookieUsername = Cookies[0];
            Username = Cookies[0];
            string CookiePwd = Cookies[1];

            System.Diagnostics.Debug.WriteLine("CookieUsername = " + CookieUsername);
            System.Diagnostics.Debug.WriteLine("CookiePwd = " + CookiePwd);

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
            Username = pUsername;

            using (SqlConnection conn = new SqlConnection(sqlConnectionString))
            {
                conn.Open();
                AccountLog.Debug("[LogIn] Sql connection opened succesfully!");



                if (GetPwdFromDb() && pPwd == dbPwd)
                {
                    AccountLog.Debug(String.Format("[LogIn] DbPwd == Pwd ({0} == {1})", dbPwd, pPwd));
                    if (GetUserDetails())
                    {
                        
                        if (pRememberUsername)
                        {
                            AccountLog.Debug(String.Format("[LogIn] RememberUsername = true. Writing permanent username cookie (userName = {0})", pUsername));
                            System.Diagnostics.Debug.WriteLine("Username permanently remembered!");
                            WritePermCookie("userName", pUsername);
                        }

                        if (pRememberPwd)
                        {
                            AccountLog.Debug(String.Format("[LogIn] RememberPassword = true. Writing permanent password cookie (userPwd = {0})", pPwd));
                            System.Diagnostics.Debug.WriteLine("Password permanently remembered!");
                            WritePermCookie("userPwd", pPwd);
                            System.Diagnostics.Debug.WriteLine("PERMANENT Password cookie = " + GetCookies()[1]); 
                        }
                        else
                        {
                            AccountLog.Debug(String.Format("[LogIn] RememberPassword = false. Writing session password cookie (userPwd = {0})", pPwd));
                            if(GetCookies()[1] == pPwd)
                            {
                                AccountLog.Debug(String.Format("[LogIn] Cookie already exists with the same value! It may have been permanently remembered! Not overwriting cookie.", pPwd));
                            }
                            else
                            {
                                AccountLog.Debug(String.Format("[LogIn] Cookie does not exist. Writing temporary password cookie.", pPwd));
                                WriteSessionCookie("userPwd", pPwd);
                                AccountLog.Debug(String.Format("[LogIn] Password stored for SESSION ONLY.", pPwd));
                                System.Diagnostics.Debug.WriteLine("Password stored for SESSION ONLY.");
                            }
                            
                            
                        }

                        if (SetLastLogIn())
                        {
                            AccountLog.Info("[LogIn] Logged in successfully!");
                            return true;
                        }
                        
                    }
                }
                else
                {
                    AccountLog.Debug(String.Format("[LogIn] DbPwd != Pwd ({0} != {1}", dbPwd, pPwd));
                }
                AccountLog.Error("[LogIn] Failed to log in.");
                return false;
                
                bool SetLastLogIn()
                {
                    int RecordsAffected;

                    AccountLog.Debug("[LogIn] Attempting to set LastLogIn...");
                    try
                    {
                        AccountLog.Debug("username = " + Username);
                        SqlCommand Command = new SqlCommand("UPDATE t_Users SET col_LastLogIn = GETDATE() WHERE col_Username = @Username;", conn);
                        Command.Parameters.Add(new SqlParameter("Username", Username));
                        RecordsAffected = Command.ExecuteNonQuery();
                        
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("[LogIn] There was an exception whilst setting the LastLogIn: " + e);
                        return false;
                    }

                    AccountLog.Debug(String.Format("[LogIn] Set LastLogIn successfully! {0} records were affected.", RecordsAffected));
                    return true;
                }

                bool GetPwdFromDb()
                {
                    AccountLog.Debug("[LogIn] Attempting to get password from database...");
                    try
                    {
                        SqlCommand GetLogInDetails = new SqlCommand("SELECT col_Pwd FROM t_Users WHERE col_Username = @Username", conn);
                        GetLogInDetails.Parameters.Add(new SqlParameter("Username", pUsername));

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
                        Console.WriteLine("[LogIn] There was an exception whilst getting the password from the database: " + e);
                        return false;
                    }
                    AccountLog.Debug("[LogIn] Got password from database successfully!");
                    return true;
                }

                bool GetUserDetails()
                {
                    AccountLog.Debug("[LogIn] Attempting to get user details...");
                    try
                    {
                        SqlCommand GetLogInDetails = new SqlCommand("SELECT col_Email, col_Fname, col_Sname, col_Dob, col_Address1, col_Address2, col_Address3, col_PostCode, col_MobilePhone, col_HomePhone, col_WorkPhone, col_DateTimeCreated, col_LastLogIn, col_AccountType, col_AccountStatus FROM t_Users WHERE col_Username = @Username", conn);
                        GetLogInDetails.Parameters.Add(new SqlParameter("Username", Username));

                        using (SqlDataReader reader = GetLogInDetails.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if(reader[0] != DBNull.Value)
                                {
                                    Email = reader[0].ToString().Trim();
                                }
                                
                                Fname = reader[1].ToString().Trim();
                                Sname = reader[2].ToString().Trim();

                                if(reader[3] != DBNull.Value)
                                {
                                    Dob = Convert.ToDateTime(reader[3]);
                                }
                                
                                if(reader[4] != DBNull.Value)
                                {
                                    Address1 = reader[4].ToString().Trim();
                                }

                                if (reader[5] != DBNull.Value)
                                {
                                    Address2 = reader[5].ToString().Trim();
                                }

                                if (reader[6] != DBNull.Value)
                                {
                                    Address3 = reader[6].ToString().Trim();
                                }

                                if (reader[7] != DBNull.Value)
                                {
                                    Postcode = reader[7].ToString().Trim();
                                }

                                if (reader[8] != DBNull.Value)
                                {
                                    MobilePhone = reader[8].ToString().Trim();
                                }

                                if (reader[9] != DBNull.Value)
                                {
                                    HomePhone = reader[9].ToString().Trim();
                                }

                                if (reader[10] != DBNull.Value)
                                {
                                    WorkPhone = reader[10].ToString().Trim();
                                }

                                DateTimeCreated = Convert.ToDateTime(reader[11]);

                                if(reader[12] != DBNull.Value)
                                {
                                    DateTimeCreated = Convert.ToDateTime(reader[12]);
                                }

                                AccountType = reader[13].ToString().Trim();

                                AccountStatus = reader[14].ToString().Trim();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("[LogIn] There was an exception whilst getting da user data: " + e);
                        return false;
                    }
                    AccountLog.Debug("[LogIn] Got user's details successfully!");
                    return true;
                }
            }
        }

        public bool LogOut()
        {
            try
            {
                WriteSessionCookie("userName", "");
                WriteSessionCookie("userPwd", "");
                return true;
            }
            catch
            {
                return false;
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
            if(Username != null && Fname != null && Sname != null)
            {
                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    conn.Open();

                    bool UsernameExists()
                    {
                        SqlCommand FindUsername = new SqlCommand("SELECT count(1) FROM t_Users WHERE username = @username");
                        FindUsername.Parameters.Add(new SqlParameter("username", Username));



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
                    InsertIntoDb.Parameters.Add(new SqlParameter("username", Username));
                    InsertIntoDb.Parameters.Add(new SqlParameter("fname", Fname));
                    InsertIntoDb.Parameters.Add(new SqlParameter("sname", Sname));

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
