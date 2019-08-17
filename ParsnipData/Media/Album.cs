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
    public class Album
    {
        public Guid Id { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime DateCreated { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static List<Album> GetAllAlbums()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all albums...");

            var albums = new List<Album>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                SqlCommand GetAlbums = new SqlCommand("SELECT * FROM media_tag INNER JOIN [user] ON [user].user_id = media_tag.created_by_user_id WHERE [user].deleted IS NULL ORDER BY media_tag.date_time_created DESC", conn);
                using (SqlDataReader reader = GetAlbums.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        albums.Add(new Album(reader));
                    }
                }
            }

            foreach (Album temp in albums)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found album with media_tag_id {0}", temp.Id));
            }

            return albums;
        }

        public static List<Image> GetAlbumsByUser(Guid pUserId)
        {
            bool logMe = true;

            if (logMe)
                Debug.WriteLine("----------Getting all albums by user...");

            var albums = new List<Image>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();

                Debug.WriteLine("---------- Selecting albums by user with media_tag_id = " + pUserId);
                SqlCommand GetAlbums = new SqlCommand("SELECT * FROM media_tag " +
                    "INNER JOIN [user] ON [user].user_id = media_tag.created_by_user_id " +
                    "WHERE media_tag.created_by_user_id = @created_by_user_id " +
                    "AND [user].deleted IS NULL " +
                    "ORDER BY media_tag.date_time_created DESC", conn);

                GetAlbums.Parameters.Add(new SqlParameter("created_by_user_id", pUserId));

                using (SqlDataReader reader = GetAlbums.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        albums.Add(new Image(reader));
                    }
                }
            }

            foreach (Image temp in albums)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found album with media_tag_id {0}", temp.Id));
            }

            return albums;
        }

        public List<Image> GetAllImages()
        {
            Debug.WriteLine("Getting all images for album");
            List<Guid> ImageGuids = new List<Guid>();
            List<Image> Images = new List<Image>();

            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                SqlCommand GetImages = new SqlCommand("SELECT image.* FROM image " +
                    "INNER JOIN media_tag_pair ON image.image_id = media_tag_pair.media_id " +
                    "INNER JOIN [user] ON [user].user_id = image.created_by_user_id " +
                    "WHERE image.deleted IS NULL AND media_tag_pair.media_tag_id = @media_tag_id AND [user].deleted IS NULL " +
                    "ORDER BY image.date_time_created DESC", conn);

                GetImages.Parameters.Add(new SqlParameter("media_tag_id", Id));

                using (SqlDataReader reader = GetImages.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Images.Add(new Image(reader));
                    }
                }

                Debug.WriteLine("" + ImageGuids.Count() + "Photos were found");

                int i = 0;

            }

            return Images;
        }

        public List<Video> GetAllVideos()
        {
            Debug.WriteLine("Getting all videos for album");
            List<Guid> VideoGuids = new List<Guid>();
            List<Video> Videos = new List<Video>();

            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                SqlCommand GetVideos = new SqlCommand("SELECT video.* FROM video " +
                    "INNER JOIN media_tag_pair ON video.video_id = media_tag_pair.media_id " +
                    "INNER JOIN[user] ON[user].user_id = video.created_by_user_id " +
                    "WHERE video.deleted IS NULL AND media_tag_pair.media_tag_id = @media_tag_id AND[user].deleted IS NULL " +
                    "ORDER BY video.date_time_created DESC", conn);

                GetVideos.Parameters.Add(new SqlParameter("media_tag_id", Id));

                using (SqlDataReader reader = GetVideos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Videos.Add(new Video(reader));
                    }
                }

                Debug.WriteLine("" + VideoGuids.Count() + "Videos were found");

                int i = 0;

            }

            return Videos;
        }

        public List<YoutubeVideo> GetAllYoutubeVideos()
        {
            Debug.WriteLine("Getting all youtube videos for album");
            List<Guid> YoutubeVideoGuids = new List<Guid>();
            List<YoutubeVideo> YoutubeVideos = new List<YoutubeVideo>();

            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                SqlCommand GetYoutubeVideos = new SqlCommand("SELECT youtube_video.* FROM youtube_video " +
                    "INNER JOIN media_tag_pair ON youtube_video.youtube_video_id = media_tag_pair.media_id " +
                    "INNER JOIN[user] ON[user].user_id = youtube_video.created_by_user_id " +
                    "WHERE youtube_video.deleted IS NULL AND media_tag_pair.media_tag_id = @media_tag_id AND[user].deleted IS NULL " +
                    "ORDER BY youtube_video.date_time_created DESC", conn);

                GetYoutubeVideos.Parameters.Add(new SqlParameter("media_tag_id", Id));

                using (SqlDataReader reader = GetYoutubeVideos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        YoutubeVideos.Add(new YoutubeVideo(reader));
                    }
                }

                Debug.WriteLine("" + YoutubeVideoGuids.Count() + "Videos were found");

                int i = 0;

            }

            return YoutubeVideos;
        }

        public Album(User pCreatedBy)
        {
            Id = Guid.NewGuid();
            DateCreated = Parsnip.AdjustedTime;
            CreatedById = pCreatedBy.Id;
        }

        public Album(Guid pGuid)
        {
            //Debug.WriteLine("Album was initialised with the guid: " + pGuid);
            Id = pGuid;
        }

        public Album(SqlDataReader pReader)
        {
            //Debug.WriteLine("Album was initialised with an SqlDataReader. Guid: " + pReader[0]);
            AddValues(pReader);
        }

        public bool ExistsOnDb()
        {
            using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
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

        private bool IdExistsOnDb(SqlConnection pOpenConn)
        {
            Debug.WriteLine(string.Format("Checking weather album exists on database by using Id {0}", Id));
            try
            {
                SqlCommand findMeById =
                        new SqlCommand("SELECT COUNT(*) FROM media_tag WHERE media_tag_id = @id", pOpenConn);

                findMeById.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int albumExists;

                using (SqlDataReader reader = findMeById.ExecuteReader())
                {
                    reader.Read();
                    albumExists = Convert.ToInt16(reader[0]);
                    //Debug.WriteLine("Found album by Id. albumExists = " + albumExists);
                }

                //Debug.WriteLine(albumExists + " album(s) were found with the media_tag_id " + Id);

                if (albumExists > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                Debug.WriteLine(
                    "There was an error whilst checking if album exists on the database by using thier Id: " + e);

                return false;
            }
        }

        internal bool AddValues(SqlDataReader pReader)
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Adding values...");

            try
            {
                if (logMe)
                    Debug.WriteLine(string.Format("----------Reading id: {0}", pReader[0]));

                Id = new Guid(pReader[0].ToString());

                CreatedById = new Guid(pReader[1].ToString());

                DateCreated = Convert.ToDateTime(pReader[2]);

                if (pReader[3] != DBNull.Value &&
                        !string.IsNullOrEmpty(pReader[3].ToString()) &&
                        !string.IsNullOrWhiteSpace(pReader[3].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading name");

                    Name = pReader[3].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Name is blank. Skipping name");
                }



                if (pReader[4] != DBNull.Value &&
                        !string.IsNullOrEmpty(pReader[4].ToString()) &&
                        !string.IsNullOrWhiteSpace(pReader[4].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading description");

                    Description = pReader[4].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Description is blank. Skipping description");
                }

                if (logMe)
                    Debug.WriteLine("added values successfully!");

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst reading the Album's values: ", e);
                return false;
            }
        }

        private bool DbInsert(SqlConnection pOpenConn)
        {
            if (Id.ToString() == Guid.Empty.ToString())
            {
                Id = Guid.NewGuid();
                Debug.WriteLine(
                    "Id was empty when trying to insert album into the database. A new guid was generated: {0}", Id);
            }
            try
            {
                if (!ExistsOnDb(pOpenConn))
                {
                    SqlCommand InsertAlbumIntoDb = new SqlCommand("INSERT INTO media_tag (id, created_by_user_id, " +
                        "date_time_created) VALUES(@id, @created_by_user_id, @date_time_created)", pOpenConn);

                    InsertAlbumIntoDb.Parameters.Add(new SqlParameter("id", Id));
                    InsertAlbumIntoDb.Parameters.Add(new SqlParameter("created_by_user_id", CreatedById));
                    InsertAlbumIntoDb.Parameters.Add(new SqlParameter("date_time_created", Parsnip.AdjustedTime));


                    InsertAlbumIntoDb.ExecuteNonQuery();

                    Debug.WriteLine(String.Format("Successfully inserted album into database ({0}) ", Id));
                }
                else
                {
                    Debug.WriteLine(string.Format("----------Tried to insert album into the database but it alread " +
                        "existed! Id = {0}", Id));
                }
            }
            catch (Exception e)
            {
                string error =
                    string.Format("[UacApi.Album.DbInsert)] Failed to insert album into the database: {0}", e);

                Debug.WriteLine(error);
                new LogEntry(Log.Default) { text = error };
                return false;
            }
            new LogEntry(Log.Default) { text = string.Format("Album was successfully inserted into the database!") };
            return DbUpdate(pOpenConn);
        }

        public bool Select()
        {
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                return DbSelect(conn);
            }

        }

        internal bool DbSelect(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get album details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get album details with media_tag_id {0}...", Id));

            try
            {
                SqlCommand SelectAccount = new SqlCommand("SELECT * FROM media_tag " +
                    "INNER JOIN [user] ON [user].user_id = media_tag.created_by_user_id " +
                    "WHERE media_tag.media_tag_id = @id AND [user].deleted IS NULL", pOpenConn);

                SelectAccount.Parameters.Add(new SqlParameter("id", Id.ToString()));

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
                    //Debug.WriteLine("----------DbSelect() - Got album's details successfully!");
                    //AccountLog.Debug("Got album's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbSelect() - No album data was returned");
                    //AccountLog.Debug("Got album's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst getting album data: " + e);
                return false;
            }
        }

        public bool Update()
        {
            bool success;
            using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                success = ExistsOnDb(conn) ? DbUpdate(conn) : DbInsert(conn);

            }

            return success;
        }

        private bool DbUpdate(SqlConnection pOpenConn)
        {
            Debug.WriteLine("Attempting to update album with Id = " + Id);
            bool HasBeenInserted = true;

            if (HasBeenInserted)
            {
                try
                {
                    Album temp = new Album(Id);
                    temp.Select();

                    if (Name != temp.Name)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update name..."));


                        SqlCommand UpdateAlbumName =
                                new SqlCommand("UPDATE media_tag SET name = @name WHERE media_tag_id = @id", pOpenConn);

                        UpdateAlbumName.Parameters.Add(new SqlParameter("id", Id));
                        UpdateAlbumName.Parameters.Add(new SqlParameter("name", Name.Trim()));

                        UpdateAlbumName.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------Name was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Name was not changed. Not updating name."));
                    }

                    if (Description != temp.Description)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update description..."));


                        SqlCommand UpdateAlt = new SqlCommand(
                            "UPDATE media_tag SET description = @description WHERE media_tag_id = @id", pOpenConn);

                        UpdateAlt.Parameters.Add(new SqlParameter("id", Id));
                        UpdateAlt.Parameters.Add(new SqlParameter("description", Description.Trim()));

                        UpdateAlt.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------Description was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Description was not changed. Not updating description."));
                    }

                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.Album.DbUpdate] There was an error whilst updating album: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("Album was successfully updated on the database!") };
                return true;
            }
            else
            {
                throw new System.InvalidOperationException(
                    "Account cannot be updated. Account must be inserted into the database before it can be updated!");
            }
        }

        public bool Delete()
        {
            using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                return DbDelete(conn);
            }

        }

        internal bool DbDelete(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get album details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get album details with media_tag_id {0}...", Id));

            try
            {
                SqlCommand deleteAccount = new SqlCommand("DELETE FROM media_tag WHERE media_tag_id = @id", pOpenConn);
                deleteAccount.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int recordsFound = deleteAccount.ExecuteNonQuery();
                //Debug.WriteLine(string.Format("----------DbDelete() - Found {0} record(s) ", recordsFound));

                if (recordsFound > 0)
                {
                    //Debug.WriteLine("----------DbDelete() - Got album's details successfully!");
                    //AccountLog.Debug("Got album's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbDelete() - No album data was deleted");
                    //AccountLog.Debug("Got album's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst deleting album data: " + e);
                return false;
            }
        }

    }
}
