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
        public IHttpActionResult Get(string username, string password)
        {
            t_Users user;

            {
                using (ParsnipTestDbEntities entities = new ParsnipTestDbEntities())
                {
                    Parsnip.AsyncLog.WriteLog(string.Format("[UsersController - Get(username, password)] Searching for user with username = {0} & password = {1}", username, password));
                    user = entities.t_Users.FirstOrDefault(u => u.username == username && u.password == password);
                }
            }

            if(user == null)
            {
                Parsnip.AsyncLog.WriteLog(string.Format("[UsersController - Get(username, password)] No user was found with username = {0} & password = {1}. Returning NotFound()", username, password));
                return NotFound();
            }
            else
            {
                Parsnip.AsyncLog.WriteLog("[UsersController - Get(username, password)] A user was found with username = {0} & password = {1}. Returning Ok(user)");
                return Ok(user);
            }       
        }   

        //Must begin with the 'Get' keyword
        public IEnumerable<t_Users> GetAll()
        {
            using(ParsnipTestDbEntities entities = new ParsnipTestDbEntities())
            {
                return entities.t_Users.ToList();
            }
        }
    }
}
