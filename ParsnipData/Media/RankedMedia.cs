using System.Data.SqlClient;

namespace ParsnipData.Media
{
    public class RankedMedia : Media
    {
        public int RankScore { get; set; }

        public RankedMedia(SqlDataReader reader, int loggedInUserId) : base(reader, loggedInUserId)
        {

        }
    }
}
