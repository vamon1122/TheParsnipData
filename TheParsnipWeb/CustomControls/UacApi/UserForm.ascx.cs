using System;
using UacApi;
using LogApi;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TheParsnipWeb
{
    public partial class UserForm1 : System.Web.UI.UserControl
    {
        Account formAccount = new Account();
        Account MyAccount;

        protected void Page_Load(object sender, EventArgs e)
        {
            /*if (e != null)
            {
                MyAccount.LogIn(e.ToString());
            }*/

            MyAccount = new Account();
            if (!MyAccount.LogIn())
            {
                Response.Redirect("login.aspx?url=create-user.aspx");
            }
            else
            {
                new LogEntry() { text = String.Format("{0} accessed the create-user page", MyAccount.fullName), userId = MyAccount.id }.Insert();
            }


        }

        void UpdateForm()
        {
            username.Text = formAccount.Username;
            email.Text = formAccount.Email;
            //updatePwd()
            forename.Text = formAccount.Forename;
            surname.Text = formAccount.Surname;
            //updateDob()
            address1.Text = formAccount.Address1;
            address2.Text = formAccount.Address2;
            address3.Text = formAccount.Address3;
            postCode.Text = formAccount.PostCode;
            mobilePhone.Text = formAccount.MobilePhone;
            homePhone.Text = formAccount.HomePhone;
            workPhone.Text = formAccount.WorkPhone;
            // updateDateTimeCreated()
            accountType.Value = formAccount.AccountType;
            accountStatus.Value = formAccount.AccountStatus;
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            WriteToObj();
            if (formAccount.Validate())
            {
                
                formAccount.DbInsert(password1.Text);
                new LogEntry() { text = String.Format("{0} created an account for {1} via the create-user page", MyAccount.fullName, formAccount.fullName), userId = MyAccount.id }.Insert();
            }


            void WriteToObj()
            {
                formAccount.Username = username.Text;
                formAccount.Email = email.Text;
                //MyAccount.Pwd = password1.Text;
                formAccount.Forename = forename.Text;
                formAccount.Surname = surname.Text;
                //MyAccount.Dob = dob.Text;
                formAccount.Address1 = address1.Text;
                formAccount.Address2 = address2.Text;
                formAccount.Address3 = address3.Text;
                formAccount.PostCode = postCode.Text;
                formAccount.MobilePhone = mobilePhone.Text;
                formAccount.HomePhone = homePhone.Text;
                formAccount.WorkPhone = workPhone.Text;
                //MyAccount.DateTimeCreated = DateTime.Now;
                formAccount.AccountType = accountType.Value;
                formAccount.AccountStatus = accountStatus.Value;
                formAccount.AccountType = accountType.Value;
                
            }
        }
    }
}