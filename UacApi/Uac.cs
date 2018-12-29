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
        public static User SecurePage(string pUrl, Page pPage, string pDeviceType, string pAccountType)
        {
            var myUser = new User();
            if (myUser.LogIn())
            {
                bool CanAccess;
                switch (pAccountType)
                {
                    case "admin":
                        if (myUser.AccountType == "admin") CanAccess = true; else CanAccess = false;
                        CanAccess = true;
                        break;
                    case "member":
                        if (myUser.AccountType == "admin" || myUser.AccountType == "member") CanAccess = true; else CanAccess = false;
                        break;
                    case "user":
                        if (myUser.AccountType == "admin" || myUser.AccountType == "member" || myUser.AccountType == "user") CanAccess = true; else CanAccess = false;
                        break;
                    default:
                        CanAccess = false;
                        break;
                }

                if (CanAccess)
                {
                    System.Diagnostics.Debug.WriteLine("{0} is allowed to access {1}", myUser.fullName, pUrl);
                    new LogEntry() { text = String.Format("{0} accessed the {1} page from {2} '{3}' device", myUser.fullName, pUrl, myUser.posessivePronoun, pDeviceType), userId = myUser.id };
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("{0} is NOT allowed to access {1}", myUser.fullName, pUrl);
                    new LogEntry() { text = String.Format("{0} attempted to access the {1} page from {2} '{3}' device but did not have sufficient permissions", myUser.fullName, pUrl, myUser.posessivePronoun, pDeviceType), userId = myUser.id };
                    pPage.Response.Redirect(String.Format("access-denied?url={0}", pUrl));
                }

                
            }
            else
            {
                new LogEntry() { text = String.Format("Someone tried to access the {0} page from {1} '{2}' device, without logging in!", pUrl, myUser.posessivePronoun, pDeviceType), userId = myUser.id };
                pPage.Response.Redirect(String.Format("login?url={0}", pUrl));
            }

            return myUser;
        }

        public static User SecurePage(string pUrl, Page pPage, string pDeviceType)
        {
            return SecurePage(pUrl, pPage, pDeviceType, "user");
        }
    }

    
}
