using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UacApi;
using MediaApi;
using LogApi;
using System.Data.SqlClient;
using ParsnipApi;
using System.Diagnostics;

namespace ParsnipWebsite
{
    public class AccessToken
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public int TimesUsed { get; set; }
        public string Redirect { get {

                var myImage = new MediaApi.Image(MediaId);
                myImage.Select();
                return string.Format("/view_image?access_token={0}", Id); } }
        public Guid MediaId { get; set; }

        public static bool TokenExists(Guid userId, Guid mediaId)
        {
            try
            {
                using (SqlConnection conn = ParsnipApi.Parsnip.GetOpenDbConnection())
                {
                    var selectAccessToken = new SqlCommand("SELECT access_token_id FROM access_token WHERE user_id = @user_id AND media_id = @media_id", conn);
                    selectAccessToken.Parameters.Add(new SqlParameter("user_id", userId));
                    selectAccessToken.Parameters.Add(new SqlParameter("media_id", mediaId));

                    using (SqlDataReader reader = selectAccessToken.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

        public static AccessToken GetToken(Guid userId, Guid mediaId)
        {
            AccessToken myToken = null;
            try
            {
                using (SqlConnection conn = ParsnipApi.Parsnip.GetOpenDbConnection())
                {
                    var selectAccessToken = new SqlCommand("SELECT access_token_id FROM access_token WHERE user_id = @user_id AND media_id = @media_id", conn);
                    selectAccessToken.Parameters.Add(new SqlParameter("user_id", userId));
                    selectAccessToken.Parameters.Add(new SqlParameter("media_id", mediaId));

                    using (SqlDataReader reader = selectAccessToken.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            myToken = new AccessToken((Guid)reader[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (myToken == null)
                throw new NullReferenceException();

            myToken.Select();

            return myToken;
        }

        public AccessToken(Guid userId, Guid mediaId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            DateTimeCreated = ParsnipApi.Parsnip.adjustedTime;
            TimesUsed = 0;
            MediaId = mediaId;
        }

        public AccessToken(Guid id)
        {
            Id = id;
        }

        public void Select()
        {
            try
            {
                using(SqlConnection conn = ParsnipApi.Parsnip.GetOpenDbConnection())
                {
                    var selectAccessToken = new SqlCommand("SELECT * FROM access_token WHERE access_token_id = @id", conn);
                    selectAccessToken.Parameters.Add(new SqlParameter("id", Id));

                    using(SqlDataReader reader = selectAccessToken.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AddValues(reader);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            void AddValues(SqlDataReader reader)
            {
                Id = (Guid)reader[0];
                UserId = (Guid)reader[1];
                DateTimeCreated = (DateTime) reader[2];
                TimesUsed = (int) reader[3];
                MediaId = (Guid) reader[4];
            }
        }

        public void Insert()
        {
            try
            {
                using(SqlConnection conn = ParsnipApi.Parsnip.GetOpenDbConnection())
                {
                    var insertAccessToken = new SqlCommand("INSERT INTO access_token VALUES (@access_token_id, @user_id, @date_time_created, @times_used, @media_id)", conn);
                    insertAccessToken.Parameters.Add(new SqlParameter("access_token_id", Id));
                    insertAccessToken.Parameters.Add(new SqlParameter("user_id", UserId));
                    insertAccessToken.Parameters.Add(new SqlParameter("date_time_created", DateTimeCreated));
                    insertAccessToken.Parameters.Add(new SqlParameter("times_used", TimesUsed));
                    insertAccessToken.Parameters.Add(new SqlParameter("media_id", MediaId));

                    insertAccessToken.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void Update()
        {
            try
            {
                using (SqlConnection conn = ParsnipApi.Parsnip.GetOpenDbConnection())
                {
                    var updateAccessToken = new SqlCommand("UPDATE access_token SET times_used = @times_used WHERE access_token_id = @access_token_id", conn);
                    updateAccessToken.Parameters.Add(new SqlParameter("access_token_id", Id));
                    updateAccessToken.Parameters.Add(new SqlParameter("times_used", TimesUsed));

                    updateAccessToken.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    

    public partial class View_Image : System.Web.UI.Page
    {
        User myUser;
        Log DebugLog = new Log("Debug");
        MediaApi.Image myImage;
        protected void Page_Load(object sender, EventArgs e)
        {
            //We secure the page using the UacApi. 
            //This ensures that the user is logged in etc
            //You only need to change where it says '_NEW TEMPLATE'.
            //Change this to match your page name without the '.aspx' extension.

            


            //If there is an access token, get the token & it's data.
            //If there is no access token, check that the user is logged in.
            if (Request.QueryString["access_token"] != null)
            {
                
                var myAccessToken = new AccessToken(new Guid(Request.QueryString["access_token"]));
                try
                {
                    myAccessToken.Select();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                myAccessToken.TimesUsed++;

                User createdBy = new User(myAccessToken.UserId);
                createdBy.Select();

                

                myAccessToken.Update();

                myImage = new MediaApi.Image(myAccessToken.MediaId);
                myImage.Select();

                new LogEntry(DebugLog) { text = string.Format("{0}'s link to {1} got another hit! Now up to {2}", createdBy.FullName, myImage.Title, myAccessToken.TimesUsed) };
            }
            else
            {
                if (Request.QueryString["imageid"] == null)
                    myUser = Uac.SecurePage("view_image", this, Data.DeviceType);
                else
                    myUser = Uac.SecurePage("view_image?imageid=" + Request.QueryString["imageid"], this, Data.DeviceType);

                

                if (Request.QueryString["imageid"] == null)
                    Response.Redirect("home");

                myImage = new MediaApi.Image(new Guid(Request.QueryString["imageid"]));
                myImage.Select();
            }

           //Get the image which the user is trying to access, and display it on the screen.
            

            Debug.WriteLine(string.Format("AlbumId {0}", myImage.AlbumId));

            

            //If the image has been deleted, display a warning.
            //If the image has not been deleted, display the image.
            if (myImage.AlbumId == Guid.Empty)
            {
                Debug.WriteLine(string.Format("AlbumId {0} == {1}", myImage.AlbumId, Guid.Empty));
                NotExistError.Visible = true;
                Button_ViewAlbum.Visible = false;
            }
            else
            {
                Debug.WriteLine(string.Format("AlbumId {0} != {1}", myImage.AlbumId, Guid.Empty));

                ImageTitle.InnerText = myImage.Title;
                Page.Title = myImage.Title;
                ImagePreview.ImageUrl = myImage.ImageSrc;   
            }

            //If there was no access token, the user is trying to share the photo.
            //Generate a shareable link and display it on the screen.
            if (Request.QueryString["access_token"] == null)
            {
                Button_ViewAlbum.Visible = false;

                AccessToken myAccessToken;

                if (AccessToken.TokenExists(myUser.Id, myImage.Id))
                {
                    myAccessToken = AccessToken.GetToken(myUser.Id, myImage.Id);
                }
                else
                {
                    myAccessToken = new AccessToken(myUser.Id, myImage.Id);
                    myAccessToken.Insert();
                }

                //Gets URL without sub pages
                ShareLink.Value = Request.Url.GetLeftPart(UriPartial.Authority) + myAccessToken.Redirect;
            }
            else
            {
                ShareLinkContainer.Visible = false;
            }

        }

        protected void Button_ViewAlbum_Click(object sender, EventArgs e)
        {
            switch (myImage.AlbumId.ToString().ToUpper())
            {
                case "4B4E450A-2311-4400-AB66-9F7546F44F4E":
                    Debug.WriteLine("Redirecting to photos");
                    Response.Redirect("~/photos?imageid=" + myImage.Id);
                    
                    break;

                case "5F15861A-689C-482A-8E31-2F13429C36E5":
                    Debug.WriteLine("Redirecting to memes");
                    Response.Redirect("~/memes?imageid=" + myImage.Id);
                    break;
                default:
                    Debug.WriteLine("Album was wrong! Album = " + myImage.AlbumId.ToString());
                    break;
            }
        }
    }
}