using System;
using UacApi;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TheParsnipWeb
{
    public partial class _TEMPLATE : System.Web.UI.Page
    {
        private User MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyAccount = new User();
            if (!MyAccount.LogIn())
            {
                Response.Redirect("login.aspx?url=photos.aspx");
            }
            else
            {

            }
        }
    }
}