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
using ParsnipWebsite.Custom_Controls.Media_Api;

namespace ParsnipWebsite
{
    public partial class Photos : System.Web.UI.Page
    {
        private User myUser;
        Log DebugLog = new Log("debug");
        Album PhotosAlbum = new Album(new Guid("4b4e450a-2311-4400-ab66-9f7546f44f4e"));

        public Photos()
        {
            PhotosAlbum.Select();
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            myUser = await Uac.SecurePage("photos", this, Data.DeviceType, "member");

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
                        string myFileExtension = myFileName.Split('.').Last().ToLower();

                        if (MediaApi.Image.IsValidFileExtension(myFileExtension))
                        {

                            string newDir = string.Format("Resources/Media/Images/Uploads/{0}{1}_{2}_{3}_{4}", myUser.Forename, myUser.Surname, Guid.NewGuid(), Parsnip.adjustedTime.ToString("dd-MM-yyyy"), myFileName);
                            Debug.WriteLine("Newdir = " + newDir);
                            /*if (PhotoUpload.PostedFile.HasFile)
                            {*/
                            PhotoUpload.PostedFile.SaveAs(Server.MapPath("~/" + newDir));
                            MediaApi.Image temp = new MediaApi.Image(newDir, myUser, PhotosAlbum);
                            temp.Update();
                            Response.Redirect("edit_image?redirect=photos&imageid=" + temp.Id);
                            //}
                        }
                        else
                        {
                            Response.Redirect("photos?error=video");
                        }
                    }
                    catch (Exception err)
                    {

                        new LogEntry(DebugLog) { text = "There was an exception whilst uploading the photo: " + err };
                    }
                }
            }

            if (myUser.AccountType == "admin" || myUser.AccountType == "member")
            {
                UploadDiv.Style.Clear();
            }
            Debug.WriteLine("Getting all photos");
            List<MediaApi.Image> AllPhotos = PhotosAlbum.GetAllImages();
            Debug.WriteLine("Got all photos");
            //new LogEntry(Debug) { text = "Got all photos. There were {0} photo(s) = " + AllPhotos.Count() };
            foreach (MediaApi.Image temp in AllPhotos)
            {
                var MyImageControl = (ImageControl)LoadControl("~/Custom_Controls/Media_Api/ImageControl.ascx");
                MyImageControl.MyImage = temp;
                DynamicPhotosDiv.Controls.Add(MyImageControl);
            }

            /*
            if (Request.QueryString["imageid"] != null)
            {
                var pImage = new MediaApi.Image(new Guid(Request.QueryString["imageid"]));
                pImage.Select();

                
            }
            */
        }

        protected void BtnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                new LogEntry(DebugLog) { text = "Attempting to upload the photo image" };

                string newDir = string.Format("Resources/Media/Images/Uploads/{0}{1}_{2}", myUser.Forename, myUser.Surname, PhotoUpload.FileName);
                if (PhotoUpload.HasFile)
                {
                    PhotoUpload.SaveAs(Server.MapPath("~/" + newDir));
                    MediaApi.Image temp = new MediaApi.Image(newDir, myUser, PhotosAlbum);
                    temp.Update();
                }
            }
            catch (Exception err)
            {

                new LogEntry(DebugLog) { text = "There was an exception whilst uploading the photo image: " + err };
            }

        }


    }
}