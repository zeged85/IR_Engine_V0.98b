using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IR_Engine
{
    public class Searcher : ISearcher
    {
        /// </summary>
        public static Dictionary<string, string> SYNONYMS_AND_ANTONYMS_Dictionary = new Dictionary<string, string>();
        public event PropertyChangedEventHandler PropertyChanged;
        public static string pathForResult;

        public static List<string> languageChosen = new List<string>();


        public static string singleQueryInput;

        public void NotifyPropertyChanged(string PropName)
        {

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropName));

        }

        private string docResult;
        public string DocResult
        {
            get { return docResult; }
            set { docResult = value; NotifyPropertyChanged("Searcher_DocResult"); }
        }

        string OutputFolder = "";

        public void setOutputFolder(string path)
        {
            OutputFolder = path;
        }
        public string getOutputFolder()
        {
            return OutputFolder;
        }

        public Searcher()
        {
            ///load SYNONYMS_AND_ANTONYMS
            ///

            LoadSYNONYMS_AND_ANTONYMS();
        }



        public string[] getSYNONYMS(string term)
        {

            var keys = SYNONYMS_AND_ANTONYMS_Dictionary.Keys.Where(x => x.ToLower().Contains(term.ToLower()));
            List<string> MatchingTerms = keys.ToList();

            List<string> res = new List<string>();
            SortedDictionary<string, int> synRank = new SortedDictionary<string, int>();
            foreach (string str in MatchingTerms)
            {

                string[] possibleSyn = str.Split(',', '.', ';', ' ');

                foreach (string termSyn in possibleSyn)
                {
                    if (termSyn != "" && termSyn.ToLower() != term.ToLower() && termSyn != "ABBREVIATIONS")
                    {
                        if (synRank.ContainsKey(termSyn) )
                        {
                            synRank[termSyn] += 1;
                        }
                        else
                        {
                            synRank.Add(termSyn, 1);
                        }
                    }
                }

                var orderByVal = synRank.OrderBy(v => v.Value);


                var desc = orderByVal.Reverse();

                int limiter = 0; //TOP 5
                foreach (KeyValuePair<string, int> termResult in desc)
                {
                    limiter++;
                    if (!res.Contains(termResult.Key))
                    {
                        res.Add(termResult.Key);
                    }
                    if (limiter == 2)
                    {
                        break;
                    }

                }


                //res.Add("");
            }
            int limiter2 = 0;
            string[] ans = null;
            if (res.Count != 0)
            {
                foreach (string s in res)
                {
                    limiter2++;
                    if (!res.Contains(s))
                    {
                        res.Add(s);
                    }
                    if (limiter2 == 2)
                    {
                        break;
                    }
                }
                ans = res.ToArray();

            }

            string[] finalTwo = null;
            
            if (ans != null && ans.Count() == 1)
            {
                finalTwo = new string[1];
                finalTwo[0] = res[0];

            }

            if (ans != null && ans.Count() > 1)
            {
                finalTwo = new string[2];
                finalTwo[0] = res[0];
                finalTwo[1] = res[1];
            }

            return finalTwo;
        }

        public void LoadSYNONYMS_AND_ANTONYMS()
        {
            //open file

            //parse to dictionary


            string ANTONYMS = "";
            string SYNONYMS = "";
            using (StreamReader sr = File.OpenText(@"C:\treceval\SYNONYMS_AND_ANTONYMS.txt"))
            {
                string s = String.Empty;

                string txt = "";
                while ((s = sr.ReadLine()) != null)
                {
             
                    //remove blank lines
                    if (s == "SYNONYMS AND ANTONYMS" || s.Any(char.IsDigit))
                        continue;
                    //  txt = s;
                    //remove blank lines
                    if (s == "")
                    {
                        if (txt != "" && SYNONYMS_AND_ANTONYMS_Dictionary.ContainsKey(txt))
                        {

                        }
                        else
                        {
                            ///http://stackoverflow.com/questions/18251875/in-c-how-to-check-whether-a-string-contains-an-integer
                            if (txt != "" && !txt.Contains('[') && !txt.Contains(']') && !txt.Any(char.IsDigit) && txt != "SYNONYMS AND ANTONYMS ")
                            {
                                if (txt.Contains("ANT."))
                                {
                                    ANTONYMS = txt.Substring(5);
                                    if (SYNONYMS_AND_ANTONYMS_Dictionary.ContainsKey(SYNONYMS))
                                    {

                                    }
                                    else
                                    {
                                        SYNONYMS_AND_ANTONYMS_Dictionary.Add(SYNONYMS, ANTONYMS);
                                    }
                                    SYNONYMS = "";
                                    ANTONYMS = "";
                                }
                                else
                                {
                                    if (SYNONYMS == "")
                                    {
                                        SYNONYMS = txt;
                                    }
                                    else
                                    {
                                        if (SYNONYMS_AND_ANTONYMS_Dictionary.ContainsKey(SYNONYMS))
                                        {

                                        }
                                        else
                                        {
                                            SYNONYMS_AND_ANTONYMS_Dictionary.Add(SYNONYMS, "");

                                        }
                                        SYNONYMS = txt;
                                    }
                                }

                            }
                            else
                            {
                                if (txt == "")
                                {

                                }
                                else
                                {

                                }

                            }
                        }
                        txt = "";
                        continue;

                    }
                    else
                    {

                        if (s[s.Length - 2] == '-')
                            s = s.Substring(0, s.Length - 2);
                        txt += s;
                    }
                }
            }




        }


        /// <summary>
        /// gil gil
        /// </summary>
        /// <param name="queryInput"></param>
        public void runSingleQuery(string queryInput, string[] SYNONYMS)
        {
            //niros
            //remove blank lines
            if (!string.IsNullOrEmpty(queryInput))
            {
                int space = queryInput.IndexOf(' ');
                Random rand = new Random();
                int query_id = rand.Next(0, 1000);
                //  bool isValidInteger = int.TryParse(queryInput.Substring(0, space), out query_id);
                //  if (!isValidInteger)
                //  {
                ////
                //  }

                string query = queryInput.Substring(space + 1);
                //first query in file
                SortedDictionary<string, double> docRankRes = processFullTermQuery(query);


               // SortedDictionary<string, double> SYNONYMSdocRankRes = new SortedDictionary<string, double>();

                foreach (string syn in SYNONYMS)
                {
                    SortedDictionary<string, double> SYNONYMSdocRankRes = processFullTermQuery(syn);
                    foreach (KeyValuePair<string, double> pair in SYNONYMSdocRankRes)
                    {
                        if (docRankRes.ContainsKey(pair.Key))
                        {
                            docRankRes[pair.Key] += pair.Value;
                        }
                        else
                        {
                            docRankRes.Add(pair.Key, pair.Value);
                        }
                    }
                }


                var orderByVal = docRankRes.OrderBy(v => v.Value);

                //reverse descending
                //http://stackoverflow.com/questions/7815930/sortedlist-desc-order

                orderByVal.Reverse();
                var desc = orderByVal.Reverse();

                //better just change comperer
                //http://stackoverflow.com/questions/7815930/sortedlist-desc-order

                //sort by val double
                //append results to file

                if (File.Exists(pathForResult + "\\result.txt"))
                {
                    File.Delete(pathForResult + "\\result.txt");
                }
                StreamWriter file6 = new StreamWriter(pathForResult + "\\result.txt"/*@"c:\treceval\results.txt"*/, true);
                /// 351   0  FR940104-0-00001  1   42.38   mt
                int limiter = 0;
                foreach (KeyValuePair<string, double> docResult in desc)
                {
                    limiter++;
                    ///query ID - ITER = 0   - 
                    file6.WriteLine(query_id + " " + "0" + " " + docResult.Key + " " + "0" + " " + "1.1" + " " + "mt");
                    if (limiter == 50)
                    {
                        break;
                    }
                }
                file6.Close();
            }
        }


        public void openQueryFile(string path)
        {
            SortedDictionary<string, string> newDic = new SortedDictionary<string, string>();
            using (StreamReader sr = File.OpenText(path))
            {
                if (File.Exists(pathForResult + "\\result.txt"))
                {
                    File.Delete(pathForResult + "\\result.txt");
                }
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    //remove blank lines
                    if (s == "")
                        continue;
                    int space = s.IndexOf(' ');
                    int query_id;
                    bool isValidInteger = int.TryParse(s.Substring(0, space), out query_id);
                    if (!isValidInteger)
                    {
                        ////
                    }

                    string query = s.Substring(space + 1);
                    //first query in file




                    query = query.Trim();

                    if (query.Last().ToString() == ".")
                    {
                        query = query.Substring(0, query.Length - 2);
                    }
                    if (query.Contains(','))
                    {
                        query = query.Remove(query.IndexOf(','));
                    }

                    if (query.Contains(' '))
                    {
                        query = query.Replace(' ', '+');
                    }







                    string[] allTerms = query.Split('+');
                    List<string> syn = new List<string>();

                    foreach (string term in allTerms)
                    {
                        string[] SYNOms = getSYNONYMS(term);

                        if (SYNOms != null)
                        {
                            foreach (string str in SYNOms)
                            {
                                if (!syn.Contains(str))
                                {
                                    syn.Add(str);
                                }
                            }
                       
                        }
                     

                    }
                    string[] SYNONYMS = syn.ToArray();


                    /*
                    foreach (string syn2 in SYNONYMS)
                    {
                        SortedDictionary<string, double> SYNONYMSdocRankRes = processFullTermQuery(syn2);
                        foreach (KeyValuePair<string, double> pair in SYNONYMSdocRankRes)
                        {
                            if (SYNONYMSdocRankRes.ContainsKey(pair.Key))
                            {
                                docRankRes[pair.Key] += pair.Value;
                            }
                            else
                            {
                                docRankRes.Add(pair.Key, pair.Value);
                            }
                        }
                    }
                    */




                    if (Indexer.ifStemming == true)
                    {
                        Stemmer stem = new Stemmer();


                        if (query.Contains('+'))
                        {
                            string[] str = query.Split('+');

                            query = stem.stemTerm(str[0]);

                            foreach (string tri in str)
                            {
                                if (tri == str[0])
                                    continue;
                                query += "+" + stem.stemTerm(tri);
                            }
                        }
                        else
                        {
                            query = stem.stemTerm(query);
                        }

                    }




                    runSingleQuery(query, SYNONYMS);

                    
                    /*
                    SortedDictionary<string, double> docRankRes = processFullTermQuery(query);

                    var orderByVal = docRankRes.OrderBy(v => v.Value);

                    //reverse descending
                    //http://stackoverflow.com/questions/7815930/sortedlist-desc-order

                    //    orderByVal.Reverse();
                    var desc = orderByVal.Reverse();

                    //better just change comperer
                    //http://stackoverflow.com/questions/7815930/sortedlist-desc-order

                    //sort by val double
                    //append results to file

                    StreamWriter file6 = new StreamWriter(pathForResult + "\\result.txt", true);
                    /// 351   0  FR940104-0-00001  1   42.38   mt
                    int limiter = 0;
                    foreach (KeyValuePair<string, double> docResult in desc)
                    {
                        limiter++;
                        ///query ID - ITER = 0   - 
                        file6.WriteLine(query_id + " " + "0" + " " + docResult.Key + " " + "0" + " " + "1.1" + " " + "mt");
                        if (limiter == 50)
                        {
                            break;
                        }
                    }
                    file6.Close();
            
                    */}
            }
            
        }

        /// MVVM

        //https://www.dotnetperls.com/tuple
        public static Document getDocData(int doc)
        {
            // string ans = string.Empty;

            if (Indexer.DocumentMetadata.ContainsKey(doc))
            {
                //parse doc data
                string[] val = Indexer.DocumentMetadata[doc].Split('#');

                string DOCNO = val[0];
                string mostFreqTermInDoc = val[1];
                int maxOccurencesInDocument = Int32.Parse(val[2]);
                string language = val[3];
                int uniqueInDocAmount = Int32.Parse(val[4]);
                int totalInDocIncludingSW = Int32.Parse(val[5]);
                int totalInDocwithoutSW = Int32.Parse(val[6]);
                int AmountUniqueInCorpus = Int32.Parse(val[7]);


                ///12: FBIS3-521 , ifp : 5, English , uniqueInDoc : 74, totalInDocIncludingSW : 201, totalInDocwithoutSW : 114}@12 #Unique in corpus:3
                ///13: FBIS3-85 , 2-28 : 2, French , uniqueInDoc : 66, totalInDocIncludingSW : 164, totalInDocwithoutSW : 89}@13 #Unique in corpus:3
                ///14: FBIS3-52 , report : 32, , uniqueInDoc : 562, totalInDocIncludingSW : 1885, totalInDocwithoutSW : 1177}@14 #Unique in corpus:129
                ///15: FBIS3-86 , oau : 6, Arabic , uniqueInDoc : 95, totalInDocIncludingSW : 264, totalInDocwithoutSW : 152}@15 #Unique in corpus:8
                ///    ans = DOCNO + "| max_tf (in Doc):" + mostFreqTermInDoc + "| tf (in Doc):" + maxOccurencesInDocument +
                ///     "| Language:" + language + "| UniqueInDoc :" + uniqueInDocAmount + "| totalInDocIncludingSW :" + totalInDocIncludingSW
                ///   + "| totalInDocwithoutSW:" + totalInDocwithoutSW + "| AmountUniqueInCorpus:" + AmountUniqueInCorpus;


                //}




                //Tuple<int, string, bool> tuple =
                return new Document(DOCNO, mostFreqTermInDoc, maxOccurencesInDocument, language, uniqueInDocAmount, totalInDocIncludingSW, totalInDocwithoutSW, AmountUniqueInCorpus);
            }
            else
            {
                return null;
            }
        }

        public string getTermValue(string term)
        {
            string val = "";// = Indexer.myDictionary[term];



            // string line = File.ReadLines(FileName).Skip(14).Take(1).First();





            string outFolder = getOutputFolder() + @"PostingFiles\";
            char c = term[0];
            if (char.IsLetter(c))
            {
                outFolder += c.ToString() + ".txt";


            }
            else
            {
                outFolder += "misc.txt";
            }


            using (StreamReader sr = File.OpenText(outFolder))
            {
                string s = String.Empty;

                while ((s = sr.ReadLine()) != null)
                {
                    //remove blank lines
                    // if (s == "")
                    //     continue;
                    string[] words = s.Split('^');
                    string comp = words[0];

                    if (comp == term)
                    {
                        val = words[1];
                        continue;
                    }


                }
            }

            // if ()

            return val;
        }

        public Tuple<string, SortedList<int, int>, int, int, int> getReleventDocumentsOfSingleTerm(string querySingleTerm, int qfi)
        {

            SortedList<int, int> termResult = new SortedList<int, int>();
            //#tf
            int termFrequency = 0; // in all corpurus //single
            //#df
            int documentFrequenct = 0;

            ///http://stackoverflow.com/questions/9007990/how-to-do-like-on-dictionary-key
            //  var matchingKeys = Indexer.myDictionary.Keys.Where(x => x.Contains(queryFullTerm));
            //bool superSearch = false;
            List<string> MatchingTerms = new List<string>();
            querySingleTerm = querySingleTerm.ToLower();
            string[] str = querySingleTerm.Split('+');
            IEnumerable<string> keys;


            if (str.Length > 1 && str[0].Length > 0 && str[1].Length > 0)
            {
                keys = Indexer.myDictionary.Keys.Where(x => x.Contains(querySingleTerm));
                MatchingTerms = keys.ToList();
            }
            else
            {
                if (Indexer.myDictionary.ContainsKey(querySingleTerm))
                {
                    MatchingTerms.Add(querySingleTerm);
                }
            }




            foreach (string match in MatchingTerms)
            {



                ///retrieve term data from HDD
                string val = Indexer.myDictionary[match]; // getTermValue(match);


                int TMPtermFrequency;
                int TMPdocumentFrequenct;
                //count docs
                char[] delimiterCharsLang = { '#' };
                string[] termData = val.Split(delimiterCharsLang);

                int.TryParse(termData[1], out TMPtermFrequency);
                int.TryParse(termData[2], out TMPdocumentFrequenct);

                termFrequency = TMPtermFrequency;
                documentFrequenct = TMPdocumentFrequenct;



                Console.WriteLine("Term: " + '"' + querySingleTerm + '"' + " " + "tf:" + termFrequency + " df:" + documentFrequenct + " qfi:" + qfi);


                /////////////////

                int length = termData.Length;
                int document;
                int frequency;

                //   termResult = new SortedList<int, int>[1];

                //relervent documents. sorted by docNum, sort/rank by termFreq

                //    termResult[0] = new SortedList<int, int>();

                //TERM1, DOCS + #
                //TERM1-TERM2, DOCS + #

                ///popular -term
                /// TERM1-TERM2  ^#3 freqInAllCorpus  #2 amountOfDocs | (DOC#:2 - 2 TIMES) (DOC#:4 - 1 TIMES)
                /// 10-percen ^# 2 # 2 # 29#1 # 77#1
                ///
                /// 
                ///10-percen+percent^#2#2#29#1#77#1
                ///10-yea^#2#2#40#1#88#1
                ///10-yea+year^#2#2#40#1#88#1
                ///




                ///return most popular doc
                ///
                /// sort/rank docs 1.mostPopularTerm+freq 2.amountOfUniqueInDoc by 3.SizeOfDoc w/wo SW
                /// 
                ///2$ FBIS3-2 , public : 95, , uniqueInDoc : 817, totalInDocIncludingSW : 4328, totalInDocwithoutSW : 3101}@2

                //find all relevent documents

                //rank docs by termFreqinDoc




                //PARSE DICTIONARY TERMPOST_TUPLE TO DOC#-FREQ
                //FIRST TERM
                for (int i = 3; i < length - 1; i = i + 2)
                {
                    int.TryParse(termData[i], out document); //DOCUMENT
                    int.TryParse(termData[i + 1], out frequency); //FREQUENCY

                    if (termResult.ContainsKey(document))
                    {
                        termResult[document] += frequency;
                    }
                    else
                    {
                        termResult.Add(document, frequency); //ADD to LIST OF DOCS
                    }                              //doc
                    //freq
                }




                //termresult
                //return list of docs + termFrequency + documentFreq




                ///language
                ///
                //retrieve final document

                //use postingList??
                //https://www.youtube.com/watch?v=IZTJgMjE5jw -- recommended




                //return document list


            }
            return new Tuple<string, SortedList<int, int>, int, int, int>(querySingleTerm, termResult, termFrequency, documentFrequenct, qfi);
        }

        public void sortDocList(SortedList<int, int> termResult)
        {

            //GET ALL lISTS OF DOCS
            //AND SORT THEM BY FREQ

            //  termResult.sortby freq


            ///http://stackoverflow.com/questions/28923062/sortedlistint-int-sort-by-value
            ///var orderByVal = sortedlist.OrderBy(v => v.Value);
            ///

            var orderByVal = termResult.OrderBy(v => v.Value);

            //reverse descending
            //http://stackoverflow.com/questions/7815930/sortedlist-desc-order

            orderByVal.Reverse();

            var desc = orderByVal.Reverse();

            //better just change comperer
            //http://stackoverflow.com/questions/7815930/sortedlist-desc-order



            DocResult += System.Environment.NewLine;
            //display the elements of the sortedlist after sorting it with Value's
            Console.WriteLine("The elements in the SortedList after sorting :");
            foreach (KeyValuePair<int, int> pair in desc)
            {

                Console.WriteLine("DocNo:{0} => Popularity:{1}", pair.Key, pair.Value);

                DocResult += "DocNo: {" + pair.Key + "} => Popularity: {" + pair.Value + "}" + getDocData(pair.Key);

                DocResult += System.Environment.NewLine;

            }

        }


        ///http://stackoverflow.com/questions/3663546/most-efficient-way-to-find-the-index-of-an-item-in-a-sorteddictionary
        ///http://stackoverflow.com/questions/8090786/c-sharp-sorted-list-how-to-get-the-next-element
        ///http://stackoverflow.com/questions/6131827/how-do-i-get-the-element-with-the-smallest-key-in-a-collection-in-o1-or-olog
        ///CREATE SORTEDLIST
        /////////////////<DOCNO, score> id?
        public SortedDictionary<string, double> processFullTermQuery(string queryFullTerm)
        {
            SortedDictionary<string, double> DocRankingList = new SortedDictionary<string, double>();




            //http://www.c-sharpcorner.com/UploadFile/dpatra/autocomplete-textbox-in-wpf/
            //break full term to single terms
            string[] queryFullTermArray = queryFullTerm.Split(' ');
            bool exists = false;
            string queryFullTermArrayPlus = queryFullTermArray[0];



            int size = queryFullTermArray.Length;

            //create fullterm string with +
            for (int i = 1; i < size; i++)
            {
                queryFullTermArrayPlus += "+" + queryFullTermArray[i];
            }



            string termStr = "";

            //get amount of unique terms in query , qfi
            Dictionary<string, int> TermQfiList = new Dictionary<string, int>();
            foreach (string querySingleTerm in queryFullTermArray)
            {
                if (TermQfiList.ContainsKey(querySingleTerm))
                {
                    TermQfiList[querySingleTerm]++;
                    //increment qfi

                }
                else
                {
                    TermQfiList.Add(querySingleTerm, 1);
                }
            }

            int count = TermQfiList.Count;
            if (count > 1)
                count++;

            Tuple<string, SortedList<int, int>, int, int, int>[] termData = new Tuple<string, SortedList<int, int>, int, int, int>[count];
            int termFreq = 0;
            int docFreq = 0;
            int FullTermQfi = 1;//qfi=1 for full term
            SortedList<int, int> termResult;

            termData[0] = getReleventDocumentsOfSingleTerm(queryFullTermArrayPlus, FullTermQfi);
            if (termData[0] == null)
            {
                termResult = new SortedList<int, int>();
            }
            else
            {
                termStr = termData[0].Item1;
                termResult = termData[0].Item2;
                termFreq = termData[0].Item3;
                docFreq = termData[0].Item4;

            }




            //get full term data
            //MVVM
            DocResult = "Full-Term: " + '"' + queryFullTerm + '"' + " " + "tf:" + termFreq + " df:" + docFreq;
            DocResult += System.Environment.NewLine;
            //MVVM




            //foreach term in full query

            //get seperated terms data



            if (queryFullTermArray.Length > 1)
            {

                int i = 1;
                foreach (KeyValuePair<string, int> pair in TermQfiList)
                {

                    string querySingleTerm = pair.Key;
                    int qfi = pair.Value;
                    //MVVM
                    // DocResult += 
                    // DocResult += System.Environment.NewLine;
                    //MVVM




                    termData[i] = getReleventDocumentsOfSingleTerm(querySingleTerm, qfi);


                    //            SortedList<int, int> termResult = termData[i].Item1;
                    //             int termFreq = termData.Item2;
                    //             int docFreq = termData.Item3;


                    DocResult += "Term" + i + ": " + '"' + termData[i].Item1 + '"' + " " + "tf:" + termData[i].Item3 + " df:" + termData[i].Item4 + " qfi:" + termData[i].Item5;

                    if (termData[i].Item3 > 0)
                    {
                        exists = true;
                        //term found!
                    }

                    //send to ranker?



                    DocResult += System.Environment.NewLine;
                    //MVVM
                    i++;
                }

            }
            //send to ranker
            // termData

            ///get avgdl
            ///


            int N = Indexer.DocumentMetadata.Count;
            int avgdl = Indexer.NumOfWordsInCorpus / N;
            //foreach 


            //compute R

            List<int> DocList = new List<int>();
            int R = 0;

            //every term result
            foreach (Tuple<string, SortedList<int, int>, int, int, int> tup in termData)
            {
                //every document
                foreach (KeyValuePair<int, int> docAndTf in tup.Item2)
                {
                    if (!DocList.Contains(docAndTf.Key))
                    {
                        R++;
                    }

                }

            }

            /*
            foreach (Tuple<string, SortedList<int, int>, int, int, int> tup in termData)
        {
            if (tup.Item1.Contains('+'))
            {
                R += tup.Item2.Count();
            }
        }
        */
            Ranker rank = new Ranker(avgdl, N);

            //every term result
            foreach (Tuple<string, SortedList<int, int>, int, int, int> tup in termData)
            {
                string term = tup.Item1;
                SortedList<int, int> termDocList = tup.Item2;
                termFreq = tup.Item3;
                docFreq = tup.Item4;
                int ni = tup.Item2.Count;
                int qfi = tup.Item5;
                //every document
                foreach (KeyValuePair<int, int> docAndTf in tup.Item2)
                {
                    int docNum = docAndTf.Key;
                    int tf = docAndTf.Value; //fi



                    Document DocData = getDocData(docNum);



                    string DOCNO = DocData.DOCNO;
                    string mostFreqTermInDoc = DocData.mostFreqTermInDoc;
                    int maxOccurencesInDocument = DocData.maxOccurencesInDocument;
                    string language = DocData.language;
                    int uniqueInDocAmount = DocData.uniqueInDocAmount;
                    int totalInDocIncludingSW = DocData.totalInDocIncludingSW;
                    int totalInDocwithoutSW = DocData.totalInDocwithoutSW;
                    int AmountUniqueInCorpus = DocData.AmountUniqueInCorpus;



                    if (languageChosen.Count != 0)
                    {
                        if (languageChosen.Contains(DocData.language))
                        {
                            double score = rank.BM25(totalInDocIncludingSW, 0, ni, 0, tf, qfi);
                            if (term.Contains('+'))
                            {

                            }

                            if (DocRankingList.ContainsKey(DOCNO))
                            {
                                DocRankingList[DOCNO] += +score;
                            }
                            else
                            {
                                DocRankingList.Add(DOCNO, score);
                            }
                            ///save to docResultList
                            ///if contains sum score
                        }
                    }
                    else
                    {
                        double score = rank.BM25(totalInDocIncludingSW, 0, ni, 0, tf, qfi);
                        if (term.Contains('+'))
                        {

                        }

                        if (DocRankingList.ContainsKey(DOCNO))
                        {
                            DocRankingList[DOCNO] += +score;
                        }
                        else
                        {
                            DocRankingList.Add(DOCNO, score);
                        }
                    }
                }




                //  rank.BM25()
                //bm25 rank every term

            }






            Console.WriteLine("mark");

            //show most 1.popularByFreq 2.Original Term/Additional -Term2


            //global
            int numOfDocsInCorpus = Indexer.DocumentMetadata.Count; //N
            int numOfTermsInCorpus = Indexer.myDictionary.Count;


            //per term


            //   Ranker rank = new Ranker();

            //    rank.BM25(termData);

            // termData

            return DocRankingList;
        }


        /// <summary>
        /// retun list of querySingleTerm-term 
        /// </summary>
        /// <param name="querySingleTerm"></param>
        public List<string> autoComplete(string querySingleTerm)
        {
            //    //AUTO COMPLETE


            int size = querySingleTerm.Count(c => c == '+');



            var keys = Indexer.myDictionary.Keys.Where(x => x.Contains(querySingleTerm + '+'));
            List<string> termList = keys.ToList();


            SortedDictionary<string, int> termAndPop = new SortedDictionary<string, int>();

            foreach (string FTerm in termList)
            {


                //  int.TryParse(termData[1], out TMPtermFrequency);

                int termFreq;
                int.TryParse(Indexer.myDictionary[FTerm].Split('#')[1], out termFreq);

                //
                int idx = FTerm.IndexOf(querySingleTerm);

                string[] slimTermArr = FTerm.Substring(idx).Split('+');
                string slimTerm = slimTermArr[0];
                for (int i = 1; i <= size + 1; i++)
                {

                    slimTerm += " " + slimTermArr[i];

                }
                //FTerm.Split(querySingleTerm  + '+')[1];

                if (termAndPop.ContainsKey(slimTerm))
                {
                    termAndPop[slimTerm] += termFreq;
                }
                else
                {
                    termAndPop.Add(slimTerm, termFreq);
                }
            }


            var orderByVal = termAndPop.OrderBy(v => v.Value);


            var desc = orderByVal.Reverse();

            List<string> orderByValList = new List<string>();
            int limiter = 0; //TOP 5
            foreach (KeyValuePair<string, int> termResult in desc)
            {
                limiter++;
                orderByValList.Add(termResult.Key);
                if (limiter == 5)
                {
                    break;
                }

            }
            //GET NEXT ELEMT
            //QUERY TERM-TERM

            //GET INDEX-KEY OF TERM IN DATABASE
            /*var IndexOfKey = Indexer.myDictionary.IndexOfKey(querySingleTerm);


            //GET NEXT TERM
            // TERM1-TERM2

            IndexOfKey++; //Handle last index case
            //Get the next item by index.

            char[] delimiterDocs = { '+', '-' };

            string nextTermValue = Indexer.myDictionary.Values[IndexOfKey];


            string nextTermFullKey = Indexer.myDictionary.Keys[IndexOfKey];

            string nextTerm1stKey = nextTermFullKey.Split(delimiterDocs)[0];

            Console.WriteLine(nextTermFullKey + ":" + nextTermValue);


            List<string> termList = new List<string>();


            while (nextTerm1stKey == querySingleTerm)//while term = "term1-"
            {

                IndexOfKey++; //Handle last index case
                //Get the next item by index.







                nextTermValue = Indexer.myDictionary.Values[IndexOfKey];
                nextTermFullKey = Indexer.myDictionary.Keys[IndexOfKey];
                nextTerm1stKey = nextTermFullKey.Split(delimiterDocs)[0];



                ////
                if (nextTerm1stKey == querySingleTerm)
                    termList.Add(nextTermFullKey);


                //    function above



                ////
                Console.WriteLine(nextTermFullKey + ":" + nextTermValue);




            }
            */
            return orderByValList;
        }


        public void move(double speed, int angle)
        {
            throw new NotImplementedException();
        }

        public void initiate()
        {
            throw new NotImplementedException();
        }
    }
}
