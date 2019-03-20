using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using LogApi;

namespace ParsnipWebsite
{
    public partial class Logout : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Data.DeviceType) || string.IsNullOrWhiteSpace(Data.DeviceType))
                Response.Redirect("get_device_info?url=logout", false);

            User myUser = await UacApi.User.LogInFromCookies();

            new LogEntry(new Log("login/out")) { text = String.Format("{0} logged out from {1} {2} device.", myUser.FullName, myUser.PosessivePronoun, Data.DeviceType) };
            UacApi.User.LogOut();
            Response.Redirect("login");
        }
    }
}