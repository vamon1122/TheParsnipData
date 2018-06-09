using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace UacApi
{
    class Account
    {
        public Account()
        {
            sqlConnectionString = "";
        }

        private string sqlConnectionString;

        public bool LogIn(string pUsername, string pPwd)
        {


            try
            {
                using(SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    SqlCommand GetLogInDetails = new SqlCommand("SELECT Pwd FROM table WHERE Username = @Username"); 

                }


            }
            catch (Exception)
            {

            }
        }

        private Guid _id;
    }
}
