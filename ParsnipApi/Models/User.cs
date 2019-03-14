using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParsnipApi.Models
{
    public class User
    {
        private Guid _id;
        public Guid Id { get { return _id; } private set { /*Debug.WriteLine(string.Format("----------{0}'s id is being set to = {1}",_id, value));*/ _id = value; } }
        private string _username;
        public string Username { get { return _username; } set { /*Debug.WriteLine(string.Format("----------username is being set to = {0}", value));*/ _username = value; } }
        private string _email;
        public string Email { get { return _email; } set { /*Debug.WriteLine(string.Format("----------email is being set to = {0}", value));*/ _email = value; } }
        private string _pwd;
        public string Password { get { return _pwd; } set { /*Debug.WriteLine(string.Format("----------pwd is being set to = {0}", value));*/ _pwd = value; } }
        private string _forename;
        public string Forename { get { return _forename; } set { /*Debug.WriteLine(string.Format("----------forename is being set to = {0}", value));*/ _forename = value; } }
        private string _surname;
        public string Surname { get { return _surname; } set { /*Debug.WriteLine(string.Format("----------surname is being set to = {0}", value));*/ _surname = value; } }
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
                switch (_gender)
                {
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
                if (value.Length > 0)
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
                if (_gender == "M")
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
        private Guid _createdByUserId;
        public Guid createdByUserId { get { return _createdByUserId; } set { /*Debug.WriteLine(string.Format("----------createdByUserId is being set to = {0}", value));*/ _createdByUserId = value; } }
        public string FullName { get { return string.Format("{0} {1}", Forename, Surname); } }
        public List<string> ValidationErrors { get; set; }
    }
}