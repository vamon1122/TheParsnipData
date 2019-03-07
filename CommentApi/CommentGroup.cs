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
    public class CommentGroup
    {
        public Guid Id { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime DateCreated { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static List<CommentGroup> GetAllCommentGroups()
        {
            bool logMe = false;

            if (logMe)
                Debug.WriteLine("----------Getting all commentGroups...");

            var commentGroups = new List<CommentGroup>();
            using (SqlConnection conn = Parsnip.GetOpenDbConnection())
            {
                SqlCommand GetCommentGroups = new SqlCommand("SELECT * FROM t_CommentGroups ORDER BY datecreated DESC", conn);
                using (SqlDataReader reader = GetCommentGroups.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        commentGroups.Add(new CommentGroup(reader));
                    }
                }
            }

            foreach (CommentGroup temp in commentGroups)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found commentGroup with id {0}", temp.Id));
            }

            return commentGroups;
        }

        public static List<Comment> GetCommentGroupsByUser(Guid pUserId)
        {
            bool logMe = true;

            if (logMe)
                Debug.WriteLine("----------Getting all commentGroups by user...");

            var commentGroups = new List<Comment>();
            using (SqlConnection conn = Parsnip.GetOpenDbConnection())
            {
                Debug.WriteLine("---------- Selecting commentGroups by user with id = " + pUserId);
                SqlCommand GetCommentGroups = new SqlCommand("SELECT * FROM t_CommentGroups WHERE createdbyid = @createdbyid ORDER BY datecreated DESC", conn);
                GetCommentGroups.Parameters.Add(new SqlParameter("createdbyid", pUserId));

                using (SqlDataReader reader = GetCommentGroups.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        commentGroups.Add(new Comment(reader));
                    }
                }
            }

            foreach (Comment temp in commentGroups)
            {
                if (logMe)
                    Debug.WriteLine(string.Format("Found commentGroup with id {0}", temp.Id));
            }

            return commentGroups;
        }

        public List<Comment> GetAllComments()
        {
            Debug.WriteLine("Getting all comments for commentGroup");
            List<Guid> CommentGuids = new List<Guid>();
            List<Comment> Comments = new List<Comment>();

            using (SqlConnection openConn = Parsnip.GetOpenDbConnection())
            {
                SqlCommand GetComments = new SqlCommand("SELECT * FROM t_Comments FULL OUTER JOIN t_CommentCommentGroupPairs ON t_Comments.id = t_CommentCommentGroupPairs.commentid WHERE t_CommentCommentGroupPairs.commentGroupid = @id ORDER BY t_Comments.datecreated DESC", openConn);
                GetComments.Parameters.Add(new SqlParameter("id", Id));

                using (SqlDataReader reader = GetComments.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Comments.Add(new Comment(reader));
                    }
                }

                Debug.WriteLine("" + CommentGuids.Count() + "Photos were found");

                int i = 0;

            }

            return Comments;
        }

        public CommentGroup(User pCreatedBy)
        {
            Id = Guid.NewGuid();
            DateCreated = Parsnip.adjustedTime;
            CreatedById = pCreatedBy.Id;
        }

        public CommentGroup(Guid pGuid)
        {
            //Debug.WriteLine("CommentGroup was initialised with the guid: " + pGuid);
            Id = pGuid;
        }

        public CommentGroup(SqlDataReader pReader)
        {
            //Debug.WriteLine("CommentGroup was initialised with an SqlDataReader. Guid: " + pReader[0]);
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
            Debug.WriteLine(string.Format("Checking weather commentGroup exists on database by using Id {0}", Id));
            try
            {
                SqlCommand findMeById = new SqlCommand("SELECT COUNT(*) FROM t_CommentGroups WHERE id = @id", pOpenConn);
                findMeById.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int commentGroupExists;

                using (SqlDataReader reader = findMeById.ExecuteReader())
                {
                    reader.Read();
                    commentGroupExists = Convert.ToInt16(reader[0]);
                    //Debug.WriteLine("Found commentGroup by Id. commentGroupExists = " + commentGroupExists);
                }

                //Debug.WriteLine(commentGroupExists + " commentGroup(s) were found with the id " + Id);

                if (commentGroupExists > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an error whilst checking if commentGroup exists on the database by using thier Id: " + e);
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

                CreatedById = new Guid(pReader[1].ToString());

                DateCreated = Convert.ToDateTime(pReader[2]);

                if (pReader[3] != DBNull.Value && !string.IsNullOrEmpty(pReader[3].ToString()) && !string.IsNullOrWhiteSpace(pReader[3].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading name");

                    Name = pReader[3].ToString().Trim();
                }
                else
                {
                    if (logMe)
                        Debug.WriteLine("----------Name is blank. Skipping name");
                }



                if (pReader[4] != DBNull.Value && !string.IsNullOrEmpty(pReader[4].ToString()) && !string.IsNullOrWhiteSpace(pReader[4].ToString()))
                {
                    if (logMe)
                        Debug.WriteLine("----------Reading description");

                    Description = pReader[4].ToString().Trim();
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
                Debug.WriteLine("There was an error whilst reading the CommentGroup's values: ", e);
                return false;
            }
        }

        private bool DbInsert(SqlConnection pOpenConn)
        {
            if (Id.ToString() == Guid.Empty.ToString())
            {
                Id = Guid.NewGuid();
                Debug.WriteLine("Id was empty when trying to insert commentGroup into the database. A new guid was generated: {0}", Id);
            }
            try
            {
                if (!ExistsOnDb(pOpenConn))
                {
                    SqlCommand InsertCommentGroupIntoDb = new SqlCommand("INSERT INTO t_CommentGroups (id, createdbyid, datecreated) VALUES(@id, @createdbyid, @datecreated)", pOpenConn);

                    InsertCommentGroupIntoDb.Parameters.Add(new SqlParameter("id", Id));
                    InsertCommentGroupIntoDb.Parameters.Add(new SqlParameter("createdbyid", CreatedById));
                    InsertCommentGroupIntoDb.Parameters.Add(new SqlParameter("datecreated", Parsnip.adjustedTime));


                    InsertCommentGroupIntoDb.ExecuteNonQuery();

                    Debug.WriteLine(String.Format("Successfully inserted commentGroup into database ({0}) ", Id));
                }
                else
                {
                    Debug.WriteLine(string.Format("----------Tried to insert commentGroup into the database but it alread existed! Id = {0}", Id));
                }
            }
            catch (Exception e)
            {
                string error = string.Format("[UacApi.CommentGroup.DbInsert)] Failed to insert commentGroup into the database: {0}", e);
                Debug.WriteLine(error);
                new LogEntry(Log.Default) { text = error };
                return false;
            }
            new LogEntry(Log.Default) { text = string.Format("CommentGroup was successfully inserted into the database!") };
            return DbUpdate(pOpenConn);
        }


        public bool Select()
        {
            return DbSelect(Parsnip.GetOpenDbConnection());
        }

        internal bool DbSelect(SqlConnection pOpenConn)
        {
            //AccountLog.Debug("Attempting to get commentGroup details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get commentGroup details with id {0}...", Id));

            try
            {
                SqlCommand SelectAccount = new SqlCommand("SELECT * FROM t_CommentGroups WHERE id = @id", pOpenConn);
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
                    //Debug.WriteLine("----------DbSelect() - Got commentGroup's details successfully!");
                    //AccountLog.Debug("Got commentGroup's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbSelect() - No commentGroup data was returned");
                    //AccountLog.Debug("Got commentGroup's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst getting commentGroup data: " + e);
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
            Debug.WriteLine("Attempting to update commentGroup with Id = " + Id);
            bool HasBeenInserted = true;

            if (HasBeenInserted)
            {
                try
                {
                    CommentGroup temp = new CommentGroup(Id);
                    temp.Select();

                    if (Name != temp.Name)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update name..."));


                        SqlCommand UpdateCommentGroupName = new SqlCommand("UPDATE t_CommentGroups SET name = @name WHERE id = @id", pOpenConn);
                        UpdateCommentGroupName.Parameters.Add(new SqlParameter("id", Id));
                        UpdateCommentGroupName.Parameters.Add(new SqlParameter("name", Name.Trim()));

                        UpdateCommentGroupName.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------Name was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Name was not changed. Not updating name."));
                    }

                    if (Description != temp.Description)
                    {
                        Debug.WriteLine(string.Format("----------Attempting to update description..."));


                        SqlCommand UpdateAlt = new SqlCommand("UPDATE t_CommentGroups SET description = @description WHERE id = @id", pOpenConn);
                        UpdateAlt.Parameters.Add(new SqlParameter("id", Id));
                        UpdateAlt.Parameters.Add(new SqlParameter("description", Description.Trim()));

                        UpdateAlt.ExecuteNonQuery();

                        Debug.WriteLine(string.Format("----------Description was updated successfully!"));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("----------Description was not changed. Not updating description."));
                    }

                }
                catch (Exception e)
                {
                    string error = string.Format("[UacApi.CommentGroup.DbUpdate] There was an error whilst updating commentGroup: {0}", e);
                    Debug.WriteLine(error);
                    new LogEntry(Log.Default) { text = error };
                    return false;
                }
                new LogEntry(Log.Default) { text = string.Format("CommentGroup was successfully updated on the database!") };
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
            //AccountLog.Debug("Attempting to get commentGroup details...");
            //Debug.WriteLine(string.Format("----------DbSelect() - Attempting to get commentGroup details with id {0}...", Id));

            try
            {
                SqlCommand deleteAccount = new SqlCommand("DELETE FROM t_CommentGroups WHERE id = @id", pOpenConn);
                deleteAccount.Parameters.Add(new SqlParameter("id", Id.ToString()));

                int recordsFound = deleteAccount.ExecuteNonQuery();
                //Debug.WriteLine(string.Format("----------DbDelete() - Found {0} record(s) ", recordsFound));

                if (recordsFound > 0)
                {
                    //Debug.WriteLine("----------DbDelete() - Got commentGroup's details successfully!");
                    //AccountLog.Debug("Got commentGroup's details successfully!");
                    return true;
                }
                else
                {
                    Debug.WriteLine("----------DbDelete() - No commentGroup data was deleted");
                    //AccountLog.Debug("Got commentGroup's details successfully!");
                    return false;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("There was an exception whilst deleting commentGroup data: " + e);
                return false;
            }
        }

    }
}
