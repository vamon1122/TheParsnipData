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
    public partial class getdeviceinfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Writing sessionId cookie = " + Session.SessionID.ToString());
            Cookie.WriteSession("sessionId", Session.SessionID.ToString());

        }
    }
}