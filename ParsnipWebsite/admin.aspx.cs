using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using System.Data.SqlClient;
using LogApi;
using ParsnipApi;

namespace ParsnipWebsite
{
    public partial class Admin : System.Web.UI.Page
    {
        User myAccount;
        Log Debug = new Log("Debug");
        protected void Page_Load(object sender, EventArgs e)
        {
            myAccount = Uac.SecurePage("admin", this, Data.deviceType, "admin");
        }

        protected void OpenLogsButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("logs");
        }

        protected void NewUserButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("create-user");
        }

        protected void BtnDeleteUploads_Click(object sender, EventArgs e)
        {
            try
            {
                new LogEntry(Debug) { text = "Attempting to delete uploaded photos" };

                using (SqlConnection conn = Parsnip.GetOpenDbConnection())
                {
                    SqlCommand DeleteUploads = new SqlCommand("DELETE FROM t_Photos WHERE photosrc LIKE '%uploads%'", conn);
                    DeleteUploads.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {

                new LogEntry(Debug) { text = "There was an exception whilst uploading the photo: " + err };
            }

        }
    }
}