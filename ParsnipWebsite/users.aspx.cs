﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using System.Data.SqlClient;
using System.Diagnostics;
using LogApi;

namespace ParsnipWebsite
{
    public partial class create_user : System.Web.UI.Page
    {
        User myUser;
        List<User> users;
        string formType;
        static string rememberSelectedValue;


        protected void Page_Load(object sender, EventArgs e)
        {
            

            myUser = Uac.SecurePage("users", this, Data.deviceType, "admin");

            if (Request.QueryString["userId"] != null)
            {
                rememberSelectedValue = Request.QueryString["userId"];
                if(rememberSelectedValue == Guid.Empty.ToString())
                    formType = "Insert";
                else
                    formType = "Update";

            }
            else
            {
                Response.Redirect("users?userId=" + Guid.Empty.ToString());
            }

            btnAction.Text = formType;

            if (formType == "Insert")
                btnDelete.Visible = false;
            else
                btnDelete.Visible = true;
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("----------Page load complete!");

            UserForm.UpdateDateCreated();
            UpdateUserList();
            if (rememberSelectedValue != Guid.Empty.ToString())
                selectUser.SelectedValue = rememberSelectedValue;

            User myUser = new User(new Guid(rememberSelectedValue));
            myUser.Select();

            UserForm.DataSubject = myUser;

            System.Diagnostics.Debug.WriteLine("Page_LoadComplete complete!");
        }

        void UpdateUserList()
        {
            users = new List<User>();
            users.Add(new UacApi.User(Guid.Empty) { Forename = "New", Surname = "User", Username = "Create a new user" });
            users.AddRange(UacApi.User.GetAllUsers());

            ListItem[] ListItems = new ListItem[users.Count];

            int i = 0;
            foreach (User temp in users)
            {
                ListItems[i] = new ListItem(String.Format("{0} ({1})", temp.FullName, temp.Username), temp.Id.ToString());
                i++;
            }
            selectUser.Items.Clear();
            selectUser.Items.AddRange(ListItems);

            selectUser.SelectedValue = rememberSelectedValue;
        }

        

        protected void SelectUser_Changed(object sender, EventArgs e)
        {
            Response.Redirect("users?userId=" + selectUser.SelectedValue);
        }

        protected void btnAction_Click(object sender, EventArgs e)
        {
            string rememberSelectedValue = selectUser.SelectedValue;
            Debug.WriteLine("BEN!!!1 " + UserForm.DataSubject.Id.ToString());
            string temp = string.Format("{0} button was clicked. Selected user id = {1}", btnAction.Text, rememberSelectedValue);
            //Debug.WriteLine(temp);
            new LogEntry(myUser.Id) { text = temp };
            
            UserForm.UpdateDataSubject();

            string actionPast = UserForm.DataSubject.ExistsOnDb() ? "edited" : "created";
            string actionPresent = UserForm.DataSubject.ExistsOnDb() ? "edit" : "create";

            if (UserForm.DataSubject.Validate())
            {
                

                if (UserForm.DataSubject.Update())
                {
                    new LogEntry(UserForm.DataSubject.Id) { text = String.Format("{0} {1} an account for {2} via the UserForm", myUser.FullName, actionPast, UserForm.DataSubject.FullName) };
                    Success.Attributes.CssStyle.Add("display", "block");
                    SuccessText.Text = string.Format("<strong>Success</strong> {0} was updated on the database successfully!", UserForm.DataSubject.FullName);
                }

                else
                {
                    new LogEntry(UserForm.DataSubject.Id) { text = String.Format("{0} tried to {1} an account for {2} via the UserForm, but there was an error whilst updating the database", myUser.FullName, actionPresent, UserForm.DataSubject.FullName) };
                    Error.Attributes.CssStyle.Add("display", "block");
                    ErrorText.Text = string.Format("<strong>Database Error</strong> There was an error whilst updating {0} on the database", UserForm.DataSubject.FullName);
                }
                    

            }
            else
            {
                Debug.WriteLine("User failed to validate!");
                new LogEntry(UserForm.DataSubject.Id) { text = String.Format("{0} attempted to {1} an account for {2} via the UserForm, but {3} was not validated successfully.", 
                    myUser.FullName, actionPresent, UserForm.DataSubject.FullName, UserForm.DataSubject.SubjectiveGenderPronoun) };
                Error.Attributes.CssStyle.Add("display", "block");

                string ValidationInfo = string.Format("<strong>Validation Error</strong> {0} could not be updated because {1} failed to validate: ", UserForm.DataSubject.FullName, UserForm.DataSubject.SubjectiveGenderPronoun);
                foreach (string error in UserForm.DataSubject.ValidationErrors)
                {
                    ValidationInfo += error + ", ";
                }
                ValidationInfo = ValidationInfo.Substring(0, ValidationInfo.Length - 2);
                ValidationInfo += ".";

                ErrorText.Text = ValidationInfo;
            }

            //Debug.WriteLine("BEN!!!2 " + UserForm.DataSubject.Id.ToString());
            UpdateUserList();

            if(new User(UserForm.DataSubject.Id).ExistsOnDb())
            {
                selectUser.SelectedValue = UserForm.DataSubject.Id.ToString();
            }
            else
            {
                selectUser.SelectedValue = Guid.Empty.ToString();
            }


        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string temp = string.Format("Delete button was clicked. Selected user id = {0}", selectUser.SelectedValue);
            //Debug.WriteLine(temp);
            new LogEntry(myUser.Id) { text = temp };

            string actionPast = UserForm.DataSubject.ExistsOnDb() ? "edited" : "created";
            string actionPresent = UserForm.DataSubject.ExistsOnDb() ? "edit" : "create";

            if (UserForm.DataSubject.ExistsOnDb())
                UserForm.DataSubject.Delete();

            Response.Redirect("users?userId=" + Guid.Empty.ToString());
        }
    }
}