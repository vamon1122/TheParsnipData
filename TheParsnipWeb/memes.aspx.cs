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
    public partial class memes : System.Web.UI.Page
    {
        private Account MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyAccount = new Account();
            if (!MyAccount.LogIn())
            {
                Response.Redirect("login.aspx?url=memes.aspx");
            }
            else
            {
                new LogEntry() { text = String.Format("{0} accessed the memes page", MyAccount.fullName), userId = MyAccount.id }.Insert();
            }
        }
    }
}