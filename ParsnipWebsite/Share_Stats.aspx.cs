using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using ParsnipApi;
using UacApi;
using System.Data;

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
            var allStats = new DataTable();
            var allStatsVideo = new DataTable();
            var allStatsImage = new DataTable();
            try
            {
                using (SqlConnection conn = Parsnip.GetOpenDbConnection())
                {
                    var getImageStats = new SqlCommand(
                        "SELECT t_Images.title," +
                        "uploaded_by.forename," +
                        "shared_by.forename," +
                        "access_token.times_used," +
                        "access_token.access_token_id " +
    
                        "FROM access_token " +
                        "INNER JOIN t_Images ON access_token.media_id = t_Images.Id " +
                        "INNER JOIN t_Users AS uploaded_by ON t_Images.createdbyid = uploaded_by.Id " +
                        "INNER JOIN t_Users AS shared_by ON access_token.user_id = shared_by.Id " +

                        "ORDER BY times_used DESC", conn);

                    var getVideoStats = new SqlCommand(
                        "SELECT video.title," +
                        "uploaded_by.forename," +
                        "shared_by.forename," +
                        "access_token.times_used AS times_used, " +
                        "access_token.access_token_id " +

                        "FROM access_token " +
                        "INNER JOIN video ON access_token.media_id = video.video_id " +
                        "INNER JOIN t_Users AS uploaded_by ON video.created_by_id = uploaded_by.Id " +
                        "INNER JOIN t_Users AS shared_by ON access_token.user_id = shared_by.Id " +

                        "ORDER BY times_used DESC", conn);

                    using (var imageStats = getImageStats.ExecuteReader())
                    {
                        allStats.Load(imageStats);
                        allStatsImage.Load(imageStats);
                    }

                    using(var videoStats = getVideoStats.ExecuteReader())
                    {
                        allStats.Load(videoStats);
                        allStatsVideo.Load(videoStats);
                    }
                    
                    allStats.DefaultView.Sort = "times_used DESC";
                    allStats = allStats.DefaultView.ToTable();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
            

            foreach(DataRow row in allStats.Rows)
            {
                var myRow = new TableRow();
                myRow.Cells.Add(new TableCell() { Text = row[0].ToString() });
                //myRow.Cells.Add(new TableCell() { Text = row[1].ToString() });
                myRow.Cells.Add(new TableCell() { Text = row[2].ToString() });
                myRow.Cells.Add(new TableCell() { Text = row[3].ToString() });
                //myRow.Cells.Add(new TableCell() { Text = row[4].ToString() });

                Table_Stats.Rows.Add(myRow);
            }
            

        }
    }
}