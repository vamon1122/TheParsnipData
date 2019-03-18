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

namespace ParsnipWebsite
{
    public partial class View_Image : System.Web.UI.Page
    {
        User myUser;
        Log DebugLog = new Log("Debug");
        protected async void Page_Load(object sender, EventArgs e)
        {
            //We secure the page using the UacApi. 
            //This ensures that the user is logged in etc
            //You only need to change where it says '_NEW TEMPLATE'.
            //Change this to match your page name without the '.aspx' extension.

            myUser = await UacApi.User.CookieLogIn();
            

            if (Request.QueryString["imageid"] == null)
                Uac.NewSecurePage("view_image", this, Data.DeviceType, "member", myUser);
            else
                Uac.NewSecurePage("view_image?imageid=" + Request.QueryString["imageid"], this, Data.DeviceType, "member", myUser);

            if (Request.QueryString["imageid"] != null)
            {
                MediaApi.Image temp = new MediaApi.Image(new Guid(Request.QueryString["imageid"]));
                temp.Select();

                Album tempAlbum = new Album(temp.AlbumIds().First());
                tempAlbum.Select();

                Album.Text = tempAlbum.Name;
                AlbumId.Text = tempAlbum.Id.ToString();

                if (Request.QueryString["title"] == null)
                {
                    Response.Redirect(string.Format("view_image?imageid={0}&title={1}", temp.Id, temp.Title));
                }
                else
                {
                    ImagePreview.ImageUrl = temp.ImageSrc;
                }

                    
            }
            else
            {
                Response.Redirect("home");
            }
        }

        
    }
}