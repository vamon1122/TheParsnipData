using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UacApi;


namespace ConsoleTest
{
    static class UacTest
    {
        static Account MyAccount;

        public static void DoTest()
        {
            if (DoLogIn())
            {
                BenText.Success("Successfully logged in!");
                PrintAccount();
            }
            else
            {
                BenText.Error("Failed to log in!");
            }
            

        }

        public static void PrintAccount()
        {
            if(MyAccount == null)
            {
                throw new InvalidOperationException("Account MyAccount has not been initialised!");
            }
            BenText.WriteYellow("username = ");
            Console.WriteLine(MyAccount.username);
            BenText.WriteYellow("email = ");
            Console.WriteLine(MyAccount.email);
            BenText.WriteYellow("fname = ");
            Console.WriteLine(MyAccount.fname);
            BenText.WriteYellow("sname = ");
            Console.WriteLine(MyAccount.sname);
        }

        public static bool DoLogIn()
        {
            try
            {
                MyAccount = new Account();
                if (MyAccount.LogIn("vamon1122", "BBTbbt1704"))
                {   
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                BenText.Error(string.Format("There was an error whilst logging in: {0}", e));
                return false;
            }
        }
    }
}
