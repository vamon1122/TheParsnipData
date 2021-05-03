using ParsnipData.Accounts;
using ParsnipData.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ParsnipData;
using System.Web.UI;

namespace ParsnipData.Media
{
    public class MediaView
    {
        public Media Media
        {
            get
            {

                if (Image != null)
                    return Image;
                if (Video != null)
                    return Video;
                if (YoutubeVideo != null)
                    return YoutubeVideo;
                return null;
            }
        }
        public Media Image { get; set; }
        public Video Video { get; set; }
        public Youtube YoutubeVideo { get; set; }

        public MediaView()
        {
            //Image = new Image();
        }
    }
}
