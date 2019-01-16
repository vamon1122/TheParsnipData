using System;
using UacApi;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ParsnipWebsite
{
    public partial class AccountTiny : System.Web.UI.UserControl
    {
        User MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyAccount = new User("AccountTiny");
            MyAccount.LogIn();
            Name.Text = MyAccount.Username;
        }

        protected void LogOut_Click(object sender, EventArgs e)
        {
            User.LogOut();
            Response.Redirect("login");
        }
    }
}