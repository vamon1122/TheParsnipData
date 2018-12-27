using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParsnipApi;

namespace LogApi
{
    public class LogEntry
    {
        public Guid id { get; }
        public Guid userId { get; set; } //Internal for loading purposes (see Data)
        public DateTime date { get; internal set; } //Internal for loading purposes (see Data)

        private string _type;
        public string type { get { return _type; } set{ if (value.Length < 11)  _type = value;  else  throw new FormatException(String.Format("The value for type \"{0}\" is too long!", value)); } }

        private string _text;

        public string text { get { return _text; } set {
                if (value.Length < 8001)
                {
                    _text = value;
                    System.Diagnostics.Debug.WriteLine(text);
                    Insert();
                }
                else
                {
                    throw new FormatException(
                        String.Format("The value for type \"{0}\" is too long!",
                        value));
                }

            } }

        public LogEntry(Guid pId)
        {
            id = pId;
        }

        public LogEntry()
        {
            id = Guid.NewGuid();
            date = ParsnipApi.Data.adjustedTime;

        }

        private bool Insert()
        {
            return Data.InsertLogEntry(this);
        }
    }
}
