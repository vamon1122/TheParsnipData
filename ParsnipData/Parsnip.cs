using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace ParsnipData
{
    public static class Parsnip
    {
        internal static readonly string ParsnipConnectionString = 
            ConfigurationManager.ConnectionStrings["ParsnipDb"].ConnectionString;
        
        public static DateTime AdjustedTime { get { return DateTime.Now.AddHours(8); } }
    }

    

    public class ParsnipId
    {
        public static readonly ParsnipId Empty = new ParsnipId();

        private string _id;

        public ParsnipId()
        {
            _id = "00000000";
        }

        public ParsnipId(string mediaId)
        {
            try
            {
                if (string.IsNullOrEmpty(mediaId))
                    throw new ArgumentNullException();

                if (mediaId.Length == 8)
                    _id = mediaId;
                else
                    throw new InvalidCastException();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception whilst converting {mediaId} to ParsnipData.Media.MediaId: {ex}");
            }
            _id = mediaId;
        }

        public static ParsnipId NewParsnipId()
        {
            return new ParsnipId(NewParsnipIdString());
        }

        private protected static string NewParsnipIdString()
        {
            return Guid.NewGuid().ToString().Split('-')[0];
        }

        public override string ToString()
        {
            return _id;
        }
    }
}
