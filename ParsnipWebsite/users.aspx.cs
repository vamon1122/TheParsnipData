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
        List<User> users = new List<User>();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("users", this, Data.deviceType, "admin");
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("----------Page load complete! Updating form...");
            UserForm.UpdateForm();
            System.Diagnostics.Debug.WriteLine("----------Update form complete!");



            GetUsers();

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
            User tempUser = users.SingleOrDefault(x => x.id.ToString() == UserForm.myUser.id.ToString());
            if (tempUser != null)
                ddlselect.SelectedValue = UserForm.myUser.id.ToString();
            else
                System.Diagnostics.Debug.WriteLine("tempUser = null! This is a new user");
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
            GetUsers();
            System.Diagnostics.Debug.WriteLine("----------Listing user id's...");
            foreach (User temp in users)
            {
                System.Diagnostics.Debug.WriteLine("----------temp.id = " + temp.id);
            }
            System.Diagnostics.Debug.WriteLine("----------ddlSelect was changed. SelectedValue = " + ddlselect.SelectedValue);
            if (ddlselect.SelectedValue == Guid.Empty.ToString())
                UserForm.myUser = new User();
            else
                UserForm.myUser = users.Single(x => x.id.ToString() == ddlselect.SelectedValue.ToString());

            UserForm.UpdateForm();
        }

    }
}