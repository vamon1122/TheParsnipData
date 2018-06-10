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

        private bool LogIn()
        {
            MyAccount = new Account();
            if (MyAccount.LogIn(inUsername.Text, inPwd.Text))
            {
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
            LogIn();
            //Do callbacks https://msdn.microsoft.com/en-us/library/aa479299.aspx
        }


        protected override void CreateChildControls()
        {
            

            title = new Label() { Text = "Log In" };

            inUsername = new TextBox();
            StyleTextBox(inUsername);

            inPwd = new TextBox();
            StyleTextBox(inPwd);

            butLogIn = new Button() { Text = "Log In" };
            butLogIn.Click += new EventHandler(this.butLogIn_Click);
            

            indicator = new Label();

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
