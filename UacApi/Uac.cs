using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogApi;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace UacApi
{
    public static class Uac
    {
        public static User SecurePage(string pUrl, Page pPage, string pDeviceType, string pAccountType)
        {
            var myUser = new User("Uac.SecurePage(4)");
            Debug.WriteLine("----------Securing page");
            bool CanAccess;

            if (myUser.LogIn())
            {
                Debug.WriteLine("----------Securing page, accountType = " + myUser.AccountType);
                
                if (myUser.AccountStatus == "active")
                {


                    switch (pAccountType)
                    {
                        case "admin":
                            if (myUser.AccountType == "admin") CanAccess = true; else CanAccess = false;
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
                }
                else
                {
                    CanAccess = false;
                    new LogEntry(Guid.Empty) { text = string.Format("{0} tried to access {0} but their account is not active!", myUser.FullName, pUrl) };
                }

                if (CanAccess)
                {
                    System.Diagnostics.Debug.WriteLine("{0} is allowed to access {1}", myUser.FullName, pUrl);
                    new LogEntry(myUser.Id) { text = String.Format("{0} accessed the {1} page from {2} '{3}' device", myUser.FullName, pUrl, myUser.PosessivePronoun, pDeviceType) };
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("{0} is NOT allowed to access {1}", myUser.FullName, pUrl);
                    new LogEntry(myUser.Id) { text = String.Format("{0} attempted to access the {1} page from {2} '{3}' device but did not have sufficient permissions", myUser.FullName, pUrl, myUser.PosessivePronoun, pDeviceType) };
                    pPage.Response.Redirect(String.Format("access-denied?url={0}", pUrl));
                }

                
            }
            else
            {
                new LogEntry(myUser.Id) { text = String.Format("Someone tried to access the {0} page from {1} '{2}' device, without logging in!", pUrl, myUser.PosessivePronoun, pDeviceType) };
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
