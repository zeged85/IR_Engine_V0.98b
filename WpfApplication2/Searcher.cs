using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IR_Engine;
using WpfApplication2;

namespace IR_Engine_TRD
{
    class Searcher
    {
        //for future implementation of more than one language in query.
        HashSet<string> languages = new HashSet<string>();

        //one language for now
        public static string languageChosed = string.Empty; //language from comboBox
        int maxDocsForQuery = 50;
        
    }
}
