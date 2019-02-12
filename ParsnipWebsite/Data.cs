using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CookieApi;

namespace ParsnipWebsite
{
    public static class Data
    {
        public static bool IsMobile { get { return Convert.ToBoolean(Cookie.Read("isMobile")); } }

        public static string DeviceType
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
        public static string DeviceLatitude { get { return Cookie.Read("deviceLatitude"); } }
        public static string DeviceLongitude { get { return Cookie.Read("deviceLongitude"); } }
        public static string SessionId { get { return Cookie.Read("sessionId"); } }
    }
}