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

        public static double onlineTest(Indexer model)
        {
            Indexer m_tester = model;
            double RMSEtop = 0;
            double RMSEbot = 0;
            //m_tester.initiate();
            
            int userCount = m_tester.getUserAmount();
           
            Random rnd1 = new Random();
            int rndUser = rnd1.Next(1, userCount+1);

            Dictionary<int,double> userRating =  m_tester.getUserData(rndUser);

            if (userRating != null)
            {
                m_tester.ignoreUser(rndUser);

                int userRatingCount = userRating.Count;

                Dictionary<int, double> train = new Dictionary<int, double>();
                Dictionary<int, double> test = new Dictionary<int, double>();

                

                Random rnd2 = new Random();
                int rndRank;

                foreach (KeyValuePair<int,double> userPair in userRating)
                {
                    int movieID = userPair.Key;
                    double movieRating = userPair.Value;

                    rndRank = rnd2.Next(1, 3);

                    if (rndRank == 1)
                    {
                        train.Add(movieID, movieRating);
                    }
                    else
                    {
                        test.Add(movieID, movieRating);
                    }


                }

                //m_tester.selectMovie(i);

                foreach (KeyValuePair<int, double> trainPair in train)
                {
                    m_tester.selectMovie(trainPair.Key, trainPair.Value);
                }

                Dictionary<int,double> bestResult =  m_tester.findKnearestNeighbours(train);

                foreach (KeyValuePair<int, double> mytestpair in test)
                {
                    int movieID = mytestpair.Key;
                    double rank = mytestpair.Value; //parameter

                    if (bestResult.ContainsKey(movieID))
                    {
                       double result = bestResult[movieID]; //expected
                        double delta = result - rank;
                        RMSEtop += delta * delta;
                        RMSEbot++;
                    }
                }

            }

            double RMSE = Math.Sqrt(RMSEtop / RMSEbot);
            
            m_tester.ignoreUser(0);
            return RMSE;
        }


        public static bool offlineTest()
        {
            bool ans;
            ans = test1();
            if (ans)
                ans = test2();
            if (ans)
                ans = test3();

            return ans;

        }

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
                else return true;
                

          
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
