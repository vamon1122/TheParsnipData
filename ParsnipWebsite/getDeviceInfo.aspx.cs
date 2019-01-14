using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CookieApi;
using LogApi;

namespace ParsnipWebsite
{
    public partial class getDeviceInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /*string Redirect;
            if (Request.QueryString["url"] != null)
            {
                Redirect = Request.QueryString["url"];
            }
            else
            {
                Redirect = "home";
            }*/

            new LogEntry(Guid.Empty) { text = "Detecting device and setting deviceType cookie..." };
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "GetDeviceType", "createCookie(\"deviceType\", deviceDetect())", true);
            if (Cookie.Exists("deviceType"))
            {
                new LogEntry(Guid.Empty) { text = string.Format("----------Cookie exists! deviceType = {0}", Cookie.Read("deviceType")) };
            }
            else
            {
                new LogEntry(Guid.Empty) { text = "----------Device type cookie did not exist" };
            }

            Response.Redirect("home");
        }
    }
}