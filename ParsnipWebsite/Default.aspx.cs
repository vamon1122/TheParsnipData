using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace TheParsnipWeb
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            User myUser = Uac.SecurePage("videos", this, Data.deviceType);
            Response.Redirect("home");
        }

        protected void MyTestButton_Click(object sender, EventArgs e)
        {

        }
    }
}