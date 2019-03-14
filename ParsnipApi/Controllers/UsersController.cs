using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ParsnipApi.Models;

namespace ParsnipApi.Controllers
{
    public class UsersController : ApiController
    {
        User[] users = new User[]
        {
            new User { Forename = "Ben", Surname = "Barton", Password = "BBTbbt1704" },
            new User { Forename = "Hadassah", Surname = "Max", Password = "P4ssw0rd" },
            new User { Forename = "Laim", Surname = "Aldred", Password = "iphone4s" }
        };

        public IEnumerable<User> GetAllUsers()
        {
            return users;
        }

        public IHttpActionResult GetUser(string pForename)
        {
            var user = users.FirstOrDefault((u) => u.Forename == pForename);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
