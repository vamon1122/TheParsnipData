using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using MediaApi;
using LogApi;
using System.Data.SqlClient;
using ParsnipApi;
using System.Diagnostics;

namespace ParsnipWebsite
{
    public partial class Video_Player : System.Web.UI.Page
    {
        User myUser;
        Log DebugLog = new Log("Debug");
        MediaApi.Video myVideo;
        AccessToken myAccessToken;

        protected void Page_Load(object sender, EventArgs e)
        {
            //We secure the page using the UacApi. 
            //This ensures that the user is logged in etc
            //You only need to change where it says '_NEW TEMPLATE'.
            //Change this to match your page name without the '.aspx' extension.
            //If there is an access token, get the token & it's data.
            //If there is no access token, check that the user is logged in.
            if (Request.QueryString["access_token"] != null)
            {

                myAccessToken = new AccessToken(new Guid(Request.QueryString["access_token"]));
                try
                {
                    myAccessToken.Select();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                myVideo = new MediaApi.Video(myAccessToken.MediaId);
            }
            else
            {
                if (Request.QueryString["videoid"] == null)
                    myUser = Uac.SecurePage("video_player", this, Data.DeviceType, "member");
                else
                    myUser = Uac.SecurePage(string.Format("video_player?videoid={0}", Request.QueryString["videoid"]), this, Data.DeviceType, "member");
                
                if (Request.QueryString["data-id"] == null)
                {
                    if(Request.QueryString["videoid"] == null)
                    {
                        Response.Redirect("home");
                    }
                    else
                    {
                        myVideo = new MediaApi.Video(new Guid(Request.QueryString["videoid"]));
                    }
                }
                else
                {
                    youtube_video.Attributes.Add("data-id", Request.QueryString["data-id"]);
                    
                }
            }
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            //If the image has been deleted, display a warning.
            //If the image has not been deleted, display the image.

            

            if (Request.QueryString["data-id"] == null)
            {
                if (Data.IsMobile)
            {
                video_container.Attributes.Add("preload", "none");
            }
            else
            {
                video_container.Attributes.Add("autoplay", "true");
            }
                myVideo.Select();

                Debug.WriteLine(string.Format("AlbumId {0}", myVideo.AlbumId));

                if (myVideo.AlbumId == Guid.Empty)
                {
                    Debug.WriteLine(string.Format("AlbumId {0} == {1}", myVideo.AlbumId, Guid.Empty));
                    NotExistError.Visible = true;
                    Button_ViewAlbum.Visible = false;
                }
                else
                {
                    Debug.WriteLine(string.Format("AlbumId {0} != {1}", myVideo.AlbumId, Guid.Empty));

                    VideoTitle.InnerText = myVideo.Title;
                    Page.Title = myVideo.Title;
                    VideoSource.Src = myVideo.Directory;
                }
            

            

            //If there was no access token, the user is trying to share the photo.
            //Generate a shareable link and display it on the screen.
            if (Request.QueryString["access_token"] == null)
                {
                    Button_ViewAlbum.Visible = false;

                    AccessToken myAccessToken;

                    if (AccessToken.TokenExists(myUser.Id, myVideo.Id))
                    {
                        myAccessToken = AccessToken.GetToken(myUser.Id, myVideo.Id);
                    }
                    else
                    {
                        myAccessToken = new AccessToken(myUser.Id, myVideo.Id);
                        myAccessToken.Insert();
                    }

                    //Gets URL without sub pages
                    ShareLink.Value = Request.Url.GetLeftPart(UriPartial.Authority) + myAccessToken.VideoRedirect;
                }
                else
                {
                    if (!IsPostBack)
                        myAccessToken.TimesUsed++;

                    User createdBy = new User(myAccessToken.UserId);
                    createdBy.Select();



                    myAccessToken.Update();

                    myVideo = new MediaApi.Video(myAccessToken.MediaId);
                    //myVideo.Select();

                    new LogEntry(DebugLog) { text = string.Format("{0}'s link to {1} got another hit! Now up to {2}", createdBy.FullName, myVideo.Title, myAccessToken.TimesUsed) };

                    ShareLinkContainer.Visible = false;
                }
            }
            else
            {
                youtube_video_container.Visible = true;
                video_container.Visible = false;
                ShareLinkContainer.Visible = false;
                Button_ViewAlbum.Visible = false;
//                youtube_video.Attributes.Add("data-id", Request.QueryString["data-id"]);

                Debug.WriteLine("Data id = " + Request.QueryString["data-id"]);
                //Response.Redirect("home?data-id=" + Request.QueryString["data-id"]);
            }
        }

        protected void Button_ViewAlbum_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/videos");
        }

    }
}