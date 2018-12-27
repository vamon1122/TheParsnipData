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
        private User MyAccount;

        protected void Page_Load(object sender, EventArgs e)
        {

            MyAccount = new User();
            if (!MyAccount.LogIn())
            {
                Response.Redirect("login.aspx?url=youtube.aspx");
            }
            else
            {
                new LogEntry() { text = String.Format("{0} accessed the youtube page via {1}", MyAccount.fullName, Data.deviceType), userId = MyAccount.id };
            }


        }
    }
}