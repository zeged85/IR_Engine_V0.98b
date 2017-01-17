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
            set { progress = value; NotifyPropertyChanged("Progress"); }
        }

        

        public static SortedDictionary<string, string> myPostings;
        // public static Dictionary<int, string> DocumentIDToFile = new Dictionary<int, string>();

        public static volatile SortedDictionary<string, string> DocumentMetadata = new SortedDictionary<string, string>();
        public static Mutex _DocumentMetadata;
        public static Mutex _mainMemory;
        private static Semaphore _pool;
        public static Mutex _DocNumber;
        private static List<Thread> threads = new List<Thread>();
        public static string documentsPath;
        public static string postingFilesPath/* = @"c:\IR_Engine\"*/;
        public static volatile int docNumber = 0;
        public static volatile int postingFolderCounter = 0;
        private static bool stopMemoryHandler = false;

        //public static string postingFilesPath = @"c:\IR_Engine\";
        List<string> PostingFileTermList = new List<string>();
        public static int amountOfUnique = 0;
        public static int wordNum = 0;
        public static bool ifStemming;

        public static SortedDictionary<string, string> stopWords;

        public static List<string> uniqueTerms = new List<string>();

        public static SortedDictionary<string, int> freqInAllCorpusList = new SortedDictionary<string, int>();

        public static SortedDictionary<string, int> Months = new SortedDictionary<string, int>();
        //  public static List<string> UniqueList = new List<string>();

        int FileCountInFolder;
        int FileCounter;

        public Indexer()
        {
            myPostings = new SortedDictionary<string, string>();
        }

        public void loadMonths()
        {
            Months.Add("january", 1);
            Months.Add("february", 2);
            Months.Add("march", 3);
            Months.Add("april", 4);
            Months.Add("may", 5);
            Months.Add("june", 6);
            Months.Add("july", 7);
            Months.Add("august", 8);
            Months.Add("september", 9);
            Months.Add("october", 10);
            Months.Add("november", 11);
            Months.Add("december", 12);

            Months.Add("jan", 1);
            Months.Add("feb", 2);

            Months.Add("apr", 4);

            Months.Add("jun", 6);
            Months.Add("jul", 7);
            Months.Add("aug", 8);
            Months.Add("sep", 9);
            Months.Add("oct", 10);
            Months.Add("nov", 11);
            Months.Add("dec", 12);


        }

     

        public void initiate()
        {
            _DocumentMetadata = new Mutex();
            _DocNumber = new Mutex();
            _pool = new Semaphore(3, 3);
            _mainMemory = new Mutex();

            if (Directory.Exists(documentsPath))
            {
                // This path is a directory
                //http://stackoverflow.com/questions/16193126/counting-the-number-of-files-in-a-folder-in-c-sharp
                FileCountInFolder = Directory.GetFiles(documentsPath).Length;
                stopWords = ReadFile.fileToDictionary(Indexer.documentsPath + "\\stop_words.txt" /*@"C:\stopWords\stop_words.txt"*/);// load stopwords
                ProcessDirectory(documentsPath, FileToParse);
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", documentsPath);
            }

            FileCounter = 0;

            foreach (Thread thread in threads)
            {
                thread.Start();
            }

          

            Thread memoryHanlder = new Thread(SavePostingToStaticDictionary);
            memoryHanlder.Priority = ThreadPriority.Highest;
            memoryHanlder.Start();


            foreach (Thread thread in threads)
            {
                thread.Join();
                Progress = (FileCounter++ * 100) / FileCountInFolder;
            }
            Console.WriteLine("Main thread exits.");

            stopMemoryHandler = true;
            Thread lastRefresh = new Thread(freeMemory);
            lastRefresh.Start();
            memoryHanlder.Join();
            lastRefresh.Join();
            threads.Clear();

        }

        public void freeMemory()
        {


            Console.WriteLine("Refreshing Memory...Last time");
            //create last foldet
            postingFolderCounter++;
           

            Directory.CreateDirectory(postingFilesPath + postingFolderCounter);
            ReadFile.saveDic(myPostings, postingFilesPath + postingFolderCounter);

            myPostings.Clear();

        }

        public void MergeAllToSingleUnSorted()
        {
            //creating the master directory
            //merging to single files un sorted.
            Console.WriteLine("merging to single files to un sorted.");
            if (Directory.Exists(postingFilesPath))
            {
                // This path is a directory
                ProcessDirectory(postingFilesPath, CreateMasterPostingFiles);
            }
        }

        public void sort()//and put in folder
        {
            Console.WriteLine("sorting");
            //sorting
            myPostings.Clear();

           // string dbpath = "PostingFiles";
           // if (Indexer.ifStemming == true)
             //   dbpath = "Stemming";

            string[] fileEntries = Directory.GetFiles(postingFilesPath);
            foreach (string fileName in fileEntries)
            {
                Console.WriteLine("loading file " + fileName);
                SortedDictionary<string, string> fileDic = ReadFile.fileToDictionary(fileName);
                Console.WriteLine("saving file to " + postingFilesPath + "PostingFiles"/*dbpath*/);
                ReadFile.saveDic(fileDic, postingFilesPath + "PostingFiles"/*dbpath*/ + @"\");
            }
            myPostings.Clear();
        }


        public void deleteGarbage()
        {
            //delete garbage
            //http://stackoverflow.com/questions/7296956/how-to-list-all-sub-directories-in-a-directory
            var directories = Directory.GetDirectories(postingFilesPath);

          //  string dbpath = "PostingFiles";
         //   if (Indexer.ifStemming == true)
           //     dbpath = "Stemming";



            Console.WriteLine("deleteing all folders...");
            foreach (string dirToBeDELETED in directories)
            {
                //delete all folders
                if (dirToBeDELETED != postingFilesPath + "PostingFiles"/*dbpath*/ && dirToBeDELETED != postingFilesPath + "Stemming")
                {
                    ProcessDirectory(dirToBeDELETED, DeletePostingFiles);
                    //delete directories
                    Directory.Delete(dirToBeDELETED, true);
                }
            }
            //delete all files
            Console.WriteLine("deleteing all files...");
            string[] fileEntries = Directory.GetFiles(postingFilesPath);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName, DeletePostingFiles);

        }


        public void dumpDocumentMetadata()
        {
            // compute DocumentFileToID file
            //most common term

            //dumping  DocumentMetadata

            StreamWriter file6 = new StreamWriter(postingFilesPath + @"\Metadata.txt", true);

            foreach (KeyValuePair<string, string> docMetadata in DocumentMetadata)
            {
                file6.WriteLine(docMetadata.Key + "$" + docMetadata.Value);
            }

            file6.Close();

            DocumentMetadata.Clear();
        }

        public void loadPostingFiles()
        {
            System.Console.WriteLine("Creating Dictionary... ");
            System.Console.WriteLine("Loading PostingFiles...");
            //0 create dictionary. use static
            myPostings.Clear();

         //   string dbpath = "PostingFiles";
          //  if (Indexer.ifStemming == true)
            //    dbpath = "Stemming";

            //upload postingfiles to main memomry Library<String,String>
            string[] fileEntries2 = Directory.GetFiles(postingFilesPath +  "PostingFiles"/*dbpath*/);
            foreach (string fileName in fileEntries2)
                ProcessFile(fileName, LoadPostingFiles);
        }

        public void loadMetadata()
        {
            //load DocumentMetadata

            using (StreamReader sr = File.OpenText(postingFilesPath + @"\Metadata.txt"))
            {
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    DocumentMetadata.Add(s.Split('$')[0], s.Split('$')[1]);
                }
            }
        }

        public void createDictionary() // count unique
        {

            // var list = myPostings.Keys.ToList();
            PostingFileTermList = myPostings.Keys.ToList();
            PostingFileTermList.Sort();

            System.Console.WriteLine("Computing stats.");
            // Loop through keys.
            //create 1 dic file
            StreamWriter file2 = new StreamWriter(postingFilesPath + @"\Dictionary.txt", true);

            List<Tuple<string, int>> shortDocAndFreq = new List<Tuple<string, int>>();
            int amountOfDocs = 0;
            foreach (var t in PostingFileTermList)
            {
                string term = t;
                char[] delimiterChars = { '@', ',' };
                int freqInAllCorpus = myPostings[term].Split(delimiterChars).Length - 1;
                freqInAllCorpusList.Add(term, freqInAllCorpus);

                //Unique terms' list

                if (freqInAllCorpus == 1)
                {
                    amountOfUnique++;
                    uniqueTerms.Add(term);
                }
                amountOfDocs = myPostings[term].Split('@').Length - 1;

                string[] FullPostString = myPostings[term].Split(' ');

                string post1 = FullPostString[0];
                foreach (string chunk in FullPostString)
                {
                    string newChunk = chunk.TrimEnd().TrimStart();
                    if (newChunk == "")
                        continue;

                    string[] splt = chunk.Split('@', ' ');
                    string docnum = splt[1];
                    int amountOfTimesTermInDoc = splt[0].ToString().Split(',').Length;
                    shortDocAndFreq.Add(new Tuple<string, int>(splt[1], amountOfTimesTermInDoc));

                }

                // myPostings[term] = "f:" + freqInAllCorpus;
                file2.Write(t.ToString() + " : #" + freqInAllCorpus + ", " + " #df : " + amountOfDocs);

                foreach (Tuple<string, int> tup in shortDocAndFreq)
                {
                    file2.Write(", (Doc#: " + tup.Item1 + " - " + tup.Item2 + ")");
                }
                file2.WriteLine();
                shortDocAndFreq.Clear();
            }
            file2.Close();

        }

        public void UniqueWordsQuery() //update metadata.txt
        {

            //Add list of unique terms to Documents string

            //loop all documents

            //add list
            System.Console.WriteLine("malloc array");

            int[] UniqueWordsDictionaryCounterForQuery = new int[docNumber + 1];

            //1.compute tf and df
            //2. list of document 
            //      System.Console.WriteLine("Press any key to exit.");
            //      System.Console.ReadKey();

            //METADATA

            System.Console.WriteLine("query:amount of Unique in corpus on doc");

            //amount of Unique in corpus on doc
            //111111111111111111111111111
            ///1111111111111111111111111111
            foreach (var term in uniqueTerms)
            {
                //list docs for every term
                List<string> docList = new List<string>();

                string debugstr = myPostings[term];
                string[] pars = debugstr.Split('@');

                for (int i = 1; i < pars.Length; i = i + 2)
                    docList.Add(pars[i].Split(' ')[0]);


                //O(N*M) <> O(N^2)

                //   update cunter for every doc

                //int DocID int counter


                ///222222222222222222222222222222222222222
                ////2222222222222222222222222
                foreach (string doc in docList)
                {

                    //NEED TO FIX
                    //NOT GOOD
                    //https://msdn.microsoft.com/en-us/library/bb397679.aspx

                    if (doc != "")
                    {
                        int number = Int32.Parse(doc);
                        UniqueWordsDictionaryCounterForQuery[number]++;
                    }
                }
            }

            StreamWriter file3 = new StreamWriter(postingFilesPath + @"\Metadata.txt", true);

            foreach (var post in DocumentMetadata)
            {
                //add stats
                //    UniqueList.Contains
                //    if (post.Key )

                int number = Int32.Parse(post.Key);

                file3.WriteLine(post.Key + ":" + post.Value + " #Unique in corpus:" + UniqueWordsDictionaryCounterForQuery[number]);
            }

            file3.Close();


        }

        public void PrintfreqInAllCorpusList()
        {
            //  using System.Linq;
            //http://stackoverflow.com/questions/21411384/sort-dictionary-string-int-by-value

            var top10 = freqInAllCorpusList.OrderByDescending(pair => pair.Value).Take(10);
            var bottom10 = freqInAllCorpusList.OrderBy(pair => pair.Value).Take(10);

            top10 = freqInAllCorpusList.OrderByDescending(pair => pair.Value).Take(10)
                  .ToDictionary(pair => pair.Key, pair => pair.Value);

            bottom10 = freqInAllCorpusList.OrderBy(pair => pair.Value).Take(10)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
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

        //https://msdn.microsoft.com/en-us/library/07wt70x2(v=vs.110).aspx
        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        //http://stackoverflow.com/questions/2082615/pass-method-as-parameter-using-c-sharp
        public static void ProcessDirectory(string targetDirectory, Func<string, int> myMethodName)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);

            Array.Sort(fileEntries, new AlphanumComparatorFast());

            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName, myMethodName);
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

            Array.Sort(subdirectoryEntries, new AlphanumComparatorFast());
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory, myMethodName);
        }

        /// <summary>
        /// 
        /// </summary>
        int LoadPostingFiles(string path)
        {
            //  myPostings.Add

            SortedDictionary<String, String> newDict = ReadFile.fileToDictionary(path);
            Console.WriteLine("Loading File '{0}'.", path);
            foreach (KeyValuePair<string, string> entry in newDict)

                ////added if statement GIL!!!!
                // if (!myPostings.ContainsKey(entry.Key))
                myPostings.Add(entry.Key.ToString(), entry.Value);
            // else
            //   myPostings[entry.Key] += entry.Value;
            Console.WriteLine("File loaded.");
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Full path to file about to be delete</param>
        /// <returns></returns>
        int DeletePostingFiles(string path)
        {
            //CAREFUL
            File.Delete(path);
            return 0;
        }

        /// <summary>
        /// create the final unsorted fully, appending folder-by semi posting files
        /// </summary>
        /// <param name="path"> semi-posting-file path to read and save in the master directory</param>
        /// <returns>success</returns>
        int CreateMasterPostingFiles(string path)
        {
            string[] fullname = path.Split('\\');
            string filename = fullname[fullname.Length - 1];
            if (!File.Exists(postingFilesPath + filename))
                File.Copy(path, postingFilesPath + filename);
            else
            {
                SortedDictionary<string, string> myDict = new SortedDictionary<string, string>();
                using (StreamReader fileToRead = File.OpenText(path))
                {
                    string lineRead = String.Empty;
                    while ((lineRead = fileToRead.ReadLine()) != null)
                    {
                        string[] termAndValue = lineRead.Split('^');

                        if (myDict.ContainsKey(termAndValue[0]))
                            myDict[termAndValue[0]] += " " + termAndValue[1];
                        else
                            myDict.Add(termAndValue[0], termAndValue[1]);
                    }
                }
                ReadFile.saveDic(myDict, postingFilesPath);
            }
            return 0;
        }
        //http://stackoverflow.com/questions/3360555/how-to-pass-parameters-to-threadstart-method-in-thread
        private static void DoWork(object path)
        {
            string str = path.ToString();
            _pool.WaitOne(); //limit threads
            SortedDictionary<string, string> newDict = ReadFile.OpenFileForParsing(str);

            //add to main memory first

            _mainMemory.WaitOne();
            Console.WriteLine("Saving postings on RAM");
            foreach (KeyValuePair<string, string> entry in newDict)
                if (myPostings.ContainsKey(entry.Key))
                    myPostings[entry.Key] += " " + entry.Value;
                else
                    myPostings.Add(entry.Key.ToString(), entry.Value);

            Console.WriteLine("postings saved");
            _mainMemory.ReleaseMutex();

            //    ReadFile.saveDic(newDict, postingFilesPath + Interlocked.Increment(ref postingFolderCounter));
            _pool.Release();
        }

        void SavePostingToStaticDictionary()
        {
            while (!stopMemoryHandler)
            {
                if (myPostings.Count > 30000)
                {
                    _mainMemory.WaitOne();
                    SortedDictionary<string, string> freeDic = new SortedDictionary<string, string>(myPostings);
                    myPostings.Clear();
                    _mainMemory.ReleaseMutex();
                    Console.WriteLine("Refreshing Memory...");


                    postingFolderCounter++;
                    Directory.CreateDirectory(postingFilesPath + postingFolderCounter);
                    ReadFile.saveDic(freeDic, postingFilesPath + postingFolderCounter);


                }

//                Progress = docNumber / 1400;

                //https://social.msdn.microsoft.com/Forums/vstudio/en-US/660a1f75-b287-4565-bfdd-75105e0a5527/c-wait-for-x-seconds?forum=netfxbcl
                System.Threading.Thread.Sleep(1500);
            }
        }

        int FileToParse(string path)
        {
            //THREADING
            //https://msdn.microsoft.com/en-us/library/system.threading.semaphore(v=vs.110).aspx

            // Thread t = new Thread(new ParameterizedThreadStart(DoWork));
            DoWork(path);
            Thread thread = new Thread(() => DoWork(path));
            // Start the thread, passing the number.

            threads.Add(thread);

            //threading
            //http://stackoverflow.com/questions/13181740/c-sharp-thread-safe-fastest-counter

            return 0;
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path, Func<string, int> myMethodName)
        {

            Console.WriteLine(myMethodName.Method.ToString() + ":Processing file '{0}'.", path);
            myMethodName(path);
        }

        public void move(double speed, int angle)
        {
            throw new NotImplementedException();
        }
    }
}
