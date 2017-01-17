using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IR_Engine
{
    public class Seracher
    {

 
        public bool proccessQuery(string query)
        {

            if (Indexer.myPostings.ContainsKey(query))
            {
                string val = Indexer.myPostings[query];
                Console.WriteLine(query + " : " + val);
                return true;
            }
            return false;

        }

        

    }
}
