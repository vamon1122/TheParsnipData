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
using System.Web;
using System.Net;

namespace ParsnipData.Media
{
    public class YoutubeVideo : Media
    {
        private static string[] _allowedFileExtensions = new string[0];
        public override string[] AllowedFileExtensions { get { return _allowedFileExtensions; } }
        static readonly Log DebugLog = new Log("Debug");


        public string DataId { get; set; }

        public static bool IsValidFileExtension(string extension)
        {
            return _allowedFileExtensions.Contains(extension);
        }

        public List<Guid> AlbumIds()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all youtubeVideo's album Ids...");

            var albumIds = new List<Guid>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                SqlCommand GetYoutubeVideos = new SqlCommand("SELECT album_id FROM media_tag_pair WHERE image_id = @youtube_video_id", conn);
                GetYoutubeVideos.Parameters.Add(new SqlParameter("youtube_video_id", Id));

                using (SqlDataReader reader = GetYoutubeVideos.ExecuteReader())
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

        public static YoutubeVideo GetLatest()
        {
            YoutubeVideo youtubeVideo = new YoutubeVideo();
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();
                    SqlCommand GetYoutubeVideos = new SqlCommand("SELECT TOP 1 * FROM youtube_video ORDER BY date_time_created DESC", conn);
                    using (SqlDataReader reader = GetYoutubeVideos.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            youtubeVideo.AddValues(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return youtubeVideo;
        }

        public static List<YoutubeVideo> GetAllYoutubeVideos()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all youtube videos...");

            var youtubeVideos = new List<YoutubeVideo>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();

                SqlCommand GetYoutubeVideos = new SqlCommand("SELECT * FROM youtube_video ORDER BY date_time_created DESC", conn);
                using (SqlDataReader reader = GetYoutubeVideos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        youtubeVideos.Add(new YoutubeVideo(reader));
                    }
                }
            }

            foreach (YoutubeVideo temp in youtubeVideos)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found youtubeVideo with id {0}", temp.Id));
            }

            return youtubeVideos;
        }

        public static List<YoutubeVideo> GetYoutubeVideosByUser(Guid userId)
        {
            bool logMe = true;

            if (logMe)
                Debug.WriteLine("----------Getting all youtubeVideos by user...");

            var youtubeVideos = new List<YoutubeVideo>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                Debug.WriteLine("---------- Selecting youtubeVideos by user with id = " + userId);
                SqlCommand GetYoutubeVideos = new SqlCommand("SELECT * FROM youtube_video WHERE created_by_user_id = @created_by_user_id ORDER BY date_time_created DESC", conn);
                GetYoutubeVideos.Parameters.Add(new SqlParameter("created_by_user_id", userId));

                using (SqlDataReader reader = GetYoutubeVideos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        youtubeVideos.Add(new YoutubeVideo(reader));
                    }
                }
            }

            foreach (YoutubeVideo temp in youtubeVideos)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found youtubeVideo with id {0}", temp.Id));
            }

            return youtubeVideos;
        }

        public static List<YoutubeVideo> GetYoutubeVideosNotInAnAlbum()
        {
            throw new NotImplementedException();
        }

        private YoutubeVideo()
        {

        }

        public YoutubeVideo(string dataId)
        {
            DataId = dataId;
        }

        public YoutubeVideo(string directory, User createdBy, Album album)
        {
            Id = Guid.NewGuid();
            Directory = directory;
            Log DebugLog = new Log("Debug");
            new LogEntry(DebugLog) { text = "YoutubeVideo created with album_id = " + album.Id };
            AlbumId = album.Id;
            DateCreated = Parsnip.AdjustedTime;
            CreatedById = createdBy.Id;
        }

        public YoutubeVideo(Guid guid)
        {
            //Debug.WriteLine("YoutubeVideo was initialised with the guid: " + pGuid);
            Id = guid;
        }

        public YoutubeVideo(SqlDataReader reader)
        {
            //Debug.WriteLine("YoutubeVideo was initialised with an SqlDataReader. Guid: " + pReader[0]);
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

        private bool ExistsOnDb(SqlConnection conn)
        {
            if (IdExistsOnDb(conn))
                return true;
            else
                return false;
        }

        private bool IdExistsOnDb(SqlConnection conn)
        {
            Debug.WriteLine(string.Format("Checking weather youtube_video exists on database by using Id {0}", Id));
            try
            {
                SqlCommand findMeById = new SqlCommand("SELECT COUNT(*) FROM youtube_video WHERE id = @id", conn);
                findMeById.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int youtubeVideoExists;

                using (SqlDataReader reader = findMeById.ExecuteReader())
                {
                    reader.Read();
                    youtubeVideoExists = Convert.ToInt16(reader[0]);
                    //Debug.WriteLine("Found youtubeVideo by Id. youtubeVideoExists = " + youtubeVideoExists);
                }

                //Debug.WriteLine(youtubeVideoExists + " youtubeVideo(s) were found with the id " + Id);

                if (youtubeVideoExists > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst checking if youtubeVideo exists on the database by using it's Id: " + e);
                return false;
            }
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
                        Debug.WriteLine("----------Title is blank. Getting title");
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

                Debug.WriteLine("Reading data id " + reader[3].ToString());
                DataId = reader[3].ToString().Trim();

                if (logMe)
                    Debug.WriteLine("----------Reading datecreated");
                DateCreated = Convert.ToDateTime(reader[4]);

                if (logMe)
                    Debug.WriteLine("----------Reading createdbyid");
                CreatedById = new Guid(reader[5].ToString());

                try
                {
                    if (reader[6] != DBNull.Value && !string.IsNullOrEmpty(reader[6].ToString()) && 
                        !string.IsNullOrWhiteSpace(reader[6].ToString()))

                    {
                        if (logMe)
                            Debug.WriteLine("----------Reading album id");

                        AlbumId = new Guid(reader[6].ToString());
                    }
                    else
                    {
                        if (logMe)
                            Debug.WriteLine("----------Album id is blank. Skipping album id");
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    if (logMe)
                        Debug.WriteLine("----------Album id was not requested in the query. Skipping album id");
                }

                if (logMe)
                    Debug.WriteLine("added values successfully! Checking title...");

                CheckTitle();
                
                Debug.WriteLine("----------Youtube video title = " + Title);

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
                {

                    Debug.WriteLine("Title was null!");
                    
                    Title = GetTitle(DataId);
                    Debug.WriteLine(string.Format("Got title! \"{0}\" Updating database", Title));
                    try
                    {


                        using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                        {
                            conn.Open();
                            SqlCommand UpdateTitle = new SqlCommand("UPDATE youtube_video SET title = @title WHERE youtube_video_id = @youtube_video_id", conn);
                            UpdateTitle.Parameters.Add(new SqlParameter("title", Title));
                            UpdateTitle.Parameters.Add(new SqlParameter("youtube_video_id", Id));

                            Debug.WriteLine("Executing query");
                            UpdateTitle.ExecuteNonQuery();
                        }
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine("Exception thrown: " + ex);
                        throw ex;
                    }

                    Debug.WriteLine("Database updated!");
                }
            }
        }

        

        private static string GetTitle(string url)
        {
            Debug.WriteLine("----------Getting youtube video title...");
            //var api = $"http://youtube.com/get_video_info?video_id={GetArgs(url, "v", '?')}";
            var api = $"http://youtube.com/get_video_info?video_id=" + url;
            return GetArgs(new WebClient().DownloadString(api), "title", '&');

            string GetArgs(string args, string key, char query)
            {
                var iqs = args.IndexOf(query);
                return iqs == -1
                    ? string.Empty
                    : HttpUtility.ParseQueryString(iqs < args.Length - 1
                        ? args.Substring(iqs + 1) : string.Empty)[key];
            }
        }

        private bool DbInsert(SqlConnection conn)
        {
            throw new NotImplementedException();
            if (Id.ToString() == Guid.Empty.ToString())
            {
                Id = Guid.NewGuid();
                Debug.WriteLine("Id was empty when trying to insert youtubeVideo into the database. A new guid was generated: {0}", Id);
            }

            if (Directory != null)
            {
                try
                {
                    if (!ExistsOnDb(conn))
                    {
                        SqlCommand InsertYoutubeVideoIntoDb = new SqlCommand("INSERT INTO youtube _videos (youtubeVideo_id, directory, date_time_created, created_by_user_id) VALUES(@youtubeVideo_id, @directory, @date_time_created, @created_by_user_id)", conn);

                        InsertYoutubeVideoIntoDb.Parameters.Add(new SqlParameter("id", Id));
                        InsertYoutubeVideoIntoDb.Parameters.Add(new SqlParameter("directory", Directory.Trim()));
                        InsertYoutubeVideoIntoDb.Parameters.Add(new SqlParameter("date_time_created", Parsnip.AdjustedTime));
                        InsertYoutubeVideoIntoDb.Parameters.Add(new SqlParameter("created_by_user_id", CreatedById));

                        InsertYoutubeVideoIntoDb.ExecuteNonQuery();

                        SqlCommand InsertYoutubeVideoAlbumPairIntoDb = new SqlCommand("INSERT INTO t_YoutubeVideoAlbumPairs VALUES(@image_id, @album_id)", conn);
                        InsertYoutubeVideoAlbumPairIntoDb.Parameters.Add(new SqlParameter("image_id", Id));
                        InsertYoutubeVideoAlbumPairIntoDb.Parameters.Add(new SqlParameter("album_id", AlbumId));

                        InsertYoutubeVideoAlbumPairIntoDb.ExecuteNonQuery();

                        Debug.WriteLine(String.Format("Successfully inserted youtubeVideo into database ({0}) ", Id));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Tried to insert youtubeVideo into the database but it alread existed! Id = {0}", Id));
                    }
                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.YoutubeVideo.DbInsert)] Failed to insert youtubeVideo into the database: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("YoutubeVideo was successfully inserted into the database!") };
                return DbUpdate(conn);
            }
            else
            {
                throw new InvalidOperationException("YoutubeVideo cannot be inserted. The youtubeVideo's property: youtubeVideosrc must be initialised before it can be inserted!");
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

        internal bool DbSelect(SqlConnection conn)
        {
            //AccountLog.Debug("Attempting to get youtubeVideo details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get youtubeVideo details with id {0}...", Id));

            try
            {
                SqlCommand SelectYoutubeVideo;

                if (DataId == null)
                {
                    Debug.WriteLine(string.Format("DataId \"{0}\" was NULL", DataId));
                    SelectYoutubeVideo = new SqlCommand("SELECT * FROM youtube_video WHERE youtube_video_id = @id", conn);
                    SelectYoutubeVideo.Parameters.Add(new SqlParameter("id", Id.ToString()));
                }
                else
                {
                    Debug.WriteLine(string.Format("DataId \"{0}\" was NOT NULL", DataId));
                    SelectYoutubeVideo = new SqlCommand("SELECT * FROM youtube_video WHERE data_id = @dataid", conn);
                    SelectYoutubeVideo.Parameters.Add(new SqlParameter("dataid", DataId));
                }

                int recordsFound = 0;
                using (SqlDataReader reader = SelectYoutubeVideo.ExecuteReader())
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
                    //Debug.WriteLine("----------DbSelect() - Got youtubeVideo's details successfully!");
                    //AccountLog.Debug("Got youtubeVideo's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbSelect() - No youtubeVideo data was returned");
                    //AccountLog.Debug("Got youtubeVideo's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst getting youtubeVideo data: " + e);
                return false;
            }
        }

        public bool Update()
        {
            bool success;
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                success = ExistsOnDb(conn) ? DbUpdate(conn) : DbInsert(conn);
            }
                
            return success;
        }

        private bool DbUpdate(SqlConnection conn)
        {
            throw new NotImplementedException();
            Debug.WriteLine("Attempting to update youtubeVideo with Id = " + Id);
            bool HasBeenInserted = true;

            if (HasBeenInserted)
            {
                try
                {
                    YoutubeVideo temp = new YoutubeVideo(Id);
                    temp.Select();

                    if (AlbumId != null && AlbumId.ToString() != Guid.Empty.ToString())
                    {
                        Log DebugLog = new Log("Debug");
                        new LogEntry(DebugLog) { text = "AlbumId != null = " + AlbumId };

                        SqlCommand DeleteOldPairs = new SqlCommand("DELETE FROM media_tag_pair WHERE image_id = @youtube_video_id", conn);
                        DeleteOldPairs.Parameters.Add(new SqlParameter("youtubeVideoid", Id));
                        DeleteOldPairs.ExecuteNonQuery();

                        SqlCommand CreatePhotoAlbumPair = new SqlCommand("INSERT INTO media_tag_pair VALUES (@youtube_video_id, @album_id)", conn);
                        CreatePhotoAlbumPair.Parameters.Add(new SqlParameter("youtube_video_id", Id));
                        CreatePhotoAlbumPair.Parameters.Add(new SqlParameter("album_id", AlbumId));

                        Debug.WriteLine("---------- YoutubeVideo album (INSERT) = " + AlbumId);

                        CreatePhotoAlbumPair.ExecuteNonQuery();
                        new LogEntry(DebugLog) { text = string.Format("INSERTED ALBUM PAIR {0}, {1} ", Id, AlbumId) };
                    }
                    else
                    {
                        Log DebugLog = new Log("Debug");
                        new LogEntry(DebugLog) { text = "YoutubeVideo created with album_id = null :( " };
                    }


                    /*
                    if (Thumbnail != temp.Thumbnail)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update placeholder..."));


                        SqlCommand UpdatePlaceholder = new SqlCommand("UPDATE youtubeVideo SET thumbnail_directory = @thumbnail_directory WHERE youtubeVideo_id = @youtubeVideo_id", conn);
                        UpdatePlaceholder.Parameters.Add(new SqlParameter("youtubeVideo_id", Id));
                        UpdatePlaceholder.Parameters.Add(new SqlParameter("thumbnail_directory", Thumbnail.Trim()));

                        UpdatePlaceholder.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------placeholder was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------placeholder was not changed. Not updating placeholder."));
                    }
                    */
                    /*
                    if (Alt != temp.Alt)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update alt..."));


                        SqlCommand UpdateAlt = new SqlCommand("UPDATE t_YoutubeVideos SET alt = @alt WHERE id = @id", pOpenConn);
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
                    /*
                    if (Title != temp.Title)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update title..."));


                        SqlCommand UpdateTitle = new SqlCommand("UPDATE youtubeVideo SET title = @title WHERE youtubeVideo_id = @youtubeVideo_id", conn);
                        UpdateTitle.Parameters.Add(new SqlParameter("youtubeVideo_id", Id));
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


                        SqlCommand UpdateDescription = new SqlCommand("UPDATE youtubeVideo SET description = @description WHERE youtubeVideo_id = @youtubeVideo_id", conn);
                        UpdateDescription.Parameters.Add(new SqlParameter("youtubeVideo_id", Id));
                        UpdateDescription.Parameters.Add(new SqlParameter("description", Description.Trim()));

                        UpdateDescription.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------Description was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Description was not changed. Not updating description."));
                    }
                    */

                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.YoutubeVideo.DbUpdate] There was an error whilst updating youtubeVideo: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("YoutubeVideo was successfully updated on the database!") };
                return true;
            }
            else
            {
                throw new System.InvalidOperationException("Account cannot be updated. Account must be inserted into the database before it can be updated!");
            }
        }

        public bool Delete()
        {
            using(var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                return DbDelete(conn);
            }
            
        }

        internal bool DbDelete(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get youtubeVideo details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get youtubeVideo details with id {0}...", Id));

            try
            {
                new LogEntry(DebugLog) { text = "Attempting to delete uploaded photo id = " + Id };


                /*using (SqlConnection conn = Parsnip.GetOpenDbConnection())
                {*/
                    SqlCommand DeleteYoutubeVideo = new SqlCommand("DELETE iap FROM media_tag_pair iap FULL OUTER JOIN youtube_video ON image_id = youtube_video.youtube_video_id  WHERE youtube_video.id = @youtube_video_id", pOpenConn);
                    DeleteYoutubeVideo.Parameters.Add(new SqlParameter("youtube_video_id", Id));
                    int recordsAffected = DeleteYoutubeVideo.ExecuteNonQuery();

                    new LogEntry(DebugLog) { text = string.Format("{0} record(s) were affected", recordsAffected) };
                //}
            }
            catch (Exception err)
            {

                new LogEntry(DebugLog) { text = "There was an exception whilst DELETING the youtubeVideo: " + err };
                return false;
            }
            new LogEntry(DebugLog) { text = "Successfully deleted youtubeVideo with id = " + Id };
            return true;

            /*
            try
            {
                SqlCommand deleteAccount = new SqlCommand("DELETE FROM t_YoutubeVideos WHERE id = @id", pOpenConn);
                deleteAccount.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int recordsFound = deleteAccount.ExecuteNonQuery();
                //Debug.WriteLine(string.Format("----------DbDelete() - Found {0} record(s) ", recordsFound));

                if (recordsFound > 0)
                {
                    //Debug.WriteLine("----------DbDelete() - Got youtubeVideo's details successfully!");
                    //AccountLog.Debug("Got youtubeVideo's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbDelete() - No youtubeVideo data was deleted");
                    //AccountLog.Debug("Got youtubeVideo's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst deleting youtubeVideo data: " + e);
                return false;
            }
            */
        }
    }
}
