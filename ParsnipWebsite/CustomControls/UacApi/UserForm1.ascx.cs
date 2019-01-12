using System;
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
    internal static class PersistentData
    {
        internal static UserForm1 myUserForm1;
        internal static User _dataSubject;
        internal static User DataSubject { get { return _dataSubject; } set { /*Debug.WriteLine(string.Format("dataSubject (id = \"{0}\") was set in UserForm", value.Id));*/ _dataSubject = value; myUserForm1.UpdateFields(); } }
    }

    public partial class UserForm1 : System.Web.UI.UserControl
    {
        public User DataSubject { get { return PersistentData.DataSubject; } set { PersistentData.DataSubject = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public UserForm1()
        {
            PersistentData.myUserForm1 = this;
            if (PersistentData._dataSubject == null)
            {
                Debug.WriteLine("----------UserForm1 was initialised without a dataSubject");

                PersistentData._dataSubject = new User(Guid.Empty);
            }
            else
            {
                //Debug.WriteLine("----------_dataSubject was already initialised");
            }
        }

        public void UpdateFields()
        {
            Debug.WriteLine(string.Format("----------Userform fields are being updated. Name: {0} Id: {1}", PersistentData.DataSubject.FullName, PersistentData.DataSubject.Id));

            Debug.WriteLine(string.Format("----------{0} != {1}", PersistentData.DataSubject.Id.ToString(), Guid.Empty.ToString()));

            

            //Debug.WriteLine("----------UpdateForm()");
            //Debug.WriteLine("----------username = " + username.Text);
            //Debug.WriteLine("----------dataSubject.username = " + dataSubject.username);
            //Debug.WriteLine("----------dataSubject.id = " + dataSubject.id);
            username.Text = PersistentData.DataSubject.Username;
            email.Text = PersistentData.DataSubject.Email;
            password1.Text = PersistentData.DataSubject.Password;
            password2.Text = PersistentData.DataSubject.Password;
            forename.Text = PersistentData.DataSubject.Forename;
            surname.Text = PersistentData.DataSubject.Surname;
            gender.Value = PersistentData.DataSubject.GenderUpper;
            if (PersistentData.DataSubject.Dob.ToString("dd/MM/yyyy") != "01/01/0001")
                dobInput.Value = PersistentData.DataSubject.Dob.ToString("dd/MM/yyyy");
            else
                dobInput.Value = "";
            address1.Text = PersistentData.DataSubject.Address1;
            address2.Text = PersistentData.DataSubject.Address2;
            address3.Text = PersistentData.DataSubject.Address3;
            postCode.Text = PersistentData.DataSubject.PostCode;
            mobilePhone.Text = PersistentData.DataSubject.MobilePhone;
            homePhone.Text = PersistentData.DataSubject.HomePhone;
            workPhone.Text = PersistentData.DataSubject.WorkPhone;
            dateTimeCreated.Attributes.Remove("placeholder");
            dateTimeCreated.Attributes.Add("placeholder", PersistentData.DataSubject.DateTimeCreated.Date.ToString("dd/MM/yyyy"));
            accountType.Value = PersistentData.DataSubject.AccountType;
            accountStatus.Value = PersistentData.DataSubject.AccountStatus;
            if (PersistentData.DataSubject.DateTimeCreated.ToString("dd/MM/yyyy") == "01/01/0001")
            {
                //Debug.WriteLine(string.Format("{0}'s datetimecreated {1} == 01/01/0001", dataSubject.fullName, dataSubject.dateTimeCreated.ToString("dd/MM/yyyy")));
                dateTimeCreated.Value = ParsnipApi.Data.adjustedTime.ToString("dd/MM/yyyy");
            }
            else
            {
                //Debug.WriteLine(string.Format("{0}'s dob {1} != 01/01/0001",dataSubject.fullName, dataSubject.dateTimeCreated.ToString("dd/MM/yyyy")));
                dateTimeCreated.Value = PersistentData.DataSubject.DateTimeCreated.ToString("dd/MM/yyyy");
            }
                
        }

        public void UpdateDataSubject()
        {
            if (PersistentData.DataSubject == null)
            {
                Debug.WriteLine("My dataSubject is null. Adding new dataSubject");
                PersistentData.DataSubject = new User("UpdateDataSubject (UserForm1)");
            }
            /*
            Debug.WriteLine(string.Format("username.Text = {0}", username.Text));
            Debug.WriteLine(string.Format("forename.Text = {0}", forename.Text));
            Debug.WriteLine(string.Format("mobilePhone.Text = {0}", mobilePhone.Text));
            Debug.WriteLine(string.Format("homePhone.Text = {0}", homePhone.Text));
            Debug.WriteLine(string.Format("workPhone.Text = {0}", workPhone.Text));
            */
            PersistentData.DataSubject.Username = username.Text;
            //Debug.WriteLine(string.Format("dataSubject.Username = username.Text ({0})", username.Text));

            PersistentData.DataSubject.Email = email.Text;
            PersistentData.DataSubject.Password = password1.Text;
            PersistentData.DataSubject.Forename = forename.Text;
            PersistentData.DataSubject.Surname = surname.Text;
            PersistentData.DataSubject.GenderUpper = gender.Value.Substring(0, 1);
            //Debug.WriteLine("DOB = " + dobInput.Value);

            
            if (DateTime.TryParse(dobInput.Value, out DateTime result))
                PersistentData.DataSubject.Dob = Convert.ToDateTime(dobInput.Value);

            PersistentData.DataSubject.Address1 = address1.Text;
            PersistentData.DataSubject.Address2 = address2.Text;
            PersistentData.DataSubject.Address3 = address3.Text;
            PersistentData.DataSubject.PostCode = postCode.Text;
            PersistentData.DataSubject.MobilePhone = mobilePhone.Text;
            PersistentData.DataSubject.HomePhone = homePhone.Text;
            PersistentData.DataSubject.WorkPhone = workPhone.Text;
            PersistentData.DataSubject.DateTimeCreated = ParsnipApi.Data.adjustedTime;
            PersistentData.DataSubject.AccountType = accountType.Value;
            PersistentData.DataSubject.AccountStatus = accountStatus.Value;
            PersistentData.DataSubject.AccountType = accountType.Value;

        }

        
    }
}