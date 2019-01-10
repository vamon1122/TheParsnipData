﻿using System;
using UacApi;
using LogApi;
using ParsnipApi;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace TheParsnipWeb
{
    public partial class UserForm1 : System.Web.UI.UserControl
    {
        private User _dataSubject;
        public User dataSubject { get { return _dataSubject; } set { Debug.WriteLine(string.Format("dataSubject (id = \"{0}\") was set in UserForm", value.id)); _dataSubject = value; UpdateForm(); } }
        string formType = "Insert";

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public UserForm1()
        {
            
            if (_dataSubject == null)
            {
                Debug.WriteLine("----------UserForm1 was initialised without a dataSubject");
                _dataSubject = new User(Guid.Empty);
            }
            else
            {
                Debug.WriteLine("----------_dataSubject was already initialised");
            }
        }

        public void UpdateForm()
        {
            Debug.WriteLine(string.Format("----------Userform is being updated. Name: {0} Id: {1}", dataSubject.fullName, dataSubject.id));

            Debug.WriteLine(string.Format("----------{0} != {1}", dataSubject.id.ToString(), Guid.Empty.ToString()));

            

            //Debug.WriteLine("----------UpdateForm()");
            //Debug.WriteLine("----------username = " + username.Text);
            //Debug.WriteLine("----------dataSubject.username = " + dataSubject.username);
            //Debug.WriteLine("----------dataSubject.id = " + dataSubject.id);
            username.Text = dataSubject.username;
            email.Text = dataSubject.email;
            password1.Text = dataSubject.pwd;
            password2.Text = dataSubject.pwd;
            forename.Text = dataSubject.forename;
            surname.Text = dataSubject.surname;
            gender.Value = dataSubject.Gender;
            if (dataSubject.dob.ToString("dd/MM/yyyy") != "01/01/0001")
                dobInput.Value = dataSubject.dob.ToString("dd/MM/yyyy");
            else
                dobInput.Value = "";
            address1.Text = dataSubject.address1;
            address2.Text = dataSubject.address2;
            address3.Text = dataSubject.address3;
            postCode.Text = dataSubject.postCode;
            mobilePhone.Text = dataSubject.mobilePhone;
            homePhone.Text = dataSubject.homePhone;
            workPhone.Text = dataSubject.workPhone;
            dateTimeCreated.Attributes.Remove("placeholder");
            dateTimeCreated.Attributes.Add("placeholder", dataSubject.dateTimeCreated.Date.ToString("dd/MM/yyyy"));
            accountType.Value = dataSubject.accountType;
            accountStatus.Value = dataSubject.accountStatus;
            if (dataSubject.dateTimeCreated.ToString("dd/MM/yyyy") == "01/01/0001")
            {
                //Debug.WriteLine(string.Format("{0}'s datetimecreated {1} == 01/01/0001", dataSubject.fullName, dataSubject.dateTimeCreated.ToString("dd/MM/yyyy")));
                dateTimeCreated.Value = ParsnipApi.Data.adjustedTime.ToString("dd/MM/yyyy");
            }
            else
            {
                //Debug.WriteLine(string.Format("{0}'s dob {1} != 01/01/0001",dataSubject.fullName, dataSubject.dateTimeCreated.ToString("dd/MM/yyyy")));
                dateTimeCreated.Value = dataSubject.dateTimeCreated.ToString("dd/MM/yyyy");
            }
                
        }

        public void UpdateFormAccount()
        {
            /*if (CookieApi.Cookie.Exists("adminUserFormSubjectId"))
            {
                if (CookieApi.Cookie.Read("adminUserFormSubjectId").Length > 0)
                {
                    dataSubject = new User(new Guid(CookieApi.Cookie.Read("adminUserFormSubjectId")));
                    dataSubject.Select();
                }
            }*/
            if (dataSubject == null)
            {
                System.Diagnostics.Debug.WriteLine("My dataSubject is null. Adding new dataSubject");
                dataSubject = new User("UpdateFormAccount (UserForm1)");
            }
            Debug.WriteLine(string.Format("username.Text = {0}", username.Text));
            Debug.WriteLine(string.Format("forename.Text = {0}", forename.Text));
            dataSubject.username = username.Text;
            Debug.WriteLine(string.Format("dataSubject.Username = username.Text ({0})", username.Text));

            dataSubject.email = email.Text;
            dataSubject.pwd = password1.Text;
            dataSubject.forename = forename.Text;
            dataSubject.surname = surname.Text;
            dataSubject.Gender = gender.Value.Substring(0, 1);
            Debug.WriteLine("DOB = " + dobInput.Value);

            
            if (DateTime.TryParse(dobInput.Value, out DateTime result))
                dataSubject.dob = Convert.ToDateTime(dobInput.Value);
            dataSubject.address1 = address1.Text;
            dataSubject.address2 = address2.Text;
            dataSubject.address3 = address3.Text;
            dataSubject.postCode = postCode.Text;
            dataSubject.mobilePhone = mobilePhone.Text;
            dataSubject.homePhone = homePhone.Text;
            dataSubject.workPhone = workPhone.Text;
            dataSubject.dateTimeCreated = ParsnipApi.Data.adjustedTime;
            dataSubject.accountType = accountType.Value;
            dataSubject.accountStatus = accountStatus.Value;
            dataSubject.accountType = accountType.Value;

        }

        
    }
}