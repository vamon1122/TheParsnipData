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
using BenLog;

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

        User MyAccount;
        LogWriter LogInControlLog;

        public LogInControl()
        {
            LogInControlLog = new LogWriter("LogInControl Log.txt", AppDomain.CurrentDomain.BaseDirectory);

            
            

            LogInControlLog.Info("Login control was initialised");
        }

        private bool LogIn(string pUsername, string pPwd)
        {
            LogInControlLog.Info(String.Format("Attempting log in with details: Username = {0} Password = {1}", pUsername, pPwd));
            //MyAccount = new Account();
            if(MyAccount.LogIn(pUsername, true, pPwd, false))
            {
                LogInControlLog.Info(String.Format("Successfully logged in  with details: Username = {0} Password = {1}", pUsername, pPwd));
                return true;
            }
            else
            {
                LogInControlLog.Info(String.Format("Failed to log in with details: Username = {0} Password = {1}", pUsername, pPwd));
                return false;
            }
            
        }

        void ButLogIn_Click(object sender, EventArgs e)
        {
            LogInControlLog.Info("LogIn buton was clicked! Attempting LogIn...");
            if (LogIn(inUsername.Text, inPwd.Text))
            {
                LogInControlLog.Info("LogIn buton was clicked! Log in was successful!");
            }
            else
            {
                LogInControlLog.Info("LogIn buton was clicked! Log in was unsuccessful.");
            }
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

            MyAccount = new User();

            MyAccount.LogIn();

            title = new Label() { Text = "Log In" };

            inUsername = new TextBox();
            if(MyAccount.username != null && !String.IsNullOrEmpty(MyAccount.username) && !String.IsNullOrWhiteSpace(MyAccount.username)){
                inUsername.Text = MyAccount.username;
            }
            StyleTextBox(inUsername);

            inPwd = new TextBox();
            StyleTextBox(inPwd);

            butLogIn = new Button() { Text = "Log In" };
            butLogIn.Click += new EventHandler(ButLogIn_Click);
            LogInControlLog.Info("Event handler was appended to butLogIn");

            


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
