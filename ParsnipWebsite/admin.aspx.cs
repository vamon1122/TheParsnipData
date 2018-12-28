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
            myAccount = Uac.SecurePage("admin", this, Data.deviceType, "admin");
        }

        protected void OpenLogsButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("logs");
        }

        protected void NewUserButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("create-user");
        }
    }
}