using System;
using UacApi;
using LogApi;
using ParsnipApi;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TheParsnipWeb
{
    public partial class UserForm1 : System.Web.UI.UserControl
    {
        User myUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            //UpdateForm();
        }

        public UserForm1()
        {
            myUser = new User();
        }

        public UserForm1(User pUser)
        {
            myUser = pUser;
            //dateTimeCreated.Value = myUser.DateTimeCreated.Date.ToString();
            //UpdateForm();
        }

        void UpdateForm()
        {
            username.Text = myUser.Username;
            email.Text = myUser.Email;
            //updatePwd()
            forename.Text = myUser.Forename;
            surname.Text = myUser.Surname;
            gender.Value = myUser.Gender;
            dobInput.Value = myUser.Dob.ToString();
            address1.Text = myUser.Address1;
            address2.Text = myUser.Address2;
            address3.Text = myUser.Address3;
            postCode.Text = myUser.PostCode;
            mobilePhone.Text = myUser.MobilePhone;
            homePhone.Text = myUser.HomePhone;
            workPhone.Text = myUser.WorkPhone;
            dateTimeCreated.Value = myUser.DateTimeCreated.Date.ToString("dd/MM/yyyy");
            accountType.Value = myUser.AccountType;
            accountStatus.Value = myUser.AccountStatus;
        }

        void UpdateFormAccount()
        {
            System.Diagnostics.Debug.WriteLine(string.Format("username.Text = {0}", username.Text));
            System.Diagnostics.Debug.WriteLine(string.Format("forename.Text = {0}", forename.Text));
            myUser.Username = username.Text;
            System.Diagnostics.Debug.WriteLine(string.Format("myUser.Username = username.Text ({0})", username.Text));

            myUser.Email = email.Text;
            myUser.Pwd = password1.Text;
            myUser.Forename = forename.Text;
            myUser.Surname = surname.Text;
            myUser.Gender = gender.Value.Substring(0, 1);
            System.Diagnostics.Debug.WriteLine("DOB = " + dobInput.Value);
            myUser.Dob = Convert.ToDateTime(dobInput.Value);
            myUser.Address1 = address1.Text;
            myUser.Address2 = address2.Text;
            myUser.Address3 = address3.Text;
            myUser.PostCode = postCode.Text;
            myUser.MobilePhone = mobilePhone.Text;
            myUser.HomePhone = homePhone.Text;
            myUser.WorkPhone = workPhone.Text;
            myUser.DateTimeCreated = ParsnipApi.Data.adjustedTime;
            myUser.AccountType = accountType.Value;
            myUser.AccountStatus = accountStatus.Value;
            myUser.AccountType = accountType.Value;

        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            UpdateFormAccount();
            if (myUser.Validate())
            {
                myUser.DbInsert(password1.Text);
                new LogEntry(myUser.id) { text = String.Format("{0} created an account for {1} via the UserForm", myUser.fullName, myUser.fullName) };
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("User failed to validate!");
                new LogEntry(myUser.id) { text = String.Format("{0} attempted to create an account for {1} via the UserForm, but the user failed fo validate!", myUser.fullName, myUser.fullName) };
            }
        }
    }
}