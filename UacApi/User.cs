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

        private Guid _id;
        public Guid id { get { return _id; } private set { Debug.WriteLine(string.Format("----------id is being set to = {0}", value)); _id = value; } }
        private string _username;
        public string username { get { return _username; } set { Debug.WriteLine(string.Format("----------username is being set to = {0}", value)); _username = value; } }
        private string _email;
        public string email { get { return _email; } set { Debug.WriteLine(string.Format("----------email is being set to = {0}", value)); _email = value; } }
        private string _pwd;
        public string pwd { get { return _pwd; } set { Debug.WriteLine(string.Format("----------pwd is being set to = {0}", value)); _pwd = value; } }
        private string _forename;
        public string forename { get { return _forename; } set { Debug.WriteLine(string.Format("----------forename is being set to = {0}", value)); _forename = value; } }
        private string _surname;
        public string surname { get { return _surname; } set { Debug.WriteLine(string.Format("----------surname is being set to = {0}", value)); _surname = value; } }
        private DateTime _dob;
        public DateTime dob { get { return _dob; } set { Debug.WriteLine(string.Format("----------dob is being set to = {0}", value)); _dob = value; } }
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
                Debug.WriteLine(string.Format("----------Gender is being set to = {0}", value));
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
                Debug.WriteLine(string.Format("----------gender is being set to = {0}", value));
                if(value.Length > 0)
                {
                    string tempGender = value.Substring(0, 1).ToUpper();
                    if (tempGender == "M" || tempGender == "F" || tempGender == "O")
                        _gender = tempGender;
                    else
                        throw new InvalidCastException("Could not convert gender!");
                }
            }
        }
                
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
        private string _address1;
        public string address1 { get { return _address1; } set { Debug.WriteLine(string.Format("----------address1 is being set to = {0}", value)); _address1 = value; } }
        private string _address2;
        public string address2 { get { return _address2; } set { Debug.WriteLine(string.Format("----------address2 is being set to = {0}", value)); _address2 = value; } }
        private string _address3;
        public string address3 { get { return _address3; } set { Debug.WriteLine(string.Format("----------address3 is being set to = {0}", value)); _address3 = value; } }
        private string _postCode;
        public string postCode { get { return _postCode; } set { Debug.WriteLine(string.Format("----------postCode is being set to = {0}", value)); _postCode = value; } }
        private string _mobilePhone;
        public string mobilePhone { get { return _mobilePhone; } set { Debug.WriteLine(string.Format("----------mobilePhone is being set to = {0}", value)); _mobilePhone = value; } }
        private string _homePhone;
        public string homePhone { get { return _homePhone; } set { Debug.WriteLine(string.Format("----------homePhone is being set to = {0}", value)); _homePhone = value; } }
        private string _workPhone;
        public string workPhone { get { return _workPhone; } set { Debug.WriteLine(string.Format("----------workPhone is being set to = {0}", value)); _workPhone = value; } }
        private DateTime _dateTimeCreated;
        public DateTime dateTimeCreated { get { return _dateTimeCreated; } set { Debug.WriteLine(string.Format("----------dateTimeCreated is being set to = {0}", value)); _dateTimeCreated = value; } }
        private DateTime _lastLogIn;
        public DateTime lastLogIn { get { return _lastLogIn; } set { Debug.WriteLine(string.Format("----------lastLogIn is being set to = {0}", value)); _lastLogIn = value; } }
        private string _accountType;
        public string accountType { get { return _accountType; } set { Debug.WriteLine(string.Format("----------accountType is being set to = {0}", value)); _accountType = value; } }
        private string _accountStatus;
        public string accountStatus { get { return _accountStatus; } set { Debug.WriteLine(string.Format("----------accountStatus is being set to = {0}", value)); _accountStatus = value; } }
        private Guid _createdByUserId;
        public Guid createdByUserId { get { return _createdByUserId; } set { Debug.WriteLine(string.Format("----------createdByUserId is being set to = {0}", value)); _createdByUserId = value; } }
        public string fullName { get { return string.Format("{0} {1}", forename, surname); } }


        public User()
        {
            id = Guid.NewGuid();
            dateTimeCreated = ParsnipApi.Data.adjustedTime;
            AccountLog = new LogWriter("Account Object.txt", AppDomain.CurrentDomain.BaseDirectory);
        }

        public User(Guid pGuid)
        {
            id = pGuid;
        }

        public User(SqlDataReader pReader)
        {
            AddValues(pReader);
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
                if (username.Length == 0)
                {
                    new LogEntry(id) { text = "Cannot create a user without a username! Username: " + username };
                    return false;
                }
                else if (username.Length > 50)
                {
                    new LogEntry(id) { text = String.Format("Username is {0} characters long. Username must be no longer than 50 characters!", username.Length) };
                    return false;
                }
                else
                {
                    return true;
                }
            }

            bool validateEmail()
            {
                string EmailAddress = email;

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
                if (forename.Length > 0)
                    return true;
                else
                    return false;
            }

            bool validateSurname()
            {
                if (surname.Length > 0)
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
                username = Cookie.Read("userName");
                AccountLog.Debug("Found a username cookie! Username = " + username);
                UserDetails[0] = username;
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
            username = Cookies[0];
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
            username = pUsername;
            return Select(new SqlConnection(sqlConnectionString));
        }

        internal bool AddValues(SqlDataReader pReader)
        {
            Debug.WriteLine("----------Adding values...");
            try
            {
                Debug.WriteLine("----------Reading id");
                id = new Guid(pReader[0].ToString());
                Debug.WriteLine("----------Reading username");
                username = pReader[1].ToString().Trim();
                Debug.WriteLine("----------Reading email");
                email = pReader[2].ToString().Trim();
                //Debug.WriteLine("----------Reading pwd");
                //pwd = pReader[3].ToString().Trim();
                Debug.WriteLine("----------Reading forename");
                forename = pReader[4].ToString().Trim();
                Debug.WriteLine("----------Reading surname");
                surname = pReader[5].ToString().Trim();
                Debug.WriteLine("----------Reading dob");
                dob = Convert.ToDateTime(pReader[6]);
                Debug.WriteLine("----------Reading gender");
                gender = pReader[7].ToString();
                Debug.WriteLine("----------Reading address1");
                address1 = pReader[8].ToString().Trim();
                Debug.WriteLine("----------Reading address2");
                address2 = pReader[9].ToString().Trim();
                Debug.WriteLine("----------Reading address3");
                address3 = pReader[10].ToString().Trim();
                Debug.WriteLine("----------Reading postCode");
                postCode = pReader[11].ToString().Trim();
                Debug.WriteLine("----------Reading mobilePhone");
                mobilePhone = pReader[12].ToString().Trim();
                Debug.WriteLine("----------Reading homePhone");
                homePhone = pReader[13].ToString().Trim();
                Debug.WriteLine("----------Reading workPhone");
                workPhone = pReader[14].ToString().Trim();
                Debug.WriteLine("----------Reading dateTimeCreated");
                dateTimeCreated = Convert.ToDateTime(pReader[15]);
                Debug.WriteLine("----------Reading lastLogIn");
                lastLogIn = Convert.ToDateTime(pReader[16]);
                Debug.WriteLine("----------Reading accountType");
                accountType = pReader[17].ToString().Trim();
                Debug.WriteLine("----------Reading accountStatus");
                accountStatus = pReader[18].ToString().Trim();

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst reading the User's values: ", e);
                return false;
            }
        }

        internal bool Select()
        {
            return Select(new SqlConnection(ParsnipApi.Data.sqlConnectionString));
        }

        internal bool Select(SqlConnection pConn)
        {
            AccountLog.Debug("Attempting to get user details...");
            Debug.WriteLine("Attempting to get user details...");
            
            try
            {
                SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_Users WHERE Username = @username", pConn);
                SelectAccount.Parameters.Add(new SqlParameter("username", username));

                using (SqlDataReader reader = SelectAccount.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AddValues(reader);
                    }
                }
                Debug.WriteLine("Got user's details successfully!");
                AccountLog.Debug("Got user's details successfully!");
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst getting user data: " + e);
                return false;
            }
        }

        public bool LogIn(string pUsername, bool pRememberUsername, string pPwd, bool pRememberPwd)
        {
            return LogIn(pUsername, pRememberUsername, pPwd, pRememberPwd, true);
        }

        public bool LogIn(string pUsername, bool pRememberUsername, string pPwd, bool pRememberPwd, bool silent)
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
                        AccountLog.Debug("username = " + username);
                        SqlCommand Command = new SqlCommand("UPDATE t_Users SET LastLogIn = GETDATE() WHERE Username = @Username;", conn);
                        Command.Parameters.Add(new SqlParameter("Username", username));
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
                        GetLogInDetails.Parameters.Add(new SqlParameter("Username", username));

                        using (SqlDataReader reader = GetLogInDetails.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                id = new Guid(reader[0].ToString());
                                username = reader[1].ToString();

                                if (reader[2] != DBNull.Value)
                                {
                                    email = reader[2].ToString().Trim();
                                }

                                pwd = reader[3].ToString();
                                
                                forename = reader[4].ToString().Trim();
                                surname = reader[5].ToString().Trim();

                                if(reader[6] != DBNull.Value)
                                {
                                    dob = Convert.ToDateTime(reader[6]);
                                }

                                if (reader[7] != DBNull.Value)
                                {
                                    gender = reader[7].ToString();
                                }

                                if (reader[8] != DBNull.Value)
                                {
                                    address1 = reader[8].ToString().Trim();
                                }

                                if (reader[9] != DBNull.Value)
                                {
                                    address2 = reader[9].ToString().Trim();
                                }

                                if (reader[10] != DBNull.Value)
                                {
                                    address3 = reader[10].ToString().Trim();
                                }

                                if (reader[11] != DBNull.Value)
                                {
                                    postCode = reader[11].ToString().Trim();
                                }

                                if (reader[12] != DBNull.Value)
                                {
                                    mobilePhone = reader[12].ToString().Trim();
                                }

                                if (reader[13] != DBNull.Value)
                                {
                                    homePhone = reader[13].ToString().Trim();
                                }

                                if (reader[14] != DBNull.Value)
                                {
                                    workPhone = reader[14].ToString().Trim();
                                }

                                dateTimeCreated = Convert.ToDateTime(reader[15]);

                                if(reader[16] != DBNull.Value)
                                {
                                    lastLogIn = Convert.ToDateTime(reader[16]);
                                }

                                accountType = reader[17].ToString().Trim();

                                accountStatus = reader[18].ToString().Trim();
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

                        User temp = new User(id);
                        temp.Select();
                        
                        if(username != temp.username)
                        {
                            Debug.WriteLine("Updating username...");

                            SqlCommand UpdateUsername = new SqlCommand("UPDATE t_Users SET Username = @username WHERE Username = @username", conn);

                            UpdateUsername.Parameters.Add(new SqlParameter("username", username));

                            UpdateUsername.ExecuteNonQuery();

                            Debug.WriteLine("Username updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Username was not changed. Not updating username.");
                        }
                        
                        if(email != temp.email)
                        {
                            Debug.WriteLine("Updating email...");

                            SqlCommand UpdateEmail = new SqlCommand("UPDATE t_Users SET Email = @email WHERE Username = @username", conn);

                            UpdateEmail.Parameters.Add(new SqlParameter("username", username));
                            UpdateEmail.Parameters.Add(new SqlParameter("email", email));

                            UpdateEmail.ExecuteNonQuery();

                            Debug.WriteLine("Email updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Email was not changed. Not updating email.");
                        }

                        if (pwd.Length > 0 && pwd != temp.pwd)
                        {
                            Debug.WriteLine("Updating password...");

                            SqlCommand UpdatePwd = new SqlCommand("UPDATE t_Users SET Pwd = @pwd WHERE Username = @username", conn);

                            UpdatePwd.Parameters.Add(new SqlParameter("username", username));
                            UpdatePwd.Parameters.Add(new SqlParameter("pwd", pwd));

                            UpdatePwd.ExecuteNonQuery();

                            Debug.WriteLine("Password updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Password was not changed. Not updating password.");
                        }

                        if (forename != temp.forename)
                        {
                            Debug.WriteLine("Updating forename...");

                            SqlCommand UpdateForename = new SqlCommand("UPDATE t_Users SET Forename = @forename WHERE Username = @username", conn);

                            UpdateForename.Parameters.Add(new SqlParameter("username", username));
                            UpdateForename.Parameters.Add(new SqlParameter("forename", email));

                            UpdateForename.ExecuteNonQuery();

                            Debug.WriteLine("Forename updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Forename was not changed. Not updating forename.");
                        }

                        if (surname != temp.surname)
                        {
                            Debug.WriteLine("Updating surname...");

                            SqlCommand UpdateSurname = new SqlCommand("UPDATE t_Users SET Surname = @surname WHERE Username = @username", conn);

                            UpdateSurname.Parameters.Add(new SqlParameter("username", username));
                            UpdateSurname.Parameters.Add(new SqlParameter("surname", surname));

                            UpdateSurname.ExecuteNonQuery();

                            Debug.WriteLine("Surname updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Surname was not changed. Not updating surname.");
                        }

                        if (_gender != temp.Gender)
                        {
                            Debug.WriteLine("Updating gender ({0} != {1}...", _gender, temp.Gender);

                            SqlCommand UpdateSurname = new SqlCommand("UPDATE t_Users SET Gender = @gender WHERE Username = @username", conn);

                            UpdateSurname.Parameters.Add(new SqlParameter("username", username));
                            UpdateSurname.Parameters.Add(new SqlParameter("gender",  _gender));

                            UpdateSurname.ExecuteNonQuery();

                            Debug.WriteLine("Gender updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Gender was not changed. Not updating gender.");
                        }

                        if (dob != temp.dob && dob != DateTime.MinValue)
                        {
                            Debug.WriteLine("Updating dob...");

                            SqlCommand UpdateDob = new SqlCommand("UPDATE t_Users SET Dob = @dob WHERE Username = @username", conn);

                            UpdateDob.Parameters.Add(new SqlParameter("username", username));
                            UpdateDob.Parameters.Add(new SqlParameter("dob", dob));

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

                        if (address1 != temp.address1)
                        {
                            Debug.WriteLine("Updating address1...");

                            SqlCommand UpdateAddress1 = new SqlCommand("UPDATE t_Users SET Address1 = @address1 WHERE Username = @username", conn);

                            UpdateAddress1.Parameters.Add(new SqlParameter("username", username));
                            UpdateAddress1.Parameters.Add(new SqlParameter("address1", address1));

                            UpdateAddress1.ExecuteNonQuery();

                            Debug.WriteLine("Address1 updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Address1 was not changed. Not updating address1.");
                        }

                        if (address2 != temp.address2)
                        {
                            Debug.WriteLine("Updating address2...");

                            SqlCommand UpdateAddress2 = new SqlCommand("UPDATE t_Users SET Address2 = @address2 WHERE Username = @username", conn);

                            UpdateAddress2.Parameters.Add(new SqlParameter("username", username));
                            UpdateAddress2.Parameters.Add(new SqlParameter("address2", address2));

                            UpdateAddress2.ExecuteNonQuery();

                            Debug.WriteLine("Address2 updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Address2 was not changed. Not updating address2.");
                        }

                        if (address3 != temp.address3)
                        {
                            Debug.WriteLine("Updating address3...");

                            SqlCommand UpdateAddress3 = new SqlCommand("UPDATE t_Users SET Address3 = @address3 WHERE Username = @username", conn);

                            UpdateAddress3.Parameters.Add(new SqlParameter("username", username));
                            UpdateAddress3.Parameters.Add(new SqlParameter("address3", address3));

                            UpdateAddress3.ExecuteNonQuery();

                            Debug.WriteLine("Address3 updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Address3 was not changed. Not updating address3.");
                        }

                        if (postCode != temp.postCode)
                        {
                            Debug.WriteLine("Updating postcode...");

                            SqlCommand UpdatePostCode = new SqlCommand("UPDATE t_Users SET PostCode = @postCode WHERE Username = @username", conn);

                            UpdatePostCode.Parameters.Add(new SqlParameter("username", username));
                            UpdatePostCode.Parameters.Add(new SqlParameter("postCode", postCode));

                            UpdatePostCode.ExecuteNonQuery();

                            Debug.WriteLine("Postcode updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Postcode was not changed. Not updating postcode.");
                        }

                        if (mobilePhone != temp.mobilePhone)
                        {
                            Debug.WriteLine("Updating mobile phone...");

                            SqlCommand UpdateMobilePhone = new SqlCommand("UPDATE t_Users SET MobilePhone = @mobilePhone WHERE Username = @username", conn);

                            UpdateMobilePhone.Parameters.Add(new SqlParameter("username", username));
                            UpdateMobilePhone.Parameters.Add(new SqlParameter("mobilePhone", mobilePhone));

                            UpdateMobilePhone.ExecuteNonQuery();

                            Debug.WriteLine("Mobile phone updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Mobile phone was not changed. Not updating mobile phone.");
                        }

                        if (homePhone != temp.homePhone)
                        {
                            Debug.WriteLine("Updating home phone...");

                            SqlCommand UpdateHomePhone = new SqlCommand("UPDATE t_Users SET HomePhone = @homePhone WHERE Username = @username", conn);

                            UpdateHomePhone.Parameters.Add(new SqlParameter("username", username));
                            UpdateHomePhone.Parameters.Add(new SqlParameter("homePhone", homePhone));

                            UpdateHomePhone.ExecuteNonQuery();

                            Debug.WriteLine("Home phone updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Home phone was not changed. Not updating home phone.");
                        }

                        if (workPhone != temp.workPhone)
                        {
                            Debug.WriteLine("Updating work phone...");

                            SqlCommand UpdateWorkPhone = new SqlCommand("UPDATE t_Users SET WorkPhone = @workPhone WHERE Username = @username", conn);

                            UpdateWorkPhone.Parameters.Add(new SqlParameter("username", username));
                            UpdateWorkPhone.Parameters.Add(new SqlParameter("workPhone", workPhone));

                            UpdateWorkPhone.ExecuteNonQuery();

                            Debug.WriteLine("Work phone updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Work phone was not changed. Not updating work phone.");
                        }

                        if (accountType != temp.accountType)
                        {
                            Debug.WriteLine("Updating account type...");

                            SqlCommand UpdateAccountType = new SqlCommand("UPDATE t_Users SET AccountType = @accountType WHERE Username = @username", conn);

                            UpdateAccountType.Parameters.Add(new SqlParameter("username", username));
                            UpdateAccountType.Parameters.Add(new SqlParameter("accountType", accountType));

                            UpdateAccountType.ExecuteNonQuery();

                            Debug.WriteLine("Account type updated successfully!");
                        }
                        else
                        {
                            Debug.WriteLine("Account type was not changed. Not account type.");
                        }

                        if (accountStatus != temp.accountStatus)
                        {
                            Debug.WriteLine("Updating account status...");

                            SqlCommand UpdateAccountStatus = new SqlCommand("UPDATE t_Users SET AccountStatus = @accountStatus WHERE Username = @username", conn);

                            UpdateAccountStatus.Parameters.Add(new SqlParameter("username", username));
                            UpdateAccountStatus.Parameters.Add(new SqlParameter("accountStatus", accountStatus));

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
                    Debug.WriteLine("Caught an error whilst updating account (\"{0}\": {1}",username, e);
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
            if (username != null && forename != null && surname != null)
            {
                try
                {


                    using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                    {
                        conn.Open();

                        bool UsernameExists()
                        {
                            Debug.WriteLine(String.Format("Checking that username \"{0}\" does not exist in database before attempting insert...", username.Trim()));

                            SqlCommand FindUsername = new SqlCommand("SELECT count(1) FROM t_Users WHERE Username = @username", conn);
                            FindUsername.Parameters.Add(new SqlParameter("username", username));



                            using (SqlDataReader reader = FindUsername.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader[0].ToString() == "1")
                                    {
                                        Debug.WriteLine(String.Format("Username \"{0}\" already exists in the database! Returning true.", username.Trim()));
                                        return true;
                                    }
                                    else
                                    {
                                        Debug.WriteLine(String.Format("Username \"{0}\" does not already exist in the database! Returning false.", username.Trim()));
                                        return false;
                                    }
                                }
                            }
                            throw new Exception("Expression evaluated to neither true or false");
                        }

                        if (!UsernameExists())
                        {
                            SqlCommand InsertIntoDb = new SqlCommand("INSERT INTO t_Users (id, Username, Forename, Surname, DateTimeCreated) VALUES(NEWID(), @username, @fname, @sname, @dateTimeCreated)", conn);
                            InsertIntoDb.Parameters.Add(new SqlParameter("username", username.Trim()));
                            InsertIntoDb.Parameters.Add(new SqlParameter("fname", forename.Trim()));
                            InsertIntoDb.Parameters.Add(new SqlParameter("sname", surname.Trim()));
                            InsertIntoDb.Parameters.Add(new SqlParameter("dateTimeCreated", ParsnipApi.Data.adjustedTime));

                            InsertIntoDb.ExecuteNonQuery();

                            Debug.WriteLine(String.Format("Successfully inserted account \"{0}\" into database: ", username));
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
