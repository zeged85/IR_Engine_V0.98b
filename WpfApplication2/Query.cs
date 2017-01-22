using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IR_Engine
{
    public class Query
    {
        string querySingleTerm;
        SortedList<int, int> listOfRelevantDocs;
        int termFrequency;
        int documentFrequenct;
        int qfi;

        public Query(string querySingleTerm, SortedList<int, int> listOfRelevantDocs, int termFrequency, int documentFrequenct, int qfi)
        {
            this.querySingleTerm = querySingleTerm;
            this.listOfRelevantDocs = listOfRelevantDocs;
            this.termFrequency = termFrequency;
            this.documentFrequenct = documentFrequenct;
            this.qfi = qfi;
            
        }



    }
}
