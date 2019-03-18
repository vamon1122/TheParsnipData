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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using ParsnipApiDataAccess;

namespace UacApi
{
    public class User
    {
        static HttpClient client;
        private static string apiUrl = "api/users/";
        static readonly Log DebugLog = new Log("Debug");

        #region Properties
        public Guid _id;
        public string _username;
        public Guid Id { get; set; }
        public string Username { get; set; }

        public string _email;
        public string _pwd;
        public string _forename;
        public string _surname;
        public DateTime _dob;
        public string _gender;
        public string _address1;
        public string _address2;
        public string _address3;
        public string _postCode;
        public string _mobilePhone;
        public string _homePhone;
        public string _workPhone;
        public DateTime _dateTimeCreated;
        public DateTime _lastLogIn;
        public string _accountType;
        public string _accountStatus;
        public Guid _createdByUserId;

        private LogWriter AccountLog;
        //public Guid Id { get { return _id; } private set { /*Debug.WriteLine(string.Format("----------{0}'s id is being set to = {1}",_id, value));*/ _id = value; } }
        //public string Username { get { return _username; } set { /*Debug.WriteLine(string.Format("----------username is being set to = {0}", value));*/ _username = value; } }
        public string Email { get { return _email; } set { /*Debug.WriteLine(string.Format("----------email is being set to = {0}", value));*/ _email = value; } }
        public string Password { get { return _pwd; } set { /*Debug.WriteLine(string.Format("----------pwd is being set to = {0}", value));*/ _pwd = value; } }
        public string Forename { get { return _forename; } set { /*Debug.WriteLine(string.Format("----------forename is being set to = {0}", value));*/ _forename = value; } }
        public string Surname { get { return _surname; } set { /*Debug.WriteLine(string.Format("----------surname is being set to = {0}", value));*/ _surname = value; } }
        public DateTime Dob { get { return _dob; } set { /*Debug.WriteLine(string.Format("----------dob is being set to = {0}", value));*/ _dob = value; } }
        public string GenderUpper
        {
            get
            {
                if (_gender == null)
                    return null;

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
                GenderLower = value;
            }
        }
        public string GenderLower
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
        public string PosessivePronoun
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
        public string SubjectiveGenderPronoun
        {
            get
            {
                if (_gender == "M")
                    return "he";

                else if (_gender == "F")
                    return "she";
                else
                    return "they";
            }
        }
        public string ObjectiveGenderPronoun
        {
            get
            {
                if (_gender == "M")
                    return "him";

                else if (_gender == "F")
                    return "her";
                else
                    return "them";
            }
        }
        public string Address1 { get { return _address1; } set { /*Debug.WriteLine(string.Format("----------address1 is being set to = {0}", value));*/ _address1 = value; } }
        public string Address2 { get { return _address2; } set { /*Debug.WriteLine(string.Format("----------address2 is being set to = {0}", value));*/ _address2 = value; } }
        public string Address3 { get { return _address3; } set { /*Debug.WriteLine(string.Format("----------address3 is being set to = {0}", value));*/ _address3 = value; } }
        public string PostCode { get { return _postCode; } set { /*Debug.WriteLine(string.Format("----------postCode is being set to = {0}", value));*/ _postCode = value; } }
        public string MobilePhone { get { return _mobilePhone; } set { /*Debug.WriteLine(string.Format("----------mobilePhone is being set to = {0}", value));*/ _mobilePhone = value; } }
        public string HomePhone { get { return _homePhone; } set { /*Debug.WriteLine(string.Format("----------homePhone is being set to = {0}", value));*/ _homePhone = value; } }
        public string WorkPhone { get { return _workPhone; } set { /*Debug.WriteLine(string.Format("----------workPhone is being set to = {0}", value));*/ _workPhone = value; } }
        public DateTime DateTimeCreated { get { return _dateTimeCreated; } set { /*Debug.WriteLine(string.Format("----------dateTimeCreated is being set to = {0}", value));*/ _dateTimeCreated = value; } }
        public DateTime LastLogIn { get { return _lastLogIn; } set { /*Debug.WriteLine(string.Format("----------lastLogIn is being set to = {0}", value));*/ _lastLogIn = value; } }
        public string AccountType { get { return _accountType; } set { /*Debug.WriteLine(string.Format("----------accountType is being set to = {0}", value));*/ _accountType = value; } }
        public string AccountStatus { get { return _accountStatus; } set { /*Debug.WriteLine(string.Format("----------accountStatus is being set to = {0}", value));*/ _accountStatus = value; } }
        public Guid createdByUserId { get { return _createdByUserId; } set { /*Debug.WriteLine(string.Format("----------createdByUserId is being set to = {0}", value));*/ _createdByUserId = value; } }
        public string FullName { get { return string.Format("{0} {1}", Forename, Surname); } }
        public List<string> ValidationErrors { get;  set; }
        #endregion

        #region Constructors

        public User(string pWhereAmI) : this()
        {
            //Debug.WriteLine(string.Format("User was initialised without a guid. WhereAmI = {0} Their guid will be: {1}", pWhereAmI, Guid.Empty));
            Id = Guid.Empty;
            
            DateTimeCreated = Parsnip.adjustedTime;
        }

        public User(Guid pGuid) : this()
        {
            //Debug.WriteLine("User was initialised with the guid: " + pGuid);
            Id = pGuid;
        }

        public User(SqlDataReader pReader) : this()
        {
            //Debug.WriteLine("User was initialised with an SqlDataReader. Guid: " + pReader[0]);
            AddValues(pReader);
        }

        public User(t_Users user)
        {
            AccountLog = new LogWriter("Account Object.txt", AppDomain.CurrentDomain.BaseDirectory);
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:59622/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/xml"));
            //Debug.WriteLine("User was initialised with an SqlDataReader. Guid: " + pReader[0]);
            AddValues(user);
        }

        private User()
        {
            AccountLog = new LogWriter("Account Object.txt", AppDomain.CurrentDomain.BaseDirectory);
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:59622/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/xml"));
        }
        #endregion

        #region Static Methods
        //Moved to users controller
        /*
        public static List<User> GetAllUsers()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all users...");

            var users = new List<User>();
            using (SqlConnection conn = Parsnip.GetOpenDbConnection())
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

            foreach (User temp in users)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found user {0} with id {1}", temp.FullName, temp.Id));
            }

            return users;
        }

        public static User GetLoggedInUser(string pUsername, string pPwd)
        {
            User tempUser = new User();
            tempUser.LogIn(pUsername, false, pPwd, false, true);
            return tempUser;
        }

        public static User LogIn(string pUsername, string pPassword)
        {
            using (var openConn = Parsnip.GetOpenDbConnection())
            {
                try
                {
                    SqlCommand getId = new SqlCommand("SELECT * FROM t_Users WHERE username = @username AND password = @password", openConn);
                    getId.Parameters.Add(new SqlParameter("username", pUsername));

                    using (SqlDataReader reader = getId.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new User(reader);
                        }
                    }
                    throw new InvalidOperationException("There is no user with this username / password combination");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[LogIn] There was an exception whilst getting the id from the database: " + e);
                    throw new InvalidOperationException("There was an error whilst finding a user with username / password combination");
                }
            }
        }
        */
        public static bool LogOut()
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
        #endregion

        #region LogIn / LogOut


        internal bool LogIn(string pUsername)
        {
            Username = pUsername;
            return DbSelect(Parsnip.GetOpenDbConnection());
        }

        

        
        #endregion

        #region Public Methods

        public async Task<User> LogIn()
        {
           return await LogIn(true);
        }

        public async Task<User> LogIn(bool silent)
        {
            string[] Cookies = GetCookies();
            string CookieUsername = Cookies[0];
            Username = Cookies[0];
            string CookiePwd = Cookies[1];
            if (string.IsNullOrEmpty(CookiePwd))
            {
                return new User() { Id = Guid.Empty };
            }


            //Debug.WriteLine("CookieUsername = " + CookieUsername);
            //Debug.WriteLine("CookiePwd = " + CookiePwd);

            
                return await LogIn(CookieUsername, false, CookiePwd, false, silent);
                

            
        }

        public async Task<User> LogIn(string pUsername, bool pRememberUsername, string pPwd, bool pRememberPwd)
        {
            return await LogIn(pUsername, pRememberUsername, pPwd, pRememberPwd, true);
        }
        //LogWriter AsyncLog = new LogWriter("Asnyc_Login.txt", @"C:\Users\benba\Documents\GitHub\TheParsnipWeb");
        static readonly LogWriter AsyncLog = new LogWriter("Async_Login.txt", @"C:\Users\ben.2ESKIMOS\Documents\GitHub\TheParsnipWeb");

        public static async Task<User> LogIn(string username, string password)
        {
            try
            {
                AsyncLog.WriteLog(string.Format("[LogIn] Trying to get user data..."));
                t_Users myUserData = await GetUserAsync(username, password);
                AsyncLog.WriteLog(string.Format("[LogIn] Login for user (username={0} password={1}) was successful! Object returned = {2}. Id = {3}. Creating user object...", username, password, myUserData, myUserData.id));
                var myUser = new User(myUserData);
                AsyncLog.WriteLog(string.Format("[LogIn] Created user object.. returning"));
                return myUser;
            }
            catch (Exception e)
            {
                AsyncLog.WriteLog(string.Format("[LogIn] There was an exception whilst logging in username={0} password={1} : {2}", username, password, e));
                throw e;
            }
        }

        public async Task<User> LogIn(string pUsername, bool pRememberUsername, string pPwd, bool pRememberPwd, bool silent)
        {
            //pPwd = "BBTbbt1704";
            AsyncLog.WriteLog(string.Format("[LogIn] Attempting to log a user in. username={0} password={1}", pUsername, pPwd));
            //AccountLog.Info(String.Format("[LogIn] Logging in with Username = {0} & Pwd = {1}...",pUsername, pPwd));
            //Debug.WriteLine(string.Format("----------User.Login() for {0}", Username));

            new LogEntry(DebugLog) { text = "Logging in. pRememberPwd = " + pRememberPwd };
            
            Username = pUsername;
            //AccountLog.Debug("[LogIn] Sql connection opened succesfully!");

            try
            {
                t_Users myUserData = await GetUserAsync(pUsername, pPwd);
                AsyncLog.WriteLog(string.Format("[LogIn] Login for user (username={0} password={1}) was successful! Object returned = {2}. Id = {3}. Creating user object...", pUsername, pPwd, myUserData, myUserData.id));
                var myUser = new User(myUserData);
                AsyncLog.WriteLog(string.Format("[LogIn] Created user object.. returning"));
                return myUser;
            }
            catch(Exception e)
            {
                AsyncLog.WriteLog(string.Format("[LogIn] There was an exception whilst logging in username={0} password={1} : {2}", pUsername, pPwd, e));
                throw e;
            }

                                //Debug.WriteLine(string.Format("----------User.Login() - Selected user {0} whilst logging in", Username));
            if (pRememberUsername)
                                {
                                    //AccountLog.Debug(String.Format("[LogIn] RememberUsername = true. Writing permanent username cookie (userName = {0})", pUsername));
                                    //Debug.WriteLine("----------User.Login() - Username permanently remembered!");
                                    Cookie.WritePerm("userName", pUsername);
                                }

                                if (pRememberPwd)
                                {
                                    //AccountLog.Debug(String.Format("[LogIn] RememberPassword = true. Writing permanent password cookie (userPwd = {0})", pPwd));
                                    //Debug.WriteLine("----------User.Login() - Password permanently remembered!");
                                    Cookie.WritePerm("userPwd", pPwd);
                                    Cookie.WritePerm("userPwdPerm", pPwd);
                                    //Debug.WriteLine("----------User.Login() - PERMANENT Password cookie = " + GetCookies()[1]);
                                }
                                else
                                {
                                    //This check ensures that permanent cookies 
                                    //are not replaced with temporary ones
                                    if (!Cookie.Exists("userPwd"))
                                    {
                                        Cookie.WriteSession("userPwd", pPwd);
                                    }
                                }

                                if (SetLastLogIn())
                                {
                                    //AccountLog.Info("[LogIn] Logged in successfully!");
                                    if (!silent)
                                    {
                                        Debug.WriteLine(string.Format("----------User.Login() - {0} logged in LOUDLY", FullName));
                                    }
                                    else
                                    {
                                        //Debug.WriteLine(String.Format("----------User.Login() - {0} logged in SILENTLY", FullName));
                                    }
                                }

                bool SetLastLogIn()
                {
                    int RecordsAffected;

                    //AccountLog.Debug("[LogIn] Attempting to set LastLogIn...");
                    try
                    {
                    //AccountLog.Debug("username = " + username);
                    using (SqlConnection conn = ParsnipApi.Parsnip.GetOpenDbConnection())
                    {
                        SqlCommand Command = new SqlCommand("UPDATE t_Users SET lastLogIn = @date WHERE username = @username;", conn);
                        Command.Parameters.Add(new SqlParameter("username", Username));
                        Command.Parameters.Add(new SqlParameter("date", Parsnip.adjustedTime));
                        RecordsAffected = Command.ExecuteNonQuery();
                    }

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("[LogIn] There was an exception whilst setting the LastLogIn: " + e);
                        return false;
                    }

                    //AccountLog.Debug(String.Format("[LogIn] Set LastLogIn successfully! {0} records were affected.", RecordsAffected));
                    return true;
                }
            
        }

        public bool Select()
        {
            return DbSelect(Parsnip.GetOpenDbConnection());
        }

        public bool Update()
        {
            bool success;
            SqlConnection UpdateConnection = Parsnip.GetOpenDbConnection();
            if (ExistsOnDb(UpdateConnection)) success = DbUpdate(UpdateConnection); else success = DbInsert(Password, UpdateConnection);
            UpdateConnection.Close();
            return success;
        }

        public bool Delete()
        {
            return DbDelete(Parsnip.GetOpenDbConnection());
        }

        public bool Validate()
        {
            ValidationErrors = new List<string>();

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

            string validateSuccessString = validateSuccess ? "was validated successfully!" : "failed to be validated. See details below:";

            new LogEntry(Log.Default) { text = string.Format("{0} {1}", FullName, validateSuccessString) };

            return validateSuccess;

            bool validateUsername()
            {
                if (Username.Length == 0)
                {
                    new LogEntry(Log.Default) { text = "Cannot create a user without a username! Username: " + Username };
                    ValidationErrors.Add("username cannot be blank");
                    return false;
                }
                else if (Username.Length > 50)
                {
                    new LogEntry(Log.Default) { text = String.Format("Username is {0} characters long. Username must be no longer than 50 characters!", Username.Length) };
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
                                new LogEntry(Log.Default) { text = String.Format("Email address \"{0}\" does not contain a dot. Email addresses must contain a dot.", EmailAddress) };
                                ValidationErrors.Add("email address must contain a dot");
                                return false;
                            }
                        }
                        else
                        {
                            //MyLog.Warning("Email address contains too many @'s. Email address will be blank!");
                            new LogEntry(Log.Default) { text = String.Format("Email address \"{0}\" contains too many '@' signs. Email addresses must contain only one '@' sign.", EmailAddress) };
                            ValidationErrors.Add("email address cannot contain more than one @");
                            return false;
                        }
                    }
                    else
                    {
                        new LogEntry(Log.Default) { text = String.Format("Email address \"{0}\" does not contain an '@' sign. Email addresses must contain an '@' sign.", EmailAddress) };
                        ValidationErrors.Add("email address must contain at least one @");
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
                if (Password.Trim().Length > 0)
                {
                    if (Password.Trim().Length > 5)
                        return true;
                    else
                    {
                        new LogEntry(Log.Default) { text = String.Format("----------Password \"{0}\" is too short. Passwords must be at least 5 characters long.", Password.Trim()) };
                        ValidationErrors.Add("if you have a password, it must be at least 5 characters long");
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            bool validateForename()
            {
                if (Forename.Length > 0)
                    return true;
                else
                {
                    new LogEntry(Log.Default) { text = String.Format("----------Forename \"{0}\" cannot be left blank!", Forename.Trim()) };
                    ValidationErrors.Add("forename cannot be blank");
                    return false;
                }
            }

            bool validateSurname()
            {
                if (Surname.Length > 0)
                    return true;
                else
                {
                    new LogEntry(Log.Default) { text = String.Format("----------Surname \"{0}\" cannot be left blank!", Forename.Trim()) };
                    ValidationErrors.Add("surname cannot be blank");
                    return false;
                }
            }


            bool validateDob()
            {
                return true;
            }

            bool validateGender()
            {
                if (_gender != null)
                {
                    string tempGender = _gender.ToString().ToUpper();
                    if (tempGender == "M" || tempGender == "F" || tempGender == "O")
                        return true;
                    else
                    {
                        new LogEntry(Log.Default) { text = String.Format("Gender \"{0}\" is not M, F or O. Gender must be M, F or O.", tempGender) };
                        ValidationErrors.Add("gender must be M, F or O");
                        return false;
                    };
                }
                else
                {
                    return true;
                }

            }


            bool validateAddress1()
            {
                if (Address1.Length > 50)
                {
                    ValidationErrors.Add("address1 must be no more than 50 characters long");
                    return false;
                }
                else
                    return true;
            }

            bool validateAddress2()
            {
                if (Address2.Length > 50)
                {
                    ValidationErrors.Add("address2 must be no more than 50 characters long");
                    return false;
                }
                else
                    return true;
            }

            bool validateAddress3()
            {
                if (Address3.Length > 50)
                {
                    ValidationErrors.Add("address1 must be no more than 50 characters long");
                    return false;
                }
                else
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

        public bool ExistsOnDb()
        {
            return ExistsOnDb(Parsnip.GetOpenDbConnection());
        }


        #endregion

        #region Other Private / Internal Methods

        public static async Task<t_Users> GetUserAsync(string username, string password)
        {
            LogWriter AsyncLog = new LogWriter("Async_Login.txt", @"C:\Users\ben.2ESKIMOS\Documents\GitHub\TheParsnipWeb");

            AsyncLog.WriteLog("[GetUserAsync] Begin!");

            string path = string.Format("api/users?username={0}&password={1}", username, password);

            AsyncLog.WriteLog("[GetUserAsync] Path to get data will be = " + path);


            t_Users user;
            AsyncLog.WriteLog("[GetUserAsync] Getting response");

            if(client == null)
            {
                AsyncLog.WriteLog("[GetUserAsync] Client was NULL!!! Attempting fix...");
                client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:59622/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/xml"));
                AsyncLog.WriteLog("[GetUserAsync] Client should no longer be null.");
            }
            else
            {
                AsyncLog.WriteLog("[GetUserAsync] Client was not null.");
            }

            HttpResponseMessage response = await client.GetAsync(path);
            AsyncLog.WriteLog("[GetUserAsync] Got response!");
            if (response.IsSuccessStatusCode)
            {
                AsyncLog.WriteLog("[GetUserAsync] Response indicated success. Waiting for user to be returned...");
                user = await response.Content.ReadAsAsync<t_Users>();
                AsyncLog.WriteLog("[GetUserAsync] A user was returned! Will return the user.");
            }
            else
            {
                user = null;
                Debug.WriteLine("There was an error whilst getting the user because " + response.ReasonPhrase);
                AsyncLog.WriteLog("[GetUserAsync] Response indicated faliure! The response was an error: " + response.ReasonPhrase);
                AsyncLog.WriteLog("[GetUserAsync] User was NOT returned. Will return null.");
            }

            return user;
        }

        public static async Task<List<User>> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        static async Task<User> GetUser(string id)
        {
            throw new NotImplementedException();
        }

        public static User GetLoggedInUser(string pUsername, string pPwd)
        {
            User tempUser = new User();
            tempUser.LogIn(pUsername, false, pPwd, false, true);
            return tempUser;
        }

        private bool ExistsOnDb(SqlConnection pOpenConn)
        {
            if (IdExistsOnDb(pOpenConn) || UsernameExistsOnDb(pOpenConn))
                return true;
            else
                return false;
        }

        private bool IdExistsOnDb(SqlConnection pOpenConn)
        {
            Debug.WriteLine(string.Format("Checking weather user {0} exists on database by using {1} Id {1}", FullName, PosessivePronoun, Id));
            try
            {
                SqlCommand findMeById = new SqlCommand("SELECT COUNT(*) FROM t_Users WHERE id = @id", pOpenConn);
                findMeById.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int userExists;

                using (SqlDataReader reader = findMeById.ExecuteReader())
                {
                    reader.Read();
                    userExists = Convert.ToInt16(reader[0]);
                    //Debug.WriteLine("Found user by Id. userExists = " + userExists);
                }

                //Debug.WriteLine(userExists + " user(s) were found with the id " + Id);

                if (userExists > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst checking if user exists on the database by using thier Id: " + e);
                return false;
            }
        }

        private bool UsernameExistsOnDb(SqlConnection pOpenConn)
        {
            Debug.WriteLine(string.Format("Checking weather user {0} exists on database by using their username {1}", FullName, Username));
            try
            {
                SqlCommand findMeById = new SqlCommand("SELECT COUNT(*) FROM t_Users WHERE username = @username", pOpenConn);
                findMeById.Parameters.Add(new SqlParameter("username", Username));

                int userExists;

                using (SqlDataReader reader = findMeById.ExecuteReader())
                {
                    reader.Read();
                    userExists = Convert.ToInt16(reader[0]);
                    //Debug.WriteLine("Found user by Id. userExists = " + userExists);
                }

                Debug.WriteLine(userExists + " user(s) were found with the username " + Username);



                if (userExists > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("There was an error whilst checking if user exists on the database by using their username: " + e);
                return false;
            }
        }

        internal void AddValues(t_Users pUser)
        {
            Debug.WriteLine("Adding values from t_User");
            if (pUser == null)
            {
                Debug.WriteLine("t_User was null");
            }
            
            Id = pUser.id;
            Username = pUser.username.Trim();
            if (!string.IsNullOrEmpty(pUser.email))
                Email = pUser.email.Trim();
            Password = pUser.password.Trim();
            Forename = pUser.forename.Trim();
            Surname = pUser.surname.Trim();
            Dob = Convert.ToDateTime(pUser.dob);

            if (!string.IsNullOrEmpty(pUser.gender))
                GenderLower = pUser.gender;

            if (!string.IsNullOrEmpty(pUser.address1))
                Address1 = pUser.address1.Trim();

            if (!string.IsNullOrEmpty(pUser.address2))
                Address2 = pUser.address2.Trim();

            if (!string.IsNullOrEmpty(pUser.address3))
                Address3 = pUser.address3.Trim();

            if (!string.IsNullOrEmpty(pUser.postCode))
                PostCode = pUser.postCode.Trim();

            if (!string.IsNullOrEmpty(pUser.mobilePhone))
                MobilePhone = pUser.mobilePhone.Trim();

            if (!string.IsNullOrEmpty(pUser.homePhone))
                HomePhone = pUser.homePhone.Trim();

            if (!string.IsNullOrEmpty(pUser.workPhone))
                WorkPhone = pUser.workPhone.Trim();

            DateTimeCreated = pUser.created;
            LastLogIn = Convert.ToDateTime(pUser.lastLogIn);
            AccountType = pUser.type.Trim();
            AccountStatus = pUser.status.Trim();
            

        }

        internal bool AddValues(SqlDataReader pReader)
        {
            bool logMe = false;

            if(logMe)
                Debug.WriteLine("----------Adding values...");

            try
            {
                if (logMe)
                    Debug.WriteLine(string.Format("----------Reading id: {0}", pReader[0]));

                Id = new Guid(pReader[0].ToString());

                if (logMe)
                    Debug.WriteLine(string.Format("----------Reading username: {0}", pReader[1]));

                Username = pReader[1].ToString().Trim();
                if (pReader[2] != DBNull.Value)
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading email");

                    Email = pReader[2].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------email is blank. Skipping email");
                }


                if (logMe)
                    Debug.WriteLine("----------Reading pwd");
                Password = pReader[3].ToString().Trim();

                if (logMe)
                    Debug.WriteLine("----------Reading forename");
                Forename = pReader[4].ToString().Trim();

                if (logMe)
                    Debug.WriteLine("----------Reading surname");
                Surname = pReader[5].ToString().Trim();


                if (pReader[6] == DBNull.Value)
                {
                    if (logMe)
                        Debug.WriteLine("----------dob is blank. Skipping dob");
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading dob");
                    Dob = Convert.ToDateTime(pReader[6]);
                }

                if (pReader[7] == DBNull.Value || pReader[7].ToString() == "")
                {
                    if (logMe)
                        Debug.WriteLine("----------gender is blank. Skipping gender");
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading gender");

                    GenderLower = pReader[7].ToString();
                }
                if (pReader[8] == DBNull.Value)
                {
                    if (logMe)
                        Debug.WriteLine("----------address1 is blank. Skipping address1");
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading address1");

                    Address1 = pReader[8].ToString().Trim();
                }
                if (pReader[9] == DBNull.Value)
                {
                    if (logMe)
                        Debug.WriteLine("----------address2 is blank. Skipping address2");
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading address2");

                    Address2 = pReader[9].ToString().Trim();
                }
                if (pReader[10] == DBNull.Value)
                {
                    if (logMe)
                        Debug.WriteLine("----------address3 is blank. Skipping address3");
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading address3");

                    Address3 = pReader[10].ToString().Trim();
                }
                if (pReader[11] == DBNull.Value)
                {
                    if (logMe)
                       Debug.WriteLine("----------postCode is blank. Skipping postCode");
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading postCode");

                    PostCode = pReader[11].ToString().Trim();
                }
                if (pReader[12] == DBNull.Value)
                {
                    if (logMe)
                        Debug.WriteLine("----------mobilePhone is blank. Skipping mobilePhone");
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading mobilePhone");

                    MobilePhone = pReader[12].ToString().Trim();
                }
                if (pReader[13] == DBNull.Value)
                {
                    if (logMe)
                       Debug.WriteLine("----------homePhone is blank. Skipping homePhone");
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading homePhone");

                    HomePhone = pReader[13].ToString().Trim();
                }
                if (pReader[14] == DBNull.Value)
                {
                    if (logMe)
                       Debug.WriteLine("----------workPhone is blank. Skipping workPhone");
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading workPhone");

                    WorkPhone = pReader[14].ToString().Trim();
                }

                if (logMe)
                    Debug.WriteLine("----------Reading dateTimeCreated");

                DateTimeCreated = Convert.ToDateTime(pReader[15]);
                if (logMe)
                    Debug.WriteLine("----------dateTimeCreated = " + DateTimeCreated);

                if (pReader[16] == DBNull.Value)
                {
                    if (logMe)
                       Debug.WriteLine("----------lastLogIn is blank. Skipping lastLogIn");
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading lastLogIn");

                    LastLogIn = Convert.ToDateTime(pReader[16]);
                }

                if (logMe)
                    Debug.WriteLine(string.Format("----------Reading {0}'s accountType", FullName));

                AccountType = pReader[17].ToString().Trim();
                if (logMe)
                    Debug.WriteLine(string.Format("----------{0}'s accountType = {1}", FullName, AccountType));

                if (logMe)
                    Debug.WriteLine("----------Reading accountStatus");

                AccountStatus = pReader[18].ToString().Trim();

                if (logMe)
                    Debug.WriteLine("added values successfully!");

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst reading the User's values: ", e);
                return false;
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

        private bool DbInsert(string pPwd, SqlConnection pOpenConn)
        {
            if (Id.ToString() == Guid.Empty.ToString())
            {
                Id = Guid.NewGuid();
                Debug.WriteLine("Id was empty when trying to insert user {0} into the database. A new guid was generated: {1}", FullName, Id);
            }

            if (Username != null && Forename != null && Surname != null)
            {
                try
                {
                    if (!ExistsOnDb(pOpenConn))
                    {
                        SqlCommand InsertIntoDb = new SqlCommand("INSERT INTO t_Users (id, username, forename, surname, created, type, status) VALUES(@id, @username, @forename, @surname, @dateTimeCreated, @accountType, @accountStatus)", pOpenConn);
                        
                        InsertIntoDb.Parameters.Add(new SqlParameter("id", Id));
                        InsertIntoDb.Parameters.Add(new SqlParameter("username", Username.Trim()));
                        InsertIntoDb.Parameters.Add(new SqlParameter("forename", Forename.Trim()));
                        InsertIntoDb.Parameters.Add(new SqlParameter("surname", Surname.Trim()));
                        InsertIntoDb.Parameters.Add(new SqlParameter("dateTimeCreated", Parsnip.adjustedTime));
                        InsertIntoDb.Parameters.Add(new SqlParameter("accountType", AccountType));
                        InsertIntoDb.Parameters.Add(new SqlParameter("accountStatus", AccountStatus));

                        InsertIntoDb.ExecuteNonQuery();

                        Debug.WriteLine(String.Format("Successfully inserted account \"{0}\" into database: ", Username));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Tried to insert user {0} but they already existed on the database! Id = {1}", FullName, Id));
                    }
                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.User.DbInsert)] Failed to insert {0}'s account into the database: {1}", FullName, e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("{0} was successfully inserted into the database!", FullName) };
                return DbUpdate(pOpenConn);
            }
            else
            {
                throw new InvalidOperationException("Account cannot be inserted. The account's properties: username, fname & sname, must be initialised before it can be inserted!");
            }
        }

        internal bool DbSelect(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get user details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get user details with id {0}...", Id));
            
            try
            {
                SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_Users WHERE id = @id", pOpenConn);
                SelectAccount.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int recordsFound = 0;
                using (SqlDataReader reader = SelectAccount.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        
                        AddValues(reader);
                        recordsFound++;
                    }
                }
                //Debug.WriteLine(string.Format("----------DbSelect() - Found {0} record(s) ", recordsFound));

                if (recordsFound > 0)
                {
                    //Debug.WriteLine("----------DbSelect() - Got user's details successfully!");
                    //AccountLog.Debug("Got user's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbSelect() - No user data was returned");
                    //AccountLog.Debug("Got user's details successfully!");
                    return false;
                }
                
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst getting user data: " + e);
                return false;
            }
        }

        private bool DbUpdate(SqlConnection pOpenConn)
        {
            Debug.WriteLine("Attempting to update user with Id = " + Id);
            bool HasBeenInserted = true;

            if (HasBeenInserted)
            {
                try
                {
                    User temp = new User(Id);
                    temp.Select();

                    if (Username != temp.Username)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update {0}'s username...", temp.FullName));
                        if (string.IsNullOrEmpty(Username))
                        {
                            string e = string.Format("The username which was supplied for {0} was null or empty", temp.FullName);
                            Debug.WriteLine("----------{0}. An exception will be thrown since username is a mandatory field", e);
                            throw new InvalidCastException(e);
                        }


                        SqlCommand UpdateUsername = new SqlCommand("UPDATE t_Users SET username = @username WHERE id = @id", pOpenConn);
                        UpdateUsername.Parameters.Add(new SqlParameter("id", Id));
                        UpdateUsername.Parameters.Add(new SqlParameter("username", Username));

                        UpdateUsername.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s username was updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s username was not changed. Not updating {0}'s username.", temp.FullName));
                    }


                    if (Email != temp.Email || Email == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s email...", temp.FullName));

                        SqlCommand UpdateEmail = new SqlCommand("UPDATE t_Users SET email = @email WHERE id = @id", pOpenConn);

                        UpdateEmail.Parameters.Add(new SqlParameter("id", Id));
                        if (Email == "")
                        {
                            UpdateEmail.Parameters.Add(new SqlParameter("email", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s email will be set to NULL in the database", temp.FullName));
                        }
                        else
                            UpdateEmail.Parameters.Add(new SqlParameter("email", Email));

                        UpdateEmail.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s email was updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s email was not changed. Not updating {0}'s email", temp.FullName));
                    }

                    if (Password.Length > 0 && Password != temp.Password || Password == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s password...", temp.FullName));

                        SqlCommand UpdatePwd = new SqlCommand("UPDATE t_Users SET password = @password WHERE id = @id", pOpenConn);

                        UpdatePwd.Parameters.Add(new SqlParameter("id", Id));
                        if (Password == "")
                        {
                            UpdatePwd.Parameters.Add(new SqlParameter("password", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s password will be set to NULL in the database", temp.FullName));
                        }
                        else
                            UpdatePwd.Parameters.Add(new SqlParameter("password", Password));

                        UpdatePwd.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s password updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s password was not changed. Not updating {0}'s password.", temp.FullName));
                    }

                    if (Forename != temp.Forename)
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s forename. {1}'s forename will be changed to \"{2}\"...", temp.FullName, temp.Forename, Forename));

                        if (string.IsNullOrEmpty(Forename))
                        {
                            string e = "The forename which was supplied was null or empty";
                            Debug.WriteLine(string.Format("----------{0}. An exception will be thrown since forename is a mandatory field", e));
                            throw new InvalidCastException(e);
                        }

                        SqlCommand UpdateForename = new SqlCommand("UPDATE t_Users SET forename = @forename WHERE id = @id", pOpenConn);

                        UpdateForename.Parameters.Add(new SqlParameter("id", Id));
                        UpdateForename.Parameters.Add(new SqlParameter("forename", Forename));



                        UpdateForename.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s forename updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s forename was not changed. Not updating {0}'s forename.", temp.FullName));
                    }

                    if (Surname != temp.Surname)
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s surname. {1}'s surname will be changed to \"{2}\"...", temp.FullName, temp.Surname, Surname));

                        if (string.IsNullOrEmpty(Surname))
                        {
                            string e = "The surname which was supplied was null or empty";
                            Debug.WriteLine(string.Format("----------{0}. An exception will be thrown since surname is a mandatory field", e));
                            throw new InvalidCastException(e);
                        }

                        SqlCommand updateSurname = new SqlCommand("UPDATE t_Users SET surname = @surname WHERE id = @id", pOpenConn);

                        updateSurname.Parameters.Add(new SqlParameter("id", Id));
                        updateSurname.Parameters.Add(new SqlParameter("surname", Surname));

                        updateSurname.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s surname was updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s surname was not changed. Not updating {0}'s surname.", temp.FullName));
                    }

                    if (_gender != temp.GenderUpper || _gender == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s gender...", temp.FullName));

                        SqlCommand updateGender = new SqlCommand("UPDATE t_Users SET gender = @gender WHERE id = @id", pOpenConn);

                        updateGender.Parameters.Add(new SqlParameter("id", Id));
                        if (_gender == "")
                        {
                            updateGender.Parameters.Add(new SqlParameter("gender", DBNull.Value));
                            Debug.WriteLine(string.Format("----------gender will be set to NULL in the database"));
                        }
                        else
                            updateGender.Parameters.Add(new SqlParameter("gender", _gender));

                        updateGender.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s gender updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s gender was not changed. Not updating {0}'s gender.", temp.FullName));
                    }

                    if (Dob != temp.Dob || Dob.ToString() == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s dob...", temp.FullName));

                        SqlCommand UpdateDob = new SqlCommand("UPDATE t_Users SET dob = @dob WHERE id = @id", pOpenConn);

                        UpdateDob.Parameters.Add(new SqlParameter("id", Id));

                        if (Dob == DateTime.MinValue)
                        {
                            Debug.WriteLine(string.Format("----------{0}'s dob will be set to NULL in the database", temp.FullName));
                            UpdateDob.Parameters.Add(new SqlParameter("dob", DBNull.Value));
                        }
                        else
                            UpdateDob.Parameters.Add(new SqlParameter("dob", Dob));

                        UpdateDob.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s dob was updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s dob was not changed. Not updatg dob.", temp.FullName));
                    }

                    if (Address1 != temp.Address1 || Address1 == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s address1...", temp.FullName));

                        SqlCommand UpdateAddress1 = new SqlCommand("UPDATE t_Users SET address1 = @address1 WHERE id = @id", pOpenConn);

                        UpdateAddress1.Parameters.Add(new SqlParameter("id", Id));
                        if (Address1 == "")
                        {
                            UpdateAddress1.Parameters.Add(new SqlParameter("address1", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s address1 will be set to NULL in the database", temp.FullName));
                        }
                        else
                            UpdateAddress1.Parameters.Add(new SqlParameter("address1", Address1));

                        UpdateAddress1.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s address1 updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("{0}'s address1 was not changed. Not updating address1.", temp.FullName));
                    }

                    if (Address2 != temp.Address2 || Address2 == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s address2...", temp.FullName));

                        SqlCommand UpdateAddress2 = new SqlCommand("UPDATE t_Users SET address2 = @address2 WHERE id = @id", pOpenConn);

                        UpdateAddress2.Parameters.Add(new SqlParameter("id", Id));
                        if (Address2 == "")
                        {
                            UpdateAddress2.Parameters.Add(new SqlParameter("address2", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s address2 will be set to NULL in the database", temp.FullName));
                        }

                        else
                            UpdateAddress2.Parameters.Add(new SqlParameter("address2", Address2));

                        UpdateAddress2.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s address2 was updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s address2 was not changed. Not updating address2.", temp.FullName));
                    }

                    if (Address3 != temp.Address3 || Address3 == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s address3...", temp.FullName));

                        SqlCommand UpdateAddress3 = new SqlCommand("UPDATE t_Users SET address3 = @address3 WHERE id = @id", pOpenConn);

                        UpdateAddress3.Parameters.Add(new SqlParameter("id", Id));
                        if (Address3 == "")
                        {
                            UpdateAddress3.Parameters.Add(new SqlParameter("address3", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s address3 will be set to NULL in the database", temp.FullName));
                        }
                        else
                            UpdateAddress3.Parameters.Add(new SqlParameter("address3", Address3));

                        UpdateAddress3.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s address3 was updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s address3 was not changed. Not updating {0}'s address3.", temp.FullName));
                    }

                    if (PostCode != temp.PostCode || PostCode == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s postcode...", temp.FullName));

                        SqlCommand UpdatePostCode = new SqlCommand("UPDATE t_Users SET postCode = @postCode WHERE id = @id", pOpenConn);

                        UpdatePostCode.Parameters.Add(new SqlParameter("id", Id));
                        if (PostCode == "")
                        {
                            UpdatePostCode.Parameters.Add(new SqlParameter("postCode", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s postCode will be set to NULL in the database", temp.FullName));
                        }
                        else
                            UpdatePostCode.Parameters.Add(new SqlParameter("postCode", PostCode));

                        UpdatePostCode.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s postCode was updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s postCode was not changed. Not updating {0}'s postcode.", temp.FullName));
                    }

                    if (MobilePhone != temp.MobilePhone || MobilePhone == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s mobilePhone...", temp.FullName));

                        SqlCommand UpdateMobilePhone = new SqlCommand("UPDATE t_Users SET mobilePhone = @mobilePhone WHERE id = @id", pOpenConn);

                        UpdateMobilePhone.Parameters.Add(new SqlParameter("id", Id));
                        if (MobilePhone == "")
                        {
                            UpdateMobilePhone.Parameters.Add(new SqlParameter("mobilePhone", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s mobilePhone will be set to NULL in the database", temp.FullName));
                        }
                        else
                            UpdateMobilePhone.Parameters.Add(new SqlParameter("mobilePhone", MobilePhone));

                        UpdateMobilePhone.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s mobilePhone updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("{0}'s mobilePhone was not changed. Not updating {0}'s mobilePhone.", temp.FullName));
                    }

                    if (HomePhone != temp.HomePhone || HomePhone == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s homePhone...", temp.FullName));

                        SqlCommand UpdateHomePhone = new SqlCommand("UPDATE t_Users SET homePhone = @homePhone WHERE id = @id", pOpenConn);

                        UpdateHomePhone.Parameters.Add(new SqlParameter("id", Id));
                        if (HomePhone == "")
                        {
                            UpdateHomePhone.Parameters.Add(new SqlParameter("homePhone", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s homePhone will be set to NULL in the database", temp.FullName));
                        }
                        else
                            UpdateHomePhone.Parameters.Add(new SqlParameter("homePhone", HomePhone));

                        UpdateHomePhone.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s homePhone was updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s homePhone was not changed. Not updating {0}'s home phone.", temp.FullName));
                    }

                    if (WorkPhone != temp.WorkPhone || WorkPhone == "")
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s workPhone...", temp.FullName));

                        SqlCommand updateWorkPhone = new SqlCommand("UPDATE t_Users SET workPhone = @workPhone WHERE id = @id", pOpenConn);

                        updateWorkPhone.Parameters.Add(new SqlParameter("id", Id));
                        if (WorkPhone == "")
                        {
                            updateWorkPhone.Parameters.Add(new SqlParameter("workPhone", DBNull.Value));
                            Debug.WriteLine(string.Format("----------{0}'s workPhone will be set to NULL in the database", temp.FullName));
                        }
                        else
                            updateWorkPhone.Parameters.Add(new SqlParameter("workPhone", WorkPhone));

                        updateWorkPhone.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s workPhone was updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s workPhone was not changed. Not updating {0}'s workPhone", temp.FullName));
                    }

                    if (AccountType != temp.AccountType)
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s accountType...", temp.FullName));

                        SqlCommand updateAccountType = new SqlCommand("UPDATE t_Users SET type = @accountType WHERE id = @id", pOpenConn);

                        updateAccountType.Parameters.Add(new SqlParameter("id", Id));
                        updateAccountType.Parameters.Add(new SqlParameter("accountType", AccountType));

                        updateAccountType.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s accountType updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s accountType was not changed. Not updating {0}'s accountType.", temp.FullName));
                    }

                    if (AccountStatus != temp.AccountStatus)
                    {
                        Debug.WriteLine(string.Format("----------Updating {0}'s accountStatus...", temp.FullName));

                        SqlCommand updateAccountStatus = new SqlCommand("UPDATE t_Users SET status = @accountStatus WHERE id = @id", pOpenConn);

                        updateAccountStatus.Parameters.Add(new SqlParameter("id", Id));
                        updateAccountStatus.Parameters.Add(new SqlParameter("accountStatus", AccountStatus));

                        updateAccountStatus.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------{0}'s accountStatus updated successfully!", temp.FullName));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------{0}'s accountStatus was not changed. Not updating {0}'s accountStatus.", temp.FullName));
                    }

                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.User.DbUpdate] There was an error whilst updating {0}'s account ({1}): {2}", FullName, Username, e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("{0}'s details were successfully updated on the database!", FullName) };
                return true;
            }
            else
            {
                throw new System.InvalidOperationException("Account cannot be updated. Account must be inserted into the database before it can be updated!");
            }
        }

        internal bool DbDelete(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get user details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get user details with id {0}...", Id));

            try
            {
                SqlCommand deleteAccount = new SqlCommand("DELETE FROM t_Users WHERE id = @id", pOpenConn);
                deleteAccount.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int recordsFound = deleteAccount.ExecuteNonQuery();
                //Debug.WriteLine(string.Format("----------DbDelete() - Found {0} record(s) ", recordsFound));

                if (recordsFound > 0)
                {
                    //Debug.WriteLine("----------DbDelete() - Got user's details successfully!");
                    //AccountLog.Debug("Got user's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbDelete() - No user data was deleted");
                    //AccountLog.Debug("Got user's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst deleting user data: " + e);
                return false;
            }
        }
        #endregion
    }
}
