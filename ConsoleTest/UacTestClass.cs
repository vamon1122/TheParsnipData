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
        public static void DoTest()
        {
            if (DoLogIn())
            {
                BenText.Success("Successfully logged in!");
            }
            else
            {
                BenText.Error("Failed to log in!");
            }
            

        }

        public static bool DoLogIn()
        {
            try
            {
                Account MyAccount = new Account();
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
