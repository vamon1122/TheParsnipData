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
            selectUser.Items.Clear();
            selectUser.Items.AddRange(ListItems);

            if(UserForm.myUser != null)
            {
                selectUser.SelectedValue = CookieApi.Cookie.Read("formSelectedUser"); //UserForm.myUser.id.ToString();
            }

            UserForm.UpdateForm();
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
                    users.Add(new User(Guid.Empty));
                    while (reader.Read())
                    {
                        users.Add(new UacApi.User(reader));
                    }
                }
            }
        }

        protected void SelectUser_Changed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SelectUser_Changed");
               /*System.Diagnostics.Debug.WriteLine("ddlselect_Changed GetUsers() START");
            GetUsers();
            System.Diagnostics.Debug.WriteLine("ddlselect_Changed GetUsers() END");*/
               System.Diagnostics.Debug.WriteLine("----------Listing user id's...");
            foreach (User temp in users)
            {
                System.Diagnostics.Debug.WriteLine("----------temp.id = " + temp.id);
            }
            System.Diagnostics.Debug.WriteLine("----------selectUser was changed. SelectedValue = " + selectUser.SelectedValue);
            /*if (selectUser.SelectedValue == Guid.Empty.ToString())
            {
                System.Diagnostics.Debug.WriteLine(string.Format("----------selectUser's value = Guid.Empty ({0} = {1})", selectUser.SelectedValue, Guid.Empty));
                UserForm.myUser = new User("selectUser_Changed");
            }
            else
            {*/
                System.Diagnostics.Debug.WriteLine(string.Format("----------selectUser's value = {0}. Finding this user in users", selectUser.SelectedValue));
                UserForm.myUser = users.Single(x => x.id.ToString() == selectUser.SelectedValue.ToString());
                System.Diagnostics.Debug.WriteLine(string.Format("----------Found a user! Fullname: {0} Id: {1}", UserForm.myUser.fullName, UserForm.myUser.id));
                CookieApi.Cookie.WriteSession("formSelectedUser", UserForm.myUser.id.ToString());
            //}
                

            UserForm.UpdateForm();
        }

    }
}