using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace TheParsnipWeb
{
    public partial class create_user : System.Web.UI.Page
    {
        User myUser;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("create-user", this, Data.deviceType);
        }
    }
}