using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParsnipData.Logging;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using ParsnipData.Media;

namespace ParsnipData.Accounts
{
    public static class Account
    {
        public static User SecurePage(string url, Page page, string deviceType, string accountType = "user", string pageDescription = null)
        {
            CheckDeviceInfo(page, deviceType, url);

            var myUser = User.LogIn();
            
            CanAccess(myUser, page, url, accountType, deviceType, pageDescription);            

            return myUser;
        }

        public static User SecurePage(string url, Page page, string deviceType, HttpRequest request, MediaView mediaView)
        {
            CheckDeviceInfo(page, deviceType, url);

            var myUser = User.LogIn();

            mediaView.Video = Video.Select(new MediaId(request.QueryString["id"]), myUser.Id);
            mediaView.YoutubeVideo = Youtube.Select(new MediaId(request.QueryString["id"]), myUser.Id);
            mediaView.Image = ParsnipData.Media.Image.Select(new MediaId(request.QueryString["id"].ToString()), myUser == null ? default : myUser.Id);
            var mediaDetail = string.IsNullOrEmpty(mediaView.Media.Title) ? string.Empty : $": {mediaView.Media.Title}";
            
            CanAccess(myUser, page, url, "user", deviceType,
                $"{char.ToUpper(mediaView.Media.Type[0]) + mediaView.Media.Type.Substring(1)}{mediaDetail}");

            return myUser;
        }

        public static void CheckDeviceInfo(Page page, string deviceType, string url)
        {
            if (string.IsNullOrEmpty(deviceType) || string.IsNullOrWhiteSpace(deviceType))
            {
                page.Response.Redirect("get_device_info?url=" + url);
            }
        }

        public static void CanAccess(User myUser, Page page, string url, string minimumAccountType, string deviceType, string pageDescription)
        {
            bool canAccess = false;
            string justification = "";
            string feedback = "";
            if (myUser == null)
            {
                feedback += $"Someone tried to access the {url} page from {new User().PosessivePronoun} {deviceType}. ";
                page.Response.Redirect(String.Format("login?url={0}", url));
            }
            else
            {
                if (page.Session["userName"] == null)
                {
                    page.Session["userName"] = myUser.Username;
                    feedback += $"{myUser.FullName} started a <b>new session</b> ({page.Session.SessionID}) from {myUser.PosessivePronoun} {deviceType}. ";
                }
                else
                    feedback += $"{myUser.FullName} continued {myUser.PosessivePronoun} session ({page.Session.SessionID}) on {myUser.PosessivePronoun} {deviceType}. ";


                if (myUser.AccountStatus == "active")
                {
                    string accTypeDescriptor = "";
                    string x = myUser.AccountType.Substring(0, 1).ToLower();
                    if (x == "a" || x == "e" || x == "i" || x == "o" || x == "u" && myUser.AccountType != "user")
                        accTypeDescriptor += "an";
                    else
                        accTypeDescriptor += "a";

                    justification += $"a) {myUser.PosessivePronoun.First().ToString().ToUpper()}{myUser.PosessivePronoun.Substring(1)} account is active and b) ";

                    string accessGrantedJustification(string pRequiredAccess)
                    {
                        return $"The page requires <b>{minimumAccountType}</b> permissions or higher and {myUser.SubjectiveGenderPronoun} is {accTypeDescriptor} <b>{myUser.AccountType}</b>.";
                    }

                    switch (minimumAccountType)
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

                    feedback += $"{myUser.SubjectiveGenderPronoun.First().ToString().ToUpper()}{myUser.SubjectiveGenderPronoun.Substring(1)} {(canAccess ? $"{(myUser.HasPluralPronoun ? "are" : "is")} currently viewing" : "tried to view")} <a href=\"{url}\">{url}</a>{(string.IsNullOrEmpty(pageDescription) ? "" : $" ({pageDescription})")}. ";

                    if (canAccess)
                    {
                        DateTime start = DateTime.Now;
                        while (DateTime.Now < start.AddMilliseconds(1)) { }
                        new LogEntry(Log.AccessJustification) { Text = $"{feedback}{myUser.Forename} is allowed to {(page.IsPostBack ? "reload" : "access")} this page because {justification}" };
                    }
                    else
                    {
                        DateTime start = DateTime.Now;
                        while (DateTime.Now < start.AddMilliseconds(1)) { }
                        new LogEntry(Log.AccessJustification) { Text = $"{feedback} {myUser.Forename} was not allowed to {(page.IsPostBack ? "reload" : "access")} the page because <b>{minimumAccountType}</b> permissions or higher are required wheras {myUser.SubjectiveGenderPronoun} only {(myUser.HasPluralPronoun ? "have" : "has")} <b>{myUser.AccountType}</b> permissions." };
                        page.Response.Redirect(String.Format("access_denied?url={0}", url));
                    }
                }
                else
                {
                    DateTime start = DateTime.Now;
                    while (DateTime.Now < start.AddMilliseconds(1)) { }
                    new LogEntry(Log.AccessJustification) { Text = $"{feedback} {myUser.Forename} was not allowed to {(page.IsPostBack ? "reload" : "access")} to the page because {myUser.PosessivePronoun} account is inactive!" };
                }
            }
        }
    }
}
