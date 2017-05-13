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
                if (c=='<')
                {
                    int docnumber = Int32.Parse( term.Split(new char[] { '>', '|' })[1]) ;
             
             //       Indexer._DocumentMetadata.WaitOne();
                    //  Console.WriteLine()
                    // char[] delimiterCharsLang = { '<', '>' };
             //       Indexer.DocumentMetadata.Add(docnumber , key.Value);
            //       Indexer._DocumentMetadata.ReleaseMutex();
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

        public static List<string> loadMoviesFile(string path)
        {
            List<string> newList = new List<string>();
            int lineCount = File.ReadLines(@"C:\file.txt").Count();
            using (StreamReader sr = File.OpenText(path))
            {
                
                newList.Add(sr.ReadLine());
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    //remove blank lines
                    if (s == "")
                        continue;

                    string[] words = s.Split(',');
                    string value = s.Substring(words[0].Length+1);
                    string key = words[0];


                    newList.Add(value);
                   

                }
            }
            return newList;
        }


        public static Dictionary<KeyValuePair<int, int>, string> loadRatingsFile(string path)
        {
            Dictionary<KeyValuePair<int, int>, string> newList = new Dictionary<KeyValuePair<int, int>, string>();

            using (StreamReader sr = File.OpenText(path))
            {
                newList.Add(new KeyValuePair<int,int>(0,0) , sr.ReadLine() );
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    //remove blank lines
                    if (s == "")
                        continue;

                    string[] words = s.Split(',');
                    KeyValuePair<int, int> key = new KeyValuePair<int, int>(Int32.Parse(words[0]),Int32.Parse( words[1]) );
                    string value = s.Substring(words[0].Length + words[1].Length + 2);
                   // string key = words[0];


                    newList.Add(key,value);


                }
            }
            return newList;
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
                    if (words.Length == 2) { 
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
