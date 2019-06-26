using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using ParsnipData;
using ParsnipData.UacApi;
using System.Data;

namespace ParsnipWebsite
{
    public partial class Share_Stats : System.Web.UI.Page
    {
        User myUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("share_stats", this, Data.DeviceType, "admin");
            
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            var allStats = new DataTable();
            
            var allStatsVideo = new DataTable();
            var allStatsImage = new DataTable();
            //try
            //{
                using (SqlConnection conn = Parsnip.GetOpenDbConnection())
                {
                    var getImageStats = new SqlCommand(
                        "SELECT t_Images.id AS media_id, " +
                        "t_Images.title, " +
                        "uploaded_by.forename," +
                        "shared_by.forename," +
                        "access_token.times_used," +
                        "access_token.access_token_id, " +
                        "t_ImageAlbumPairs.albumid, " +
                        "shared_by.id " +

                        "FROM access_token " +
                        "INNER JOIN t_Images ON access_token.media_id = t_Images.Id " +
                        "INNER JOIN t_Users AS uploaded_by ON t_Images.createdbyid = uploaded_by.Id " +
                        "INNER JOIN t_Users AS shared_by ON access_token.user_id = shared_by.Id " +
                        "INNER JOIN t_ImageAlbumPairs ON t_Images.id = t_ImageAlbumPairs.imageid " +

                        "ORDER BY times_used DESC", conn);

                    var getVideoStats = new SqlCommand(
                        "SELECT video.video_id AS media_id, " +
                        "video.title, "+
                        "uploaded_by.forename, "+
                        "shared_by.forename, "+
                        "access_token.times_used , "+
                        "access_token.access_token_id, "+
                        "t_ImageAlbumPairs.albumid, "+
                        "shared_by.id " +



                        "FROM access_token " +
                        "INNER JOIN video ON access_token.media_id = video.video_id "+
                        "INNER JOIN t_Users AS uploaded_by ON video.created_by_id = uploaded_by.Id "+
                        "INNER JOIN t_Users AS shared_by ON access_token.user_id = shared_by.Id "+
                        "INNER JOIN t_ImageAlbumPairs ON video.video_id = t_ImageAlbumPairs.imageid "+

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
            /*}
            catch(Exception ex)
            {
                throw ex;
            }*/
            
            

            foreach(DataRow row in allStats.Rows)
            {
                string mediaRedirect;
                string shareRedirect;
                switch (row[6].ToString().ToUpper())
                {
                    case "73C436A1-893B-4418-8800-821823C18DFE":
                        mediaRedirect = "video_player?videoid=";
                        shareRedirect = "video_player?access_token=";

                        break;
                    default:
                        mediaRedirect = "view_image?imageid=";
                        shareRedirect = "view_image?access_token=";
                        break;
                }
                var myRow = new TableRow();

                var titleCell = new TableCell();
                titleCell.Controls.Add(new LiteralControl(string.Format("<a href={0}>{1}</a>", mediaRedirect + row[0].ToString(), row[1].ToString())));
                myRow.Cells.Add(titleCell);

                var userCell = new TableCell();
                userCell.Controls.Add(new LiteralControl(string.Format("<a href={0}>{1}</a>", "users?userid=" + row[7].ToString(), row[3].ToString())));
                myRow.Cells.Add(userCell);

                var timesUsedCell = new TableCell();
                timesUsedCell.Controls.Add(new LiteralControl(string.Format("<a href={0}>{1}</a>", shareRedirect + row[5].ToString(), row[4].ToString())));
                myRow.Cells.Add(timesUsedCell);


                Table_Stats.Rows.Add(myRow);
            }
            

        }
    }
}