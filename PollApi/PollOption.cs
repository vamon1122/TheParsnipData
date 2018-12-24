using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PollApi
{
    class PollOption
    {
        public Guid PollId { get; set; }
        public string Value { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime DateTimeCreated { get; set; }
        
    }
}
