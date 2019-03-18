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
    public partial class _TEMPLATE : System.Web.UI.Page
    {
        User myUser;
        protected async void Page_Load(object sender, EventArgs e)
        {
            //We secure the page using the UacApi. 
            //This ensures that the user is logged in etc
            //You only need to change where it says '_NEW TEMPLATE'.
            //Change this to match your page name without the '.aspx' extension.
            myUser = await Uac.SecurePage("_NEW TEMPLATE", this, Data.DeviceType);
        }
    }
}