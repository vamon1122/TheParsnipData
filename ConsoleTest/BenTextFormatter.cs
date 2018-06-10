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
            WriteLineRed(pMsg);
        }

        public static void Success(string pMsg)
        {
            WriteLineGreen(pMsg);
        }

        public static void WriteLineRed(string pMsg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(pMsg);
            Console.ResetColor();
        }

        public static void WriteLineGreen(string pMsg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(pMsg);
            Console.ResetColor();
        }

        public static void WriteLineYellow(string pMsg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(pMsg);
            Console.ResetColor();
        }

        public static void WriteYellow(string pMsg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(pMsg);
            Console.ResetColor();
        }
    }

    
}
