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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace ParsnipWebsite
{
    public partial class Users : System.Web.UI.Page
    {
        User myUser;
        static Guid selectedUserId;
        static HttpClient client;

        protected async void Page_Load(object sender, EventArgs e)
        {
            

            

            myUser = await UacApi.User.LogInFromCookies();
            Uac.SecurePage("users", this, Data.DeviceType, "admin", myUser);

            
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            Debug.WriteLine("----------Page load complete!");

            //For consuming webservices
            /*
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:59622/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/xml"));
                */
            //Moved to loadcomplete because it was causing thread abort in asyncronous pageload method
            if (Request.QueryString["userId"] == null)
            {
                //False stops thread abort
                Response.Redirect("users?userId=" + Guid.Empty.ToString(), false);
            }
            else
            {


                //if (!pSecurePage.Response.IsRequestBeingRedirected)

                selectedUserId = new Guid(Request.QueryString["userId"]);

                if (selectedUserId.ToString() == Guid.Empty.ToString())
                {
                    btnAction.Text = "Insert";
                    btnDelete.Visible = false;
                }
                else
                {
                    btnAction.Text = "Update";
                    btnDelete.Visible = true;
                }
            }

            if (Request.QueryString["action"] != null)
            {
                string action = Request.QueryString["action"];
                if (Request.QueryString["success"] != null)
                {
                    string success = Request.QueryString["success"];

                    if (success == "true")
                    {
                        SuccessText.Text = string.Format("<strong>Success</strong> User was successfully {0}d on the database!", action);
                        Success.Attributes.CssStyle.Add("display", "block");
                    }
                }
            }

            UserForm.UpdateDateCreated();
            UpdateUserList();
            if (selectedUserId.ToString() != Guid.Empty.ToString())
                selectUser.SelectedValue = selectedUserId.ToString();

            UserForm.UpdateDataSubject(selectedUserId);

            System.Diagnostics.Debug.WriteLine("Page_LoadComplete complete!");
        }

        static async Task<User> GetUserAsync(string path)
        {
            throw new NotImplementedException();
        }

        protected async void Button1_Click(object sender, EventArgs e)
        {
            //ParsnipApi.Models.User thing = await GetUserAsync("api/users/GetUser?id=1");
            //System.Diagnostics.Debug.WriteLine("Thing = " + thing._forename);
        }

        async void UpdateUserList()
        {
            var tempUsers = new List<User>();
            tempUsers.Add(new UacApi.User(Guid.Empty) { Forename = "New", Surname = "User", Username = "Create a new user" });

            try
            {
                tempUsers.AddRange(await UacApi.User.GetAllUsers());
            }
            catch (Exception e)
            {
                Debug.WriteLine("[Users.aspx] There was an exception when getting all users to update the list: " + e);
            }
            ListItem[] ListItems = new ListItem[tempUsers.Count];

            int i = 0;
            foreach (User temp in tempUsers)
            {
                ListItems[i] = new ListItem(String.Format("{0} ({1})", temp.FullName, temp.Username), temp.Id.ToString());
                i++;
            }
            selectUser.Items.Clear();
            selectUser.Items.AddRange(ListItems);

            selectUser.SelectedValue = selectedUserId.ToString();
        }

        protected void SelectUser_Changed(object sender, EventArgs e)
        {

            Response.Redirect("users?userId=" + selectUser.SelectedValue, false);
        }

        protected void btnAction_Click(object sender, EventArgs e)
        {
            string rememberSelectedValue = selectUser.SelectedValue;
            Debug.WriteLine("BEN!!!1 " + UserForm.DataSubject.Id.ToString());
            string temp = string.Format("{0} button was clicked. Selected user id = {1}", btnAction.Text, rememberSelectedValue);
            //Debug.WriteLine(temp);
            new LogEntry(Log.Default) { text = temp };

            UserForm.UpdateDataSubject();

            string actionPast = UserForm.DataSubject.ExistsOnDb() ? "edited" : "created";
            string actionPresent = UserForm.DataSubject.ExistsOnDb() ? "edit" : "create";

            if (UserForm.DataSubject.Validate())
            {
                if (UserForm.DataSubject.Update())
                {
                    new LogEntry(Log.Default) { text = String.Format("{0} {1} an account for {2} via the UserForm", myUser.FullName, actionPast, UserForm.DataSubject.FullName) };
                    Response.Redirect(string.Format("users?userId={0}&action=update&success=true", UserForm.DataSubject.Id.ToString()));
                }

                else
                {
                    new LogEntry(Log.Default) { text = String.Format("{0} tried to {1} an account for {2} via the UserForm, but there was an error whilst updating the database", myUser.FullName, actionPresent, UserForm.DataSubject.FullName) };
                    ErrorText.Text = string.Format("<strong>Database Error</strong> There was an error whilst updating {0} on the database.", UserForm.DataSubject.FullName);
                }


            }
            else
            {
                Debug.WriteLine("User failed to validate!");
                new LogEntry(Log.Default)
                {
                    text = String.Format("{0} attempted to {1} an account for {2} via the UserForm, but {3} was not validated successfully.",
                    myUser.FullName, actionPresent, UserForm.DataSubject.FullName, UserForm.DataSubject.SubjectiveGenderPronoun)
                };
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


        }

        protected void btnDeleteConfirm_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Delete was confirmed");
            string temp = string.Format("Delete button was clicked. Selected user id = {0}", selectUser.SelectedValue);
            //Debug.WriteLine(temp);
            new LogEntry(Log.Default) { text = temp };



            bool success;
            string feedback;

            if (UserForm.DataSubject.ExistsOnDb())
            {
                success = UserForm.DataSubject.Delete();
            }
            else
                success = false;


            Response.Redirect(string.Format("users?userId={0}&action=delete&success=true", Guid.Empty.ToString()));
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Delete was clicked");
        }
    }
}