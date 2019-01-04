using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using System.Data.SqlClient;

namespace TheParsnipWeb
{
    public partial class create_user : System.Web.UI.Page
    {
        User myUser;
        List<User> users;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("users", this, Data.deviceType, "admin");
            users = new List<User>();
            GetUsers();
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("----------Page load complete! Updating form...");
            
            System.Diagnostics.Debug.WriteLine("----------Update form complete!");



            

            //List<ListItem> ListItems = new List<ListItem>();
            ListItem[] ListItems = new ListItem[users.Count + 1];
            
            ListItems[0] = new ListItem(String.Format("New User"), Guid.Empty.ToString());
            int i = 1;
            foreach (User temp in users)
            {
                ListItems[i] = new ListItem(String.Format("{0} ({1})", temp.fullName, temp.username), temp.id.ToString());
                i++;
            }
            ddlselect.Items.Clear();
            ddlselect.Items.AddRange(ListItems);

            if(UserForm.myUser != null)
            {
                ddlselect.SelectedValue = UserForm.myUser.id.ToString();
            }
            /*
            User tempUser = users.SingleOrDefault(x => x.id.ToString() == ddlselect.SelectedValue);
            if (tempUser != null)
                ddlselect.SelectedValue = UserForm.myUser.id.ToString();
            else
                System.Diagnostics.Debug.WriteLine("tempUser = null! This is a new user");
                

            if(ddlselect.SelectedValue.ToString() == Guid.Empty.ToString())
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Not attempting to find user in list as '{0}' appears to be a blank Guid!", ddlselect.SelectedValue.ToString()));
                UserForm.myUser = new User("users - A blank Guid was selected!");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Attempting to find user {0} in users list", ddlselect.SelectedValue.ToString()));
                UserForm.myUser = UserForm.myUser = users.Single(x => x.id.ToString() == ddlselect.SelectedValue.ToString());
            }
            
            UserForm.UpdateForm();
            */
            System.Diagnostics.Debug.WriteLine("Page_LoadComplete complete!");
        }

        private void GetUsers()
        {
            users = new List<User>();
            using (SqlConnection conn = new SqlConnection(ParsnipApi.Data.sqlConnectionString))
            {
                conn.Open();
                SqlCommand GetUsers = new SqlCommand("SELECT * FROM t_Users", conn);
                using (SqlDataReader reader = GetUsers.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UacApi.User(reader));
                    }
                }
            }
        }

        protected void ddlselect_Changed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ddlselect_Changed");
               /*System.Diagnostics.Debug.WriteLine("ddlselect_Changed GetUsers() START");
            GetUsers();
            System.Diagnostics.Debug.WriteLine("ddlselect_Changed GetUsers() END");*/
               System.Diagnostics.Debug.WriteLine("----------Listing user id's...");
            foreach (User temp in users)
            {
                System.Diagnostics.Debug.WriteLine("----------temp.id = " + temp.id);
            }
            System.Diagnostics.Debug.WriteLine("----------ddlSelect was changed. SelectedValue = " + ddlselect.SelectedValue);
            if (ddlselect.SelectedValue == Guid.Empty.ToString())
            {
                System.Diagnostics.Debug.WriteLine(string.Format("----------ddlSelect's value = Guid.Empty ({0} = {1})", ddlselect.SelectedValue, Guid.Empty));
                UserForm.myUser = new User("ddselect_Changed");
            }

            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format("----------ddlSelect's value = {0}. Finding this user in users", ddlselect.SelectedValue));
                UserForm.myUser = users.Single(x => x.id.ToString() == ddlselect.SelectedValue.ToString());
                System.Diagnostics.Debug.WriteLine(string.Format("----------Found a user! Fullname: {0} Id: {1}", UserForm.myUser.fullName, UserForm.myUser.id));
                CookieApi.Cookie.WriteSession("formUser", UserForm.myUser.id.ToString());
            }
                

            UserForm.UpdateForm();
        }

    }
}