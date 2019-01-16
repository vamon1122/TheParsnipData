using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using LogApi;

namespace ParsnipWebsite
{
    public partial class logs : System.Web.UI.Page
    {
        User myUser;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = Uac.SecurePage("logs", this, Data.deviceType, "admin");

            if (myUser.AccountType != "admin")
            {
                new LogEntry(myUser.Id) { text = String.Format("{0} attempted (and failed) to access the logs page via {1}", myUser.FullName, Data.deviceType) };
                Response.Redirect("access-denied?url=logs");
            }

            LogApi.Data.LoadLogEntries();

            List<LogEntry> LogEntries = LogApi.Data.logEntries.OrderByDescending(x => x.date ).ToList();

            foreach (LogEntry myEntry in LogEntries)
            {
                TableRow MyRow = new TableRow();
                //MyRow.Cells.Add(new TableCell() { Text = myEntry.userId.ToString() });
                MyRow.Cells.Add(new TableCell() { Text = myEntry.date.ToString() });
                //MyRow.Cells.Add(new TableCell() { Text = myEntry.type });
                MyRow.Cells.Add(new TableCell() { Text = myEntry.text });

                //myEntry.userId, myEntry.date, myEntry.type, myEntry.text

                LogTable.Rows.Add(MyRow);
            }
        }

        protected void b_ClearLogs_Click(object sender, EventArgs e)
        {
            LogApi.Data.ClearLogs();
            Response.Redirect("logs");
        }
    }
}