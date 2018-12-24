using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using LogApi;

namespace TheParsnipWeb
{
    public partial class logs : System.Web.UI.Page
    {
        Account MyAccount;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyAccount = new Account();
            if (!MyAccount.LogIn())
            {
                Response.Redirect("login.aspx?url=logs.aspx");
            }
            else
            {
                new LogEntry() { text = String.Format("{0} accessed the logs page", MyAccount.fullName), userId = MyAccount.id }.Insert();
            }

            if (MyAccount.AccountType != "admin")
            {
                new LogEntry() { text = String.Format("{0} attempted (and failed) to access the logs page", MyAccount.fullName), userId = MyAccount.id }.Insert();
                Response.Redirect("access-denied.aspx?url=logs.aspx");
            }

            LogApi.Data.LoadLogEntries();

            List<LogEntry> LogEntries = LogApi.Data.LogEntries.OrderByDescending(x => x.date ).ToList();

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
    }
}