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
    public class Video : Media
    {
        private static string[] _allowedFileExtensions = new string[] { "mp4", "m4v" };
        public override string[] AllowedFileExtensions { get { return _allowedFileExtensions; } }
        static readonly Log DebugLog = new Log("Debug");
        public string Thumbnail { get; set; }

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
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();
                    SqlCommand GetVideos = new SqlCommand("SELECT TOP 1 * FROM video ORDER BY date_time_created DESC", conn);
                    using (SqlDataReader reader = GetVideos.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            video.AddValues(reader);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
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

        }

        public Video(string directory, User createdBy, Album album)
        {
            Id = Guid.NewGuid();
            Directory = directory;
            Log DebugLog = new Log("Debug");
            new LogEntry(DebugLog) { text = "Video created with album_id = " + album.Id };
            AlbumId = album.Id;
            DateCreated = Parsnip.AdjustedTime;
            CreatedById = createdBy.Id;
        }

        public Video(Guid guid)
        {
            //Debug.WriteLine("Video was initialised with the guid: " + pGuid);
            Id = guid;
        }

        public Video(SqlDataReader reader)
        {
            //Debug.WriteLine("Video was initialised with an SqlDataReader. Guid: " + pReader[0]);
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
                        Debug.WriteLine("----------Reading title");

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

                if (logMe)
                    Debug.WriteLine("----------Reading Directory");
                Directory = reader[3].ToString().Trim();

                if (reader[4] != DBNull.Value && !string.IsNullOrEmpty(reader[4].ToString()) && !string.IsNullOrWhiteSpace(reader[4].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading thumbnail");

                    Thumbnail = reader[4].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Thumbnail is blank. Skipping thumbnail");
                }

                if (logMe)
                    Debug.WriteLine("----------Reading datecreated");
                DateCreated = Convert.ToDateTime(reader[5]);

                if (logMe)
                    Debug.WriteLine("----------Reading createdbyid");
                CreatedById = new Guid(reader[6].ToString());

                if (logMe)
                    Debug.WriteLine("----------Reading ImageSrc");
                DateTimeMediaCreated = Convert.ToDateTime(reader[8]);

                try
                {
                    if (reader[9] != DBNull.Value && !string.IsNullOrEmpty(reader[9].ToString()) && !string.IsNullOrWhiteSpace(reader[9].ToString()))
                    {
                        if (logMe)
                            Debug.WriteLine("----------Reading album id");

                        AlbumId = new Guid(reader[9].ToString());
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
                        SqlCommand InsertVideoIntoDb = new SqlCommand("INSERT INTO t_Videos (video_id, directory, date_time_created, date_time_media_created, created_by_user_id) VALUES(@video_id, @directory, @date_time_created, @date_time_media_created, @created_by_user_id)", pOpenConn);

                        InsertVideoIntoDb.Parameters.Add(new SqlParameter("video_id", Id));
                        InsertVideoIntoDb.Parameters.Add(new SqlParameter("directory", Directory.Trim()));
                        InsertVideoIntoDb.Parameters.Add(new SqlParameter("date_time_created", Parsnip.AdjustedTime));
                        InsertVideoIntoDb.Parameters.Add(new SqlParameter("date_time_media_created", Parsnip.AdjustedTime));
                        InsertVideoIntoDb.Parameters.Add(new SqlParameter("created_by_user_id", CreatedById));

                        InsertVideoIntoDb.ExecuteNonQuery();

                        SqlCommand InsertVideoAlbumPairIntoDb = new SqlCommand("INSERT INTO media_tag_pairs VALUES(@media_id, @album_id)", pOpenConn);
                        InsertVideoAlbumPairIntoDb.Parameters.Add(new SqlParameter("media_id", Id));
                        InsertVideoAlbumPairIntoDb.Parameters.Add(new SqlParameter("album_id", AlbumId));

                        InsertVideoAlbumPairIntoDb.ExecuteNonQuery();

                        Debug.WriteLine(String.Format("Successfully inserted video into database ({0}) ", Id));
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
                SqlCommand SelectAccount = new SqlCommand("SELECT video.*, media_tag_pair.media_tag_id FROM video LEFT JOIN media_tag_pair ON media_tag_pair.media_id = video.video_id INNER JOIN [user] ON [user].user_id = video.created_by_user_id WHERE video_id = @video_id AND video.deleted IS NULL AND [user].deleted IS NULL", pOpenConn);
                SelectAccount.Parameters.Add(new SqlParameter("video_id", Id.ToString()));

                int recordsFound = 0;
                using (SqlDataReader reader = SelectAccount.ExecuteReader())
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
                    Video temp = new Video(Id);
                    temp.Select();

                    if (AlbumId != null)
                    {
                        Log DebugLog = new Log("Debug");
                        new LogEntry(DebugLog) { text = "AlbumId != null = " + AlbumId };

                        SqlCommand DeleteOldPairs = 
                            new SqlCommand("DELETE FROM media_tag_pair WHERE media_id = @video_id", pOpenConn);

                        DeleteOldPairs.Parameters.Add(new SqlParameter("video_id", Id));
                        DeleteOldPairs.ExecuteNonQuery();

                        if (AlbumId.ToString() != Guid.Empty.ToString())
                        {
                            SqlCommand CreatePhotoAlbumPair =
                                new SqlCommand("INSERT INTO media_tag_pair VALUES (@media_id, @media_tag_id)", pOpenConn);

                            CreatePhotoAlbumPair.Parameters.Add(new SqlParameter("media_id", Id));
                            CreatePhotoAlbumPair.Parameters.Add(new SqlParameter("media_tag_id", AlbumId));

                            Debug.WriteLine("---------- Video album pair (INSERT) = " + AlbumId);

                            CreatePhotoAlbumPair.ExecuteNonQuery();
                            new LogEntry(DebugLog) { text = string.Format("INSERTED ALBUM PAIR {0}, {1} ", Id, AlbumId) };
                        }
                    }
                    else
                    {
                        Log DebugLog = new Log("Debug");
                        new LogEntry(DebugLog) { text = "Video created with album_id = null :( " };
                    }


                    if (Thumbnail != temp.Thumbnail)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update placeholder..."));


                        SqlCommand UpdatePlaceholder = new SqlCommand("UPDATE video " +
                            "SET thumbnail_directory = @thumbnail_directory WHERE video_id = @video_id", pOpenConn);

                        UpdatePlaceholder.Parameters.Add(new SqlParameter("video_id", Id));
                        UpdatePlaceholder.Parameters.Add(new SqlParameter("thumbnail_directory", Thumbnail.Trim()));

                        UpdatePlaceholder.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------placeholder was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------placeholder was not changed. Not updating placeholder."));
                    }

                    /*
                    if (Alt != temp.Alt)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update alt..."));


                        SqlCommand UpdateAlt = new SqlCommand("UPDATE t_Videos SET alt = @alt WHERE id = @id", pOpenConn);
                        UpdateAlt.Parameters.Add(new SqlParameter("id", Id));
                        UpdateAlt.Parameters.Add(new SqlParameter("alt", Alt.Trim()));

                        UpdateAlt.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------alt was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------alt was not changed. Not updating alt."));
                    }
                    */

                    if (Title != temp.Title)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update title..."));


                        SqlCommand UpdateTitle = 
                            new SqlCommand("UPDATE video SET title = @title WHERE video_id = @video_id", pOpenConn);

                        UpdateTitle.Parameters.Add(new SqlParameter("video_id", Id));
                        UpdateTitle.Parameters.Add(new SqlParameter("title", Title.Trim()));

                        UpdateTitle.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------Title was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Title was not changed. Not updating title."));
                    }

                    if (Description != temp.Description)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update description..."));


                        SqlCommand UpdateDescription = new SqlCommand(
                            "UPDATE video SET description = @description WHERE video_id = @video_id", pOpenConn);
                        UpdateDescription.Parameters.Add(new SqlParameter("video_id", Id));
                        UpdateDescription.Parameters.Add(new SqlParameter("description", Description.Trim()));

                        UpdateDescription.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------Description was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Description was not changed. Not updating description."));
                    }

                    if (DateTimeMediaCreated != temp.DateTimeMediaCreated)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update date time media created..."));


                        SqlCommand UpdateDateTimeMediaCreated = new SqlCommand(
                            "UPDATE video SET date_time_media_created = @date_time_media_created WHERE video_id = @video_id", pOpenConn);
                        UpdateDateTimeMediaCreated.Parameters.Add(new SqlParameter("video_id", Id));
                        UpdateDateTimeMediaCreated.Parameters.Add(new SqlParameter("date_time_media_created", DateTimeMediaCreated));

                        UpdateDateTimeMediaCreated.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------Date time media created was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Date time media created was not changed. Not updating DateTimeMediaCreated."));
                    }

                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.Video.DbUpdate] There was an error whilst updating video: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("Video was successfully updated on the database!") };
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
