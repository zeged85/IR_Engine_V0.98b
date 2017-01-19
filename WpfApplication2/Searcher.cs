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

        SortedList<int, int>[] termResult;

     //   public se


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
            set { docResult = value; NotifyPropertyChanged("DocResult"); }
        }


        /// MVVM




        public bool proccessQuery(string querySingleTerm)
        {

            //http://www.c-sharpcorner.com/UploadFile/dpatra/autocomplete-textbox-in-wpf/
            //foreach startswith

         //   foreach (string s in )
            



            //#tf
            int termFrequency; // in all corpurus //single
            //#df
            int documentFrequenct;

     

            if (Indexer.myDictionary  .ContainsKey(querySingleTerm)) //single term
            {
             
                string val = Indexer.myDictionary[querySingleTerm];
                Console.WriteLine(querySingleTerm + " : " + val);


                //MVVM
                DocResult = querySingleTerm + " : " + val;
                //MVVM


                //count docs
                char[] delimiterCharsLang = { '#'};
                string[] termData  = val.Split(delimiterCharsLang);

                int.TryParse(termData[1], out termFrequency);
                int.TryParse(termData[2], out documentFrequenct);


                int length = termData.Length;
                int document;
                int frequency;

                termResult = new SortedList<int, int>[1];

                //relervent documents. sorted by docNum, sort/rank by termFreq

                termResult[0] = new SortedList<int, int>();

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
                //return doc/s




                //present docs on screen


                //PARSE DICTIONARY TERMPOST_TUPLE TO DOC#-FREQ
                //FIRST TERM
                for (int i=3; i <length-1; i=i+2)
                {
                    int.TryParse(termData[i], out document); //DOCUMENT
                    int.TryParse(termData[i+1], out frequency); //FREQUENCY

                    termResult[0].Add(document, frequency); //ADD TO ARRAY OF LISTS OF DOCS
                    //doc
                    //freq
                }

                // string test = Indexer.myPostings[query].Skip(1).Take(1).ToDictionary;

                //GET ALL lISTS OF DOCS
                //AND SORT THEM BY FREQ

              //  termResult.sortby freq


                    ///http://stackoverflow.com/questions/28923062/sortedlistint-int-sort-by-value
                    ///var orderByVal = sortedlist.OrderBy(v => v.Value);
                    ///

                var orderByVal = termResult[0].OrderBy(v => v.Value);

                //reverse descending
                //http://stackoverflow.com/questions/7815930/sortedlist-desc-order

                orderByVal.Reverse();

                var desc = orderByVal.Reverse();

                //better just change comperer
                //http://stackoverflow.com/questions/7815930/sortedlist-desc-order




                //display the elements of the sortedlist after sorting it with Value's
                Console.WriteLine("The elements in the SortedList after sorting :");
                foreach (KeyValuePair<int, int> pair in desc)
                {
                    Console.WriteLine("DocNo:{0} => Popularity:{1}", pair.Key, pair.Value);
                }



                ///language
                ///
                //retrieve final document

                //use postingList??
                //https://www.youtube.com/watch?v=IZTJgMjE5jw -- recommended






                Console.WriteLine("mark");

                //show most 1.popularByFreq 2.Original Term/Additional -Term2



                ///http://stackoverflow.com/questions/3663546/most-efficient-way-to-find-the-index-of-an-item-in-a-sorteddictionary
                ///http://stackoverflow.com/questions/8090786/c-sharp-sorted-list-how-to-get-the-next-element
                ///http://stackoverflow.com/questions/6131827/how-do-i-get-the-element-with-the-smallest-key-in-a-collection-in-o1-or-olog
                ///CREATE SORTEDLIST
                ///

                //     Indexer.myPostings.

                // SortedList<string, string> sdfg = new SortedList<string, string>();

                //   var s = new SortedList<string, string>{ { "a", "Ay" }, { "b", "Bee" }, { "c", "Cee" } };


                // Outputs 1
                //    Console.WriteLine(s.IndexOfKey("b"));
                //   if (Indexer.myPostings.ContainsKey(query))

                //   var index = Indexer.myDictionary.IndexOfKey(key);

                //   var first = Indexer.myDictionary.Values[index];
                //   var second = Indexer.myDictionary.Values[index + 1];

                //  SortedList can be accessed by both key and index

                //   var IndexOfKey = Indexer.myDictionary.IndexOfKey(key);
                //Increment the index,









                //string val;

                //query

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


                // querySingleTerm.Substring(querySingleTerm.Length-1) = "-related"


            //    string related;
                //        related = 

                // querySingleTerm.Substring(0,querySingleTerm.Length-1) = "term1"



                //or querySingleTerm.split('-')[] // all related

                //if [0] === term

                //while [0] == term
            
              //  string nextTerm1stKey = nextTermFullKey.Split(delimiterDocs)[0];


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

          //      char[] delimiterDocs = { '+', '-'  };
          //      string nextTerm1stKey = nextTermFullKey.Split(delimiterDocs)[0];

          
                //need to get alll term-term
                //find most popular

                //this returns only documents of the 1st term in query
                //all term-


                //if input = term1-term2
                //search term1-
                //search term2-
                //search term1-term2
                //search term2-term1

                //day-by-day


                ///output all documents to ranker?
                ///vector search

                //need two more terms

                //intersect relevence of long-term in all documents




                //return most relevent document

                //language

                return true;
            }
            else
            {
                DocResult = "";
            }
            return false;

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
