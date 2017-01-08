using IR_Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2
{
    class Engine
    {
        Indexer idx;

        public Engine()
        {
            idx = new Indexer();

            idx.loadMonths();


            idx.initiate(); //

            idx.freeMemory(); // create last folder

            idx.MergeAllToSingleUnSorted();

            idx.sort();

            idx.deleteGarbage();

            idx.dumpDocumentMetadata();

            idx.loadPostingFiles();

            idx.loadMetadata();

            idx.createDictionary();

            idx.UniqueWordsQuery();

            idx.PrintfreqInAllCorpusList(); //

            idx.mmm();




        }
    }
}
