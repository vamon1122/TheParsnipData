using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    static class BenText
    {
        public static void Error(string pMsg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(pMsg);
            Console.ResetColor();
        }

        public static void Success(string pMsg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(pMsg);
            Console.ResetColor();
        }
    }
}
