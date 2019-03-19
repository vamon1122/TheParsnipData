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
        protected async void Page_Load(object sender, EventArgs e)
        {
            //We secure the page using the UacApi. 
            //This ensures that the user is logged in etc
            //You only need to change where it says '_NEW TEMPLATE'.
            //Change this to match your page name without the '.aspx' extension.

            myUser = await UacApi.User.LogInFromCookies();
            Uac.SecurePage("users", this, Data.DeviceType, "member", myUser);
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            //REQUIRED TO VIEW POSTBACK
            form1.Action = Request.RawUrl;

            if (Request.QueryString["imageid"] == null)
                Uac.SecurePage("edit_image", this, Data.DeviceType, "user", myUser);
            else
                Uac.SecurePage("edit_image?imageid=" + Request.QueryString["imageid"], this, Data.DeviceType, "user", myUser);

            //myUser = Uac.SecurePage("edit_image", this, Data.DeviceType);

            if (Request.QueryString["imageid"] != null)
            {
                MyImage = new MediaApi.Image(new Guid(Request.QueryString["imageid"]));
                MyImage.Select();

                NewAlbumsDropDown.Items.Clear();

                foreach (Album tempAlbum in Album.GetAllAlbums())
                {
                    NewAlbumsDropDown.Items.Add(new ListItem() { Value = Convert.ToString(tempAlbum.Id), Text = tempAlbum.Name });
                }

                var AlbumIds = MyImage.AlbumIds();
                int NumberOfAlbums = AlbumIds.Count();

                if (NumberOfAlbums > 0)
                {
                    new LogEntry(DebugLog) { text = "First album guid = " + AlbumIds.First().ToString() };
                    NewAlbumsDropDown.SelectedValue = AlbumIds.First().ToString();
                }
                else
                {
                    NewAlbumsDropDown.Items.Insert(0, new ListItem("(Deleted)", Guid.Empty.ToString()));
                }

                if (Request.QueryString["delete"] != null)
                {
                    new LogEntry(DebugLog) { text = "Delete image clicked" };
                    MyImage.Delete();

                    string Redirect;

                    switch (NewAlbumsDropDown.SelectedValue.ToString().ToUpper())
                    {
                        case "4B4E450A-2311-4400-AB66-9F7546F44F4E":
                            Redirect = "photos";
                            break;
                        case "5F15861A-689C-482A-8E31-2F13429C36E5":
                            Redirect = "memes";
                            break;
                        case "00000000-0000-0000-0000-000000000000":
                            Redirect = "manage_photos";
                            break;
                        default:
                            Redirect = "home?error=noimagealbum2";
                            break;
                    }

                    Response.Redirect(Redirect);
                }

                if (IsPostBack)
                {
                    new LogEntry(DebugLog) { text = "Delete image NOT clicked" };
                    /*
                    new LogEntry(DebugLog) { text = "Posted back title3 = " + Request["InputTitleTwo"].ToString() };
                    new LogEntry(DebugLog) { text = "Posted back albumid3 = " + Request["NewAlbumsDropDown"].ToString() };
                    */

                    MyImage.Title = Request["InputTitleTwo"].ToString();

                    //This breaks on some older browsers. Seems android specific?
                    var tempAlbumId = Request["NewAlbumsDropDown"].ToString();

                    if (tempAlbumId != Guid.Empty.ToString())
                    {
                        MyImage.AlbumId = new Guid(tempAlbumId);
                    }

                    MyImage.Update();

                    string Redirect;

                    switch (Request["NewAlbumsDropDown"].ToString().ToUpper())
                    {
                        case "4B4E450A-2311-4400-AB66-9F7546F44F4E":
                            Redirect = "photos?imageid=" + MyImage.Id.ToString();
                            break;
                        case "5F15861A-689C-482A-8E31-2F13429C36E5":
                            Redirect = "memes?imageid=" + MyImage.Id.ToString();
                            break;
                        case "00000000-0000-0000-0000-000000000000":
                            Redirect = "manage_photos?imageid=" + MyImage.Id.ToString();
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
                    if (AlbumIds.Count() > 0)
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




            }
            else
            {
                Response.Redirect("home");
            }
        }

        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            new LogEntry(DebugLog) { text = "Save button clicked. Saving changes..." };
        }
    }
}