using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParsnipData.Accounts;


namespace ConsoleTest
{
    static class UacTest
    {
        static User MyAccount;

        public static void DoTest()
        {
            if (DoLogIn())
            {
                BenText.Success("Successfully logged in!");
                PrintAccount(MyAccount);

                if (CreateNewAccount())
                {
                    BenText.Success("Successfully created new account!");
                }
                else
                {
                    BenText.Error("Failed to create new account!");
                }
            }
            else
            {
                BenText.Error("Failed to log in!");
            }
            

        }

        public static bool CreateNewAccount()
        {
            User MyNewAccount = new User("Console Test - CreateNewAccount");
            MyNewAccount.Username = "jbloggs";
            MyNewAccount.Forename = "Joe";
            MyNewAccount.Surname = "Bloggs";

            if (MyNewAccount.Update())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void PrintAccount(User pAccount)
        {
            if(pAccount == null)
            {
                throw new InvalidOperationException("Account MyAccount has not been initialised!");
            }
            BenText.WriteYellow("username = ");
            Console.WriteLine(MyAccount.Username);
            BenText.WriteYellow("email = ");
            Console.WriteLine(MyAccount.Email);
            BenText.WriteYellow("fname = ");
            Console.WriteLine(MyAccount.Forename);
            BenText.WriteYellow("sname = ");
            Console.WriteLine(MyAccount.Surname);
        }

        public static bool DoLogIn()
        {
            try
            {
                MyAccount = new User("Console Test - DoLogIn");
                if (MyAccount.LogIn("vamon1122", false, "BBTbbt1704", false))
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
