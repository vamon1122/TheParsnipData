using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using ParsnipData;
using ParsnipData.Accounts;
using ParsnipData.Logging;
using System.Web;
using System.Net;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace ParsnipData.Media
{
    public class Youtube : Media
    {
        public string DataId { get; set; }
        public override string Type { get { return "youtube"; } }
        public override string UploadsDir { get { return "Resources/Media/Youtube/Thumbnails/"; } }
        private static string[] _allowedFileExtensions = new string[0];
        #region Constructors
        private Youtube()
        {

        }
        public Youtube(string dataId, User createdBy)
        {
            Id = MediaId.NewMediaId();
            DataId = dataId;
            DateTimeCaptured = Parsnip.AdjustedTime;
            DateTimeCreated = Parsnip.AdjustedTime;
            CreatedById = createdBy.Id;
        }
        #endregion

        #region Process Media
        public static string ParseDataId(string url)
        {
            Regex regex = new Regex(@"^(https?\:\/\/)?(www\.)?(youtube\.com|youtu\.?be)\/.+$");
            if (string.IsNullOrEmpty(url) || !regex.IsMatch(url) || (url.Length == 11 && !url.Contains('/') && !url.Contains('?')))
            {
                return null;
            }

            Uri uri = (url.Contains("https://") || url.Contains("http://")) ? new Uri(url) : new Uri($"https://{url}");
            string dataId;

            if (url.Contains("youtube.com/watch?v="))
            {
                dataId =  HttpUtility.ParseQueryString(uri.Query)["v"];
            }
            else if (url.Contains("youtu.be"))
            {
                dataId = uri.Segments[1];
            }           
            else
            {
                return null;
            }

            return dataId.Length == 11 ? dataId : null;
        }
        public void Scrape()
        {
            ScrapeTitle();
            ScrapeThumbnail();
            //int scale = Media.GetAspectScale(originalImage.Width, originalImage.Height);
        }
        private void ScrapeTitle()
        {
            var api = $"http://youtube.com/get_video_info?video_id=" + DataId;

            var youtubeApiResponse = WebUtility.UrlDecode(new WebClient().DownloadString(api));

            int titleStart = youtubeApiResponse.IndexOf("\",\"title\":\"");
            int titleEnd = youtubeApiResponse.IndexOf("\",\"lengthSeconds\":\"");

            Title = youtubeApiResponse.Substring(titleStart + 11, titleEnd - titleStart - 11);
        }
        public void ScrapeThumbnail()
        {
            string uploadsDir = "Resources/Media/Youtube/Thumbnails/";
            string fullyQualifiedUploadsDir = HttpContext.Current.Server.MapPath($"~/{uploadsDir}");



            string generatedFileName = $"{Id}_{DataId}";


            string ScrapeThumbnailUrl = $"https://i.ytimg.com/vi/{DataId}/mqdefault.jpg";


            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(ScrapeThumbnailUrl), $"{fullyQualifiedUploadsDir}/Originals/{generatedFileName}.jpg");
            }

            ProcessMediaThumbnail(this, generatedFileName, ".jpg");
        }
        #endregion

        #region CRUD
        public new static Youtube Select(MediaId youtubeVideoId, int loggedInUserId)
        {
            Youtube youtubeVideo = null;
            try
            {
                using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {


                    using (var selectYoutubeVideo = new SqlCommand("youtube_SELECT_WHERE_media_id", conn))
                    {
                        selectYoutubeVideo.CommandType = CommandType.StoredProcedure;
                        selectYoutubeVideo.Parameters.Add(new SqlParameter("media_id", youtubeVideoId.ToString()));
                        selectYoutubeVideo.Parameters.Add(new SqlParameter("logged_in_user_id", loggedInUserId));

                        conn.Open();
                        using (SqlDataReader reader = selectYoutubeVideo.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                youtubeVideo = new Youtube();
                                youtubeVideo.AddValues(reader, loggedInUserId);
                            }

                            if (youtubeVideo != null)
                            {
                                reader.NextResult();

                                youtubeVideo.MediaTagPairs = new List<MediaTagPair>();
                                while (reader.Read())
                                {
                                    var mediaTag = new MediaTagPair(reader);
                                    youtubeVideo.MediaTagPairs.Add(mediaTag);
                                }

                                reader.NextResult();

                                youtubeVideo.MediaUserPairs = new List<MediaUserPair>();
                                while (reader.Read())
                                {
                                    var mediaUserPair = new MediaUserPair(reader);
                                    youtubeVideo.MediaUserPairs.Add(mediaUserPair);
                                }
                            }
                        }
                    }

                    if (youtubeVideo == null)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"There was an exception whilst getting youtubeVideo data: {ex}");
            }
            return youtubeVideo;
        }
        public override bool Insert()
        {
            if (Compressed != null)
            {
                if (Id == null || Id.ToString() == MediaId.Empty.ToString() || Type == default)
                {
                    Id = MediaId.NewMediaId();
                }


                try
                {
                    using (var conn = new SqlConnection(ParsnipData.Parsnip.ParsnipConnectionString))
                    {
                        using (SqlCommand insertMedia = new SqlCommand("youtube_INSERT", conn))
                        {
                            insertMedia.CommandType = CommandType.StoredProcedure;

                            insertMedia.Parameters.AddWithValue("media_id", Id.ToString());
                            insertMedia.Parameters.AddWithValue("data_id", DataId);
                            insertMedia.Parameters.AddWithValue("type", Type);
                            insertMedia.Parameters.AddWithValue("datetime_captured", DateTimeCaptured);
                            insertMedia.Parameters.AddWithValue("datetime_created", DateTimeCreated);
                            insertMedia.Parameters.AddWithValue("x_scale", XScale);
                            insertMedia.Parameters.AddWithValue("y_scale", YScale);
                            insertMedia.Parameters.AddWithValue("original_dir", Original);
                            insertMedia.Parameters.AddWithValue("compressed_dir", Compressed);
                            insertMedia.Parameters.AddWithValue("placeholder_dir", Placeholder);
                            insertMedia.Parameters.AddWithValue("created_by_user_id", CreatedById);
                            if (AlbumId != default)
                                insertMedia.Parameters.AddWithValue("media_tag_id", AlbumId);

                            conn.Open();
                            insertMedia.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    string error = $"Failed to insert media into the database: {e}";
                    Debug.WriteLine(error);
                    new LogEntry(Log.General) { text = error };
                    return false;
                }
                new LogEntry(Log.General) { text = "Media was successfully inserted into the database!" };
                return Update();

            }
            else
            {
                throw new InvalidOperationException("Media cannot be inserted. The media's property: src must be initialised before it can be inserted!");
            }
        }
        public override bool Update()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ParsnipData.Parsnip.ParsnipConnectionString))
                {
                    using (SqlCommand updateYoutubeVideo = new SqlCommand("media_UPDATE", conn))
                    {
                        updateYoutubeVideo.CommandType = CommandType.StoredProcedure;

                        updateYoutubeVideo.Parameters.Add(new SqlParameter("id", Id.ToString()));
                        updateYoutubeVideo.Parameters.Add(new SqlParameter("title", Title));
                        updateYoutubeVideo.Parameters.Add(new SqlParameter("description", Description));
                        updateYoutubeVideo.Parameters.Add(new SqlParameter("alt", Alt));
                        updateYoutubeVideo.Parameters.Add(new SqlParameter("datetime_captured", DateTimeCaptured));
                        updateYoutubeVideo.Parameters.Add(new SqlParameter("media_tag_id", AlbumId));

                        //Needs updating so the person who updates is inserted here
                        updateYoutubeVideo.Parameters.AddWithValue("media_tag_created_by_user_id", CreatedById);
                        updateYoutubeVideo.Parameters.AddWithValue("x_scale", XScale);
                        updateYoutubeVideo.Parameters.AddWithValue("y_scale", YScale);
                        updateYoutubeVideo.Parameters.AddWithValue("placeholder_dir", Placeholder);
                        updateYoutubeVideo.Parameters.AddWithValue("compressed_dir", Compressed);
                        updateYoutubeVideo.Parameters.AddWithValue("original_dir", Original);

                        conn.Open();
                        updateYoutubeVideo.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                string error = string.Format("There was an error whilst updating youtubeVideo: {0}", e);
                Debug.WriteLine(error);
                new LogEntry(Log.General) { text = error };
                return false;
            }
            new LogEntry(Log.Debug) { text = string.Format("YoutubeVideo was successfully updated on the database!") };
            return true;
        }
        protected override bool AddValues(SqlDataReader reader, int loggedInUserId)
        {
            bool logMe = true;

            try
            {
                Id = new MediaId(reader[0].ToString());
                DataId = reader[1].ToString().Trim();
                DateTimeCaptured = Convert.ToDateTime(reader[2]);
                DateTimeCreated = Convert.ToDateTime(reader[3]);
                CreatedById = (int)reader[4];

                if (reader[5] != DBNull.Value && !string.IsNullOrEmpty(reader[5].ToString()) && !string.IsNullOrWhiteSpace(reader[5].ToString()))
                    Title = reader[5].ToString().Trim();


                if (reader[6] != DBNull.Value && !string.IsNullOrEmpty(reader[6].ToString()) && !string.IsNullOrWhiteSpace(reader[6].ToString()))
                    Description = reader[6].ToString().Trim();


                try
                {
                    if (reader[8] != DBNull.Value && !string.IsNullOrEmpty(reader[8].ToString()) && !string.IsNullOrWhiteSpace(reader[8].ToString()))
                    {
                        AlbumId = (int)reader[8];
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    //These values are not necessarily returned in all queries
                }

                try
                {
                    if (reader[9] == DBNull.Value || string.IsNullOrEmpty(reader[9].ToString()) ||
                        string.IsNullOrWhiteSpace(reader[9].ToString()))

                    {
                        if (loggedInUserId != default)
                        {
                            MyMediaShare = new MediaShare(Id, loggedInUserId);
                            MyMediaShare.Insert();
                        }

                    }
                    else
                    {
                        MyMediaShare = new MediaShare(new MediaShareId((string)reader[9]), (int)reader[10], Convert.ToDateTime(reader[11]), default, new MediaId((string)reader[12]));
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    //These values are not necessarily returned in all queries. No exception needs to be thrown
                }

                Placeholder = reader[13].ToString().Trim();
                Compressed = reader[14].ToString().Trim();
                Original = (string)reader[15].ToString().Trim();

                if(reader[16] != DBNull.Value)
                    Alt = (string)reader[16].ToString().Trim();

                XScale = (double)reader[17];

                YScale = (double)reader[18];

                CheckTitle();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("There was an error whilst reading the YoutubeVideo's values: " + ex);
                return false;
            }

            void CheckTitle()
            {
                if (string.IsNullOrEmpty(Title))
                    ScrapeTitle();
            }
        }
        #endregion


    }
}
