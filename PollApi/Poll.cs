using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PollApi
{
    public class Poll
    {
        public Guid CreatedByUserId { get; set; }
        public Guid DateTimeCreated { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
