using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaApi;
using ParsnipApi;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //UacTest.DoTest();
            Console.WriteLine("Testing photos...");
            string[] newphotos = new string[]
            {
                "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057402/Photos/Photos/Tom_Smellz.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057402/Photos/Photos/Tom_Selfie.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057401/Photos/Photos/Tom_Loves_Balloon.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057403/Photos/Photos/kieron_2.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057400/Photos/Photos/baby_kiwi.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057404/Photos/Photos/philips_go-karting_2.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/philips_go-karting.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057399/Photos/Photos/Aaron_s_FOD.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057399/Photos/Photos/aaron_yes_ladies.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057400/Photos/Photos/ben_loldred.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057400/Photos/Photos/ben_facetime.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057400/Photos/Photos/ben_licking_the_pussy.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057401/Photos/Photos/ben_nye.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057401/Photos/Photos/dan_damn_daniel.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057402/Photos/Photos/dan_future_dan.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/loldred_bender.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/loldred_dan_cross.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/loldred_moustache.jpg",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057399/Photos/Photos/loldred_santa.jpg",

            "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057399/Photos/Photos/loldred_teacher.jpg",
            "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057399/Photos/Photos/loldred_yes_we_can.jpg",
            "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/mason_1.JPG",
            "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/mason_2.JPG",

            "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/squad.JPG",
"http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/tom_1.JPG",

           "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/tom_2.JPG",
            "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/tom_glasses.JPG",
            "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/kieron_tongue.JPG",
            "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/kieron_peter_andre.JPG",
            "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/kieron_fuckboy.JPG",
            "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/kieron_fingering_machine.PNG",
            "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/kieron_smile.JPG",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/kieron_fat.JPG",
        "http://res.cloudinary.com/lqrrvz3pc/image/upload/v1477057406/Photos/Photos/kieron_1.JPG"
 };
            int i = 0;
            using (SqlConnection openConn = Parsnip.GetOpenDbConnection())
            {
                foreach (string temp in newphotos)
                {

                    SqlCommand InsertPhotoIntoDb = new SqlCommand("INSERT INTO t_Photos (photosrc) VALUES(@photosrc)", openConn);

                    //InsertPhotoIntoDb.Parameters.Add(new SqlParameter("id", Id));
                    InsertPhotoIntoDb.Parameters.Add(new SqlParameter("photosrc", temp));

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
