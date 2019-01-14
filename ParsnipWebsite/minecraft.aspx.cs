using System;
using UacApi;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogApi;

namespace TheParsnipWeb
{
    public partial class minecraft : System.Web.UI.Page
    {
        private User myUser;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(Data.deviceType) || string.IsNullOrWhiteSpace(Data.deviceType))
            {
                new LogEntry(Guid.Empty) { text = "Device type was null. Redirecting to getDeviceInfo" };
                Response.Redirect("getDeviceInfo?url=minecraft");
            }
            else
            {
                new LogEntry(Guid.Empty) { text = string.Format("Device type ({0}) was not null. Not redirecting to getDeviceInfo", Data.deviceType )};
            }
            
            myUser = Uac.SecurePage("minecraft", this, Data.deviceType);
        }
    }
}