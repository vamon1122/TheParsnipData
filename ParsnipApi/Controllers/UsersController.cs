using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ParsnipApi.Models;
using System.Diagnostics;
using System.Data.SqlClient;

namespace ParsnipApi.Controllers
{
    public class UsersController : ApiController
    {
        public static List<User> GetAllUsers()
        {
         
            Debug.WriteLine("---------- [User Controller] Getting all users ...");
            var users = new List<User>();
            try
            {
                
                using (SqlConnection conn = Parsnip.GetOpenDbConnection())
                {
                    SqlCommand GetUsers = new SqlCommand("SELECT * FROM t_Users", conn);
                    using (SqlDataReader reader = GetUsers.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User(reader));
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine("---------- [User Controller] There was an exception whilst getting all users: " + e);
                throw e;
            }

            return users;
        }

        
        
        

        public static User LogIn(string pUsername, string pPassword)
        {
            using (var openConn = Parsnip.GetOpenDbConnection())
            {
                try
                {
                    SqlCommand getId = new SqlCommand("SELECT * FROM t_Users WHERE username = @username AND password = @password", openConn);
                    getId.Parameters.Add(new SqlParameter("username", pUsername));

                    using (SqlDataReader reader = getId.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new User(reader);
                        }
                    }
                    throw new InvalidOperationException("There is no user with this username / password combination");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[LogIn] There was an exception whilst getting the id from the database: " + e);
                    throw new InvalidOperationException("There was an error whilst finding a user with username / password combination");
                }
            }
        }
    }
}
