﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Web;
using System.Diagnostics;
using ParsnipData.Logging;
using ParsnipData.Cookies;
using ParsnipData;

namespace ParsnipData.Accounts
{
    public class User
    {
        #region Properties
        private int _id;
        public int Id { get { return _id; } private set { _id = value; } }
        private string _username;
        public string Username { get { return _username; } set { _username = value; } }
        private string _email;
        public string Email { get { return _email; } set { _email = value; } }
        private string _pwd;
        public string Password { get { return _pwd; } set { _pwd = value; } }
        private string _forename;
        public string Forename { get { return _forename; } set { _forename = value; } }
        private string _surname;
        public string Surname { get { return _surname; } set { _surname = value; } }
        private DateTime _dob;
        public DateTime Dob { get { return _dob; } set { /*Debug.WriteLine(string.Format("----------dob is being set to = {0}", value));*/ _dob = value; } }
        private string _gender;
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
        public bool HasPluralPronoun { get { return SubjectiveGenderPronoun == "they"; } }
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
        private string _address1;
        public string Address1 { get { return _address1; } set { /*Debug.WriteLine(string.Format("----------address1 is being set to = {0}", value));*/ _address1 = value; } }
        private string _address2;
        public string Address2 { get { return _address2; } set { /*Debug.WriteLine(string.Format("----------address2 is being set to = {0}", value));*/ _address2 = value; } }
        private string _address3;
        public string Address3 { get { return _address3; } set { /*Debug.WriteLine(string.Format("----------address3 is being set to = {0}", value));*/ _address3 = value; } }
        private string _postCode;
        public string PostCode { get { return _postCode; } set { /*Debug.WriteLine(string.Format("----------postCode is being set to = {0}", value));*/ _postCode = value; } }
        private string _mobilePhone;
        public string MobilePhone { get { return _mobilePhone; } set { /*Debug.WriteLine(string.Format("----------mobilePhone is being set to = {0}", value));*/ _mobilePhone = value; } }
        private string _homePhone;
        public string HomePhone { get { return _homePhone; } set { /*Debug.WriteLine(string.Format("----------homePhone is being set to = {0}", value));*/ _homePhone = value; } }
        private string _workPhone;
        public string WorkPhone { get { return _workPhone; } set { /*Debug.WriteLine(string.Format("----------workPhone is being set to = {0}", value));*/ _workPhone = value; } }
        private DateTime _dateTimeCreated;
        public DateTime DateTimeCreated { get { return _dateTimeCreated; } set { /*Debug.WriteLine(string.Format("----------dateTimeCreated is being set to = {0}", value));*/ _dateTimeCreated = value; } }
        private DateTime _lastLogIn;
        public DateTime LastLogIn { get { return _lastLogIn; } set { /*Debug.WriteLine(string.Format("----------lastLogIn is being set to = {0}", value));*/ _lastLogIn = value; } }
        private string _accountType;
        public string AccountType { get { return _accountType; } set { /*Debug.WriteLine(string.Format("----------accountType is being set to = {0}", value));*/ _accountType = value; } }
        private string _accountStatus;
        public string AccountStatus { get { return _accountStatus; } set { /*Debug.WriteLine(string.Format("----------accountStatus is being set to = {0}", value));*/ _accountStatus = value; } }
        private int _createdByUserId;
        public int createdByUserId { get { return _createdByUserId; } set { /*Debug.WriteLine(string.Format("----------createdByUserId is being set to = {0}", value));*/ _createdByUserId = value; } }
        public string FullName { get { return string.IsNullOrEmpty(Forename) ? "Someone who was not logged in" : string.Format("{0} {1}", Forename, Surname); } }
        public List<string> ValidationErrors { get;  set; }
        private string _searchTerms;
        public string SearchTerms { get { return _searchTerms; } set { _searchTerms = string.IsNullOrEmpty(value) ? null : Parsnip.SanitiseSearchString(value); } }
        #endregion

        #region Constructors
        public User(string pWhereAmI)
        {
            DateTimeCreated = Parsnip.AdjustedTime;
        }

        public User(SqlDataReader pReader)
        {
            //Debug.WriteLine("User was initialised with an SqlDataReader. Guid: " + pReader[0]);
            AddValues(pReader);
        }

        public User()
        {

        }
        #endregion

        #region Static Methods
        public static List<User> GetAllUsers()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all users...");

            var users = new List<User>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                using (SqlCommand GetUsers = new SqlCommand("user_select", conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = GetUsers.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new Accounts.User(reader));
                        }
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

        public bool Exists()
        {
            return Exists(Id);
        }

        public static bool Exists(int id)
        {
            if (id == default) return false; else return true;
        }

        public static User LogIn()
        {
            var username = string.IsNullOrEmpty(Cookie.Read("persistentUsername")) ? Cookie.Read("sessionUsername") : Cookie.Read("persistentUsername");
            var password = string.IsNullOrEmpty(Cookie.Read("persistentPassword")) ? Cookie.Read("sessionPassword") : Cookie.Read("persistentPassword");
            return LogIn(username, UsernameRemembered(), password, PasswordRemembered());
        }

        public static User LogIn(string username, string password)
        {
            return LogIn(username, UsernameRemembered(), password, PasswordRemembered());
        }

        public static User LogIn(string username, bool rememberUsername, string password, bool rememberPassword)
        {
            using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                User user = byUsername();

                if(user == null)
                    user = byEmail();

                if (user != null)
                {
                    if (rememberUsername)
                        Cookie.WritePerm("persistentUsername", username);
                    else
                        Cookie.WriteSession("sessionUsername", username);

                    if (rememberPassword)
                        Cookie.WritePerm("persistentPassword", password);
                    else
                        Cookie.WriteSession("sessionPassword", password);
                }

                return user;

                User byUsername()
                {
                    try
                    {
                        using (var loginWithUsername = new SqlCommand("user_login_where_username", conn))
                        {
                            loginWithUsername.CommandType = System.Data.CommandType.StoredProcedure;
                            loginWithUsername.Parameters.Add(new SqlParameter("username", username));
                            loginWithUsername.Parameters.Add(new SqlParameter("password", password));
                            loginWithUsername.Parameters.Add(new SqlParameter("datetime_now", Parsnip.AdjustedTime));

                            conn.Open();

                            using (SqlDataReader reader = loginWithUsername.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    //WriteSessionCookies(username, password);
                                    return new User(reader);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"There was an exception whilst logging in by username: {ex} : {ex.Message} {ex.InnerException}");
                    }
                    return null;
                }
                
                User byEmail()
                {
                    try
                    {
                        using (var loginWithEmail = new SqlCommand("user_login_where_email", conn))
                        {

                            loginWithEmail.CommandType = System.Data.CommandType.StoredProcedure;
                            loginWithEmail.Parameters.Add(new SqlParameter("email", username));
                            loginWithEmail.Parameters.Add(new SqlParameter("password", password));
                            loginWithEmail.Parameters.Add(new SqlParameter("datetime_now", Parsnip.AdjustedTime));

                            using (SqlDataReader reader = loginWithEmail.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    return new User(reader);
                                }
                            }
                        }
                    }
                    catch (Exception emailEx)
                    {
                        Debug.WriteLine("[LogIn] Email not found, login failed: " + emailEx);
                    }
                    return null;
                }
            }
        }

        /*
        public static void WritePermanentCookies(string username, string password)
        {
            Cookie.WriteSession("persistentUsername", username);
            Cookie.WriteSession("persistentPassword", password);
        }

        public static void WriteSessionCookies(string username, string password)
        {
            Cookie.WriteSession("sessionUsername", username);
            Cookie.WriteSession("sessionPassword", password);
        }
        */

        public static bool LogOut()
        {
            try
            {
                Cookie.WriteSession("persistentUsername", null);
                Cookie.WriteSession("persistentPassword", null);
                Cookie.WriteSession("sessionUsername", null);
                Cookie.WriteSession("sessionPassword", null);
                Cookie.WriteSession("accountType", null);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Public Methods

        public bool Insert()
        {
            bool success;
            using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                success = DbInsert(Password, conn);
            }
            return success;
        }

        public bool Delete()
        {
            using(var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                return DbDelete(conn);
            }
            
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

            if(!validateSuccess)
                new LogEntry(Log.General) { Text = $"{FullName} {validateSuccessString}" };

            return validateSuccess;

            bool validateUsername()
            {
                if (Username.Length == 0)
                {
                    new LogEntry(Log.General) { Text = $"Cannot create a user without a username! Username: {Username}" };
                    ValidationErrors.Add("username cannot be blank");
                    return false;
                }
                else if (Username.Length > 50)
                {
                    new LogEntry(Log.General) { Text = $"Username is {Username.Length} characters long. Username must be no longer than 50 characters!" };
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
                                new LogEntry(Log.General) { Text = $"Email address \"{EmailAddress}\" does not contain a dot. Email addresses must contain a dot." };
                                ValidationErrors.Add("email address must contain a dot");
                                return false;
                            }
                        }
                        else
                        {
                            new LogEntry(Log.General) { Text = $"Email address \"{EmailAddress}\" contains too many '@' signs. Email addresses must contain only one '@' sign." };
                            ValidationErrors.Add("email address cannot contain more than one @");
                            return false;
                        }
                    }
                    else
                    {
                        new LogEntry(Log.General) { Text = $"Email address \"{EmailAddress}\" does not contain an '@' sign. Email addresses must contain an '@' sign." };
                        ValidationErrors.Add("email address must contain at least one @");
                        return false;
                    }
                }
                else
                {
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
                        new LogEntry(Log.General) { Text = $"Password \"{Password.Trim()}\" is too short. Passwords must be at least 5 characters long." };
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
                    new LogEntry(Log.General) { Text = "Forename cannot be left blank!" };
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
                    new LogEntry(Log.General) { Text = "Surname cannot be left blank!" };
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
                        new LogEntry(Log.General) { Text = $"Gender \"{ tempGender}\" is not M, F or O. Gender must be M, F or O." };
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

        
        #endregion

        #region Other Private / Internal Methods
        internal bool AddValues(SqlDataReader pReader)
        {
            try
            {
                Id = (int) pReader[0];
                Username = pReader[1].ToString().Trim();

                if (pReader[2] != DBNull.Value)
                    Email = pReader[2].ToString();

                Password = pReader[3].ToString().Trim();
                Forename = pReader[4].ToString().Trim();
                Surname = pReader[5].ToString().Trim();

                if (pReader[6] != DBNull.Value)
                    Dob = Convert.ToDateTime(pReader[6]);

                if (pReader[7] != DBNull.Value && pReader[7].ToString() != "")
                    GenderLower = pReader[7].ToString();
                
                if (pReader[8] != DBNull.Value)
                    Address1 = pReader[8].ToString().Trim();
                
                if (pReader[9] != DBNull.Value)
                    Address2 = pReader[9].ToString().Trim();
                
                if (pReader[10] != DBNull.Value)
                    Address3 = pReader[10].ToString().Trim();
                
                if (pReader[11] != DBNull.Value)
                    PostCode = pReader[11].ToString().Trim();
                
                if (pReader[12] != DBNull.Value)
                    MobilePhone = pReader[12].ToString().Trim();
                
                if (pReader[13] != DBNull.Value)
                    HomePhone = pReader[13].ToString().Trim();
                
                if (pReader[14] != DBNull.Value)
                    WorkPhone = pReader[14].ToString().Trim();

                DateTimeCreated = Convert.ToDateTime(pReader[15]);
                
                if (pReader[16] != DBNull.Value)
                    LastLogIn = Convert.ToDateTime(pReader[16]);

                //Debug.WriteLine($"{pReader[17].ToString()}, {pReader[18].ToString()}");

                AccountType = pReader[17].ToString().Trim();
                AccountStatus = pReader[18].ToString().Trim();

                if (pReader.FieldCount > 21 && pReader[21] != DBNull.Value &&
                        !string.IsNullOrEmpty(pReader[21].ToString()) &&
                        !string.IsNullOrWhiteSpace(pReader[21].ToString()))
                {
                    SearchTerms = pReader[21].ToString().Trim();
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst reading the User's values: ", e);
                return false;
            }
        }

        public static bool UsernameRemembered()
        {
            if (string.IsNullOrEmpty(Cookie.Read("persistentUsername")))
                return false;
            else
                return true;
        }

        public static bool PasswordRemembered()
        {
            if (string.IsNullOrEmpty(Cookie.Read("persistentPassword")))
                return false;
            else
                return true;
            
        }

        private bool DbInsert(string pPwd, SqlConnection pOpenConn)
        {
            if (Username != null && Forename != null && Surname != null)
            {
                try
                {
                    {
                        using (SqlCommand InsertIntoDb = new SqlCommand("user_insert", pOpenConn))
                        {
                            InsertIntoDb.CommandType = System.Data.CommandType.StoredProcedure;

                            InsertIntoDb.Parameters.Add(new SqlParameter("username", Username.Trim()));
                            InsertIntoDb.Parameters.Add(new SqlParameter("forename", Forename.Trim()));
                            InsertIntoDb.Parameters.Add(new SqlParameter("surname", Surname.Trim()));
                            InsertIntoDb.Parameters.Add(new SqlParameter("datetime_created", Parsnip.AdjustedTime));
                            InsertIntoDb.Parameters.Add(new SqlParameter("type", AccountType));
                            InsertIntoDb.Parameters.Add(new SqlParameter("status", AccountStatus));
                            InsertIntoDb.Parameters.Add(new SqlParameter("email", Email));
                            InsertIntoDb.Parameters.Add(new SqlParameter("password", Password));
                            InsertIntoDb.Parameters.Add(new SqlParameter("gender", GenderUpper));
                            if (Dob != default)
                                InsertIntoDb.Parameters.Add(new SqlParameter("dob", Dob));
                            InsertIntoDb.Parameters.Add(new SqlParameter("address_1", Address1));
                            InsertIntoDb.Parameters.Add(new SqlParameter("address_2", Address2));
                            InsertIntoDb.Parameters.Add(new SqlParameter("address_3", Address3));
                            InsertIntoDb.Parameters.Add(new SqlParameter("post_code", PostCode));
                            InsertIntoDb.Parameters.Add(new SqlParameter("mobile_phone", MobilePhone));
                            InsertIntoDb.Parameters.Add(new SqlParameter("home_phone", HomePhone));
                            InsertIntoDb.Parameters.Add(new SqlParameter("work_phone", WorkPhone));

                            InsertIntoDb.ExecuteNonQuery();

                            Debug.WriteLine(String.Format("Successfully inserted account \"{0}\" into database: ", Username));
                        }
                    }       
                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.User.DbInsert)] Failed to insert {0}'s account into the database: {1}", FullName, e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.General) { Text = error };
                    return false;
                }
                return DbUpdate(pOpenConn);
            }
            else
            {
                throw new InvalidOperationException("Account cannot be inserted. The account's properties: username, fname & sname, must be initialised before it can be inserted!");
            }
        }

        public static User Select(int userId)
        {
            try
            {
                using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (SqlCommand selectAccount = new SqlCommand("user_select_where_id", conn))
                    {
                        selectAccount.CommandType = System.Data.CommandType.StoredProcedure;
                        selectAccount.Parameters.Add(new SqlParameter("id", userId));

                        conn.Open();
                        using (SqlDataReader reader = selectAccount.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var myUser = new User();
                                myUser.AddValues(reader);
                                return myUser;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst getting user data: " + e);
            }
            return null;
        }

        public bool Update()
        {
            bool success = false;
            using(var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                success = DbUpdate(conn);
            }

            return success;
        }

        private bool DbUpdate(SqlConnection pOpenConn)
        {
            Debug.WriteLine("Attempting to update user with Id = " + Id);
            bool HasBeenInserted = true;

            if (HasBeenInserted)
            {
                try
                {
                    using(var updateUser = new SqlCommand("user_update", pOpenConn))
                    {
                        updateUser.CommandType = System.Data.CommandType.StoredProcedure;
                        updateUser.Parameters.Add(new SqlParameter("id", Id));
                        updateUser.Parameters.Add(new SqlParameter("username", Username));
                        updateUser.Parameters.Add(new SqlParameter("email", Email));
                        updateUser.Parameters.Add(new SqlParameter("password", Password));
                        updateUser.Parameters.Add(new SqlParameter("forename", Forename));
                        updateUser.Parameters.Add(new SqlParameter("surname", Surname));
                        updateUser.Parameters.Add(new SqlParameter("gender", GenderUpper));
                        if(Dob != default)
                            updateUser.Parameters.Add(new SqlParameter("dob", Dob));
                        updateUser.Parameters.Add(new SqlParameter("address_1", Address1));
                        updateUser.Parameters.Add(new SqlParameter("address_2", Address2));
                        updateUser.Parameters.Add(new SqlParameter("address_3", Address3));
                        updateUser.Parameters.Add(new SqlParameter("post_code", PostCode));
                        updateUser.Parameters.Add(new SqlParameter("mobile_phone", MobilePhone));
                        updateUser.Parameters.Add(new SqlParameter("home_phone", HomePhone));
                        updateUser.Parameters.Add(new SqlParameter("work_phone", WorkPhone));
                        updateUser.Parameters.Add(new SqlParameter("type", AccountType));
                        updateUser.Parameters.Add(new SqlParameter("status", AccountStatus));

                        updateUser.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.User.DbUpdate] There was an error whilst updating {0}'s account ({1}): {2}", FullName, Username, e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.General) { Text = error };
                    return false;
                }
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
                Debug.WriteLine(string.Format("Setting account with id = '{0}' to deleted", Id));
                using (SqlCommand deleteAccount = new SqlCommand("user_DELETE_WHERE_id", pOpenConn))
                {
                    deleteAccount.CommandType = System.Data.CommandType.StoredProcedure;

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
