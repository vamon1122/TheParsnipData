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
using System.Drawing;
using System.Web;
using System.Drawing.Imaging;

namespace ParsnipData.Media
{
    public class MediaId : ParsnipId
    {
        public MediaId() : base() { }
        public MediaId(string mediaId) : base(mediaId) { }

        public static MediaId NewMediaId()
        {
            return new MediaId(NewParsnipIdString());
        }
    }
    public class Media
    {
        #region Database Properties
        public MediaId Id { get; set; }
        public virtual string Type { get; set; }
        public int CreatedById { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime DateTimeCaptured { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Alt { get; set; }
        public double XScale { get; set; }
        public double YScale { get; set; }
        public string Placeholder { get { if (string.IsNullOrEmpty(_placeholder)) return "Resources/Media/Images/Web_Media/placeholder.gif"; else return _placeholder; } set { _placeholder = value; } }
        private string _placeholder;
        public string Compressed { get; set; }
        public string Original { get; set; }
        #endregion

        #region Extra Properties
        protected static readonly Log DebugLog = Log.Select(3);
        public int AlbumId { get; set; }
        public virtual string[] AllowedFileExtensions { get { return _allowedFileExtensions; } }
        private static string[] _allowedFileExtensions = new string[] { "png", "gif", "jpg", "jpeg", "tiff" };
        public virtual string UploadsDir { get; }
        public MediaShare MyMediaShare { get; set; }
        #endregion

        #region Constructors
        public Media()
        {

        }

        public Media(SqlDataReader pReader, int loggedInUserId)
        {
            AddValues(pReader, loggedInUserId);
        }
        #endregion

        public static bool IsValidFileExtension(string pExtension)
        {
            return _allowedFileExtensions.Contains(pExtension);
        }

        #region Get Media
        public static Media SelectLatestVideo(int loggedInUserId)
        {
            Media myMedia = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    conn.Open();
                    using (SqlCommand getLatestVideo = new SqlCommand("media_SELECT_latest_video", conn))
                    {
                        getLatestVideo.CommandType = CommandType.StoredProcedure;
                        getLatestVideo.Parameters.Add(new SqlParameter("logged_in_user_id", loggedInUserId));

                        using (SqlDataReader reader = getLatestVideo.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                myMedia = new Media(reader, loggedInUserId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return myMedia;
        }
        public static List<Media> SelectByUserId(int userId, int loggedInUserId)
        {
            var media = new List<Media>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                using (SqlCommand getMediaByUser = new SqlCommand("media_SELECT_WHERE_created_by_user_id", conn))
                {
                    getMediaByUser.CommandType = CommandType.StoredProcedure;
                    getMediaByUser.Parameters.Add(new SqlParameter("created_by_user_id", userId));
                    getMediaByUser.Parameters.Add(new SqlParameter("logged_in_user_id", loggedInUserId));
                    conn.Open();

                    using (SqlDataReader reader = getMediaByUser.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            media.Add(new Image(reader, loggedInUserId));
                        }
                    }
                }
            }

            return media;
        }
        public virtual List<int> SelectMediaTagIds()
        {
            var mediaTagIds = new List<int>();
            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                using (SqlCommand getImages = new SqlCommand("media_SELECT_media_tag_id", conn))
                {
                    getImages.CommandType = CommandType.StoredProcedure;
                    getImages.Parameters.Add(new SqlParameter("media_id", Id.ToString()));

                    conn.Open();
                    using (SqlDataReader reader = getImages.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (Convert.ToInt16(reader[0]) != default)
                            {
                                new LogEntry(DebugLog) { text = "An album id was found! = " + reader[0].ToString() };
                                mediaTagIds.Add((int)reader[0]);
                            }
                            else
                            {
                                new LogEntry(DebugLog) { text = "A BLANK album id was found! Not adding Guid = " + reader[0].ToString() };
                            }
                        }
                    }
                }
            }
            return mediaTagIds;
        }
        #endregion
        
        #region Process Media
        public static int GetAspectScale(int width, int height)
        {
            //Gets HCF of width & height. This is used to get the aspect ratio
            double shortSide = width < height ? width : height;

            int hcf = 0;
            for (int i = 1; i <= shortSide; i++)
            {

                if (width % i == 0 && height % i == 0)
                {
                    hcf = i;
                }
            }

            return hcf;
        }
        public static void ProcessMediaThumbnail(Media myMedia, string newFileName)
        {

            string fullyQualifiedUploadsDir = HttpContext.Current.Server.MapPath(myMedia.UploadsDir);

            System.Drawing.Image originalImage = Bitmap.FromFile($"{fullyQualifiedUploadsDir}Originals\\{newFileName}");

            int scale = Media.GetAspectScale(originalImage.Width, originalImage.Height);


            //1280x720
            double compressedLongSide = 1280;
            double compressedMinShortSide = 200;
            Bitmap compressedBitmap;
            if (originalImage.Width < compressedMinShortSide || originalImage.Height < compressedMinShortSide)
            {
                compressedBitmap = originalImage.Width > originalImage.Height ? DrawNewBitmap(originalImage, (int)(originalImage.Width * (compressedMinShortSide / originalImage.Height)), (int)compressedMinShortSide) : DrawNewBitmap(originalImage, (int)((compressedMinShortSide / originalImage.Width) * originalImage.Height), (int)compressedMinShortSide);
            }
            else if (originalImage.Width > compressedLongSide || originalImage.Height > compressedLongSide)
            {
                compressedBitmap = originalImage.Width > originalImage.Height ? DrawNewBitmap(originalImage, (int)compressedLongSide, (int)(originalImage.Height * (compressedLongSide / originalImage.Width))) : DrawNewBitmap(originalImage, (int)(originalImage.Width * (compressedLongSide / originalImage.Height)), (int)compressedLongSide);
            }
            else
            {
                compressedBitmap = DrawNewBitmap(originalImage, originalImage.Width, originalImage.Height);
            }

            //One of the numbers must be a double in order for the result to be double
            //Longest side of the thumbnail should be 250px
            double thumbnailLongSide = 250;
            Bitmap thumbnail = originalImage.Width > originalImage.Height ? DrawNewBitmap(originalImage, (int)thumbnailLongSide, (int)(originalImage.Height * (thumbnailLongSide / originalImage.Width))) : DrawNewBitmap(originalImage, (int)(originalImage.Width * (thumbnailLongSide / originalImage.Height)), (int)thumbnailLongSide);

            if (originalImage.PropertyIdList.Contains(0x112)) //0x112 = Orientation
            {
                var prop = originalImage.GetPropertyItem(0x112);
                if (prop.Type == 3 && prop.Len == 2)
                {
                    //invertScale = true;
                    UInt16 orientationExif = BitConverter.ToUInt16(originalImage.GetPropertyItem(0x112).Value, 0);
                    if (orientationExif == 8)
                    {
                        //We rotate the original image because we need the correct dimensions later on.
                        //This will not affect the original image because it has already been saved.
                        originalImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        compressedBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        thumbnail.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    }
                    else if (orientationExif == 3)
                    {
                        originalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        compressedBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        thumbnail.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }
                    else if (orientationExif == 6)
                    {
                        originalImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        compressedBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        thumbnail.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                }
            }

            //Change image quality
            //https://docs.microsoft.com/en-us/dotnet/api/system.drawing.image.save?view=netframework-4.8
            ImageCodecInfo myImageCodecInfo;
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;
            string newFileExtension = ".jpg";

            // Get an ImageCodecInfo object that represents the JPEG codec.
            myImageCodecInfo = GetEncoderInfo("image/jpeg");

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            myEncoder = System.Drawing.Imaging.Encoder.Quality;
            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one

            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            //L value (e.g. 50L sets compression quality. 0 = min quality / smaller size, 
            //100 = max quality / larger size
            myEncoderParameter = new EncoderParameter(myEncoder, 85L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            compressedBitmap.Save(HttpContext.Current.Server.MapPath(myMedia.UploadsDir + "Compressed\\" + newFileName), myImageCodecInfo, myEncoderParameters);

            myEncoderParameter = new EncoderParameter(myEncoder, 15L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            thumbnail.Save(HttpContext.Current.Server.MapPath(myMedia.UploadsDir + "Placeholders\\" + newFileName), myImageCodecInfo, myEncoderParameters);

            //ParsnipData.Media.Image image = new ParsnipData.Media.Image(uploadsDir + generatedFileName + newFileExtension, uploader, album);
            myMedia.Original = myMedia.UploadsDir + "Originals/" + newFileName;
            myMedia.Compressed = myMedia.UploadsDir + "Compressed/" + newFileName;
            myMedia.Placeholder = myMedia.UploadsDir + "Placeholders/" + newFileName;


            myMedia.XScale = originalImage.Width / scale;
            myMedia.YScale = originalImage.Height / scale;
            //Change image quality
            //https://docs.microsoft.com/en-us/dotnet/api/system.drawing.image.save?view=netframework-4.8

            ImageCodecInfo GetEncoderInfo(string mimeType)
            {
                int j;
                ImageCodecInfo[] encoders;
                encoders = ImageCodecInfo.GetImageEncoders();
                for (j = 0; j < encoders.Length; ++j)
                {
                    if (encoders[j].MimeType == mimeType)
                        return encoders[j];
                }
                return null;
            }
        }
        public static Bitmap DrawNewBitmap(System.Drawing.Image source, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(source, 0, 0, width, height);
            }

            return result;
        }
        #endregion

        #region CRUD
        public virtual bool Insert()
        {
            if (Compressed != null)
            {
                if (Id == null || Id.ToString() == MediaId.Empty.ToString() || Type == default)
                {
                    Id = MediaId.NewMediaId();
                }


                try
                {
                    using (var conn = new SqlConnection(ParsnipData.Parsnip.ParsnipConnectionString))
                    {
                        using (SqlCommand insertMedia = new SqlCommand("media_INSERT", conn))
                        {
                            insertMedia.CommandType = CommandType.StoredProcedure;

                            insertMedia.Parameters.AddWithValue("id", Id.ToString());
                            insertMedia.Parameters.AddWithValue("type", Type);
                            insertMedia.Parameters.AddWithValue("datetime_captured", DateTimeCaptured);
                            insertMedia.Parameters.AddWithValue("datetime_created", DateTimeCreated);
                            insertMedia.Parameters.AddWithValue("x_scale", XScale);
                            insertMedia.Parameters.AddWithValue("y_scale", YScale);
                            insertMedia.Parameters.AddWithValue("original_dir", Original);
                            insertMedia.Parameters.AddWithValue("compressed_dir", Compressed);
                            insertMedia.Parameters.AddWithValue("placeholder_dir", Placeholder);
                            insertMedia.Parameters.AddWithValue("created_by_user_id", CreatedById);
                            insertMedia.Parameters.AddWithValue("media_tag_id", AlbumId);

                            conn.Open();
                            insertMedia.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    string error = $"Failed to insert media into the database: {e}";
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = "Media was successfully inserted into the database!" };
                return Update();

            }
            else
            {
                throw new InvalidOperationException("Media cannot be inserted. The media's property: src must be initialised before it can be inserted!");
            }
        }
        public static Media Select(MediaId mediaId, int loggedInUserId)
        {
            Media media = null;
            try
            {
                using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (SqlCommand SelectImage = new SqlCommand("media_SELECT_WHERE_id", conn))
                    {
                        SelectImage.CommandType = CommandType.StoredProcedure;
                        SelectImage.Parameters.Add(new SqlParameter("id", mediaId.ToString()));
                        SelectImage.Parameters.Add(new SqlParameter("logged_in_user_id", loggedInUserId));

                        int recordsFound = 0;
                        conn.Open();
                        using (SqlDataReader reader = SelectImage.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                media = new Media();
                                media.AddValues(reader, loggedInUserId);
                                recordsFound++;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst getting image data: " + e);
            }
            return media;
        }
        public virtual bool Update()
        {
            if (Id != default)
            {

                try
                {
                    using (var conn = new SqlConnection(ParsnipData.Parsnip.ParsnipConnectionString))
                    {


                        using (var updateImage = new SqlCommand("media_UPDATE", conn))
                        {
                            updateImage.CommandType = CommandType.StoredProcedure;

                            updateImage.Parameters.AddWithValue("id", Id.ToString());
                            updateImage.Parameters.AddWithValue("title", Title);
                            updateImage.Parameters.AddWithValue("description", Description);
                            updateImage.Parameters.AddWithValue("alt", Alt);
                            updateImage.Parameters.AddWithValue("datetime_captured", DateTimeCaptured);
                            updateImage.Parameters.AddWithValue("media_tag_id", AlbumId);
                            //Needs updating so the person who updates is inserted here
                            updateImage.Parameters.AddWithValue("media_tag_created_by_user_id", CreatedById);


                            conn.Open();
                            updateImage.ExecuteNonQuery();
                        }
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
                    "Media cannot be updated. Media must be inserted into the database before it can be updated!");
            }

        }
        public virtual bool Delete()
        {
            try
            {
                using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {

                    new LogEntry(DebugLog) { text = "Attempting to delete uploaded media id = " + Id };

                    using (SqlCommand deleteImage = new SqlCommand("media_DELETE_WHERE_id", conn))
                    {
                        deleteImage.CommandType = CommandType.StoredProcedure;

                        deleteImage.Parameters.Add(new SqlParameter("id", Id.ToString()));

                        conn.Open();
                        int recordsAffected = deleteImage.ExecuteNonQuery();

                        new LogEntry(DebugLog) { text = string.Format("{0} record(s) were affected", recordsAffected) };
                    }

                }
            }
            catch (Exception err)
            {

                new LogEntry(DebugLog) { text = "There was an exception whilst DELETING the media: " + err };
                return false;
            }
            new LogEntry(DebugLog) { text = "Successfully deleted media with id = " + Id };
            return true;
        }
        protected virtual bool AddValues(SqlDataReader reader, int loggedInUserId)
        {
            bool logMe = false;

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
                    XScale = (double)reader[6];

                if (reader[7] != DBNull.Value && !string.IsNullOrEmpty(reader[7].ToString()) && !string.IsNullOrWhiteSpace(reader[7].ToString()))
                    YScale = (double)reader[7];

                if (reader[8] != DBNull.Value && !string.IsNullOrEmpty(reader[8].ToString()) && !string.IsNullOrWhiteSpace(reader[8].ToString()))
                    Original = reader[8].ToString().Trim();

                if (reader[9] != DBNull.Value && !string.IsNullOrEmpty(reader[9].ToString()) && !string.IsNullOrWhiteSpace(reader[9].ToString()))
                    Compressed = reader[9].ToString().Trim();

                if (reader[10] != DBNull.Value && !string.IsNullOrEmpty(reader[10].ToString()) && !string.IsNullOrWhiteSpace(reader[10].ToString()))
                    Placeholder = reader[10].ToString().Trim();

                CreatedById = (int)reader[11];

                try
                {
                    if (reader[12] != DBNull.Value && !string.IsNullOrEmpty(reader[12].ToString()) && !string.IsNullOrWhiteSpace(reader[12].ToString()))
                        AlbumId = (int)reader[12];
                }
                catch (IndexOutOfRangeException)
                {
                    //These values are not necessarily returned in all queries
                }

                try
                {
                    if (reader[13] == DBNull.Value && string.IsNullOrEmpty(reader[13].ToString()) &&
                        string.IsNullOrWhiteSpace(reader[13].ToString()))

                    {
                        if (loggedInUserId.ToString() != default)
                        {
                            MyMediaShare = new MediaShare(Id, loggedInUserId);
                            MyMediaShare.Insert();
                        }
                    }
                    else
                    {
                        MyMediaShare = new MediaShare(new MediaShareId((string)reader[13]), (int)reader[14], Convert.ToDateTime(reader[15]), default, new MediaId((string)reader[0]));
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    //These values are not necessarily returned in all queries
                }

                Type = reader[16].ToString().Trim();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("There was an error whilst reading the media's values: " + ex);
                return false;
            }
        }
        #endregion
    }
}
