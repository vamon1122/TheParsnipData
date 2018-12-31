using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

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

        public Comment(SqlDataReader reader)
        {

        }

        public bool UpdateDb()
        {
            SqlConnection temp = new SqlConnection(ParsnipApi.Data.sqlConnectionString);
            temp.Open();
            return UpdateDb(temp);
        }

        public bool UpdateDb(SqlConnection pConn)
        {

        }
    }
}
