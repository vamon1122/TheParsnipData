using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using BenLog;
using CookieApi;
using System.Diagnostics;
using LogApi;

namespace ParsnipWebsite
{
    public partial class LogInBarrier : System.Web.UI.Page
    {
        private User myUser;
        private string Redirect;
        LogWriter AccountLog = new LogWriter("Account Object.txt", AppDomain.CurrentDomain.BaseDirectory);

        protected void Page_Load(object sender, EventArgs e)
        {
            AccountLog.Warning("PAGE IS BEING LOADED");

            

            /*
            new LogEntry(Log.Default) { text = "Detecting device and setting deviceType cookie..." };
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "GetDeviceType", "createCookie(\"deviceType\", deviceDetect())", true);
            if (Cookie.Exists("deviceType"))
            {
                new LogEntry(Log.Default) { text = string.Format("----------Cookie exists! deviceType = {0}", Cookie.Read("deviceType")) };
            }
            else
            {
                new LogEntry(Log.Default) { text = "----------Device type cookie did not exist" };
            }
            */

            if (Request.QueryString["url"] != null)
            {
                Redirect = Request.QueryString["url"];
                Warning.Attributes.CssStyle.Add("display", "block");
                
            }
            else
            {
                Redirect = "home";
            }
                


            myUser = new User("login");

            if (String.IsNullOrEmpty(inputUsername.Text) && String.IsNullOrWhiteSpace(inputUsername.Text)){
                if (myUser.LogIn(false))
                {
                    WriteCookie();
                    Response.Redirect(Redirect);
                }
                else
                {

                    AccountLog.Warning("Input username was reset");
                    inputUsername.Text = myUser.Username;

                }
            }
        }

        private void WriteCookie()
        {
            Cookie.WritePerm("accountType", myUser.AccountType);
            System.Diagnostics.Debug.WriteLine("----------accountType = " + myUser.AccountType);
            System.Diagnostics.Debug.WriteLine("----------accountType = " + Cookie.Read("accountType"));
        }

        protected void ButLogIn_Click(object sender, EventArgs e)
        {
            AccountLog.Warning("Button Was clicked!");

            System.Diagnostics.Debug.WriteLine("CheckBox = " + RememberPwd.Checked);

            if (myUser.LogIn(inputUsername.Text, true, inputPwd.Text, RememberPwd.Checked, false))
            {
                new LogEntry(new Log("login/out")) { text = String.Format("{0} logged in from {1} {2}.", myUser.FullName, myUser.PosessivePronoun, Data.deviceType) };
                WriteCookie();
                Response.Redirect(Redirect);
            }
        }
    }
}