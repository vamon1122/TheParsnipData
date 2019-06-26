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
                using (SqlConnection conn = ParsnipData.Parsnip.GetOpenDbConnection())
                {
                    var selectAccessToken = new SqlCommand("SELECT access_token_id FROM access_token WHERE user_id = @user_id AND media_id = @media_id", conn);
                    selectAccessToken.Parameters.Add(new SqlParameter("user_id", userId));
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

            var allStatsVideo = new DataTable();
            var allStatsImage = new DataTable();
            //try
            //{
            using (SqlConnection conn = Parsnip.GetOpenDbConnection())
            {
                var getImageStats = new SqlCommand(
                    "SELECT t_Images.id AS media_id, " +
                    "t_Images.title, " +
                    "uploaded_by.forename," +
                    "shared_by.forename," +
                    "access_token.times_used," +
                    "access_token.access_token_id, " +
                    "t_ImageAlbumPairs.albumid, " +
                    "shared_by.id " +

                    "FROM access_token " +
                    "INNER JOIN t_Images ON access_token.media_id = t_Images.Id " +
                    "INNER JOIN t_Users AS uploaded_by ON t_Images.createdbyid = uploaded_by.Id " +
                    "INNER JOIN t_Users AS shared_by ON access_token.user_id = shared_by.Id " +
                    "INNER JOIN t_ImageAlbumPairs ON t_Images.id = t_ImageAlbumPairs.imageid " +

                    "ORDER BY times_used DESC", conn);

                var getVideoStats = new SqlCommand(
                    "SELECT video.video_id AS media_id, " +
                    "video.title, " +
                    "uploaded_by.forename, " +
                    "shared_by.forename, " +
                    "access_token.times_used , " +
                    "access_token.access_token_id, " +
                    "t_ImageAlbumPairs.albumid, " +
                    "shared_by.id " +



                    "FROM access_token " +
                    "INNER JOIN video ON access_token.media_id = video.video_id " +
                    "INNER JOIN t_Users AS uploaded_by ON video.created_by_id = uploaded_by.Id " +
                    "INNER JOIN t_Users AS shared_by ON access_token.user_id = shared_by.Id " +
                    "INNER JOIN t_ImageAlbumPairs ON video.video_id = t_ImageAlbumPairs.imageid " +

                    "ORDER BY times_used DESC", conn);

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
                using (SqlConnection conn = ParsnipData.Parsnip.GetOpenDbConnection())
                {
                    var selectAccessToken = new SqlCommand("SELECT access_token_id FROM access_token WHERE user_id = @user_id AND media_id = @media_id", conn);
                    selectAccessToken.Parameters.Add(new SqlParameter("user_id", userId));
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
            DateTimeCreated = ParsnipData.Parsnip.adjustedTime;
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
                using (SqlConnection conn = ParsnipData.Parsnip.GetOpenDbConnection())
                {
                    var selectAccessToken = new SqlCommand("SELECT * FROM access_token WHERE access_token_id = @id", conn);
                    selectAccessToken.Parameters.Add(new SqlParameter("id", Id));

                    using (SqlDataReader reader = selectAccessToken.ExecuteReader())
                    {
                        while (reader.Read())
                        {
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
                using (SqlConnection conn = ParsnipData.Parsnip.GetOpenDbConnection())
                {
                    var insertAccessToken = new SqlCommand("INSERT INTO access_token VALUES (@access_token_id, @user_id, @date_time_created, @times_used, @media_id)", conn);
                    insertAccessToken.Parameters.Add(new SqlParameter("access_token_id", Id));
                    insertAccessToken.Parameters.Add(new SqlParameter("user_id", UserId));
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
                using (SqlConnection conn = ParsnipData.Parsnip.GetOpenDbConnection())
                {
                    var updateAccessToken = new SqlCommand("UPDATE access_token SET times_used = @times_used WHERE access_token_id = @access_token_id", conn);
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