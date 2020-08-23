using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsnipData.Media
{
    public class SequencedVideo : Video
    {
        public int Position { get; set; }
        
        public SequencedVideo(SqlDataReader pReader, int loggedInUserId = default) : base()
        {
            AddValues(pReader, loggedInUserId);
        }

        protected override bool AddValues(SqlDataReader reader, int loggedInUserId)
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
                Position = (int)reader[17];

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("There was an error whilst reading the media's values: " + ex);
                return false;
            }
        }
    }

    public class VideoSequence
    {
        public Video Video { get; set; }

        public List<SequencedVideo> SequencedVideos { get; set; }

        public VideoSequence()
        {

        }

        public VideoSequence(Video video)
        {
            Video = video;
        }

        public bool Insert()
        {
            if(Video == null || SequencedVideos.Count() < 2)
            {
                return false;
            }

            try
            {
                using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (var insertVideoSequence = new SqlCommand("video_sequence_INSERT", conn))
                    {
                        insertVideoSequence.CommandType = CommandType.StoredProcedure;

                        insertVideoSequence.Parameters.AddWithValue("media_id", Video.Id);
                        insertVideoSequence.Parameters.AddWithValue("title", Video.Title);
                        insertVideoSequence.Parameters.AddWithValue("video_original_dir", Video.VideoData.Original);
                        insertVideoSequence.Parameters.AddWithValue("thumbnail_original_dir", Video.Original);
                        insertVideoSequence.Parameters.AddWithValue("thumbnail_compressed_dir", Video.Compressed);
                        insertVideoSequence.Parameters.AddWithValue("thumbnail_placeholder_dir", Video.Placeholder);
                        insertVideoSequence.Parameters.AddWithValue("date_time_captured", Video.DateTimeCaptured);
                        insertVideoSequence.Parameters.AddWithValue("media_x_scale", Video.XScale);
                        insertVideoSequence.Parameters.AddWithValue("media_y_scale", Video.YScale);
                        if (Video.AlbumId != default)
                            insertVideoSequence.Parameters.AddWithValue("media_tag_id", Video.AlbumId);
                        insertVideoSequence.Parameters.AddWithValue("created_by_user_id", Video.CreatedById);
                        insertVideoSequence.Parameters.AddWithValue("datetime_now", Parsnip.AdjustedTime);
                        DataTable sequencedVideoIds = new DataTable();
                        sequencedVideoIds.Columns.Add("original_media_id", typeof(string));
                        sequencedVideoIds.Columns.Add("position", typeof(int));
                        foreach (var video in SequencedVideos.OrderBy(v => v.Position))
                        {
                            sequencedVideoIds.Rows.Add(video.Id, video.Position);
                        }
                        var sequencedVideoIdsParam = insertVideoSequence.Parameters.AddWithValue("sequenced_video_ids", sequencedVideoIds);
                        sequencedVideoIdsParam.SqlDbType = SqlDbType.Structured;

                        conn.Open();
                        insertVideoSequence.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"There was an exception whilst inserting the video sequence: {ex}");
                return false;
            }
        }

        public static VideoSequence Select(string mediaId, int loggedInUserId)
        {
            var VideoSequence = new VideoSequence();

            if (mediaId != null)
            {
                try
                {
                    using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                    {
                        using (var selectVideoSequence = new SqlCommand("video_sequence_SELECT", conn))
                        {
                            selectVideoSequence.CommandType = CommandType.StoredProcedure;

                            selectVideoSequence.Parameters.AddWithValue("media_id", mediaId);

                            conn.Open();
                            using(var reader = selectVideoSequence.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    VideoSequence.Video = new Video(reader, loggedInUserId);
                                }
                                reader.NextResult();
                                while (reader.Read())
                                {
                                    VideoSequence.SequencedVideos.Add(new SequencedVideo(reader, loggedInUserId));
                                }

                            }
                        }
                    }
                }
                catch 
                {
                    VideoSequence = null;
                }
            }
            return VideoSequence;
        }

        public static VideoSequence SelectOldestUnstitchedVideoSequence()
        {
            var VideoSequence = new VideoSequence();

            try
            {
                using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                {
                    using (var selectVideoSequence = new SqlCommand("video_sequence_SELECT_WHERE_unstitched", conn))
                    {
                        selectVideoSequence.CommandType = CommandType.StoredProcedure;

                        conn.Open();
                        using (var reader = selectVideoSequence.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VideoSequence = new VideoSequence(new Video(reader));
                            }

                            reader.NextResult();
                            while (reader.Read())
                            {
                                VideoSequence.SequencedVideos.Add(new SequencedVideo(reader));
                            }

                        }
                    }
                }
            }
            catch
            {
                VideoSequence = null;
            }
            return VideoSequence;
        }

        public bool Delete()
        {
            if(Video == null || Video.Id == null)
            {
                return false;
            }
            else
            {
                try
                {
                    using (var conn = new SqlConnection(Parsnip.ParsnipConnectionString))
                    {
                        using (var deleteVideoSequence = new SqlCommand("video_sequence_DELETE", conn))
                        {
                            deleteVideoSequence.CommandType = CommandType.StoredProcedure;

                            deleteVideoSequence.Parameters.AddWithValue("media_id", Video.Id);

                            conn.Open();
                            deleteVideoSequence.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
