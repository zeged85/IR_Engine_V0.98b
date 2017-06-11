using IR_Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IR_Engine
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private IModel m_indexer;
       // private ISearcher search;
        public ViewModel(IModel index)
        {
            this.m_indexer = index;
       //     this.search = searcher;

     

            m_indexer.PropertyChanged +=
                          delegate(Object sender, PropertyChangedEventArgs e)
                          {
                              this.NotifyPropertyChanged("VM_" + e.PropertyName);
                          };

           // docResult = new string.DefaultIfEmpty();
        }

        public void NotifyPropertyChanged(string PropName)
        {


          //  progress = model_indexer.Progress;


            if (PropName == "VM_DocResult")
            {
                docResult = m_indexer.DocResult;
                PropName = "VM_DocResult";
            }



            if (PropName == "VM_Progress")
            {
                progress = m_indexer.Progress;
                //PropName = "VM_DocResult";
            }


            if (PropName == "VM_listBoxMyMovies")
            {
                _listBoxMyMovies = m_indexer.listBoxMyMovies;
                //PropName = "VM_DocResult";
            }
            


            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropName));



            
        }

        

           private int _listBoxMyMovies;
        public int VM_listBoxMyMovies
        {
            get { return _listBoxMyMovies; }
            set
            {
                _listBoxMyMovies = value;
                this.NotifyPropertyChanged("VM_listBoxMyMovies");
            }
        }


        private int progress;
        public int VM_Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                this.NotifyPropertyChanged("VM_Progress");
            }
        }

        private int docSum;
        public int VM_DocSum
        {
            get { return docSum; }
            set
            {
                docSum = value;
                this.NotifyPropertyChanged("VM_DocSum");
            }
        }


        private string docResult;
        public string VM_DocResult
        {
            get { return docResult; }
            set
            {
                docResult = value;
                m_indexer.DocResult = docResult;
                this.NotifyPropertyChanged("VM_DocResult");
            }
        }

        private string queryInput;
        public string VM_QueryInput
        {
            get { return queryInput; }
            set
            {
                //check if dic loaded


                //fire query
              
                
                /*
                if (value[value.Length-1] == ' ')// if user entered ' ' char
                {//check term1-term*

             
                    //show most populer term*


                }
                */

                
                queryInput = value;


                   

        //        SortedDictionary<string,double> docRankRes = VMsearchQuery(queryInput);


          //      string[] SYNONYMS = search.getSYNONYMS(queryInput);
                //sort

                //show results on screen

                //queryoutput



                this.NotifyPropertyChanged("VM_QueryInput");
                


             //   triger searchResult
            }
        }






        /*
        public void loadPostingFiles()
        {
            model.loadPostingFiles();

        }

        public void createDictionary()
        {
            model.createDictionary();
        }
        */

        public void reset()
        {
            m_indexer.reset();
        }
      
            public void VM_selectMovie(string title, double rating)
        {
           int movieID =  m_indexer.getMovieID(title);
            m_indexer.selectMovie(movieID, rating);
        }

        public void startEngine()
        {
          //  Indexer.clearAllData();

            VM_DocResult = "loading files...";

            //  model_indexer.loadMonths();

            m_indexer.ProgressTest();

            m_indexer.initiate(); //

            // model_indexer.Progress = 55;

            //       model_indexer.freeMemory(); // create last folder

            m_indexer.Progress = 0;

            VM_DocResult = "creating dictionary...";
            m_indexer.createMovieDictionary();

            
    //        model_indexer.MergeAllToSingleUnSorted();

            m_indexer.Progress = 65;
            VM_DocResult = "sorting...";
    //        model_indexer.sort();

            m_indexer.Progress = 70;

//            model_indexer.deleteGarbage();

            m_indexer.Progress = 75;

          //  model_indexer.dumpDocumentMetadata(); //create meta.txt

            m_indexer.Progress = 80;
            VM_DocResult = "loading Posting Files...";
     //       model_indexer.loadPostingFiles();

            m_indexer.Progress = 85;

         //   model_indexer.loadMetadata();

            m_indexer.Progress = 90;
            VM_DocResult = "createing Dictionary...";
    //        model_indexer.createDictionary(); // and save to file dict.txt

            //

            m_indexer.Progress = 95;

            //

            //   model_indexer. loadDictionary(); //from dict.txt

            //


  //          model_indexer.UniqueWordsQuery(); //CREATE METADATA

            m_indexer.Progress = 97;

           // model_indexer.PrintfreqInAllCorpusList(); //TOP 10 BOTTOM 10 

            m_indexer.Progress = 99;

            m_indexer.mmm();


            //
            //check is indexer full

    //        Indexer.myPostings.Clear();


            loadDictionary(); //from dict.txt


            m_indexer.Progress = 100;
            //
            VM_DocResult = "done.";
        }

     

        public SortedDictionary<string,double> VMsearchQuery(string query)
        {
       /*     SortedDictionary<string, double> DocRankRes = search.processFullTermQuery(query);
            if (DocRankRes.Count > 1)
                Console.WriteLine("Word exists in memory");
            else
                Console.WriteLine("Word does not exist in memory");

         */   return null;
        }

        public List<string> autoComplete(string querySingleTerm)
        {
           // if (queryInput.Substring(querySingleTerm.Length-1)==" ")
          //  {
                string res = querySingleTerm.TrimEnd();

               // string plus = res.Replace(' ', '+');
              // return search.autoComplete(plus);
         //   }

            return m_indexer.autocomplete(querySingleTerm);
        }

        public string getOutputFolder()
        {
            return m_indexer.getOutputFolder();
        }

        public void setOutputFolder(string path)
        {

            m_indexer.setOutputFolder(path);
            //search.setOutputFolder(path);
        }

        public void loadDictionary()
        {

            //make dict singleton
            VM_DocResult = "Loading Dictionary...";
       //     model_indexer.loadDictionary();

            //load metadata also

        //    model_indexer.loadMetadata();
            VM_DocResult = "Ready.";
            //get dictionary size

            //get metadata size

            //is this needed?
            //  Indexer.clearAllData();

        }
    }
}
