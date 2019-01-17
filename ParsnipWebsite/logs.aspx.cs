using LogApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using UacApi;

namespace ParsnipWebsite
{
    public partial class logs : System.Web.UI.Page
    {
        User myUser;
        Guid selectedLogId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["logId"] != null)
            {
                selectedLogId = new Guid(Request.QueryString["logId"]);
            }
            else
            {
                Response.Redirect("logs?logId=" + Guid.Empty);
            }

            myUser = Uac.SecurePage("logs", this, Data.deviceType, "admin");

            List<LogEntry> LogEntries;

            if(selectedLogId.ToString() == Guid.Empty.ToString())
                LogEntries = LogApi.Data.GetAllLogEntries().OrderByDescending(x => x.date ).ToList();
            else
            {
                Log temp = new Log(selectedLogId);
                LogEntries = temp.GetLogEntries().OrderByDescending(x => x.date).ToList();
            }
                
            

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

        void Page_LoadComplete(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("----------Page load complete!");
            
            UpdateLogList();
            SelectLog.SelectedValue = selectedLogId.ToString();

            System.Diagnostics.Debug.WriteLine("Page_LoadComplete complete!");
        }

        protected void b_ClearLogs_Click(object sender, EventArgs e)
        {
            LogApi.Data.ClearLogs();
            Response.Redirect("logs");
        }

        protected void SelectLog_Changed(object sender, EventArgs e)
        {
            Response.Redirect("logs?logId=" + SelectLog.SelectedValue);
        }

        void UpdateLogList()
        {
            List<Log> logs;
            logs = new List<Log>();
            logs.AddRange(Log.GetAllLogs());

            ListItem[] ListItems = new ListItem[logs.Count + 1];

            ListItems[0] = new ListItem("all", Guid.Empty.ToString());

            int i = 1;
            foreach (Log temp in logs)
            {
                ListItems[i] = new ListItem(temp.Name, temp.Id.ToString());
                i++;
            }
            SelectLog.Items.Clear();
            SelectLog.Items.AddRange(ListItems);

            SelectLog.SelectedValue = selectedLogId.ToString();
        }
    }
}