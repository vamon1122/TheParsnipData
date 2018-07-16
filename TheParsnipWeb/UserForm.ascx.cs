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
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            Account MyAccount = new Account();
            MyAccount.Username = username.Text;
            MyAccount.Email = email.Text;
            //MyAccount.Pwd = password1.Text;
            MyAccount.Fname = forename.Text;
            MyAccount.Sname = surname.Text;
            //MyAccount.Dob = dob.Text;
            MyAccount.Address1 = address1.Text;
            MyAccount.Address2 = address2.Text;
            MyAccount.Address3 = address3.Text;
            MyAccount.Postcode = postCode.Text;
            MyAccount.MobilePhone = mobilePhone.Text;
            MyAccount.HomePhone = homePhone.Text;
            MyAccount.WorkPhone = workPhone.Text;
            MyAccount.DateTimeCreated = DateTime.Now;
            MyAccount.AccountType = accountType.Equals

        }
    }
}