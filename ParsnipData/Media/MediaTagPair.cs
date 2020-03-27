using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsnipData.Media
{
    public class MediaTagPair
    {
        public MediaId MediaId { get; set; }
        public MediaTag MediaTag { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime DateTimeDeleted { get; set; }
        public DateTime DateTimeCreated { get; set; }

        public MediaTagPair(SqlDataReader reader)
        {
            AddValues(reader);
        }

        public void AddValues(SqlDataReader reader)
        {
            MediaId = new MediaId(reader[0].ToString());
            MediaTag = new MediaTag((int)reader[1]);
            CreatedByUserId = (int)reader[2];
            DateTimeCreated = Convert.ToDateTime(reader[3]);
            MediaTag.CreatedById = (int)reader[4];
            MediaTag.DateCreated = Convert.ToDateTime(reader[5]);
            MediaTag.Name = reader[6].ToString().Trim();

            if (reader[7] != DBNull.Value)
                MediaTag.Description = reader[8].ToString();
        }

        public static void Delete(MediaId mediaId, int mediaTagId )
        {
            using(var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                using(var deleteMediaTagPair = new SqlCommand("media_tag_pair_DELETE_where_media_id_AND_media_tag_id", conn))
                {
                    deleteMediaTagPair.CommandType = System.Data.CommandType.StoredProcedure;

                    deleteMediaTagPair.Parameters.AddWithValue("media_id", mediaId.ToString());
                    deleteMediaTagPair.Parameters.AddWithValue("media_tag_id", mediaTagId);
                    deleteMediaTagPair.Parameters.AddWithValue("datetime_deleted", Parsnip.AdjustedTime);

                    conn.Open();

                    deleteMediaTagPair.ExecuteNonQuery();
                }
            }
        }
    }
}
