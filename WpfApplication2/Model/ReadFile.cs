using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IR_Engine
{
    /// <summary>
    /// This class will read the document data.
    /// This class will "know" to get a path of the folder where all the files are in. (after the unzip)
    /// Every file has lots of documents. It is madetory to identify the beginning of every document
    /// and the separate them accordingly.
    /// 
    /// 
    /// 
    /// 
    /// https://en.wikipedia.org/wiki/Rabin%E2%80%93Karp_algorithm
    /// </summary>
    public class ReadFile
    {
        //  public static int wordPosition = 0;
        //UTF!!!
        public string filesPathToDelete;
        // public static volatile int totalDocs = 0;
        // private static List<Thread> ReadFileThreads;

        // private static Semaphore _ReadFileSemaphore;
        static int counter;


        int gil = 0;

        public static SortedDictionary<string, string> OpenFileForParsing(string path)
        {
            Semaphore _ReadFileSemaphore = new Semaphore(8, 8); //one for every file
            counter = 10;
            Mutex _myFilePostings = new Mutex();
            List<Thread> ReadFileThreads = new List<Thread>();
            List<SortedDictionary<string, string>> DicList = new List<SortedDictionary<string, string>>();
            SortedDictionary<string, string> myFilePostings = new SortedDictionary<string, string>();
            // Reference 1:

            string fileName = path;
            //http://stackoverflow.com/questions/2161895/reading-large-text-files-with-streams-in-c-sharp

            int docNumberInFile = 0;
            int linesInDoc = 0;
            string newDocument = String.Empty;
            //https://msdn.microsoft.com/en-us/library/system.text.stringbuilder(v=vs.110).aspx#StringAndSB
            StringBuilder bufferDocument = new StringBuilder();

            using (StreamReader sr = File.OpenText(path))
            {
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    //remove blank lines
                    if (s == "")
                        continue;
                    string term = "<DOC>";
                    int docidx = NaiveSearch(s, term);

                    if (docidx != -1) //new term found in line!
                    {
                        if (linesInDoc != 0) //end of document before
                        {

                            //      System.Console.WriteLine(newDocument);

                            docNumberInFile++;

                            //update docnumbr
                            Indexer._DocNumber.WaitOne();

                            int freshNum = Interlocked.Increment(ref Indexer.docNumber);

                            Indexer._DocNumber.ReleaseMutex();

                            //  Console.WriteLine("Processed file :" + path + "| Found DOC#" + freshNum);




                            //the document in string
                            string str = bufferDocument.ToString();





                            // DoWork(ref _ReadFileSemaphore, ref _myFilePostings, str, freshNum, ref DicList);
                            Thread thread = new Thread(() => DoWork(ref _ReadFileSemaphore, ref _myFilePostings, str, freshNum, ref DicList, fileName));
                            // Start the thread, passing the number.





                            ReadFileThreads.Add(thread);




                            //remove metadata

                            //merge dic
                            //http://stackoverflow.com/questions/8459928/how-to-count-occurences-of-unique-values-in-dictionary

                            bufferDocument.Clear();

                        }
                        linesInDoc = 1;
                    }
                    else
                    {
                        linesInDoc++;
                        bufferDocument.Append(s.Trim() + System.Environment.NewLine);
                    }
                    //save the new document, line by line
                    // newDocument += s + System.Environment.NewLine;
                }
            }

            Console.WriteLine("File add to threadpool!");
            long fileSize = new System.IO.FileInfo(path).Length;
            Console.WriteLine("path:" + path);
            Console.WriteLine("File size: " + GetBytesReadable(fileSize));
            Console.WriteLine("Total amount:" + Indexer.docNumber + " Documents.");
            Console.WriteLine("Documents in file:" + docNumberInFile + " Documents.");
            Console.WriteLine("-----------------------");


            //   System.Console.WriteLine("Press any key to exit.");
            //   System.Console.ReadKey();

            //save doclist

            //DocumentFileToID

            // _pool.Release(4);

            Console.WriteLine("starting " + ReadFileThreads.Count + " threads...");
            foreach (Thread thred in ReadFileThreads)
            {

                //semaphore
                _ReadFileSemaphore.WaitOne();
                thred.Start();

            }



            foreach (Thread tread in ReadFileThreads)
            {
                tread.Join();
            }


            _myFilePostings.WaitOne();
            Console.WriteLine("Saving File postings on ReadFile-temp-RAM");

            foreach (SortedDictionary<string, string> dic in DicList)
            {
                //     SortedDictionary<string, string> dic2 = new SortedDictionary<string, string>(dic);
                foreach (KeyValuePair<string, string> entry in dic)
                    if (myFilePostings.ContainsKey(entry.Key))
                    {
                        myFilePostings[entry.Key] += entry.Value;
                        //   Console.WriteLine("Term Conflict:" + entry.Key.ToString());
                    }
                    else
                    {
                        myFilePostings.Add(entry.Key.ToString(), entry.Value);

                    }

            }

            Console.WriteLine("postings saved");
            _myFilePostings.ReleaseMutex();




            return myFilePostings;
            //    System.Console.Clear();
        }


        private static void DoWork(ref Semaphore _ReadFileSemaphore, ref Mutex _DictionaryListMutex, object path, int num, ref List<SortedDictionary<string, string>> DicList, string fileName)
        {
            string str = path.ToString();
            //   counter--;

            ///late edition
            //   _ReadFileSemaphore.WaitOne(); //limit threads




            SortedDictionary<string, string> newDict = Parse.parseString(str, num, fileName);
            //add to main memory first
            //  return newDict;
            _DictionaryListMutex.WaitOne();
            DicList.Add(newDict);
            //    ReadFile.saveDic(newDict, postingFilesPath + Interlocked.Increment(ref postingFolderCounter));
            //   counter++;
            _DictionaryListMutex.ReleaseMutex();
            _ReadFileSemaphore.Release();
        }



        /// <summary>
        /// http://stackoverflow.com/questions/7306214/append-lines-to-a-file-using-a-streamwriter
        /// </summary>
        /// <param name="dic">input dictionary.</param>
        /// <param name="directoryPath">output file (full path)</param>
        public static void saveDic(SortedDictionary<string, string> dic, string directoryPath)
        {
            //  var list = dic.Keys.ToList();
            // list.Sort();

            char last = ' ';
            string filePath;
            Directory.CreateDirectory(directoryPath);

            ///method: dictionary2file and file2dictionary
            Console.WriteLine("saving " + dic.Count + " terms to HDD payh: " + directoryPath);


            //new StreamWriter(Path, true, Encoding.UTF8, 65536))
            //http://www.jeremyshanks.com/fastest-way-to-write-text-files-to-disk-in-c/
            StreamWriter file2 = new StreamWriter(directoryPath + @"\misc.txt", true, Encoding.UTF8, 65536);
            // Loop through keys.
            foreach (var key in dic)
            {
                string term = key.Key.ToString();

                ///Writing Large Strings
                ///http://www.jeremyshanks.com/fastest-way-to-write-text-files-to-disk-in-c/
                ///

                char c = term[0];
                if (c == '<')
                {
                    int docnumber = Int32.Parse(term.Split(new char[] { '>', '|' })[1]);

                    Indexer._DocumentMetadata.WaitOne();
                    //  Console.WriteLine()
                    // char[] delimiterCharsLang = { '<', '>' };
                    Indexer.DocumentMetadata.Add(docnumber, key.Value);
                    Indexer._DocumentMetadata.ReleaseMutex();
                    continue;

                }
                else
                if (!Char.IsLetter(c))
                {
                    filePath = directoryPath + @"\misc.txt";
                    last = '?';
                }
                else
                {
                    if (last != term[0])
                    {
                        file2.Close();
                        filePath = directoryPath + @"\" + term[0] + ".txt";
                        file2 = new StreamWriter(filePath, true, Encoding.UTF8, 65536);
                    }
                }

                string line = key.Value;


                //deadlock
                file2.WriteLine(term + "^" + line);
                if (Char.IsLetter(term[0]))
                {
                    last = term[0];
                }
            }


            file2.Close();
        }

        public static SortedDictionary<String, String> fileToDictionary(string path)
        {
            SortedDictionary<string, string> newDic = new SortedDictionary<string, string>();

            using (StreamReader sr = File.OpenText(path))
            {
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    //remove blank lines
                    if (s == "")
                        continue;

                    int splitIndex = s.IndexOf('^');

                    string key = string.Empty;
                    string value = string.Empty;

                    if (splitIndex != -1)
                    {
                        key = s.Substring(0, splitIndex);
                        value = s.Substring(splitIndex + 1);
                    }
                    else
                    {
                        key = s;
                    }
                    if (!newDic.ContainsKey(key))
                        newDic.Add(key, value);
                    else
                    {
                        newDic[key] += value;
                    }

                }
            }
            return newDic;
        }

        /// <summary>
        /// SHOULD USE TEMPLATE
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SortedList<String, String> fileToSortedList(string path)
        {
            ///add comperer
            ///


            SortedList<string, string> newDic = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);


            string[] words;
            ///
            using (StreamReader sr = File.OpenText(path))
            {
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    //remove blank lines
                    if (s == "")
                        continue;

                    words = s.Split('^');
                    string value = string.Empty;
                    string key = words[0];
                    //maybe waseful
                    if (words.Length == 2)
                    {
                        value = words[1];
                    }
                    // Console.WriteLine("Term Num:" + newDic.Count);

                    newDic.Add(key, value);

                }
            }
            return newDic;
        }


        // Reference 2:
        public static int NaiveSearch(string str, string pattern, int index = 0)
        {

            if (str == null)
                return -1;
            int n = str.Length;
            int m = pattern.Length;
            bool find = true;
            for (int i = index; i < n - m + 1; i++)
            {
                find = true;
                for (int j = 0; j < m; j++)
                {
                    if (str.Substring(i + j, 1) != pattern.Substring(j, 1))
                    {
                        // i++;
                        find = false;
                        break; // jump to next iteration of outer loop
                    }
                }
                if (find)
                    return i;
            }
            return -1;
        }



    



        //http://www.somacon.com/p576.php
        //http://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
        // Returns the human-readable file size for an arbitrary, 64-bit file size 
        // The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        public static string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }
    }

}