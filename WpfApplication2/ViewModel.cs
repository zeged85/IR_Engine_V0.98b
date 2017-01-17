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
        public ViewModel(IModel index)
        {
            this.model_indexer = index;
            model_indexer.PropertyChanged +=
                          delegate(Object sender, PropertyChangedEventArgs e)
                          {
                              this.NotifyPropertyChanged("VM_" + e.PropertyName);
                          };
        }

        public void NotifyPropertyChanged(string PropName)
        {

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropName));

            progress = model_indexer.Progress;

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

            public void loadDictionary()
        {
            //make dict singleton
            model_indexer.loadDictionary();
        }
        
        public void startEngine()
        {

            model_indexer.loadMonths();

            model_indexer.initiate(); //

            model_indexer.Progress = 5;

            model_indexer.freeMemory(); // create last folder

            model_indexer.Progress = 10;

            model_indexer.MergeAllToSingleUnSorted();

            model_indexer.Progress = 20;

            model_indexer.sort();

            model_indexer.Progress = 30;

            model_indexer.deleteGarbage();

            model_indexer.Progress = 40;

            model_indexer.dumpDocumentMetadata(); //create meta.txt

            model_indexer.Progress = 50;

            model_indexer.loadPostingFiles();

            model_indexer.Progress = 60;

            model_indexer.loadMetadata();

            model_indexer.Progress = 70;

            model_indexer.createDictionary(); // and save to file dict.txt

            //

            model_indexer.Progress = 80;

            //

           model_indexer. loadDictionary(); //from dict.txt

            //


            model_indexer.UniqueWordsQuery();

            model_indexer.Progress = 90;

            model_indexer.PrintfreqInAllCorpusList(); //

            model_indexer.Progress = 100;

            model_indexer.mmm();


            //

            model_indexer.loadDictionary(); //from dict.txt

            //

        }

        public void startSearcher()
        {
            Seracher search = new Seracher();
            if (search.proccessQuery("test"))
                Console.WriteLine("Word exists in memory");
            else
                Console.WriteLine("Word does not exist in memory");

        }
    }
}
