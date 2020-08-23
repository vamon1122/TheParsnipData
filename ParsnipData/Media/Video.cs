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
using System.Data;
using System.Web;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ParsnipData.Media
{
    public class VideoData
    {
        public string Original { get; set; }
        public string Compressed { get { return string.IsNullOrEmpty(_compressed) ? Original : _compressed; } set { _compressed = value; } }
        private string _compressed;

        private string _placeholder;
        public string Placeholder { get { if (string.IsNullOrEmpty(_placeholder)) return "Resources/Media/Images/Web_Media/placeholder.gif"; else return _placeholder; } set { _placeholder = value; } }
        public double YScale { get; set; }
        public double XScale { get; set; }
    }
    public class Video : Media
    {
        public override string Type { get { return "video"; } }
        public override string UploadsDir { get { return "Resources/Media/Videos/Thumbnails/"; } }
        public string VideoUploadsDir { get { return "Resources/Media/Videos/"; } }
        public VideoData VideoData { get; }
        private static string[] AllowedFileExtensions = new string[] { "mp4", "m4v", "mov" };
        public static bool IsValidFileExtension(string ext)
        {
            return AllowedFileExtensions.Contains(ext.ToLower());

        }

        #region Constructors
        private Video()
        {
            VideoData = new VideoData();
        }

        public Video(SqlDataReader pReader, int loggedInUserId = default) : this()
        {
            AddValues(pReader, loggedInUserId);
        }

        public Video(User uploader, HttpPostedFile videoFile, HttpPostedFile thumbnailFile)
        {
            Id = MediaId.NewMediaId();

            new LogEntry(Log.Debug) { Text = "POSTBACK with video" };
            if (thumbnailFile.FileName.Length > 0)
            {
                try
                {
                    new LogEntry(Log.Debug) { Text = "Attempting to upload the video thumbnail" };

                    string[] thumbnailFileDir = thumbnailFile.FileName.Split('\\');
                    string originalThumbnailFileName = thumbnailFileDir.Last();
                    string originalThumbnailFileExtension = "." + originalThumbnailFileName.Split('.').Last();

                    string[] videoFileDir = videoFile.FileName.Split('\\');
                    string originalVideoFileName = videoFileDir.Last();
                    string originalVideoFileExtension = "." + originalVideoFileName.Split('.').Last();

                    if (ParsnipData.Media.Image.IsValidFileExtension(originalThumbnailFileExtension.Substring(1, originalThumbnailFileExtension.Length - 1).ToLower()))
                    {
                        string generatedFileName = $"{Id.ToString()}";

                        var relativeThumbnailDir = UploadsDir + "Originals/" + generatedFileName + originalThumbnailFileExtension;
                        var fullyQualifiedThumbnailDir = HttpContext.Current.Server.MapPath("~/" + relativeThumbnailDir);
                        thumbnailFile.SaveAs(fullyQualifiedThumbnailDir);

                        var relativeVideoDir = VideoUploadsDir + "Originals/" + generatedFileName + originalVideoFileExtension;
                        var fullyQualifiedVideoDir = HttpContext.Current.Server.MapPath("~/" + relativeVideoDir);
                        videoFile.SaveAs(fullyQualifiedVideoDir);

                        Original = relativeThumbnailDir;

                        //No resize is done here but image needs to go through the process so that it displays properly 
                        //on PC's. If we use the 'original' bitmap, the image will display fine on mobile browser, fine 
                        //on Windows File Explorer, but will be rotated in desktop browsers. However, I noticed that 
                        //the thumbnail was displayed correctly at all times. So, I simply put the original image 
                        //through the same process, and called the new image 'uncompressedImage'. *NOTE* we also need 
                        //to rotate the new image (as we do with the thumbnail), as they loose their rotation 
                        //properties when they are processed using the 'ResizeBitmap' function. This is done after the 
                        //resize. MAKE SURE THAT COMPRESSED IMAGE IS SCALED EXACTLY (it is used to get scale soon)

                        ProcessMediaThumbnail(this, generatedFileName, originalThumbnailFileExtension);
                        VideoData = new VideoData();
                        VideoData.Original = relativeVideoDir;
                        VideoData.XScale = XScale;
                        VideoData.YScale = YScale;
                        CreatedById = uploader.Id;

                        DateTimeCreated = Parsnip.AdjustedTime;
                        DateTimeCaptured = DateTimeCreated;
                    }
                }
                catch (Exception err)
                {
                    new LogEntry(Log.Debug) { Text = "There was an exception whilst uploading the photo: " + err };
                }
            }
        }
        #endregion

        #region CRUD
        public override bool Insert()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (var insertVideo = new SqlCommand("video_INSERT", conn))
                    {
                        insertVideo.CommandType = CommandType.StoredProcedure;

                        insertVideo.Parameters.AddWithValue("videoTitle", Title);
                        insertVideo.Parameters.AddWithValue("videoOriginalDir", VideoData.Original);
                        insertVideo.Parameters.AddWithValue("thumbnailOriginalDir", Original);
                        insertVideo.Parameters.AddWithValue("thumbnailCompressedDir", Compressed);
                        insertVideo.Parameters.AddWithValue("thumbnailPlaceholderDir", Placeholder);
                        insertVideo.Parameters.AddWithValue("dateTimeCaptured", DateTimeCaptured);
                        insertVideo.Parameters.AddWithValue("media_x_scale", XScale);
                        insertVideo.Parameters.AddWithValue("media_y_scale", YScale);
                        if(AlbumId != default)
                            insertVideo.Parameters.AddWithValue("mediaTagId", AlbumId);
                        insertVideo.Parameters.AddWithValue("createdByUserId", CreatedById);
                        insertVideo.Parameters.AddWithValue("newMediaId", Id.ToString());
                        insertVideo.Parameters.AddWithValue("now", Parsnip.AdjustedTime);

                        conn.Open();
                        insertVideo.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"There was an exception whilst inserting the video: {ex}");
                return false;
            }
        }
        public new static Video Select(MediaId mediaId, int loggedInUserId)
        {
            Video video = null;
            try
            {
                using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (SqlCommand selectVideo = new SqlCommand("video_SELECT_WHERE_media_id", conn))
                    {
                        selectVideo.CommandType = CommandType.StoredProcedure;

                        selectVideo.Parameters.Add(new SqlParameter("media_id", mediaId.ToString()));
                        selectVideo.Parameters.Add(new SqlParameter("logged_in_user_id", loggedInUserId));

                        conn.Open();
                        using (SqlDataReader reader = selectVideo.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                video = new Video();
                                video.AddValues(reader, loggedInUserId);
                            }

                            if (video != null)
                            {
                                reader.NextResult();

                                video.MediaTagPairs = new List<MediaTagPair>();
                                while (reader.Read())
                                {
                                    var mediaTag = new MediaTagPair(reader);
                                    video.MediaTagPairs.Add(mediaTag);
                                }

                                reader.NextResult();

                                video.MediaUserPairs = new List<MediaUserPair>();
                                while (reader.Read())
                                {
                                    var mediaUserPair = new MediaUserPair(reader);
                                    video.MediaUserPairs.Add(mediaUserPair);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"There was an exception whilst getting video data: {ex}");
            }
            return video;
        }

        public static Video SelectOldestUncompressed()
        {
            var oldestUncompressedVideo = new Video();
            using(var conn = new SqlConnection(ParsnipData.Parsnip.ParsnipConnectionString))
            {
                using(var selectUncompressed = new SqlCommand("video_SELECT_WHERE_uncompressed", conn))
                {
                    selectUncompressed.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using(var reader = selectUncompressed.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            oldestUncompressedVideo = new Video(reader);
                        }
                    }
                }
            }
            return oldestUncompressedVideo;
        }

        public bool UpdateDirectories()
        {
            if (Id != default)
            {

                try
                {
                    using (var conn = new SqlConnection(ParsnipData.Parsnip.ParsnipConnectionString))
                    {


                        using (var updateMedia = new SqlCommand("video_UPDATE_directories", conn))
                        {
                            updateMedia.CommandType = CommandType.StoredProcedure;

                            updateMedia.Parameters.AddWithValue("media_id", Id.ToString());
                            updateMedia.Parameters.AddWithValue("compressed_dir", VideoData.Compressed);
                            updateMedia.Parameters.AddWithValue("original_dir", VideoData.Original);


                            conn.Open();
                            updateMedia.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    string error =
                        string.Format("There was an error whilst updating media: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.General) { Text = error };
                    return false;
                }
                new LogEntry(Log.Debug) { Text = string.Format("Media was successfully updated on the database!") };
                return true;
            }
            else
            {
                throw new System.InvalidOperationException(
                    "Media cannot be updated. Media must be inserted into the database before it can be updated!");
            }

        }
        protected override bool AddValues(SqlDataReader reader, int loggedInUserId = default)
        {
            try
            {
                Id = new MediaId(reader[0].ToString());

                if (reader[1] != DBNull.Value && !string.IsNullOrEmpty(reader[1].ToString()) && !string.IsNullOrWhiteSpace(reader[1].ToString()))
                {
                    Title = reader[1].ToString().Trim();
                }

                if (reader[2] != DBNull.Value && !string.IsNullOrEmpty(reader[2].ToString()) && !string.IsNullOrWhiteSpace(reader[2].ToString()))
                    Description = reader[2].ToString().Trim();

                if (reader[3] != DBNull.Value && !string.IsNullOrEmpty(reader[3].ToString()) && !string.IsNullOrWhiteSpace(reader[3].ToString()))
                    Alt = reader[3].ToString().Trim();

                DateTimeCaptured = Convert.ToDateTime(reader[4]);
                DateTimeCreated = Convert.ToDateTime(reader[5]);

                if (reader[6] != DBNull.Value && !string.IsNullOrEmpty(reader[6].ToString()) && !string.IsNullOrWhiteSpace(reader[6].ToString()))
                    VideoData.XScale = (double)reader[6];

                if (reader[7] != DBNull.Value && !string.IsNullOrEmpty(reader[7].ToString()) && !string.IsNullOrWhiteSpace(reader[7].ToString()))
                    VideoData.YScale = (double)reader[7];

                if (reader[8] != DBNull.Value && !string.IsNullOrEmpty(reader[8].ToString()) && !string.IsNullOrWhiteSpace(reader[8].ToString()))
                    VideoData.Original = reader[8].ToString().Trim();

                if (reader[9] != DBNull.Value && !string.IsNullOrEmpty(reader[9].ToString()) && !string.IsNullOrWhiteSpace(reader[9].ToString()))
                    VideoData.Compressed = reader[9].ToString().Trim();


                if (reader[10] != DBNull.Value && !string.IsNullOrEmpty(reader[10].ToString()) && !string.IsNullOrWhiteSpace(reader[10].ToString()))
                    XScale = (double)reader[10];

                if (reader[11] != DBNull.Value && !string.IsNullOrEmpty(reader[11].ToString()) && !string.IsNullOrWhiteSpace(reader[11].ToString()))
                    YScale = (double)reader[11];

                if (reader[12] != DBNull.Value && !string.IsNullOrEmpty(reader[12].ToString()) && !string.IsNullOrWhiteSpace(reader[12].ToString()))
                    Original = reader[12].ToString().Trim();

                if (reader[13] != DBNull.Value && !string.IsNullOrEmpty(reader[13].ToString()) && !string.IsNullOrWhiteSpace(reader[13].ToString()))
                    Compressed = reader[13].ToString().Trim();

                if (reader[14] != DBNull.Value && !string.IsNullOrEmpty(reader[14].ToString()) && !string.IsNullOrWhiteSpace(reader[14].ToString()))
                    Placeholder = reader[14].ToString().Trim();

                CreatedById = (int)reader[15];

                try
                {
                    if (reader[17] != DBNull.Value && !string.IsNullOrEmpty(reader[17].ToString()) && !string.IsNullOrWhiteSpace(reader[17].ToString()))
                        AlbumId = (int)reader[17];
                }
                catch (IndexOutOfRangeException)
                {
                    //These values are not necessarily returned in all queries
                }

                try
                {
                    if (reader[19] == DBNull.Value && string.IsNullOrEmpty(reader[19].ToString()) &&
                        string.IsNullOrWhiteSpace(reader[19].ToString()))

                    {
                        if (loggedInUserId != default)
                        {
                            MyMediaShare = new MediaShare(Id, loggedInUserId);
                            MyMediaShare.Insert();
                        }
                    }
                    else
                    {
                        MyMediaShare = new MediaShare(new MediaShareId((string)reader[19]), (int)reader[20], Convert.ToDateTime(reader[21]), default, new MediaId((string)reader[0]));
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    //These values are not necessarily returned in all queries
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("There was an error whilst reading the Video's values: " + ex);
                return false;
            }
        }
        #endregion
    }
}
