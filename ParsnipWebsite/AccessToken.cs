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
        public string Redirect
        {
            get
            {

                var myImage = new MediaApi.Image(MediaId);
                myImage.Select();
                return string.Format("/view_image?access_token={0}", Id);
            }
        }
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
                using (SqlConnection conn = ParsnipApi.Parsnip.GetOpenDbConnection())
                {
                    var selectAccessToken = new SqlCommand("SELECT * FROM access_token WHERE access_token_id = @id", conn);
                    selectAccessToken.Parameters.Add(new SqlParameter("id", Id));

                    using (SqlDataReader reader = selectAccessToken.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AddValues(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            void AddValues(SqlDataReader reader)
            {
                Id = (Guid)reader[0];
                UserId = (Guid)reader[1];
                DateTimeCreated = (DateTime)reader[2];
                TimesUsed = (int)reader[3];
                MediaId = (Guid)reader[4];
            }
        }

        public void Insert()
        {
            try
            {
                using (SqlConnection conn = ParsnipApi.Parsnip.GetOpenDbConnection())
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
            catch (Exception ex)
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

}