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
            if (string.IsNullOrEmpty(Data.deviceType) || string.IsNullOrWhiteSpace(Data.deviceType))
                Response.Redirect("getdeviceinfo?url=logout");

            User myUser = new User("logout get name");
            myUser.LogIn();
            new LogEntry(myUser.Id) { text = String.Format("{0} logged out from {1} {2} device.", myUser.FullName, myUser.PosessivePronoun, Data.deviceType) };
            UacApi.User.LogOut();
            Response.Redirect("login");
        }
    }
}