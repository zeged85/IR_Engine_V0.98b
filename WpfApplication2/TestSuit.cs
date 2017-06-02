using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IR_Engine
{
    public class TestSuit
    {
        public static string s_testFolder;

        public static bool test1()
        {
            try
            {
                // Get the current directory.
              //  string path = Directory.GetCurrentDirectory();
                string target = @"c:\temp";
                s_testFolder = target;
                //    Console.WriteLine("The current directory is {0}", path);
                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }
                else return false;
                

          
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        
            return true;
        }

        public static bool test2()
        {
            //create file movies.csv
            //movieId,title,genres
            //http://recommender-systems.org/collaborative-filtering/

            string timeStamp = "0000000000";

            string s_moviesTxt =
                "movieId,title,genres" + Environment.NewLine +
                "1,The Piano," + Environment.NewLine +
                "2,Pulp Fiction" + Environment.NewLine +
                "3,Clueless,Drama|Comedy" + Environment.NewLine +
                "4,Cliffhanger,Action" + Environment.NewLine +
                "5,Fargo";

            
            //userId,movieId,rating,timestamp
            string s_ratingTxt =
                "userId,movieId,rating,timestamp" + Environment.NewLine +
                "1,1,0.5,"+ timeStamp + Environment.NewLine +
                "1,2,0.5,"+ timeStamp + Environment.NewLine +
                "1,3,5," + timeStamp + Environment.NewLine +
                "1,4,0.5,"+ timeStamp + Environment.NewLine +
                "1,5,0.5,"+ timeStamp + Environment.NewLine +
                "2,1,0.5,"+ timeStamp + Environment.NewLine +
                "2,2,5," + timeStamp +Environment.NewLine +
                "2,4,0.5,"+ timeStamp + Environment.NewLine +
                "2,5,5," + timeStamp + Environment.NewLine +
                "3,1,5," + timeStamp + Environment.NewLine +
                "3,2,5," + timeStamp + Environment.NewLine +
                "3,3,0.5,"+ timeStamp + Environment.NewLine +
                "3,4,5," + timeStamp + Environment.NewLine +
                "3,5,5," + timeStamp + Environment.NewLine +
                "4,2,0.5,"+ timeStamp + Environment.NewLine +
                "4,3,5," + timeStamp + Environment.NewLine +
                "4,4,0.5,"+ timeStamp + Environment.NewLine +
                "4,5,0.5," + timeStamp + Environment.NewLine;

            string moviesPath = s_testFolder + @"\movies.csv";
            if (!File.Exists(moviesPath))
            {
                // Create a file to write to.

                File.WriteAllText(moviesPath, s_moviesTxt);
            }
            else
                return false;

            string ratingsPath = s_testFolder + @"\ratings.csv";
            if (!File.Exists(ratingsPath))
            {
                // Create a file to write to.

                File.WriteAllText(ratingsPath, s_ratingTxt);
            }
            else
                return false;





            return true;
        }




        public static bool test3()
        {

            Indexer.s_documentsPath = s_testFolder;
            Indexer m_tester = new Indexer();
            m_tester.initiate();




            return true;
        }

    }

 
}
