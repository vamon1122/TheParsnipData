using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ParsnipData.Accounts;
using ParsnipData.Media;
using ParsnipData.Logs;
using System.Data.SqlClient;
using ParsnipData;
using System.Diagnostics;
using System.Data;

namespace ParsnipData.Media
{
    public class AccessToken
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public int TimesUsed { get; set; }
        public string ImageRedirect
        {
            get
            {

                var myImage = new ParsnipData.Media.Image(MediaId);
                myImage.Select();
                return string.Format("/view_image?access_token={0}", Id);
            }
        }
        public string VideoRedirect
        {
            get
            {
                var myImage = new ParsnipData.Media.Image(MediaId);
                myImage.Select();
                return string.Format("/video_player?access_token={0}", Id);
            }
        }
        public Guid MediaId { get; set; }

        public static bool TokenExists(Guid userId, Guid mediaId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();

                    var selectAccessToken = new SqlCommand("SELECT access_token_id FROM access_token WHERE " +
                        "created_by_user_id = @created_by_user_id AND media_id = @media_id", conn);

                    selectAccessToken.Parameters.Add(new SqlParameter("created_by_user_id", userId));
                    selectAccessToken.Parameters.Add(new SqlParameter("media_id", mediaId));

                    using (SqlDataReader reader = selectAccessToken.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

        public static DataTable GetStats()
        {
            var allStats = new DataTable();

            var allStatsYoutubeVideo = new DataTable();
            var allStatsVideo = new DataTable();
            var allStatsImage = new DataTable();
            //try
            //{
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                var getImageStats = new SqlCommand(
                    "SELECT image.image_id AS media_id, image.title, uploaded_by.forename, shared_by.forename, " +
                    "access_token.times_used, access_token.access_token_id, media_tag_pair.media_tag_id, " +
                    "shared_by.user_id " +

                    "FROM access_token " +
                    "INNER JOIN image ON access_token.media_id = image.image_id " +
                    "INNER JOIN[user] AS uploaded_by ON image.created_by_user_id = uploaded_by.user_id " +
                    "INNER JOIN[user] AS shared_by ON access_token.created_by_user_id = shared_by.user_id " +
                    "LEFT JOIN media_tag_pair ON image.image_id = media_tag_pair.media_id " +

                    "ORDER BY times_used DESC", conn);

                var getVideoStats = new SqlCommand(
                    "SELECT video.video_id AS media_id, " +
                    "video.title, " +
                    "uploaded_by.forename, " +
                    "shared_by.forename, " +
                    "access_token.times_used , " +
                    "access_token.access_token_id, " +
                    "media_tag_pair.media_tag_id, " +
                    "shared_by.user_id " +



                    "FROM access_token " +
                    "INNER JOIN video ON access_token.media_id = video.video_id " +
                    "INNER JOIN [user] AS uploaded_by ON video.created_by_user_id = uploaded_by.user_id " +
                    "INNER JOIN [user] AS shared_by ON access_token.created_by_user_id = shared_by.user_id " +
                    "LEFT JOIN media_tag_pair ON video.video_id = media_tag_pair.media_id " +

                    "ORDER BY times_used DESC", conn);

                var getYoutubeVideoStats = new SqlCommand(
                    "SELECT youtube_video.youtube_video_id AS media_id, " +
                    "youtube_video.title, " +
                    "uploaded_by.forename, " +
                    "shared_by.forename, " +
                    "access_token.times_used, " +
                    "access_token.access_token_id, " +
                    "media_tag_pair.media_tag_id, " +
                    "shared_by.user_id " +

                    "FROM access_token " +
                    "INNER JOIN youtube_video ON access_token.media_id = youtube_video.youtube_video_id " +
                    "INNER JOIN[user] AS uploaded_by ON youtube_video.created_by_user_id = uploaded_by.user_id " +
                    "INNER JOIN[user] AS shared_by ON access_token.created_by_user_id = shared_by.user_id " +
                    "LEFT JOIN media_tag_pair ON youtube_video.youtube_video_id = media_tag_pair.media_id " +

                    "ORDER BY times_used DESC" ,conn);

                using (var imageStats = getImageStats.ExecuteReader())
                {
                    allStats.Load(imageStats);
                    allStatsImage.Load(imageStats);
                }

                using (var videoStats = getVideoStats.ExecuteReader())
                {
                    allStats.Load(videoStats);
                    allStatsVideo.Load(videoStats);
                }

                using (var youtubeVideoStats = getYoutubeVideoStats.ExecuteReader())
                {
                    allStats.Load(youtubeVideoStats);
                    allStatsYoutubeVideo.Load(youtubeVideoStats);
                }

                allStats.DefaultView.Sort = "times_used DESC";
                allStats = allStats.DefaultView.ToTable();
            }
            /*}
            catch(Exception ex)
            {
                throw ex;
            }*/

            return allStats;
        }

        public static AccessToken GetToken(Guid userId, Guid mediaId)
        {
            AccessToken myToken = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();

                    var selectAccessToken = new SqlCommand("SELECT access_token_id FROM access_token INNER JOIN [user] ON [user].user_id = access_token.created_by_user_id WHERE " +
                        "created_by_user_id = @created_by_user_id AND media_id = @media_id AND [user].deleted IS NULL", conn);
                    selectAccessToken.Parameters.Add(new SqlParameter("created_by_user_id", userId));
                    selectAccessToken.Parameters.Add(new SqlParameter("media_id", mediaId));

                    using (SqlDataReader reader = selectAccessToken.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            myToken = new AccessToken((Guid)reader[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (myToken == null)
                throw new NullReferenceException();

            myToken.Select();

            return myToken;
        }

        public AccessToken(Guid userId, Guid mediaId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            DateTimeCreated = ParsnipData.Parsnip.AdjustedTime;
            TimesUsed = 0;
            MediaId = mediaId;
        }

        public AccessToken(Guid id)
        {
            Id = id;
        }

        public void Select()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();

                    Debug.WriteLine("Selecting access token...");

                    var selectAccessToken = 
                        new SqlCommand("SELECT * FROM access_token INNER JOIN [user] ON [user].user_id = access_token.created_by_user_id WHERE access_token_id = @id AND [user].deleted IS NULL", conn);

                    selectAccessToken.Parameters.Add(new SqlParameter("id", Id));

                    Debug.WriteLine("Reading...");

                    using (SqlDataReader reader = selectAccessToken.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Debug.WriteLine("reader[1] = " + reader[1]);
                            AddValues(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            void AddValues(SqlDataReader reader)
            {
                Debug.WriteLine("Reading MediaId = " + reader[4]);
                Id = (Guid)reader[0];
                UserId = (Guid)reader[1];
                DateTimeCreated = (DateTime)reader[2];
                TimesUsed = (int)reader[3];
                MediaId = (Guid)reader[4];
            }
        }

        public void Insert()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();

                    var insertAccessToken = new SqlCommand("INSERT INTO access_token VALUES (@access_token_id, " +
                        "@created_by_user_id, @date_time_created, @times_used, @media_id)", conn);

                    insertAccessToken.Parameters.Add(new SqlParameter("access_token_id", Id));
                    insertAccessToken.Parameters.Add(new SqlParameter("created_by_user_id", UserId));
                    insertAccessToken.Parameters.Add(new SqlParameter("date_time_created", DateTimeCreated));
                    insertAccessToken.Parameters.Add(new SqlParameter("times_used", TimesUsed));
                    insertAccessToken.Parameters.Add(new SqlParameter("media_id", MediaId));

                    insertAccessToken.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();

                    var updateAccessToken = new SqlCommand("UPDATE access_token SET times_used = @times_used WHERE " +
                        "access_token_id = @access_token_id", conn);

                    updateAccessToken.Parameters.Add(new SqlParameter("access_token_id", Id));
                    updateAccessToken.Parameters.Add(new SqlParameter("times_used", TimesUsed));

                    updateAccessToken.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}