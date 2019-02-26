using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MediaApi;
using LogApi;

namespace ParsnipWebsite.Custom_Controls.Media_Api
{
    public partial class ImageControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        private MediaApi.Image myImage;
        public MediaApi.Image MyImage { get { return myImage; } set
            {
                myImage = value;
                MyTitle.InnerHtml = MyImage.Title;
                MyImageHolder.ImageUrl = "../../Resources/Media/Images/Web_Media/placeholder.gif";
                MyImageHolder.Attributes.Add("data-src", MyImage.ImageSrc);
                MyImageHolder.Attributes.Add("data-srcset", MyImage.ImageSrc);
                MyImageHolder.CssClass = "meme lazy";
                MyEdit.HRef = string.Format("../../edit_image?redirect=photos&imageid={0}", MyImage.Id);
                MyShare.HRef = string.Format("../../view_image?imageid={0}", MyImage.Id);
            }
        }
    }

    
}