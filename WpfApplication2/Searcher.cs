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

    
        public event PropertyChangedEventHandler PropertyChanged;

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


        public void openQueryFile(string path)
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


                    int space = s.IndexOf(' ');
                    int query_id;
                    bool isValidInteger = int.TryParse(s.Substring(0, space), out query_id);

                    if (!isValidInteger)
                    {
                        ////
                    }


                    string query = s.Substring(space + 1);

                    //first query in file
                   SortedDictionary<string, double> docRankRes = processFullTermQuery(query);


                    var orderByVal = docRankRes.OrderBy(v => v.Value);

                    //reverse descending
                    //http://stackoverflow.com/questions/7815930/sortedlist-desc-order

                    orderByVal.Reverse();

                    var desc = orderByVal.Reverse();

                    //better just change comperer
                    //http://stackoverflow.com/questions/7815930/sortedlist-desc-order


                    //sort by val double


                    //append results to file

                    StreamWriter file6 = new StreamWriter(@"c:\treceval\results.txt", true);
                    /// 351   0  FR940104-0-00001  1   42.38   mt
                    foreach (KeyValuePair<string, double> docResult in desc)
                    {
                        ///query ID - ITER = 0   - 
                        file6.WriteLine(query_id + " " + "0" + " " + docResult.Key + " " + "0" + " " + "1.1" + " " + "mt");
                    }

                    file6.Close();

                }
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


        public Tuple<string, SortedList<int, int>, int, int, int> getReleventDocumentsOfSingleTerm(string querySingleTerm, int qfi)
        {

            SortedList<int, int> termResult = new SortedList<int, int>();
            //#tf
            int termFrequency = 0; // in all corpurus //single
            //#df
            int documentFrequenct = 0;


            if (!Indexer.myDictionary.ContainsKey(querySingleTerm))
            {
                return new Tuple<string, SortedList<int, int>, int, int, int>(querySingleTerm, termResult, termFrequency,documentFrequenct, 0);

            }
            string val = Indexer.myDictionary[querySingleTerm];
      



            //count docs
            char[] delimiterCharsLang = { '#' };
            string[] termData = val.Split(delimiterCharsLang);

            int.TryParse(termData[1], out termFrequency);
            int.TryParse(termData[2], out documentFrequenct);




          
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

                termResult.Add(document, frequency); //ADD to LIST OF DOCS
                                                     //doc
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

            return new Tuple<string, SortedList<int, int>,int,int,int> (querySingleTerm, termResult, termFrequency, documentFrequenct, qfi);

        }

        public void sortDocList (SortedList<int, int> termResult)
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
        public SortedDictionary<string,double> processFullTermQuery(string queryFullTerm)
        {
            SortedDictionary<string, double> DocRankingList = new SortedDictionary<string, double>();


            //http://www.c-sharpcorner.com/UploadFile/dpatra/autocomplete-textbox-in-wpf/
            //break full term to single terms
            string[] queryFullTermArray = queryFullTerm.Split(' ');
            bool exists = false;
            string queryFullTermArrayPlus = queryFullTermArray[0];



            int size = queryFullTermArray.Length;

            //create fullterm string with +
            for (int i = 1; i<size; i++)
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


                    // if (Indexer.myDictionary.ContainsKey(querySingleTerm)) //single term



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
                int avgdl = Indexer.NumOfWordsInCorpus / N ;
                //foreach 


                //compute R

                List<int> DocList = new List<int>();
                int R = 0; ;
               
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

                        Ranker rank = new Ranker(avgdl, N);

                //every term result
                foreach (Tuple<string, SortedList<int, int>, int, int, int> tup in termData)
                {
                    string term = tup.Item1;
                    SortedList<int, int>  termDocList = tup.Item2;
                    termFreq = tup.Item3;
                    docFreq = tup.Item4;
                    int ri = tup.Item2.Count;
                    int qfi = tup.Item5;
                    //every document
                    foreach (KeyValuePair<int,int> docAndTf in tup.Item2)
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





                        double score = rank.BM25(totalInDocIncludingSW, ri, 0, R, tf, qfi);

                    
                    if (DocRankingList.ContainsKey(DOCNO))
                    {
                        DocRankingList[DOCNO]+=  + score;
                    }
                    else
                    {
                        DocRankingList.Add(DOCNO, score);
                    }
                    ///save to docResultList
                    ///if contains sum score
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

            //GET NEXT ELEMT
            //QUERY TERM-TERM

            //GET INDEX-KEY OF TERM IN DATABASE
            var IndexOfKey = Indexer.myDictionary.IndexOfKey(querySingleTerm);


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

            return termList;
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
