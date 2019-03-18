using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ParsnipApi.Models;
using System.Diagnostics;
using System.Data.SqlClient;
using ParsnipApiDataAccess;
using BenLog;

namespace ParsnipApi.Controllers
{
    public class UsersController : ApiController
    {
        readonly LogWriter AsyncLog = new LogWriter("Async_Login.txt", @"C:\Users\ben.2ESKIMOS\Documents\GitHub\TheParsnipWeb");

        public IHttpActionResult Get(string username, string password)
        {
            {
                using (ParsnipTestDbEntities entities = new ParsnipTestDbEntities())
                {
                    t_Users myUser = entities.t_Users.FirstOrDefault(u => u.username == username && u.password == password);

                    return Ok(myUser);
                }
            }
        }
        

        public IEnumerable<t_Users> Get()
        {
            using(ParsnipTestDbEntities entities = new ParsnipTestDbEntities())
            {
                return entities.t_Users.ToList();
            }
        }


        /*
        public IEnumerable<User> GetAllUsers()
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
        */


            /*
        public IHttpActionResult LogIn(string pUsername, string pPassword)
        {
            return Ok(new User() { Username = pUsername, _pwd = pPassword, _forename = pUsername + "'s Forename'"});
            using (var openConn = Parsnip.GetOpenDbConnection())
            {
                try
                {
                    Debug.WriteLine(string.Format("[UserController] Finding user username = {0} password = {1}", pUsername, pPassword));
                    SqlCommand getUser = new SqlCommand("SELECT * FROM t_Users WHERE username = @username AND password = @password", openConn);
                    getUser.Parameters.Add(new SqlParameter("username", pUsername));
                    getUser.Parameters.Add(new SqlParameter("password", pPassword));

                    using (SqlDataReader reader = getUser.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User temp = new User(reader);
                            Debug.WriteLine("[UserController] Found a user. Username = {0} Password = {1} Forename = {2}", pUsername, pPassword, temp._forename);
                            Ok(temp);
                        }
                    }
                    throw new InvalidOperationException("There is no user with this username / password combination");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[LogIn] There was an exception whilst getting the id from the database: " + e);
                    //throw new InvalidOperationException("There was an error whilst finding a user with username / password combination");
                    return NotFound();
                }
            }
        }
        */

        /*
        public User LogIn(string pUsername, string pPassword)
        {
            using (var openConn = Parsnip.GetOpenDbConnection())
            {
                try
                {
                    Debug.WriteLine(string.Format("[UserController] Finding user username = {0} password = {1}", pUsername, pPassword));
                    SqlCommand getUser = new SqlCommand("SELECT * FROM t_Users WHERE username = @username AND password = @password", openConn);
                    getUser.Parameters.Add(new SqlParameter("username", pUsername));
                    getUser.Parameters.Add(new SqlParameter("password", pPassword));

                    using (SqlDataReader reader = getUser.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User temp = new User(reader);
                            Debug.WriteLine("[UserController] Found a user. Username = {0} Password = {1} Forename = {2}", pUsername, pPassword, temp._forename);
                            return temp;
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
        */
    }
}
