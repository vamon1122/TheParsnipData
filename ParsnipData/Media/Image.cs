using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using ParsnipData;
using ParsnipData.Accounts;
using ParsnipData.Logging;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;
using System.Drawing;
using System.Web;
using System.Data;

namespace ParsnipData.Media
{
    public class Image : Media
    {
        public override string UploadsDir { get { return "Resources/Media/Images/Uploads/"; } }

        public override string Type { get { return "image"; } }

        private static string[] AllowedFileExtensions = new string[] { "png", "gif", "jpg", "jpeg", "tiff" };
        public static bool IsValidFileExtension(string ext)
        {
            return AllowedFileExtensions.Contains(ext.ToLower());

        }

    #region Constructors
    public Image()
        {

        }
        public Image(SqlDataReader pReader, int loggedInUserId)
        {
            AddValues(pReader, loggedInUserId);
        }
        public Image(User uploader, HttpPostedFile originalFile)
        {
            Id = MediaId.NewMediaId();

            new LogEntry(Log.Debug) { Text = "POSTBACK with image" };
            if (originalFile.FileName.Length > 0)
            {
                try
                {
                    new LogEntry(Log.Debug) { Text = "Attempting to upload the photo" };

                    string[] fileDir = originalFile.FileName.Split('\\');
                    string originalFileName = fileDir.Last();
                    string originalFileExtension = "." + originalFileName.Split('.').Last();

                    if (ParsnipData.Media.Image.IsValidFileExtension(originalFileExtension.Substring(1, originalFileExtension.Length - 1).ToLower()))
                    {
                        string generatedFileName = $"{Id}";

                        var relativeDir = UploadsDir + "Originals/" + generatedFileName + originalFileExtension;
                        var fullyQualifiedDir = HttpContext.Current.Server.MapPath("~/" + relativeDir);
                        originalFile.SaveAs(fullyQualifiedDir);
                        Original = relativeDir;


                        //No resize is done here but image needs to go through the process so that it displays properly 
                        //on PC's. If we use the 'original' bitmap, the image will display fine on mobile browser, fine 
                        //on Windows File Explorer, but will be rotated in desktop browsers. However, I noticed that 
                        //the thumbnail was displayed correctly at all times. So, I simply put the original image 
                        //through the same process, and called the new image 'uncompressedImage'. *NOTE* we also need 
                        //to rotate the new image (as we do with the thumbnail), as they loose their rotation 
                        //properties when they are processed using the 'ResizeBitmap' function. This is done after the 
                        //resize. MAKE SURE THAT COMPRESSED IMAGE IS SCALED EXACTLY (it is used to get scale soon)

                        ProcessMediaThumbnail(this, generatedFileName, originalFileExtension);

                        CreatedById = uploader.Id;

                        DateTimeCreated = Parsnip.AdjustedTime;
                        DateTimeCaptured = DateTimeCreated;
                    }
                    else
                    {

                    }
                }
                catch (Exception err)
                {
                    new LogEntry(Log.Debug) { Text = "There was an exception whilst uploading the photo: " + err };
                }
            }

        }

        #endregion

        #region CRUD
       
        #endregion
    }
}
