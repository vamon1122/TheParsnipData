using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace TheParsnipWeb
{
    public partial class Home : System.Web.UI.Page
    {
        private Account MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyAccount = new Account();
            if (!MyAccount.LogIn())
            {
                Response.Redirect("LogInBarrier.aspx?url=Home.aspx");
            }
            else
            {
                WelcomeLabel.Text = String.Format("Welcome back {0}!", MyAccount.Fname);
            }
        }
    }
}