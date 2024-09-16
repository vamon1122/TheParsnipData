using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace ParsnipData
{
    public static class Parsnip
    {
        internal static readonly string ParsnipConnectionString = 
            ConfigurationManager.ConnectionStrings["ParsnipDb"].ConnectionString;
        
        public static DateTime AdjustedTime { get { return DateTime.Now.AddHours(8); } }

        public static string SanitiseSearchString(string text) => System.Text.RegularExpressions.Regex.Replace(text.ToLower(), "[^a-z0-9_ ]", "");

        public static string RemoveStrings(this string stringToEdit, string[] stringsToRemove)
        {
            stringToEdit += " ";
            
            foreach(var s in stringsToRemove)
                stringToEdit = stringToEdit.Replace($" {s.Trim()} ", " ");

            return stringToEdit.Substring(0, stringToEdit.Length - 1);
        }

        public static string[] Split(this string stringToSplit, char separator, StringSplitOptions stringSplitOptions) =>
            stringToSplit.Split(new[] { separator }, stringSplitOptions);
        
        public static string GetAtsAndTagsAsString(this Media.Media myMedia)
        {
            var returnValue = string.Empty;
            var tags = new List<Tuple<char, string>>();
            myMedia.MediaUserPairs.ForEach(p => tags.Add(new Tuple<char, string>('@', p.Name)));
            myMedia.MediaTagPairs.ForEach(p => tags.Add(new Tuple<char, string>('#', p.MediaTag.Name)));
            tags.OrderBy(t => t.Item2).ToList().ForEach(t => returnValue += $"{t.Item1}{t.Item2} ");
            returnValue.TrimEnd();

            return returnValue;
        }

        public static DateTime DateTimeFileCreated(this FileInfo file) =>
            file.CreationTime < file.LastWriteTime ? file.CreationTime : file.LastWriteTime;
    }



    public class ParsnipId : IEquatable<ParsnipId>, IEquatable<string>
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

        public bool Equals(string other)
        {
            return this._id == other;
        }
        public bool Equals(ParsnipId other)
        {
            return this._id == other._id;
        }
    }
}
