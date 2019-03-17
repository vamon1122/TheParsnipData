using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Data.SqlClient;

namespace ParsnipApi.Models
{
    public class User
    {
        
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

        public User()
        {

        }

        public User(SqlDataReader pReader)
        {
            AddValues(pReader);
        }

        internal bool AddValues(SqlDataReader pReader)
        {
            bool logMe = false;

            if (logMe)
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

                    _email = pReader[2].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------email is blank. Skipping email");
                }


                if (logMe)
                    Debug.WriteLine("----------Reading pwd");
                _pwd = pReader[3].ToString().Trim();

                if (logMe)
                    Debug.WriteLine("----------Reading forename");
                _forename = pReader[4].ToString().Trim();

                if (logMe)
                    Debug.WriteLine("----------Reading surname");
                _surname = pReader[5].ToString().Trim();


                if (pReader[6] == DBNull.Value)
                {
                    if (logMe)
                        Debug.WriteLine("----------dob is blank. Skipping dob");
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading dob");
                    _dob = Convert.ToDateTime(pReader[6]);
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

                    _gender = pReader[7].ToString();
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

                    _address1 = pReader[8].ToString().Trim();
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

                    _address2 = pReader[9].ToString().Trim();
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

                    _address3 = pReader[10].ToString().Trim();
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

                    _postCode = pReader[11].ToString().Trim();
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

                    _mobilePhone = pReader[12].ToString().Trim();
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

                    _homePhone = pReader[13].ToString().Trim();
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

                    _workPhone = pReader[14].ToString().Trim();
                }

                if (logMe)
                    Debug.WriteLine("----------Reading dateTimeCreated");

                _dateTimeCreated = Convert.ToDateTime(pReader[15]);
                if (logMe)
                    Debug.WriteLine("----------dateTimeCreated = " + _dateTimeCreated);

                if (pReader[16] == DBNull.Value)
                {
                    if (logMe)
                        Debug.WriteLine("----------lastLogIn is blank. Skipping lastLogIn");
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading lastLogIn");

                    _lastLogIn = Convert.ToDateTime(pReader[16]);
                }

                if (logMe)
                    Debug.WriteLine(string.Format("----------Reading {0}'s accountType", _forename));

                _accountType = pReader[17].ToString().Trim();
                if (logMe)
                    Debug.WriteLine(string.Format("----------{0}'s accountType = {1}", _forename, _accountType));

                if (logMe)
                    Debug.WriteLine("----------Reading accountStatus");

                _accountStatus = pReader[18].ToString().Trim();

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
    }
}