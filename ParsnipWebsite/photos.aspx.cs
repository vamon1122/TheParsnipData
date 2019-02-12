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
using ParsnipApi;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ParsnipWebsite
{
    public partial class photos : System.Web.UI.Page
    {
        private User myUser;
        Log DebugLog = new Log("debug");
        protected void Page_Load(object sender, EventArgs e)
        {
            //new LogEntry(Debug) { text = "Loading photos page..." };
            myUser = Uac.SecurePage("photos", this, Data.DeviceType, "member");

            if (IsPostBack && PhotoUpload.PostedFile != null)
            {
                new LogEntry(DebugLog) { text = "POSTBACK with photo" };
                if (PhotoUpload.PostedFile.FileName.Length > 0)
                {
                    try
                    {
                        new LogEntry(DebugLog) { text = "Attempting to upload the photo" };

                        string[] fileDir = PhotoUpload.PostedFile.FileName.Split('\\');
                        string myFileName = fileDir.Last();

                        string newDir = string.Format("resources/media/images/uploads/{0}{1}_{2}_{3}_{4}", myUser.Forename, myUser.Surname,Guid.NewGuid(), Parsnip.adjustedTime.ToString("dd-MM-yyyy"), myFileName);
                        Debug.WriteLine("Newdir = " + newDir);
                        /*if (PhotoUpload.PostedFile.HasFile)
                        {*/
                            PhotoUpload.PostedFile.SaveAs(Server.MapPath("~/" + newDir));
                            Photo temp = new Photo(newDir, myUser);
                            temp.Update();
                        //}
                    }
                    catch (Exception err)
                    {

                        new LogEntry(DebugLog) { text = "There was an exception whilst uploading the photo: " + err };
                    }
                }
            }
        }
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if(myUser.AccountType == "admin" || myUser.AccountType == "member")
            {
                UploadDiv.Style.Clear();
            }
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
                new LogEntry(DebugLog) { text = "Attempting to upload the photo" };

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

                new LogEntry(DebugLog) { text = "There was an exception whilst uploading the photo: " + err };
            }
                
        }

        
    }
}