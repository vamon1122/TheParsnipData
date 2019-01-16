using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace ParsnipWebsite
{
    public partial class AccessDenied : System.Web.UI.Page
    {
        User myUser;
        string attemptedAccess;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["url"] != null)
                attemptedAccess = Request.QueryString["url"];
            
            myUser = Uac.SecurePage("access-denied", this, Data.deviceType);

            if(attemptedAccess == null)
            {
                Info.Text = "Why are you trying to access this page directly? :P";
            }
            else
            {
                Info.Text = string.Format("You don't have permission to access the {0} page.", attemptedAccess);
            }
            
        }
    }
}