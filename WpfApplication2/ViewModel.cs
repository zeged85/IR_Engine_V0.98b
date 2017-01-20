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
        private IModel model_indexer;
        private ISearcher search;
        public ViewModel(IModel index, ISearcher searcher)
        {
            this.model_indexer = index;
            this.search = searcher;

            search.PropertyChanged +=
                          delegate (Object sender, PropertyChangedEventArgs e)
                          {
                              this.NotifyPropertyChanged("VM_" + e.PropertyName);
                          };

            model_indexer.PropertyChanged +=
                          delegate(Object sender, PropertyChangedEventArgs e)
                          {
                              this.NotifyPropertyChanged("VM_" + e.PropertyName);
                          };

           // docResult = new string.DefaultIfEmpty();
        }

        public void NotifyPropertyChanged(string PropName)
        {


          //  progress = model_indexer.Progress;


            if (PropName == "VM_Searcher_DocResult")
            {
                docResult = search.DocResult;
                PropName = "VM_DocResult";
            }



            if (PropName == "VM_Progress")
            {
                progress = model_indexer.Progress;
                //PropName = "VM_DocResult";
            }



            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropName));



            
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
                searchQuery(queryInput);

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


        public void startEngine()
        {
            Indexer.clearAllData();

            VM_DocResult = "loading files...";

            model_indexer.loadMonths();

            model_indexer.initiate(); //

           // model_indexer.Progress = 55;

            model_indexer.freeMemory(); // create last folder

            model_indexer.Progress = 50;
            VM_DocResult = "merging...";
            model_indexer.MergeAllToSingleUnSorted();

            model_indexer.Progress = 65;
            VM_DocResult = "sorting...";
            model_indexer.sort();

            model_indexer.Progress = 70;

            model_indexer.deleteGarbage();

            model_indexer.Progress = 75;

          //  model_indexer.dumpDocumentMetadata(); //create meta.txt

            model_indexer.Progress = 80;
            VM_DocResult = "loading Posting Files...";
            model_indexer.loadPostingFiles();

            model_indexer.Progress = 85;

         //   model_indexer.loadMetadata();

            model_indexer.Progress = 90;
            VM_DocResult = "createing Dictionary...";
            model_indexer.createDictionary(); // and save to file dict.txt

            //

            model_indexer.Progress = 95;

            //

            //   model_indexer. loadDictionary(); //from dict.txt

            //


            model_indexer.UniqueWordsQuery(); //CREATE METADATA

            model_indexer.Progress = 97;

            model_indexer.PrintfreqInAllCorpusList(); //TOP 10 BOTTOM 10 

            model_indexer.Progress = 99;

            model_indexer.mmm();


            //
            //check is indexer full
            model_indexer.loadDictionary(); //from dict.txt


            model_indexer.Progress = 100;
            //
            VM_DocResult = "done.";
        }

        public void searchQuery(string query)
        {
           
            if (search.proccessQuery(query))
                Console.WriteLine("Word exists in memory");
            else
                Console.WriteLine("Word does not exist in memory");

        
        }

        public void loadDictionary()
        {

            //make dict singleton
            model_indexer.loadDictionary();

            //load metadata also

            model_indexer.loadMetadata();

            //get dictionary size

            //get metadata size





//is this needed?
          //  Indexer.clearAllData();




            



        }


    }
}
