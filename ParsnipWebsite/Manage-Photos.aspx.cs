﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using LogApi;
using System.Data.SqlClient;
using ParsnipApi;
using MediaApi;
using System.Diagnostics;

namespace ParsnipWebsite
{
    public partial class Manage_Photos : System.Web.UI.Page
    {
        User myUser;
        Guid selectedUserId;
        Log DebugLog = new Log("Debug");



        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("manage-photos", this, Data.DeviceType, "admin");
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            UpdateUserList();

            if (Request.QueryString["userId"] != null && Request.QueryString["userId"].ToString() != "")
            {
                new LogEntry(DebugLog) { text = "Manage-Photos userId = " + Request.QueryString["userId"].ToString() };
                selectedUserId = new Guid(Request.QueryString["userId"].ToString());
                
                SelectUser.SelectedValue = selectedUserId.ToString();

                Debug.WriteLine("---------- posted back with id = " + selectedUserId);

                List<Photo> MyPhotos = Photo.GetPhotosByUser(selectedUserId);
                //new LogEntry(Debug) { text = "Got all photos. There were {0} photo(s) = " + AllPhotos.Count() };
                foreach (Photo temp in MyPhotos)
                {
                    Image tempControl = new Image();

                    
                    tempControl.ImageUrl = "resources/media/images/webMedia/pix-vertical-placeholder.jpg";
                    tempControl.Attributes.Add("data-src", temp.PhotoSrc);
                    tempControl.Attributes.Add("data-srcset", temp.PhotoSrc);
                    
                    

                    tempControl.CssClass = "mobile-image lazy";
                    DisplayPhotosDiv.Controls.Add(tempControl);
                    this.Page.Form.FindControl("DisplayPhotosDiv").Controls.Add(new LiteralControl("<br />"));
                    this.Page.Form.FindControl("DisplayPhotosDiv").Controls.Add(new LiteralControl("<br />"));

                    //new LogEntry(Debug) { text = "Added new image to the page. Url = " + temp.PhotoSrc };
                }

            }
            else
            {
                Debug.WriteLine("---------- not a postback");

                if (Request.QueryString["userId"] == null)
                    Response.Redirect("manage-photos?userId=" + Guid.Empty.ToString());

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
                    SqlCommand DeleteUploads = new SqlCommand("DELETE FROM t_Photos WHERE createdbyid = @createdbyid", conn);
                    DeleteUploads.Parameters.Add(new SqlParameter("createdbyid", selectedUserId));
                    DeleteUploads.ExecuteNonQuery();
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
            tempUsers.AddRange(UacApi.User.GetAllUsers());

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
            Response.Redirect("manage-photos?userId=" + SelectUser.SelectedValue);
        }
    }
}