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
    
    

    public partial class View_Image : System.Web.UI.Page
    {
        User myUser;
        Log DebugLog = new Log("Debug");
        MediaApi.Image myImage;
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
                
                var myAccessToken = new AccessToken(new Guid(Request.QueryString["access_token"]));
                try
                {
                    myAccessToken.Select();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                myAccessToken.TimesUsed++;

                User createdBy = new User(myAccessToken.UserId);
                createdBy.Select();

                

                myAccessToken.Update();

                myImage = new MediaApi.Image(myAccessToken.MediaId);
                myImage.Select();

                new LogEntry(DebugLog) { text = string.Format("{0}'s link to {1} got another hit! Now up to {2}", createdBy.FullName, myImage.Title, myAccessToken.TimesUsed) };
            }
            else
            {
                if (Request.QueryString["imageid"] == null)
                    myUser = Uac.SecurePage("view_image", this, Data.DeviceType);
                else
                    myUser = Uac.SecurePage("view_image?imageid=" + Request.QueryString["imageid"], this, Data.DeviceType);

                

                if (Request.QueryString["imageid"] == null)
                    Response.Redirect("home");

                myImage = new MediaApi.Image(new Guid(Request.QueryString["imageid"]));
                myImage.Select();
            }

           //Get the image which the user is trying to access, and display it on the screen.
            

            Debug.WriteLine(string.Format("AlbumId {0}", myImage.AlbumId));

            

            //If the image has been deleted, display a warning.
            //If the image has not been deleted, display the image.
            if (myImage.AlbumId == Guid.Empty)
            {
                Debug.WriteLine(string.Format("AlbumId {0} == {1}", myImage.AlbumId, Guid.Empty));
                NotExistError.Visible = true;
                Button_ViewAlbum.Visible = false;
            }
            else
            {
                Debug.WriteLine(string.Format("AlbumId {0} != {1}", myImage.AlbumId, Guid.Empty));

                ImageTitle.InnerText = myImage.Title;
                Page.Title = myImage.Title;
                ImagePreview.ImageUrl = myImage.Directory;   
            }

            //If there was no access token, the user is trying to share the photo.
            //Generate a shareable link and display it on the screen.
            if (Request.QueryString["access_token"] == null)
            {
                Button_ViewAlbum.Visible = false;

                AccessToken myAccessToken;

                if (AccessToken.TokenExists(myUser.Id, myImage.Id))
                {
                    myAccessToken = AccessToken.GetToken(myUser.Id, myImage.Id);
                }
                else
                {
                    myAccessToken = new AccessToken(myUser.Id, myImage.Id);
                    myAccessToken.Insert();
                }

                //Gets URL without sub pages
                ShareLink.Value = Request.Url.GetLeftPart(UriPartial.Authority) + myAccessToken.Redirect;
            }
            else
            {
                ShareLinkContainer.Visible = false;
            }

        }

        protected void Button_ViewAlbum_Click(object sender, EventArgs e)
        {
            switch (myImage.AlbumId.ToString().ToUpper())
            {
                case "4B4E450A-2311-4400-AB66-9F7546F44F4E":
                    Debug.WriteLine("Redirecting to photos");
                    Response.Redirect("~/photos?imageid=" + myImage.Id);
                    
                    break;

                case "5F15861A-689C-482A-8E31-2F13429C36E5":
                    Debug.WriteLine("Redirecting to memes");
                    Response.Redirect("~/memes?imageid=" + myImage.Id);
                    break;
                default:
                    Debug.WriteLine("Album was wrong! Album = " + myImage.AlbumId.ToString());
                    break;
            }
        }
    }
}