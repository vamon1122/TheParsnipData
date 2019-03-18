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
using BenLog;
using ParsnipApi;

namespace UacApi
{
    public static class Uac
    {
        private static readonly Log PageAccessLog = new Log("access");
        private static readonly Log PageAccessJustificationLog = new Log("access justification");
        private static readonly Log DebugLog = new Log("debug");
        private static readonly Log SessionLog = new Log("session");

        public static void SecurePage(string pSecurePageUrl, Page pSecurePage, string pDeviceType, string pRequiredAccountType, User myUser)
        {
            Debug.WriteLine("Securing page...");
            Parsnip.AsyncLog.WriteLog("Securing page...");

            if(myUser.Id == Guid.Empty)
            {
                Debug.WriteLine("User was not logged in. Redirecting to login");
                Parsnip.AsyncLog.WriteLog("[NewSecurePage] User was not logged in. Redirecting to login");

                //The false stops threadabortexeption
                if (!pSecurePage.Response.IsRequestBeingRedirected)
                    pSecurePage.Response.Redirect("login?url=" + pSecurePageUrl, false);
            }

            if (string.IsNullOrEmpty(pDeviceType) || string.IsNullOrWhiteSpace(pDeviceType))
            {
                Debug.WriteLine("Devicetype is empty, getting device info...");
                //new LogEntry(Log.Default) { text = "Attempted to secure the page but deviceInfo was incomplete. Getting device info..." };

                //The false stops threadabortexeption
                if (!pSecurePage.Response.IsRequestBeingRedirected)
                    pSecurePage.Response.Redirect("get_device_info?url=" + pSecurePageUrl, false);
            }
            else
            {
                Debug.WriteLine("Devicetype is NOT empty...");
                //new LogEntry(Log.Default) { text = "Secure page - Device info is already complete!" };
            }


            //Debug.WriteLine("----------Securing page");
            bool canAccess;
            string justification = "";

            if (pSecurePage.Session["userName"] == null)
            {
                pSecurePage.Session["userName"] = myUser.Username;
                new LogEntry(SessionLog) { text = string.Format("{0} started a new session from {1} {2}. Session ID = {3}.", myUser.FullName, myUser.PosessivePronoun, pDeviceType, pSecurePage.Session.SessionID.ToString()) };
            }
            else
                new LogEntry(SessionLog) { text = string.Format("{0} continued {1} session on {1} {2}. Session ID = {3}.", myUser.FullName, myUser.PosessivePronoun, pDeviceType, pSecurePage.Session.SessionID.ToString()) };




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

                switch (pRequiredAccountType)
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
                    if (!pSecurePage.IsPostBack)
                    {
                        new LogEntry(PageAccessLog) { text = String.Format("{0} accessed the {1} page from {2} {3}.", myUser.FullName, pSecurePageUrl, myUser.PosessivePronoun, pDeviceType, myUser.Forename, justification) };
                        DateTime start = DateTime.Now;
                        while (DateTime.Now < start.AddMilliseconds(1)) { }
                        new LogEntry(PageAccessJustificationLog) { text = String.Format("{0} was allowed to access the {1} page because {2}", myUser.FullName, pSecurePageUrl, justification) };
                    }
                    else
                    {
                        new LogEntry(PageAccessLog) { text = String.Format("{0} reloaded the {1} page from {2} {3}.", myUser.FullName, pSecurePageUrl, myUser.PosessivePronoun, pDeviceType, myUser.Forename, justification) };
                    }
                }
                else
                {
                    if (!pSecurePage.IsPostBack)
                    {
                        Debug.WriteLine("----------{0} is NOT allowed to access {1}", myUser.FullName, pSecurePageUrl);
                        new LogEntry(PageAccessLog) { text = String.Format("{0} tried to access the {1} page but access was denied.", myUser.FullName, pSecurePageUrl) };
                        DateTime start = DateTime.Now;
                        while (DateTime.Now < start.AddMilliseconds(1)) { }
                        new LogEntry(PageAccessJustificationLog) { text = String.Format("{0} was denied access to the {1} page because {2} did not have sufficient permissions.", myUser.FullName, pSecurePageUrl, myUser.PosessivePronoun) };
                        if (!pSecurePage.Response.IsRequestBeingRedirected)
                            pSecurePage.Response.Redirect(string.Format("access_denied?url={0}", pSecurePageUrl));
                    }
                    else
                    {
                        new LogEntry(PageAccessLog) { text = String.Format("{0} tried to reload the page to access the {1} page but access was denied.", myUser.FullName, pSecurePageUrl) };
                    }
                }

            }
            else
            {
                canAccess = false;
                if (!pSecurePage.IsPostBack)
                {
                    new LogEntry(PageAccessLog) { text = string.Format("{0} tried to access the {1} page from {2} {3} but access was denied.", myUser.FullName, pSecurePageUrl, myUser.PosessivePronoun, pDeviceType) };
                    DateTime start = DateTime.Now;
                    while (DateTime.Now < start.AddMilliseconds(1)) { }
                    new LogEntry(PageAccessJustificationLog) { text = string.Format("{0} was denied access to the {1} page because {2} account is not active!", myUser.FullName, pSecurePageUrl, myUser.PosessivePronoun) };
                }
                else
                {
                    
                        new LogEntry(PageAccessLog) { text = string.Format("{0} tried to reload the page to access the {1} page from {2} {3} but access was denied.", myUser.FullName, pSecurePageUrl, myUser.PosessivePronoun, pDeviceType) };
                }
            }

            if (canAccess)
            {
                Debug.WriteLine(string.Format("[New SecurePage] {0} was allowed to access the {1} page because {2}", myUser.Forename, pSecurePageUrl, justification));
            }
            else
            {
                Debug.WriteLine(string.Format("[New SecurePage] {0} was NOT allowed to access the {1} page because {2}", myUser.Forename, pSecurePageUrl, justification));


                //The false stops threadabortexeption
                if (!pSecurePage.Response.IsRequestBeingRedirected)
                    pSecurePage.Response.Redirect("login?url=" + pSecurePageUrl, false);
            }


        }

            
        
    }
}
