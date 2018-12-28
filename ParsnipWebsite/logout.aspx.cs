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
            User myUser = Uac.SecurePage("logout", this, Data.deviceType);
            new LogEntry() { text = String.Format("{0} logged out from {1} '{2}' device", myUser.fullName, myUser.posessivePronoun, Data.deviceType), userId = myUser.id };
            new User().LogOut();
            Response.Redirect("login.aspx");
        }
    }
}