using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymousApi
{
    public class Post
    {
        public Guid id { get; set; }
        public Guid userId { get; set; }
        public DateTime date { get; set; }
        public string text { get; set; }

        public Post()
        {
            id = Guid.NewGuid();
            date = ParsnipApi.Data.adjustedTime;
        }

        public Post(Guid pGuid)
        {
            id = pGuid;
        }
    }
}
