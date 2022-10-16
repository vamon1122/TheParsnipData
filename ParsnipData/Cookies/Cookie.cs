using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;

namespace ParsnipData.Cookies
{
    public static class Cookie
    {
        public static string Read(string pName)
        {
            HttpCookie myCookie;

            if (HttpContext.Current != null && HttpContext.Current.Request.Cookies[pName] != null)
            {
                myCookie = HttpContext.Current.Request.Cookies[pName];
                //System.Diagnostics.Debug.WriteLine("Cookie \"{0}\" = \"{1}\"", pName, myCookie.Value);
                return myCookie.Value;
            }
            else
            {
                return null;
            }
            
        }

        public static bool Exists(string pName)
        {
            if (Read(pName) != null)
                return true;
            else
                return false;
        }

        public static bool WritePerm(string pName, string pVal)
        {
            try
            {
                HttpContext.Current.Response.Cookies[pName].Value = pVal;
                HttpContext.Current.Response.Cookies[pName].Expires = DateTime.Now.AddDays(365);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool WriteSession(string pName, string pVal)
        {
            try
            {
                HttpContext.Current.Response.Cookies[pName].Value = pVal;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
