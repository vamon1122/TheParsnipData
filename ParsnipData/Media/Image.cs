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
    public class Image : Media
    {
        private string _placeholder;
        public string Placeholder { get { if (string.IsNullOrEmpty(_placeholder)) return "Resources/Media/Images/Web_Media/placeholder.gif"; else return _placeholder; } set { _placeholder = value; } }
        
        public string Classes { get; set; }
        public string Alt { get; set; }
        private static string[] _allowedFileExtensions = new string[] { "png", "gif", "jpg", "jpeg", "tiff" };
        public override string[] AllowedFileExtensions { get { return _allowedFileExtensions; } }

        public static bool IsValidFileExtension(string pExtension)
        {
            return _allowedFileExtensions.Contains(pExtension);
        }

        static readonly Log DebugLog = new Log("Debug");

        public override List<Guid> AlbumIds()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all image's album Ids...");

            var albumIds = new List<Guid>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();

                SqlCommand GetImages = new SqlCommand("SELECT media_tag_id FROM media_tag_pair WHERE media_id = @image_id", conn);
                GetImages.Parameters.Add(new SqlParameter("image_id", Id));

                using (SqlDataReader reader = GetImages.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if(reader[0].ToString() != Guid.Empty.ToString())
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

        public static bool Exists(Guid id)
        {
            using(SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                return IdExistsOnDb(conn, id);
            }
        }

        public static List<Image> GetAllImages()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all images...");

            var images = new List<Image>();
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();

                    SqlCommand GetImages = new SqlCommand("SELECT * FROM image INNER JOIN [user] ON [user].user_id = image.created_by_user_id WHERE image.deleted IS NULL AND [user].deleted IS NULL ORDER BY image.date_time_created DESC", conn);
                    using (SqlDataReader reader = GetImages.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            images.Add(new Image(reader));
                        }
                    }
                }

                foreach (Image temp in images)
                {
                    if (logMe)
                        Debug.WriteLine(string.Format("Found image with id {0}", temp.Id));
                }
            }
            catch
            {
                Debug.WriteLine("There was an exception whilst getting all images");
            }

            return images;
        }

        public static List<Image> GetAllDeleted()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all images...");

            var images = new List<Image>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();

                SqlCommand GetImages = new SqlCommand("SELECT * FROM image INNER JOIN [user] ON [user].user_id = image.created_by_user_id WHERE image.deleted NOT NULL AND [user].deleted NOT NULL ORDER BY image.date_time_created DESC", conn);
                using (SqlDataReader reader = GetImages.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        images.Add(new Image(reader));
                    }
                }
            }

            foreach (Image temp in images)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found image with id {0}", temp.Id));
            }

            return images;
        }

        public static List<Image> GetImagesByUser(Guid pUserId)
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all images by user...");

            var images = new List<Image>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();

                Debug.WriteLine("---------- Selecting images by user with id = " + pUserId);
                SqlCommand GetImages = new SqlCommand("SELECT * FROM image INNER JOIN [user] ON [user].user_id = image.created_by_user_id  WHERE image.deleted IS NULL AND created_by_user_id = @created_by_user_id ORDER BY image.date_time_created DESC", conn);
                GetImages.Parameters.Add(new SqlParameter("created_by_user_id", pUserId));

                using (SqlDataReader reader = GetImages.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        images.Add(new Image(reader));
                    }
                }
            }

            foreach (Image temp in images)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found image with id {0}", temp.Id));
            }

            return images;
        }

        public static void DeleteMediaTagPairsByUserId(Guid userId)
        {
            try
            {
                new LogEntry(DebugLog) { text = "Attempting to delete uploaded photos created_by_user_id = " + userId };

                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();
                    
                    SqlCommand DeleteUploads = new SqlCommand("DELETE media_tag_pair FROM media_tag_pair INNER JOIN image ON media_tag_pair.media_id = image.image_id  WHERE image.created_by_user_id = @created_by_user_id", conn);
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

        public static List<Image> GetImagesNotInAnAlbum()
        {
            throw new NotImplementedException();
        }

        public Image(string pSrc, User pCreatedBy, Album pAlbum)
        {
            Id = Guid.NewGuid();
            Directory = pSrc;
            Log DebugLog = new Log("Debug");
            new LogEntry(DebugLog) { text = "Image created with album_id = " + pAlbum.Id };
            AlbumId = pAlbum.Id;
            DateCreated = Parsnip.AdjustedTime;
            DateTimeMediaCreated = DateCreated;
            CreatedById = pCreatedBy.Id;
        }

        public Image(Guid pGuid)
        {
            //Debug.WriteLine("Image was initialised with the guid: " + pGuid);
            Debug.WriteLine("Reading reader");
            Id = pGuid;
        }

        public Image(SqlDataReader pReader)
        {
            //Debug.WriteLine("Image was initialised with an SqlDataReader. Guid: " + pReader[0]);
            AddValues(pReader);
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

        private bool IdExistsOnDb(SqlConnection pOpenConn)
        {
            return IdExistsOnDb(pOpenConn, Id);
        }

        private static bool IdExistsOnDb(SqlConnection pOpenConn, Guid id)
        {
            Debug.WriteLine(string.Format("Checking weather image exists on database by using id {0}", id));
            try
            {
                SqlCommand findMeById = new SqlCommand("SELECT COUNT(*) FROM image WHERE image_id = @image_id", pOpenConn);
                findMeById.Parameters.Add(new SqlParameter("image_id", id.ToString()));

                int imageExists;

                using (SqlDataReader reader = findMeById.ExecuteReader())
                {
                    reader.Read();
                    imageExists = Convert.ToInt16(reader[0]);
                    //Debug.WriteLine("Found image by id. imageExists = " + imageExists);
                }

                //Debug.WriteLine(imageExists + " image(s) were found with the id " + id);

                if (imageExists > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst checking if image exists on the database by using thier id: " + e);
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
                        Debug.WriteLine("----------Reading placeholder");

                    Placeholder = reader[1].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Placeholder is blank. Skipping placeholder");
                }


                if (logMe)
                    Debug.WriteLine("----------Reading ImageSrc");
                Directory = reader[2].ToString().Trim();



                if (reader[3] != DBNull.Value && !string.IsNullOrEmpty(reader[3].ToString()) && !string.IsNullOrWhiteSpace(reader[3].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading alt");

                    Alt = reader[3].ToString().Trim();
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
                    Debug.WriteLine("----------Reading date_time_created");
                DateCreated = Convert.ToDateTime(reader[5]);

                if (logMe)
                    Debug.WriteLine("----------Reading created_by_user_id");
                CreatedById = new Guid(reader[6].ToString());

                if (reader[7] != DBNull.Value && !string.IsNullOrEmpty(reader[7].ToString()) && !string.IsNullOrWhiteSpace(reader[7].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading image title: " + reader[7].ToString().Trim());

                    Title = reader[7].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Title is blank. Skipping title");
                }

                if (reader[8] != DBNull.Value && !string.IsNullOrEmpty(reader[8].ToString()) && !string.IsNullOrWhiteSpace(reader[8].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading description");

                    Description = reader[8].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Description is blank. Skipping description");
                }

                

                try
                {
                    if (reader[10] != DBNull.Value && !string.IsNullOrEmpty(reader[10].ToString()) && !string.IsNullOrWhiteSpace(reader[10].ToString()))
                    {

                        AlbumId = new Guid(reader[10].ToString());
                        if (logMe)
                            Debug.WriteLine("----------Reading album id");

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

                try
                {
                    if (reader[11] != DBNull.Value && !string.IsNullOrEmpty(reader[11].ToString()) &&
                        !string.IsNullOrWhiteSpace(reader[11].ToString()))

                    {
                        if (logMe)
                            Debug.WriteLine("----------Reading access_token id");

                        MyAccessToken = new AccessToken((Guid)reader[11], (Guid)reader[12], Convert.ToDateTime(reader[13]), (int)reader[14], (Guid)reader[15]);
                    }
                    else
                    {
                        if (logMe)
                            Debug.WriteLine("----------Access_token id is blank. Creating new access token");

                        Guid loggedInUserId = ParsnipData.Accounts.User.GetLoggedInUserId();
                        if(loggedInUserId.ToString() != Guid.Empty.ToString())
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

                if (logMe)
                    Debug.WriteLine("added values successfully!");
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst reading the Image's values: ", e);
                return false;
            }
        }

        private bool DbInsert(SqlConnection pOpenConn)
        {
            if (Id.ToString() == Guid.Empty.ToString())
            {
                Id = Guid.NewGuid();
                Debug.WriteLine("Id was empty when trying to insert image into the database. A new guid was generated: {0}", Id);
            }

            if (Directory != null)
            {
                try
                {
                    if (!ExistsOnDb(pOpenConn))
                    {
                        SqlCommand InsertImageIntoDb = new SqlCommand("INSERT INTO image (image_id, src, date_time_created, date_time_media_created, created_by_user_id) VALUES(@image_id, @src, @date_time_created, @date_time_media_created, @created_by_user_id)", pOpenConn);

                        InsertImageIntoDb.Parameters.Add(new SqlParameter("image_id", Id));
                        InsertImageIntoDb.Parameters.Add(new SqlParameter("src", Directory.Trim()));
                        InsertImageIntoDb.Parameters.Add(new SqlParameter("date_time_created", Parsnip.AdjustedTime));
                        InsertImageIntoDb.Parameters.Add(new SqlParameter("date_time_media_created", Parsnip.AdjustedTime));
                        InsertImageIntoDb.Parameters.Add(new SqlParameter("created_by_user_id", CreatedById));

                        InsertImageIntoDb.ExecuteNonQuery();

                        SqlCommand InsertImageAlbumPairIntoDb = new SqlCommand("INSERT INTO media_tag_pair VALUES(@media_id, @media_tag_id)", pOpenConn);
                        InsertImageAlbumPairIntoDb.Parameters.Add(new SqlParameter("media_id", Id));
                        InsertImageAlbumPairIntoDb.Parameters.Add(new SqlParameter("media_tag_id", AlbumId));

                        InsertImageAlbumPairIntoDb.ExecuteNonQuery();

                        Debug.WriteLine(String.Format("Successfully inserted image into database ({0}) ", Id));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Tried to insert image into the database but it alread existed! Id = {0}", Id));
                    }
                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.Image.DbInsert)] Failed to insert image into the database: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("Image was successfully inserted into the database!") };
                return DbUpdate(pOpenConn);
            }
            else
            {
                throw new InvalidOperationException("Image cannot be inserted. The image's property: src must be initialised before it can be inserted!");
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
            //AccountLog.Debug("Attempting to get image details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get image details with id {0}...", Id));

            try
            {
                SqlCommand SelectImage = new SqlCommand("SELECT image.*, media_tag_pair.media_tag_id FROM image LEFT JOIN media_tag_pair ON media_tag_pair.media_id = image.image_id INNER JOIN [user] ON [user].user_id = image.created_by_user_id LEFT JOIN access_token ON access_token.media_id = image.image_id AND access_token.created_by_user_id = @logged_in_user_id WHERE image_id = @image_id AND image.deleted IS NULL AND [user].deleted IS NULL", pOpenConn);
                SelectImage.Parameters.Add(new SqlParameter("image_id", Id.ToString()));
                SelectImage.Parameters.Add(new SqlParameter("logged_in_user_id", User.GetLoggedInUserId()));

                int recordsFound = 0;
                using (SqlDataReader reader = SelectImage.ExecuteReader())
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
                    //Debug.WriteLine("----------DbSelect() - Got image's details successfully!");
                    //AccountLog.Debug("Got image's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbSelect() - No image data was returned");
                    //AccountLog.Debug("Got image's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst getting image data: " + e);
                return false;
            }
        }
        
        public override bool Update()
        {
            bool success;

            using (SqlConnection UpdateConnection = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                UpdateConnection.Open();
                success = ExistsOnDb(UpdateConnection) ? DbUpdate(UpdateConnection) : DbInsert(UpdateConnection);
                UpdateConnection.Close();
            }
                
            return success;
        }

        private bool DbUpdate(SqlConnection pOpenConn)
        {
            Debug.WriteLine("Attempting to update image with Id = " + Id);
            bool HasBeenInserted = true;

            if (HasBeenInserted)
            {
                try
                {
                    Image temp = new Image(Id);
                    //temp.Select();

                    //Null AlbumId would suggest that there was an error whilst loading / editing the image
                    if(AlbumId != null)
                    {
                        Log DebugLog = new Log("Debug");
                        new LogEntry(DebugLog) { text = "AlbumId != null = " + AlbumId };
                        
                        DbRemoveFromAllAlbums(pOpenConn);

                        //If the image is not an album, we want to re-insert the correct image-album-pair
                        if (AlbumId.ToString() != Guid.Empty.ToString())
                        {
                            SqlCommand CreateImageTagPair = new SqlCommand(
                                "INSERT INTO media_tag_pair VALUES (@media_id, @media_tag_id)", pOpenConn);
                            CreateImageTagPair.Parameters.Add(new SqlParameter("media_id", Id));
                            CreateImageTagPair.Parameters.Add(new SqlParameter("media_tag_id", AlbumId));

                            Debug.WriteLine("---------- Image album (INSERT) = " + AlbumId);

                            CreateImageTagPair.ExecuteNonQuery();
                            new LogEntry(DebugLog) {
                                text = string.Format("INSERTED ALBUM PAIR {0}, {1} ", Id, AlbumId) };
                        }
                    }
                    else
                    {
                        Log DebugLog = new Log("Debug");
                        new LogEntry(DebugLog) { text = "Image created with album_id = null :( " };
                    }
                    

                    if (Placeholder != temp.Placeholder)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update placeholder..."));


                        SqlCommand UpdatePlaceholder = new SqlCommand(
                            "UPDATE image SET placeholder = @placeholder WHERE image_id = @image_id", pOpenConn);
                        UpdatePlaceholder.Parameters.Add(new SqlParameter("image_id", Id));
                        UpdatePlaceholder.Parameters.Add(new SqlParameter("placeholder", Placeholder.Trim()));

                        UpdatePlaceholder.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------placeholder was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(
                            string.Format("----------placeholder was not changed. Not updating placeholder."));
                    }

                    if (Alt != temp.Alt)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update alt..."));


                        SqlCommand UpdateAlt = new SqlCommand(
                            "UPDATE image SET alt = @alt WHERE image_id = @image_id", pOpenConn);
                        UpdateAlt.Parameters.Add(new SqlParameter("image_id", Id));
                        UpdateAlt.Parameters.Add(new SqlParameter("alt", Alt.Trim()));

                        UpdateAlt.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------alt was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------alt was not changed. Not updating alt."));
                    }

                    if (Title != temp.Title)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update title..."));


                        SqlCommand UpdateTitle = new SqlCommand(
                            "UPDATE image SET title = @title WHERE image_id = @image_id", pOpenConn);
                        UpdateTitle.Parameters.Add(new SqlParameter("image_id", Id));
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
                            "UPDATE image SET description = @description WHERE image_id = @image_id", pOpenConn);
                        UpdateDescription.Parameters.Add(new SqlParameter("image_id", Id));
                        UpdateDescription.Parameters.Add(new SqlParameter("description", Description.Trim()));

                        UpdateDescription.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------Description was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(
                            string.Format("----------Description was not changed. Not updating description."));
                    }

                    if (DateTimeMediaCreated != temp.DateTimeMediaCreated)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update DateTimeMediaCreated..."));


                        SqlCommand UpdateDateTimeMediaCreated = new SqlCommand(
                            "UPDATE image SET date_time_media_created = @date_time_media_created WHERE image_id = @image_id", pOpenConn);
                        UpdateDateTimeMediaCreated.Parameters.Add(new SqlParameter("image_id", Id));
                        UpdateDateTimeMediaCreated.Parameters.Add(new SqlParameter("date_time_media_created", DateTimeMediaCreated));

                        UpdateDateTimeMediaCreated.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------DateTimeMediaCreated was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(
                            string.Format("----------DateTimeMediaCreated was not changed. Not updating DateTimeMediaCreated."));
                    }

                }
                catch (Exception e)
                {
                    string error = 
                        string.Format("[UacApi.Image.DbUpdate] There was an error whilst updating image: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(DebugLog) { text = string.Format("Image was successfully updated on the database!") };
                return true;
            }
            else
            {
                throw new System.InvalidOperationException(
                    "Image cannot be updated. Image must be inserted into the database before it can be updated!");
            }
        }

        public override bool Delete()
        {
            using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                return DbDelete(conn);
            }
        }

        private bool DbDelete(SqlConnection pOpenConn)
        {
            try
            {
                new LogEntry(DebugLog) { text = "Attempting to delete uploaded photo id = " + Id };

                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();

                    SqlCommand DeleteImage = new SqlCommand("UPDATE image SET deleted = @dateTimeNow WHERE image_id = @image_id", conn);
                    DeleteImage.Parameters.Add(new SqlParameter("dateTimeNow", Parsnip.AdjustedTime));
                    DeleteImage.Parameters.Add(new SqlParameter("image_id", Id));
                    int recordsAffected = DeleteImage.ExecuteNonQuery();

                    new LogEntry(DebugLog) { text = string.Format("{0} record(s) were affected", recordsAffected) };
                }
            }
            catch (Exception err)
            {

                new LogEntry(DebugLog) { text = "There was an exception whilst DELETING the photo: " + err };
                return false;
            }
            new LogEntry(DebugLog) { text = "Successfully deleted photo with id = " + Id };
            return true;
        }

        public bool RemoveFromAllAlbums()
        {
            using(var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                return DbRemoveFromAllAlbums(conn);
            }
        }

        internal bool DbRemoveFromAllAlbums(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get image details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get image details with id {0}...", Id));

            try
            {
                new LogEntry(DebugLog) { text = string.Format("Attempting to remove uploaded photo id = {0} from all albums!", Id) };

                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();

                    SqlCommand DeleteImage = new SqlCommand("DELETE FROM media_tag_pair WHERE media_id = @image_id", conn);
                    DeleteImage.Parameters.Add(new SqlParameter("image_id", Id));
                    int recordsAffected = DeleteImage.ExecuteNonQuery();

                    new LogEntry(DebugLog) { text = string.Format("{0} record(s) were affected", recordsAffected) };
                }
            }
            catch (Exception err)
            {

                new LogEntry(DebugLog) { text = "There was an exception whilst removing the photo from all albums: " + err };
                return false;
            }
            new LogEntry(DebugLog) { text = string.Format("Successfully removed the photo with id = {0} from all albums!", Id) };
            return true;
        }
    }
}
