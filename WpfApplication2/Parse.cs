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
                }

                //http://stackoverflow.com/questions/8459928/how-to-count-occurences-of-unique-values-in-dictionary

                SortedDictionary<string, string> myMiniPostingListDict = new SortedDictionary<string, string>();

                //text parsing - main work
                int lineIdx = 0; // lines in document
                // int limiter = 10;

                string longTerm = string.Empty;
                int longTermSize = 0;
                bool addTermToLongTerm = false;
                bool resetLongTerm = false;
                bool partialDate = false;
                bool possibleDay = false;
                bool possibleYear = false;
                bool possibleMonth = false;
                int month;
                int day;
                int year;

               // int limiter = 10;
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
                   

                    foreach (string s in words) /// MAIN PARSE LOOP
                    {
                      
                        addTermToLongTerm = false;
                        resetLongTerm = false;

                        //discard single letters
                        if (s.Length == 1)
                            continue;


                        //discard signs in the beginning
                        string term = s;

                        while (term.Length > 1 && !char.IsLetterOrDigit(term[0]))
                        {
                            term = term.Split(term[0])[1];
                        }

                        // int length = term.Length;
                        // char lastChar = term[length - 1];
                        //discard signs in the ending
                        while (term.Length > 1 && !char.IsLetterOrDigit(term[term.Length - 1]))
                        {
                            string[] split = term.Split(term[term.Length - 1]);
                            char sign = term[term.Length - 1];
                            term = split[0];


                            if (sign == '.')
                            {
                                resetLongTerm = true;
                                possibleDay = false;
                                day = 0;
                                possibleMonth = false;
                                month = 0;
                                possibleYear = false;
                                year = 0;
                                partialDate = false;
                            }
                            //   longTerm = string.Empty;//restart long term

                            if (sign == ',')
                            {

                            }

                            if (sign == '(')
                            {

                            }

                            if (sign == ')')
                            {

                            }



                        }
                        term = term.TrimEnd();

                        //discard single letters
                        if (term.Length < 2)
                            continue;



                        string termToLower = term.ToLower();

                        //months before SW because SW contains "may"
                        if (Indexer.Months.ContainsKey(termToLower))
                        {
                            partialDate = true;
                            possibleMonth = true;
                            month = Indexer.Months[termToLower];
                            addTermToLongTerm = false;
                        }

                        //STOP WORDS
                        if (Indexer.stopWords.ContainsKey(termToLower))
                        {
                            wordPositionWithSW++;
                            continue;
                        }



                        //check number 
                        //https://msdn.microsoft.com/en-us/library/bb384043.aspx
                        //try parse
                        //NUMBERS
                        //https://msdn.microsoft.com/en-us/library/9zbda557(v=vs.110).aspx
                        string stringTerm = termToLower;

                        string value = stringTerm; //"$1,643.57";
                        bool isValidNumber;
                        bool isValidInteger;

                        if (char.IsNumber(termToLower[0]))
                        {
                            int i = 0;
                            decimal number;
                            // "108";
                            isValidNumber = Decimal.TryParse(stringTerm, out number); //i now = 108  
                            isValidInteger = int.TryParse(stringTerm, out i);

                            if (isValidInteger && i > 0 && i < 31) //possible partial day
                            {
                                possibleDay = true;
                                partialDate = true;
                                day = i;
                            }

                            if (isValidInteger)
                            {
                                if (i > 31)
                                {
                                    if (possibleYear)//there is only a year in reg
                                    {
                                        //add last year or discard
                                    }

                                    year = i;
                                    possibleYear = true;
                                    partialDate = true;

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

                                //Term Number
                            }

                            if (isValidNumber && i >= 1000000)
                            {
                                string newNumber = (i / 1000000).ToString() + 'M';
                                if (myMiniPostingListDict.ContainsKey(newNumber))
                                    myMiniPostingListDict[newNumber] += "," + wordPositionWithSW;
                                else
                                    myMiniPostingListDict.Add(newNumber, "{" + wordPositionWithSW);

                                continue;
                                //  bigNumber = Func(i) + 'M'; // or + " M" ie/ "1.234 M" "1M" "7000M" 
                                //35 3/4
                            }
                            else
                                   if (isValidNumber && isValidInteger)
                            {
                                // Console.WriteLine(number);
                                string strNum = number.ToString();
                                if (myMiniPostingListDict.ContainsKey(strNum))
                                    myMiniPostingListDict[strNum] += "," + wordPositionWithSW;
                                else
                                    myMiniPostingListDict.Add(strNum, "{" + wordPositionWithSW);

                                continue;
                            }
                            if (!isValidNumber)
                            {

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



                   
                                Console.WriteLine("Unable to parse '{0}'.", value);
                                continue;
                            }
                            }
                        
                    
                        if (Indexer.Months.ContainsKey(termToLower))
                        {
                            partialDate = true;
                            possibleMonth = true;
                            month = Indexer.Months[termToLower];
                            addTermToLongTerm = false;
                        }

                        //UPPERCASE 1ST CHAR

                        if (addTermToLongTerm && char.IsUpper(term[0]) && !char.IsUpper(term[1])) // is capital letter Term
                        {
                            if (!partialDate)
                            {  //ADD TO LONG TERM
                                addTermToLongTerm = true;
                                longTermSize++;
                            }
                            else
                            {

                            }
                        }


                        string stemTerm = string.Empty;
                        //STEMMER
                        if (Indexer.ifStemming == true)
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
                            longTerm += stemTerm + "+";
                        }

                        wordPositionWithoutSW++;
                        wordPositionWithSW++;

                        //end of long term
                        if (resetLongTerm)
                        {
                            if (longTermSize > 1)
                            {
                                if (myMiniPostingListDict.ContainsKey(longTerm))
                                    myMiniPostingListDict[longTerm] += "," + wordPositionWithSW;
                                else
                                    myMiniPostingListDict.Add(longTerm, "{" + wordPositionWithSW);
                            }

                            longTerm = string.Empty;
                            longTermSize = 0;

                        }


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
                        }


                        if (myMiniPostingListDict.ContainsKey(stemTerm))
                            myMiniPostingListDict[stemTerm] += "," + wordPositionWithSW;
                        else
                            myMiniPostingListDict.Add(stemTerm, "{" + wordPositionWithSW);
                    }

                    // Keep the console window open in debug mode.
                    //NEW DEBUG


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
                string METADATA_SECURE = /*thisDocNumber + "^" + */DOCNO /*+ " max_tf (in Document)="*/+ ", " + maxTerm + /*" tf (in Document)="*/ " : " + maxOccurencesInDocument +
                    /*", Language : "*/ ", " + languageDocument + ", uniqueInDoc : " + countAmountOfUniqueInDoc + ", totalInDocIncludingSW : " + wordPositionWithSW + ", totalInDocwithoutSW : " + wordPositionWithoutSW;

                myMiniPostingListDict.Add("<DOCDATA>" + thisDocNumber + '|', METADATA_SECURE);


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

                Console.WriteLine("Doc#: " + thisDocNumber + " Parsed!");
                return myMiniPostingListDict;
            }
        }

      


    }
}
