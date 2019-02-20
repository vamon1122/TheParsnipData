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
        protected void Page_Load(object sender, EventArgs e)
        {
            //We secure the page using the UacApi. 
            //This ensures that the user is logged in etc
            //You only need to change where it says '_NEW TEMPLATE'.
            //Change this to match your page name without the '.aspx' extension.

            if (Request.QueryString["imageid"] == null)
                myUser = Uac.SecurePage("edit_image", this, Data.DeviceType);
            else
                myUser = Uac.SecurePage("edit_image?imageid=" + Request.QueryString["imageid"], this, Data.DeviceType);

            myUser = Uac.SecurePage("edit_image", this, Data.DeviceType);

            if (Request.QueryString["imageid"] != null)
            {
                MediaApi.Image temp = new MediaApi.Image(new Guid(Request.QueryString["imageid"]));
                temp.Select();

                if (myUser.AccountType == "admin")
                {
                    btn_AdminDelete.Visible = true;
                }

                if (temp.CreatedById.ToString() != myUser.Id.ToString())
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

                if (Request.QueryString["save"] != null)
                {
                    if (Request.QueryString["title"] != null)
                    {
                        temp.Title = Request.QueryString["title"];   
                    }
                    temp.Update();

                    if (Request.QueryString["redirect"] != null)
                    {
                        Response.Redirect(Request.QueryString["redirect"]);
                    }
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
    }
}