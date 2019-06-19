using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using ParsnipApi;
using UacApi;

namespace ParsnipWebsite
{
    public partial class Share_Stats : System.Web.UI.Page
    {
        User myUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("users", this, Data.DeviceType, "admin");
            
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = Parsnip.GetOpenDbConnection())
                {
                    var getStats = new SqlCommand(
                        "SELECT t_Images.title," +
                        "uploaded_by.forename," +
                        "shared_by.forename," +
                        "access_token.times_used," +
                        "access_token.access_token_id " +
    
                        "FROM access_token " +
                        "LEFT JOIN t_Images ON access_token.media_id = t_Images.Id " +
                        "LEFT JOIN t_Users AS uploaded_by ON t_Images.createdbyid = uploaded_by.Id " +
                        "LEFT JOIN t_Users AS shared_by ON access_token.user_id = shared_by.Id " +

                        "ORDER BY times_used DESC", conn);

                    using(SqlDataReader reader = getStats.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TableRow stats = new TableRow();
                            stats.Cells.Add(new TableCell() { Text = reader[0].ToString() });
                            stats.Cells.Add(new TableCell() { Text = reader[1].ToString() });
                            stats.Cells.Add(new TableCell() { Text = reader[2].ToString() });
                            stats.Cells.Add(new TableCell() { Text = reader[3].ToString() });
                            stats.Cells.Add(new TableCell() { Text = string.Format("{0}/view_image?access_token={1}", Request.Url.GetLeftPart(UriPartial.Authority), reader[4]) });
                            Table_Stats.Rows.Add(stats);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            

            
        }
    }
}