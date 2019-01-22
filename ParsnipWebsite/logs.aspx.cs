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
    public partial class Logs : System.Web.UI.Page
    {
        User myUser;
        Guid selectedLogId;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["logId"] == null)
                Response.Redirect("logs?logId=" + Guid.Empty);

            myUser = Uac.SecurePage("logs", this, Data.deviceType, "admin");

            selectedLogId = new Guid(Request.QueryString["logId"]);

            List<LogEntry> LogEntries;

            if (selectedLogId.ToString() == Guid.Empty.ToString())
                LogEntries = LogApi.Data.GetAllLogEntries().OrderByDescending(x => x.date).ToList();
            else
            {
                Log temp = new Log(selectedLogId);
                LogEntries = temp.GetLogEntries().OrderByDescending(x => x.date).ToList();
            }

            foreach (LogEntry myEntry in LogEntries)
            {
                TableRow MyRow = new TableRow();
                MyRow.Cells.Add(new TableCell() { Text = myEntry.date.ToString() });
                MyRow.Cells.Add(new TableCell() { Text = myEntry.text });
                LogTable.Rows.Add(MyRow);
            }

            EntryCount.Text = string.Format("{0} entries found", LogEntries.Count());
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("----------Page load complete!");

            if (Request.QueryString["action"] != null)
            {
                string action = Request.QueryString["action"];
                if (Request.QueryString["success"] != null)
                {
                    string success = Request.QueryString["success"];

                    if (success == "true")
                    {
                        SuccessText.Text = string.Format("<strong>Success</strong> All logs were successfully {0}d on the database!", action);
                        Success.Attributes.CssStyle.Add("display", "block");
                    }

                }

            }

            UpdateLogList();
            SelectLog.SelectedValue = selectedLogId.ToString();

            System.Diagnostics.Debug.WriteLine("Page_LoadComplete complete!");
        }

        protected void SelectLog_Changed(object sender, EventArgs e)
        {
            Response.Redirect("logs?logId=" + SelectLog.SelectedValue);
        }

        protected void btnClearLogsConfirm_Click(object sender, EventArgs e)
        {
            LogApi.Data.ClearLogs();

            new LogEntry(Log.Default) { text = string.Format("Logs were cleared by {0}!", myUser.FullName) };

            Response.Redirect(string.Format("logs?logId={0}&action=delete&success=true", Guid.Empty.ToString()));
        }

        void UpdateLogList()
        {
            var logs = new List<Log>();
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