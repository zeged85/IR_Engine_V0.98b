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

        //Ratings Dictionary
        //userId,movieId,rating,timestamp
        public static volatile Dictionary<KeyValuePair<int,int>, string> myRatings;


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

            
           foreach (string s in myMovies)
            {
                string title = s.Split(new string[] { ")," }, StringSplitOptions.None)[0] + ')';
                if (!myMoviesDictionary.ContainsKey(title))
                {
                    myMoviesDictionary.Add(title, 0);
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

                

                DocResult = "loading Movies.csv";
                myMovies = loadMoviesFile(documentsPath + @"\movies.csv");
                titleNumber = myMovies.Count - 1;



                DocResult = "loading ratings.csv";
                myRatings = loadRatingsFile(documentsPath + @"\ratings.csv");

            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", documentsPath);
            }

            FileCounter = 0;






        }

    
      public  List<string> autocomplete(string query)
        {

            var keys = Indexer.myMoviesDictionary.Keys.Where(x => x.Contains(query));
            List<string> termList = keys.ToList();

            return termList;
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


        public  Dictionary<KeyValuePair<int, int>, string> loadRatingsFile(string path)
        {
            Dictionary<KeyValuePair<int, int>, string> newList = new Dictionary<KeyValuePair<int, int>, string>();
            int lineCount = File.ReadLines(path).Count();
            int count = 0;
            int limiter = 100000;
            lineCount = limiter;
            using (StreamReader sr = File.OpenText(path))
            {
                newList.Add(new KeyValuePair<int, int>(0, 0), sr.ReadLine());
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null && limiter != 0)
                {
                    count++;
                    limiter--;
                    Progress = count*100 / lineCount;
                    //remove blank lines
                    if (s == "")
                        continue;

                    string[] words = s.Split(',');
                    KeyValuePair<int, int> key = new KeyValuePair<int, int>(Int32.Parse(words[0]), Int32.Parse(words[1]));
                    string value = s.Substring(words[0].Length + words[1].Length + 2);
                    // string key = words[0];


                    newList.Add(key, value);


                }
            }
            return newList;
        }


    }

       

    

      
    
}
