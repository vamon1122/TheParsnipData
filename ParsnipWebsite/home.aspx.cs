using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using LogApi;
using CookieApi;

namespace ParsnipWebsite
{
    public partial class Home : System.Web.UI.Page
    {
        private User myAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            myAccount = Uac.SecurePage("home", this, Data.deviceType);
        }
    }
}