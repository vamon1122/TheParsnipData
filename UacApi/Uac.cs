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
            //Debug.WriteLine("----------Securing page");
            bool canAccess;
            string justification = "";

            if (myUser.LogIn())
            {
                //Debug.WriteLine("----------Securing page, accountType = " + myUser.AccountType);
                
                if (myUser.AccountStatus == "active")
                {
                    string accTypeDescriptor = "";
                    string x = myUser.AccountType.Substring(0, 1).ToLower();
                    if (x == "a" || x == "e" || x == "i" || x == "o" || x == "u" && myUser.AccountType != "user")
                        accTypeDescriptor += "an";
                    else
                        accTypeDescriptor += "a";

                    justification += string.Format("a) {0} account is active and b) ", myUser.PosessivePronoun);

                    string accessGrantedJustification(string pRequiredAccess)
                    {
                        return string.Format("this page requires {0} to have {1} access and {2} is {3} {4} which means that {2} has the required permission level.", myUser.ObjectiveGenderPronoun, pRequiredAccess, myUser.SubjectiveGenderPronoun, accTypeDescriptor, myUser.AccountType);
                    }

                    switch (pAccountType)
                    {
                        case "admin":
                            if (myUser.AccountType == "admin")
                            {
                                justification += accessGrantedJustification("admin");
                                canAccess = true;
                            }
                            else
                                canAccess = false;
                            break;
                        case "member":
                            if (myUser.AccountType == "admin" || myUser.AccountType == "member")
                            {
                                justification += accessGrantedJustification("member");
                                canAccess = true;
                            }
                            else
                                canAccess = false;
                            break;
                        case "user":
                            if (myUser.AccountType == "admin" || myUser.AccountType == "member" || myUser.AccountType == "user")
                            {
                                justification += accessGrantedJustification("user");
                                canAccess = true;
                            }
                            else
                                canAccess = false;
                            break;
                        default:
                            canAccess = false;
                            break;
                    }
                }
                else
                {
                    canAccess = false;
                    new LogEntry(Guid.Empty) { text = string.Format("{0} was denied access to {1} because {2} account is not active!", myUser.FullName, pUrl, myUser.PosessivePronoun) };
                }

                if (canAccess)
                {
                    //Debug.WriteLine("----------{0} is allowed to access {1}", myUser.FullName, pUrl);
                    

                    new LogEntry(myUser.Id) { text = String.Format("{0} accessed the {1} page from {2} '{3}' device. {4} was allowed to access this page because {5}", myUser.FullName, pUrl, myUser.PosessivePronoun, pDeviceType, myUser.Forename, justification) };
                }
                else
                {
                    Debug.WriteLine("----------{0} is NOT allowed to access {1}", myUser.FullName, pUrl);
                    new LogEntry(myUser.Id) { text = String.Format("{0} was denied access to the {1} page because {3} did not have sufficient permissions.", myUser.FullName, pUrl, myUser.PosessivePronoun, pDeviceType) };
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
