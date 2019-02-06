using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Helpers;

namespace ParsnipWebsite.dev
{
    public partial class Photos : System.Web.UI.Page
    {
        //https://docs.microsoft.com/en-us/aspnet/web-pages/overview/ui-layouts-and-themes/9-working-with-images
        protected void Page_Load(object sender, EventArgs e)
        {
            /*
            WebImage photo = null;
            var newFileName = "";
            var imagePath = "";

            if (IsPost)
            {
                photo = WebImage.GetImageFromRequest();
                if (photo != null)
                {
                    newFileName = Guid.NewGuid().ToString() + "_" +
                        Path.GetFileName(photo.FileName);
                    imagePath = @"images\" + newFileName;

                    photo.Save(@"~\" + imagePath);
                }
            }
            */
        }
    }
}