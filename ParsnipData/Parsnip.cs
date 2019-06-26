using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ParsnipData
{
    public static class Parsnip
    {
        internal static readonly string sqlConnectionString = "Server=198.38.83.33;Database=vamon112_parsnipdb;Uid=vamon112_ben;Password=ccjO07JT";
        //internal static readonly string sqlConnectionString = @"Data Source=BEN-PC\SQLEXPRESS;Initial Catalog=ParsnipTestDb;Integrated Security=True";
        public static SqlConnection GetOpenDbConnection()
        {
            SqlConnection conn = new SqlConnection(sqlConnectionString);
            conn.Open();
            return conn;
        }
        public static DateTime adjustedTime { get { return DateTime.Now.AddHours(8); } }
    }
}
