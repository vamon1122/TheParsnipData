using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ParsnipData.UacApi;
using ParsnipData.Logs;

namespace ParsnipWebsite
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Data.DeviceType) || string.IsNullOrWhiteSpace(Data.DeviceType))
                Response.Redirect("get_device_info?url=logout");

            User myUser = new User("logout get name");
            myUser.LogIn();
            new LogEntry(new Log("login/out")) { text = String.Format("{0} logged out from {1} {2} device.", myUser.FullName, myUser.PosessivePronoun, Data.DeviceType) };
            ParsnipData.UacApi.User.LogOut();
            Response.Redirect("login");
        }
    }
}