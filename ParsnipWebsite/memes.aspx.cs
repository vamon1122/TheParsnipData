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
    public partial class Memes : System.Web.UI.Page
    {
        private User myUser;
        Log DebugLog = new Log("debug");
        Album MemesAlbum = new Album(new Guid("5F15861A-689C-482A-8E31-2F13429C36E5"));

        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("memes", this, Data.DeviceType, "member");

            if (Request.QueryString["error"] != null)
            {

                Warning.Attributes.CssStyle.Add("display", "block");

            }

            if (IsPostBack && PhotoUpload.PostedFile != null)
            {
                new LogEntry(DebugLog) { text = "POSTBACK with meme image" };
                if (PhotoUpload.PostedFile.FileName.Length > 0)
                {
                    try
                    {
                        new LogEntry(DebugLog) { text = "Attempting to upload the meme image" };

                        string[] fileDir = PhotoUpload.PostedFile.FileName.Split('\\');
                        string myFileName = fileDir.Last();

                        string newDir = string.Format("Resources/Media/Images/Uploads/{0}{1}_{2}_{3}_{4}", myUser.Forename, myUser.Surname, Guid.NewGuid(), Parsnip.adjustedTime.ToString("dd-MM-yyyy"), myFileName);
                        Debug.WriteLine("Newdir = " + newDir);
                        /*if (PhotoUpload.PostedFile.HasFile)
                        {*/
                        PhotoUpload.PostedFile.SaveAs(Server.MapPath("~/" + newDir));
                        MediaApi.Image temp = new MediaApi.Image(newDir, myUser, MemesAlbum);
                        temp.Update();
                        Response.Redirect("edit_image?redirect=memes&imageid=" + temp.Id);
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
            Debug.WriteLine("Starting pageloadcomplete");
            if (myUser.AccountType == "admin" || myUser.AccountType == "member")
            {
                UploadDiv.Style.Clear();
            }
            Debug.WriteLine("Getting all photos");
            List<MediaApi.Image> AllPhotos = MemesAlbum.GetAllImages();
            Debug.WriteLine("Got all photos");
            //new LogEntry(Debug) { text = "Got all photos. There were {0} photo(s) = " + AllPhotos.Count() };
            foreach (MediaApi.Image temp in AllPhotos)
            {
                if (temp.Title != null && !string.IsNullOrEmpty(temp.Title) && !string.IsNullOrWhiteSpace(temp.Title))
                {
                    Debug.WriteLine("Title NOT blank = " + temp.Title);
                    this.Page.Form.FindControl("DynamicPhotosDiv").Controls.Add(new LiteralControl(string.Format("<h2>{0}</h2>", temp.Title)));
                }
                else
                {
                    Debug.WriteLine("Title BLANK = " + temp.Title);
                }

                System.Web.UI.WebControls.Image tempControl = new System.Web.UI.WebControls.Image();

                tempControl.ImageUrl = "Resources/Media/Images/Web_Media/placeholder.gif";
                tempControl.Attributes.Add("data-src", temp.ImageSrc);
                tempControl.Attributes.Add("data-srcset", temp.ImageSrc);
                tempControl.CssClass = "meme lazy";
                DynamicPhotosDiv.Controls.Add(tempControl);
                this.Page.Form.FindControl("DynamicPhotosDiv").Controls.Add(new LiteralControl("<br />"));
                this.Page.Form.FindControl("DynamicPhotosDiv").Controls.Add(new LiteralControl(string.Format("<a href=\"edit_image?redirect=memes&imageid={0}&title={1}\">Edit</a>", temp.Id, temp.Title)));
                this.Page.Form.FindControl("DynamicPhotosDiv").Controls.Add(new LiteralControl(" "));
                this.Page.Form.FindControl("DynamicPhotosDiv").Controls.Add(new LiteralControl(string.Format("<a href=\"view_image?imageid={0}\">Share</a>", temp.Id)));
                this.Page.Form.FindControl("DynamicPhotosDiv").Controls.Add(new LiteralControl("<br />"));
            }

        }

        public Memes()
        {
            MemesAlbum.Select();
        }

        protected void BtnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                new LogEntry(DebugLog) { text = "Attempting to upload the meme image" };

                string newDir = string.Format("Resources/Media/Images/Uploads/{0}{1}_{2}", myUser.Forename, myUser.Surname, PhotoUpload.FileName);
                if (PhotoUpload.HasFile)
                {
                    PhotoUpload.SaveAs(Server.MapPath("~/" + newDir));
                    MediaApi.Image temp = new MediaApi.Image(newDir, myUser, MemesAlbum);
                    temp.Update();
                }
            }
            catch (Exception err)
            {

                new LogEntry(DebugLog) { text = "There was an exception whilst uploading the meme image: " + err };
            }

        }
    }
}