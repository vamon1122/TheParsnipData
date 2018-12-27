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
    public partial class logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            User myAccount = new User();
            new LogEntry() { text = String.Format("{0} logged out from '{1}'", myAccount.fullName, Data.deviceType), userId = myAccount.id };
            new User().LogOut();
            Response.Redirect("login.aspx");
        }
    }
}