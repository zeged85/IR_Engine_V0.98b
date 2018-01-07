using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IR_Engine
{
    interface ISearcher : INotifyPropertyChanged
    {
        string DocResult { set; get; }
        void move(double speed, int angle);

        SortedDictionary<string,double> processFullTermQuery(string query);
        //  void startSearcher();
        List<string> autoComplete(string querySingleTerm);
        void initiate();

        void openQueryFile(string path);

        void runSingleQuery(string query , string[] SYNONYMS, int query_id);


        //     Tuple<string, string, int, string, int, int, int, int> getDocData(int doc);

        string[] getSYNONYMS(string term);

        void setOutputFolder(string path);
        string getOutputFolder();
        string getDocument(int docID);

        string[] getWiki(string term);
    }
}
