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

        public Comment(SqlDataReader pReader)
        {
            id = new Guid(pReader[0].ToString());
            userId = new Guid(pReader[1].ToString());
            postId = new Guid(pReader[2].ToString());
            date = Convert.ToDateTime(pReader[3]);
            text = pReader[4].ToString();
        }

        public bool Insert()
        {
            SqlConnection temp = new SqlConnection(ParsnipApi.Data.sqlConnectionString);
            temp.Open();
            return Insert(temp);
        }

        public bool Insert(SqlConnection pConn)
        {
            try
            {
                using (pConn)
                {
                    SqlCommand insertComment = new SqlCommand("INSERT INTO t_Comments VALUES(@id, @userId, @postId, @date, @text)", pConn);
                    insertComment.Parameters.Add(new SqlParameter("id", id));
                    insertComment.Parameters.Add(new SqlParameter("userId", userId));
                    insertComment.Parameters.Add(new SqlParameter("postId", postId));
                    insertComment.Parameters.Add(new SqlParameter("date", date));
                    insertComment.Parameters.Add(new SqlParameter("text", text));

                    insertComment.ExecuteNonQuery();

                    return true;
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("There was an exception whilst inserting the post comment: {1}", e);
                return false;
            }
        }
    }
}
