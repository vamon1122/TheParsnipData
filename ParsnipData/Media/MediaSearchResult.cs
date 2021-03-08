using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsnipData.Media
{
    public class MediaSearchResult
    {
        public MediaSearchResult(string searchTerms)
        {
            SearchTerms = searchTerms;
            Media = new List<RankedMedia>();
            MediaTags = new List<MediaTag>();
            Users = new List<Accounts.User>();
        }

        public string SearchTerms { get; set; }

        public List<RankedMedia> Media { get; set; }

        public List<MediaTag> MediaTags { get; set; }

        public List<Accounts.User> Users { get; set; }
    }
}
