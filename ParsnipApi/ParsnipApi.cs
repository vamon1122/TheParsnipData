using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;

namespace ParsnipApi
{
    public static class Parsnip
    {
        public static readonly string apiUrl = @"api/";

        //Live
        //public static string sqlConnectionString = "Server=198.38.83.33;Database=vamon112_parsnipdb;Uid=vamon112_ben;Password=ccjO07JT";
        //public static readonly string baseAddress = "https://theparsnip.co.uk/";

        //Home
        public static readonly LogWriter AsyncLog = new LogWriter("Async_Login.txt", @"C:\Users\benba\Documents\GitHub\TheParsnipWeb");
        public static readonly string sqlConnectionString = @"Data Source=BEN-PC\SQLEXPRESS;Initial Catalog=ParsnipTestDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static readonly string baseAddress = "http://localhost:59622/";

        //Branson
        //static readonly LogWriter AsyncLog = new LogWriter("Async_Login.txt", @"C:\Users\ben.2ESKIMOS\Documents\GitHub\TheParsnipWeb");
        //public static readonly string sqlConnectionString = @"Data Source=branson\sqlexpress;Initial Catalog=ParsnipTestDb;Integrated Security=True";
        //public static readonly string baseAddress = "http://localhost:59622/";

        public static SqlConnection GetOpenDbConnection()
        {
            SqlConnection conn = new SqlConnection(sqlConnectionString);
            conn.Open();
            return conn;
        }
        public static DateTime adjustedTime { get { return DateTime.Now.AddHours(8); } }
    }
}
