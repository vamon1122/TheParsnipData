using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using ParsnipData;
using ParsnipData.Accounts;
using ParsnipData.Logs;

namespace ParsnipData.Media
{
    public class MediaGroup
    {

    }

    public abstract class Media
    {
        static readonly Log DebugLog = new Log("Debug");

        public static List<Image> GetImagesByUserId(Guid userId)
        {
            Debug.WriteLine("Getting all images for user");
            List<Image> Images = new List<Image>();

            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                SqlCommand GetImages = new SqlCommand("SELECT image.*, media_tag_pair.media_tag_id FROM image " +
                    "LEFT JOIN media_tag_pair ON image.image_id = media_tag_pair.media_id " +
                    "INNER JOIN [user] ON [user].user_id = image.created_by_user_id " +
                    "WHERE image.deleted IS NULL AND image.created_by_user_id = @created_by_user_id AND " +
                    "[user].deleted IS NULL ORDER BY image.date_time_created DESC", conn);

                GetImages.Parameters.Add(new SqlParameter("created_by_user_id", userId));

                using (SqlDataReader reader = GetImages.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Images.Add(new Image(reader));
                    }
                }

            }

            Debug.WriteLine("" + Images.Count() + "images were found");

            return Images;
        }

        public static List<Video> GetVideosByUserId(Guid userId)
        {
            Debug.WriteLine("Getting all Videos for user");
            List<Video> Videos = new List<Video>();

            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                SqlCommand GetVideos = new SqlCommand("SELECT video.*, media_tag_pair.media_tag_id FROM video " +
                    "LEFT JOIN media_tag_pair ON video.video_id = media_tag_pair.media_id " +
                    "INNER JOIN [user] ON [user].user_id = video.created_by_user_id " +
                    "WHERE video.deleted IS NULL AND video.created_by_user_id = @created_by_user_id AND " +
                    "[user].deleted IS NULL ORDER BY video.date_time_created DESC", conn);

                GetVideos.Parameters.Add(new SqlParameter("created_by_user_id", userId));

                using (SqlDataReader reader = GetVideos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Videos.Add(new Video(reader));
                    }
                }

            }

            Debug.WriteLine("" + Videos.Count() + "videos were found");

            return Videos;
        }

        public static List<YoutubeVideo> GetYoutubeVideosByUserId(Guid userId)
        {
            Debug.WriteLine("Getting all YoutubeVideos for user");
            List<YoutubeVideo> YoutubeVideos = new List<YoutubeVideo>();

            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                SqlCommand GetYoutubeVideos = new SqlCommand("SELECT youtube_video.*, media_tag_pair.media_tag_id FROM youtube_video " +
                    "LEFT JOIN media_tag_pair ON youtube_video.youtube_video_id = media_tag_pair.media_id " +
                    "INNER JOIN [user] ON [user].user_id = youtube_video.created_by_user_id " +
                    "WHERE youtube_video.deleted IS NULL AND youtube_video.created_by_user_id = @created_by_user_id AND " +
                    "[user].deleted IS NULL ORDER BY youtube_video.date_time_created DESC", conn);

                GetYoutubeVideos.Parameters.Add(new SqlParameter("created_by_user_id", userId));

                using (SqlDataReader reader = GetYoutubeVideos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        YoutubeVideos.Add(new YoutubeVideo(reader));
                    }
                }

            }

            Debug.WriteLine("" + YoutubeVideos.Count() + "youtube_videos were found");

            return YoutubeVideos;
        }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AlbumId { get; set; }
        public abstract List<Guid> AlbumIds();
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public abstract string[] AllowedFileExtensions { get; }
        public string Directory { get; set; }
        public abstract bool Delete();

        public abstract bool Update();
        //src="resources/media/images/webMedia/pix-vertical-placeholder.jpg"	data-src="https://lh3.googlephotocontent.com/4jCXzK4Yn5FMLVHnHAh3SZ1CG2HvfKrMHc7bqTv22xS8OXu3m4lR2xgnQG8uA_-maD7MrJek1HWYVR8QdjR3sGaih7BW7cOP-iGSXYfupYFnEQDQ_BnDtc_GMO5V3HfmMgPJ69H08g=w1920-h1080"	
        //data-srcset="https://lh3.googlephotocontent.com/4jCXzK4Yn5FMLVHnHAh3SZ1CG2HvfKrMHc7bqTv22xS8OXu3m4lR2xgnQG8uA_-maD7MrJek1HWYVR8QdjR3sGaih7BW7cOP-iGSXYfupYFnEQDQ_BnDtc_GMO5V3HfmMgPJ69H08g=w1920-h1080"	class="meme	lazy" 	alt=""	



    }

    

    
}
