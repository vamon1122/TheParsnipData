using System;
using UacApi;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogApi;
using MediaApi;
using System.Web.UI.HtmlControls;

namespace ParsnipWebsite
{
    public partial class photos : System.Web.UI.Page
    {
        private User myUser;
        Log Debug = new Log("debug");
        protected void Page_Load(object sender, EventArgs e)
        {
            //new LogEntry(Debug) { text = "Loading photos page..." };
            myUser = Uac.SecurePage("photos", this, Data.deviceType, "member");

            
        }
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            
            List<Photo> AllPhotos = Photo.GetAllPhotos();
            //new LogEntry(Debug) { text = "Got all photos. There were {0} photo(s) = " + AllPhotos.Count() };
            foreach (Photo temp in AllPhotos)
            {   
                Image tempControl = new Image();
                
                tempControl.ImageUrl = "resources/media/images/webMedia/pix-vertical-placeholder.jpg";
                tempControl.Attributes.Add("data-src", temp.PhotoSrc);
                tempControl.Attributes.Add("data-srcset", temp.PhotoSrc);
                tempControl.CssClass = "meme lazy";
                DynamicPhotosDiv.Controls.Add(tempControl);
                this.Page.Form.FindControl("DynamicPhotosDiv").Controls.Add(new LiteralControl("<br />"));
                //new LogEntry(Debug) { text = "Added new image to the page. Url = " + temp.PhotoSrc };
            }

        }

        protected void BtnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                new LogEntry(Debug) { text = "Attempting to upload the photo" };

                string newDir = string.Format("resources/media/images/uploads/{0}{1}_{2}", myUser.Forename, myUser.Surname, PhotoUpload.FileName);
                if (PhotoUpload.HasFile)
                {
                    PhotoUpload.SaveAs(Server.MapPath("~/" + newDir));
                    Photo temp = new Photo(newDir, myUser);
                    temp.Update();
                }
            }
            catch (Exception err)
            {

                new LogEntry(Debug) { text = "There was an exception whilst uploading the photo: " + err };
            }
                
        }
    }
}