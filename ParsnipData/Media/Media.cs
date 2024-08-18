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
using System.Data;
using System.Drawing;
using System.Web;
using System.Drawing.Imaging;
using System.Configuration;
using System.Web.Configuration;
using FreeImageAPI;
using System.Web.UI.WebControls;
using ImageMagick;
using System.IO;

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
    public class MediaStatus : IEquatable<MediaStatus>, IEquatable<string>
    {
        public MediaStatus(string value)
        {
            switch (value)
            {
                case "raw":
                    Value = value;
                    break;
                case "reprocess":
                    Value = value;
                    break;
                case "processing":
                    Value = value;
                    break;
                case "complete":
                    Value = value;
                    break;
                case "error":
                    Value = value;
                    break;
                case "scraping":
                    Value = value;
                    break;
                default:
                    Value = "raw";
                    break;

            }
        }

        public bool Equals(MediaStatus other)
        {
            if (this.Value != other.Value) return false;

            return true;
        }

        public bool Equals(string other)
        {
            if (this.Value != other) return false;

            return true;
        }

        public override string ToString()
        {
            return Value;
        }

        public string Value { get; }

        public static readonly MediaStatus Unprocessed = new MediaStatus("raw");
        public static readonly MediaStatus Reprocess = new MediaStatus("reprocess");
        public static readonly MediaStatus Processing = new MediaStatus("processing");
        public static readonly MediaStatus Complete = new MediaStatus("complete");
        public static readonly MediaStatus Error = new MediaStatus("error");
        public static readonly MediaStatus Scraping = new MediaStatus("scraping");
    }
    public class Media
    {
        private short _xScale;
        private short _yScale;
        private string _placeholder;
        private string _compressed;
        private string _searchTerms;

        #region Database Properties
        public MediaId Id { get; set; }
        public virtual string Type { get; set; }
        public int CreatedById { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime DateTimeCaptured { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Alt { get; set; }
        public short XScale { get { if (_xScale == default) return 16; else return _xScale; } set { _xScale = value; } }
        public short YScale { get { if (_yScale == default) return 9; else return _yScale; } set { _yScale = value; } }
        public string Placeholder { get { if (string.IsNullOrEmpty(_placeholder)) return "Resources/Media/Images/Web_Media/video_thumbnail_placeholder.jpg"; else return _placeholder; } set { _placeholder = value; } }
        public string Compressed { get { if (string.IsNullOrEmpty(_compressed)) return "Resources/Media/Images/Web_Media/video_thumbnail.jpg"; else return _compressed; } set { _compressed = value; } }
        public string Original { get; set; }
        public int ViewCount { get; set; }
        public long FileSize { get; set; }
        public MediaStatus Status { get; set; }
        public string SearchTerms { get { return _searchTerms; } set { _searchTerms = string.IsNullOrEmpty(value) ? null : Parsnip.SanitiseSearchString(value); } }
        #endregion

        #region Extra Properties
        public int AlbumId { get; set; }
        public virtual string UploadsDir { get; }
        public MediaShare MyMediaShare { get; set; }
        public List<MediaTagPair> MediaTagPairs { get; set; }
        public List<MediaUserPair> MediaUserPairs { get; set; }
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

        #region Get Media
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
                            media.Add(new Media(reader, loggedInUserId));
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
                using (SqlCommand selectMediaTagIds = new SqlCommand("media_SELECT_media_tag_id", conn))
                {
                    selectMediaTagIds.CommandType = CommandType.StoredProcedure;
                    selectMediaTagIds.Parameters.Add(new SqlParameter("media_id", Id.ToString()));

                    conn.Open();
                    using (SqlDataReader reader = selectMediaTagIds.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (Convert.ToInt16(reader[0]) != default)
                            {
                                mediaTagIds.Add((int)reader[0]);
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

        public static Bitmap GenerateBitmapOfSize(System.Drawing.Image originalImage, double maxLongSide, double minShortSide)
        {
            if (originalImage.Width < minShortSide || originalImage.Height < minShortSide)
            {
                return originalImage.Width > originalImage.Height ? DrawNewBitmap(originalImage, (int)(originalImage.Width * (minShortSide / originalImage.Height)), (int)minShortSide) : DrawNewBitmap(originalImage, (int)((minShortSide / originalImage.Width) * originalImage.Height), (int)minShortSide);
            }
            else if (originalImage.Width > maxLongSide || originalImage.Height > maxLongSide)
            {
                return originalImage.Width > originalImage.Height ? DrawNewBitmap(originalImage, (int)maxLongSide, (int)(originalImage.Height * (maxLongSide / originalImage.Width))) : DrawNewBitmap(originalImage, (int)(originalImage.Width * (maxLongSide / originalImage.Height)), (int)maxLongSide);
            }
            else
            {
                return DrawNewBitmap(originalImage, originalImage.Width, originalImage.Height);
            }

            Bitmap DrawNewBitmap(System.Drawing.Image source, int width, int height)
            {
                Bitmap result = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(source, 0, 0, width, height);
                }

                return result;
            }
        }

        public static RotateFlipType GetRotateFlipType(System.Drawing.Image image)
        {
            if (image.PropertyIdList.Contains(0x112)) //0x112 = Orientation
            {
                var prop = image.GetPropertyItem(0x112);
                if (prop.Type == 3 && prop.Len == 2)
                {
                    //invertScale = true;
                    UInt16 orientationExif = BitConverter.ToUInt16(image.GetPropertyItem(0x112).Value, 0);
                    if (orientationExif == 8)
                    {
                        //We rotate the original image because we need the correct dimensions later on.
                        //This will not affect the original image because it has already been saved.
                        return RotateFlipType.Rotate270FlipNone;
                    }
                    else if (orientationExif == 3)
                    {
                        return RotateFlipType.Rotate180FlipNone;
                    }
                    else if (orientationExif == 6)
                    {
                        return RotateFlipType.Rotate90FlipNone;
                    }
                }
            }
            return RotateFlipType.RotateNoneFlipNone;
        }

        //public static void RotateFlipMagickImage(MagickImage image)
        //{
        //    var orientation = image.GetAttribute("EXIF:Orientation");

        //    switch (orientation)
        //    {
        //        case "3":
        //            image.Rotate(180);
        //            break;
        //        case "6":
        //            image.Rotate(90);
        //            break;
        //        case "8":
        //            image.Rotate(-90);
        //            break;
        //    }
        //}

        //public static RotateFlipType GetRotateFlipType(MagickImage image)
        //{
        //    //var orientation = image.GetAttribute("EXIF:Orientation");
        //    var orientation = image.Orientation;
        //    //image.AutoOrient();

        //    switch (orientation)
        //    {
        //        case OrientationType.BottomRight:
        //            return RotateFlipType.Rotate180FlipNone;
        //        case OrientationType.RightTop:
        //            return RotateFlipType.Rotate90FlipNone;
        //        case OrientationType.LeftBottom:
        //            return RotateFlipType.Rotate270FlipNone;
        //    }

        //    return RotateFlipType.RotateNoneFlipNone;
        //}

        public static void SaveBitmapWithCompression(Bitmap bitmap, Int64 quality, string newFileDir)
        {
            //Change image quality
            //https://docs.microsoft.com/en-us/dotnet/api/system.drawing.image.save?view=netframework-4.8
            ImageCodecInfo myImageCodecInfo;
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

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
            myEncoderParameter = new EncoderParameter(myEncoder, quality);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bitmap.Save(newFileDir, myImageCodecInfo, myEncoderParameters);

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

        public static void ProcessMediaThumbnail(Media myMedia, string newFileName, string originalFileExtension, string localOverride = null)
        {
            string newFileExtension = ".jpg";
            //string fullyQualifiedUploadsDir = HttpContext.Current.Server.MapPath(myMedia.UploadsDir);
            string fullyQualifiedUploadsDir = localOverride ?? HttpContext.Current.Server.MapPath(myMedia.UploadsDir);
            System.Drawing.Image originalImage;
            Bitmap compressedBitmap;
            Bitmap placeholderBitmap;

            GenerateBitmaps();
            UpdateMetadata();

            void GenerateBitmaps()
            {
                var originalDir = $"{fullyQualifiedUploadsDir}Originals\\{newFileName}{originalFileExtension}";
                //originalImage = originalFileExtension.ToLower() == ".dng" ? FreeImage.GetBitmap(FreeImage.LoadEx(originalDir)) : Bitmap.FromFile(originalDir);

                RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;

                if (originalFileExtension.ToLower() == ".dng" || originalFileExtension.ToLower() == ".heic")
                {
                    #region freeimage
                    if (false)
                    {
                        //var imageThing = FreeImage.LoadEx(originalDir);
                        var imageThing = FreeImageAPI.FreeImage.Load(FREE_IMAGE_FORMAT.FIF_RAW, originalDir, FREE_IMAGE_LOAD_FLAGS.DEFAULT);

                        #region Convert from HDR to SDR
                        //I found that DRAGO made skin tones a bit orange
                        //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(imageThing, FREE_IMAGE_TMO.FITMO_DRAGO03, 1.0, 0.85);
                        //I found that REINHARD makes things a bit dark
                        //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(imageThing, FREE_IMAGE_TMO.FITMO_REINHARD05, 0.6, 0.4);
                        //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(imageThing, FREE_IMAGE_TMO.FITMO_REINHARD05, 1, 0.8);
                        //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(imageThing, FREE_IMAGE_TMO.FITMO_REINHARD05, 0.9, 0.5);
                        FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(imageThing, FREE_IMAGE_TMO.FITMO_REINHARD05, 1, 0.5);
                        //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(imageThing, FREE_IMAGE_TMO.FITMO_REINHARD05, 1, 0.3);
                        //I found that FATTAL was very bright and washed out
                        //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(imageThing, FREE_IMAGE_TMO.FITMO_FATTAL02, 1.0, 0.85);

                        //REINHARD seems like the best bet? Maybe just increase gamma?

                        //Stops at 31 if this is removed (but still have the last one 3/3)
                        //Stops at 41 if I remove 3/3 but not this
                        //Stops at 23 if I remove both
                        FreeImageAPI.FreeImage.Unload(imageThing);
                        imageThing = sdrDib;
                        #endregion

                        //Only FIT_BITMAPS can be vonverted to C# bitmaps. Convert to FIT_BITMAP if not already
                        if (FreeImageAPI.FreeImage.GetImageType(imageThing) != FREE_IMAGE_TYPE.FIT_BITMAP)
                        {
                            FIBITMAP temp = FreeImageAPI.FreeImage.ConvertToType(imageThing, FREE_IMAGE_TYPE.FIT_BITMAP, true);
                            //Doesn't seem to mind whether this one is unloaded or not for some reason.
                            //Leaving it in because it seems wrong not to unload... strange though!
                            FreeImageAPI.FreeImage.Unload(imageThing);
                            imageThing = temp;
                        }
                        var bitmap = FreeImage.GetBitmap(imageThing);
                        //Fixes stopping at 41
                        FreeImageAPI.FreeImage.Unload(imageThing);
                        originalImage = bitmap;
                    }
                    #endregion
                    else
                    {
                        using (var image = new MagickImage(originalDir))
                        {
                            //rotateFlipType = GetRotateFlipType(image);
                            image.AutoOrient();

                            #region shite
                            //if (image.Depth == 16 || image.Depth == 32)
                            //{
                            //    #region shite
                            //    //image.ColorSpace = ColorSpace.sRGB;
                            //    image.ColorSpace = ColorSpace.RGB;
                            //    image.Equalize();
                            //    image.AutoGamma();
                            //    image.AutoLevel();
                            //    ////image.BrightnessContrast(new Percentage(50), new Percentage(50));
                            //    ////image.GammaCorrect(1.5);
                            //    ////image.AutoLevel();

                            //    ////image.BrightnessContrast(new Percentage(20), new Percentage(20));

                            //    //image.Modulate(new Percentage(110), new Percentage(150), new Percentage(100));
                            //    //image.GammaCorrect(1.6);
                            //    ////image.Level(0, Convert.ToUInt16(Quantum.Max * 0.8));
                            //    //image.Equalize();
                            //    //image.AutoGamma();
                            //    //image.AutoLevel();

                            //    //image.Ton
                            //    #endregion

                            //    var imageThing = FreeImageAPI.FreeImage.Load(FREE_IMAGE_FORMAT.FIF_RAW, originalDir, FREE_IMAGE_LOAD_FLAGS.DEFAULT);

                            //    FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(imageThing, FREE_IMAGE_TMO.FITMO_REINHARD05, 1, 0.5);
                            //    FreeImageAPI.FreeImage.Unload(imageThing);
                            //    imageThing = sdrDib;
                            //}
                            using (var memoryStream = new System.IO.MemoryStream())
                            {
                                image.Write(memoryStream, MagickFormat.Bmp);
                                memoryStream.Position = 0;


                                if (image.Depth == 16 || image.Depth == 32)
                                {
                                    //https://github.com/imazen/freeimage/blob/master/Source/FreeImage/ToneMapping.cpp
                                    //REINHARD DEFAULT 0,0 (intensity, contrast)
                                    //DRAGO DEFAULT 2.2, 0 (gamma, exposure)
                                    //FATTAL DEFAULT 0.5, 0.85 (saturation, attenuation)

                                    var hrdDib = FreeImageAPI.FreeImage.LoadFromStream(memoryStream);
                                    FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_REINHARD05, 1, 0.3); //Holy Grail
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_REINHARD05, 2, 0.3); //Reinhard 2 //No difference
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_REINHARD05, 1, 0.5); //Reinhard 3 //Too washed out
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_REINHARD05, 0.8, 0.5); //Reinhard 4 //Also washed out
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_REINHARD05, 0.8, 0.3); //Reinhard 5 //Just dark
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_DRAGO03, 1.0, 0.3); //Drago 1
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_DRAGO03, 0.8, 0.2); //Drago 2
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_DRAGO03, 2.2, 0.3); //Drago 3
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_DRAGO03, 2.2, 0.2); //Drago 4
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_DRAGO03, 1.8, 0.3); //Drago 5
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_DRAGO03, 1.6, 0.3); //Drago 6 //Contender??
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_DRAGO03, 1.4, 0.3); //Drago 7 //Best drago??
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_DRAGO03, 1.2, 0.3); //Drago 8 //Colour perfect??
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_DRAGO03, 1.2, 0.4); //Drago 9
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_DRAGO03, 1.2, 0.2); //Drago 10
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_DRAGO03, 1, 0.3); //Drago 11
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 1.0, 0.3); //Fattal 1
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 0.5, 0.85); //Fattal 2 (Default)
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 0.5, 1); //Fattal 3
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 0.5, 1.5); //Fattal 4 //No difference
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 0.7, 1.0); //Fattal 5
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 1, 1.0); //Fattal 6
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 0, 0); //Fattal 7
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 0, 0.5); //Fattal 8
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 0.5, 0); //Fattal 9
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 0.5, 2); //Fattal 10 //Best so far
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 0.5, 3); //Fattal 11 NOPE - 2 is max
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 1, 2); //Fattal 12
                                    //FIBITMAP sdrDib = FreeImageAPI.FreeImage.ToneMapping(hrdDib, FREE_IMAGE_TMO.FITMO_FATTAL02, 0, 2); //Fattal 13
                                    FreeImage.AdjustBrightness(sdrDib, 10); //Uncomment for Holy Grail
                                    //FreeImage.AdjustContrast(sdrDib, -10);
                                    FreeImageAPI.FreeImage.Unload(hrdDib);
                                    originalImage = FreeImage.GetBitmap(sdrDib);
                                    FreeImageAPI.FreeImage.Unload(sdrDib);
                                }
                                else
                                {
                                    originalImage = new Bitmap(memoryStream);
                                }
                            }
                        }
                        #endregion

                        //if (image.Depth == 16 || image.Depth == 32)
                        //{
                        //    #region shite
                        //    //image.ColorSpace = ColorSpace.sRGB;
                        //    image.ColorSpace = ColorSpace.RGB;
                        //    image.Equalize();
                        //    image.AutoGamma();
                        //    image.AutoLevel();
                        //    ////image.BrightnessContrast(new Percentage(50), new Percentage(50));
                        //    ////image.GammaCorrect(1.5);
                        //    ////image.AutoLevel();

                        //    ////image.BrightnessContrast(new Percentage(20), new Percentage(20));

                        //    //image.Modulate(new Percentage(110), new Percentage(150), new Percentage(100));
                        //    //image.GammaCorrect(1.6);
                        //    ////image.Level(0, Convert.ToUInt16(Quantum.Max * 0.8));
                        //    //image.Equalize();
                        //    //image.AutoGamma();
                        //    //image.AutoLevel();

                        //    //image.Ton
                        //    #endregion
                        //}
                        //using (var memoryStream = new System.IO.MemoryStream())
                        //    {
                        //        image.Write(memoryStream, MagickFormat.Bmp);

                        //        originalImage = new System.Drawing.Bitmap(memoryStream);
                        //    }
                        //}
                    }
                }
                else
                {
                    originalImage = Bitmap.FromFile(originalDir);
                    rotateFlipType = GetRotateFlipType(originalImage);
                }

                compressedBitmap = GenerateBitmapOfSize(originalImage, 1280, 200);
                placeholderBitmap = GenerateBitmapOfSize(originalImage, 250, 0);


                originalImage.RotateFlip(rotateFlipType);
                compressedBitmap.RotateFlip(rotateFlipType);
                placeholderBitmap.RotateFlip(rotateFlipType);

                SaveBitmapWithCompression(compressedBitmap, 85L, $"{fullyQualifiedUploadsDir}\\Compressed\\{newFileName}{newFileExtension}");
                SaveBitmapWithCompression(placeholderBitmap, 15L, $"{fullyQualifiedUploadsDir}\\Placeholders\\{newFileName}{newFileExtension}");
            }

            void UpdateMetadata()
            {



                int scale = Media.GetAspectScale(originalImage.Width, originalImage.Height);
                myMedia.XScale = Convert.ToInt16(originalImage.Width / scale);
                myMedia.YScale = Convert.ToInt16(originalImage.Height / scale);

                myMedia.Original = $"{myMedia.UploadsDir}Originals/{newFileName}{originalFileExtension}";
                myMedia.Compressed = $"{myMedia.UploadsDir}Compressed/{newFileName}{newFileExtension}";
                myMedia.Placeholder = $"{myMedia.UploadsDir}Placeholders/{newFileName}{newFileExtension}";
            }
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
                            if (AlbumId != default)
                                insertMedia.Parameters.AddWithValue("media_tag_id", AlbumId);

                            if (Type == "image" && !string.IsNullOrEmpty(Compressed) && !string.IsNullOrEmpty(Placeholder))
                                insertMedia.Parameters.AddWithValue("status", MediaStatus.Complete.Value);

                            conn.Open();
                            insertMedia.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    string error = $"Failed to insert media into the database: {e}";
                    Debug.WriteLine(error);
                    new LogEntry(Log.General) { Text = error };
                    return false;
                }
                return true;

            }
            else
            {
                throw new InvalidOperationException("Media cannot be inserted. The media's property: src must be initialised before it can be inserted!");
            }
        }
        public void View(User viewedBy, bool isScroll = false, DateTime? timeViewStarted = null, TimeSpan? thresholdTimespan = null, TimeSpan? totalSeconds = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {

                    using (var updateMediaShare = new SqlCommand("media_view_INSERT", conn))
                    {
                        updateMediaShare.CommandType = CommandType.StoredProcedure;
                        updateMediaShare.Parameters.Add(new SqlParameter("media_id", Id.ToString()));
                        updateMediaShare.Parameters.Add(new SqlParameter("created_by_user_id", viewedBy.Id));
                        updateMediaShare.Parameters.Add(new SqlParameter("datetime_view_started", timeViewStarted ?? Parsnip.AdjustedTime));
                        updateMediaShare.Parameters.Add(new SqlParameter("is_scroll", isScroll));
                        if (isScroll)
                        {
                            updateMediaShare.Parameters.Add(new SqlParameter("threshold_timespan", thresholdTimespan));
                            updateMediaShare.Parameters.Add(new SqlParameter("view_timespan", totalSeconds));
                        }

                        conn.Open();
                        updateMediaShare.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            ViewCount++;
        }
        public static Media Select(MediaId mediaId, int loggedInUserId = default)
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

                            if (media != null)
                            {
                                reader.NextResult();

                                media.MediaTagPairs = new List<MediaTagPair>();
                                while (reader.Read())
                                {
                                    var mediaTag = new MediaTagPair(reader);
                                    media.MediaTagPairs.Add(mediaTag);
                                }

                                reader.NextResult();

                                media.MediaUserPairs = new List<MediaUserPair>();
                                while (reader.Read())
                                {
                                    var mediaUserPair = new MediaUserPair(reader);
                                    media.MediaUserPairs.Add(mediaUserPair);
                                }

                                reader.NextResult();
                                while (reader.Read())
                                {
                                    media.ViewCount = (int)reader[0];
                                }
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
        public virtual bool Update(bool isNew = false)
        {
            if (Id != default)
            {

                try
                {
                    using (var conn = new SqlConnection(ParsnipData.Parsnip.ParsnipConnectionString))
                    {


                        using (var updateMedia = new SqlCommand("media_UPDATE", conn))
                        {
                            updateMedia.CommandType = CommandType.StoredProcedure;

                            updateMedia.Parameters.AddWithValue("id", Id.ToString());
                            updateMedia.Parameters.AddWithValue("title", string.IsNullOrWhiteSpace(Title) ? null : Title);
                            updateMedia.Parameters.AddWithValue("description", string.IsNullOrWhiteSpace(Description) ? null : Description);
                            updateMedia.Parameters.AddWithValue("alt", Alt);
                            updateMedia.Parameters.AddWithValue("datetime_captured", DateTimeCaptured);
                            if (AlbumId != default)
                                updateMedia.Parameters.AddWithValue("media_tag_id", AlbumId);
                            //Needs updating so the person who updates is inserted here
                            updateMedia.Parameters.AddWithValue("media_tag_created_by_user_id", CreatedById);
                            if (string.IsNullOrEmpty(SearchTerms))
                                updateMedia.Parameters.AddWithValue("search_terms", DBNull.Value);
                            else
                                updateMedia.Parameters.AddWithValue("search_terms", SearchTerms);
                            updateMedia.Parameters.AddWithValue("is_new", isNew);

                            conn.Open();
                            updateMedia.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    string error =
                        string.Format("There was an error whilst updating media: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.General) { Text = error };
                    return false;
                }
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
                    using (SqlCommand deleteMedia = new SqlCommand("media_DELETE_WHERE_id", conn))
                    {
                        deleteMedia.CommandType = CommandType.StoredProcedure;

                        deleteMedia.Parameters.Add(new SqlParameter("id", Id.ToString()));

                        conn.Open();
                        int recordsAffected = deleteMedia.ExecuteNonQuery();

                        new LogEntry(Log.Debug) { Text = $"{recordsAffected} record(s) were affected" };
                    }

                }
            }
            catch (Exception ex)
            {

                new LogEntry(Log.Debug) { Text = $"There was an exception whilst DELETING the media: {ex}" };
                return false;
            }
            return true;
        }
        protected virtual bool AddValues(SqlDataReader reader, int loggedInUserId)
        {
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
                    XScale = (short)reader[6];

                if (reader[7] != DBNull.Value && !string.IsNullOrEmpty(reader[7].ToString()) && !string.IsNullOrWhiteSpace(reader[7].ToString()))
                    YScale = (short)reader[7];

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
                        if (loggedInUserId.ToString() != default(int).ToString())
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
                Status = new MediaStatus(reader[17].ToString().Trim());

                try
                {
                    if (reader.FieldCount > 18 && reader[18] != DBNull.Value && !string.IsNullOrEmpty(reader[18].ToString()))
                        SearchTerms = reader[18].ToString().Trim();
                }
                catch (IndexOutOfRangeException)
                {

                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("There was an error whilst reading the media's values: " + ex);
                return false;
            }
        }
        #endregion

        public bool IsPortrait()
        {
            if (XScale == default || YScale == default)
                throw new InvalidOperationException();

            return YScale > XScale;
        }

        public bool IsLandscape()
        {
            if (XScale == default || YScale == default)
                throw new InvalidOperationException();

            return XScale > YScale;
        }


        public static MediaSearchResult Search(string text, int loggedInUserId)
        {
            text = System.Text.RegularExpressions.Regex.Replace
            (
                Parsnip.SanitiseSearchString
                (
                    System.Text.RegularExpressions.Regex.Replace(text, " {2,}", " ")
                ).ToLower(), "[^a-z0-9_ ]", ""
            ).RemoveStrings
            (
                ConfigurationManager.AppSettings["IgnoreSearchTerms"]
                    .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
            );
            var mediaSearchResult = new MediaSearchResult(text);
            var tempMediaTagPair = new List<MediaTagPair>();
            var tempMediaUserPair = new List<MediaUserPair>();
            using (var conn = new SqlConnection(ParsnipData.Parsnip.ParsnipConnectionString))
            {
                using (var searchForMedia = new SqlCommand("media_SEARCH_WHERE_text", conn))
                {
                    searchForMedia.CommandType = CommandType.StoredProcedure;

                    searchForMedia.Parameters.AddWithValue("text", text);
                    searchForMedia.Parameters.AddWithValue("logged_in_user_id", loggedInUserId);
                    searchForMedia.Parameters.AddWithValue("now", Parsnip.AdjustedTime);

                    conn.Open();
                    using (SqlDataReader reader = searchForMedia.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var media = new RankedMedia(reader, loggedInUserId);
                            mediaSearchResult.Media.Add(media);
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            var mediaTag = new MediaTag(reader);
                            mediaSearchResult.MediaTags.Add(mediaTag);
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            var mediaTagPair = new MediaTagPair(reader);
                            var media = mediaSearchResult.Media.SingleOrDefault(x => x.Id.Equals(mediaTagPair.MediaId));
                            if (media != null) //(media not returned if deleted)
                            {
                                var tag = mediaSearchResult.MediaTags.SingleOrDefault(mediaTag => mediaTag.Id.Equals(mediaTagPair.MediaTag.Id));
                                media.SearchTerms += $" {tag.Name} {tag.SearchTerms}";
                                tempMediaTagPair.Add(mediaTagPair);
                            }
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            var user = new User(reader);
                            mediaSearchResult.Users.Add(user);
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            var mediaUserPair = new MediaUserPair(reader);
                            var media = mediaSearchResult.Media.SingleOrDefault(m => m.Id.Equals(mediaUserPair.MediaId));
                            if (media != null) //(media not returned if deleted)
                            {
                                var user = mediaSearchResult.Users.SingleOrDefault(u => u.Id == mediaUserPair.UserId);
                                media.SearchTerms += $" {user.FullName} {user.Username} {user.SearchTerms}";
                                tempMediaUserPair.Add(mediaUserPair);
                            }
                        }
                    }
                }

                foreach (var media in mediaSearchResult.Media)
                {
                    var searchedTerms = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var mediaTitle = string.IsNullOrEmpty(media.Title) ? null :
                        $"{System.Text.RegularExpressions.Regex.Replace(media.Title.ToLower(), "[^a-z0-9_ ]", "")}".Split(' ');
                    var mediaSearchTerms = string.IsNullOrEmpty(media.SearchTerms) ? null : media.SearchTerms.Split(' ');

                    foreach (var searchTerm in searchedTerms)
                    {
                        if ((mediaTitle != null && Array.IndexOf(mediaTitle, searchTerm) >= 0) ||
                            (mediaSearchTerms != null && Array.IndexOf(mediaSearchTerms, searchTerm) >= 0))
                        {
                            media.RankScore++;
                        }
                    }
                }

                //StringSplitOptions.RemoveEmptyEntries prevents blanks from atting to the max score
                var maxScore = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Count();
                var halfScore = (double)maxScore / 2;
                int minScore;

                if (mediaSearchResult.Media.FindIndex(x => x.RankScore == maxScore) >= 0)
                    minScore = maxScore;
                else if (mediaSearchResult.Media.FindIndex(x => x.RankScore >= halfScore) >= 0)
                    minScore = halfScore >= 1 ? Convert.ToInt16(halfScore) : 1;
                else
                    minScore = 1;

                mediaSearchResult.Media = mediaSearchResult.Media.Where(x => x.RankScore >= minScore).OrderByDescending(x => x.RankScore).ThenByDescending(x => x.DateTimeCaptured).ToList();


                return mediaSearchResult;
            }
        }

        public static List<Media> SelectLatestMedia(int loggedInUserId)
        {
            List<Media> latestMedia = new List<Media>();

            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                using (SqlCommand getLatestMedia = new SqlCommand("media_SELECT_latest", conn))
                {
                    getLatestMedia.CommandType = CommandType.StoredProcedure;
                    getLatestMedia.Parameters.Add(new SqlParameter("now", Parsnip.AdjustedTime));
                    getLatestMedia.Parameters.Add(new SqlParameter("logged_in_user_id", loggedInUserId));

                    using (SqlDataReader reader = getLatestMedia.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            latestMedia.Add(new Media(reader, loggedInUserId));
                        }
                    }
                }
            }

            return latestMedia;
        }
    }
}
