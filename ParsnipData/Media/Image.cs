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
        public string Placeholder { get; set; }
        
        public string Classes { get; set; }
        public string Alt { get; set; }
        private static string[] _allowedFileExtensions = new string[] { "png", "gif", "jpg", "jpeg", "tiff" };
        public override string[] AllowedFileExtensions { get { return _allowedFileExtensions; } }

        public static bool IsValidFileExtension(string pExtension)
        {
            return _allowedFileExtensions.Contains(pExtension);
        }

        static readonly Log DebugLog = new Log("Debug");

        public List<Guid> AlbumIds()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all image's album Ids...");

            var albumIds = new List<Guid>();
            using (SqlConnection conn = new SqlConnection(Parsnip.sqlConnectionString))
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

        public static List<Image> GetAllImages()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all images...");

            var images = new List<Image>();
            using (SqlConnection conn = new SqlConnection(Parsnip.sqlConnectionString))
            {
                conn.Open();

                SqlCommand GetImages = new SqlCommand("SELECT * FROM image ORDER BY date_time_created DESC", conn);
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
            using (SqlConnection conn = new SqlConnection(Parsnip.sqlConnectionString))
            {
                conn.Open();

                Debug.WriteLine("---------- Selecting images by user with id = " + pUserId);
                SqlCommand GetImages = new SqlCommand("SELECT * FROM image WHERE created_by_user_id = @created_by_user_id ORDER BY date_time_created DESC", conn);
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

        public static void DeleteImagesByUser(Guid userId)
        {
            try
            {
                new LogEntry(DebugLog) { text = "Attempting to delete uploaded photos created_by_user_id = " + userId };

                using (SqlConnection conn = new SqlConnection(Parsnip.sqlConnectionString))
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
            DateCreated = Parsnip.adjustedTime;
            CreatedById = pCreatedBy.Id;
        }

        public Image(Guid pGuid)
        {
            //Debug.WriteLine("Image was initialised with the guid: " + pGuid);
            Id = pGuid;
        }

        public Image(SqlDataReader pReader)
        {
            //Debug.WriteLine("Image was initialised with an SqlDataReader. Guid: " + pReader[0]);
            AddValues(pReader);
        }

        public bool ExistsOnDb()
        {
            using(var conn = new SqlConnection(Parsnip.sqlConnectionString))
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
            Debug.WriteLine(string.Format("Checking weather image exists on database by using Id {0}", Id));
            try
            {
                SqlCommand findMeById = new SqlCommand("SELECT COUNT(*) FROM image WHERE image_id = @image_id", pOpenConn);
                findMeById.Parameters.Add(new SqlParameter("image_id", Id.ToString()));

                int imageExists;

                using (SqlDataReader reader = findMeById.ExecuteReader())
                {
                    reader.Read();
                    imageExists = Convert.ToInt16(reader[0]);
                    //Debug.WriteLine("Found image by Id. imageExists = " + imageExists);
                }

                //Debug.WriteLine(imageExists + " image(s) were found with the id " + Id);

                if (imageExists > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst checking if image exists on the database by using thier Id: " + e);
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

                if (pReader[1] != DBNull.Value && !string.IsNullOrEmpty(pReader[1].ToString()) && !string.IsNullOrWhiteSpace(pReader[1].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading placeholder");

                    Placeholder = pReader[1].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Placeholder is blank. Skipping placeholder");
                }


                if (logMe)
                    Debug.WriteLine("----------Reading ImageSrc");
                Directory = pReader[2].ToString().Trim();



                if (pReader[3] != DBNull.Value && !string.IsNullOrEmpty(pReader[3].ToString()) && !string.IsNullOrWhiteSpace(pReader[3].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading alt");

                    Alt = pReader[3].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Alt is blank. Skipping alt");
                }

                if (logMe)
                    Debug.WriteLine("----------Reading date_time_created");
                DateCreated = Convert.ToDateTime(pReader[4]);

                if (logMe)
                    Debug.WriteLine("----------Reading created_by_user_id");
                CreatedById = new Guid(pReader[5].ToString());

                if (pReader[6] != DBNull.Value && !string.IsNullOrEmpty(pReader[6].ToString()) && !string.IsNullOrWhiteSpace(pReader[6].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading title");

                    Title = pReader[6].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Title is blank. Skipping title");
                }

                if (pReader[7] != DBNull.Value && !string.IsNullOrEmpty(pReader[7].ToString()) && !string.IsNullOrWhiteSpace(pReader[7].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading description");

                    Description = pReader[7].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Description is blank. Skipping description");
                }

                try
                {
                    if (pReader[8] != DBNull.Value && !string.IsNullOrEmpty(pReader[8].ToString()) && !string.IsNullOrWhiteSpace(pReader[8].ToString()))
                    {

                        AlbumId = new Guid(pReader[8].ToString());
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
                        SqlCommand InsertImageIntoDb = new SqlCommand("INSERT INTO image (image_id, src, date_time_created, created_by_user_id) VALUES(@image_id, @src, @date_time_created, @created_by_user_id)", pOpenConn);

                        InsertImageIntoDb.Parameters.Add(new SqlParameter("image_id", Id));
                        InsertImageIntoDb.Parameters.Add(new SqlParameter("src", Directory.Trim()));
                        InsertImageIntoDb.Parameters.Add(new SqlParameter("date_time_created", Parsnip.adjustedTime));
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
            using(var conn = new SqlConnection(Parsnip.sqlConnectionString))
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
                SqlCommand SelectAccount = new SqlCommand("SELECT * FROM image WHERE image_id = @image_id", pOpenConn);
                SelectAccount.Parameters.Add(new SqlParameter("image_id", Id.ToString()));

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
        
        public bool Update()
        {
            bool success;

            using (SqlConnection UpdateConnection = new SqlConnection(Parsnip.sqlConnectionString))
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
                    temp.Select();

                    if(AlbumId != null && AlbumId.ToString() != Guid.Empty.ToString())
                    {
                        Log DebugLog = new Log("Debug");
                        new LogEntry(DebugLog) { text = "AlbumId != null = " + AlbumId };

                        SqlCommand DeleteOldPairs = new SqlCommand("DELETE FROM media_tag_pair WHERE media_id = @image_id", pOpenConn);
                        DeleteOldPairs.Parameters.Add(new SqlParameter("image_id", Id));
                        DeleteOldPairs.ExecuteNonQuery();

                        SqlCommand CreatePhotoAlbumPair = new SqlCommand("INSERT INTO media_tag_pair VALUES (@media_id, @media_tag_id)", pOpenConn);
                        CreatePhotoAlbumPair.Parameters.Add(new SqlParameter("media_id", Id));
                        CreatePhotoAlbumPair.Parameters.Add(new SqlParameter("media_tag_id", AlbumId));

                        Debug.WriteLine("---------- Image album (INSERT) = " + AlbumId);

                        CreatePhotoAlbumPair.ExecuteNonQuery();
                        new LogEntry(DebugLog) { text = string.Format("INSERTED ALBUM PAIR {0}, {1} ",Id, AlbumId) };
                    }
                    else
                    {
                        Log DebugLog = new Log("Debug");
                        new LogEntry(DebugLog) { text = "Image created with album_id = null :( " };
                    }
                    

                    if (Placeholder != temp.Placeholder)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update placeholder..."));


                        SqlCommand UpdatePlaceholder = new SqlCommand("UPDATE image SET placeholder = @placeholder WHERE image_id = @image_id", pOpenConn);
                        UpdatePlaceholder.Parameters.Add(new SqlParameter("image_id", Id));
                        UpdatePlaceholder.Parameters.Add(new SqlParameter("placeholder", Placeholder.Trim()));

                        UpdatePlaceholder.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------placeholder was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------placeholder was not changed. Not updating placeholder."));
                    }

                    if (Alt != temp.Alt)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update alt..."));


                        SqlCommand UpdateAlt = new SqlCommand("UPDATE image SET alt = @alt WHERE image_id = @image_id", pOpenConn);
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


                        SqlCommand UpdateTitle = new SqlCommand("UPDATE image SET title = @title WHERE image_id = @image_id", pOpenConn);
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


                        SqlCommand UpdateDescription = new SqlCommand("UPDATE image SET description = @description WHERE image_id = @image_id", pOpenConn);
                        UpdateDescription.Parameters.Add(new SqlParameter("image_id", Id));
                        UpdateDescription.Parameters.Add(new SqlParameter("description", Description.Trim()));

                        UpdateDescription.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------Description was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Description was not changed. Not updating description."));
                    }

                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.Image.DbUpdate] There was an error whilst updating image: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("Image was successfully updated on the database!") };
                return true;
            }
            else
            {
                throw new System.InvalidOperationException("Image cannot be updated. Image must be inserted into the database before it can be updated!");
            }
        }

        public bool Delete()
        {
            using(var conn = new SqlConnection(Parsnip.sqlConnectionString))
            {
                conn.Open();
                return DbDelete(conn);
            }
            
        }

        internal bool DbDelete(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get image details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get image details with id {0}...", Id));

            try
            {
                new LogEntry(DebugLog) { text = "Attempting to delete uploaded photo id = " + Id };

                using (SqlConnection conn = new SqlConnection(Parsnip.sqlConnectionString))
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

                new LogEntry(DebugLog) { text = "There was an exception whilst DELETING the photo: " + err };
                return false;
            }
            new LogEntry(DebugLog) { text = "Successfully deleted photo with id = " + Id };
            return true;

            /*
            try
            {
                SqlCommand deleteAccount = new SqlCommand("DELETE FROM image WHERE id = @id", pOpenConn);
                deleteAccount.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int recordsFound = deleteAccount.ExecuteNonQuery();
                //Debug.WriteLine(string.Format("----------DbDelete() - Found {0} record(s) ", recordsFound));

                if (recordsFound > 0)
                {
                    //Debug.WriteLine("----------DbDelete() - Got image's details successfully!");
                    //AccountLog.Debug("Got image's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbDelete() - No image data was deleted");
                    //AccountLog.Debug("Got image's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst deleting image data: " + e);
                return false;
            }
            */
        }

    }
}
