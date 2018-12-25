using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using LogApi;
using CookieApi;

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
                Response.Redirect("login.aspx?url=home.aspx");
            }
            else
            {
                new LogEntry() { text = String.Format("{0} accessed the home page via {1}. Location (Lat/Long): {2}", MyAccount.fullName, Data.deviceType, Data.deviceLocation), userId = MyAccount.id }.Insert();
                WelcomeLabel.Text = String.Format("Welcome back {0}!", MyAccount.Forename);
            }
        }
    }
}