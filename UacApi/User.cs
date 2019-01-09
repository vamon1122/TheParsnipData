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
        public Guid id { get { return _id; } private set { /*Debug.WriteLine(string.Format("----------{0}'s id is being set to = {1}",_id, value));*/ _id = value; } }
        private string _username;
        public string username { get { return _username; } set { /*Debug.WriteLine(string.Format("----------username is being set to = {0}", value));*/ _username = value; } }
        private string _email;
        public string email { get { return _email; } set { /*Debug.WriteLine(string.Format("----------email is being set to = {0}", value));*/ _email = value; } }
        private string _pwd;
        public string pwd { get { return _pwd; } set { /*Debug.WriteLine(string.Format("----------pwd is being set to = {0}", value));*/ _pwd = value; } }
        private string _forename;
        public string forename { get { return _forename; } set { /*Debug.WriteLine(string.Format("----------forename is being set to = {0}", value));*/ _forename = value; } }
        private string _surname;
        public string surname { get { return _surname; } set { /*Debug.WriteLine(string.Format("----------surname is being set to = {0}", value));*/ _surname = value; } }
        private DateTime _dob;
        public DateTime dob { get { return _dob; } set { /*Debug.WriteLine(string.Format("----------dob is being set to = {0}", value));*/ _dob = value; } }
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
                //Debug.WriteLine(string.Format("----------Gender is being set to = {0}", value));
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
                //Debug.WriteLine(string.Format("----------gender is being set to = {0}", value));
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
        public string address1 { get { return _address1; } set { /*Debug.WriteLine(string.Format("----------address1 is being set to = {0}", value));*/ _address1 = value; } }
        private string _address2;
        public string address2 { get { return _address2; } set { /*Debug.WriteLine(string.Format("----------address2 is being set to = {0}", value));*/ _address2 = value; } }
        private string _address3;
        public string address3 { get { return _address3; } set { /*Debug.WriteLine(string.Format("----------address3 is being set to = {0}", value));*/ _address3 = value; } }
        private string _postCode;
        public string postCode { get { return _postCode; } set { /*Debug.WriteLine(string.Format("----------postCode is being set to = {0}", value));*/ _postCode = value; } }
        private string _mobilePhone;
        public string mobilePhone { get { return _mobilePhone; } set { /*Debug.WriteLine(string.Format("----------mobilePhone is being set to = {0}", value));*/ _mobilePhone = value; } }
        private string _homePhone;
        public string homePhone { get { return _homePhone; } set { /*Debug.WriteLine(string.Format("----------homePhone is being set to = {0}", value));*/ _homePhone = value; } }
        private string _workPhone;
        public string workPhone { get { return _workPhone; } set { /*Debug.WriteLine(string.Format("----------workPhone is being set to = {0}", value));*/ _workPhone = value; } }
        private DateTime _dateTimeCreated;
        public DateTime dateTimeCreated { get { return _dateTimeCreated; } set { /*Debug.WriteLine(string.Format("----------dateTimeCreated is being set to = {0}", value));*/ _dateTimeCreated = value; } }
        private DateTime _lastLogIn;
        public DateTime lastLogIn { get { return _lastLogIn; } set { /*Debug.WriteLine(string.Format("----------lastLogIn is being set to = {0}", value));*/ _lastLogIn = value; } }
        private string _accountType;
        public string accountType { get { return _accountType; } set { /*Debug.WriteLine(string.Format("----------accountType is being set to = {0}", value));*/ _accountType = value; } }
        private string _accountStatus;
        public string accountStatus { get { return _accountStatus; } set { /*Debug.WriteLine(string.Format("----------accountStatus is being set to = {0}", value));*/ _accountStatus = value; } }
        private Guid _createdByUserId;
        public Guid createdByUserId { get { return _createdByUserId; } set { /*Debug.WriteLine(string.Format("----------createdByUserId is being set to = {0}", value));*/ _createdByUserId = value; } }
        public string fullName { get { return string.Format("{0} {1}", forename, surname); } }


        public User(string pWhereAmI)
        {
            _id = Guid.NewGuid();
            //Debug.WriteLine(string.Format("User was initialised without a guid. WhereAmI = {0} Their guid will be: {1}", pWhereAmI, id));
            dateTimeCreated = ParsnipApi.Data.adjustedTime;
            AccountLog = new LogWriter("Account Object.txt", AppDomain.CurrentDomain.BaseDirectory);
        }

        public User(Guid pGuid)
        {
            //Debug.WriteLine("User was initialised with the guid: " + pGuid);
            _id = pGuid;
        }

        public User(SqlDataReader pReader)
        {
            //Debug.WriteLine("User was initialised with an SqlDataReader. Guid: " + pReader[0]);
            AddValues(pReader);
        }

        public bool Validate()
        {
            bool validateSuccess = true;

            validateSuccess = validateUsername() ? validateSuccess : false;
            validateSuccess = validateEmail() ? validateSuccess : false;
            validateSuccess = validatePwd() ? validateSuccess : false;
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
            
            bool validatePwd()
            {
                if (pwd.Trim().Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

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
            

            //Debug.WriteLine("CookieUsername = " + CookieUsername);
            //Debug.WriteLine("CookiePwd = " + CookiePwd);

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

        public static List<User> GetAllUsers()
        {

            var users = new List<User>();
            using (SqlConnection conn = ParsnipApi.Data.GetOpenDbConnection())
            {
                SqlCommand GetUsers = new SqlCommand("SELECT * FROM t_Users", conn);
                using (SqlDataReader reader = GetUsers.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UacApi.User(reader));
                    }
                }
            }

            return users;
        }

        internal bool LogIn(string pUsername)
        {
            username = pUsername;
            return Select(new SqlConnection(sqlConnectionString));
        }

        internal bool AddValues(SqlDataReader pReader)
        {
            //Debug.WriteLine("----------Adding values...");
            try
            {
                //Debug.WriteLine(string.Format("----------Reading id: {0}",pReader[0]));
                id = new Guid(pReader[0].ToString());
                //Debug.WriteLine(string.Format("----------Reading username: {0}", pReader[1]));
                username = pReader[1].ToString().Trim();
                if (pReader[2] == DBNull.Value)
                { }   //Debug.WriteLine("----------email is blank. Skipping email");
                else
                {
                    //Debug.WriteLine("----------Reading email");
                    email = pReader[2].ToString().Trim();
                }
                
                //Debug.WriteLine("----------Reading pwd");
                pwd = pReader[3].ToString().Trim();
                //Debug.WriteLine("----------Reading forename");
                forename = pReader[4].ToString().Trim();
                //Debug.WriteLine("----------Reading surname");
                surname = pReader[5].ToString().Trim();
                if (pReader[6] == DBNull.Value) { }
                //Debug.WriteLine("----------dob is blank. Skipping dob");
                else
                {
                    //Debug.WriteLine("----------Reading dob");
                    dob = Convert.ToDateTime(pReader[6]);
                }
                if (pReader[7] == DBNull.Value || pReader[7].ToString() == "")
                { }// Debug.WriteLine("----------gender is blank. Skipping gender");
                else
                {
                    //Debug.WriteLine("----------Reading gender");
                    gender = pReader[7].ToString();
                }
                if (pReader[8] == DBNull.Value)
                { }//Debug.WriteLine("----------address1 is blank. Skipping address1");
                else
                {
                    //Debug.WriteLine("----------Reading address1");
                    address1 = pReader[8].ToString().Trim();
                }
                if (pReader[9] == DBNull.Value)
                { }// Debug.WriteLine("----------address2 is blank. Skipping address2");
                else
                {
                    //Debug.WriteLine("----------Reading address2");
                    address2 = pReader[9].ToString().Trim();
                }
                if (pReader[10] == DBNull.Value)
                { }// Debug.WriteLine("----------address3 is blank. Skipping address3");
                else
                {
                    //Debug.WriteLine("----------Reading address3");
                    address3 = pReader[10].ToString().Trim();
                }
                if (pReader[11] != DBNull.Value)
                { }// Debug.WriteLine("----------postCode is blank. Skipping postCode");
                else
                {
                    //Debug.WriteLine("----------Reading postCode");
                    postCode = pReader[11].ToString().Trim();
                }
                if (pReader[12] != DBNull.Value)
                { }   //Debug.WriteLine("----------mobilePhone is blank. Skipping mobilePhone");
                else
                {
                    { }// Debug.WriteLine("----------Reading mobilePhone");
                    mobilePhone = pReader[12].ToString().Trim();
                }
                if (pReader[13] != DBNull.Value)
                { }// Debug.WriteLine("----------homePhone is blank. Skipping homePhone");
                else
                {
                    //Debug.WriteLine("----------Reading homePhone");
                    homePhone = pReader[13].ToString().Trim();
                }
                if (pReader[14] != DBNull.Value)
                { }// Debug.WriteLine("----------workPhone is blank. Skipping workPhone");
                else
                {
                    //Debug.WriteLine("----------Reading workPhone");
                    workPhone = pReader[14].ToString().Trim();
                }
                //Debug.WriteLine("----------Reading dateTimeCreated");
                dateTimeCreated = Convert.ToDateTime(pReader[15]);
                if (pReader[16] == DBNull.Value)
                { }// Debug.WriteLine("----------lastLogIn is blank. Skipping lastLogIn");
                else
                {
                    //Debug.WriteLine("----------Reading lastLogIn");
                    lastLogIn = Convert.ToDateTime(pReader[16]);
                }
                
                //Debug.WriteLine("----------Reading accountType");
                accountType = pReader[17].ToString().Trim();
                //Debug.WriteLine("----------Reading accountStatus");
                accountStatus = pReader[18].ToString().Trim();

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst reading the User's values: ", e);
                return false;
            }
        }

        public bool Select()
        {
            return Select(ParsnipApi.Data.GetOpenDbConnection());
        }

        internal bool Select(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get user details...");
            Debug.WriteLine("Attempting to get user details...");
            
            try
            {
                SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_Users WHERE id = @id", pOpenConn);
                SelectAccount.Parameters.Add(new SqlParameter("id", id.ToString()));

                using (SqlDataReader reader = SelectAccount.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AddValues(reader);
                    }
                }
                Debug.WriteLine("Got user's details successfully!");
                //AccountLog.Debug("Got user's details successfully!");
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
            //AccountLog.Info(String.Format("[LogIn] Logging in with Username = {0} & Pwd = {1}...",pUsername, pPwd));

            

            string dbPwd = null;
            username = pUsername;

            using (SqlConnection conn = ParsnipApi.Data.GetOpenDbConnection()) 
            {
                //AccountLog.Debug("[LogIn] Sql connection opened succesfully!");



                if (GetPwdFromDb() && pPwd == dbPwd)
                {
                    //AccountLog.Debug(String.Format("[LogIn] DbPwd == Pwd ({0} == {1})", dbPwd, pPwd));
                    if (DbSelect())
                    {
                        
                        if (pRememberUsername)
                        {
                            //AccountLog.Debug(String.Format("[LogIn] RememberUsername = true. Writing permanent username cookie (userName = {0})", pUsername));
                            System.Diagnostics.Debug.WriteLine("Username permanently remembered!");
                            Cookie.WritePerm("userName", pUsername);
                        }

                        if (pRememberPwd)
                        {
                            //AccountLog.Debug(String.Format("[LogIn] RememberPassword = true. Writing permanent password cookie (userPwd = {0})", pPwd));
                            System.Diagnostics.Debug.WriteLine("Password permanently remembered!");
                            Cookie.WritePerm("userPwd", pPwd);
                            System.Diagnostics.Debug.WriteLine("PERMANENT Password cookie = " + GetCookies()[1]); 
                        }
                        else
                        {
                            //AccountLog.Debug(String.Format("[LogIn] RememberPassword = false. Writing session password cookie (userPwd = {0})", pPwd));
                            if(GetCookies()[1] == pPwd)
                            {
                                //AccountLog.Debug(String.Format("[LogIn] Cookie already exists with the same value! It may have been permanently remembered! Not overwriting cookie.", pPwd));
                            }
                            else
                            {
                                //AccountLog.Debug(String.Format("[LogIn] Cookie does not exist. Writing temporary password cookie.", pPwd));
                                Cookie.WriteSession("userPwd", pPwd);
                                //AccountLog.Debug(String.Format("[LogIn] Password stored for SESSION ONLY.", pPwd));
                                System.Diagnostics.Debug.WriteLine("Password stored for SESSION ONLY.");
                            }
                            
                            
                        }

                        if (SetLastLogIn())
                        {
                            //AccountLog.Info("[LogIn] Logged in successfully!");
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
                    //AccountLog.Debug(String.Format("[LogIn] DbPwd != Pwd ({0} != {1}", dbPwd, pPwd));
                }
                //AccountLog.Error("[LogIn] Failed to log in.");
                return false;
                
                bool SetLastLogIn()
                {
                    int RecordsAffected;

                    //AccountLog.Debug("[LogIn] Attempting to set LastLogIn...");
                    try
                    {
                        //AccountLog.Debug("username = " + username);
                        SqlCommand Command = new SqlCommand("UPDATE t_Users SET LastLogIn = @date WHERE Username = @Username;", conn);
                        Command.Parameters.Add(new SqlParameter("Username", username));
                        Command.Parameters.Add(new SqlParameter("date", ParsnipApi.Data.adjustedTime));
                        RecordsAffected = Command.ExecuteNonQuery();
                        
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine("[LogIn] There was an exception whilst setting the LastLogIn: " + e);
                        return false;
                    }

                    //AccountLog.Debug(String.Format("[LogIn] Set LastLogIn successfully! {0} records were affected.", RecordsAffected));
                    return true;
                }

                bool GetPwdFromDb()
                {
                    //AccountLog.Debug("[LogIn] Attempting to get password from database...");
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
                    //AccountLog.Debug("[LogIn] Got password from database successfully!");
                    return true;
                }

                bool DbSelect()
                {
                    //AccountLog.Debug("[LogIn] Attempting to get user details...");
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
                    //AccountLog.Debug("[LogIn] Got user's details successfully!");
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

        public bool Update()
        {
            bool success;
            SqlConnection UpdateConnection = ParsnipApi.Data.GetOpenDbConnection();
            UpdateConnection.Open();
            if (ExistsOnDb(UpdateConnection)) success = DbUpdate(UpdateConnection); else success = DbInsert(pwd, UpdateConnection);
            UpdateConnection.Close();
            return success;
        }

        public bool ExistsOnDb()
        {
            return ExistsOnDb(ParsnipApi.Data.GetOpenDbConnection());
        }

        private bool ExistsOnDb(SqlConnection pOpenConn)
        {
            Debug.WriteLine(string.Format("Checking {0} weather user exists on database", id));
            try
            {

                Guid UnconsumableGuid = new Guid(id.ToString());

                SqlCommand findMe = new SqlCommand("SELECT COUNT(*) FROM t_Users WHERE id = @id", pOpenConn);
                findMe.Parameters.Add(new SqlParameter("id", id.ToString()));

                int userExists;

                using(SqlDataReader reader = findMe.ExecuteReader())
                {
                    reader.Read();
                    userExists = Convert.ToInt16(reader[0]);
                    Debug.WriteLine("userExists = " + userExists);
                }

                Debug.WriteLine(userExists + " user(s) were found with the id " + id);

                if (userExists > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("There was an error whilst checking if user exists on the database: " + e);
                return false;
            }
        }

        private bool DbUpdate(SqlConnection pOpenConn)
        {
            bool HasBeenInserted = true;

            if (HasBeenInserted)
            {
                try
                {
                    User temp = new User(id);
                    temp.Select();

                    if (username != temp.username)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update {0}'s username...", temp.fullName));
                        if (string.IsNullOrEmpty(username))
                        {
                            string e = string.Format("The username which was supplied for {0} was null or empty", temp.fullName);
                            Debug.WriteLine("----------{0}. An exception will be thrown since username is a mandatory field", e);
                            throw new InvalidCastException(e);
                        }
                        

                        SqlCommand UpdateUsername = new SqlCommand("UPDATE t_Users SET Username = @username WHERE Username = @username", pOpenConn);

                        UpdateUsername.Parameters.Add(new SqlParameter("username", username));

                        UpdateUsername.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s username was updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s username was not changed. Not updating {0}'s username.", temp.fullName));
                    }

                    
                    if (email != temp.email || email == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s email...", temp.fullName));

                        SqlCommand UpdateEmail = new SqlCommand("UPDATE t_Users SET Email = @email WHERE Username = @username", pOpenConn);

                        UpdateEmail.Parameters.Add(new SqlParameter("username", username));
                        if (email == "")
                        {
                            UpdateEmail.Parameters.Add(new SqlParameter("email", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s email will be set to NULL in the database", temp.fullName));
                        }
                        else
                            UpdateEmail.Parameters.Add(new SqlParameter("email", email));

                        UpdateEmail.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s email was updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s email was not changed. Not updating {0}'s email", temp.fullName));
                    }

                    if (pwd.Length > 0 && pwd != temp.pwd || pwd == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s password...", temp.fullName));

                        SqlCommand UpdatePwd = new SqlCommand("UPDATE t_Users SET Pwd = @pwd WHERE Username = @username", pOpenConn);

                        UpdatePwd.Parameters.Add(new SqlParameter("username", username));
                        if (pwd == "")
                        {
                            UpdatePwd.Parameters.Add(new SqlParameter("pwd", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s password will be set to NULL in the database", temp.fullName));
                        }
                        else
                            UpdatePwd.Parameters.Add(new SqlParameter("pwd", pwd));

                        UpdatePwd.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s password updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s password was not changed. Not updating {0}'s password.", temp.fullName));
                    }

                    if (forename != temp.forename)
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s forename. {1}'s forename will be changed to \"{2}\"...", temp.fullName, temp.forename, forename));

                        if (string.IsNullOrEmpty(forename))
                        {
                            string e = "The forename which was supplied was null or empty";
                            Debug.WriteLine(string.Format("----------{0}. An exception will be thrown since forename is a mandatory field", e));
                            throw new InvalidCastException(e);
                        }

                        SqlCommand UpdateForename = new SqlCommand("UPDATE t_Users SET Forename = @forename WHERE id = @id", pOpenConn);

                        UpdateForename.Parameters.Add(new SqlParameter("id", id));
                        UpdateForename.Parameters.Add(new SqlParameter("forename", forename));
                            
                        

                        UpdateForename.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s forename updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s forename was not changed. Not updating {0}'s forename.", temp.fullName));
                    }

                    if (surname != temp.surname)
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s surname. {1}'s surname will be changed to \"{2}\"...", temp.fullName, temp.surname, surname));

                        if (string.IsNullOrEmpty(surname))
                        {
                            string e = "The surname which was supplied was null or empty";
                            Debug.WriteLine(string.Format("----------{0}. An exception will be thrown since surname is a mandatory field", e));
                            throw new InvalidCastException(e);
                        }

                        SqlCommand updateSurname = new SqlCommand("UPDATE t_Users SET Surname = @surname WHERE id = @id", pOpenConn);

                        updateSurname.Parameters.Add(new SqlParameter("id", id));
                        updateSurname.Parameters.Add(new SqlParameter("surname", surname));

                        updateSurname.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s surname was updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s surname was not changed. Not updating {0}'s surname.", temp.fullName));
                    }

                    if (_gender != temp.Gender || _gender == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s gender...", temp.fullName));

                        SqlCommand updateGender = new SqlCommand("UPDATE t_Users SET Gender = @gender WHERE Username = @username", pOpenConn);

                        updateGender.Parameters.Add(new SqlParameter("username", username));
                        if (_gender == "")
                        {
                            updateGender.Parameters.Add(new SqlParameter("gender", DBNull.Value));
                            Debug.WriteLine(string.Format("----------gender will be set to NULL in the database"));
                        }
                        else
                            updateGender.Parameters.Add(new SqlParameter("gender", _gender));

                        updateGender.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s gender updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s gender was not changed. Not updating {0}'s gender.", temp.fullName));
                    }

                    if (dob != temp.dob || dob.ToString() == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s dob...", temp.fullName));

                        SqlCommand UpdateDob = new SqlCommand("UPDATE t_Users SET Dob = @dob WHERE Username = @username", pOpenConn);

                        UpdateDob.Parameters.Add(new SqlParameter("username", username));

                        if (dob == DateTime.MinValue)
                        {
                            Debug.WriteLine(string.Format("----------{0}'s dob will be set to NULL in the database", temp.fullName));
                            UpdateDob.Parameters.Add(new SqlParameter("dob", DBNull.Value));
                        }
                        else
                        UpdateDob.Parameters.Add(new SqlParameter("dob", dob));

                        UpdateDob.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s dob was updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s dob was not changed. Not updatg dob.", temp.fullName));
                    }

                    if (address1 != temp.address1 || address1 == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s address1...", temp.fullName));

                        SqlCommand UpdateAddress1 = new SqlCommand("UPDATE t_Users SET Address1 = @address1 WHERE Username = @username", pOpenConn);

                        UpdateAddress1.Parameters.Add(new SqlParameter("username", username));
                        if (address1 == "")
                        {
                            UpdateAddress1.Parameters.Add(new SqlParameter("address1", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s address1 will be set to NULL in the database", temp.fullName));
                        }
                        else
                            UpdateAddress1.Parameters.Add(new SqlParameter("address1", address1));

                        UpdateAddress1.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s address1 updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("{0}'s address1 was not changed. Not updating address1.", temp.fullName));
                    }

                    if (address2 != temp.address2 || address2 == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s address2...", temp.fullName));

                        SqlCommand UpdateAddress2 = new SqlCommand("UPDATE t_Users SET Address2 = @address2 WHERE Username = @username", pOpenConn);

                        UpdateAddress2.Parameters.Add(new SqlParameter("username", username));
                        if(address2 == "")
                        {
                            UpdateAddress2.Parameters.Add(new SqlParameter("address2", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s address2 will be set to NULL in the database", temp.fullName));
                        }
                            
                        else
                            UpdateAddress2.Parameters.Add(new SqlParameter("address2", address2));

                        UpdateAddress2.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s address2 was updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s address2 was not changed. Not updating address2.", temp.fullName));
                    }

                    if (address3 != temp.address3 || address3 == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s address3...", temp.fullName));

                        SqlCommand UpdateAddress3 = new SqlCommand("UPDATE t_Users SET Address3 = @address3 WHERE Username = @username", pOpenConn);

                        UpdateAddress3.Parameters.Add(new SqlParameter("username", username));
                        if (address3 == "")
                        {
                            UpdateAddress3.Parameters.Add(new SqlParameter("address3", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s address3 will be set to NULL in the database", temp.fullName));
                        }
                        else
                            UpdateAddress3.Parameters.Add(new SqlParameter("address3", address3));

                        UpdateAddress3.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s address3 was updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s address3 was not changed. Not updating {0}'s address3.", temp.fullName));
                    }

                    if (postCode != temp.postCode || postCode == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s postcode...", temp.fullName));

                        SqlCommand UpdatePostCode = new SqlCommand("UPDATE t_Users SET PostCode = @postCode WHERE Username = @username", pOpenConn);

                        UpdatePostCode.Parameters.Add(new SqlParameter("username", username));
                        if(postCode == "")
                        {
                            UpdatePostCode.Parameters.Add(new SqlParameter("postCode", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s postCode will be set to NULL in the database", temp.fullName));
                        }
                        else
                            UpdatePostCode.Parameters.Add(new SqlParameter("postCode", postCode));

                        UpdatePostCode.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s postCode was updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s postCode was not changed. Not updating {0}'s postcode.", temp.fullName));
                    }

                    if (mobilePhone != temp.mobilePhone || mobilePhone == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s mobilePhone...", temp.fullName));

                        SqlCommand UpdateMobilePhone = new SqlCommand("UPDATE t_Users SET MobilePhone = @mobilePhone WHERE Username = @username", pOpenConn);

                        UpdateMobilePhone.Parameters.Add(new SqlParameter("username", username));
                        if(mobilePhone == "")
                        {
                            UpdateMobilePhone.Parameters.Add(new SqlParameter("mobilePhone", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s mobilePhone will be set to NULL in the database", temp.fullName));
                        }
                        else
                            UpdateMobilePhone.Parameters.Add(new SqlParameter("mobilePhone", mobilePhone));

                        UpdateMobilePhone.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s mobilePhone updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("{0}'s mobilePhone was not changed. Not updating {0}'s mobilePhone.", temp.fullName));
                    }

                    if (homePhone != temp.homePhone || homePhone == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s homePhone...", temp.fullName));

                        SqlCommand UpdateHomePhone = new SqlCommand("UPDATE t_Users SET HomePhone = @homePhone WHERE Username = @username", pOpenConn);

                        UpdateHomePhone.Parameters.Add(new SqlParameter("username", username));
                        if (homePhone != "")
                        {
                            UpdateHomePhone.Parameters.Add(new SqlParameter("homePhone", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s homePhone will be set to NULL in the database", temp.fullName));
                        }
                        else
                            UpdateHomePhone.Parameters.Add(new SqlParameter("homePhone", homePhone));

                        UpdateHomePhone.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s homePhone was updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s homePhone was not changed. Not updating {0}'s home phone.", temp.fullName));
                    }

                    if (workPhone != temp.workPhone || workPhone == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s workPhone...", temp.fullName));

                        SqlCommand updateWorkPhone = new SqlCommand("UPDATE t_Users SET WorkPhone = @workPhone WHERE id = @id", pOpenConn);

                        updateWorkPhone.Parameters.Add(new SqlParameter("id", id));
                        if (workPhone != "")
                        {
                            updateWorkPhone.Parameters.Add(new SqlParameter("workPhone", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s workPhone will be set to NULL in the database", temp.fullName));
                        }
                        else
                            updateWorkPhone.Parameters.Add(new SqlParameter("workPhone", workPhone));

                        updateWorkPhone.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s workPhone was updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s workPhone was not changed. Not updating {0}'s workPhone", temp.fullName));
                    }

                    if (accountType != temp.accountType)
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s accountType...", temp.fullName));

                        SqlCommand updateAccountType = new SqlCommand("UPDATE t_Users SET AccountType = @accountType WHERE id = @id", pOpenConn);

                        updateAccountType.Parameters.Add(new SqlParameter("id", id));
                        updateAccountType.Parameters.Add(new SqlParameter("accountType", accountType));

                        updateAccountType.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s accountType updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s accountType was not changed. Not updating {0}'s accountType.", temp.fullName));
                    }

                    if (accountStatus != temp.accountStatus)
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s accountStatus...", temp.fullName));

                        SqlCommand updateAccountStatus = new SqlCommand("UPDATE t_Users SET AccountStatus = @accountStatus WHERE Username = @username", pOpenConn);

                        updateAccountStatus.Parameters.Add(new SqlParameter("username", username));
                        updateAccountStatus.Parameters.Add(new SqlParameter("accountStatus", accountStatus));

                        updateAccountStatus.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s accountStatus updated successfully!", temp.fullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s accountStatus was not changed. Not updating {0}'s accountStatus.", temp.fullName));
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(string.Format("Caught an error whilst updating account (\"{0}\": {1}", username, e));
                    return false;
                }
                return true;
            }
            else
            {
                throw new System.InvalidOperationException("Account cannot be updated. Account must be inserted into the database before it can be updated!");
            }
        }

        private bool DbInsert(string pPwd, SqlConnection pOpenConn)
        {
            if (username != null && forename != null && surname != null)
            {
                try
                {
                    if (!ExistsOnDb(pOpenConn))
                    {
                        SqlCommand InsertIntoDb = new SqlCommand("INSERT INTO t_Users (id, Username, Forename, Surname, DateTimeCreated, AccountType, AccountStatus) VALUES(@id, @username, @fname, @sname, @dateTimeCreated, @accountType, @accountStatus)", pOpenConn);
                        InsertIntoDb.Parameters.Add(new SqlParameter("id", id));
                        InsertIntoDb.Parameters.Add(new SqlParameter("username", username.Trim()));
                        InsertIntoDb.Parameters.Add(new SqlParameter("fname", forename.Trim()));
                        InsertIntoDb.Parameters.Add(new SqlParameter("sname", surname.Trim()));
                        InsertIntoDb.Parameters.Add(new SqlParameter("dateTimeCreated", ParsnipApi.Data.adjustedTime));
                        InsertIntoDb.Parameters.Add(new SqlParameter("accountType", accountType));
                        InsertIntoDb.Parameters.Add(new SqlParameter("accountStatus", accountStatus));

                        InsertIntoDb.ExecuteNonQuery();

                        Debug.WriteLine(String.Format("Successfully inserted account \"{0}\" into database: ", username));
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Failed to insert account into database: " + e);
                    return false;
                }

                return DbUpdate(pOpenConn);
            }
            else
            {
                throw new InvalidOperationException("Account cannot be inserted. The account's properties: username, fname & sname, must be initialised before it can be inserted!");
            }
        }
    }
}
