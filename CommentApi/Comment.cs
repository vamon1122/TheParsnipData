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

namespace CommentApi
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Placeholder { get; set; }
        public string CommentSrc { get; set; }
        public string Classes { get; set; }
        public string Alt { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedById { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CommentGroupId { get; set; }
        public static string[] AllowedFileExtensions = new string[] { "png", "gif", "jpg", "jpeg", "tiff" };

        public static bool IsValidFileExtension(string pExtension)
        {
            return AllowedFileExtensions.Contains(pExtension);
        }

        Log DebugLog = new Log("Debug");

        public List<Guid> CommentGroupIds()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all comment's commentGroup Ids...");

            var commentGroupIds = new List<Guid>();
            using (SqlConnection conn = Parsnip.GetOpenDbConnection())
            {
                SqlCommand GetComments = new SqlCommand("SELECT commentGroupid FROM t_CommentCommentGroupPairs WHERE commentid = @commentid", conn);
                GetComments.Parameters.Add(new SqlParameter("commentid", Id));

                using (SqlDataReader reader = GetComments.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader[0].ToString() != Guid.Empty.ToString())
                        {
                            new LogEntry(DebugLog) { text = "An commentGroup guid was found! = " + reader[0].ToString() };
                            commentGroupIds.Add(new Guid(reader[0].ToString()));
                        }
                        else
                        {
                            new LogEntry(DebugLog) { text = "A BLANK commentGroup guid was found! Not adding Guid = " + reader[0].ToString() };
                        }

                    }
                }
            }

            return commentGroupIds;
        }

        public static List<Comment> GetAllComments()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all comments...");

            var comments = new List<Comment>();
            using (SqlConnection conn = Parsnip.GetOpenDbConnection())
            {
                SqlCommand GetComments = new SqlCommand("SELECT * FROM t_Comments ORDER BY datecreated DESC", conn);
                using (SqlDataReader reader = GetComments.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comments.Add(new Comment(reader));
                    }
                }
            }

            foreach (Comment temp in comments)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found comment with id {0}", temp.Id));
            }

            return comments;
        }

        public static List<Comment> GetCommentsByUser(Guid pUserId)
        {
            bool logMe = true;

            if (logMe)
                Debug.WriteLine("----------Getting all comments by user...");

            var comments = new List<Comment>();
            using (SqlConnection conn = Parsnip.GetOpenDbConnection())
            {
                Debug.WriteLine("---------- Selecting comments by user with id = " + pUserId);
                SqlCommand GetComments = new SqlCommand("SELECT * FROM t_Comments WHERE createdbyid = @createdbyid ORDER BY datecreated DESC", conn);
                GetComments.Parameters.Add(new SqlParameter("createdbyid", pUserId));

                using (SqlDataReader reader = GetComments.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comments.Add(new Comment(reader));
                    }
                }
            }

            foreach (Comment temp in comments)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found comment with id {0}", temp.Id));
            }

            return comments;
        }


        public static List<Comment> GetCommentsNotInAnCommentGroup()
        {
            throw new NotImplementedException();
        }

        public Comment(string pSrc, User pCreatedBy, CommentGroup pCommentGroup)
        {
            Id = Guid.NewGuid();
            CommentSrc = pSrc;
            Log DebugLog = new Log("Debug");
            new LogEntry(DebugLog) { text = "Comment created with commentGroupid = " + pCommentGroup.Id };
            CommentGroupId = pCommentGroup.Id;
            DateCreated = Parsnip.adjustedTime;
            CreatedById = pCreatedBy.Id;
        }

        public Comment(Guid pGuid)
        {
            //Debug.WriteLine("Comment was initialised with the guid: " + pGuid);
            Id = pGuid;
        }

        public Comment(SqlDataReader pReader)
        {
            //Debug.WriteLine("Comment was initialised with an SqlDataReader. Guid: " + pReader[0]);
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
            Debug.WriteLine(string.Format("Checking weather comment exists on database by using Id {0}", Id));
            try
            {
                SqlCommand findMeById = new SqlCommand("SELECT COUNT(*) FROM t_Comments WHERE id = @id", pOpenConn);
                findMeById.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int commentExists;

                using (SqlDataReader reader = findMeById.ExecuteReader())
                {
                    reader.Read();
                    commentExists = Convert.ToInt16(reader[0]);
                    //Debug.WriteLine("Found comment by Id. commentExists = " + commentExists);
                }

                //Debug.WriteLine(commentExists + " comment(s) were found with the id " + Id);

                if (commentExists > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst checking if comment exists on the database by using thier Id: " + e);
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
                    Debug.WriteLine("----------Reading CommentSrc");
                CommentSrc = pReader[2].ToString().Trim();



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
                    Debug.WriteLine("----------Reading datecreated");
                DateCreated = Convert.ToDateTime(pReader[4]);

                if (logMe)
                    Debug.WriteLine("----------Reading createdbyid");
                CreatedById = new Guid(pReader[5].ToString());

                if (pReader[6] != DBNull.Value && !string.IsNullOrEmpty(pReader[6].ToString()) && !string.IsNullOrWhiteSpace(pReader[6].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading title");

                    Title = pReader[6].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Title is blank. Skipping title");
                }

                if (pReader[7] != DBNull.Value && !string.IsNullOrEmpty(pReader[7].ToString()) && !string.IsNullOrWhiteSpace(pReader[7].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading description");

                    Description = pReader[7].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Description is blank. Skipping description");
                }

                if (logMe)
                    Debug.WriteLine("added values successfully!");

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst reading the Comment's values: ", e);
                return false;
            }
        }

        private bool DbInsert(SqlConnection pOpenConn)
        {
            if (Id.ToString() == Guid.Empty.ToString())
            {
                Id = Guid.NewGuid();
                Debug.WriteLine("Id was empty when trying to insert comment into the database. A new guid was generated: {0}", Id);
            }

            if (CommentSrc != null)
            {
                try
                {
                    if (!ExistsOnDb(pOpenConn))
                    {
                        SqlCommand InsertCommentIntoDb = new SqlCommand("INSERT INTO t_Comments (id, commentsrc, datecreated, createdbyid) VALUES(@id, @commentsrc, @datecreated, @createdbyid)", pOpenConn);

                        InsertCommentIntoDb.Parameters.Add(new SqlParameter("id", Id));
                        InsertCommentIntoDb.Parameters.Add(new SqlParameter("commentsrc", CommentSrc.Trim()));
                        InsertCommentIntoDb.Parameters.Add(new SqlParameter("datecreated", Parsnip.adjustedTime));
                        InsertCommentIntoDb.Parameters.Add(new SqlParameter("createdbyid", CreatedById));

                        InsertCommentIntoDb.ExecuteNonQuery();

                        Debug.WriteLine(String.Format("Successfully inserted comment into database ({0}) ", Id));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Tried to insert comment into the database but it alread existed! Id = {0}", Id));
                    }
                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.Comment.DbInsert)] Failed to insert comment into the database: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("Comment was successfully inserted into the database!") };
                return DbUpdate(pOpenConn);
            }
            else
            {
                throw new InvalidOperationException("Comment cannot be inserted. The comment's property: commentsrc must be initialised before it can be inserted!");
            }
        }

        public bool Select()
        {
            return DbSelect(Parsnip.GetOpenDbConnection());
        }

        internal bool DbSelect(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get comment details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get comment details with id {0}...", Id));

            try
            {
                SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_Comments WHERE id = @id", pOpenConn);
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
                    //Debug.WriteLine("----------DbSelect() - Got comment's details successfully!");
                    //AccountLog.Debug("Got comment's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbSelect() - No comment data was returned");
                    //AccountLog.Debug("Got comment's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst getting comment data: " + e);
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
            Debug.WriteLine("Attempting to update comment with Id = " + Id);
            bool HasBeenInserted = true;

            if (HasBeenInserted)
            {
                try
                {
                    Comment temp = new Comment(Id);
                    temp.Select();

                    if (CommentGroupId != null && CommentGroupId.ToString() != Guid.Empty.ToString())
                    {
                        Log DebugLog = new Log("Debug");
                        new LogEntry(DebugLog) { text = "CommentGroupId != null = " + CommentGroupId };

                        SqlCommand DeleteOldPairs = new SqlCommand("DELETE FROM t_CommentCommentGroupPairs WHERE commentid = @commentid", pOpenConn);
                        DeleteOldPairs.Parameters.Add(new SqlParameter("commentid", Id));
                        DeleteOldPairs.ExecuteNonQuery();

                        SqlCommand CreatePhotoCommentGroupPair = new SqlCommand("INSERT INTO t_CommentCommentGroupPairs VALUES (@commentid, @commentGroupid)", pOpenConn);
                        CreatePhotoCommentGroupPair.Parameters.Add(new SqlParameter("commentid", Id));
                        CreatePhotoCommentGroupPair.Parameters.Add(new SqlParameter("commentGroupid", CommentGroupId));

                        CreatePhotoCommentGroupPair.ExecuteNonQuery();
                        new LogEntry(DebugLog) { text = string.Format("INSERTED ALBUM PAIR {0}, {1} ", Id, CommentGroupId) };
                    }
                    else
                    {
                        Log DebugLog = new Log("Debug");
                        new LogEntry(DebugLog) { text = "Comment created with commentGroupid = null :( " };
                    }


                    if (Placeholder != temp.Placeholder)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update placeholder..."));


                        SqlCommand UpdatePlaceholder = new SqlCommand("UPDATE t_Comments SET placeholder = @placeholder WHERE id = @id", pOpenConn);
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


                        SqlCommand UpdateAlt = new SqlCommand("UPDATE t_Comments SET alt = @alt WHERE id = @id", pOpenConn);
                        UpdateAlt.Parameters.Add(new SqlParameter("id", Id));
                        UpdateAlt.Parameters.Add(new SqlParameter("alt", Alt.Trim()));

                        UpdateAlt.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------alt was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------alt was not changed. Not updating alt."));
                    }

                    if (Title != temp.Title)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update title..."));


                        SqlCommand UpdateTitle = new SqlCommand("UPDATE t_Comments SET title = @title WHERE id = @id", pOpenConn);
                        UpdateTitle.Parameters.Add(new SqlParameter("id", Id));
                        UpdateTitle.Parameters.Add(new SqlParameter("title", Title.Trim()));

                        UpdateTitle.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------Title was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Title was not changed. Not updating title."));
                    }

                    if (Description != temp.Description)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update description..."));


                        SqlCommand UpdateDescription = new SqlCommand("UPDATE t_Comments SET description = @description WHERE id = @id", pOpenConn);
                        UpdateDescription.Parameters.Add(new SqlParameter("id", Id));
                        UpdateDescription.Parameters.Add(new SqlParameter("description", Description.Trim()));

                        UpdateDescription.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------Description was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Description was not changed. Not updating description."));
                    }

                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.Comment.DbUpdate] There was an error whilst updating comment: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("Comment was successfully updated on the database!") };
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
            //AccountLog.Debug("Attempting to get comment details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get comment details with id {0}...", Id));

            try
            {
                new LogEntry(DebugLog) { text = "Attempting to delete uploaded photo id = " + Id };

                using (SqlConnection conn = Parsnip.GetOpenDbConnection())
                {
                    SqlCommand DeleteComment = new SqlCommand("DELETE iap FROM t_CommentCommentGroupPairs iap FULL OUTER JOIN t_Comments ON commentid = t_Comments.id  WHERE t_Comments.id = @commentid", conn);
                    DeleteComment.Parameters.Add(new SqlParameter("commentid", Id));
                    int recordsAffected = DeleteComment.ExecuteNonQuery();

                    new LogEntry(DebugLog) { text = string.Format("{0} record(s) were affected", recordsAffected) };
                }
            }
            catch (Exception err)
            {

                new LogEntry(DebugLog) { text = "There was an exception whilst DELETING the photo: " + err };
                return false;
            }
            new LogEntry(DebugLog) { text = "Successfully deleted photo with id = " + Id };
            return true;

            /*
            try
            {
                SqlCommand deleteAccount = new SqlCommand("DELETE FROM t_Comments WHERE id = @id", pOpenConn);
                deleteAccount.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int recordsFound = deleteAccount.ExecuteNonQuery();
                //Debug.WriteLine(string.Format("----------DbDelete() - Found {0} record(s) ", recordsFound));

                if (recordsFound > 0)
                {
                    //Debug.WriteLine("----------DbDelete() - Got comment's details successfully!");
                    //AccountLog.Debug("Got comment's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbDelete() - No comment data was deleted");
                    //AccountLog.Debug("Got comment's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst deleting comment data: " + e);
                return false;
            }
            */
        }

    }
}
