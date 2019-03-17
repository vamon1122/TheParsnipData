using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using ParsnipApi.Models;

namespace ParsnipWebsite
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        static HttpClient client;

        protected void Page_Load(object sender, EventArgs e)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:59622/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            
        }

        static async Task<Product> GetProductAsync(string path)
        {
            Product product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<Product>();
            }
            else
            {
                
                System.Diagnostics.Debug.WriteLine("There was an error whilst getting the value because " + response.ReasonPhrase);
            }
            return product;
        }

        static async Task<User> GetUserAsync(string path)
        {
            List<User> user = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                user = await response.Content.ReadAsAsync<List<User>>();
            }
            else
            {

                System.Diagnostics.Debug.WriteLine("There was an error whilst getting the user because " + response.ReasonPhrase);
            }
            return user.First();
        }

        protected async void Button1_Click(object sender, EventArgs e)
        {
            Product thing = await GetProductAsync("api/products/getproduct?pid=1");
            System.Diagnostics.Debug.WriteLine("Thing = " + thing.Name);
        }

        protected async void Button_GetUsers_Click(object sender, EventArgs e)
        {
            User me = await GetUserAsync("api/users/LogIn?pUsername=vamon1122&pPassword=BBTbbt1704");

            CheckMe();
            void CheckMe()
            {
                if (me != null)
                {
                    if (string.IsNullOrEmpty(me._forename))
                    {
                        System.Diagnostics.Debug.WriteLine("forename is blank");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Thing = " + me._forename);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Me is null! Waiting 3 seconds before retry");
                    var end = DateTime.Now.AddSeconds(3);
                    while (DateTime.Now < end) { }
                    CheckMe();
                }
            }
            
        }
    }
}