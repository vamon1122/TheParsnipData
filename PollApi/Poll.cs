using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParsnipApi;

namespace PollApi
{
    public class Poll
    {
        public Guid id { get; }
        public Guid createdByUserId { get; set; }
        public DateTime dateCreated { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public Poll(Guid pPollId)
        {
            id = pPollId;
        }

        public Poll()
        {
            id = Guid.NewGuid();
            dateCreated = ParsnipApi.Data.adjustedTime;
        }

        public bool Insert()
        {
            return Data.InsertPoll(this); ;
        }
    }
}
