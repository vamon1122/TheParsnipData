﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsnipApi
{
    public static class Data
    {
        public static string sqlConnectionString = "Server=198.38.83.33;Database=vamon112_parsnipdb;Uid=vamon112_ben;Password=ccjO07JT";
        public static DateTime adjustedTime { get { return DateTime.Now.AddHours(8); } }
    }
}