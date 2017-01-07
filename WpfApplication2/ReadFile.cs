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
        public static int wordPosition = 0;
        //UTF!!!
        public string filesPathToDelete;
        public static int totalDocs = 0;

        public static Dictionary<string, string> OpenFileForParsing(string path)
        {
            Dictionary<string, string> myPostings = new Dictionary<string, string>();
            // Reference 1:
            //http://stackoverflow.com/questions/2161895/reading-large-text-files-with-streams-in-c-sharp

            int docNumber = 0;
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
                            ReadFile.wordPosition = 0;
                            //      System.Console.WriteLine(newDocument);
                            Indexer.docNumber++;
                            docNumber++;
                            totalDocs++;
                            Parse.countAmountOfUniqueInDoc = 0;

                            Console.WriteLine("Document #: " + Indexer.docNumber);
                            Console.WriteLine("Document #: " + docNumber);
                            Console.WriteLine("Processed file '{0}'.", path);
                            System.Console.WriteLine("Lines in document:" + linesInDoc);


                            

                            Dictionary<string, string> newDict = Parse.parseString(bufferDocument.ToString());
                            //dictionary resault

                            //remove metadata

                            //merge dic
                            //http://stackoverflow.com/questions/8459928/how-to-count-occurences-of-unique-values-in-dictionary

                            System.Console.WriteLine("terms in document:" + newDict.Count);
                            System.Console.WriteLine("Merging in ReadFile...");
                            foreach (KeyValuePair<string, string> entry in newDict)
                                if (myPostings.ContainsKey(entry.Key))
                                    myPostings[entry.Key] += " " + entry.Value + "}@" + Indexer.docNumber;
                                else
                                    myPostings.Add(entry.Key.ToString(), entry.Value + "}@" + Indexer.docNumber);
                            System.Console.WriteLine("Merging in ReadFile... Done.");

                            System.Console.WriteLine("Deleteing string...");
                            //newDocument = String.Empty; //refresh string

                            //print original

                            bufferDocument.Clear();
                            // System.Console.WriteLine("Deleteing stringg... Done.");
                            //      printDic(myPostings);
                            //      Console.WriteLine("-------------------------------");
                            //           Console.WriteLine("Press any key to continue.");
                            //      System.Console.ReadKey();
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

            Console.WriteLine("File Processed!");
            long fileSize = new System.IO.FileInfo(path).Length;
            Console.WriteLine("File size: " + GetBytesReadable(fileSize));
            Console.WriteLine("Total amount:" + Indexer.docNumber + " Documents.");
            Console.WriteLine("Documents in file:" + docNumber + " Documents.");
            Console.WriteLine("-----------------------");

            //  printDic(myPostings);
            //  saveDic(myPostings,"");

            //   System.Console.WriteLine("Press any key to exit.");
            //   System.Console.ReadKey();

            //save doclist

            //DocumentFileToID

            return myPostings;
            //    System.Console.Clear();
        }
        /// <summary>
        /// http://stackoverflow.com/questions/7306214/append-lines-to-a-file-using-a-streamwriter
        /// </summary>
        /// <param name="dic">input dictionary.</param>
        /// <param name="directoryPath">output file (full path)</param>
        public static void saveDic(Dictionary<string, string> dic, string directoryPath)
        {
            var list = dic.Keys.ToList();
            list.Sort();
            char last = ' ';
            string filePath;
            Directory.CreateDirectory(directoryPath);

            ///method: dictionary2file and file2dictionary


            StreamWriter file2 = new StreamWriter(directoryPath + @"\misc.txt", true);
            // Loop through keys.
            foreach (var key in list)
            {

                if (!Char.IsLetter(key[0]))
                {
                    filePath = directoryPath + @"\misc.txt";
                    last = '?';
                }
                else
                {
                    if (last != key[0])
                    {
                        file2.Close();
                        filePath = directoryPath + @"\" + key[0] + ".txt";
                        file2 = new StreamWriter(filePath, true);
                    }
                }

                string line = dic[key];
                //deadlock
                file2.WriteLine(key + ": " + line);
                if (Char.IsLetter(key[0]))
                {
                    last = key[0];
                }
            }
            file2.Close();
        }

        public static Dictionary<String, String> fileToDictionary(string path)
        {
            Dictionary<string, string> newDic = new Dictionary<string, string>();

            using (StreamReader sr = File.OpenText(path))
            {
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    //remove blank lines
                    if (s == "")
                        continue;

                    string[] words = s.Split(':');
                    string value = string.Empty;
                    string key = words[0];
                    if (words.Length == 2)
                        value = words[1];
                    if (!newDic.ContainsKey(key))
                        newDic.Add(key, value);
                    else
                        newDic[key] += value;

                }
            }
            return newDic;
        }

        public static void printDic(Dictionary<string, string> dic)
        {
            var list = dic.Keys.ToList();
            list.Sort();

            // Loop through keys.
            foreach (var key in list)
            {
                Console.WriteLine("{0}: {1}", key, dic[key]);
            }
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
