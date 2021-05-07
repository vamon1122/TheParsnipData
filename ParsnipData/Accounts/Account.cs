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
        public static User PublicPage(Page page, string deviceType)
        {
            var url = page.Request.Url.PathAndQuery.Substring(1);
            var myUser = ParsnipData.Accounts.User.LogIn();

            CanAccess(myUser, page, null, deviceType, null);

            return myUser;
        }

        public static User PublicPage(Page page, MediaShare mediaShare, MediaView mediaView, string deviceType)
        {
            var url = page.Request.Url.PathAndQuery.Substring(1);
            var myUser = ParsnipData.Accounts.User.LogIn();
            var shareCreatedBy = ParsnipData.Accounts.User.Select(mediaShare.UserId);

            mediaView.Video = Video.Select(mediaShare.MediaId, default);
            mediaView.YoutubeVideo = Youtube.Select(mediaShare.MediaId, default);
            mediaView.Image = ParsnipData.Media.Image.Select(mediaShare.MediaId, default);

            if (mediaView.Media != null)
            {
                mediaShare.View(myUser);
                mediaView.Media.ViewCount++;
            }

            var mediaDetail = string.IsNullOrEmpty(mediaView.Media?.Title) ? string.Empty : $": {mediaView.Media.Title}";
            CanAccess(myUser, page, null, deviceType, $"{mediaView.Media.Type}{(mediaView.Media.Type == "youtube" ? " video" : string.Empty)}{mediaDetail}", mediaView.Media);

            return myUser;
        }

        public static User SecurePage(Page page, string deviceType, string accountType = "user", string pageDescription = null)
        {
            var url = page.Request.Url.PathAndQuery.Substring(1);
            CheckDeviceInfo(page, deviceType);
            var myUser = User.LogIn();
            CanAccess(myUser, page, accountType, deviceType, pageDescription);            
            
            return myUser;
        }

        public static User SecurePage(Page page, string deviceType, HttpRequest request, MediaView mediaView)
        {
            var url = page.Request.Url.PathAndQuery.Substring(1);

            CheckDeviceInfo(page, deviceType);

            var myUser = User.LogIn();
            var userId = myUser == null ? default : myUser.Id;

            mediaView.Video = Video.Select(new MediaId(request.QueryString["id"]), userId);
            mediaView.YoutubeVideo = Youtube.Select(new MediaId(request.QueryString["id"]), userId);
            mediaView.Image = ParsnipData.Media.Image.Select(new MediaId(request.QueryString["id"].ToString()), userId);
            var mediaDetail = string.IsNullOrEmpty(mediaView.Media?.Title) ? string.Empty : $": {mediaView.Media.Title}";
            
            CanAccess(myUser, page, "user", deviceType,
                $"{mediaView.Media.Type}{(mediaView.Media.Type == "youtube" ? " video" : string.Empty)}{mediaDetail}", (mediaView.Media == null ? null : mediaView.Media));

            return myUser;
        }

        public static void CheckDeviceInfo(Page page, string deviceType)
        {
            var url = page.Request.Url.PathAndQuery.Substring(1);

            if (string.IsNullOrEmpty(deviceType) || string.IsNullOrWhiteSpace(deviceType))
            {
                page.Response.Redirect("get_device_info?url=" + url);
            }
        }

        public static void CanAccess(User myUser, Page page, string minimumAccountType, string deviceType, string pageDescription, Media.Media media = null)
        {
            var url = page.Request.Url.PathAndQuery.Substring(1);
            bool canAccess = false;
            string justification = "";
            string feedback = "";
            var isViewMedia = url.Contains("view?id=") || url.Contains("view?share=");
            var isSearch = url.Contains("search?text=");
            var a = $"<a href=\"{url}\">{url}</a>";
            var urlNoParams = url.Split('?')[0];
            var aNoParams = $"<a href=\"{urlNoParams}\">{urlNoParams}</a>";

            if (myUser == null && minimumAccountType != null)
            {
                feedback += $"Someone tried to access the {url} page from {new User().PosessivePronoun} {deviceType}. ";
                StoryLog();
                page.Response.Redirect(String.Format("login?url={0}", url));
            }

            //TODO - Determine whether session is new or not
            if (myUser == null)
            { 
                feedback += $"Session ({page.Session.SessionID}) was started or continued from a {deviceType} (not logged in). ";
                feedback += $"They are currently viewing <a href=\"{url}\">{url}</a>{(string.IsNullOrEmpty(pageDescription) ? "" : $" ({pageDescription})")}. ";
                new LogEntry(Log.AccessJustification) { Text = $"The person who was not logged in was allowed to {(page.IsPostBack ? "reload" : "access")} this page because {justification}" };
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

                    if (minimumAccountType == null)
                    {
                        canAccess = true;
                        justification += $"The page does not require the user to ";

                        if (myUser.Id == default)
                            justification += "to be logged in.";
                        else
                            justification += "have any pernissions.";
                    }
                    else
                    {
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
                        new LogEntry(Log.AccessJustification) { Text = $"{feedback}{myUser.Forename} was not allowed to {(page.IsPostBack ? "reload" : "access")} the page because <b>{minimumAccountType}</b> permissions or higher are required wheras {myUser.SubjectiveGenderPronoun} only {(myUser.HasPluralPronoun ? "have" : "has")} <b>{myUser.AccountType}</b> permissions." };
                        StoryLog();
                        page.Response.Redirect(String.Format("access_denied?url={0}", url));
                    }
                }
                else if (minimumAccountType != null)
                {
                    DateTime start = DateTime.Now;
                    while (DateTime.Now < start.AddMilliseconds(1)) { }
                    new LogEntry(Log.AccessJustification) { Text = $"{feedback} {myUser.Forename} was not allowed to {(page.IsPostBack ? "reload" : "access")} to the page because {myUser.PosessivePronoun} account is inactive!" };
                }
            }

            if (minimumAccountType == null)
                canAccess = true;
            
            StoryLog();

            void StoryLog()
            {
                myUser = myUser ?? new User();
                new LogEntry(Log.Access) { Text = $"{myUser.FullName} {(isSearch ? (canAccess ? $"searched" : "tried to search") : (canAccess ? $"viewed" : "tried to view"))}{(isViewMedia ? (media != null && media.Type == "image" || string.IsNullOrEmpty(media.Title) ? " an" : " a" ) + (string.IsNullOrEmpty(media.Title) ? " untitled" : string.Empty) : (isSearch ? string.Empty : " the"))} {(string.IsNullOrEmpty(pageDescription) ? (urlNoParams == url ? a : (isSearch ? string.Empty : urlNoParams)) : $" {pageDescription}")}{(isViewMedia || isSearch ? string.Empty : $" {(url.Contains("tag?") ? "tag" : "page")}")}{(urlNoParams == url ? string.Empty : $" ({a})")}." };
            }
        }
    }
}
