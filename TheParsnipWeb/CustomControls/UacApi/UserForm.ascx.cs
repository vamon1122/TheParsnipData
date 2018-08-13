using System;
using UacApi;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TheParsnipWeb
{
    public partial class UserForm1 : System.Web.UI.UserControl
    {
        Account MyAccount = new Account();

        protected void Page_Load(object sender, EventArgs e)
        {
            /*if (e != null)
            {
                MyAccount.LogIn(e.ToString());
            }*/
        }

        void UpdateForm()
        {
            username.Text = MyAccount.Username;
            email.Text = MyAccount.Email;
            //updatePwd()
            forename.Text = MyAccount.Forename;
            surname.Text = MyAccount.Surname;
            //updateDob()
            address1.Text = MyAccount.Address1;
            address2.Text = MyAccount.Address2;
            address3.Text = MyAccount.Address3;
            postCode.Text = MyAccount.PostCode;
            mobilePhone.Text = MyAccount.MobilePhone;
            homePhone.Text = MyAccount.HomePhone;
            workPhone.Text = MyAccount.WorkPhone;
            // updateDateTimeCreated()
            accountType.Value = MyAccount.AccountType;
            accountStatus.Value = MyAccount.AccountStatus;
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            WriteToObj();
            if (MyAccount.Validate())
            {
                
                MyAccount.DbInsert(password1.Text);
            }


            void WriteToObj()
            {
                MyAccount.Username = username.Text;
                MyAccount.Email = email.Text;
                //MyAccount.Pwd = password1.Text;
                MyAccount.Forename = forename.Text;
                MyAccount.Surname = surname.Text;
                //MyAccount.Dob = dob.Text;
                MyAccount.Address1 = address1.Text;
                MyAccount.Address2 = address2.Text;
                MyAccount.Address3 = address3.Text;
                MyAccount.PostCode = postCode.Text;
                MyAccount.MobilePhone = mobilePhone.Text;
                MyAccount.HomePhone = homePhone.Text;
                MyAccount.WorkPhone = workPhone.Text;
                MyAccount.DateTimeCreated = DateTime.Now;
                MyAccount.AccountType = accountType.Value;
                MyAccount.AccountStatus = accountStatus.Value;
                MyAccount.AccountType = accountType.Value;
                
            }
        }
    }
}