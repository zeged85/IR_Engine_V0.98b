using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IR_Engine
{
    class Ranker
    {

        //to remember needed data
        int documentLength; // (in terms)
        double avgdl; //average document length
        int ri; //number of relevant documents contains term i
        int ni; //number of docs contains term i
        int NumOfDocuments; //in corpus
        int R; //num of relevant documents for current query
        int fi; //term i frequency in document
        int qfi; //term i frequency in query

        const double b = 0.75;
        const double K1 = 1.2;
        int K2;

        public Ranker(int docLength,double avgdl, int ri,int ni, int NumofDocs, int R, int fi, int qfi)
        {
            this.documentLength = docLength;
            this.avgdl = avgdl;
            this.ri = ri;
            this.ni = ni;
            this.NumOfDocuments = NumofDocs;
            this.R = R;
            this.fi = fi;
            this.qfi = qfi;
            Random rand = new Random();
            this.K2 = rand.Next(0, 1001);

            //BM25();

        }

        public void rankDocuments(Tuple<string, SortedList<int, int>, int, int>[] termData)
        {
            int R = termData[0].Item2.Count;

            ///foreach Term i
            for (int i = 0; i< termData.Length; i++)
            {
                Tuple<string, SortedList<int, int>, int, int> singleTermData = termData[i];

                string termStr = singleTermData.Item1;
                SortedList<int, int> DocList = singleTermData.Item2;
                int tf = singleTermData.Item3;
                int df = singleTermData.Item4;
                ////////////////////

                int ri = DocList.Count; //or R if this is termData[0] FullTerm
               // R += DocList.Count;

                foreach (KeyValuePair<int, int> pair in DocList)
                {
                    int fi = pair.Value;

                    Console.WriteLine("DocNo:{0} => Popularity:{1}", pair.Key, pair.Value);


                    //get Doc length



                }

                }

            }

        // this needs to be calculate for every! term i in the query. (sigma)
        public double BM25()
        {
            double logNumerator = (ri+0.5)/(R-ri+0.5);
            double logDenominator = (ni - ri + 0.5) / (NumOfDocuments - ni - R + ri + 0.5);

            double K = K1 * ((1 - b) + b * (documentLength / avgdl));

            double log = Math.Log10(logNumerator / logDenominator);
            double first = ((K1 + 1) * fi) / (K + fi);
            double second = ((K2 + 1) * qfi) / (K2 + qfi);

            double result = log * first * second;


            return result;
        }
         
    }
}
