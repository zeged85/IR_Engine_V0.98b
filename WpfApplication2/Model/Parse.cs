using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IR_Engine
{
    /// <summary>
    /// This class will parse every document into terms.
    /// The parser needs to be fit with the documents in our "maagar"
    /// It is enough to regard only the text within the tags: <TEXT> (and later other tags).
    /// The parser could be done in any way possible.
    /// 
    /// When separating terms from the documents we must obey these rules:
    /// 
    /// A. Numbers
    /// B. Ranges/Expressions with hyphens
    /// C. Precentages
    /// D. Prices
    /// E. Dates
    /// F. 2 Rules we need to come up with
    /// G. No need to regard punctuation marks (unless they are part of a term?).
    ///    Can be used to separate words.
    /// H. Stop-Words
    /// I. Enable stemming
    /// 
    /// 
    /// ref:
    /// http://stackoverflow.com/questions/1500194/c-looping-through-lines-of-multiline-string
    /// </summary>
    class Parse
    {

        public static List<string> languagesList = new List<string>();
        //https://www.dotnetperls.com/enum
        enum termType {Term, Month, Number, Name };
        //test for commit

        //  private static int thisDocNumber;
        public static SortedDictionary<string, string> parseString(string str, int docNumber)
        {

            int thisDocNumber = docNumber;
            int countAmountOfUniqueInDoc = 0;
            int wordPositionWithSW = 0;
            int wordPositionWithoutSW = 0;




            // Console.WriteLine("Parsing");
            string DOCNO = "";
            string languageDocument = "";

            using (StringReader reader = new StringReader(str))
            {
                string line; // full line string  
                line = reader.ReadLine();

                SortedDictionary<string, string> myMiniPostingListDict = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                //moving this to static may improve preformence?


                if (line == null)
                {

                }

                Console.WriteLine(line);

                if (line == @"<DOCNO>FT924-11895</DOCNO>")
                {

                }

                while (ReadFile.NaiveSearch(line, "<DOCNO>") != 0)
                {
                    line = reader.ReadLine();
                }
                //DocNO

                if (ReadFile.NaiveSearch(line, @"<DOCNO>") != -1)
                {
                    char[] delimiterCharsLang = { '<', '>' };
                    string[] splittedLine = line.Split(delimiterCharsLang);
                    DOCNO = splittedLine[2];
                }

                while (ReadFile.NaiveSearch(line, "<TEXT>") != 0)
                {
                    line = reader.ReadLine();
                    if (line == null)  //NO TEXT HEADER FOUND!!
                    {
                        Console.WriteLine(str);
                       // Console.ReadKey();
                        return myMiniPostingListDict;
                    }
                }

                //http://stackoverflow.com/questions/8459928/how-to-count-occurences-of-unique-values-in-dictionary

              
                //text parsing - main work
                int lineIdx = 0; // lines in document
                // int limiter = 10;

                //LONG TERM
                Queue<string> termQueue = new Queue<string>();

                string longTerm = string.Empty;
                int longTermSize = 0;
                bool addTermToLongTerm = false;
               

                //DATE
                bool partialDate = false;
                bool possibleDay = false;
                bool possibleYear = false;
                bool possibleMonth = false;
                bool stopDate = false;
                int month=0;
                int day=0;
                int year=0;
                int partialDateCounter = 0;

                // percent dgree dollar
                string casualNumber = string.Empty;
                bool casualNumberBool = false;
                string complexTerm;

                //SYNTEX
                bool stopLongTerm = false; //end of line
                bool addNextTermToLongTerm = false;
                termType type = new termType();

                // int limiter = 10;
                string previousLine;
                while (ReadFile.NaiveSearch(line = reader.ReadLine(), "</TEXT>") != 0/* && limiter!=0*/)
                {
                    //https://msdn.microsoft.com/en-us/library/ms228388.
                    
                    //Language
                    
                    if (ReadFile.NaiveSearch(line, @"Language: <F P=105>") != -1)
                    {
                        char[] delimiterCharsLang = { '<', '>' };
                        string[] splittedLine = line.Split(delimiterCharsLang);
                        //update language pool

                        string lang = splittedLine[2].TrimStart();
                        languageDocument = lang;

                        if (!languagesList.Contains(lang))
                        {
                            languagesList.Add(lang); ///need to add mutex
                            //add to string
                        }

                    }

                    //  limiter--;
                    //    char[] delimiterChars = { ' ', ',', '.', ':', '\t', '(', ')', '"', '/', '-', '\'', '?', '[', ']', '$', '%', ';', '*', '+', '=', '&', '\'', '`', '#', '|', '\"', '{', '}', '!', '<', '>', '_', '\\', '@' };
                    char[] delimiterChars = { ' ' };



                    string[] words = line.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);

                    //   System.Console.WriteLine("{0} words in text:", words.Length);
                 

                    lineIdx++;
                    if (lineIdx == 89)
                    {

                    }

                    foreach (string s in words) /// MAIN PARSE LOOP
                    {

                        string term = s.ToString();
                        addTermToLongTerm = false;
                        if (term.Length == 0)
                        {
                            continue;
                        }
                        termQueue.Enqueue(term);
                        while (termQueue.Count > 0)
                        {
                            term = termQueue.Dequeue();
                            if (termQueue.Count > 1)
                            {

                            }

                         if (addNextTermToLongTerm)
                            {
                                addTermToLongTerm = true;
                                addNextTermToLongTerm = false;
                            }
                     





                          //  addTermToLongTerm = false;




                            type = termType.Term;





                            //discard single letters
                            if (s.Length == 1)
                            {
                                //continue;
                            }

                            if (!char.IsLetterOrDigit(s[0]))
                            {
                                //        continue;
                            }

                            if (s == "--")
                            {
                                //     continue;
                            }

                            if (s == "+")
                            {
                                continue;
                            }

                            if (s == "..")
                            {
                                continue;
                            }
                            if (s == "...")
                            {
                                continue;
                            }

                            //"without Portfolio        --          23         31              24"



                            if (s[0] == '-')
                            {
                                //           continue;
                            }


                            char firstChar;
                            //discard symbol in the beginning
                            while (term.Length > 1 && !char.IsLetterOrDigit(firstChar = term[0]) && type == termType.Term)
                            {

                                //  term = term.Split(firstChar)[1];
                                if (firstChar == '.')
                                {

                                }
                                else if (firstChar == '%')
                                {

                                }
                                else if (firstChar == '-') //check negative number
                                {
                                    if (char.IsDigit(term[1]))
                                    {//negative number
                                     //  maybe negativeNumber = true;
                                     //   if (negative)
                                        {
                                            // skip stem
                                            type = termType.Number;
                                            continue;
                                        }
                                    }
                                    else
                                    {

                                    }
                                }
                                else if (firstChar == '(')///"Japan Technology Production Association (Nihon gijutsu seisan"
                                {
                                    //          save and restart
                                    stopLongTerm = true;
                                }
                                else if(firstChar== '$')
                                {
                               //     type = termType.Currency;
                                }
                                else if (firstChar == '%')
                                {
                              //      type = termType.Percent;
                                }


                                ///"the \"media chiefs\" had just attended a meeting of the Russian"
                                ///"\"elitist\" and \"bourgeoi\"  journalists' club on returning from the"
                                ///"selected according to \"CPSU principles,\" came to enjoy \"expensive"
                                ///
                                else if (firstChar == 34)
                                {
                                    ///"assuring viewers that, \"as soon as we learn,\" Vesti would inform"
                                    ///add bool quotes? = true; false on new string line?
                                    ///"viewers \"whom the government will help and how.\""
                                    ///
                                    addTermToLongTerm = true;
                                }
                                else
                                {

                                }
                                term = term.Substring(1);
                            }


                            // int length = term.Length;
                            // char lastChar = term[length - 1];
                            //discard symbols in the ending
                            //CLEAN ENDING
                            char lastChar;
                            char upperSymbol = new char(); //for uppercase
                            bool Symboled = false;
                            while (term.Length >= 1 && !char.IsLetterOrDigit(lastChar = term[term.Length - 1]))
                            {

                                term = term.Split(lastChar)[0];

                                if (type != termType.Term)
                                {

                                }

                                if (lastChar == '.')
                                {

                                    if (term.Length < 2)
                                    {//"T. GURBADAM -- Previous:  MPRP CC member and director of MPRP CC"
                                        //"U.S."

                                    }
                                    else
                                    { // full stop
                                      //"\"Maharashi Vedic\" University Branch--T. Sumiya, member of the"
                                      //"Branch--T."
                                        stopLongTerm = true;
                                        casualNumberBool = false;
                                    }
                                }
                                //   longTerm = string.Empty;//restart long term
                                else
                                if (lastChar == ',') // dump partial date
                                {
                                    stopDate = true;
                                }
                                else
                                if (lastChar == '(')
                                {

                                }
                                else
                                //"you vote?\" (all numbers are percentages)"
                                if (lastChar == ')')
                                {
                                    //maybe stop long term
                                    stopLongTerm = true;
                                }

                                ///"address topics such as research and innovation in small and medium-"
                                ///"sized enterprises, competitiveness-oriented research and the private"
                                ///
                                else if (lastChar == '-')
                                {//add to long term?

                                }


                                ///"the \"media chiefs\" had just attended a meeting of the Russian"
                                ///"\"elitist\" and \"bourgeoi\"  journalists' club on returning from the"
                                ///"selected according to \"CPSU principles,\" came to enjoy \"expensive"
                                ///
                                else if (lastChar == 34)
                                {
                                    addTermToLongTerm = true;
                                    stopLongTerm = true;

                                }

                                else if (lastChar == '\"')
                                {

                                }

                                else
                                {
                                    /// ':'
                                    ///"(AUTHOR:  CRIMMINS.  QUESTIONS AND/OR COMMENTS, PLEASE CALL"
                                    ///"FEATURE:"
                                }
                                Symboled = true;
                                upperSymbol = lastChar;


                            }


                            if (term.Length == 0)//          save and restart
                            {//"B. PUREB -- Previous:   Director of the Organization and"
                             //stop long term?
                             // stopLongTerm = true;
                                continue;
                            }


                            int index = 0;
                            //TERM-TERM
                            if (type == termType.Number)//negative number
                                                        // -5****j
                            {
                                index++;
                            }

                            int j = index;
                            bool stopWhile = false;
                            /// TERM-TERM
                            /// TERM-NUMBER
                            /// NUMBER-TERM
                            /// NUMBER-NUMBER
                            /// '-'
                            /// '/'
                            /// '.'
                            /// 

                            if (term == "R&amp;D")
                            {
                                ///term = R&D ?
                                ///&amp = &
                                continue;
                            }


                            ///check for complex term
                            while (!stopWhile && j < term.Length - 2)
                            {
                                if (!char.IsLetterOrDigit(term[j])) //foun devider "-"
                                {//"Markovic--Milosevic's"
                                    ///"1991-95"
                                    ///"the Mid-Term Defense Plan (1991-95) with then-JDA Director-General"
                                    ///"AND/OR"
                                    ///"strip-tease"
                                    ///"ENEAG/BLOUGH"
                                    ///"ENEAG/BLOUGH  cka 23/0145z  mar"
                                    if (term[j] == '&')
                                    {
                                        break;
                                    }
                                    
                                    //"199O-92" - fix
                                    //"1988-9O"

                                    if (char.IsDigit(term[j - 1]) && char.IsDigit(term[j + 1]))
                                    {///num - num
                                     ///"22/1916z"
                                     ///"733-6070"
                                     ///"23/0145z"
                                     ///"1991-95"
                                        if (term[j] == '-')
                                        {

                                            
                                        }
                                        else if (term[j] == '/')
                                        {
                                            //FRACTIONS. maybe add ',' 1,300
                                            //"76/15"
                                            //"78/13"
                                            term=term.ToLower();
                                            if (term[term.Length - 1] == 'z')
                                            {

                                            }
                                            else
                                            {
                                                //"vp2100/10"
                                                //"INTERNATIONAL 22/28 Nov 93) AM"
                                                //"*BT 1992/93 annual review, 28 pages, in English."
                                                //"$1/5.8 yuan), would now total 1009 yuan ($115.98 at $1/8.7 yuan)"
                                                //"Ordinance No. 81/027 of 22 May 1981, which punishes acts of"
                                                //"plane, an A-300/600 [as heard] Airbus. The Ethiopians were"
                                                //"02/03/94 to 08/03/94. </H3>" !!!
                                                //"place at midnight of 13/14 March. Mr Kriel added that the"
                                                //"areas already approved by the government (decree 18/93)."
                                                //"complementary tax in 1978/79 was 80 percent. He added: \"I do"
                                                //"1-12/93"

                                            }

                                        }
                                        // else if (//1,212)
                                    }
                                    else
                                    {
                                        //"Lebanon (for example, Damascus radio, 27 February)--the"
                                        //"talks\"--which"
                                        //"DOP--must"
                                        if (term[j] == term[j + 1])// "--"
                                        {
                                            stopDate = true;
                                            stopLongTerm = true;
                                            j++;
                                        }
                                        else
                                        {//"199O-92"


                                            ///TERM-TERM
                                            ///
                                            //"U.S.-Russian"
                                            //"hand-in-hand"
                                            //"day-to-day"
                                            //"tug-of-war"
                                            ///"then-JDA"
                                            ///

                                            //"state-of-the-art"
                                            //"See entry under Belgrade RTB Television Network,page 28."
                                            //"are the \"first batch\" of a 2,000-person contingent of"


                                            ///"3-III,"
                                            //add to queue
                                            //"100-mark"
                                            string toQueue = term.Substring(j + 1);
                                            if (toQueue == "")
                                            {

                                            }
                                            termQueue.Enqueue(toQueue);

                                            //continue with trimmed term1



                                            term = term.Substring(0, j);
                                            addTermToLongTerm = true;

                                            addNextTermToLongTerm = true;
                                            stopWhile = true;
                                            continue;


                                        }







                                        ///term-term
                                        ///"ENEAG/BLOUGH"


                                        ///term-num
                                        ///"60-page"
                                        /// 

                                        ///num-term
                                        ///"60-page" -> "60-pag+page"



                                    }

                                    if (char.GetUnicodeCategory(term[index]) != char.GetUnicodeCategory(term[j + 1])) //"term-number"
                                    {
                                    }

                                    index = j;
                                }
                                j++;
                            }





                            term = term.TrimEnd();

                            //discard single letters
                            //   if (term.Length < 2)
                            //       continue;



                            string termToLower = term.ToLower();

                            //months before SW because SW contains "may"
                            if (type == termType.Term && Indexer.Months.ContainsKey(termToLower))
                            {
                                if (possibleMonth == true) // MONTH MONTH CONFLICT
                                {

                                }
                                partialDate = true;
                                possibleMonth = true;
                                month = Indexer.Months[termToLower];
                                addTermToLongTerm = false;
                                type = termType.Month;
                            }

                            //STOP WORDS
                            //may is in SW. 
                            if (type == termType.Term && Indexer.stopWords.ContainsKey(termToLower))
                            {
                                wordPositionWithSW++;
                                continue;
                            }
                            wordPositionWithoutSW++;
                            wordPositionWithSW++;



                            //check number 
                            //https://msdn.microsoft.com/en-us/library/bb384043.aspx
                            //try parse
                            //NUMBERS
                            //"percent among respondents between 18 and 24.  Residents of Skopje"
                            //https://msdn.microsoft.com/en-us/library/9zbda557(v=vs.110).aspx
                            string stringTerm = termToLower;

                            // string value = stringTerm; //"$1,643.57";
                            bool isValidNumber;
                            bool isValidInteger;

                            if (type == termType.Number)
                            {

                            }

                            ///check if term is number
                            if (char.IsNumber(termToLower[0]) || type == termType.Number)
                            {   //  TERM IS NUMBER
                                type = termType.Number;
                                int i = 0;
                                decimal number;
                                // "108";
                                isValidNumber = Decimal.TryParse(stringTerm, out number); //i now = 108  
                                isValidInteger = int.TryParse(stringTerm, out i);

                                //true integer -> true decimal
                                if (!isValidNumber)
                                {

                                    ///"1991-95"
                                    ///"733-6346"
                                    ///"21mar/techtf/milf/ti"
                                    ///"22/1916z"
                                    ///"7's"
                                    ///"21st"
                                    ///"70-percen"
                                    ///"50-year-ol"
                                    ///180-million-lira
                                 //   Console.WriteLine("Unable to parse '{0}'.", stringTerm);
                                    string fixedNum;
                                    /*
                                    if (stringTerm.Contains("o"))
                                    {
                                       fixedNum = stringTerm.Replace('o', '0');
                                        termQueue.Enqueue(fixedNum);
                                        continue;
                                    }
                                    */



                                    }
                                else {
                                    ///TERM IS NUMBER
                                    ///
                                    type = termType.Number;



                                    if (!stopLongTerm) //only if not full stop
                                                       // 1993.
                                    {
                                        casualNumber = number.ToString();
                                        casualNumberBool = true;
                                    }

                                //    stringTerm = i.ToString();
                                    if (isValidInteger && i > 0 && i < 32)  //possible partial day
                                    {
                                        if (possibleDay)//"The 22-23 January edition of the Skopje newspaper VECER in"
                                        {//conflict

                                        }
                                        possibleDay = true;
                                        partialDate = true;
                                        day = i;

                                    }
                                    else if (number > 31 && number < 1000000)
                                    {
                                        //POSSIBLE YEAR = FULL DATE
                                        if (possibleYear)//there is only a year in reg
                                        {//YEAR YEAR CONFLICT
                                         //add last year or discard
                                        }

                                        year = (int)number;
                                        possibleYear = true;
                                        partialDate = true;

                                        //FULL DATE
                                        stopDate = true;
                                    } //possible year
                                    else if (number >= 1000000)
                                    {
                                        stringTerm = (number / 1000000).ToString() + 'M';

                                        //  bigNumber = Func(i) + 'M'; // or + " M" ie/ "1.234 M" "1M" "7000M" 
                                        //35 3/4
                                    }
                                    else if (number < 0)
                                    {

                                    }
                                    else
                                    {

                                    }
                                }

       

                                termToLower = stringTerm;
                            }
                            //possible year
                            /*
                            "the \"Laser 2000\" program to promote the development of semiconductor"
                            //    

                        "lasers.  A total of 270 million marks ($159 million) is to be made"
                            "Semiconductor lasers.  For example, one can already buy 100 watt"
                            "Center,  distributed 1.4 billion markkas ($254 million) in 1993 to"
                            "of about 1 billion marks ($580 million).  Initial projects will"
                            "INTERNATIONAL 22/28 Nov 93) AM"
                            "Media Note from  Cathy Grant at (703) 482-4182."
                            "*Aerospatiale 1992 annual report, 90 pages, in English."
                            "*Carl Zeiss 1991/92 annual report, 87 pages, in English."
                            "Using unusually candid language during more than 12 hours of"
                            "position in talks on 12 and 14 March and bluntly told Secretary"
                            "law permits\" (Xinhua, 12, 14 March).  Qian went on to declare"
                            "Like Qian, Li, in his meeting on 12 March, told Secretary"
                            "and Li, telling Secretary Christopher, in their meeting on 13"

          */
                            ///"Si Ahmed Mourad, a 29-year-old Afghan veteran living in Kouba,"
                            ///"SHIMBUN 1O Jan 87)."
                            ///"Macedonian published on pages 6-7 the results of an opinion poll"
                            ///"Kiro Gligorov, President of the Republic      76/15           78/13"
                            ///"On pages 6 and 7 of its 15-16 January issue, VECER also published"
                            ///"BALKANS BRANCH AT (703) 733-6481)"
                            ///"ELAG/25 February/POLCHF/EED/DEW 28/2023Z FEB"
                            ///"Hours of operation:      0400-2400 GMT Monday-Friday; 24 hours"
                            ///"Day of publication:      1st and 15th of the month"
                            ///"Address:                 Zvonimirova 20/a, Rijeka"
                            ///"Hours of operation:      Monday, Thursday 0810-0000 GMT; Tuesday,"
                            ///"Wednesday 0810-0045 GMT; Friday"
                            ///"0825-0015 GMT"
                            ///"Address:                 Mito Hadzivasilev Jasmin, 910O0 Skopje"
                            ///"Address:                 Cetinjska 3-III, 11001 Beograd"
                            ///"Address:                 Brankova 13-15, 11000 Beograd"
                            ///"Address:                 Narodnog Fronta 45/VII, 11000 Belgrade"
                            ///"Hours of operation:      0715-0030 GMT (Monday-Friday); 0730-0015"
                            ///"733-6120.  Comments and queries concerning the World Media Report"
                            ///"Group, at (703) 733-6131."
                            ///"Document Number: WMR 94-001, Publication Date: 17 February 1994)"
                            ///"Washington, DC 20013-2604, Fax: (703) 733-6042.  For additional"
                            ///"information or assistance, call FBIS at (202) 338-6735."
                            ///"GIG/28FEB94/OSD/PF 01/0305z Mar"
                            ///"EAG/BIETZ/jf 1/1717Z MAR"
                            ///"93-151 \"Regional Census Prompted by Security, Economic Concerns,\" 8"
                            ///"April 1993, and NEAR EAST SOUTH ASIA REPORT 93AFO622B \"Illegal"
                            ///"ENEAG/01 Mar/POLCHF/ECONF/TOTF/NEASA Division/jf 1/2129Z MAR"
                            ///"after the exposure of more than 2O foreign intelligence agents,"
                            ///"a private television company access to Russia's 4th television"
                            ///"an approximate cost between 3O and 7O million francs.  The company's"
                            ///"imported meat from 2O percent to 35 percent; in other words, for"











                            //UPPERCASE 1ST CHAR, s2nd char lowercase

                            if (char.IsUpper(term[0]) && (term.Length > 1 && !char.IsUpper(term[1]) && type == termType.Term)) // is capital letter Term
                            {
                                //ADD TO LONG TERM

                                //no conflict 
                                addTermToLongTerm = true;
                              //  longTermSize++;


                                if (partialDate) //CONFLICt DATE LONGTERM
                                {
                                    if (!possibleMonth)
                                    {
                                        //delete day or year?
                                    }

                                }


                                type = termType.Name;
                                if (Symboled) //has symbol at the end
                                {



                                    if (upperSymbol == ':')
                                    {//restart longterm
                                        stopLongTerm = true;
                                    }
                                    else
                                    if (upperSymbol == '.')
                                    {

                                    }
                                    else
                                    if (upperSymbol == ',')
                                    {

                                    }
                                    else if (upperSymbol == ')')
                                    {
                                        //  stopLongTerm = true;
                                    }
                                    else
                                    {

                                    }
                                }

                                //"R&amp;D;"



                            }


                            string stemTerm = string.Empty;
                            //STEMMER
                            //SHOULD WE STEM NAMES?
                            //months??
                            //numbers
                            if (Indexer.ifStemming == true) //ERROR ON: "Kiro Gligorov, President of the Republic      76/15           78/13"
                            {
                                Stemmer stem = new Stemmer();
                                stem.stemTerm(termToLower);
                                stemTerm = stem.ToString();
                            }

                            else
                            {
                                stemTerm = termToLower;
                            }


                            //term is part of a long term. need to save for next iteration
                            //LONG TERMS
                            if (addTermToLongTerm == true)
                            {
                                if (longTermSize > 0)
                                {
                                    longTerm += "+" + stemTerm; //"Bosnia-Herzegovina."
                                }
                                else
                                {
                                    longTerm = stemTerm;
                                }
                                longTermSize++;
                            }
                    



                            if (type == termType.Term)
                            {

                            }
                            else
                            {

                            }

                            //end of long term



                            //add to dictionary

                            //not if partial date

                            if (partialDate)
                            {
                                //day month year
                                //  YYYY-MM-DD
                                //"KANKEI KOEKI HOJIN BENAAN 1993 May 93), JADI seeks to \"work towards"
                                if (possibleDay && possibleMonth && possibleYear)
                                {

                                }
                                //day month
                                //  MM-DD
                                if (possibleDay && possibleMonth && !possibleYear)
                                {

                                }
                                //month year
                                //  YYYY-MM
                                if (!possibleDay && possibleMonth && possibleYear)
                                {

                                }


                                if (!possibleMonth)
                                {

                                }
                                partialDateCounter++;
                            }

                            //SAVE DATE TO MEMORY
                            if (possibleMonth && type == termType.Term)
                            {
                                stopDate = true;
                            }





                            //ADD TERM TO MEMORY
                            if (myMiniPostingListDict.ContainsKey(stemTerm))
                                myMiniPostingListDict[stemTerm] += "," + wordPositionWithSW;
                            else
                                myMiniPostingListDict.Add(stemTerm, "{" + wordPositionWithSW);



                            if (addNextTermToLongTerm)
                            {

                            }
                            else
                            {

                            }

                            ///"association's annual New Year's party, for example, have been then-"
                            ///"JDA Director-General Koichi Kato (NIKKEI SANGYO SHIMBUN 10 Jan 86)."
                            if (stopLongTerm || (longTermSize > 0 && !addTermToLongTerm && !addTermToLongTerm)) //restart long terms
                            {

                                //ADD SAVE DATE

                                if (addNextTermToLongTerm)
                                {

                                }
                                else
                                {

                                }

                                if (longTermSize > 1)
                                {
                                    ///"association chairman Kosaka Inaba and JDA Equipment Bureau Director-" -> "equipment+bureau+director+general+masaji+yamamoto" no JDA
                                    if (myMiniPostingListDict.ContainsKey(longTerm))
                                        myMiniPostingListDict[longTerm] += "," + (wordPositionWithSW - longTermSize);
                                    else
                                        myMiniPostingListDict.Add(longTerm, "{" + (wordPositionWithSW - longTermSize));
                                }

                                longTerm = string.Empty;
                                longTermSize = 0;
                                stopLongTerm = false;
                            }

                            if (stopDate)
                            {

                                if ((possibleDay && possibleMonth) || (possibleMonth && possibleYear))
                                {
                                    string longDate = "";

                                    if (possibleDay)
                                    {
                                        longDate += day + "/";
                                    }

                                    longDate += month;

                                    
                                    if (possibleYear)
                                    {
                                        longDate +=  "/" + year;
                                    }

                                    

                                   
                                    //add date

                                    //ADD DATE TO MEMORY
                                    if (myMiniPostingListDict.ContainsKey(longDate))
                                        myMiniPostingListDict[longDate] += "," + (wordPositionWithSW - partialDateCounter);
                                    else
                                        myMiniPostingListDict.Add(longDate, "{" + (wordPositionWithSW - partialDateCounter));

                                }
                                //clear partialdate

                                partialDateCounter = 0;

                                possibleDay = false;
                                day = 0;
                                possibleMonth = false;
                                month = 0;
                                possibleYear = false;
                                year = 0;
                                partialDate = false;
                                stopDate = false;
                            }


                            //percent
                            if (casualNumberBool)
                            {
                                if (type == termType.Name)
                                {
                                    complexTerm = "";
                                    bool addIt = false;
                                    if (stemTerm == "percent")
                                    {
                                        //ADD DATE TO MEMORY
                                        addIt = true;
                                        complexTerm = casualNumber + @"%";
                                    }
                                    else
                                    if (stemTerm == "dollar")
                                    {
                                        addIt = true;
                                        complexTerm = casualNumber + @"$";
                                    }
                                    else
                                    {//"Address:                 Dunajska 9, Ljubljana 61000"

                                    }
                                    if (addIt)
                                    {
                                        if (myMiniPostingListDict.ContainsKey(complexTerm))
                                            myMiniPostingListDict[complexTerm] += "," + (wordPositionWithSW - 1);
                                        else
                                            myMiniPostingListDict.Add(complexTerm, "{" + (wordPositionWithSW - 1));
                                    }
                                    casualNumberBool = false;

                                    partialDateCounter = 0;

                                    possibleDay = false;
                                    day = 0;
                                    possibleMonth = false;
                                    month = 0;
                                    possibleYear = false;
                                    year = 0;
                                    partialDate = false;
                                    stopDate = false;
                                }

                                else
                                if (type != termType.Number)
                                {
                                    //restart casual number?
                                    casualNumberBool = false;
                                }

                            }

                        }
                    }
                    previousLine = line;
                }


                //https://www.dotnetperls.com/sort-dictionary



                //compute
                //1.most common term in document
                //term freq:
                //find max

                //compute
                //2.amount of special terms
                //term freq == 1
                //add to?

                //find max tf
                //loop mydict
                //save max

                int maxOccurencesInDocument = 0;
                string maxTerm = "";
                int count = 0;
                foreach (KeyValuePair<string, string> post in myMiniPostingListDict)
                {
                    // added now

                    count = post.Value.Split(',').Length;
                    //found new max
                    if (count > maxOccurencesInDocument)
                    {
                        maxOccurencesInDocument = count;
                        maxTerm = post.Key;
                    }

                    //UniqueList
                    //create array int siveof BIG

                    //countAmountOfUniqueInDoc
                    if (count == 1)
                    {
                        // TempClass.
                        //   UniqueList.Add(post.Key);
                        countAmountOfUniqueInDoc++;
                    }

                }

                //  myDocumentData.                
                string METADATA_SECURE = DOCNO + "#" + maxTerm + "#" + maxOccurencesInDocument +"#" 
                    + languageDocument + "#" + countAmountOfUniqueInDoc + "#" + wordPositionWithSW + "#" + wordPositionWithoutSW;

     
                //MUTEX
                //http://www.c-sharpcorner.com/UploadFile/1d42da/threading-with-mutex/
                //  Indexer._DocumentMetadata.WaitOne();
                //  Console.WriteLine()
                //  Indexer.DocumentMetadata.Add(thisDocNumber.ToString(), METADATA_SECURE);
                //  Indexer._DocumentMetadata.ReleaseMutex();

                wordPositionWithSW = 0;
                wordPositionWithoutSW = 0;
                //http://www.csharpstar.com/return-multiple-values-from-function-csharp/


                foreach (var key in myMiniPostingListDict.Keys.ToList())
                {
                    myMiniPostingListDict[key] += "}@" + thisDocNumber;
                }
                myMiniPostingListDict.Add("<DOCDATA>" + thisDocNumber + '|', METADATA_SECURE);


              //  Console.WriteLine("Doc#: " + thisDocNumber + " Parsed!");
                return myMiniPostingListDict;
            }
        }

      


    }
}
