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
        private static Log PageAccessLog = new Log("access");
        private static Log PageAccessJustificationLog = new Log("access justification");
        private static Log DebugLog = new Log("debug");
        private static Log SessionLog = new Log("session");
        public static User SecurePage(string pUrl, Page pPage, string pDeviceType, string pAccountType)
        {
            Debug.WriteLine("Securing page...");

            if(string.IsNullOrEmpty(pDeviceType) || string.IsNullOrWhiteSpace(pDeviceType))
            {
                Debug.WriteLine("Devicetype is empty, getting device info...");
                //new LogEntry(Log.Default) { text = "Attempted to secure the page but deviceInfo was incomplete. Getting device info..." };
                pPage.Response.Redirect("get-device-info?url=" + pUrl);
            }
            else
            {
                Debug.WriteLine("Devicetype is NOT empty...");
                //new LogEntry(Log.Default) { text = "Secure page - Device info is already complete!" };
            }


            var myUser = new User("Uac.SecurePage(4)");
            //Debug.WriteLine("----------Securing page");
            bool canAccess;
            string justification = "";


            Debug.WriteLine("Attempting to log user in...");
            if (myUser.LogIn())
            {
                Debug.WriteLine("User logged in");
                //Debug.WriteLine("----------Securing page, accountType = " + myUser.AccountType);

                if (pPage.Session["userName"] == null)
                {
                    pPage.Session["userName"] = myUser.Username;
                    new LogEntry(SessionLog) { text = string.Format("{0} started a new session from {1} {2}. Session ID = {3}.",myUser.FullName, myUser.PosessivePronoun, pDeviceType, pPage.Session.SessionID.ToString()) };
                }
                else
                    new LogEntry(SessionLog) { text = string.Format("{0} continued {1} session on {1} {2}. Session ID = {3}.", myUser.FullName, myUser.PosessivePronoun, pDeviceType, pPage.Session.SessionID.ToString()) };




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

                    if (canAccess)
                    {
                        //Debug.WriteLine("----------{0} is allowed to access {1}", myUser.FullName, pUrl);
                        if (!pPage.IsPostBack)
                        {
                            new LogEntry(PageAccessLog) { text = String.Format("{0} accessed the {1} page from {2} {3}.", myUser.FullName, pUrl, myUser.PosessivePronoun, pDeviceType, myUser.Forename, justification) };
                            DateTime start = DateTime.Now;
                            while (DateTime.Now < start.AddMilliseconds(1)) { }
                            new LogEntry(PageAccessJustificationLog) { text = String.Format("{0} was allowed to access the {1} page because {2}", myUser.FullName, pUrl, justification) };
                        }
                        else
                        {
                            new LogEntry(PageAccessLog) { text = String.Format("{0} reloaded the {1} page from {2} {3}.", myUser.FullName, pUrl, myUser.PosessivePronoun, pDeviceType, myUser.Forename, justification) };
                        }
                    }
                    else
                    {
                        if (!pPage.IsPostBack)
                        {
                            Debug.WriteLine("----------{0} is NOT allowed to access {1}", myUser.FullName, pUrl);
                            new LogEntry(PageAccessLog) { text = String.Format("{0} tried to access the {1} page but access was denied.", myUser.FullName, pUrl) };
                            DateTime start = DateTime.Now;
                            while (DateTime.Now < start.AddMilliseconds(1)) { }
                            new LogEntry(PageAccessJustificationLog) { text = String.Format("{0} was denied access to the {1} page because {2} did not have sufficient permissions.", myUser.FullName, pUrl, myUser.PosessivePronoun) };
                            pPage.Response.Redirect(String.Format("access-denied?url={0}", pUrl));
                        }
                        else
                        {
                            new LogEntry(PageAccessLog) { text = String.Format("{0} tried to reload the page to access the {1} page but access was denied.", myUser.FullName, pUrl) };
                        }
                    }

                }
                else
                {
                    canAccess = false;
                    if (!pPage.IsPostBack)
                    {
                        new LogEntry(PageAccessLog) { text = string.Format("{0} tried to access the {1} page from {2} {3} but access was denied.", myUser.FullName, pUrl, myUser.PosessivePronoun, pDeviceType) };
                        DateTime start = DateTime.Now;
                        while (DateTime.Now < start.AddMilliseconds(1)) { }
                        new LogEntry(PageAccessJustificationLog) { text = string.Format("{0} was denied access to the {1} page because {2} account is not active!", myUser.FullName, pUrl, myUser.PosessivePronoun) };
                    }
                    else
                    {
                        new LogEntry(PageAccessLog) { text = string.Format("{0} tried to reload the page to access the {1} page from {2} {3} but access was denied.", myUser.FullName, pUrl, myUser.PosessivePronoun, pDeviceType) };
                    }
                }

                

                
            }
            else
            {
                new LogEntry(PageAccessLog) { text = String.Format("Someone tried to access the {0} page from {1} {2}, without logging in!", pUrl, myUser.PosessivePronoun, pDeviceType) };
                pPage.Response.Redirect(String.Format("login?url={0}", pUrl));
            }

            return myUser;
        }

        public static User SecurePage(string pUrl, Page pPage, string pDeviceType)
        {
            Debug.WriteLine("Attempting to secure page in...");
            return SecurePage(pUrl, pPage, pDeviceType, "user");
        }
    }
}
