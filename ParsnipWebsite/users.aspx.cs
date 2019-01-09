using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using System.Data.SqlClient;
using System.Diagnostics;

namespace TheParsnipWeb
{
    public partial class create_user : System.Web.UI.Page
    {
        User myUser;
        User mySelectedUser;
        List<User> users;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("users", this, Data.deviceType, "admin");

            if (mySelectedUser == null)
            {


                if (Request.QueryString["userId"] != null)
                {
                    mySelectedUser = new User(Request.QueryString["userId"]);
                    mySelectedUser.Select();
                }
                else
                {
                    mySelectedUser = new User(Guid.Empty);
                }
                Debug.WriteLine("mySelectedUser ALREADY EXISTED!!!");
            }
            users = new List<User>();
            users.Add(new UacApi.User(Guid.Empty) { forename = "New", surname = "User", username = "Create a new user" });
            users.AddRange(UacApi.User.GetAllUsers());
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("----------Page load complete!");

            
            ListItem[] ListItems = new ListItem[users.Count];
            
            int i = 0;
            foreach (User temp in users)
            {
                ListItems[i] = new ListItem(String.Format("{0} ({1})", temp.fullName, temp.username), temp.id.ToString());
                i++;
            }
            selectUser.Items.Clear();
            selectUser.Items.AddRange(ListItems);

            selectUser.SelectedValue = mySelectedUser.id.ToString();
            System.Diagnostics.Debug.WriteLine("Page_LoadComplete complete!");
        }

        

        protected void SelectUser_Changed(object sender, EventArgs e)
        {
            //Response.Redirect("users?userId=" + selectUser.SelectedValue);
            Debug.WriteLine(string.Format("{0} selected {1} which has a value of {2}", myUser.fullName, selectUser.SelectedItem, selectUser.SelectedValue));
            mySelectedUser = new User(new Guid(selectUser.SelectedValue));
            string rememberSelectedValue = selectUser.SelectedValue;
            if (mySelectedUser.Select())
            {
                Debug.WriteLine(string.Format("----------{0} selected a different user! Replacing the UserForm with a new UserForm with the user {1} (Id = {2})", myUser.fullName, mySelectedUser.fullName, mySelectedUser.id));
                UserForm.dataSubject = mySelectedUser;
                selectUser.SelectedValue = rememberSelectedValue;
            }
            else
                Debug.WriteLine("----------SELECT FAILED");
            
            
        }

    }
}