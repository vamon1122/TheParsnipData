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
    public partial class UserForm1 : System.Web.UI.UserControl
    {
        private User _dataSubject;
        public User dataSubject { get { return _dataSubject; } set { Debug.WriteLine(string.Format("dataSubject (id = \"{0}\") was set in UserForm", value.Id)); _dataSubject = value; UpdateFields(); } }

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

        public void UpdateFields()
        {
            Debug.WriteLine(string.Format("----------Userform fields are being updated. Name: {0} Id: {1}", dataSubject.FullName, dataSubject.Id));

            Debug.WriteLine(string.Format("----------{0} != {1}", dataSubject.Id.ToString(), Guid.Empty.ToString()));

            

            //Debug.WriteLine("----------UpdateForm()");
            //Debug.WriteLine("----------username = " + username.Text);
            //Debug.WriteLine("----------dataSubject.username = " + dataSubject.username);
            //Debug.WriteLine("----------dataSubject.id = " + dataSubject.id);
            username.Text = dataSubject.Username;
            email.Text = dataSubject.Email;
            password1.Text = dataSubject.Password;
            password2.Text = dataSubject.Password;
            forename.Text = dataSubject.Forename;
            surname.Text = dataSubject.Surname;
            gender.Value = dataSubject.GenderUpper;
            if (dataSubject.Dob.ToString("dd/MM/yyyy") != "01/01/0001")
                dobInput.Value = dataSubject.Dob.ToString("dd/MM/yyyy");
            else
                dobInput.Value = "";
            address1.Text = dataSubject.Address1;
            address2.Text = dataSubject.Address2;
            address3.Text = dataSubject.Address3;
            postCode.Text = dataSubject.PostCode;
            mobilePhone.Text = dataSubject.MobilePhone;
            homePhone.Text = dataSubject.HomePhone;
            workPhone.Text = dataSubject.WorkPhone;
            dateTimeCreated.Attributes.Remove("placeholder");
            dateTimeCreated.Attributes.Add("placeholder", dataSubject.DateTimeCreated.Date.ToString("dd/MM/yyyy"));
            accountType.Value = dataSubject.AccountType;
            accountStatus.Value = dataSubject.AccountStatus;
            if (dataSubject.DateTimeCreated.ToString("dd/MM/yyyy") == "01/01/0001")
            {
                //Debug.WriteLine(string.Format("{0}'s datetimecreated {1} == 01/01/0001", dataSubject.fullName, dataSubject.dateTimeCreated.ToString("dd/MM/yyyy")));
                dateTimeCreated.Value = ParsnipApi.Data.adjustedTime.ToString("dd/MM/yyyy");
            }
            else
            {
                //Debug.WriteLine(string.Format("{0}'s dob {1} != 01/01/0001",dataSubject.fullName, dataSubject.dateTimeCreated.ToString("dd/MM/yyyy")));
                dateTimeCreated.Value = dataSubject.DateTimeCreated.ToString("dd/MM/yyyy");
            }
                
        }

        public void UpdateDataSubject()
        {
            if (dataSubject == null)
            {
                System.Diagnostics.Debug.WriteLine("My dataSubject is null. Adding new dataSubject");
                dataSubject = new User("UpdateDataSubject (UserForm1)");
            }
            Debug.WriteLine(string.Format("username.Text = {0}", username.Text));
            Debug.WriteLine(string.Format("forename.Text = {0}", forename.Text));
            Debug.WriteLine(string.Format("mobilePhone.Text = {0}", mobilePhone.Text));
            Debug.WriteLine(string.Format("homePhone.Text = {0}", homePhone.Text));
            Debug.WriteLine(string.Format("workPhone.Text = {0}", workPhone.Text));
            dataSubject.Username = username.Text;
            Debug.WriteLine(string.Format("dataSubject.Username = username.Text ({0})", username.Text));

            dataSubject.Email = email.Text;
            dataSubject.Password = password1.Text;
            dataSubject.Forename = forename.Text;
            dataSubject.Surname = surname.Text;
            dataSubject.GenderUpper = gender.Value.Substring(0, 1);
            Debug.WriteLine("DOB = " + dobInput.Value);

            
            if (DateTime.TryParse(dobInput.Value, out DateTime result))
                dataSubject.Dob = Convert.ToDateTime(dobInput.Value);
            dataSubject.Address1 = address1.Text;
            dataSubject.Address2 = address2.Text;
            dataSubject.Address3 = address3.Text;
            dataSubject.PostCode = postCode.Text;
            dataSubject.MobilePhone = mobilePhone.Text;
            dataSubject.HomePhone = homePhone.Text;
            dataSubject.WorkPhone = workPhone.Text;
            dataSubject.DateTimeCreated = ParsnipApi.Data.adjustedTime;
            dataSubject.AccountType = accountType.Value;
            dataSubject.AccountStatus = accountStatus.Value;
            dataSubject.AccountType = accountType.Value;

        }

        
    }
}