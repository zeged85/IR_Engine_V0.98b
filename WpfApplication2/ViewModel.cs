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

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropName));

            progress = model_indexer.Progress;

            docResult = search.DocResult;

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

            model_indexer.loadMonths();

            model_indexer.initiate(); //

            model_indexer.Progress = 55;

            model_indexer.freeMemory(); // create last folder

            model_indexer.Progress = 60;

            model_indexer.MergeAllToSingleUnSorted();

            model_indexer.Progress = 65;

            model_indexer.sort();

            model_indexer.Progress = 70;

            model_indexer.deleteGarbage();

            model_indexer.Progress = 75;

            model_indexer.dumpDocumentMetadata(); //create meta.txt

            model_indexer.Progress = 80;

            model_indexer.loadPostingFiles();

            model_indexer.Progress = 85;

            model_indexer.loadMetadata();

            model_indexer.Progress = 90;

            model_indexer.createDictionary(); // and save to file dict.txt

            //

            model_indexer.Progress = 95;

            //

        //   model_indexer. loadDictionary(); //from dict.txt

            //


            model_indexer.UniqueWordsQuery();

            model_indexer.Progress = 97;

            model_indexer.PrintfreqInAllCorpusList(); //

            model_indexer.Progress = 99;

            model_indexer.mmm();


            //
            //check is indexer full
            model_indexer.loadDictionary(); //from dict.txt


            model_indexer.Progress = 100;
            //

        }

        public void startSearcher()
        {
           
            if (search.proccessQuery("al"))
                Console.WriteLine("Word exists in memory");
            else
                Console.WriteLine("Word does not exist in memory");

        
        }

        public void loadDictionary()
        {
            //make dict singleton
            model_indexer.loadDictionary();
            startSearcher();
            Indexer.clearAllData();
        }


    }
}
