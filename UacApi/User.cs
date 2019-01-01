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
using CookieApi;
using ParsnipApi;

namespace UacApi
{
    public class User
    {
        private LogWriter AccountLog;
        private static string sqlConnectionString = ParsnipApi.Data.sqlConnectionString;

        public Guid id { get; private set; }
        private string _username;
        public string Username { get { return _username; } set { _username = value; System.Diagnostics.Debug.WriteLine(string.Format("Username was changed to {0}", _username));  } }
        public string Email { get; set; }
        public string Pwd { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public DateTime Dob { get; set; }
        private string _gender;
        public string Gender
        {
            get
            {
                switch (_gender)
                {
                    case "M":
                        return "Male";
                    case "F":
                        return "Female";
                    case "O":
                        return "Other";
                    default:
                        return "Error";
                }
            }
            set
            {
                gender = value;
            }
        }
        public string gender
        {
            get
            {
                switch (_gender) {
                    case "M":
                        return "male";
                    case "F":
                        return "female";
                    case "O":
                        return "other";
                    default:
                        return "error";
                }
            }
            set
            {
                string tempGender = value.Substring(0, 1).ToUpper();
                if (tempGender == "M" || tempGender == "F" || tempGender == "O")
                    _gender = tempGender;
                else
                    throw new InvalidCastException("Could not convert gender!"); } }
        public string posessivePronoun
        {
            get
            {
                if(_gender == "M")
                    return "his";
                
                else if (_gender == "F")
                    return "her";
                else
                    return "their";
            }
        }
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


        public User()
        {
            id = Guid.NewGuid();
            DateTimeCreated = ParsnipApi.Data.adjustedTime;
            AccountLog = new LogWriter("Account Object.txt", AppDomain.CurrentDomain.BaseDirectory);
        }

        public User(Guid pGuid)
        {
            id = pGuid;
        }

        public bool Validate()
        {
            bool validateSuccess = true;

            validateSuccess = validateUsername() ? validateSuccess : false;
            validateSuccess = validateEmail() ? validateSuccess : false;
            //ValidateSuccess = validatePwd() ? ValidateSuccess : false;
            validateSuccess = validateForename() ? validateSuccess : false;
            validateSuccess = validateSurname() ? validateSuccess : false;
            validateSuccess = validateDob() ? validateSuccess : false;
            validateSuccess = validateGender() ? validateSuccess : false;
            validateSuccess = validateAddress1() ? validateSuccess : false;
            validateSuccess = validateAddress2() ? validateSuccess : false;
            validateSuccess = validateAddress3() ? validateSuccess : false;
            validateSuccess = validatePostCode() ? validateSuccess : false;
            validateSuccess = validateMobilePhone() ? validateSuccess : false;
            validateSuccess = validateHomePhone() ? validateSuccess : false;
            validateSuccess = validateWorkPhone() ? validateSuccess : false;
            validateSuccess = validateDateTimeCreated() ? validateSuccess : false;
            validateSuccess = validateAccountType() ? validateSuccess : false;
            validateSuccess = validateAccountStatus() ? validateSuccess : false;

            new LogEntry(id) { text = "Validate success: " + validateSuccess };

            return validateSuccess;

            bool validateUsername()
            {
                if (Username.Length == 0)
                {
                    new LogEntry(id) { text = "Cannot create a user without a username! Username: " + Username };
                    return false;
                }
                else if (Username.Length > 50)
                {
                    new LogEntry(id) { text = String.Format("Username is {0} characters long. Username must be no longer than 50 characters!", Username.Length) };
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
                                new LogEntry(id) { text = String.Format("Email address \"{0}\" does not contain a dot. Email addresses must contain a dot.", EmailAddress) };
                                return false;
                            }
                        }
                        else
                        {
                            //MyLog.Warning("Email address contains too many @'s. Email address will be blank!");
                            new LogEntry(id) { text = String.Format("Email address \"{0}\" contains too many '@' signs. Email addresses must contain only one '@' sign.", EmailAddress) };
                            return false;
                        }
                    }
                    else
                    {
                        new LogEntry(id) { text = String.Format("Email address \"{0}\" does not contain an '@' sign. Email addresses must contain an '@' sign.", EmailAddress) };
                        //MyLog.Warning("Email address does not contain an \"@\" sign. Email address will be blank!");
                        return false;
                    }
                }
                else
                {
                    //Don't really need to be warned about blank fields.
                    //MyLog.Warning(String.Format("Email \"{0}\" is made up from blank characters! Email address will be blank!", EmailVal));
                    return true;
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
                if (Forename.Length > 0)
                    return true;
                else
                    return false;
            }

            bool validateSurname()
            {
                if (Surname.Length > 0)
                    return true;
                else
                    return false;
            }

            
            bool validateDob()
            {
                return true;
            }

            bool validateGender()
            {
                if(_gender != null)
                {
                    string tempGender = _gender.ToString().ToUpper();
                    if (tempGender == "M" || tempGender == "F" || tempGender == "O") return true; else { new LogEntry(id) { text = String.Format("Gender \"{0}\" is not M, F or O. Gender must be M, F or O.", tempGender) }; return false; };
                }
                else
                {
                    return true;
                }
                
            }
            

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

            
            bool validateDateTimeCreated()
            {
                return true;
            }

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
            

            string[] UserDetails = new string[2];

            if (Cookie.Read("userName") != null)
            {
                Username = Cookie.Read("userName");
                AccountLog.Debug("Found a username cookie! Username = " + Username);
                UserDetails[0] = Username;
            }
            else
            {
                AccountLog.Debug("No username cookie was found.");
                UserDetails[0] = "";
            }

            if (Cookie.Read("userPwd") != null)
            {
                UserDetails[1] = Cookie.Read("userPwd");
                AccountLog.Debug("Found a password cookie! Password = " + UserDetails[1]);
                
            }
            else
            {
                AccountLog.Debug("No password cookie was found.");
                UserDetails[1] = "";
            }

            AccountLog.Info("Returning user details from cookies.");
            return UserDetails;
        }

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
            
            User ValidateMe = new User(new Guid(DbAccountDetails[0].ToString())) {
                Username = DbAccountDetails[1].ToString().Trim(),
                Email = DbAccountDetails[2].ToString().Trim(),
                Pwd = DbAccountDetails[3].ToString().Trim(),
                Forename = DbAccountDetails[4].ToString().Trim(),
                Surname = DbAccountDetails[5].ToString().Trim(),
                Dob = Convert.ToDateTime(DbAccountDetails[6]),
                gender = DbAccountDetails[7].ToString(),
                Address1 = DbAccountDetails[8].ToString().Trim(),
                Address2 = DbAccountDetails[9].ToString().Trim(),
                Address3 = DbAccountDetails[10].ToString().Trim(),
                PostCode = DbAccountDetails[11].ToString().Trim(),
                MobilePhone = DbAccountDetails[12].ToString().Trim(),
                HomePhone = DbAccountDetails[13].ToString().Trim(),
                WorkPhone = DbAccountDetails[14].ToString().Trim(),
                DateTimeCreated = Convert.ToDateTime(DbAccountDetails[15]),
                LastLogIn = Convert.ToDateTime(DbAccountDetails[16]),
                AccountType = DbAccountDetails[17].ToString().Trim(),
                AccountStatus = DbAccountDetails[18].ToString().Trim()
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
                gender = DbAccountDetails[6].ToString();
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
            string[] rVals = new string[19];
            try
            {
                SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_Users WHERE Username = @username", pConn);
                SelectAccount.Parameters.Add(new SqlParameter("username", Username));

                using (SqlDataReader reader = SelectAccount.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for(int i = 0; i < 19; i++)
                        {
                            Debug.WriteLine("----------1-{0}", i);
                            /*if(reader[i] != DBNull.Value)
                            {*/
                            Debug.WriteLine(String.Format("reader[{0}].ToString().Trim() = {1} (rVals[{0}] = {1})", i, reader[i].ToString().Trim()));
                            rVals[i] = reader[i].ToString().Trim();
                            Debug.WriteLine("----------2-{0}", i);
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
                            Cookie.WritePerm("userName", pUsername);
                        }

                        if (pRememberPwd)
                        {
                            AccountLog.Debug(String.Format("[LogIn] RememberPassword = true. Writing permanent password cookie (userPwd = {0})", pPwd));
                            System.Diagnostics.Debug.WriteLine("Password permanently remembered!");
                            Cookie.WritePerm("userPwd", pPwd);
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
                                Cookie.WriteSession("userPwd", pPwd);
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
                                new LogEntry(id) { text = String.Format("{0} logged in", fullName)  };
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
                        Debug.WriteLine("[LogIn] There was an exception whilst setting the LastLogIn: " + e);
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
                        Debug.WriteLine("[LogIn] There was an exception whilst getting the password from the database: " + e);
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
                        SqlCommand GetLogInDetails = new SqlCommand("SELECT * FROM t_Users WHERE Username = @Username", conn);
                        GetLogInDetails.Parameters.Add(new SqlParameter("Username", Username));

                        using (SqlDataReader reader = GetLogInDetails.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                id = new Guid(reader[0].ToString());
                                Username = reader[1].ToString();

                                if (reader[2] != DBNull.Value)
                                {
                                    Email = reader[2].ToString().Trim();
                                }

                                Pwd = reader[3].ToString();
                                
                                Forename = reader[4].ToString().Trim();
                                Surname = reader[5].ToString().Trim();

                                if(reader[6] != DBNull.Value)
                                {
                                    Dob = Convert.ToDateTime(reader[6]);
                                }

                                if (reader[7] != DBNull.Value)
                                {
                                    gender = reader[7].ToString();
                                }

                                if (reader[8] != DBNull.Value)
                                {
                                    Address1 = reader[8].ToString().Trim();
                                }

                                if (reader[9] != DBNull.Value)
                                {
                                    Address2 = reader[9].ToString().Trim();
                                }

                                if (reader[10] != DBNull.Value)
                                {
                                    Address3 = reader[10].ToString().Trim();
                                }

                                if (reader[11] != DBNull.Value)
                                {
                                    PostCode = reader[11].ToString().Trim();
                                }

                                if (reader[12] != DBNull.Value)
                                {
                                    MobilePhone = reader[12].ToString().Trim();
                                }

                                if (reader[13] != DBNull.Value)
                                {
                                    HomePhone = reader[13].ToString().Trim();
                                }

                                if (reader[14] != DBNull.Value)
                                {
                                    WorkPhone = reader[14].ToString().Trim();
                                }

                                DateTimeCreated = Convert.ToDateTime(reader[15]);

                                if(reader[16] != DBNull.Value)
                                {
                                    LastLogIn = Convert.ToDateTime(reader[16]);
                                }

                                AccountType = reader[17].ToString().Trim();

                                AccountStatus = reader[18].ToString().Trim();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("[LogIn] There was an exception whilst getting da user data: " + e);
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
                Cookie.WriteSession("userName", "");
                Cookie.WriteSession("userPwd", "");
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

                        string[] dbAccountDetails = DbSelectValues(conn);

                        if(dbAccountDetails == null)
                        {
                            Debug.WriteLine("No details were retrieved 1");
                        }

                        if (dbAccountDetails[0] == null)
                        {
                            Debug.WriteLine("No details were retrieved 2");
                        }

                        Debug.WriteLine(String.Format("string dbId: Converting x = DbAccountDetails[0] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[0]));
                        string dbId = dbAccountDetails[0].ToString().Trim();

                        Debug.WriteLine(String.Format("string dbUsername: Converting x = DbAccountDetails[1] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[1]));
                        string dbUsername = dbAccountDetails[1].ToString().Trim();

                        Debug.WriteLine(String.Format("string dbEmail: Converting x = DbAccountDetails[2] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[2]));
                        string dbEmail = dbAccountDetails[2].ToString().Trim();

                        Debug.WriteLine(String.Format("string dbPwd: Converting x = DbAccountDetails[3] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[3]));
                        string dbPwd = dbAccountDetails[3].ToString().Trim();

                        Debug.WriteLine(String.Format("string dbForename: Converting x = DbAccountDetails[4] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[4]));
                        string dbForename = dbAccountDetails[4].ToString().Trim();

                        Debug.WriteLine(String.Format("string DbSurname: Converting x = DbAccountDetails[5] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[5]));
                        string dbSurname = dbAccountDetails[5].ToString().Trim();

                        DateTime dbDob;
                        if (dbAccountDetails[6] == String.Empty || dbAccountDetails[6].ToString() == String.Empty)
                        {
                            Debug.WriteLine(String.Format("string dbDob: Converting x = DbAccountDetails[6] / x = \"{0}\" (DateTime.MinValue)", dbAccountDetails[6]));
                            dbDob = DateTime.MinValue;
                        }
                        else
                        {
                            Debug.WriteLine(String.Format("string dbDob: Converting x = DbAccountDetails[6] / x = \"{0}\" (Convert.ToDateTime(x))", dbAccountDetails[6]));
                            dbDob = Convert.ToDateTime(dbAccountDetails[6]);
                        }

                        string dbGender = "";
                        if (dbAccountDetails[7] != String.Empty || dbAccountDetails[7].ToString() != String.Empty)
                        {
                            Debug.WriteLine(String.Format("string dbGender: Converting x = DbAccountDetails[7] / x = \"{0}\"", dbAccountDetails[7]));
                            dbGender = dbAccountDetails[7].ToString();
                        }

                        Debug.WriteLine(String.Format("string dbAddress1: Converting x = DbAccountDetails[8] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[8]));
                        string dbAddress1 = dbAccountDetails[8].ToString().Trim();

                        Debug.WriteLine(String.Format("string dbAddress2: Converting x = DbAccountDetails[9] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[9]));
                        string dbAddress2 = dbAccountDetails[9].ToString().Trim();

                        Debug.WriteLine(String.Format("string dbAddress3: Converting x = DbAccountDetails[10] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[10]));
                        string dbAddress3 = dbAccountDetails[10].ToString().Trim();

                        Debug.WriteLine(String.Format("string dbPostCode: Converting x = DbAccountDetails[11] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[11]));
                        string dbPostCode = dbAccountDetails[11].ToString().Trim();

                        Debug.WriteLine(String.Format("string dbMobilePhone: Converting x = DbAccountDetails[12] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[12]));
                        string dbMobilePhone = dbAccountDetails[12].ToString().Trim();

                        Debug.WriteLine(String.Format("string dbHomePhone: Converting x = DbAccountDetails[13] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[13]));
                        string dbHomePhone = dbAccountDetails[13].ToString().Trim();

                        Debug.WriteLine(String.Format("string dbWorkPhone: Converting x = DbAccountDetails[14] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[14]));
                        string dbWorkPhone = dbAccountDetails[14].ToString().Trim();

                        DateTime dbDateTimeCreated;
                        if (dbAccountDetails[15] == String.Empty || dbAccountDetails[15].ToString() == String.Empty)
                        {
                            Debug.WriteLine(String.Format("string DbDateTimeCreated: Converting x = DbAccountDetails[15] / x = \"{0}\" (DateTime.MinValue)", dbAccountDetails[15]));
                            dbDateTimeCreated = DateTime.MinValue;
                        }
                        else
                        {
                            Debug.WriteLine(String.Format("string DbDateTimeCreated: Converting x = DbAccountDetails[15] / x = \"{0}\" (Convert.ToDateTime(x))", dbAccountDetails[15]));
                            dbDateTimeCreated = Convert.ToDateTime(dbAccountDetails[15]);
                        }

                        DateTime dbLastLogIn;
                        if (dbAccountDetails[16] == String.Empty || dbAccountDetails[16].ToString() == String.Empty)
                        {
                            Debug.WriteLine(String.Format("string DbLastLogIn: Converting x = DbAccountDetails[16] / x = \"{0}\" (DateTime.MinValue)", dbAccountDetails[16]));
                            dbLastLogIn = DateTime.MinValue;
                        }
                        else
                        {
                            Debug.WriteLine(String.Format("string DbLastLogIn: Converting x = DbAccountDetails[16] / x = \"{0}\" (Convert.ToDateTime(x))", dbAccountDetails[16]));
                            dbLastLogIn = Convert.ToDateTime(dbAccountDetails[16]);
                        }

                        Debug.WriteLine(String.Format("string DbAccountType: Converting x = DbAccountDetails[17] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[17]));
                        string DbAccountType = dbAccountDetails[17].ToString().Trim();

                        Debug.WriteLine(String.Format("string DbAccountStatus: Converting x = DbAccountDetails[18] / x = \"{0}\" (x.ToString()Trim()).", dbAccountDetails[18]));
                        string DbAccountStatus = dbAccountDetails[18].ToString().Trim();
                        
                        if(Username != dbUsername)
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
                        
                        if(Email != dbEmail)
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

                        if (Pwd.Length > 0 && Pwd != dbPwd)
                        {
                            Debug.WriteLine("Updating password...");

                            SqlCommand UpdatePwd = new SqlCommand("UPDATE t_Users SET Pwd = @pwd WHERE Username = @username", conn);

                            UpdatePwd.Parameters.Add(new SqlParameter("username", Username));
                            UpdatePwd.Parameters.Add(new SqlParameter("pwd", Pwd));

                            UpdatePwd.ExecuteNonQuery();

                            Debug.WriteLine("Password updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Password was not changed. Not updating password.");
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

                        if (Surname != dbSurname)
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

                        if (_gender != dbGender)
                        {
                            Debug.WriteLine("Updating gender ({0} != {1}...", _gender, dbGender);

                            SqlCommand UpdateSurname = new SqlCommand("UPDATE t_Users SET Gender = @gender WHERE Username = @username", conn);

                            UpdateSurname.Parameters.Add(new SqlParameter("username", Username));
                            UpdateSurname.Parameters.Add(new SqlParameter("gender",  _gender));

                            UpdateSurname.ExecuteNonQuery();

                            Debug.WriteLine("Gender updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Gender was not changed. Not updating gender.");
                        }

                        if (Dob != dbDob && Dob != DateTime.MinValue)
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
                            Debug.WriteLine("Dob was not changed. Not updatg dob.");
                        }

                        /*if (dbGender != null && gender != null && dbGender != gender && (gender.ToString().ToUpper() == "M" || gender.ToString().ToUpper() == "F" || gender.ToString().ToUpper() == "O"))
                        {
                            Debug.WriteLine("Updating gender...");

                            SqlCommand UpdateGender = new SqlCommand("UPDATE t_Users SET gender = @dob WHERE Username = @username", conn);

                            UpdateGender.Parameters.Add(new SqlParameter("username", Username));
                            UpdateGender.Parameters.Add(new SqlParameter("gender", gender));

                            UpdateGender.ExecuteNonQuery();

                            Debug.WriteLine("Gender updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Gender was not changed. Not updating gender.");
                        }*/

                        if (Address1 != dbAddress1)
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

                        if (Address2 != dbAddress2)
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

                        if (Address3 != dbAddress3)
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

                        if (PostCode != dbPostCode)
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

                        if (MobilePhone != dbMobilePhone)
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

                        if (HomePhone != dbHomePhone)
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

                        if (WorkPhone != dbWorkPhone)
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
                            SqlCommand InsertIntoDb = new SqlCommand("INSERT INTO t_Users (id, Username, Forename, Surname, DateTimeCreated) VALUES(NEWID(), @username, @fname, @sname, @dateTimeCreated)", conn);
                            InsertIntoDb.Parameters.Add(new SqlParameter("username", Username.Trim()));
                            InsertIntoDb.Parameters.Add(new SqlParameter("fname", Forename.Trim()));
                            InsertIntoDb.Parameters.Add(new SqlParameter("sname", Surname.Trim()));
                            InsertIntoDb.Parameters.Add(new SqlParameter("dateTimeCreated", ParsnipApi.Data.adjustedTime));

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
