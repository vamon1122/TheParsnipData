using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using ParsnipApi;
using UacApi;
using LogApi;

namespace MediaApi
{
    public class MediaGroup
    {

    }

    public abstract class Media
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AlbumId { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public abstract string[] AllowedFileExtensions { get; }
        public string Directory { get; set; }
        //src="resources/media/images/webMedia/pix-vertical-placeholder.jpg"	data-src="https://lh3.googlephotocontent.com/4jCXzK4Yn5FMLVHnHAh3SZ1CG2HvfKrMHc7bqTv22xS8OXu3m4lR2xgnQG8uA_-maD7MrJek1HWYVR8QdjR3sGaih7BW7cOP-iGSXYfupYFnEQDQ_BnDtc_GMO5V3HfmMgPJ69H08g=w1920-h1080"	
        //data-srcset="https://lh3.googlephotocontent.com/4jCXzK4Yn5FMLVHnHAh3SZ1CG2HvfKrMHc7bqTv22xS8OXu3m4lR2xgnQG8uA_-maD7MrJek1HWYVR8QdjR3sGaih7BW7cOP-iGSXYfupYFnEQDQ_BnDtc_GMO5V3HfmMgPJ69H08g=w1920-h1080"	class="meme	lazy" 	alt=""	



    }

    

    
}
