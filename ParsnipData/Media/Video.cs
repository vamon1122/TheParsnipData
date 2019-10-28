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
    
    public class Video : Media
    {
        public class VideoThumbnail
        {
            public string Original { get; set; }

            public string Compressed { get; set; }

            private string _placeholder;
            public string Placeholder { get { if (string.IsNullOrEmpty(_placeholder)) return "Resources/Media/Images/Web_Media/placeholder.gif"; else return _placeholder; } set { _placeholder = value; } }

            public double YScale { get; set; }

            public double XScale { get; set; }
        }

        public VideoThumbnail Thumbnail { get; }

        private static string[] _allowedFileExtensions = new string[] { "mp4", "m4v" };
        public override string[] AllowedFileExtensions { get { return _allowedFileExtensions; } }
        static readonly Log DebugLog = new Log("Debug");

        public static bool IsValidFileExtension(string extension)
        {
            return _allowedFileExtensions.Contains(extension);
        }

        public static bool Exists(Guid videoId)
        {
            using(SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                return IdExistsOnDb(conn, videoId);
            }
            
        }

        public static void DeleteMediaTagPairsByUserId(Guid userId)
        {
            try
            {
                new LogEntry(DebugLog) { text = "Attempting to delete uploaded photos created_by_user_id = " + userId };

                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();

                    SqlCommand DeleteUploads = new SqlCommand("DELETE media_tag_pair FROM media_tag_pair INNER JOIN video ON media_tag_pair.media_id = video.video_id  WHERE video.created_by_user_id = @created_by_user_id", conn);
                    DeleteUploads.Parameters.Add(new SqlParameter("created_by_user_id", userId));
                    int recordsAffected = DeleteUploads.ExecuteNonQuery();

                    new LogEntry(DebugLog) { text = string.Format("{0} record(s) were affected", recordsAffected) };
                }
            }
            catch (Exception err)
            {

                new LogEntry(DebugLog) { text = "There was an exception whilst DELETING the photo: " + err };
            }
        }

        public override List<Guid> AlbumIds()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all video's album Ids...");

            var albumIds = new List<Guid>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                SqlCommand GetVideos = new SqlCommand("SELECT media_tag_id FROM media_tag_pair WHERE media_id = @video_id", conn);
                GetVideos.Parameters.Add(new SqlParameter("video_id", Id));

                using (SqlDataReader reader = GetVideos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader[0].ToString() != Guid.Empty.ToString())
                        {
                            new LogEntry(DebugLog) { text = "An album guid was found! = " + reader[0].ToString() };
                            albumIds.Add(new Guid(reader[0].ToString()));
                        }
                        else
                        {
                            new LogEntry(DebugLog) { text = "A BLANK album guid was found! Not adding Guid = " + reader[0].ToString() };
                        }

                    }
                }
            }

            return albumIds;
        }

        public static Video GetLatest()
        {
            Video video = new Video();
            /*try
            {*/
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();
                    SqlCommand GetVideos = new SqlCommand("SELECT TOP 1 video.*, NULL , access_token.* FROM video LEFT JOIN access_token ON access_token.media_id = video.video_id AND access_token.created_by_user_id = @logged_in_user_id ORDER BY video.date_time_media_created DESC", conn);
                    GetVideos.Parameters.Add(new SqlParameter("logged_in_user_id", User.GetLoggedInUserId()));
                    using (SqlDataReader reader = GetVideos.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            video.AddValues(reader);
                        }
                    }
                }
            /*}
            catch(Exception ex)
            {
                throw ex;
            }*/
            
            return video;
        }

        public static List<Video> GetAllVideos()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all videos...");

            var videos = new List<Video>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                SqlCommand GetVideos = new SqlCommand("SELECT * FROM video ORDER BY date_time_created DESC", conn);
                using (SqlDataReader reader = GetVideos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        videos.Add(new Video(reader));
                    }
                }
            }

            foreach (Video temp in videos)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found video with id {0}", temp.Id));
            }

            return videos;
        }

        public static List<Video> GetVideosByUser(Guid userId)
        {
            bool logMe = true;

            if (logMe)
                Debug.WriteLine("----------Getting all videos by user...");

            var videos = new List<Video>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                Debug.WriteLine("---------- Selecting videos by user with id = " + userId);
                SqlCommand GetVideos = new SqlCommand("SELECT * FROM video WHERE created_by_user_id = @created_by_user_id ORDER BY date_time_created DESC", conn);
                GetVideos.Parameters.Add(new SqlParameter("created_by_user_id", userId));

                using (SqlDataReader reader = GetVideos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        videos.Add(new Video(reader));
                    }
                }
            }

            foreach (Video temp in videos)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found video with id {0}", temp.Id));
            }

            return videos;
        }

        public static List<Video> GetVideosNotInAnAlbum()
        {
            throw new NotImplementedException();
        }

        private Video()
        {
            Thumbnail = new VideoThumbnail();
        }

        public Video(string directory, User createdBy, Album album) : this()
        {
            Id = Guid.NewGuid();
            Directory = directory;
            Log DebugLog = new Log("Debug");
            new LogEntry(DebugLog) { text = "Video created with album_id = " + album.Id };
            AlbumId = album.Id;
            DateTimeCreated = Parsnip.AdjustedTime;
            CreatedById = createdBy.Id;
        }

        public Video(Guid guid) : this()
        {
            //Debug.WriteLine("Video was initialised with the guid: " + pGuid);
            Id = guid;
        }

        public Video(SqlDataReader reader)
        {
            //Debug.WriteLine("Video was initialised with an SqlDataReader. Guid: " + pReader[0]);
            Thumbnail = new VideoThumbnail();
            AddValues(reader);
        }

        public bool ExistsOnDb()
        {
            using(var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                return ExistsOnDb(conn);
            }
            
        }

        private bool ExistsOnDb(SqlConnection pOpenConn)
        {
            if (IdExistsOnDb(pOpenConn))
                return true;
            else
                return false;
        }

        private static bool IdExistsOnDb(SqlConnection pOpenConn, Guid id)
        {
            Debug.WriteLine(string.Format("Checking weather video exists on database by using Id {0}", id));
            try
            {
                SqlCommand findMeById = new SqlCommand("SELECT COUNT(*) FROM video WHERE video_id = @id", pOpenConn);
                findMeById.Parameters.Add(new SqlParameter("id", id.ToString()));

                int videoExists;

                using (SqlDataReader reader = findMeById.ExecuteReader())
                {
                    reader.Read();
                    videoExists = Convert.ToInt16(reader[0]);
                    //Debug.WriteLine("Found video by Id. videoExists = " + videoExists);
                }

                //Debug.WriteLine(videoExists + " video(s) were found with the id " + Id);

                if (videoExists > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst checking if video exists on the database by using it's Id: " + e);
                return false;
            }
        }

        private bool IdExistsOnDb(SqlConnection pOpenConn)
        {
            return IdExistsOnDb(pOpenConn, Id);
        }

        internal bool AddValues(SqlDataReader reader)
        {
            bool logMe = true;

            if (logMe)
                Debug.WriteLine("----------Adding values...");

            try
            {
                if (logMe)
                    Debug.WriteLine(string.Format("----------Reading id: {0}", reader[0]));

                Id = new Guid(reader[0].ToString());

                if (reader[1] != DBNull.Value && !string.IsNullOrEmpty(reader[1].ToString()) && !string.IsNullOrWhiteSpace(reader[1].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading video title: " + reader[1].ToString().Trim());

                    Title = reader[1].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Title is blank. Skipping title");
                }

                if (reader[2] != DBNull.Value && !string.IsNullOrEmpty(reader[2].ToString()) && !string.IsNullOrWhiteSpace(reader[2].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading description");

                    Description = reader[2].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Description is blank. Skipping description");
                }

                if (reader[3] != DBNull.Value && !string.IsNullOrEmpty(reader[3].ToString()) && !string.IsNullOrWhiteSpace(reader[3].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading alt");

                    Description = reader[3].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Alt is blank. Skipping alt");
                }

                if (logMe)
                    Debug.WriteLine("----------Reading DateTimeMediaCreated");
                DateTimeMediaCreated = Convert.ToDateTime(reader[4]);

                if (logMe)
                    Debug.WriteLine("----------Reading datecreated");
                DateTimeCreated = Convert.ToDateTime(reader[5]);

                if (reader[6] != DBNull.Value && !string.IsNullOrEmpty(reader[6].ToString()) && !string.IsNullOrWhiteSpace(reader[6].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading width");

                    XScale = (double)reader[6];
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Width is blank. Skipping width");
                }

                if (reader[7] != DBNull.Value && !string.IsNullOrEmpty(reader[7].ToString()) && !string.IsNullOrWhiteSpace(reader[7].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading height");

                    YScale = (double)reader[7];
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Height is blank. Skipping height");
                }

                if (logMe)
                    Debug.WriteLine("----------Reading Directory");
                Directory = reader[9].ToString().Trim();


                if (reader[10] != DBNull.Value && !string.IsNullOrEmpty(reader[10].ToString()) && !string.IsNullOrWhiteSpace(reader[10].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading thumbnail width");

                    Thumbnail.XScale = (double)reader[10];
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Thumbnail width is blank. Skipping thumbnail width");
                }

                if (reader[11] != DBNull.Value && !string.IsNullOrEmpty(reader[11].ToString()) && !string.IsNullOrWhiteSpace(reader[11].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading thumbnail height");

                    Thumbnail.YScale = (double)reader[11];
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Thumbnail height is blank. Skipping thumbnail height");
                }

                if (reader[12] != DBNull.Value && !string.IsNullOrEmpty(reader[12].ToString()) && !string.IsNullOrWhiteSpace(reader[12].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading original thumbnail");

                    Thumbnail.Original = reader[12].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Original thumbnail is blank. Skipping original thumbnail");
                }

                if (reader[13] != DBNull.Value && !string.IsNullOrEmpty(reader[13].ToString()) && !string.IsNullOrWhiteSpace(reader[13].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading compressed thumbnail");
                    Thumbnail.Compressed = reader[13].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Compressed thumbnail is blank. Skipping compressed thumbnail");
                }

                if (reader[14] != DBNull.Value && !string.IsNullOrEmpty(reader[14].ToString()) && !string.IsNullOrWhiteSpace(reader[14].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading placeholder thumbnail");

                    Thumbnail.Placeholder = reader[14].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Placeholder thumbnail is blank. Skipping placeholder thumbnail");
                }



                if (logMe)
                    Debug.WriteLine("----------Reading createdbyid");
                CreatedById = new Guid(reader[15].ToString());

                

                try
                {
                    if (reader[17] != DBNull.Value && !string.IsNullOrEmpty(reader[17].ToString()) && !string.IsNullOrWhiteSpace(reader[17].ToString()))
                    {
                        if (logMe)
                            Debug.WriteLine("----------Reading album id");

                        AlbumId = new Guid(reader[16].ToString());
                    }
                    else
                    {
                        if (logMe)
                            Debug.WriteLine("----------Album id is blank. Skipping album id");
                    }
                }
                catch(IndexOutOfRangeException)
                {
                    if (logMe)
                        Debug.WriteLine("----------Album id was not requested in the query. Skipping album id");
                }

                try
                {
                    if (reader[18] != DBNull.Value && !string.IsNullOrEmpty(reader[18].ToString()) &&
                        !string.IsNullOrWhiteSpace(reader[18].ToString()))

                    {
                        if (logMe)
                            Debug.WriteLine("----------Reading access_token id");

                        MyAccessToken = new AccessToken((Guid)reader[18], (Guid)reader[19], Convert.ToDateTime(reader[20]), (int)reader[21], (Guid)reader[22]);
                    }
                    else
                    {
                        if (logMe)
                            Debug.WriteLine("----------Access_token id is blank. Creating new access token");

                        Guid loggedInUserId = ParsnipData.Accounts.User.GetLoggedInUserId();
                        if (loggedInUserId.ToString() != Guid.Empty.ToString())
                        {
                            MyAccessToken = new AccessToken(loggedInUserId, Id);
                            MyAccessToken.Insert();
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    if (logMe)
                        Debug.WriteLine("----------Access_token was not in the query");
                }


                //AlbumId = new Guid(reader[8].ToString());

                if (logMe)
                    Debug.WriteLine("added values successfully!");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("There was an error whilst reading the Video's values: " + ex);
                return false;
            }
        }

        private bool DbInsert(SqlConnection pOpenConn)
        {
            if (Id.ToString() == Guid.Empty.ToString())
            {
                Id = Guid.NewGuid();
                Debug.WriteLine("Id was empty when trying to insert video into the database. A new guid was generated: {0}", Id);
            }

            if (Directory != null)
            {
                try
                {
                    if (!ExistsOnDb(pOpenConn))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insert_video", pOpenConn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("@video_id", SqlDbType.UniqueIdentifier).Value = Id;
                            cmd.Parameters.Add("@date_time_media_created", SqlDbType.DateTime).Value = DateTimeMediaCreated;
                            cmd.Parameters.Add("@date_time_created", SqlDbType.DateTime).Value = DateTimeCreated;
                            cmd.Parameters.Add("@x_scale", SqlDbType.Int).Value = XScale;
                            cmd.Parameters.Add("@y_scale", SqlDbType.Char).Value = YScale;
                            cmd.Parameters.Add("@video_dir", SqlDbType.Char).Value = Directory;
                            cmd.Parameters.Add("@thumbnail_x_scale", SqlDbType.Int).Value = Thumbnail.XScale;
                            cmd.Parameters.Add("@thumbnail_y_scale", SqlDbType.Char).Value = Thumbnail.YScale;
                            cmd.Parameters.Add("@thumbnail_original_dir", SqlDbType.Char).Value = Thumbnail.Original;
                            cmd.Parameters.Add("@thumbnail_compressed_dir", SqlDbType.Char).Value = Thumbnail.Compressed;
                            cmd.Parameters.Add("@thumbnail_placeholder_dir", SqlDbType.Char).Value = Thumbnail.Placeholder;
                            cmd.Parameters.Add("@created_by_user_id", SqlDbType.UniqueIdentifier).Value = CreatedById;
                            cmd.Parameters.Add("@media_tag_id", SqlDbType.UniqueIdentifier).Value = AlbumId;

                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Tried to insert video into the database but it alread existed! Id = {0}", Id));
                    }
                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.Video.DbInsert)] Failed to insert video into the database: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("Video was successfully inserted into the database!") };
                return DbUpdate(pOpenConn);
            }
            else
            {
                throw new InvalidOperationException("Video cannot be inserted. The video's property: videosrc must be initialised before it can be inserted!");
            }
        }

        public bool Select()
        {
            using(var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                return DbSelect(conn);
            }
            
        }

        internal bool DbSelect(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get video details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get video details with id {0}...", Id));

            try
            {
                SqlCommand SelectVideo = new SqlCommand("SELECT video.*, media_tag_pair.media_tag_id FROM video LEFT JOIN media_tag_pair ON media_tag_pair.media_id = video.video_id INNER JOIN [user] ON [user].user_id = video.created_by_user_id LEFT JOIN access_token ON access_token.media_id = video.video_id AND access_token.created_by_user_id = @logged_in_user_id WHERE video_id = @video_id AND video.deleted IS NULL AND [user].deleted IS NULL", pOpenConn);
                SelectVideo.Parameters.Add(new SqlParameter("video_id", Id.ToString()));
                SelectVideo.Parameters.Add(new SqlParameter("logged_in_user_id", User.GetLoggedInUserId()));

                int recordsFound = 0;
                using (SqlDataReader reader = SelectVideo.ExecuteReader())
                {

                    while (reader.Read())
                    {

                        AddValues(reader);
                        recordsFound++;
                    }
                }
                //Debug.WriteLine(string.Format("----------DbSelect() - Found {0} record(s) ", recordsFound));

                if (recordsFound > 0)
                {
                    //Debug.WriteLine("----------DbSelect() - Got video's details successfully!");
                    //AccountLog.Debug("Got video's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbSelect() - No video data was returned");
                    //AccountLog.Debug("Got video's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst getting video data: " + e);
                return false;
            }
        }

        public override bool Update()
        {
            bool success;
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                success = ExistsOnDb(conn) ?  DbUpdate(conn) : DbInsert(conn);
            }
                
            return success;
        }

        private bool DbUpdate(SqlConnection pOpenConn)
        {
            Debug.WriteLine("Attempting to update video with Id = " + Id);
            bool HasBeenInserted = true;

            if (HasBeenInserted)
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_update_video", pOpenConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@video_id", SqlDbType.UniqueIdentifier).Value = Id;
                        cmd.Parameters.Add("@title", SqlDbType.NChar).Value = Title;
                        cmd.Parameters.Add("@description", SqlDbType.NChar).Value = Description;
                        cmd.Parameters.Add("@alt", SqlDbType.Char).Value = Alt;
                        cmd.Parameters.Add("@date_time_media_created", SqlDbType.DateTime).Value = DateTimeMediaCreated;
                        cmd.Parameters.Add("@media_tag_id", SqlDbType.UniqueIdentifier).Value = AlbumId;

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.Video.DbUpdate] There was an error whilst updating video: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(DebugLog) { text = string.Format("Video was successfully updated on the database!") };
                return true;
            }
            else
            {
                throw new System.InvalidOperationException(
                    "Account cannot be updated. Account must be inserted into the database before it can be updated!");
            }
        }

        public override bool Delete()
        {
            using(var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();

                return DbDelete(conn);
            }
        }

        internal bool DbDelete(SqlConnection pOpenConn)
        {
            try
            {
                new LogEntry(DebugLog) { text = "Attempting to delete uploaded video id = " + Id };

                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();

                    SqlCommand DeleteVideo = new SqlCommand("UPDATE video SET deleted = @date_time_now WHERE video_id = @video_id", conn);
                    DeleteVideo.Parameters.Add(new SqlParameter("date_time_now", Parsnip.AdjustedTime));
                    DeleteVideo.Parameters.Add(new SqlParameter("video_id", Id));
                    int recordsAffected = DeleteVideo.ExecuteNonQuery();

                    new LogEntry(DebugLog) { text = string.Format("{0} record(s) were affected", recordsAffected) };
                }
            }
            catch (Exception err)
            {

                new LogEntry(DebugLog) { text = "There was an exception whilst DELETING the video: " + err };
                return false;
            }
            new LogEntry(DebugLog) { text = "Successfully deleted video with id = " + Id };
            return true;
        }
    }
}
