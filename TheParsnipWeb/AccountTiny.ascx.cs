using System;
using UacApi;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TheParsnipWeb
{
    public partial class AccountTiny : System.Web.UI.UserControl
    {
        Account MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyAccount = new Account();
            MyAccount.LogIn();
            Name.Text = MyAccount.Username;
        }

        protected void LogOut_Click(object sender, EventArgs e)
        {
            MyAccount.LogOut();
            Response.Redirect("login.aspx");
        }
    }
}