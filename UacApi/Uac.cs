using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogApi;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UacApi
{
    public static class Uac
    {
        public static User SecurePage(string pUrl, Page pPage, string pDeviceType)
        {
            var MyAccount = new User();
            if (!MyAccount.LogIn())
            {
                pPage.Response.Redirect("login.aspx?url=home.aspx");
                new LogEntry() { text = String.Format("Someone tried to access the home page from an {1}, without logging in!", MyAccount.fullName, pDeviceType), userId = MyAccount.id }.Insert();
            }
            else
            {
                new LogEntry() { text = String.Format("{0} accessed the home page from {1} {2}", MyAccount.fullName, MyAccount. pDeviceType), userId = MyAccount.id }.Insert();
            }
            new LogEntry() { text = String.Format("{0} accessed the home page via {1}", MyAccount.fullName, pDeviceType), userId = MyAccount.id }.Insert();
        }
    }

    
}
