using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParsnipData.Logs;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace ParsnipData.Accounts
{
    public static class Account
    {
        private static readonly Log PageAccessLog = Log.Select(1);
        private static readonly Log PageAccessJustificationLog = Log.Select(2);
        private static readonly Log DebugLog = Log.Select(3);
        private static readonly Log SessionLog = Log.Select(6);

        public static User SecurePage(string url, Page page, string deviceType, string accountType)
        {
            if (string.IsNullOrEmpty(deviceType) || string.IsNullOrWhiteSpace(deviceType))
            {
                page.Response.Redirect("get_device_info?url=" + url);
            }

            var myUser = User.LogIn();
            bool canAccess;
            string justification = "";

            if (myUser == null)
            {
                new LogEntry(PageAccessLog) { text = String.Format("Someone tried to access the {0} page from {1} {2}, without logging in!", url, new User().PosessivePronoun, deviceType) };
                page.Response.Redirect(String.Format("login?url={0}", url));
            }
            else
            {
                if (page.Session["userName"] == null)
                {
                    page.Session["userName"] = myUser.Username;

                    string logString = string.Format("{0} started a new session from {1} {2}. Session ID = {3}", myUser.FullName, myUser.PosessivePronoun, deviceType, page.Session.SessionID.ToString());
                    new LogEntry(Log.Select(4)) { text = logString };
                    new LogEntry(SessionLog) { text = logString };
                }
                else
                    new LogEntry(SessionLog) { text = string.Format("{0} continued {1} session on {1} {2}. Session ID = {3}", myUser.FullName, myUser.PosessivePronoun, deviceType, page.Session.SessionID.ToString()) };




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

                    switch (accountType)
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
                        case "media":
                            if (myUser.AccountType == "admin" || myUser.AccountType == "media")
                            {
                                justification += accessGrantedJustification("media");
                                canAccess = true;
                            }
                            else
                                canAccess = false;
                            break;
                        case "member":
                            if (myUser.AccountType == "admin" || myUser.AccountType == "media" || myUser.AccountType == "member")
                            {
                                justification += accessGrantedJustification("member");
                                canAccess = true;
                            }
                            else
                                canAccess = false;
                            break;
                        case "user":
                            if (myUser.AccountType == "admin" || myUser.AccountType == "media" || myUser.AccountType == "member" || myUser.AccountType == "user")
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
                        if (!page.IsPostBack)
                        {
                            new LogEntry(PageAccessLog) { text = String.Format("{0} accessed the {1} page from {2} {3}.", myUser.FullName, url, myUser.PosessivePronoun, deviceType, myUser.Forename, justification) };
                            DateTime start = DateTime.Now;
                            while (DateTime.Now < start.AddMilliseconds(1)) { }
                            new LogEntry(PageAccessJustificationLog) { text = String.Format("{0} was allowed to access the {1} page because {2}", myUser.FullName, url, justification) };
                        }
                        else
                        {
                            new LogEntry(PageAccessLog) { text = String.Format("{0} reloaded the {1} page from {2} {3}.", myUser.FullName, url, myUser.PosessivePronoun, deviceType, myUser.Forename, justification) };
                        }
                    }
                    else
                    {
                        if (!page.IsPostBack)
                        {
                            new LogEntry(PageAccessLog) { text = String.Format("{0} tried to access the {1} page but access was denied.", myUser.FullName, url) };
                            DateTime start = DateTime.Now;
                            while (DateTime.Now < start.AddMilliseconds(1)) { }
                            new LogEntry(PageAccessJustificationLog) { text = String.Format("{0} was denied access to the {1} page because {2} did not have sufficient permissions.", myUser.FullName, url, myUser.PosessivePronoun) };
                            page.Response.Redirect(String.Format("access_denied?url={0}", url));
                        }
                        else
                        {
                            new LogEntry(PageAccessLog) { text = String.Format("{0} tried to reload the page to access the {1} page but access was denied.", myUser.FullName, url) };
                        }
                    }

                }
                else
                {
                    canAccess = false;
                    if (!page.IsPostBack)
                    {
                        new LogEntry(PageAccessLog) { text = string.Format("{0} tried to access the {1} page from {2} {3} but access was denied.", myUser.FullName, url, myUser.PosessivePronoun, deviceType) };
                        DateTime start = DateTime.Now;
                        while (DateTime.Now < start.AddMilliseconds(1)) { }
                        new LogEntry(PageAccessJustificationLog) { text = string.Format("{0} was denied access to the {1} page because {2} account is not active!", myUser.FullName, url, myUser.PosessivePronoun) };
                    }
                    else
                    {
                        new LogEntry(PageAccessLog) { text = string.Format("{0} tried to reload the page to access the {1} page from {2} {3} but access was denied.", myUser.FullName, url, myUser.PosessivePronoun, deviceType) };
                    }
                }




            }
            

            return myUser;
        }
        public static User SecurePage(string url, Page page, string deviceType)
        {
            return SecurePage(url, page, deviceType, "user");
        }

        //public static User SecurePage(string url, Page page, string deviceType, string accountType)
        //{
        //    //if(string.IsNullOrEmpty(deviceType) || string.IsNullOrWhiteSpace(deviceType))
        //    //{
        //    //    //new LogEntry(Log.Default) { text = "Attempted to secure the page but deviceInfo was incomplete. Getting device info..." };
        //    //    page.Response.Redirect("get_device_info?url=" + url);
        //    //}
        //    //else
        //    //{
        //    //    //new LogEntry(Log.Default) { text = "Secure page - Device info is already complete!" };
        //    //}


        //    var myUser = new User("Uac.SecurePage(4)");
        //    bool canAccess;
        //    string justification = "";


        //    if (User.LogIn() != null)
        //    {
        //        if (page.Session["userName"] == null)
        //        {
        //            page.Session["userName"] = myUser.Username;

        //            string logString = string.Format("{0} started a new session from {1} {2}. Session ID = {3}", myUser.FullName, myUser.PosessivePronoun, deviceType, page.Session.SessionID.ToString());
        //            new LogEntry(Log.Select("General")) { text = logString };
        //            new LogEntry(SessionLog) { text = logString };
        //        }
        //        else
        //            new LogEntry(SessionLog) { text = string.Format("{0} continued {1} session on {1} {2}. Session ID = {3}", myUser.FullName, myUser.PosessivePronoun, deviceType, page.Session.SessionID.ToString()) };




        //        if (myUser.AccountStatus == "active")
        //        {
        //            string accTypeDescriptor = "";
        //            string x = myUser.AccountType.Substring(0, 1).ToLower();
        //            if (x == "a" || x == "e" || x == "i" || x == "o" || x == "u" && myUser.AccountType != "user")
        //                accTypeDescriptor += "an";
        //            else
        //                accTypeDescriptor += "a";

        //            justification += string.Format("a) {0} account is active and b) ", myUser.PosessivePronoun);

        //            string accessGrantedJustification(string pRequiredAccess)
        //            {
        //                return string.Format("this page requires {0} to have {1} access and {2} is {3} {4} which means that {2} has the required permission level.", myUser.ObjectiveGenderPronoun, pRequiredAccess, myUser.SubjectiveGenderPronoun, accTypeDescriptor, myUser.AccountType);
        //            }

        //            switch (accountType)
        //            {
        //                case "admin":
        //                    if (myUser.AccountType == "admin")
        //                    {
        //                        justification += accessGrantedJustification("admin");
        //                        canAccess = true;
        //                    }
        //                    else
        //                        canAccess = false;
        //                    break;
        //                case "media":
        //                    if (myUser.AccountType == "admin" || myUser.AccountType == "media")
        //                    {
        //                        justification += accessGrantedJustification("media");
        //                        canAccess = true;
        //                    }
        //                    else
        //                        canAccess = false;
        //                    break;
        //                case "member":
        //                    if (myUser.AccountType == "admin" || myUser.AccountType == "media" || myUser.AccountType == "member")
        //                    {
        //                        justification += accessGrantedJustification("member");
        //                        canAccess = true;
        //                    }
        //                    else
        //                        canAccess = false;
        //                    break;
        //                case "user":
        //                    if (myUser.AccountType == "admin" || myUser.AccountType == "media" || myUser.AccountType == "member" || myUser.AccountType == "user")
        //                    {
        //                        justification += accessGrantedJustification("user");
        //                        canAccess = true;
        //                    }
        //                    else
        //                        canAccess = false;
        //                    break;
        //                default:
        //                    canAccess = false;
        //                    break;
        //            }

        //            if (canAccess)
        //            {
        //                if (!page.IsPostBack)
        //                {
        //                    new LogEntry(PageAccessLog) { text = String.Format("{0} accessed the {1} page from {2} {3}.", myUser.FullName, url, myUser.PosessivePronoun, deviceType, myUser.Forename, justification) };
        //                    DateTime start = DateTime.Now;
        //                    while (DateTime.Now < start.AddMilliseconds(1)) { }
        //                    new LogEntry(PageAccessJustificationLog) { text = String.Format("{0} was allowed to access the {1} page because {2}", myUser.FullName, url, justification) };
        //                }
        //                else
        //                {
        //                    new LogEntry(PageAccessLog) { text = String.Format("{0} reloaded the {1} page from {2} {3}.", myUser.FullName, url, myUser.PosessivePronoun, deviceType, myUser.Forename, justification) };
        //                }
        //            }
        //            else
        //            {
        //                if (!page.IsPostBack)
        //                {
        //                    new LogEntry(PageAccessLog) { text = String.Format("{0} tried to access the {1} page but access was denied.", myUser.FullName, url) };
        //                    DateTime start = DateTime.Now;
        //                    while (DateTime.Now < start.AddMilliseconds(1)) { }
        //                    new LogEntry(PageAccessJustificationLog) { text = String.Format("{0} was denied access to the {1} page because {2} did not have sufficient permissions.", myUser.FullName, url, myUser.PosessivePronoun) };
        //                    page.Response.Redirect(String.Format("access_denied?url={0}", url));
        //                }
        //                else
        //                {
        //                    new LogEntry(PageAccessLog) { text = String.Format("{0} tried to reload the page to access the {1} page but access was denied.", myUser.FullName, url) };
        //                }
        //            }

        //        }
        //        else
        //        {
        //            canAccess = false;
        //            if (!page.IsPostBack)
        //            {
        //                new LogEntry(PageAccessLog) { text = string.Format("{0} tried to access the {1} page from {2} {3} but access was denied.", myUser.FullName, url, myUser.PosessivePronoun, deviceType) };
        //                DateTime start = DateTime.Now;
        //                while (DateTime.Now < start.AddMilliseconds(1)) { }
        //                new LogEntry(PageAccessJustificationLog) { text = string.Format("{0} was denied access to the {1} page because {2} account is not active!", myUser.FullName, url, myUser.PosessivePronoun) };
        //            }
        //            else
        //            {
        //                new LogEntry(PageAccessLog) { text = string.Format("{0} tried to reload the page to access the {1} page from {2} {3} but access was denied.", myUser.FullName, url, myUser.PosessivePronoun, deviceType) };
        //            }
        //        }




        //    }
        //    else
        //    {
        //        new LogEntry(PageAccessLog) { text = String.Format("Someone tried to access the {0} page from {1} {2}, without logging in!", url, myUser.PosessivePronoun, deviceType) };
        //        page.Response.Redirect(String.Format("login?url={0}", url));
        //    }

        //    return myUser;
        //}
        //public static User SecurePage(string url, Page page, string deviceType)
        //{
        //    return SecurePage(url, page, deviceType, "user");
        //}
    }
}
