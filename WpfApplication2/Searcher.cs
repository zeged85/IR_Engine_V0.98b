using System;
using System.Collections.Generic;
using System.ComponentModel;
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


        /// MVVM

            
            public Tuple<string,string,int,string,int,int,int,int> getDocData(int doc)
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
                return new Tuple<string, string, int, string, int, int, int, int>(DOCNO, mostFreqTermInDoc,
                maxOccurencesInDocument, language, uniqueInDocAmount, totalInDocIncludingSW, totalInDocwithoutSW, AmountUniqueInCorpus);
            }

            return null;
        }


        public Tuple<SortedList<int, int>, int, int> getReleventDocumentsOfSingleTerm(string querySingleTerm)
        {

            SortedList<int, int> termResult = new SortedList<int, int>();
            //#tf
            int termFrequency = 0; // in all corpurus //single
            //#df
            int documentFrequenct = 0;


            if (!Indexer.myDictionary.ContainsKey(querySingleTerm))
            {
                return new Tuple<SortedList<int, int>, int, int>(termResult,termFrequency,documentFrequenct);

            }
            string val = Indexer.myDictionary[querySingleTerm];
      



            //count docs
            char[] delimiterCharsLang = { '#' };
            string[] termData = val.Split(delimiterCharsLang);

            int.TryParse(termData[1], out termFrequency);
            int.TryParse(termData[2], out documentFrequenct);




          
            Console.WriteLine("Term: " + '"' + querySingleTerm + '"' + " " + "tf:" + termFrequency + " df:" + documentFrequenct);


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

            return new Tuple<SortedList<int, int>,int,int> (termResult, termFrequency, documentFrequenct);

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
        public bool proccessQuery(string queryFullTerm)
        {

            //http://www.c-sharpcorner.com/UploadFile/dpatra/autocomplete-textbox-in-wpf/
            //break full term to single terms
            string[] queryFullTermArray = queryFullTerm.Split(' ');
            bool exists = false;

            Tuple<SortedList<int, int>, int, int>[] termData = new Tuple<SortedList<int, int>, int, int>[queryFullTermArray.Length+ 1];
            int termFreq = 0;
            int docFreq = 0;
            SortedList<int, int> termResult;
            termData[0] = getReleventDocumentsOfSingleTerm(queryFullTerm);
            if (termData[0] == null)
            {
                termResult = new SortedList<int, int>();
            }
            else
            {
                termResult = termData[0].Item1;
                termFreq = termData[0].Item2;
                docFreq = termData[0].Item3;
            }
            
     


            //get full term data
            //MVVM
            DocResult = "Full-Term: " + '"' + queryFullTerm + '"' + " " + "tf:" + termFreq + " df:" + docFreq;
            DocResult += System.Environment.NewLine;
            //MVVM




            //foreach term in full query

            //get seperated terms data
            if (queryFullTermArray.Length> 1) { 
            int i = 0;
                foreach (string querySingleTerm in queryFullTermArray)
                {

                    i++;

                    //MVVM
                    // DocResult += 
                    // DocResult += System.Environment.NewLine;
                    //MVVM


                    if (Indexer.myDictionary.ContainsKey(querySingleTerm)) //single term
                    {
                        exists = true;
                        termData[i] = getReleventDocumentsOfSingleTerm(querySingleTerm);

                        //            SortedList<int, int> termResult = termData[i].Item1;
                        //             int termFreq = termData.Item2;
                        //             int docFreq = termData.Item3;


                        DocResult += "Term" + i + ": " + '"' + querySingleTerm + '"' + " " + "tf:" + termData[i].Item2 + " df:" + termData[i].Item3;



                        //send to ranker?

                    }

                    else
                    {

                    }

                    DocResult += System.Environment.NewLine;
                    //MVVM
                }
            }






            Console.WriteLine("mark");

            //show most 1.popularByFreq 2.Original Term/Additional -Term2



            return exists;
        }


        /// <summary>
        /// retun list of querySingleTerm-term 
        /// </summary>
        /// <param name="querySingleTerm"></param>
        public void autoComplete(string querySingleTerm)
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





            while (nextTerm1stKey == querySingleTerm)//while term = "term1-"
            {

                IndexOfKey++; //Handle last index case
                              //Get the next item by index.







                nextTermValue = Indexer.myDictionary.Values[IndexOfKey];
                nextTermFullKey = Indexer.myDictionary.Keys[IndexOfKey];
                nextTerm1stKey = nextTermFullKey.Split(delimiterDocs)[0];



                ////



                //    function above



                ////
                Console.WriteLine(nextTermFullKey + ":" + nextTermValue);




            }
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
