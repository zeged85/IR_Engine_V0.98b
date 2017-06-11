using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IR_Engine
{
    public class Indexer : IModel
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string PropName)
        {

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropName));


        }

        

        private int _listBoxMyMovies;
        public int listBoxMyMovies
        {
            get { return _listBoxMyMovies; }
            set
            {
                if (_listBoxMyMovies != value)
                {
                    _listBoxMyMovies = value;
                    NotifyPropertyChanged("listBoxMyMovies");
                }
            }
        }

        private int progress;
        public int Progress
        {
            get { return progress; }
            set {
                if (progress != value)
                {
                    progress = value;
                    NotifyPropertyChanged("Progress");
                }
                }
        }

        private string docResult;
        public string DocResult
        {
            get { return docResult; }
            set
            {
                if (docResult != value)
                {
                    docResult = value;
                    this.NotifyPropertyChanged("DocResult");
                }
            }
        }
        /// MVVM






        /// </summary>


        //Movies Dictionary
        //movieId,title,genres
        public static volatile Dictionary<int,string> Dic_IdxMovieTitle;

        Dictionary<int, double> YmeanDictionary = new Dictionary<int, double>();

        //Ratings Dictionary
        //userId,movieId,rating,timestamp
        public static volatile Dictionary<int, Dictionary<int, double>> Dic_UsersRatings;


        public static Dictionary<string, int> Dic_movieTitleIdx;

        public static string s_documentsPath; //input folder
        public string postingFilesPath; //output folder

   
        public static volatile int titleNumber /*= 0*/;

        public static volatile int postingFolderCounter = 0;
  
        public static bool limitMemory;

        int userIgnore = 0;

        int FileCountInFolder;
      


        public static int NumOfWordsInCorpus = 0;

        public Indexer()
        {

        }

     public void ProgressTest()
        {
            Progress = 0;
            while (progress != 100)
            {
                Progress++;
                Thread.Sleep(10);
            }

        }

        public static void clearAllData()
        {

            Dic_IdxMovieTitle.Clear();
       
        }

     
        public void createMovieDictionary()
        {
            Dic_movieTitleIdx = new Dictionary<string, int>();
            Progress = 0;
            int size = Dic_IdxMovieTitle.Count;
            int counter = 0;

           foreach (KeyValuePair<int,string> pair in Dic_IdxMovieTitle)
            {
                string s = pair.Value;
                int idx = pair.Key;
                Progress = (counter / size) * 100;
               
                //http://stackoverflow.com/questions/2245442/c-sharp-split-a-string-by-another-string
                string title = s.Split(new string[] { ")," }, StringSplitOptions.None)[0] + ')';
                if (!Dic_movieTitleIdx.ContainsKey(title))
                {
                    Dic_movieTitleIdx.Add(title, idx);
                }
               
            }

        }

        public void initiate()
        {
    

            if (Directory.Exists(s_documentsPath))
            {
                // This path is a directory
                //http://stackoverflow.com/questions/16193126/counting-the-number-of-files-in-a-folder-in-c-sharp
                FileCountInFolder = Directory.GetFiles(s_documentsPath).Length;
                //   stopWords = ReadFile.fileToDictionary(Indexer.documentsPath + "\\stop_words.txt" /*@"C:\stopWords\stop_words.txt"*/);// load stopwords


                Progress = 0;
                DocResult = "loading Movies.csv";
                Dic_IdxMovieTitle = loadMoviesFile(s_documentsPath + @"\movies.csv");
                titleNumber = Dic_IdxMovieTitle.Count - 1;


                Progress = 0;
                DocResult = "loading ratings.csv";
                Dic_UsersRatings = loadRatingsFile(s_documentsPath + @"\ratings.csv");

            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", s_documentsPath);
            }

          //  FileCounter = 0;






        }

    
      public  List<string> autocomplete(string query)
        {

            var keys = Indexer.Dic_movieTitleIdx.Keys.Where(x => x.ToLower().Contains(query));
            List<string> termList = keys.ToList();

            return termList;
        }

        Dictionary<int, double> myRankings = new Dictionary<int, double>();

        public int getMovieID(string str)
        {
            return Dic_movieTitleIdx[str];
        }


        public void selectMovie(int movieID, double rating)
        {

           
            myRankings.Add(movieID, rating);

         //   Dictionary<int,double> result =  findKnearestNeighbours(myRankings);

          //  DocResult = "I suggest you the movie: \"The Matrix\"";
       //     string value = myRatings
            


        }


        double calcMeanX()
        {
            //calculate mean X

            double Xmean = 0;
            double sumOfX = 0;
            int nX = 0;

            foreach (KeyValuePair<int, double> movieRank in myRankings)
            {
                nX++;
                sumOfX += movieRank.Value;
            }

            Xmean = sumOfX / nX;
            return Xmean;
        }

        void calcMeanY()
        {
            //calculate mean Y

           
            double sumOfY = 0;
            int nY = 0;
            int userID;

         
            foreach (KeyValuePair<int, Dictionary<int, double>> pair in Dic_UsersRatings)
            {
                sumOfY = 0;
                userID = pair.Key;
                Dictionary<int, double> userRanking = pair.Value;
                nY = pair.Value.Count;
                foreach (double d in userRanking.Values)
                    sumOfY += d;

                YmeanDictionary[userID] = sumOfY / nY;



            }



        }


        public Dictionary<int, double> getUserData(int userID)
        {
           
            if (Dic_UsersRatings.ContainsKey(userID))
             return  Dic_UsersRatings[userID];

                return null;
        }

        public int getUserAmount()
        {

            int count = Dic_UsersRatings.Count();
            return count;
        }

        public void ignoreUser(int userID)
        {
            userIgnore = userID;
        }

        public Dictionary<int,double> findKnearestNeighbours(Dictionary<int, double> myRankings)
        {
            Dictionary<int, double> Dic_RecommendedMovies = new Dictionary<int, double>();

            double Xmean = calcMeanX();

            calcMeanY();



            //calculate r(X,Y)

            Dictionary<int, double> rXY = new Dictionary<int, double>();

            rXY.Add(0, 0);


            foreach (KeyValuePair<int, Dictionary<int, double>> pair in Dic_UsersRatings)
            {
                int UserID = pair.Key;
                if (UserID != userIgnore) { 
                    Dictionary<int, double> UserRatings = pair.Value;

                    double Sxy = 0;
                    double Sxx = 0;
                    double Syy = 0;

                    double Ymean = YmeanDictionary[UserID];


                    foreach (KeyValuePair<int, double> movieRank in myRankings)
                    {
                        int myMovieID = movieRank.Key;
                        double myMovieRating = movieRank.Value;


                        if (UserRatings.ContainsKey(myMovieID))
                        {

                            Sxy += (myMovieRating - Xmean) * (UserRatings[myMovieID] - YmeanDictionary[UserID]);
                            Sxx += (myMovieRating - Xmean) * (myMovieRating - Xmean);
                            Syy += (UserRatings[myMovieID] - YmeanDictionary[UserID]) * (UserRatings[myMovieID] - YmeanDictionary[UserID]);
                        }


                    }
                    double top = Sxy;
                    double bot = Sxx * Syy;
                    bot = Math.Sqrt(bot);
                    double r = top / bot;
                    if (!double.IsNaN(r) && r != -1)
                    {

                    }
                    rXY.Add(UserID, r);


            }

        }
            var maxGuid = rXY.OrderByDescending(x => x.Value).FirstOrDefault().Key;

            Dictionary<int, Double> Dic_myRatings = new Dictionary<int, double>();
            foreach(KeyValuePair<int,Double> pair in myRankings)
            {
                Dic_myRatings.Add(pair.Key, pair.Value);
            }


            if (maxGuid != 0)
            {

                Dictionary<int, double> similaruserList = Dic_UsersRatings[maxGuid];
               // similaruserList.Add(0, 0);
                DocResult = "UserID similar =" + maxGuid + Environment.NewLine;
             
              
                foreach (KeyValuePair<int,double> pair in similaruserList)
                {
                    // double myRank = pair.Value;
                    double userRank = pair.Value;
                    int movieIDX = pair.Key;
                    string movieTitle = Dic_IdxMovieTitle[movieIDX];

                   
                    if (Dic_myRatings.ContainsKey (movieIDX)) //movie we've both seen
                    {
                    


                    }
                    else //Movie I didnt see. recommended or not...
                    {

                        Dic_RecommendedMovies.Add(movieIDX, userRank);
                        DocResult += movieTitle + " :" + userRank + Environment.NewLine;

                    }
                }
              //  var maxMovieRate = Dic_RecommendedMovies.OrderByDescending(x => x.Value).FirstOrDefault().Key;

            }

            return Dic_RecommendedMovies;
        }

        public void reset()
        {
            myRankings.Clear();
        }

        public void mmm()
        {

        
            System.Console.WriteLine("Job Done.");


        }

        public void setOutputFolder(string path)
        {
            this.postingFilesPath = path;
        }

        public string getOutputFolder()
        {
            return this.postingFilesPath;
        }

        public void loadDictionary()
        {
            Console.WriteLine("Loading File '{0}'.", postingFilesPath + @"\Dictionary.txt");

            //myDictionary.Clear();
        //    myDictionary = ReadFile.fileToSortedList(postingFilesPath + @"\Dictionary.txt");
            
          


            Console.WriteLine("Dictionary loaded.");

        }


        public  Dictionary<int, string> loadMoviesFile(string path)
        {

            try
            {
              
                if (!File.Exists(path))
                    throw new FileNotFoundException();

                //The reste of the code
            }
            catch (FileNotFoundException)
            {
   //             MessageBox.Show("The file is not found in the specified location");
            }
            catch (Exception ex)
            {
    //            MessageBox.Show(ex.Message);
            }


            Dictionary<int, string> newList = new Dictionary<int, string>();
            int lineCount = File.ReadLines(path).Count();
            int count = 0;
            using (StreamReader sr = File.OpenText(path))
            {

                sr.ReadLine();
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    count++;
                    Progress = count*100 / lineCount;
                    
                    //remove blank lines
                    if (s == "")
                        continue;

                    string[] words = s.Split(',');
                    string value = s.Substring(words[0].Length + 1);
                    int key =  Int32.Parse ( words[0]);


                    newList.Add(key,value);


                }
            }
            return newList;
        }


        public Dictionary<int, Dictionary<int, double>> loadRatingsFile(string path)
        {
            Dictionary<int, Dictionary<int,double>> newList = new Dictionary<int, Dictionary<int,double> >();
            DocResult = "calculating ranking.csv";
            decimal lineCount = File.ReadLines(path).Count();
            decimal count = 0;
           
            int limiter = 100000;
            if (limitMemory)
                    lineCount = limiter;
            DocResult = "loading ranking.csv";
            using (StreamReader sr = File.OpenText(path))
            {
                // newList.Add(new KeyValuePair<int, int>(0, 0), sr.ReadLine());
               string header = sr.ReadLine();
                string line = String.Empty;
                decimal temp = 0;
                decimal temp2 = 0;
                while ((line = sr.ReadLine()) != null && limiter != 0)
                {
                    //update progress
                    count++;
                    if (limitMemory)
                    {
                        limiter--;
                    }
                   temp = (decimal)(count / lineCount);
                    temp2 = temp * 100;
                    if (temp2 != Progress)
                        Progress = Convert.ToInt32( temp2);


                    //remove blank lines
                    if (line == "")
                        continue;

                    string[] words = line.Split(',');
                    int userID = Int32.Parse(words[0]);
                    int movieID = Int32.Parse(words[1]);
                   // KeyValuePair <int, int> key = new KeyValuePair<int, int>(userID, movieID);
                    string value = line.Substring(words[0].Length + words[1].Length + 2);
                    string[] splt = value.Split(',');
                    double rank = Double.Parse(splt[0]);
                    int time = Int32.Parse(splt[1]);
                    //  Tuple<double, int> tup = new Tuple<double, int>(rank, time);
                    // string key = words[0];

                    Dictionary<int, double> movieRankDic = new Dictionary<int, double>();


                    if (!newList.ContainsKey(userID))
                    {
                        newList.Add(userID, new Dictionary<int, double>());
                    }
                    newList[userID].Add(movieID, rank);

                    // newList.Add(key, tup);


                }
            }
            return newList;
        }


    }

       

    

      
    
}
