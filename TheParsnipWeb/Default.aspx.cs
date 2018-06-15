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
            Account MyAccount = new Account();

            /*if (MyAccount.LogIn("vamon1122", true, "BBTbbt1704",false))
            {
                Response.Redirect("Home.aspx");
            }
             else
            {
                Response.Redirect("LogInBarrier.aspx");
            }*/

            if (MyAccount.LogIn())
            {
                Response.Redirect("Home.aspx");
            }
            else
            {
                Response.Redirect("LogInBarrier.aspx");
            }
        }

        protected void MyTestButton_Click(object sender, EventArgs e)
        {

        }
    }
}