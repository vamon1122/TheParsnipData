using System;
using UacApi;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogApi;

namespace TheParsnipWeb
{
    public partial class youtube : System.Web.UI.Page
    {
        private User myUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("videos", this, Data.deviceType);
        }
    }
}