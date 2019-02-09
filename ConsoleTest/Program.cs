using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaApi;
using ParsnipApi;
using System.Data.SqlClient;
using System.Diagnostics;
using UacApi;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            User tempUser = User.LogIn("vamon1122", "BBTbbt1704");
            //UacTest.DoTest();
            Console.WriteLine("Testing photos...");
            string[] newphotos = new string[]
            {
                @"resources\media\images\photos\Photos\Tom_Smellz.jpg",
                @"resources\media\images\photos\Photos\Tom_Selfie.jpg",
                @"resources\media\images\photos\Photos\Tom_Loves_Balloon.jpg",
                @"resources\media\images\photos\Photos\kieron_2.jpg",
                @"resources\media\images\photos\Photos\baby_kiwi.jpg",
                @"resources\media\images\photos\Photos\philips_go-karting_2.jpg",
                @"resources\media\images\photos\Photos\philips_go-karting.jpg",
                @"resources\media\images\photos\Photos\Aaron_s_FOD.jpg",
                @"resources\media\images\photos\Photos\aaron_yes_ladies.jpg",
                @"resources\media\images\photos\Photos\ben_loldred.jpg",
                @"resources\media\images\photos\Photos\ben_facetime.jpg",
                @"resources\media\images\photos\Photos\ben_licking_the_pussy.jpg",
                @"resources\media\images\photos\Photos\ben_nye.jpg",
                @"resources\media\images\photos\Photos\dan_damn_daniel.jpg",
                @"resources\media\images\photos\Photos\dan_future_dan.jpg",
                @"resources\media\images\photos\Photos\loldred_bender.jpg",
                @"resources\media\images\photos\Photos\loldred_dan_cross.jpg",
                @"resources\media\images\photos\Photos\loldred_moustache.jpg",
                @"resources\media\images\photos\Photos\loldred_santa.jpg",
                @"resources\media\images\photos\Photos\loldred_teacher.jpg",
                @"resources\media\images\photos\Photos\loldred_yes_we_can.jpg",
                @"resources\media\images\photos\Photos\mason_1.JPG",
                @"resources\media\images\photos\Photos\mason_2.JPG",
                @"resources\media\images\photos\Photos\squad.JPG",
                @"resources\media\images\photos\Photos\tom_1.JPG",
                @"resources\media\images\photos\Photos\tom_2.JPG",
                @"resources\media\images\photos\Photos\tom_glasses.JPG",
                @"resources\media\images\photos\Photos\kieron_tongue.JPG",
                @"resources\media\images\photos\Photos\kieron_peter_andre.JPG",
                @"resources\media\images\photos\Photos\kieron_fuckboy.JPG",
                @"resources\media\images\photos\Photos\kieron_fingering_machine.PNG",
                @"resources\media\images\photos\Photos\kieron_smile.JPG",
                @"resources\media\images\photos\Photos\kieron_fat.JPG",
                @"resources\media\images\photos\Photos\kieron_1.JPG"
            };
            int i = 0;
            using (SqlConnection openConn = Parsnip.GetOpenDbConnection())
            {
                foreach (string temp in newphotos)
                {

                    SqlCommand InsertPhotoIntoDb = new SqlCommand("INSERT INTO t_Photos (photosrc, datecreated, createdbyid) VALUES(@photosrc, @datecreated, @createdbyid)", openConn);

                    //InsertPhotoIntoDb.Parameters.Add(new SqlParameter("id", Id));
                    InsertPhotoIntoDb.Parameters.Add(new SqlParameter("photosrc", temp));
                    InsertPhotoIntoDb.Parameters.Add(new SqlParameter("datecreated", Parsnip.adjustedTime));
                    InsertPhotoIntoDb.Parameters.Add(new SqlParameter("createdbyid", tempUser.Id));

                    InsertPhotoIntoDb.ExecuteNonQuery();

                    Debug.WriteLine(String.Format("Successfully inserted photo into database"));
                    i++;
                    Console.Write(string.Format("\r {0}/{1}", i, newphotos.Count()));
                }
            }

            
            
            Console.ReadLine();
        }
    }
}
