using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace ParsnipControls
{
    //[DefaultProperty("Text")]
    [ToolboxData("<{0}:LogInControl runat=server></{0}:LogInControl>")]
    public class LogInControl : CompositeControl
    {
        /*[Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]*/

        /*public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }*/

        Label title;
        TextBox inUsername;
        TextBox inPwd;
        Button butLogIn;
        Label indicator;

        Account MyAccount;

        string CookieUsername;
        string CookiePwd;

        LogInControl()
        {
            if (MyAccount.LogIn())
            {
                if(LogIn(CookieUsername, CookiePwd))
                {

                }
            }


            
        }

        private bool LogIn(string pUsername, string pPwd)
        {
            MyAccount = new Account();
            if (MyAccount.LogIn(pUsername, pPwd))
            {
                Response.Cookies["userName"].Value = "patrick";
                Response.Cookies["userName"].Expires = DateTime.Now.AddDays(1);

                HttpCookie aCookie = new HttpCookie("lastVisit");
                aCookie.Value = DateTime.Now.ToString();
                aCookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(aCookie);

                indicator.ForeColor = System.Drawing.Color.Green;
                indicator.Text = "Logged in succeeded!";
                return true;
            }
            else
            {
                indicator.ForeColor = System.Drawing.Color.Red;
                indicator.Text = "Logged in falied";
                return false;
            }
        }

        void butLogIn_Click(Object sender, EventArgs e)
        {
            LogIn(inUsername.Text, inPwd.Text);
            //Do callbacks https://msdn.microsoft.com/en-us/library/aa479299.aspx
        }


        /*void ICallbackEventHandler.RaiseCallbackEvent(string argument)
        {
            // No input data to process; just returns
            return;
        }
        string ICallbackEventHandler.GetCallbackResult()
        {
            // Get the feed
            GetFeed();

            // Process the feed
            RssInfo info = ProcessFeed();

            // Prepare the return value for the client
            return RssHelpers.SerializeFeed(info);
        }*/


        protected override void CreateChildControls()
        {
            

            title = new Label() { Text = "Log In" };

            inUsername = new TextBox();
            StyleTextBox(inUsername);

            inPwd = new TextBox();
            StyleTextBox(inPwd);

            butLogIn = new Button() { Text = "Log In" };
            butLogIn.Click += new EventHandler(this.butLogIn_Click);
            

            indicator = new Label() { Text = "Default indicator text" };

            void StyleTextBox(TextBox pTextBox)
            {
                pTextBox.Width = 200;
                pTextBox.BackColor = System.Drawing.Color.Red;
            }

            
        }

        protected override void Render(HtmlTextWriter writer)
        {
            title.RenderControl(writer);
            inUsername.RenderControl(writer);
            inPwd.RenderControl(writer);
            butLogIn.RenderControl(writer);
            indicator.RenderControl(writer);
        }
    }
}
