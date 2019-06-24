using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;

namespace ParsnipWebsite
{
    public partial class Videos : System.Web.UI.Page
    {
        private User myUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("videos", this, Data.DeviceType, "member");

            List<MediaApi.Video> allVideos = MediaApi.Video.GetAllVideos();

            foreach(MediaApi.Video video in allVideos)
            {
                
                links_div.InnerHtml += "<hr class=\"break\" />";
                links_div.InnerHtml += string.Format("<h2>{0}</ h2>", video.Title);
                links_div.InnerHtml += string.Format("<a href=\"{0}/video_player?videoid={1}\">", Request.Url.GetLeftPart(UriPartial.Authority), video.Id, video.Thumbnail);
                links_div.InnerHtml += "<div class=\"play-button-div\">";
                links_div.InnerHtml += string.Format("<img src=\"{2}\" class=\"thumbnail\" />", Request.Url.GetLeftPart(UriPartial.Authority), video.Id, video.Thumbnail);
                links_div.InnerHtml += "<span class=\"play-button-icon\"><img src=\"Resources\\Media\\Images\\Web_Media\\play_button_100.png\" /></span>";
                links_div.InnerHtml += "</div></a><!--End-->";
            }
                
        }


    }
}