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
            var myUser = new User();
            if (myUser.LogIn())
            {
                new LogEntry() { text = String.Format("{0} accessed the {1} page from {2} '{3}' device", myUser.fullName, pUrl, myUser.posessivePronoun, pDeviceType), userId = myUser.id };
            }
            else
            {
                new LogEntry() { text = String.Format("Someone tried to access the {0} page from an {1}, without logging in!", pUrl, pDeviceType), userId = myUser.id };
                pPage.Response.Redirect(String.Format("login.aspx?url={0}.aspx", pUrl));
            }
            return myUser;

        }
    }

    
}
