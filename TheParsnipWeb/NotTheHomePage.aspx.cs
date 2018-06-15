using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace TheParsnipWeb
{
    public partial class NotTheHomePage : System.Web.UI.Page
    {
        private Account MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyAccount = new Account();
            if (!MyAccount.LogIn())
            {
                Response.Redirect("LogInBarrier.aspx?url=NotTheHomePage.aspx");
            }
        }
    }
}