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
    public class Youtube : Video
    {
        public string DataId { get; set; }
        public override string Type { get { return "youtube"; } }
        private static string[] _allowedFileExtensions = new string[0];
        #region Constructors
        private Youtube()
        {

        }
        public Youtube(SqlDataReader pReader, int loggedInUserId = default) : this()
        {
            AddValues(pReader, loggedInUserId);
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

                                reader.NextResult();

                                youtubeVideo.Thumbnails = new List<VideoThumbnail>();
                                while (reader.Read())
                                {
                                    var videoThumbnail = new VideoThumbnail(reader);
                                    youtubeVideo.Thumbnails.Add(videoThumbnail);
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
        public static Youtube SelectOldestUnprocessed()
        {
            Youtube oldestUncompressedYoutube = null;
            using (var conn = new SqlConnection(ParsnipData.Parsnip.ParsnipConnectionString))
            {
                using (var selectUnprocessed = new SqlCommand("youtube_SELECT_WHERE_unprocessed", conn))
                {
                    selectUnprocessed.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (var reader = selectUnprocessed.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            oldestUncompressedYoutube = new Youtube(reader);
                        }
                    }
                }
            }
            return oldestUncompressedYoutube;
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
                            insertMedia.Parameters.AddWithValue("datetime_created", DateTimeCreated);
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
                    new LogEntry(Log.General) { Text = error };
                    return false;
                }
                return true;

            }
            else
            {
                throw new InvalidOperationException("Media cannot be inserted. The media's property: src must be initialised before it can be inserted!");
            }
        }
        protected override bool AddValues(SqlDataReader reader, int loggedInUserId)
        {
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

                if (reader[17] != DBNull.Value)
                    XScale = (short)reader[17];

                if (reader[18] != DBNull.Value)
                    YScale = (short)reader[18];

                try
                {
                    if (reader[19] != DBNull.Value && !string.IsNullOrEmpty(reader[19].ToString()))
                        SearchTerms = reader[19].ToString().Trim();

                    if (reader[20] != DBNull.Value)
                        VideoData.CompressedFileDir = reader[20].ToString().Trim();

                    if (reader[21] != DBNull.Value)
                        VideoData.OriginalFileDir = reader[21].ToString().Trim();
                }
                catch (IndexOutOfRangeException)
                {

                }

                Status = new MediaStatus(reader[22].ToString().Trim());

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("There was an error whilst reading the YoutubeVideo's values: " + ex);
                return false;
            }
        }
        #endregion


    }
}
