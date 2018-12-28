using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace TheParsnipWeb
{
    public partial class AccessDenied : System.Web.UI.Page
    {
        User MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyAccount = new User();

            
            if (Request.QueryString["url"] != null)
            {
                if (MyAccount.LogIn())
                {
                    Info.Text = String.Format("You aren't allowed to visit \"{0}\". Nice try {1}...", Request.QueryString["url"], MyAccount.Forename);
                }
                else
                {
                    Response.Redirect("LogInBarrier?" + Request.QueryString["url"]);
                }

                
            }
            else
            {
                Info.Text = "Why are you trying to get back here? :P";
            }

            
        }
    }
}