using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsnipData.Media
{
    public class MediaUserPair
    {
        public MediaId MediaId { get; set; }
        public int UserId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime DateTimeDeleted { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public string Name { get; set; }

        public MediaUserPair()
        {

        }

        public MediaUserPair(Media media, int userId, ParsnipData.Accounts.User createdByUser)
        {
            MediaId = media.Id;
            UserId = userId;
            CreatedByUserId = createdByUser.Id;
            DateTimeCreated = ParsnipData.Parsnip.AdjustedTime;
        }

        public MediaUserPair(SqlDataReader reader)
        {
            AddValues(reader);
        }

        public void AddValues(SqlDataReader reader)
        {
            MediaId = new MediaId(reader[0].ToString());
            UserId = (int)reader[1];
            CreatedByUserId = (int)reader[2];
            Name = reader[3].ToString().Trim();
        }

        public static List<Media> GetAllMedia(int tagUserId, int loggedInUserId)
        {
            List<Media> media = new List<Media>();

            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                using (SqlCommand getMedia = new SqlCommand("media_SELECT_WHERE_media_user_pair_user_id", conn))
                {
                    getMedia.CommandType = CommandType.StoredProcedure;
                    getMedia.Parameters.Add(new SqlParameter("user_id", tagUserId));
                    getMedia.Parameters.Add(new SqlParameter("logged_in_user_id", loggedInUserId));

                    conn.Open();
                    using (SqlDataReader reader = getMedia.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            media.Add(new Media(reader, loggedInUserId));
                        }
                    }
                }

            }
            return media;
        }

        public void Insert()
        {
            using (var conn = new SqlConnection(ParsnipData.Parsnip.ParsnipConnectionString))
            {
                using (var insertMediaUserPair = new SqlCommand("media_user_pair_INSERT", conn))
                {
                    insertMediaUserPair.CommandType = System.Data.CommandType.StoredProcedure;

                    insertMediaUserPair.Parameters.AddWithValue("media_id", MediaId.ToString());
                    insertMediaUserPair.Parameters.AddWithValue("user_id", UserId);
                    insertMediaUserPair.Parameters.AddWithValue("created_by_user_id", CreatedByUserId);

                    conn.Open();
                    insertMediaUserPair.ExecuteNonQuery();
                }
            }
        }

        public static void Delete(MediaId mediaId, int userId)
        {
            using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                using (var deleteMediaUserPair = new SqlCommand("media_user_pair_DELETE_where_media_id_AND_user_id", conn))
                {
                    deleteMediaUserPair.CommandType = System.Data.CommandType.StoredProcedure;

                    deleteMediaUserPair.Parameters.AddWithValue("media_id", mediaId.ToString());
                    deleteMediaUserPair.Parameters.AddWithValue("user_id", userId);
                    deleteMediaUserPair.Parameters.AddWithValue("datetime_deleted", Parsnip.AdjustedTime);

                    conn.Open();

                    deleteMediaUserPair.ExecuteNonQuery();
                }
            }
        }
    }
}
