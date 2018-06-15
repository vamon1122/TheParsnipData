using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace TheParsnipWeb
{
    public partial class Admin : System.Web.UI.Page
    {
        Account MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyAccount = new Account();
            if (!MyAccount.LogIn())
            {
                Response.Redirect("LogInBarrier.aspx?url=Admin.aspx");
            }

            if(MyAccount.AccountType != "admin")
            {
                Response.Redirect("AccessDenied.aspx?url=Admin.aspx");
            }
        }
    }
}