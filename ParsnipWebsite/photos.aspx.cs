using System;
using UacApi;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogApi;

namespace ParsnipWebsite
{
    public partial class photos : System.Web.UI.Page
    {
        private User myUser;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("photos", this, Data.deviceType, "member");
        }
    }
}