using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace ParsnipWebsite
{
    public partial class Home : System.Web.UI.Page
    {
        private User myUser;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("home", this, Data.DeviceType);
            WelcomeLabel.Text = string.Format("Hiya {0}, welcome back to the parsnip website!", myUser.Forename);
        }
    }
}