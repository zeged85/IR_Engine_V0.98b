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
        public static volatile List<string> myMovies;

        Dictionary<int, double> YmeanDictionary = new Dictionary<int, double>();

        //Ratings Dictionary
        //userId,movieId,rating,timestamp
        public static volatile Dictionary<int, Dictionary<int, double>> DBRatings;


        public static Dictionary<string, int> myMoviesDictionary;

        public static string documentsPath; //input folder
        public string postingFilesPath; //output folder

   //     public static Mutex _TitleNumberMutex;
        public static volatile int titleNumber /*= 0*/;

        public static volatile int postingFolderCounter = 0;
   //     private static bool stopMemoryHandler = false;

        //public static string postingFilesPath = @"c:\IR_Engine\";
     //   public static List<string> PostingFileTermList = new List<string>();
        
    //    public static int amountOfUnique/* = 0*/;
    //    public static int wordNum = 0;
        public static bool ifStemming;

   //     public static SortedDictionary<string, string> stopWords;

   //     public static List<string> uniqueTerms = new List<string>();

     //   public static SortedDictionary<string, int> freqInAllCorpusList = new SortedDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

     //  public static SortedDictionary<string, int> Months = new SortedDictionary<string, int>();
        //  public static List<string> UniqueList = new List<string>();

        int FileCountInFolder;
        int FileCounter;


        public static int NumOfWordsInCorpus = 0;

        public Indexer()
        {//added comparer
         ///for case
         ///al-sharq^#4#4(Doc#:45-1)(Doc#:4-1)(Doc#:93-1)(Doc#:52-1)
            // al - sharq + al - awsat ^#2#2(Doc#:45-1)(Doc#:93-1)
            ///altai ^#2#2(Doc#:46-1)(Doc#:94-1)
            ///altanbulag ^#2#2(Doc#:18-1)(Doc#:66-1)
            ///altanbulag + customhous ^#2#2(Doc#:18-1)(Doc#:66-1)
            ///alter ^#6#6(Doc#:23-1)(Doc#:36-1)(Doc#:46-1)(Doc#:71-1)(Doc#:84-1)(Doc#:94-1)
            ///altern ^#16#12(Doc#:23-1)(Doc#:26-1)(Doc#:29-1)(Doc#:35-3)(Doc#:27-1)(Doc#:42-1)(Doc#:71-1)(Doc#:74-1)(Doc#:75-1)(Doc#:77-1)(Doc#:83-3)(Doc#:90-1)
            ///al - thawrah ^#6#6(Doc#:7-1)(Doc#:34-1)(Doc#:48-1)(Doc#:55-1)(Doc#:82-1)(Doc#:96-1)
            ///al - thaw...
            ///http://stackoverflow.com/questions/19370734/sortedlist-sorteddictionary-weird-behavior
            ///https://www.dotnetperls.com/dictionary-stringcomparer
            ///
          //  myMovies = new SortedDictionary<string, string>();
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

            myMovies.Clear();
           // Months.Clear();
   
            //stopWords.Clear();
          
         //   threads.Clear();
        }

     
        public void createMovieDictionary()
        {
            myMoviesDictionary = new Dictionary<string, int>();
            Progress = 0;
            int size = myMovies.Count;
            int counter = 0;

           foreach (string s in myMovies)
            {
                Progress = (counter / size) * 100;
               
                //http://stackoverflow.com/questions/2245442/c-sharp-split-a-string-by-another-string
                string title = s.Split(new string[] { ")," }, StringSplitOptions.None)[0] + ')';
                if (!myMoviesDictionary.ContainsKey(title))
                {
                    myMoviesDictionary.Add(title, counter++);
                }
               
            }

        }

        public void initiate()
        {
    

            if (Directory.Exists(documentsPath))
            {
                // This path is a directory
                //http://stackoverflow.com/questions/16193126/counting-the-number-of-files-in-a-folder-in-c-sharp
                FileCountInFolder = Directory.GetFiles(documentsPath).Length;
                //   stopWords = ReadFile.fileToDictionary(Indexer.documentsPath + "\\stop_words.txt" /*@"C:\stopWords\stop_words.txt"*/);// load stopwords


                Progress = 0;
                DocResult = "loading Movies.csv";
                myMovies = loadMoviesFile(documentsPath + @"\movies.csv");
                titleNumber = myMovies.Count - 1;


                Progress = 0;
                DocResult = "loading ratings.csv";
                DBRatings = loadRatingsFile(documentsPath + @"\ratings.csv");

            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", documentsPath);
            }

            FileCounter = 0;






        }

    
      public  List<string> autocomplete(string query)
        {

            var keys = Indexer.myMoviesDictionary.Keys.Where(x => x.ToLower().Contains(query));
            List<string> termList = keys.ToList();

            return termList;
        }

        List<KeyValuePair<int, int>> myRankings = new List<KeyValuePair<int, int>>();

        public void selectMovie(string title, int rating)
        {

            int movieID = myMoviesDictionary[title];
            myRankings.Add(new KeyValuePair<int, int>(movieID, rating));

            List<int> result =  findKnearestNeighbours(myRankings);

            DocResult = "I suggest you the movie: \"The Matrix\"";
       //     string value = myRatings
            


        }


        double calcMeanX()
        {
            //calculate mean X

            double Xmean = 0;
            double sumOfX = 0;
            int nX = 0;

            foreach (KeyValuePair<int, int> movieRank in myRankings)
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

         
            foreach (KeyValuePair<int, Dictionary<int, double>> pair in DBRatings)
            {
                userID = pair.Key;
                Dictionary<int, double> userRanking = pair.Value;
                nY = pair.Value.Count;
                foreach (double d in userRanking.Values)
                    sumOfY += d;

                YmeanDictionary[userID] = sumOfY / nY;



            }



        }


        List<int> findKnearestNeighbours(List<KeyValuePair<int,int>> myRankings)
        {

            double Xmean = calcMeanX();

            calcMeanY();
         


            //calculate r(X,Y)

            Dictionary<int, double> rXY = new Dictionary<int, double>();


               

                foreach ( KeyValuePair< int , Dictionary<int,double>>  pair in DBRatings)
                {
                int UserID = pair.Key;
                Dictionary<int, double> UserRatings = pair.Value;

                double Sxy = 0;
                double Sxx = 0;
                double Syy = 0;

                double Ymean = YmeanDictionary[UserID];


                foreach (KeyValuePair<int, int> movieRank in myRankings)
                {
                    int myMovieID = movieRank.Key;
                    int myMovieRating = movieRank.Value;


                    if ( UserRatings.ContainsKey( myMovieID))
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

            var maxGuid = rXY.OrderByDescending(x => x.Value).FirstOrDefault().Key;

            

            return null;
        }

        public void mmm()
        {

            //http://stackoverflow.com/questions/10391481/number-of-occurrences-of-a-character-in-a-string

            //1 go over every letter in DB Folder
            //2 add term and ...
            // Value??
            /// count number of occurrences in document
            /// get document number
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


        public  List<string> loadMoviesFile(string path)
        {
            List<string> newList = new List<string>();
            int lineCount = File.ReadLines(path).Count();
            int count = 0;
            using (StreamReader sr = File.OpenText(path))
            {
        
                newList.Add(sr.ReadLine());
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
                    string key = words[0];


                    newList.Add(value);


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
            if (ifStemming)
                    lineCount = limiter;
            DocResult = "loading ranking.csv";
            using (StreamReader sr = File.OpenText(path))
            {
                // newList.Add(new KeyValuePair<int, int>(0, 0), sr.ReadLine());
                sr.ReadLine();
                string s = String.Empty;
                decimal temp = 0;
                decimal temp2 = 0;
                while ((s = sr.ReadLine()) != null && limiter != 0)
                {
                    count++;
                    if (ifStemming)
                    {
                        limiter--;
                    }
                   temp = (decimal)(count / lineCount);
                    temp2 = temp * 100;
                    if (temp2 != Progress)
                        Progress = Convert.ToInt32( temp2);
                    //remove blank lines
                    if (s == "")
                        continue;

                    string[] words = s.Split(',');
                    int userID = Int32.Parse(words[0]);
                    int movieID = Int32.Parse(words[1]);
                   // KeyValuePair <int, int> key = new KeyValuePair<int, int>(userID, movieID);
                    string value = s.Substring(words[0].Length + words[1].Length + 2);
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
