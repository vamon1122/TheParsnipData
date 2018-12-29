using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymousApi
{
    public class Comment
    {
        public Guid id { get; set; }
        public Guid userId { get; set; }
        public Guid postId { get; set; }
        public DateTime date { get; set; }
        public string text { get; set; }

        public Comment()
        {
            id = Guid.NewGuid();
            date = ParsnipApi.Data.adjustedTime;
        }

        public Comment(Guid pGuid)
        {
            id = pGuid;
        }
    }
}
