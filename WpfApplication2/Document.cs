using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IR_Engine
{
    public class Document
    {
        public string DOCNO;
        public string mostFreqTermInDoc;
        public int maxOccurencesInDocument;
        public string language;
        public int uniqueInDocAmount;
        public int totalInDocIncludingSW;
        public int totalInDocwithoutSW;
        public int AmountUniqueInCorpus;

        public Document(string DOCNO, string mostFreqTermInDoc, int maxOccurencesInDocument, string language, int uniqueInDocAmount, int totalInDocIncludingSW, int totalInDocwithoutSW, int AmountUniqueInCorpus)
        {
            this.DOCNO=DOCNO;
            this. mostFreqTermInDoc=mostFreqTermInDoc;
            this. maxOccurencesInDocument=maxOccurencesInDocument;
            this.language=language;
            this. uniqueInDocAmount=uniqueInDocAmount;
            this. totalInDocIncludingSW=totalInDocIncludingSW;
            this. totalInDocwithoutSW=totalInDocwithoutSW;
            this.AmountUniqueInCorpus= AmountUniqueInCorpus;
        }
    }
}
