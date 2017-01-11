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
       
    
      //  private static int thisDocNumber;
        public static Dictionary<string, string> parseString(string str, int docNumber)
        {

        int  thisDocNumber = docNumber;
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

                Dictionary<string, string> myMiniPostingListDict = new Dictionary<string, string>();

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
                            languagesList.Add(lang);
                            //add to string
                        }

                    }

                    //   Console.WriteLine("");

                    //     Console.WriteLine(line.TrimStart());

                    //    System.Console.WriteLine("Press any key to exit.");
                    //    System.Console.ReadKey();

                    //  limiter--;
                    //    char[] delimiterChars = { ' ', ',', '.', ':', '\t', '(', ')', '"', '/', '-', '\'', '?', '[', ']', '$', '%', ';', '*', '+', '=', '&', '\'', '`', '#', '|', '\"', '{', '}', '!', '<', '>', '_', '\\', '@' };
                    char[] delimiterChars = { ' ' };


          //          List<char> signs = new List<char>();
          //          foreach (char c in delimiterChars)
          //              signs.Add(c);

                     string[] words = line.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);

                    //   System.Console.WriteLine("{0} words in text:", words.Length);
               //     List<string> splitWords = SplitAndKeepDelimiters(line, delimiterChars);

                    lineIdx++;
                    //     foreach (string withspaces in splitWords)
                    //     {
                    //         string[] noSpecs = withspaces.Split();
                


                    foreach (string s in words) /// MAIN PARSE LOOP
                        {
                        //      string term = s.TrimStart().TrimEnd();

                        //    System.Console.WriteLine(s);
                        addTermToLongTerm = false;
                        resetLongTerm = false;

                        //discard single letters
                        if (s.Length == 1)
                            continue;


                        //discard signs in the beginning
                        string term = s;

                        while (term.Length > 1 && !char.IsLetterOrDigit( term[0]))
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
                        if (char.IsNumber(termToLower[0]))
                        {
                            int i = 0;
                            string stringTerm = termToLower; // "108";
                            bool isValidNumber = int.TryParse(stringTerm, out i); //i now = 108  

                            if (isValidNumber && i > 0 && i < 31) //possible partial day
                            {
                                possibleDay = true;
                                partialDate = true;
                                day = i;
                            }

                            if (isValidNumber)
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

                            if (isValidNumber && i > 1000000)
                            {
                              //  bigNumber = Func(i) + 'M'; // or + " M" ie/ "1.234 M" "1M" "7000M" 
                                //35 3/4
                            }

                            if (!isValidNumber)
                            {

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
                           if ( !partialDate)
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
                //   System.Console.WriteLine("");

                //https://www.dotnetperls.com/sort-dictionary

                /*
                var list = myDict.Keys.ToList();
                list.Sort();

                // Loop through keys.
                foreach (var key in list)
                {
                    Console.WriteLine("{0}: {1}", key, myDict[key]);
                }
             
                System.Console.WriteLine("Press any key to exit.");
                System.Console.ReadKey();
                */

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
                    if (post.Value.Split(',').Length == 1)
                    {
                        // TempClass.
                        //   UniqueList.Add(post.Key);
                        countAmountOfUniqueInDoc++;
                    }
                    //unique term found
                    /*
                    int sumOfUniqueTerms = 0;
                    if (count == 0)
                    {
                        //add term to list
                        //count the temr

                        sumOfUniqueTerms++;

                    }
                    */
                }

                //  myDocumentData.                
                string METADATA_SECURE = thisDocNumber + "," + DOCNO /*+ " max_tf (in Document)="*/+ ", " + maxTerm + /*" tf (in Document)="*/ " : " + maxOccurencesInDocument +
                    /*", Language : "*/ ", " + languageDocument + ", uniqueInDoc : " + countAmountOfUniqueInDoc + ", totalInDocIncludingSW : " + wordPositionWithSW + ", totalInDocwithoutSW : " + wordPositionWithoutSW;

                // myMiniPostingListDict.Add("METADATA_SECURE", METADATA_SECURE);

                //MUTEX
                //http://www.c-sharpcorner.com/UploadFile/1d42da/threading-with-mutex/
                Indexer._DocumentMetadata.WaitOne();
              //  Console.WriteLine()
                Indexer.DocumentMetadata.Add(thisDocNumber.ToString(), METADATA_SECURE);
                Indexer._DocumentMetadata.ReleaseMutex();

                wordPositionWithSW = 0;
                wordPositionWithoutSW = 0;
                //http://www.csharpstar.com/return-multiple-values-from-function-csharp/

            
                foreach (var key in myMiniPostingListDict.Keys.ToList())
                {
                    myMiniPostingListDict[key] += "}@" + Indexer.docNumber;
                }


                return myMiniPostingListDict;
            }
        }

        /// http://stackoverflow.com/questions/4680128/c-split-a-string-with-delimiters-but-keep-the-delimiters-in-the-result
        /// </summary>
        /// <param name="s"></param>
        /// <param name="delimiters"></param>
        /// <returns></returns>
        public static List<string> SplitAndKeepDelimiters(string s, params char[] delimiters)
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(s))
            {
                int iFirst = 0;
          
                do
                {
                    int space = s.IndexOf(' ', iFirst);
                    if (space == iFirst) {
                        iFirst++;
                        continue;
                    }


                    int iLast = s.IndexOfAny(delimiters, iFirst);

                    if ((space != -1 && (iLast == -1 || space < iLast)))
                    {
                        parts.Add(s.Substring(space, space - iFirst));
                        iFirst = space + 1;

                        continue;
                    }

                    if (iLast >= 0)
                    {
                        if (iLast > iFirst)
                            parts.Add(s.Substring(iFirst, iLast - iFirst)); //part before the delimiter
                        parts.Add(new string(s[iLast], 1));//the delimiter
                        iFirst = iLast + 1;
                     
                        continue;
                    }

          


                        //No delimiters were found, but at least one character remains. Add the rest and stop.
                        parts.Add(s.Substring(iFirst, s.Length - iFirst));
                    break;

                } while (iFirst < s.Length);
            }

            return parts;
        }


    }
}
