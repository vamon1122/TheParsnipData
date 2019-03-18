using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace ParsnipWebsite
{
    public partial class Videos : System.Web.UI.Page
    {
        private User myUser;

        protected async void Page_Load(object sender, EventArgs e)
        {
            myUser = await UacApi.User.LogInFromCookies();
            Uac.SecurePage("videos", this, Data.DeviceType, "member", myUser);
        }
    }
}