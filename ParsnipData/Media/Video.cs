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
using System.Data;

namespace ParsnipData.Media
{
    public class VideoData
    {
        public string Original { get; set; }
        public string Compressed { get; set; }
        private string _placeholder;
        public string Placeholder { get { if (string.IsNullOrEmpty(_placeholder)) return "Resources/Media/Images/Web_Media/placeholder.gif"; else return _placeholder; } set { _placeholder = value; } }
        public double YScale { get; set; }
        public double XScale { get; set; }
    }
    public class Video : Media
    {
        public override string Type { get { return "video"; } }
        public override string UploadsDir { get { return "Resources/Media/Videos/"; } }
        public VideoData VideoData { get; }
        public override string[] AllowedFileExtensions { get { return new string[] { "mp4", "m4v" }; } }

        #region Constructors
        private Video()
        {
            VideoData = new VideoData();
        }
        #endregion

        #region CRUD
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
        public override bool Delete()
        {
            try
            {
                new LogEntry(Log.Debug) { text = "Attempting to delete uploaded video id = " + Id };

                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (SqlCommand deleteVideo = new SqlCommand("video_delete", conn))
                    {
                        deleteVideo.Parameters.Add(new SqlParameter("video_id", Id));
                        conn.Open();
                        int recordsAffected = deleteVideo.ExecuteNonQuery();
                        new LogEntry(Log.Debug) { text = string.Format("{0} record(s) were affected", recordsAffected) };
                    }
                }
            }
            catch (Exception err)
            {

                new LogEntry(Log.Debug) { text = "There was an exception whilst DELETING the video: " + err };
                return false;
            }
            new LogEntry(Log.Debug) { text = "Successfully deleted video with id = " + Id };
            return true;
        }
        protected override bool AddValues(SqlDataReader reader, int loggedInUserId)
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
                        if (loggedInUserId.ToString() != default)
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
