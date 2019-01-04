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
        User MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyAccount = new User("AccountTiny");
            MyAccount.LogIn();
            Name.Text = MyAccount.username;
        }

        protected void LogOut_Click(object sender, EventArgs e)
        {
            MyAccount.LogOut();
            Response.Redirect("login");
        }
    }
}