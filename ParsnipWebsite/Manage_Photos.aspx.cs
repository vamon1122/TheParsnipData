using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ParsnipData.UacApi;
using ParsnipData.Logs;
using System.Data.SqlClient;
using ParsnipData;
using ParsnipData.Media;
using System.Diagnostics;
using ParsnipWebsite.Custom_Controls.Media_Api;

namespace ParsnipWebsite
{
    public partial class Manage_Photos : System.Web.UI.Page
    {
        User myUser;
        Guid selectedUserId;
        Log DebugLog = new Log("Debug");



        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("manage_photos", this, Data.DeviceType, "admin");
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            UpdateUserList();

            if (Request.QueryString["userId"] != null && Request.QueryString["userId"].ToString() != "")
            {
                new LogEntry(DebugLog) { text = "Manage_Photos userId = " + Request.QueryString["userId"].ToString() };
                selectedUserId = new Guid(Request.QueryString["userId"].ToString());
                
                SelectUser.SelectedValue = selectedUserId.ToString();

                Debug.WriteLine("---------- posted back with id = " + selectedUserId);

                List<ParsnipData.Media.Image> MyPhotos = ParsnipData.Media.Image.GetImagesByUser(selectedUserId).Where(user => user.AlbumId != Guid.Empty).ToList();
                //new LogEntry(Debug) { text = "Got all photos. There were {0} photo(s) = " + AllPhotos.Count() };
                foreach (ParsnipData.Media.Image temp in MyPhotos)
                {
                    var MyImageControl = (ImageControl)LoadControl("~/Custom_Controls/Media_Api/ImageControl.ascx");
                    MyImageControl.MyImage = temp;
                    DisplayPhotosDiv.Controls.Add(MyImageControl);

                    //new LogEntry(Debug) { text = "Added new image to the page. Url = " + temp.PhotoSrc };
                }

            }
            else
            {
                Debug.WriteLine("---------- not a postback");

                if (Request.QueryString["userId"] == null)
                    Response.Redirect("manage_photos?userId=" + Guid.Empty.ToString());

            }


        }

        protected void BtnDeleteUploads_Click(object sender, EventArgs e)
        {
            selectedUserId = new Guid(Request.QueryString["userId"].ToString());
            try
            {
                new LogEntry(DebugLog) { text = "Attempting to delete uploaded photos createdbyid = " + selectedUserId };

                using (SqlConnection conn = Parsnip.GetOpenDbConnection())
                {
                    SqlCommand DeleteUploads = new SqlCommand("DELETE iap FROM t_ImageAlbumPairs iap FULL OUTER JOIN t_Images ON imageid = t_Images.id  WHERE t_Images.createdbyid = @createdbyid", conn);
                    DeleteUploads.Parameters.Add(new SqlParameter("createdbyid", selectedUserId));
                    int recordsAffected = DeleteUploads.ExecuteNonQuery();

                    new LogEntry(DebugLog) { text = string.Format("{0} record(s) were affected", recordsAffected) };
                }
            }
            catch (Exception err)
            {

                new LogEntry(DebugLog) { text = "There was an exception whilst DELETING the photo: " + err };
            }
            new LogEntry(DebugLog) { text = "Successfully deleted photos uploaded photos createdbyid = " + selectedUserId };
        }

        void UpdateUserList()
        {
            var tempUsers = new List<User>();
            tempUsers.AddRange(ParsnipData.UacApi.User.GetAllUsers());

            ListItem[] ListItems = new ListItem[tempUsers.Count];

            int i = 0;
            foreach (User temp in tempUsers)
            {
                ListItems[i] = new ListItem(String.Format("{0} ({1})", temp.FullName, temp.Username), temp.Id.ToString());
                i++;
            }
            SelectUser.Items.Clear();
            SelectUser.Items.AddRange(ListItems);

            SelectUser.SelectedValue = selectedUserId.ToString();
        }

        protected void SelectUser_Changed(object sender, EventArgs e)
        {
            Response.Redirect("manage_photos?userId=" + SelectUser.SelectedValue);
        }
    }
}