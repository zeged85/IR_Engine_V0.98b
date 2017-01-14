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
        private IModel model;
        public ViewModel(IModel index)
        {
            this.model = index;
            model.PropertyChanged +=
                          delegate (Object sender, PropertyChangedEventArgs e)
                          {
                              this.NotifyPropertyChanged("VM_" + e.PropertyName);
                          };
        }

        public void NotifyPropertyChanged(string PropName)
        {
            
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropName));

            progress = model.Progress;
           
        }

        private int progress;
        public int VM_Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                this.NotifyPropertyChanged("VM_Progress");
                //    model.moveArm(az, elv1, elv2, grip);
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



        public void loadPostingFiles()
        {
            model.loadPostingFiles();
           
        }

        public void createDictionary()
        {
            model.createDictionary();
        }

        public void startEngine()
        {


            model.loadMonths();

            model.initiate(); //

            model.Progress = 5;

            model.freeMemory(); // create last folder

            model.Progress = 10;

            model.MergeAllToSingleUnSorted();

            model.Progress = 20;

            model.sort();

            model.Progress = 30;

            model.deleteGarbage();

            model.Progress = 40;

            model.dumpDocumentMetadata();

            model.Progress = 50;

            model.loadPostingFiles();

            model.Progress = 60;

            model.loadMetadata();

            model.Progress = 70;

            model.createDictionary();

            model.Progress = 80;

            model.UniqueWordsQuery();

            model.Progress = 90;

            model.PrintfreqInAllCorpusList(); //

            model.Progress = 100;

            model.mmm();
        }
    }
}
