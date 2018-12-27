using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace TheParsnipWeb
{
    public partial class polls : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            myAccount = Uac.SecurePage("polls", this, Data.deviceType);
        }
    }
}