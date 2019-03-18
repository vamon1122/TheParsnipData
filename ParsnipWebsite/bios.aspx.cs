using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace ParsnipWebsite
{
    public partial class Bios : System.Web.UI.Page
    {
        private User myUser;
        protected async void Page_Load(object sender, EventArgs e)
        {
            myUser = await UacApi.User.LogInFromCookies();
            Uac.SecurePage("minecraft", this, Data.DeviceType, "user", myUser);
        }
    }
}