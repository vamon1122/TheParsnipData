using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParsnipData;

namespace AnonymousApi
{
    class Reaction
    {
        public Guid id { get; set; }
        public Guid userId { get; set; }
        public Guid postId { get; set; }
        public DateTime date { get; set; }
        public ReactionType reactionType { get; set; }

        public Reaction()
        {
            id = Guid.NewGuid();
            date = Parsnip.adjustedTime;
        }

        public Reaction(Guid pGuid)
        {
            id = pGuid;
        }
    }
}
