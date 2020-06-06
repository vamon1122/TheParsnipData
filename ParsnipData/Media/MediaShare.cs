using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ParsnipData.Accounts;
using ParsnipData.Media;
using ParsnipData.Logging;
using System.Data.SqlClient;
using ParsnipData;
using System.Diagnostics;
using System.Data;

namespace ParsnipData.Media
{
    public class MediaShareId : ParsnipId
    {
        public MediaShareId() : base() { }
        public MediaShareId(string mediaShareId) : base(mediaShareId) { }

        public static MediaShareId NewMediaShareId()
        {
            return new MediaShareId(NewParsnipIdString());
        }
    }
    public class MediaShare
    {
        public MediaShareId Id { get; private set; }
        public int UserId { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public int TimesUsed { get; set; }
        public MediaId MediaId { get; set; }

        public static DataTable GetStats()
        {
            var allStats = new DataTable();

            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                conn.Open();
                var getMediaStats = new SqlCommand(
                    "media_SELECT_share_stats", conn);

                using (var mediaStats = getMediaStats.ExecuteReader())
                {
                    allStats.Load(mediaStats);
                }

                //allStats.DefaultView.Sort = "times_used DESC";
                allStats = allStats.DefaultView.ToTable();
            }

            return allStats;
        } 

        public MediaShare(SqlDataReader reader)
        {
            AddValues(reader);
        }

        public MediaShare(MediaId mediaId, int userId)
        {
            Id = MediaShareId.NewMediaShareId();
            UserId = userId;
            DateTimeCreated = ParsnipData.Parsnip.AdjustedTime;
            TimesUsed = 0;
            MediaId = mediaId;
        }

        public MediaShare(MediaShareId id)
        {
            Id = id;
        }

        public MediaShare(MediaShareId mediaShareId, int createdByUserId, DateTime dateTimeCreated, int timesUsed, MediaId mediaId)
        {
            Id = mediaShareId;
            UserId = createdByUserId;
            DateTimeCreated = dateTimeCreated;
            TimesUsed = timesUsed;
            MediaId = mediaId;
        }

        public void Select()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using(var selectAccessToken = new SqlCommand("media_share_SELECT_WHERE_id", conn))
                    {
                        selectAccessToken.CommandType = CommandType.StoredProcedure;
                        selectAccessToken.Parameters.Add(new SqlParameter("id", Id.ToString()));
                        conn.Open();
                        using (SqlDataReader reader = selectAccessToken.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AddValues(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void AddValues(SqlDataReader reader)
        {
            Id = new MediaShareId((string)reader[0]);
            UserId = (int)reader[1];
            DateTimeCreated = (DateTime)reader[2];
            TimesUsed = (int)reader[3];
            MediaId = new MediaId((string)reader[4]);
        }

        public void Insert()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (var insertAccessToken = new SqlCommand("media_share_INSERT", conn))
                    {

                        insertAccessToken.CommandType = CommandType.StoredProcedure;
                        insertAccessToken.Parameters.Add(new SqlParameter("id", Id.ToString()));
                        insertAccessToken.Parameters.Add(new SqlParameter("created_by_user_id", User.LogIn().Id));
                        insertAccessToken.Parameters.Add(new SqlParameter("datetime_created", DateTimeCreated));
                        insertAccessToken.Parameters.Add(new SqlParameter("media_id", MediaId.ToString()));

                        conn.Open();
                        insertAccessToken.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                new LogEntry(Log.Debug) { text = $"There was an exception whilst inserting the media share: {ex}" };
            }
        }

        public void View(User viewedBy)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {

                    using (var updateMediaShare = new SqlCommand("media_share_view_INSERT", conn))
                    {
                        updateMediaShare.CommandType = CommandType.StoredProcedure;
                        updateMediaShare.Parameters.Add(new SqlParameter("id", Id.ToString()));
                        if(viewedBy != null && !string.IsNullOrEmpty(viewedBy.Id.ToString()))
                            updateMediaShare.Parameters.Add(new SqlParameter("created_by_user_id", viewedBy.Id));
                        
                        updateMediaShare.Parameters.Add(new SqlParameter("date_time_created", Parsnip.AdjustedTime));

                        conn.Open();
                        updateMediaShare.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            TimesUsed++;
        }
    }
}