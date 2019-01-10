using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using System.Data.SqlClient;
using System.Diagnostics;
using LogApi;

namespace TheParsnipWeb
{
    public partial class create_user : System.Web.UI.Page
    {
        User myUser;
        User mySelectedUser;
        List<User> users;
        string formType = "Insert";
        

        protected void Page_Load(object sender, EventArgs e)
        {
            btnAction.Text = formType;

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
            Debug.WriteLine("----------User selection was changed...");
            
            Debug.WriteLine("----------User selection was changed - Creating new user with id = " + selectUser.SelectedValue);
            mySelectedUser = new User(new Guid(selectUser.SelectedValue));
            if (selectUser.SelectedValue.ToString() != Guid.Empty.ToString())
            {
                mySelectedUser.Select();
            }
                

            if (mySelectedUser.ExistsOnDb())
                btnAction.Text = "Update";
            else
                btnAction.Text = "Insert";

            //Response.Redirect("users?userId=" + selectUser.SelectedValue);
            Debug.WriteLine(string.Format("{0} selected {1} which has a value of {2}", myUser.fullName, selectUser.SelectedItem, selectUser.SelectedValue));

            UserForm.dataSubject = mySelectedUser;
        }

        protected void btnAction_Click(object sender, EventArgs e)
        {
            string rememberSelectedValue = selectUser.SelectedValue;
            Debug.WriteLine("Insert / Update button was clicked. dataSubject.id = " + UserForm.dataSubject.id);
            
            UserForm.UpdateFormAccount();
            if (UserForm.dataSubject.Validate())
            {
                if (UserForm.dataSubject.Update())
                {
                    new LogEntry(UserForm.dataSubject.id) { text = String.Format("{0} created / edited an account for {1} via the UserForm", UserForm.dataSubject.fullName, UserForm.dataSubject.fullName) };
                }
                else
                    new LogEntry(UserForm.dataSubject.id) { text = String.Format("{0} tried to create / edit an account for {1} via the UserForm, but there was an error whilst updating the database", UserForm.dataSubject.fullName, UserForm.dataSubject.fullName) };

            }
            else
            {
                Debug.WriteLine("User failed to validate!");
                new LogEntry(UserForm.dataSubject.id) { text = String.Format("{0} attempted to create / edit an account for {1} via the UserForm, but the user failed fo validate!", UserForm.dataSubject.fullName, UserForm.dataSubject.fullName) };
            }

            selectUser.SelectedValue = rememberSelectedValue;
            mySelectedUser = new User(new Guid(rememberSelectedValue));
            if(mySelectedUser.id.ToString() != Guid.Empty.ToString())
            {
                mySelectedUser.Select();
            }
        }

    }
}