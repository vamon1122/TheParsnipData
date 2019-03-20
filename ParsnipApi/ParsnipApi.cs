using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace ParsnipApi
{
    public static class Parsnip
    {
        public static readonly string apiUrl = @"/api/";
        public static readonly HttpClient client = new HttpClient();
        

        static Parsnip()
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/xml"));
        }


        ////public static string sqlConnectionString = ";";

        //Live
        //public static readonly string sqlConnectionString = "Server=198.38.83.33;Database=vamon112_parsnipdb;Uid=vamon112_ben;Password=ccjO07JT";
        //public static readonly string baseAddress = "http://api.theparsnip.co.uk";
        //public static readonly LogWriter AsyncLog = new LogWriter("Async_Login.txt", HttpContext.Current.Server.MapPath("~"));

        //Home
        public static readonly string sqlConnectionString = @"Data Source=BEN-PC\SQLEXPRESS;Initial Catalog=ParsnipTestDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static readonly string baseAddress = "http://localhost:59622";
        public static readonly LogWriter AsyncLog = new LogWriter("Async_Login.txt", @"C:\Users\benba\Documents\GitHub\TheParsnipWeb");

        //Branson
        //public static readonly string sqlConnectionString = @"Data Source=branson\sqlexpress;Initial Catalog=ParsnipTestDb;Integrated Security=True";
        //public static readonly string baseAddress = "http://localhost:59622";
        //public static readonly LogWriter AsyncLog = new LogWriter("Async_Login.txt", @"C:\");

        public static SqlConnection GetOpenDbConnection()
        {
            SqlConnection conn = new SqlConnection(sqlConnectionString);
            conn.Open();
            return conn;
        }
        public static DateTime adjustedTime { get { return DateTime.Now.AddHours(8); } }
    }
}
