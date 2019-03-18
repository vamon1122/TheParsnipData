using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using LogApi;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ParsnipWebsite
{
    public partial class Home : System.Web.UI.Page
    {
        private User myUser;
        //static HttpClient client;

        protected async void Page_Load(object sender, EventArgs e)
        {
            //For consuming webservices
            /*
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:59622/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/xml"));
                */

            myUser = await UacApi.User.LogIn("vamon1122", "BBTbbt1704"); 
            Uac.NewSecurePage("home", this, Data.DeviceType, "user", myUser);
            WelcomeLabel.Text = string.Format("Hiya {0}, welcome back to the parsnip website!", myUser.Forename);
            var UacServiceClient = new UacService.UacClient();
        }
    }
}