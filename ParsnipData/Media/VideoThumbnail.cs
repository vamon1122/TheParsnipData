using ParsnipData.Accounts;
using ParsnipData.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ParsnipData.Media
{
    public class VideoThumbnail
    {
        public MediaId MediaId { get; set; }
        public int CreatedById { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public short XScale { get; set; }
        public short YScale { get; set; }
        public short DisplayOrder { get; set; }
        public bool Active { get; set; }
        public string Placeholder { get; set; }
        public string Compressed { get; set; }
        public string Original { get; set; }
        public long FileSize { get; set; }
        public string UploadsDir { get { return "Resources/Media/Videos/Thumbnails/"; } }
        public VideoThumbnail(Media video, string remoteThumbnailsDir, string thumbnailIdentifier)
        {
            MediaId = video.Id;
            Placeholder = $"{remoteThumbnailsDir}/Placeholders/{video.Id}_{thumbnailIdentifier}.jpg";
            Compressed = $"{remoteThumbnailsDir}/Compressed/{video.Id}_{thumbnailIdentifier}.jpg";
            Original = $"{remoteThumbnailsDir}/Originals/{video.Id}_{thumbnailIdentifier}.png";
        }
        public VideoThumbnail(SqlDataReader reader)
        {
            AddValues(reader);
        }
        public VideoThumbnail(Video video, User uploader, HttpPostedFile thumbnailFile)
        {
            MediaId = video.Id;

            if (thumbnailFile.FileName.Length > 0)
            {
                try
                {
                    string[] thumbnailFileDir = thumbnailFile.FileName.Split('\\');
                    string originalThumbnailFileName = thumbnailFileDir.Last();
                    string originalThumbnailFileExtension = "." + originalThumbnailFileName.Split('.').Last();

                    if (Image.IsValidFileExtension(originalThumbnailFileExtension.Substring(1, originalThumbnailFileExtension.Length - 1).ToLower()))
                    {
                        string generatedFileName = $"{video.Id}_{MediaId.NewMediaId()}";

                        var relativeThumbnailDir = UploadsDir + "Originals/" + generatedFileName + originalThumbnailFileExtension;
                        var fullyQualifiedThumbnailDir = HttpContext.Current.Server.MapPath("~/" + relativeThumbnailDir);
                        thumbnailFile.SaveAs(fullyQualifiedThumbnailDir);


                        //No resize is done here but image needs to go through the process so that it displays properly 
                        //on PC's. If we use the 'original' bitmap, the image will display fine on mobile browser, fine 
                        //on Windows File Explorer, but will be rotated in desktop browsers. However, I noticed that 
                        //the thumbnail was displayed correctly at all times. So, I simply put the original image 
                        //through the same process, and called the new image 'uncompressedImage'. *NOTE* we also need 
                        //to rotate the new image (as we do with the thumbnail), as they loose their rotation 
                        //properties when they are processed using the 'ResizeBitmap' function. This is done after the 
                        //resize. MAKE SURE THAT COMPRESSED IMAGE IS SCALED EXACTLY (it is used to get scale soon)

                        video.Original = relativeThumbnailDir;
                        Media.ProcessMediaThumbnail(video, generatedFileName, originalThumbnailFileExtension);

                        MediaId = video.Id;
                        CreatedById = uploader.Id;
                        DateTimeCreated = Parsnip.AdjustedTime;
                        XScale = video.XScale;
                        YScale = video.YScale;
                        Original = video.Original;
                        Compressed = video.Compressed;
                        Placeholder = video.Placeholder;
                    }
                }
                catch (Exception err)
                {
                    new LogEntry(Log.Debug) { Text = $"There was an exception whilst uploading the video thumbnail: {err}" };
                }
            }
        }

        private bool AddValues(SqlDataReader reader, int loggedInUserId = default)
        {
            try
            {
                MediaId = new MediaId(reader[0].ToString());
                if (reader[1] != DBNull.Value)
                    CreatedById = (int)reader[1];

                DateTimeCreated = Convert.ToDateTime(reader[3]);
                XScale = (short)reader[4];
                YScale = (short)reader[5];
                DisplayOrder = (short)reader[6];
                Active = (bool)reader[7];
                Placeholder = reader[8].ToString().Trim();
                Compressed = reader[9].ToString().Trim();
                Original = reader[10].ToString().Trim();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("There was an error whilst reading the Video's values: " + ex);
                return false;
            }
        }

        public bool Insert()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (var insertVideoThumbnail = new SqlCommand("video_thumbnail_INSERT", conn))
                    {
                        insertVideoThumbnail.CommandType = CommandType.StoredProcedure;

                        insertVideoThumbnail.Parameters.AddWithValue("media_id", MediaId.ToString());
                        if(CreatedById != default)
                            insertVideoThumbnail.Parameters.AddWithValue("created_by_user_id", CreatedById);

                        insertVideoThumbnail.Parameters.AddWithValue("datetime_created", DateTime.Now);
                        insertVideoThumbnail.Parameters.AddWithValue("x_scale", XScale);
                        insertVideoThumbnail.Parameters.AddWithValue("y_scale", YScale);
                        insertVideoThumbnail.Parameters.AddWithValue("placeholder_dir", Placeholder);
                        insertVideoThumbnail.Parameters.AddWithValue("compressed_dir", Compressed);
                        insertVideoThumbnail.Parameters.AddWithValue("original_dir", Original);

                        conn.Open();
                        insertVideoThumbnail.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"There was an exception whilst inserting the video thumbnail: {ex}");
                return false;
            }
        }

        public bool SetAsActive()
        {
            try
            {
                using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (var setAsActive = new SqlCommand("video_thumbnail_UPDATE_active", conn))
                    {
                        setAsActive.CommandType = CommandType.StoredProcedure;

                        setAsActive.Parameters.AddWithValue("media_id", MediaId.ToString());
                        setAsActive.Parameters.AddWithValue("display_order", DisplayOrder);

                        conn.Open();
                        setAsActive.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"There was an error whilst setting the active thumbnail: {ex}");
                return false;
            }
            return true;
        }

        public bool Delete()
        {
            try
            {
                using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (SqlCommand deleteVideoThumbnail = new SqlCommand("video_thumbnail_DELETE_WHERE_media_id_AND_display_order", conn))
                    {
                        deleteVideoThumbnail.CommandType = CommandType.StoredProcedure;

                        deleteVideoThumbnail.Parameters.Add(new SqlParameter("media_id", MediaId.ToString()));
                        deleteVideoThumbnail.Parameters.Add(new SqlParameter("display_order", DisplayOrder));

                        conn.Open();
                        int recordsAffected = deleteVideoThumbnail.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                new LogEntry(Log.Debug) { Text = $"There was an exception whilst DELETING the video thumbnail: {ex}" };
                return false;
            }
            return true;
        }
    }
}
