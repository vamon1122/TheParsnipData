using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CookieApi;

namespace TheParsnipWeb
{
    public static class Data
    {
        public static string deviceType { get { return Cookie.Read("deviceType"); } }
        public static string deviceLocation { get { return Cookie.Read("deviceLocation"); } }
    }
}