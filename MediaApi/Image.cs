using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using ParsnipApi;
using UacApi;
using LogApi;

namespace MediaApi
{
    public class Image
    {
        public Guid Id { get; set; }
        public string Placeholder { get; set; }
        public string ImageSrc { get; set; }
        public string Classes { get; set; }
        public string Alt { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public static List<Image> GetAllImages()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all images...");

            var images = new List<Image>();
            using (SqlConnection conn = Parsnip.GetOpenDbConnection())
            {
                SqlCommand GetImages = new SqlCommand("SELECT * FROM t_Images ORDER BY datecreated DESC", conn);
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
            bool logMe = true;

            if (logMe)
                Debug.WriteLine("----------Getting all images by user...");

            var images = new List<Image>();
            using (SqlConnection conn = Parsnip.GetOpenDbConnection())
            {
                Debug.WriteLine("---------- Selecting images by user with id = " + pUserId);
                SqlCommand GetImages = new SqlCommand("SELECT * FROM t_Images WHERE createdbyid = @createdbyid ORDER BY datecreated DESC", conn);
                GetImages.Parameters.Add(new SqlParameter("createdbyid", pUserId));

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

        public Image(string pSrc, User pCreatedBy)
        {
            Id = Guid.NewGuid();
            ImageSrc = pSrc;
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
            return ExistsOnDb(Parsnip.GetOpenDbConnection());
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
                SqlCommand findMeById = new SqlCommand("SELECT COUNT(*) FROM t_Images WHERE id = @id", pOpenConn);
                findMeById.Parameters.Add(new SqlParameter("id", Id.ToString()));

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
                ImageSrc = pReader[2].ToString().Trim();



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
                    Debug.WriteLine("----------Reading datecreated");
                DateCreated = Convert.ToDateTime(pReader[4]);

                if (logMe)
                    Debug.WriteLine("----------Reading createdbyid");
                CreatedById = new Guid(pReader[5].ToString());

                if (pReader[6] != DBNull.Value && !string.IsNullOrEmpty(pReader[6].ToString()) && !string.IsNullOrWhiteSpace(pReader[6].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading title");

                    Alt = pReader[6].ToString().Trim();
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

                    Alt = pReader[7].ToString().Trim();
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

            if (ImageSrc != null)
            {
                try
                {
                    if (!ExistsOnDb(pOpenConn))
                    {
                        SqlCommand InsertImageIntoDb = new SqlCommand("INSERT INTO t_Images (id, imagesrc, datecreated, createdbyid) VALUES(@id, @imagesrc, @datecreated, @createdbyid)", pOpenConn);

                        InsertImageIntoDb.Parameters.Add(new SqlParameter("id", Id));
                        InsertImageIntoDb.Parameters.Add(new SqlParameter("imagesrc", ImageSrc.Trim()));
                        InsertImageIntoDb.Parameters.Add(new SqlParameter("datecreated", Parsnip.adjustedTime));
                        InsertImageIntoDb.Parameters.Add(new SqlParameter("createdbyid", CreatedById));

                        InsertImageIntoDb.ExecuteNonQuery();

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
                throw new InvalidOperationException("Image cannot be inserted. The image's property: imagesrc must be initialised before it can be inserted!");
            }
        }

        public bool Select()
        {
            return DbSelect(Parsnip.GetOpenDbConnection());
        }

        internal bool DbSelect(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get image details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get image details with id {0}...", Id));

            try
            {
                SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_Images WHERE id = @id", pOpenConn);
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
            SqlConnection UpdateConnection = Parsnip.GetOpenDbConnection();
            if (ExistsOnDb(UpdateConnection)) success = DbUpdate(UpdateConnection); else success = DbInsert(UpdateConnection);
            UpdateConnection.Close();
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

                    if (Placeholder != temp.Placeholder)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update placeholder..."));


                        SqlCommand UpdatePlaceholder = new SqlCommand("UPDATE t_Images SET placeholder = @placeholder WHERE id = @id", pOpenConn);
                        UpdatePlaceholder.Parameters.Add(new SqlParameter("id", Id));
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


                        SqlCommand UpdateAlt = new SqlCommand("UPDATE t_Images SET alt = @alt WHERE id = @id", pOpenConn);
                        UpdateAlt.Parameters.Add(new SqlParameter("id", Id));
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


                        SqlCommand UpdateTitle = new SqlCommand("UPDATE t_Images SET title = @title WHERE id = @id", pOpenConn);
                        UpdateTitle.Parameters.Add(new SqlParameter("id", Id));
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


                        SqlCommand UpdateDescription = new SqlCommand("UPDATE t_Images SET description = @description WHERE id = @id", pOpenConn);
                        UpdateDescription.Parameters.Add(new SqlParameter("id", Id));
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
                throw new System.InvalidOperationException("Account cannot be updated. Account must be inserted into the database before it can be updated!");
            }
        }

        public bool Delete()
        {
            return DbDelete(Parsnip.GetOpenDbConnection());
        }

        internal bool DbDelete(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get image details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get image details with id {0}...", Id));

            try
            {
                SqlCommand deleteAccount = new SqlCommand("DELETE FROM t_Images WHERE id = @id", pOpenConn);
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
        }

    }
}
