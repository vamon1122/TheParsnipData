using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using MediaApi;
using LogApi;

namespace ParsnipWebsite
{
    public partial class Home : System.Web.UI.Page
    {
        public static readonly Log DebugLog = new Log("Debug");
        private User myUser;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = UacApi.User.GetLoggedInUser();
            if (string.IsNullOrEmpty(Data.DeviceType))
            {
                Response.Redirect("get_device_info?url=home");
            }

            new LogEntry(DebugLog) { text = string.Format("The home page was accessed by {0} from {1} {2} device.", string.IsNullOrEmpty(myUser.Forename) ? "someone who was not logged in" : myUser.Forename, string.IsNullOrEmpty(myUser.PosessivePronoun) ? "their" : myUser.PosessivePronoun, Data.DeviceType) };

            WelcomeLabel.Text = string.IsNullOrEmpty(myUser.Forename) ? "Hiya stranger. Welcome to the parsnip website!" : string.Format("Hiya {0}, welcome back to the parsnip website!", myUser.Forename);

            Video latestVideo = Video.GetLatest();
            LatestVideo.InnerHtml += string.Format("<div><h2>LATEST VIDEO: {0}</h2>", latestVideo.Title);
            LatestVideo.InnerHtml += string.Format("<a href=\"{0}/video_player?videoid={1}\">", Request.Url.GetLeftPart(UriPartial.Authority), latestVideo.Id, latestVideo.Thumbnail);
            LatestVideo.InnerHtml += "<div class=\"play-button-div\">";
            LatestVideo.InnerHtml += string.Format("<img src=\"{2}\" class=\"thumbnail\" />", Request.Url.GetLeftPart(UriPartial.Authority), latestVideo.Id, latestVideo.Thumbnail);
            LatestVideo.InnerHtml += "<span class=\"play-button-icon\"><img src=\"Resources\\Media\\Images\\Web_Media\\play_button_100.png\" /></span>";
            LatestVideo.InnerHtml += "</div></a></div>";

        }
    }
}