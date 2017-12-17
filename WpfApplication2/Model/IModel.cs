using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IR_Engine
{
    interface IModel : INotifyPropertyChanged
    {
        int Progress { set; get; }
        void move(double speed, int angle);


        void loadMonths();
        void initiate(); 
        void freeMemory(); // create las
        void MergeAllToSingleUnSorted();
        void sort();
        void deleteGarbage();
        void loadMetadata();
        void UniqueWordsQuery();
        void PrintfreqInAllCorpusList(); 
        void mmm();


        void dumpDocumentMetadata();
        void loadPostingFiles();
        void createDictionary();

        void loadDictionary();
        void setOutputFolder(string path);
        string getOutputFolder();
        void clearAllData();
        void createCache();
        void addPointers();
    }
}
