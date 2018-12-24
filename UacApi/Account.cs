using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Web;
using BenLog;
using System.Diagnostics;
using LogApi;

namespace UacApi
{
    public class Account
    {
        private LogWriter AccountLog;
        private static string sqlConnectionString = "Server=198.38.83.33;Database=vamon112_parsnipdb;Uid=vamon112_ben;Password=ccjO07JT;";

        public Guid id { get; private set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Pwd { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public DateTime Dob { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostCode { get; set; }
        public string MobilePhone { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime LastLogIn { get; set; }
        public string AccountType { get; set; }
        public string AccountStatus { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string fullName { get { return string.Format("{0} {1}", Forename, Surname); } }


        public Account()
        {
            id = Guid.NewGuid();
            DateTimeCreated = DateTime.Now.AddHours(8);
            AccountLog = new LogWriter("Account Object.txt", AppDomain.CurrentDomain.BaseDirectory);
        }

        public Account(Guid pGuid)
        {
            id = pGuid;
        }

        public bool Validate()
        {
            bool ValidateSuccess = true;

            ValidateSuccess = (validateUsername()) ? ValidateSuccess : false;
            ValidateSuccess = (validateEmail()) ? ValidateSuccess : false;
            //ValidateSuccess = (validatePwd()) ? ValidateSuccess : false;
            ValidateSuccess = (validateForename()) ? ValidateSuccess : false;
            ValidateSuccess = (validateSurname()) ? ValidateSuccess : false;
            //ValidateSuccess = (validateDob()) ? ValidateSuccess : false; 
            ValidateSuccess = (validateAddress1()) ? ValidateSuccess : false;
            ValidateSuccess = (validateAddress2()) ? ValidateSuccess : false;
            ValidateSuccess = (validateAddress3()) ? ValidateSuccess : false;
            ValidateSuccess = (validatePostCode()) ? ValidateSuccess : false;
            ValidateSuccess = (validateMobilePhone()) ? ValidateSuccess : false;
            ValidateSuccess = (validateHomePhone()) ? ValidateSuccess : false;
            ValidateSuccess = (validateWorkPhone()) ? ValidateSuccess : false;
            //ValidateSuccess = (validateDateTimeCreated()) ? ValidateSuccess : false;
            ValidateSuccess = (validateAccountType()) ? ValidateSuccess : false;
            ValidateSuccess = (validateAccountStatus()) ? ValidateSuccess : false;

            return ValidateSuccess;

            bool validateUsername()
            {
                if (Username.Length == 0)
                {
                    return false;
                }
                else if (Username.Length > 50)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            bool validateEmail()
            {
                string EmailAddress = Email;

                if (EmailAddress.Length != 0)
                {
                    int AsperandIndex; //Index of "@" sign
                    int PointIndex; //Index of "."
                    string Username;
                    string MailServer;
                    string DomainExtension;

                    if (EmailAddress.Contains("@"))
                    {
                        int NoOfAsperand = EmailAddress.Split('@').Length - 1;
                        if (NoOfAsperand == 1)
                        {
                            AsperandIndex = EmailAddress.IndexOf("@");
                            Username = EmailAddress.Substring(0, AsperandIndex);
                            //MyLog.Debug("Email: Username = " + Username);
                            if (EmailAddress.Substring(AsperandIndex + 1, EmailAddress.Length - AsperandIndex - 1).Contains("."))
                            {
                                PointIndex = EmailAddress.LastIndexOf('.');
                                MailServer = EmailAddress.Substring(AsperandIndex + 1, PointIndex - AsperandIndex - 1);
                                //MyLog.Debug("Email: Mail server = " + MailServer);
                                DomainExtension = EmailAddress.Substring(PointIndex + 1, EmailAddress.Length - PointIndex - 1);
                                return true;
                            }
                            else
                            {
                                //MyLog.Warning("Email address domain does not contain a \".\". Email address will be blank!");
                                return false;
                            }
                        }
                        else
                        {
                            //MyLog.Warning("Email address contains too many @'s. Email address will be blank!");
                            return false;
                        }
                    }
                    else
                    {
                        //MyLog.Warning("Email address does not contain an \"@\" sign. Email address will be blank!");
                        return false;
                    }
                }
                else
                {
                    //Don't really need to be warned about blank fields.
                    //MyLog.Warning(String.Format("Email \"{0}\" is made up from blank characters! Email address will be blank!", EmailVal));
                    return false;
                }
            }

            #region bool validatePwd()
            /*
            bool validatePwd()
            {
                if (.Text.Trim().Length == 0)
                {
                    password1.CssClass = "invalid";
                    password2.CssClass = "invalid";
                    return false;
                }
                else
                {
                    return true;
                }
            }
            */
            #endregion

            bool validateForename()
            {
                return true;
            }

            bool validateSurname()
            {
                return true;
            }

            /*
            bool validateDob()
            {

            }
            */

            bool validateAddress1()
            {
                return true;
            }

            bool validateAddress2()
            {
                return true;
            }

            bool validateAddress3()
            {
                return true;
            }

            bool validatePostCode()
            {
                return true;
            }

            bool validateMobilePhone()
            {
                return true;
            }

            bool validateHomePhone()
            {
                return true;
            }

            bool validateWorkPhone()
            {
                return true;
            }

            #region bool validateDateTimeCreated()
            /*
            bool validateDateTimeCreated()
            {
                return true;
            }
            */
            #endregion

            bool validateAccountType()
            {
                return true;
            }

            bool validateAccountStatus()
            {
                return true;
            }
        }
        
        
        
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

        #region Specific Cookies (Deprecaded)
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
        #endregion

        public bool LogIn()
        {
            return LogIn(true);
        }

        public bool LogIn(bool silent)
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
                if (LogIn(CookieUsername, false, CookiePwd, false, silent))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        internal bool LogIn(string pUsername)
        {
            Username = pUsername;
            return DbSelect(new SqlConnection(sqlConnectionString));
        }

        internal bool DbSelect(SqlConnection pConn)
        {
            AccountLog.Debug("Attempting to get user details...");

            string[] DbAccountDetails = DbSelectValues(pConn);
            
            Account ValidateMe = new Account(new Guid(DbAccountDetails[0].ToString())) {
                Username = DbAccountDetails[1].ToString().Trim(),
                Email = DbAccountDetails[2].ToString().Trim(),
                Pwd = DbAccountDetails[3].ToString().Trim(),
                Forename = DbAccountDetails[4].ToString().Trim(),
                Surname = DbAccountDetails[5].ToString().Trim(),
                Dob = Convert.ToDateTime(DbAccountDetails[6]),
                Address1 = DbAccountDetails[7].ToString().Trim(),
                Address2 = DbAccountDetails[8].ToString().Trim(),
                Address3 = DbAccountDetails[9].ToString().Trim(),
                PostCode = DbAccountDetails[10].ToString().Trim(),
                MobilePhone = DbAccountDetails[11].ToString().Trim(),
                HomePhone = DbAccountDetails[12].ToString().Trim(),
                WorkPhone = DbAccountDetails[13].ToString().Trim(),
                DateTimeCreated = Convert.ToDateTime(DbAccountDetails[14]),
                LastLogIn = Convert.ToDateTime(DbAccountDetails[15]),
                AccountType = DbAccountDetails[16].ToString().Trim(),
                AccountStatus = DbAccountDetails[17].ToString().Trim()
        };

            if (ValidateMe.Validate())
            {
                id = new Guid(DbAccountDetails[0].ToString());
                Username = DbAccountDetails[1].ToString().Trim();
                Email = DbAccountDetails[2].ToString().Trim();
                //Pwd = DbAccountDetails[3].ToString().Trim();
                Forename = DbAccountDetails[4].ToString().Trim();
                Surname = DbAccountDetails[5].ToString().Trim();
                Dob = Convert.ToDateTime(DbAccountDetails[6]);
                Address1 = DbAccountDetails[7].ToString().Trim();
                Address2 = DbAccountDetails[8].ToString().Trim();
                Address3 = DbAccountDetails[9].ToString().Trim();
                PostCode = DbAccountDetails[10].ToString().Trim();
                MobilePhone = DbAccountDetails[11].ToString().Trim();
                HomePhone = DbAccountDetails[12].ToString().Trim();
                WorkPhone = DbAccountDetails[13].ToString().Trim();
                DateTimeCreated = Convert.ToDateTime(DbAccountDetails[14]);
                LastLogIn = Convert.ToDateTime(DbAccountDetails[15]);
                AccountType = DbAccountDetails[16].ToString().Trim();
                AccountStatus = DbAccountDetails[17].ToString().Trim();
            }

            
            
            AccountLog.Debug("Got user's details successfully!");

            return true;
        }

        private string[] DbSelectValues(SqlConnection pConn)
        {
            Debug.WriteLine("Attempting to get user details...");
            string[] rVals = new string[18];
            try
            {
                SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_Users WHERE Username = @username", pConn);
                SelectAccount.Parameters.Add(new SqlParameter("username", Username));

                using (SqlDataReader reader = SelectAccount.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for(int i = 0; i < 18; i++)
                        {
                            /*if(reader[i] != DBNull.Value)
                            {*/
                            Debug.WriteLine(String.Format("reader[{0}].ToString().Trim() = {1} (rVals[{0}] = {1})", i, reader[i].ToString().Trim()));
                            rVals[i] = reader[i].ToString().Trim();
                            //}
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst getting user data: " + e);
                throw e;
            }
            Debug.WriteLine("Got user's details successfully!");
            return rVals;
        }

        public bool LogIn(string pUsername, bool pRememberUsername, string pPwd, bool pRememberPwd)
        {
            return LogIn(pUsername, pRememberUsername, pPwd, pRememberPwd, true);
        }

        public bool LogIn(string pUsername, bool pRememberUsername, string pPwd, bool pRememberPwd, bool silent)
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
                    if (DbSelect())
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
                            if (!silent)
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("----------{0} logged in LOUDLY", fullName));
                                new LogEntry() { text = String.Format("{0} logged in", fullName), userId = id  }.Insert();
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("----------{0} logged in SILENTLY", fullName));
                            }
                            
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
                        SqlCommand Command = new SqlCommand("UPDATE t_Users SET LastLogIn = GETDATE() WHERE Username = @Username;", conn);
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
                        SqlCommand GetLogInDetails = new SqlCommand("SELECT Pwd FROM t_Users WHERE Username = @Username", conn);
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

                bool DbSelect()
                {
                    AccountLog.Debug("[LogIn] Attempting to get user details...");
                    try
                    {
                        SqlCommand GetLogInDetails = new SqlCommand("SELECT Email, Forename, Surname, Dob, Address1, Address2, Address3, PostCode, MobilePhone, HomePhone, WorkPhone, DateTimeCreated, LastLogIn, AccountType, AccountStatus FROM t_Users WHERE Username = @Username", conn);
                        GetLogInDetails.Parameters.Add(new SqlParameter("Username", Username));

                        using (SqlDataReader reader = GetLogInDetails.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if(reader[0] != DBNull.Value)
                                {
                                    Email = reader[0].ToString().Trim();
                                }
                                
                                Forename = reader[1].ToString().Trim();
                                Surname = reader[2].ToString().Trim();

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
                                    PostCode = reader[7].ToString().Trim();
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
            bool HasBeenInserted = true;

            if (HasBeenInserted)
            {
                try
                {

                    using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                    {
                        conn.Open();

                        string[] DbAccountDetails = DbSelectValues(conn);

                        if(DbAccountDetails == null)
                        {
                            Debug.WriteLine("No details were retrieved 1");
                        }

                        if (DbAccountDetails[0] == null)
                        {
                            Debug.WriteLine("No details were retrieved 2");
                        }

                        Debug.WriteLine(String.Format("string DbUsername: Converting x = DbAccountDetails[0] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[0]));
                        string DbUsername = DbAccountDetails[0].ToString().Trim();

                        Debug.WriteLine(String.Format("string DbEmail: Converting x = DbAccountDetails[1] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[1]));
                        string DbEmail = DbAccountDetails[1].ToString().Trim();

                        Debug.WriteLine("Skipping DbAccountDetails[2] (password)");

                        Debug.WriteLine(String.Format("string DbForename: Converting x = DbAccountDetails[3] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[3]));
                        string dbForename = DbAccountDetails[3].ToString().Trim();

                        Debug.WriteLine(String.Format("string DbSurname: Converting x = DbAccountDetails[4] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[4]));
                        string DbSurname = DbAccountDetails[4].ToString().Trim();

                        DateTime DbDob;
                        if (DbAccountDetails[5] == String.Empty || DbAccountDetails[5].ToString() == String.Empty)
                        {
                            Debug.WriteLine(String.Format("string DbDob: Converting x = DbAccountDetails[5] / x = \"{0}\" (DateTime.MinValue)", DbAccountDetails[5]));
                            DbDob = DateTime.MinValue;
                        }
                        else
                        {
                            Debug.WriteLine(String.Format("string DbDob: Converting x = DbAccountDetails[5] / x = \"{0}\" (Convert.ToDateTime(x))", DbAccountDetails[5]));
                            DbDob = Convert.ToDateTime(DbAccountDetails[5]);
                        }
                        
                        Debug.WriteLine(String.Format("string DbAddress1: Converting x = DbAccountDetails[6] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[6]));
                        string DbAddress1 = DbAccountDetails[6].ToString().Trim();

                        Debug.WriteLine(String.Format("string DbAddress2: Converting x = DbAccountDetails[7] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[7]));
                        string DbAddress2 = DbAccountDetails[7].ToString().Trim();

                        Debug.WriteLine(String.Format("string DbAddress3: Converting x = DbAccountDetails[8] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[8]));
                        string DbAddress3 = DbAccountDetails[8].ToString().Trim();

                        Debug.WriteLine(String.Format("string DbPostCode: Converting x = DbAccountDetails[9] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[9]));
                        string DbPostCode = DbAccountDetails[9].ToString().Trim();

                        Debug.WriteLine(String.Format("string DbMobilePhone: Converting x = DbAccountDetails[10] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[10]));
                        string DbMobilePhone = DbAccountDetails[10].ToString().Trim();

                        Debug.WriteLine(String.Format("string DbHomePhone: Converting x = DbAccountDetails[11] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[11]));
                        string DbHomePhone = DbAccountDetails[11].ToString().Trim();

                        Debug.WriteLine(String.Format("string DbWorkPhone: Converting x = DbAccountDetails[12] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[12]));
                        string DbWorkPhone = DbAccountDetails[12].ToString().Trim();

                        DateTime DbDateTimeCreated;
                        if (DbAccountDetails[13] == String.Empty || DbAccountDetails[13].ToString() == String.Empty)
                        {
                            Debug.WriteLine(String.Format("string DbDateTimeCreated: Converting x = DbAccountDetails[13] / x = \"{0}\" (DateTime.MinValue)", DbAccountDetails[13]));
                            DbDateTimeCreated = DateTime.MinValue;
                        }
                        else
                        {
                            Debug.WriteLine(String.Format("string DbDateTimeCreated: Converting x = DbAccountDetails[13] / x = \"{0}\" (Convert.ToDateTime(x))", DbAccountDetails[13]));
                            DbDateTimeCreated = Convert.ToDateTime(DbAccountDetails[13]);
                        }

                        DateTime DbLastLogIn;
                        if (DbAccountDetails[14] == String.Empty || DbAccountDetails[14].ToString() == String.Empty)
                        {
                            Debug.WriteLine(String.Format("string DbLastLogIn: Converting x = DbAccountDetails[14] / x = \"{0}\" (DateTime.MinValue)", DbAccountDetails[14]));
                            DbLastLogIn = DateTime.MinValue;
                        }
                        else
                        {
                            Debug.WriteLine(String.Format("string DbLastLogIn: Converting x = DbAccountDetails[14] / x = \"{0}\" (Convert.ToDateTime(x))", DbAccountDetails[14]));
                            DbLastLogIn = Convert.ToDateTime(DbAccountDetails[14]);
                        }

                        Debug.WriteLine(String.Format("string DbAccountType: Converting x = DbAccountDetails[15] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[15]));
                        string DbAccountType = DbAccountDetails[15].ToString().Trim();

                        Debug.WriteLine(String.Format("string DbAccountStatus: Converting x = DbAccountDetails[16] / x = \"{0}\" (x.ToString()Trim()).", DbAccountDetails[16]));
                        string DbAccountStatus = DbAccountDetails[16].ToString().Trim();
                        
                        if(Username != DbUsername)
                        {
                            Debug.WriteLine("Updating username...");

                            SqlCommand UpdateUsername = new SqlCommand("UPDATE t_Users SET Username = @username WHERE Username = @username", conn);

                            UpdateUsername.Parameters.Add(new SqlParameter("username", Username));

                            UpdateUsername.ExecuteNonQuery();

                            Debug.WriteLine("Username updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Username was not changed. Not updating username.");
                        }
                        
                        if(Email != DbEmail)
                        {
                            Debug.WriteLine("Updating email...");

                            SqlCommand UpdateEmail = new SqlCommand("UPDATE t_Users SET Email = @email WHERE Username = @username", conn);

                            UpdateEmail.Parameters.Add(new SqlParameter("username", Username));
                            UpdateEmail.Parameters.Add(new SqlParameter("email", Email));

                            UpdateEmail.ExecuteNonQuery();

                            Debug.WriteLine("Email updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Email was not changed. Not updating email.");
                        }

                        if (Forename != dbForename)
                        {
                            Debug.WriteLine("Updating forename...");

                            SqlCommand UpdateForename = new SqlCommand("UPDATE t_Users SET Forename = @forename WHERE Username = @username", conn);

                            UpdateForename.Parameters.Add(new SqlParameter("username", Username));
                            UpdateForename.Parameters.Add(new SqlParameter("forename", Email));

                            UpdateForename.ExecuteNonQuery();

                            Debug.WriteLine("Forename updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Forename was not changed. Not updating forename.");
                        }

                        if (Surname != DbSurname)
                        {
                            Debug.WriteLine("Updating surname...");

                            SqlCommand UpdateSurname = new SqlCommand("UPDATE t_Users SET Surname = @surname WHERE Username = @username", conn);

                            UpdateSurname.Parameters.Add(new SqlParameter("username", Username));
                            UpdateSurname.Parameters.Add(new SqlParameter("surname", Surname));

                            UpdateSurname.ExecuteNonQuery();

                            Debug.WriteLine("Surname updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Surname was not changed. Not updating surname.");
                        }

                        if (Dob != DbDob && Dob != DateTime.MinValue)
                        {
                            Debug.WriteLine("Updating dob...");

                            SqlCommand UpdateDob = new SqlCommand("UPDATE t_Users SET Dob = @dob WHERE Username = @username", conn);

                            UpdateDob.Parameters.Add(new SqlParameter("username", Username));
                            UpdateDob.Parameters.Add(new SqlParameter("dob", Dob));

                            UpdateDob.ExecuteNonQuery();

                            Debug.WriteLine("Dob updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Dob was not changed. Not updating dob.");
                        }

                        if (Address1 != DbAddress1)
                        {
                            Debug.WriteLine("Updating address1...");

                            SqlCommand UpdateAddress1 = new SqlCommand("UPDATE t_Users SET Address1 = @address1 WHERE Username = @username", conn);

                            UpdateAddress1.Parameters.Add(new SqlParameter("username", Username));
                            UpdateAddress1.Parameters.Add(new SqlParameter("address1", Address1));

                            UpdateAddress1.ExecuteNonQuery();

                            Debug.WriteLine("Address1 updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Address1 was not changed. Not updating address1.");
                        }

                        if (Address2 != DbAddress2)
                        {
                            Debug.WriteLine("Updating address2...");

                            SqlCommand UpdateAddress2 = new SqlCommand("UPDATE t_Users SET Address2 = @address2 WHERE Username = @username", conn);

                            UpdateAddress2.Parameters.Add(new SqlParameter("username", Username));
                            UpdateAddress2.Parameters.Add(new SqlParameter("address2", Address2));

                            UpdateAddress2.ExecuteNonQuery();

                            Debug.WriteLine("Address2 updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Address2 was not changed. Not updating address2.");
                        }

                        if (Address3 != DbAddress3)
                        {
                            Debug.WriteLine("Updating address3...");

                            SqlCommand UpdateAddress3 = new SqlCommand("UPDATE t_Users SET Address3 = @address3 WHERE Username = @username", conn);

                            UpdateAddress3.Parameters.Add(new SqlParameter("username", Username));
                            UpdateAddress3.Parameters.Add(new SqlParameter("address3", Address3));

                            UpdateAddress3.ExecuteNonQuery();

                            Debug.WriteLine("Address3 updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Address3 was not changed. Not updating address3.");
                        }

                        if (PostCode != DbPostCode)
                        {
                            Debug.WriteLine("Updating postcode...");

                            SqlCommand UpdatePostCode = new SqlCommand("UPDATE t_Users SET PostCode = @postCode WHERE Username = @username", conn);

                            UpdatePostCode.Parameters.Add(new SqlParameter("username", Username));
                            UpdatePostCode.Parameters.Add(new SqlParameter("postCode", PostCode));

                            UpdatePostCode.ExecuteNonQuery();

                            Debug.WriteLine("Postcode updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Postcode was not changed. Not updating postcode.");
                        }

                        if (MobilePhone != DbMobilePhone)
                        {
                            Debug.WriteLine("Updating mobile phone...");

                            SqlCommand UpdateMobilePhone = new SqlCommand("UPDATE t_Users SET MobilePhone = @mobilePhone WHERE Username = @username", conn);

                            UpdateMobilePhone.Parameters.Add(new SqlParameter("username", Username));
                            UpdateMobilePhone.Parameters.Add(new SqlParameter("mobilePhone", MobilePhone));

                            UpdateMobilePhone.ExecuteNonQuery();

                            Debug.WriteLine("Mobile phone updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Mobile phone was not changed. Not updating mobile phone.");
                        }

                        if (HomePhone != DbHomePhone)
                        {
                            Debug.WriteLine("Updating home phone...");

                            SqlCommand UpdateHomePhone = new SqlCommand("UPDATE t_Users SET HomePhone = @homePhone WHERE Username = @username", conn);

                            UpdateHomePhone.Parameters.Add(new SqlParameter("username", Username));
                            UpdateHomePhone.Parameters.Add(new SqlParameter("homePhone", HomePhone));

                            UpdateHomePhone.ExecuteNonQuery();

                            Debug.WriteLine("Home phone updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Home phone was not changed. Not updating home phone.");
                        }

                        if (WorkPhone != DbWorkPhone)
                        {
                            Debug.WriteLine("Updating work phone...");

                            SqlCommand UpdateWorkPhone = new SqlCommand("UPDATE t_Users SET WorkPhone = @workPhone WHERE Username = @username", conn);

                            UpdateWorkPhone.Parameters.Add(new SqlParameter("username", Username));
                            UpdateWorkPhone.Parameters.Add(new SqlParameter("workPhone", WorkPhone));

                            UpdateWorkPhone.ExecuteNonQuery();

                            Debug.WriteLine("Work phone updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Work phone was not changed. Not updating work phone.");
                        }

                        if (AccountType != DbAccountType)
                        {
                            Debug.WriteLine("Updating account type...");

                            SqlCommand UpdateAccountType = new SqlCommand("UPDATE t_Users SET AccountType = @accountType WHERE Username = @username", conn);

                            UpdateAccountType.Parameters.Add(new SqlParameter("username", Username));
                            UpdateAccountType.Parameters.Add(new SqlParameter("accountType", AccountType));

                            UpdateAccountType.ExecuteNonQuery();

                            Debug.WriteLine("Account type updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Account type was not changed. Not account type.");
                        }

                        if (AccountStatus != DbAccountStatus)
                        {
                            Debug.WriteLine("Updating account status...");

                            SqlCommand UpdateAccountStatus = new SqlCommand("UPDATE t_Users SET AccountStatus = @accountStatus WHERE Username = @username", conn);

                            UpdateAccountStatus.Parameters.Add(new SqlParameter("username", Username));
                            UpdateAccountStatus.Parameters.Add(new SqlParameter("accountStatus", AccountStatus));

                            UpdateAccountStatus.ExecuteNonQuery();

                            Debug.WriteLine("Account status updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Account status was not changed. Not updating account status.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Caught an error whilst updating account (\"{0}\": {1}",Username, e);
                    return false;
                }
                return true;
            }
            else
            {
                throw new System.InvalidOperationException("Account cannot be updated. Account must be inserted into the database before it can be updated!");
            }
        }

        public bool DbInsert(string pPwd)
        {
            if (Username != null && Forename != null && Surname != null)
            {
                try
                {


                    using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                    {
                        conn.Open();

                        bool UsernameExists()
                        {
                            Debug.WriteLine(String.Format("Checking that username \"{0}\" does not exist in database before attempting insert...", Username.Trim()));

                            SqlCommand FindUsername = new SqlCommand("SELECT count(1) FROM t_Users WHERE Username = @username", conn);
                            FindUsername.Parameters.Add(new SqlParameter("username", Username));



                            using (SqlDataReader reader = FindUsername.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader[0].ToString() == "1")
                                    {
                                        Debug.WriteLine(String.Format("Username \"{0}\" already exists in the database! Returning true.", Username.Trim()));
                                        return true;
                                    }
                                    else
                                    {
                                        Debug.WriteLine(String.Format("Username \"{0}\" does not already exist in the database! Returning false.", Username.Trim()));
                                        return false;
                                    }
                                }
                            }
                            throw new Exception("Expression evaluated to neither true or false");
                        }

                        if (!UsernameExists())
                        {
                            SqlCommand InsertIntoDb = new SqlCommand("INSERT INTO t_Users (Username, Forename, Surname, Pwd, DateTimeCreated) VALUES(@username, @fname, @sname, @pwd, getdate())", conn);
                            InsertIntoDb.Parameters.Add(new SqlParameter("username", Username.Trim()));
                            InsertIntoDb.Parameters.Add(new SqlParameter("fname", Forename.Trim()));
                            InsertIntoDb.Parameters.Add(new SqlParameter("sname", Surname.Trim()));
                            InsertIntoDb.Parameters.Add(new SqlParameter("pwd", pPwd.Trim()));

                            InsertIntoDb.ExecuteNonQuery();

                            Debug.WriteLine(String.Format("Successfully inserted account \"{0}\" into database: ", Username));
                        }
                        
                    }
                }
                catch(Exception e)
                {
                    Debug.WriteLine("Failed to insert account into database: " + e);
                    return false;
                }

                return DbUpdate();
            }
            else
            {
                throw new InvalidOperationException("Account cannot be inserted. The account's properties: username, fname & sname, must be initialised before it can be inserted!");
            }
        }
    }
}
