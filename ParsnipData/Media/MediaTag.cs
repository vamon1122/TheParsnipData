﻿using System;
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
using System.Runtime.Remoting.Messaging;

namespace ParsnipData.Media
{
    public class MediaTag
    {
        public string Url { get; set; }

        

        public int Id { get; set; }
        public int CreatedById { get; set; }
        public DateTime DateCreated { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        private string _searchTerms;
        public string SearchTerms { get { return _searchTerms; } set { _searchTerms = string.IsNullOrEmpty(value) ? null : Parsnip.SanitiseSearchString(value); } }

        public static List<MediaTag> GetAllTags()
        {
            var mediaTags = new List<MediaTag>();

            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                using (SqlCommand getAlbums = new SqlCommand("media_tag_SELECT", conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = getAlbums.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            mediaTags.Add(new MediaTag(reader));
                        }
                    }
                }
            }

            return mediaTags;
        }

        public List<Media> GetAllMedia(int loggedInUserId)
        {
            List<Media> media = new List<Media>();

            using (SqlConnection conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                using (SqlCommand getMedia = new SqlCommand("media_SELECT_WHERE_media_tag_id", conn))
                {
                    getMedia.CommandType = CommandType.StoredProcedure;
                    getMedia.Parameters.Add(new SqlParameter("media_tag_id", Id));
                    getMedia.Parameters.Add(new SqlParameter("logged_in_user_id", loggedInUserId));

                    conn.Open();
                    using (SqlDataReader reader = getMedia.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            media.Add(new Media(reader, loggedInUserId));
                        }
                    }
                }

            }
            Debug.WriteLine($"{media.Count()} image(s) were found for media_tag {Name}");
            return media;
        }

        public static MediaTag Select(int mediaTagId)
        {
            using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                using(var selectMediaTagById = new SqlCommand("media_tag_SELECT_WHERE_id", conn))
                {
                    selectMediaTagById.CommandType = CommandType.StoredProcedure;

                    selectMediaTagById.Parameters.AddWithValue("id", mediaTagId);

                    conn.Open();

                    MediaTag myMediaTag = new MediaTag(mediaTagId);

                    using(var reader = selectMediaTagById.ExecuteReader())
                    {
                        if (reader.Read())
                            myMediaTag.AddValues(reader);
                        else
                            return null;
                    }

                    return myMediaTag;
                }
            }
        }

        public MediaTag(int id)
        {
            Id = id;
        }

        public MediaTag(string name) => Name = name;

        public MediaTag(SqlDataReader pReader)
        {
            AddValues(pReader);
        }

        internal bool AddValues(SqlDataReader pReader)
        {

            try
            {
                Id = (int)pReader[0];
                CreatedById = (int) pReader[1];
                DateCreated = Convert.ToDateTime(pReader[2]);

                if (pReader[3] != DBNull.Value &&
                        !string.IsNullOrEmpty(pReader[3].ToString()) &&
                        !string.IsNullOrWhiteSpace(pReader[3].ToString()))
                {
                    Name = pReader[3].ToString().Trim();
                }

                if (pReader[4] != DBNull.Value &&
                        !string.IsNullOrEmpty(pReader[4].ToString()) &&
                        !string.IsNullOrWhiteSpace(pReader[4].ToString()))
                {
                    Description = pReader[4].ToString().Trim();
                }

                try
                {
                    if (pReader.FieldCount > 5 && pReader[5] != DBNull.Value &&
                            !string.IsNullOrEmpty(pReader[5].ToString()) &&
                            !string.IsNullOrWhiteSpace(pReader[5].ToString()))
                    {
                        SearchTerms = pReader[5].ToString().Trim();
                    }
                }
                catch (IndexOutOfRangeException) { };

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst reading the Album's values: ", e);
                return false;
            }
        }

        public static void DeleteCreatedByUser(int userId)
        {
            using(var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
            {
                using (var deleteCreatedByUser = new SqlCommand("media_tag_pair_DELETE_WHERE_media_created_by_user_id", conn))
                {
                    deleteCreatedByUser.CommandType = CommandType.StoredProcedure;

                    deleteCreatedByUser.Parameters.AddWithValue("created_by_user_id", userId);
                    deleteCreatedByUser.Parameters.AddWithValue("datetime_deleted", Parsnip.AdjustedTime);

                    conn.Open();
                    deleteCreatedByUser.ExecuteNonQuery();
                }
            }
        }
    }
}
