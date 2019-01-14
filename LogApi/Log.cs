using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;

namespace LogApi
{
    public class Log
    {
        private bool isNew;
        public Guid Id { get; private set; }
        public DateTime DateTimeCreated { get; private set; }
        public string Name { get; set;  }


        public Log(SqlDataReader pReader)
        {
            AddValues(pReader);
        }

        public Log(Guid pLogId)
        {
            isNew = true;
            Id = pLogId;
            DateTimeCreated = ParsnipApi.Data.adjustedTime;

        }

        public Log(string pName)
        {
            using (SqlConnection openConn = ParsnipApi.Data.GetOpenDbConnection())
            {
                Name = pName;

                if (NameExists(openConn))
                {
                    SelectByName(openConn);
                }
                else
                {
                    Id = Guid.NewGuid();
                    DateTimeCreated = ParsnipApi.Data.adjustedTime;
                    Insert();
                }
            }
        }

        internal bool AddValues(SqlDataReader pReader)
        {
            try
            {
                isNew = false;
                Id = new Guid(pReader[0].ToString());
                DateTimeCreated = Convert.ToDateTime(pReader[1].ToString());
                Name = pReader[2].ToString();
            }
            catch(Exception e)
            {
                Debug.WriteLine("Exception whilst adding values to a log: " + e);
                return false;
            }
            return true;
        }

        public bool Select()
        {
            using (SqlConnection openConnection = ParsnipApi.Data.GetOpenDbConnection())
            {
                if (Id != null && Id != Guid.Empty)
                {
                    if (IdExists(openConnection))
                        return SelectById(openConnection);
                    else
                        return false;

                }

                if (Name != null && Name != "")
                {
                    if (NameExists(openConnection))
                        return SelectByName(openConnection);
                    else
                        return false;
                }

                return false;

                
            }
        }

        bool SelectById(SqlConnection pOpenConn)
        {
            try
            {
                SqlCommand selectLog = new SqlCommand("SELECT * FROM t_Logs WHERE id = @id", pOpenConn);
                selectLog.Parameters.Add(new SqlParameter("id", Id));

                AddValues(selectLog.ExecuteReader());
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception whilst selecting log by id: " + e);
                return false;
            }
            return true;
        }

        bool SelectByName(SqlConnection pOpenConn)
        {
            try
            {
                SqlCommand selectLog = new SqlCommand("SELECT * FROM t_Logs WHERE name = @name", pOpenConn);
                selectLog.Parameters.Add(new SqlParameter("name", Name));

                AddValues(selectLog.ExecuteReader());
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception whilst selecting log by name: " + e);
                return false;
            }
            return true;
        }

        public bool Exists()
        {
            using (SqlConnection openConn = ParsnipApi.Data.GetOpenDbConnection())
            {
                if (IdExists(openConn))
                {
                    Debug.WriteLine("The log Id already existed!");
                }

                if (NameExists(openConn))
                {
                    Debug.WriteLine("The log name already existed!");
                }

                return true;
            }
        }

        private bool IdExists(SqlConnection pOpenConn)
        {
            try
            {
                SqlCommand doesLogIdExist = new SqlCommand("SELECT COUNT(*) FROM t_Logs WHERE id = @id", pOpenConn);
                doesLogIdExist.Parameters.Add(new SqlParameter("id", Id));

                int logsFound;
                using (SqlDataReader reader = doesLogIdExist.ExecuteReader())
                {
                    reader.Read();
                    logsFound = Convert.ToInt16(reader[0]);
                }

                if (logsFound > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception whilst checking if log exists " + e);
                return false;
            }
        }

        private bool NameExists(SqlConnection pOpenConn)
        {
            try
            {
                SqlCommand doesLogNameExist = new SqlCommand("SELECT COUNT(*) FROM t_Logs WHERE name = @name", pOpenConn);
                doesLogNameExist.Parameters.Add(new SqlParameter("name", Name));

                int logsFound;
                using (SqlDataReader reader = doesLogNameExist.ExecuteReader())
                {
                    reader.Read();
                    logsFound = Convert.ToInt16(reader[0]);
                }

                if (logsFound > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception whilst checking if log exists " + e);
                return false;
            }
        }







        private bool Insert()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ParsnipApi.Data.sqlConnectionString))
                {
                    conn.Open();

                    SqlCommand insertLog = new SqlCommand("INSERT INTO t_Logs (id, dateTime, name) VALUES(@id, @dateTime, @name)", conn);
                    insertLog.Parameters.Add(new SqlParameter("id", Id));
                    insertLog.Parameters.Add(new SqlParameter("dateTime", DateTimeCreated));
                    insertLog.Parameters.Add(new SqlParameter("name", Name));

                    insertLog.ExecuteNonQuery();
                }

            }    
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }
    }
}
