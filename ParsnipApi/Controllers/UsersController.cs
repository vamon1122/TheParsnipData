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
using LogApi;

namespace ParsnipApi.Controllers
{
    public class UsersController : ApiController
    {
        public IHttpActionResult Get(string username, string password)
        {
            t_Users user;

            using (ParsnipTestDbEntities entities = new ParsnipTestDbEntities())
            {
                Parsnip.AsyncLog.WriteLog(string.Format("[UsersController - Get(username, password)] Searching for user with username = {0} & password = {1}", username, password));
                user = entities.t_Users.FirstOrDefault(u => u.username == username && u.password == password);
            }


            if (user == null)
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

        //[FromBody]: From the body of the Http request
        public HttpResponseMessage Put(string username, [FromBody]t_Users user)
        {
            if (user == null)
            {
                Parsnip.AsyncLog.WriteLog("[UsersController - Put(user)] user is null!");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User parameter was null!");
            }
            else
            {
                Parsnip.AsyncLog.WriteLog("[UsersController - Put(user)] user = " + user);
                Parsnip.AsyncLog.WriteLog("[UsersController - Put(user)] user.id = " + user.id);
                Parsnip.AsyncLog.WriteLog("[UsersController - Put(user)] user.forename = " + user.forename);

                try
                {
                    using (var entities = new ParsnipTestDbEntities())
                    {
                        var existingUser = entities.t_Users.FirstOrDefault(u => u.username == username);

                        if (existingUser == null)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("No user was found with id = {0}. Could not update user.", user.id));
                        }
                        else
                        {
                            existingUser.id = user.id;
                            existingUser.username = user.username;
                            existingUser.email = user.email;
                            existingUser.password = user.password;
                            existingUser.forename = user.forename;
                            existingUser.surname = user.surname;
                            existingUser.dob = user.dob;
                            existingUser.gender = user.gender;
                            existingUser.address1 = user.address1;
                            existingUser.address2 = user.address2;
                            existingUser.address3 = user.address3;
                            existingUser.postCode = user.postCode;
                            existingUser.mobilePhone = user.mobilePhone;
                            existingUser.homePhone = user.homePhone;
                            existingUser.workPhone = user.workPhone;
                            //existingUser.created = user.created;
                            existingUser.lastLogIn = user.lastLogIn;
                            existingUser.type = user.type;
                            existingUser.status = user.status;
                            existingUser.ProfilePicUrl = user.ProfilePicUrl;

                            entities.SaveChanges();
                            return Request.CreateResponse(HttpStatusCode.OK, existingUser);
                        }

                    }
                }
                catch (Exception e)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
                }
            }
        }

        //Must begin with the 'Get' keyword
        public IEnumerable<t_Users> GetAll()
        {
            new LogEntry(Parsnip.debugLog) { text = "[UsersController - GetAll()] " };
            using (ParsnipTestDbEntities entities = new ParsnipTestDbEntities())
            {
                return entities.t_Users.ToList();
            }
        }
    }
}
