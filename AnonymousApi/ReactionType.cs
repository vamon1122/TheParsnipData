using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymousApi
{
    public class ReactionType
    {
        public Guid id { get; set; }
        public DateTime date { get; set; }
        public string text { get; set; }

        public ReactionType()
        {
            id = Guid.NewGuid();
            date = ParsnipApi.Data.adjustedTime;
        }

        public ReactionType(Guid pGuid)
        {
            id = pGuid;
        }
    }
}
