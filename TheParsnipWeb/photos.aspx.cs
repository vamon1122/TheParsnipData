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
    public partial class photos : System.Web.UI.Page
    {
        private Account MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyAccount = new Account();
            if (!MyAccount.LogIn())
            {
                Response.Redirect("login.aspx?url=photos.aspx");
            }
            else
            {
                new LogEntry() { text = String.Format("{0} accessed the photos page via {1}", MyAccount.fullName, Data.deviceType), userId = MyAccount.id }.Insert();
            }
        }
    }
}