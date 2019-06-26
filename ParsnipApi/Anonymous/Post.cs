using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ParsnipData;

namespace AnonymousApi
{
    public class Post
    {
        public Guid id { get; set; }
        public Guid userId { get; set; }
        public DateTime date { get; set; }
        public string text { get; set; }
        public string type { get; set; }

        public Post()
        {
            id = Guid.NewGuid();
            date = Parsnip.adjustedTime;
        }

        public Post (Guid guid)
        {
            throw new NotImplementedException();
        }

        public Post(SqlDataReader pReader)
        {
            id = new Guid(pReader[0].ToString());
            userId = new Guid(pReader[1].ToString());
            date = Convert.ToDateTime(pReader[2]);
            text = pReader[3].ToString();
        }

        public bool Insert()
        {
            SqlConnection temp = new SqlConnection(Parsnip.sqlConnectionString);
            temp.Open();
            return Insert(temp);
        }

        public bool Insert(SqlConnection pConn)
        {
            try
            {
                using (pConn)
                {
                    SqlCommand insertPost = new SqlCommand("INSERT INTO t_Posts VALUES(@id, @userid, @date, @text)", pConn);
                    insertPost.Parameters.Add(new SqlParameter("id", id));
                    insertPost.Parameters.Add(new SqlParameter("userid", userId));
                    insertPost.Parameters.Add(new SqlParameter("date", date));
                    insertPost.Parameters.Add(new SqlParameter("text", text));

                    insertPost.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("There was an exception whilst inserting the post: {1}", e);
                return false;
            }
        }
    }
}
