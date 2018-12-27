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
        User myAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            myAccount = new User();
            myAccount = Uac.SecurePage("admin", this, Data.deviceType);

            if (myAccount.AccountType != "admin")
            {
                new LogEntry() { text = String.Format("{0} attempted (and failed) to access the admin page via {1}", myAccount.fullName, Data.deviceType), userId = myAccount.id };
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