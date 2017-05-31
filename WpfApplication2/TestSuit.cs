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


        public static bool test2()
        {
            try
            {
                // Get the current directory.
                string path = Directory.GetCurrentDirectory();
                string target = @"c:\temp";
                Console.WriteLine("The current directory is {0}", path);
                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }

                

                /*
                // Change the current directory.
                Environment.CurrentDirectory = (target);
                if (path.Equals(Directory.GetCurrentDirectory()))
                {
                    Console.WriteLine("You are in the temp directory.");
                }
                else
                {
                    Console.WriteLine("You are not in the temp directory.");
                }
                */
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        
            return true;
        }

        public static bool test1()
        {
            //create file movies.csv
            //movieId,title,genres
            //http://recommender-systems.org/collaborative-filtering/

            string movies =
                "1,The Piano" + Environment.NewLine +
                "2,Pulp Fiction" + Environment.NewLine +
                "3,Clueless" + Environment.NewLine +
                "4,Cliffhanger" + Environment.NewLine +
                "5,Fargo";


            //userId,movieId,rating,timestamp
            string rating =
                "1,1,0.5" + Environment.NewLine +
                "1,2,0.5" + Environment.NewLine +
                "1,3,5" + Environment.NewLine +
                "1,4,0.5" + Environment.NewLine +
                "1,5,0.5" + Environment.NewLine +
                "2,1,0.5" + Environment.NewLine +
                "2,2,5" + Environment.NewLine +
                "2,4,0.5" + Environment.NewLine +
                "2,5,5" + Environment.NewLine +
                "3,1,5" + Environment.NewLine +
                "3,2,5" + Environment.NewLine +
                "3,3,0.5" + Environment.NewLine +
                "3,4,5" + Environment.NewLine +
                "3,5,5" + Environment.NewLine +
                "4,2,0.5" + Environment.NewLine +
                "4,3,5" + Environment.NewLine +
                "4,4,0.5" + Environment.NewLine +
                "4,5,0.5" + Environment.NewLine;


            return true;
        }
    }

 
}
