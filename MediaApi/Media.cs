using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaApi
{
    public class MediaGroup
    {

    }

    public class Media
    {
        //src="resources/media/images/webMedia/pix-vertical-placeholder.jpg"	data-src="https://lh3.googleusercontent.com/4jCXzK4Yn5FMLVHnHAh3SZ1CG2HvfKrMHc7bqTv22xS8OXu3m4lR2xgnQG8uA_-maD7MrJek1HWYVR8QdjR3sGaih7BW7cOP-iGSXYfupYFnEQDQ_BnDtc_GMO5V3HfmMgPJ69H08g=w1920-h1080"	
        //data-srcset="https://lh3.googleusercontent.com/4jCXzK4Yn5FMLVHnHAh3SZ1CG2HvfKrMHc7bqTv22xS8OXu3m4lR2xgnQG8uA_-maD7MrJek1HWYVR8QdjR3sGaih7BW7cOP-iGSXYfupYFnEQDQ_BnDtc_GMO5V3HfmMgPJ69H08g=w1920-h1080"	class="meme	lazy" 	alt=""	

        

    }

    public class Photo
    {
        public Guid Id { get; set; }
        public string Placeholder { get; set; }
        public string PhotoSrc { get; set; }
        public string Classes { get; set; }
        public string Alt { get; set; }

    }

    public class Video : Media
    {

    }
}
