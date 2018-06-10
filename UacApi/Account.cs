using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace UacApi
{
    public class Account
    {
        public Account()
        {
            sqlConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"M:\\Users\\benba\\My Documents\\Production\\Coding\\TheParsnipWeb\\Program\\TheParsnipWeb\\App_Data\\ParsnipDb.mdf\";Integrated Security=True";
        }

        private string sqlConnectionString;

        public string email { get; set; }
        public string username { get; set; }
        public string fname { get; set; }
        public string sname { get; set; }
        public DateTime dob { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string postcode { get; set; }
        public string mobilephone { get; set; }
        public string homephone { get; set; }
        public string workphone { get; set; }

        

        public bool LogIn(string pUsername, string pPwd)
        {


            string dbPwd = null;

            using (SqlConnection conn = new SqlConnection(sqlConnectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand GetLogInDetails = new SqlCommand("SELECT Pwd FROM t_Users WHERE username = @username", conn);
                    GetLogInDetails.Parameters.Add(new SqlParameter("username", pUsername));

                    using (SqlDataReader reader = GetLogInDetails.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dbPwd = reader[0].ToString();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an exception whilst getting da password: " + e);
                }

                if (pPwd == dbPwd)
                {
                    try
                    {
                        SqlCommand GetLogInDetails = new SqlCommand("SELECT email, fname, sname, dob, address1, address2, address3, postcode, mobilephone, homephone, workphone FROM t_Users WHERE username = @username", conn);
                        GetLogInDetails.Parameters.Add(new SqlParameter("username", pUsername));

                        using (SqlDataReader reader = GetLogInDetails.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dbPwd = reader[0].ToString();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("There was an exception whilst getting da user data: " + e);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private Guid _id;
    }
}
