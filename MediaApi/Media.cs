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
    public class MediaGroup
    {

    }

    public class Media
    {
        //src="resources/media/images/webMedia/pix-vertical-placeholder.jpg"	data-src="https://lh3.googlephotocontent.com/4jCXzK4Yn5FMLVHnHAh3SZ1CG2HvfKrMHc7bqTv22xS8OXu3m4lR2xgnQG8uA_-maD7MrJek1HWYVR8QdjR3sGaih7BW7cOP-iGSXYfupYFnEQDQ_BnDtc_GMO5V3HfmMgPJ69H08g=w1920-h1080"	
        //data-srcset="https://lh3.googlephotocontent.com/4jCXzK4Yn5FMLVHnHAh3SZ1CG2HvfKrMHc7bqTv22xS8OXu3m4lR2xgnQG8uA_-maD7MrJek1HWYVR8QdjR3sGaih7BW7cOP-iGSXYfupYFnEQDQ_BnDtc_GMO5V3HfmMgPJ69H08g=w1920-h1080"	class="meme	lazy" 	alt=""	

        

    }

    public class Photo
    {
        public Guid Id { get; set; }
        public string Placeholder { get; set; }
        public string PhotoSrc { get; set; }
        public string Classes { get; set; }
        public string Alt { get; set; }

        public static List<Photo> GetAllPhotos()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all photos...");

            var photos = new List<Photo>();
            using (SqlConnection conn = Parsnip.GetOpenDbConnection())
            {
                SqlCommand GetPhotos = new SqlCommand("SELECT * FROM t_Photos", conn);
                using (SqlDataReader reader = GetPhotos.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        photos.Add(new Photo(reader));
                    }
                }
            }

            foreach (Photo temp in photos)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found photo with id {0}", temp.Id));
            }

            return photos;
        }

        public Photo (string pSrc)
        {
            Id = Guid.NewGuid();
            PhotoSrc = pSrc;
        }

        public Photo(Guid pGuid)
        {
            //Debug.WriteLine("Photo was initialised with the guid: " + pGuid);
            Id = pGuid;
        }

        public Photo(SqlDataReader pReader)
        {
            //Debug.WriteLine("Photo was initialised with an SqlDataReader. Guid: " + pReader[0]);
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
            Debug.WriteLine(string.Format("Checking weather photo exists on database by using Id {0}", Id));
            try
            {
                SqlCommand findMeById = new SqlCommand("SELECT COUNT(*) FROM t_Photos WHERE id = @id", pOpenConn);
                findMeById.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int photoExists;

                using (SqlDataReader reader = findMeById.ExecuteReader())
                {
                    reader.Read();
                    photoExists = Convert.ToInt16(reader[0]);
                    //Debug.WriteLine("Found photo by Id. photoExists = " + photoExists);
                }

                //Debug.WriteLine(photoExists + " photo(s) were found with the id " + Id);

                if (photoExists > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst checking if photo exists on the database by using thier Id: " + e);
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
                    Debug.WriteLine("----------Reading PhotoSrc");
                PhotoSrc = pReader[2].ToString().Trim();

                

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
                    Debug.WriteLine("added values successfully!");

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst reading the Photo's values: ", e);
                return false;
            }
        }

        private bool DbInsert(SqlConnection pOpenConn)
        {
            if (Id.ToString() == Guid.Empty.ToString())
            {
                Id = Guid.NewGuid();
                Debug.WriteLine("Id was empty when trying to insert photo into the database. A new guid was generated: {0}",  Id);
            }

            if (PhotoSrc != null)
            {
                try
                {
                    if (!ExistsOnDb(pOpenConn))
                    {
                        SqlCommand InsertPhotoIntoDb = new SqlCommand("INSERT INTO t_Photos (id, photosrc) VALUES(@id, @photosrc)", pOpenConn);

                        InsertPhotoIntoDb.Parameters.Add(new SqlParameter("id", Id));
                        InsertPhotoIntoDb.Parameters.Add(new SqlParameter("photosrc", PhotoSrc.Trim()));

                        InsertPhotoIntoDb.ExecuteNonQuery();

                        Debug.WriteLine(String.Format("Successfully inserted photo into database ({0}) ", Id));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Tried to insert photo into the database but it alread existed! Id = {0}", Id));
                    }
                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.Photo.DbInsert)] Failed to insert photo into the database: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("Photo was successfully inserted into the database!") };
                return DbUpdate(pOpenConn);
            }
            else
            {
                throw new InvalidOperationException("Photo cannot be inserted. The photo's property: photosrc must be initialised before it can be inserted!");
            }
        }

        public bool Select()
        {
            return DbSelect(Parsnip.GetOpenDbConnection());
        }

        internal bool DbSelect(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get photo details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get photo details with id {0}...", Id));

            try
            {
                SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_Photos WHERE id = @id", pOpenConn);
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
                    //Debug.WriteLine("----------DbSelect() - Got photo's details successfully!");
                    //AccountLog.Debug("Got photo's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbSelect() - No photo data was returned");
                    //AccountLog.Debug("Got photo's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst getting photo data: " + e);
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
            Debug.WriteLine("Attempting to update photo with Id = " + Id);
            bool HasBeenInserted = true;

            if (HasBeenInserted)
            {
                try
                {
                    Photo temp = new Photo(Id);
                    temp.Select();

                    if (Placeholder != temp.Placeholder)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update placeholder..."));


                        SqlCommand UpdatePlaceholder = new SqlCommand("UPDATE t_Photos SET placeholder = @placeholder WHERE id = @id", pOpenConn);
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


                        SqlCommand UpdatePlaceholder = new SqlCommand("UPDATE t_Photos SET alt = @alt WHERE id = @id", pOpenConn);
                        UpdatePlaceholder.Parameters.Add(new SqlParameter("id", Id));
                        UpdatePlaceholder.Parameters.Add(new SqlParameter("alt", Alt.Trim()));

                        UpdatePlaceholder.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------alt was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------alt was not changed. Not updating placeholder."));
                    }

                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.Photo.DbUpdate] There was an error whilst updating photo: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("Photo was successfully updated on the database!") };
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
            //AccountLog.Debug("Attempting to get photo details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get photo details with id {0}...", Id));

            try
            {
                SqlCommand deleteAccount = new SqlCommand("DELETE FROM t_Photos WHERE id = @id", pOpenConn);
                deleteAccount.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int recordsFound = deleteAccount.ExecuteNonQuery();
                //Debug.WriteLine(string.Format("----------DbDelete() - Found {0} record(s) ", recordsFound));

                if (recordsFound > 0)
                {
                    //Debug.WriteLine("----------DbDelete() - Got photo's details successfully!");
                    //AccountLog.Debug("Got photo's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbDelete() - No photo data was deleted");
                    //AccountLog.Debug("Got photo's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst deleting photo data: " + e);
                return false;
            }
        }

    }

    public class Video : Media
    {

    }
}
