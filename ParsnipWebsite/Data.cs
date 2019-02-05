using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CookieApi;

namespace ParsnipWebsite
{
    public static class Data
    {
        public static string deviceType
        {
            get
            {
                string deviceType = Cookie.Read("deviceType");
                switch (deviceType)
                {
                    case "Android":
                        deviceType += " device";
                        break;
                    case "webOS":
                        deviceType += " device";
                        break;
                    case "BlackBerry":
                        deviceType += " device";
                        break;
                    case "Windows":
                        deviceType += " device";
                        break;
                    case "MacOS":
                        deviceType += " device";
                        break;
                    case "UNIX":
                        deviceType += " device";
                        break;
                    case "Linux":
                        deviceType += " device";
                        break;
                    default:
                        break;
                }

                return deviceType;
            }
        }
        public static string deviceLatitude { get { return Cookie.Read("deviceLatitude"); } }
        public static string deviceLongitude { get { return Cookie.Read("deviceLongitude"); } }
        public static string SessionId { get { return Cookie.Read("sessionId"); } }
    }
}