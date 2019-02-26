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
    public partial class Edit_Image : System.Web.UI.Page
    {
        User myUser;
        Log DebugLog = new Log("Debug");
        MediaApi.Image MyImage;
        protected void Page_Load(object sender, EventArgs e)
        {
            //REQUIRED TO VIEW POSTBACK
            form1.Action = Request.RawUrl;

            //We secure the page using the UacApi. 
            //This ensures that the user is logged in etc
            //You only need to change where it says '_NEW TEMPLATE'.
            //Change this to match your page name without the '.aspx' extension.

            if (Request.QueryString["imageid"] == null)
                myUser = Uac.SecurePage("edit_image", this, Data.DeviceType);
            else
                myUser = Uac.SecurePage("edit_image?imageid=" + Request.QueryString["imageid"], this, Data.DeviceType);

            //myUser = Uac.SecurePage("edit_image", this, Data.DeviceType);

            if (Request.QueryString["imageid"] != null)
            {
                MyImage = new MediaApi.Image(new Guid(Request.QueryString["imageid"]));
                MyImage.Select();

                if (IsPostBack)
                {
                    /*
                    new LogEntry(DebugLog) { text = "Posted back title3 = " + Request["InputTitleTwo"].ToString() };
                    new LogEntry(DebugLog) { text = "Posted back albumid3 = " + Request["NewAlbumsDropDown"].ToString() };
                    */
                    
                    MyImage.Title = Request["InputTitleTwo"].ToString();
                    MyImage.AlbumId = new Guid(Request["NewAlbumsDropDown"].ToString());
                    MyImage.Update();

                    string Redirect;

                    switch (Request["NewAlbumsDropDown"].ToString().ToUpper())
                    {
                        case "4b4e450a-2311-4400-ab66-9f7546f44f4e":
                            Redirect = "photos";
                            break;
                        case "5F15861A-689C-482A-8E31-2F13429C36E5":
                            Redirect = "memes";
                            break;
                        default:
                            Redirect = "home?error=noimagealbum2";
                            break;
                    }

                    Response.Redirect(Redirect);
                }

                if (MyImage.Title != null && !string.IsNullOrEmpty(MyImage.Title) && !string.IsNullOrWhiteSpace(MyImage.Title))
                {
                    InputTitleTwo.Text = MyImage.Title;
                }

                if (myUser.AccountType == "admin")
                {
                    btn_AdminDelete.Visible = true;
                    DropDownDiv.Visible = true;
                }

                if (MyImage.CreatedById.ToString() != myUser.Id.ToString())
                {

                    new LogEntry(DebugLog) { text = string.Format("{0} attempted to edit an image which {1} did not own.", myUser.FullName, myUser.SubjectiveGenderPronoun) };
                    if (myUser.AccountType == "admin")
                    {

                        new LogEntry(DebugLog) { text = string.Format("{0} was allowed to edit the image anyway because {1} is an admin.", myUser.FullName, myUser.SubjectiveGenderPronoun) };
                    }
                    else
                    {
                        Response.Redirect("photos?error=0");
                    }
                }
                    ImagePreview.ImageUrl = MyImage.ImageSrc;

                    NewAlbumsDropDown.Items.Clear();

                    foreach (Album tempAlbum in Album.GetAllAlbums())
                    {
                        NewAlbumsDropDown.Items.Add(new ListItem() { Value = Convert.ToString(tempAlbum.Id), Text = tempAlbum.Name });
                    }

                    new LogEntry(DebugLog) { text = "First album guid = " + MyImage.AlbumIds().First().ToString() };

                    NewAlbumsDropDown.SelectedValue = MyImage.AlbumIds().First().ToString();
                

            }
            else
            {
                Response.Redirect("home");
            }

            
        }

        protected void SelectAlbum_Changed(object sender, EventArgs e)
        {
            //Response.Redirect("users?userId=" + NewAlbumsDropDown.SelectedValue);
        }

        protected void BtnDeleteImage_Click(object sender, EventArgs e)
        {
            try
            {
                new LogEntry(DebugLog) { text = "Attempting to delete uploaded photo id = " + Request.QueryString["imageid"] };

                using (SqlConnection conn = Parsnip.GetOpenDbConnection())
                {
                    SqlCommand DeleteUploads = new SqlCommand("DELETE iap FROM t_ImageAlbumPairs iap FULL OUTER JOIN t_Images ON imageid = t_Images.id  WHERE t_Images.id = @imageid", conn);
                    DeleteUploads.Parameters.Add(new SqlParameter("imageid", Request.QueryString["imageid"]));
                    int recordsAffected = DeleteUploads.ExecuteNonQuery();

                    new LogEntry(DebugLog) { text = string.Format("{0} record(s) were affected", recordsAffected) };
                }
            }
            catch (Exception err)
            {

                new LogEntry(DebugLog) { text = "There was an exception whilst DELETING the photo: " + err };
            }
            new LogEntry(DebugLog) { text = "Successfully deleted photo with id = " + Request.QueryString["imageid"] };
            Response.Redirect(Request.QueryString["redirect"]);
        }

        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            new LogEntry(DebugLog) { text = "Save button clicked. Saving changes..." };
        }
    }
}