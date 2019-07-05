using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParsnipData.Media;
using ParsnipData;
using System.Data.SqlClient;
using System.Diagnostics;
using ParsnipData.Accounts;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            User tempUser = User.LogIn("ADMIN", "BBTbbt1704");
            //UacTest.DoTest();
            Console.WriteLine("Beginning batch photo upload");
            string[] newphotos = new string[]
            {
                @"Resources\Media\Images\Local\Memes\Tom_Smellz.jpg",
                @"Resources\Media\Images\Local\Memes\Tom_Selfie.jpg",
                @"Resources\Media\Images\Local\Memes\Tom_Loves_Balloon.jpg",
                @"Resources\Media\Images\Local\Memes\kieron_2.jpg",
                @"Resources\Media\Images\Local\Memes\baby_kiwi.jpg",
                @"Resources\Media\Images\Local\Memes\philips_go-karting_2.jpg",
                @"Resources\Media\Images\Local\Memes\philips_go-karting.jpg",
                @"Resources\Media\Images\Local\Memes\Aaron_s_FOD.jpg",
                @"Resources\Media\Images\Local\Memes\aaron_yes_ladies.jpg",
                @"Resources\Media\Images\Local\Memes\ben_loldred.jpg",
                @"Resources\Media\Images\Local\Memes\ben_facetime.jpg",
                @"Resources\Media\Images\Local\Memes\ben_licking_the_pussy.jpg",
                @"Resources\Media\Images\Local\Memes\ben_nye.jpg",
                @"Resources\Media\Images\Local\Memes\dan_damn_daniel.jpg",
                @"Resources\Media\Images\Local\Memes\dan_future_dan.jpg",
                @"Resources\Media\Images\Local\Memes\loldred_bender.jpg",
                @"Resources\Media\Images\Local\Memes\loldred_dan_cross.jpg",
                @"Resources\Media\Images\Local\Memes\loldred_moustache.jpg",
                @"Resources\Media\Images\Local\Memes\loldred_santa.jpg",
                @"Resources\Media\Images\Local\Memes\loldred_teacher.jpg",
                @"Resources\Media\Images\Local\Memes\loldred_yes_we_can.jpg",
                @"Resources\Media\Images\Local\Memes\mason_1.JPG",
                @"Resources\Media\Images\Local\Memes\mason_2.JPG",
                @"Resources\Media\Images\Local\Memes\squad.JPG",
                @"Resources\Media\Images\Local\Memes\tom_1.JPG",
                @"Resources\Media\Images\Local\Memes\tom_2.JPG",
                @"Resources\Media\Images\Local\Memes\tom_glasses.JPG",
                @"Resources\Media\Images\Local\Memes\kieron_tongue.JPG",
                @"Resources\Media\Images\Local\Memes\kieron_peter_andre.JPG",
                @"Resources\Media\Images\Local\Memes\kieron_fuckboy.JPG",
                @"Resources\Media\Images\Local\Memes\kieron_fingering_machine.PNG",
                @"Resources\Media\Images\Local\Memes\kieron_smile.JPG",
                @"Resources\Media\Images\Local\Memes\kieron_fat.JPG",
                @"Resources\Media\Images\Local\Memes\kieron_1.JPG"


            };

            var OrderedPhotos = newphotos.Reverse();

            int i = 0;
            using (SqlConnection openConn = Parsnip.GetOpenDbConnection())
            {
                foreach (string temp in OrderedPhotos)
                {
                    DateTime TimerEnd = DateTime.Now.AddMilliseconds(100);
                    while (DateTime.Now < TimerEnd) { }

                    SqlCommand InsertPhotoIntoDb = new SqlCommand("INSERT INTO image (photosrc, datecreated, createdbyid) VALUES(@photosrc, @datecreated, @createdbyid)", openConn);

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

            Console.WriteLine();
            Debug.WriteLine(String.Format("Upload complete!"));
            Console.ReadLine();
            */
        }
        
    }
}
