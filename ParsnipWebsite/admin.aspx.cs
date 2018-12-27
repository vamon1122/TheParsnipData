using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using LogApi;

namespace TheParsnipWeb
{
    public partial class Admin : System.Web.UI.Page
    {
        User MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyAccount = new User();
            if (!MyAccount.LogIn())
            {
                Response.Redirect("login.aspx?url=admin.aspx");
            }
            else
            {
                new LogEntry() { text = String.Format("{0} accessed the admin page", MyAccount.fullName), userId = MyAccount.id };
            }

            if(MyAccount.AccountType != "admin")
            {
                new LogEntry() { text = String.Format("{0} attempted (and failed) to access the admin page via {1}", MyAccount.fullName, Data.deviceType), userId = MyAccount.id };
                Response.Redirect("access-denied.aspx?url=admin.aspx");
            }
        }

        protected void OpenLogsButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("logs.aspx");
        }

        protected void NewUserButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("create-user.aspx");
        }
    }
}