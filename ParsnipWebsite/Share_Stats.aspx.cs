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
using ParsnipData.Media;

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
            DataTable allStats = AccessToken.GetStats();

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