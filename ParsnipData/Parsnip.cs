using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace ParsnipData
{
    public static class Parsnip
    {
        internal static readonly string ParsnipConnectionString = 
            ConfigurationManager.ConnectionStrings["ParsnipDb"].ConnectionString;
        
        public static DateTime AdjustedTime { get { return DateTime.Now.AddHours(8); } }
    }
}
