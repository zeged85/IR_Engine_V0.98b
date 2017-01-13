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
        public void MoveRobot(double speed, int angle)
        {
            model.move(speed, angle);
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
         
            model.freeMemory(); // create last folder
          
            model.MergeAllToSingleUnSorted();
           
            model.sort();
          
            model.deleteGarbage();
           
            model.dumpDocumentMetadata();
         
            model.loadPostingFiles();
          
            model.loadMetadata();
           
            model.createDictionary();
         
            model.UniqueWordsQuery();
            
            model.PrintfreqInAllCorpusList(); //
        
            model.mmm();
        }
    }
}
