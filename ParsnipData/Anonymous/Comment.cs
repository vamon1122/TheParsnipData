using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ParsnipData;

namespace ParsnipData.AnonymousApi
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
            date = Parsnip.adjustedTime;
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
                    SqlCommand insertComment = new SqlCommand("INSERT INTO comment VALUES(@id, @user_id, @post_id, @date_time_id, @text)", pConn);
                    insertComment.Parameters.Add(new SqlParameter("id", id));
                    insertComment.Parameters.Add(new SqlParameter("user_id", userId));
                    insertComment.Parameters.Add(new SqlParameter("post_id", postId));
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
