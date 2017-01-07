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
        public static int countAmountOfUniqueInDoc = 0;
        public static Dictionary<string, string> parseString(string str)
        {
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

                    string longTerm = string.Empty;
                        foreach (string s in words) /// MAIN PARSE LOOP
                        {
                        //      string term = s.TrimStart().TrimEnd();

                        //    System.Console.WriteLine(s);



                        //discard single letters
                        if (s.Length == 1)
                            continue;


                        //discard signs in the beginning
                        string term = s;

                        while (!char.IsLetterOrDigit( term[0]))
                        {
                            term = term.Split(term[0])[1];
                        }


                        //discard single letters
                       if (term.Length == 1)
                          continue;

                        //end of line or sentence or comma



                        int length = term.Length;
                        char lastChar = term[length - 1];
                        if (!char.IsLetterOrDigit(lastChar))
                        {
                            term = term.Split(lastChar)[0];
                            longTerm = string.Empty;//restart long term

                        }
                        else
                        {
                        //    term = s;
                        }




                        if ( char.IsUpper(term[0])  && !char.IsUpper(term[1]) ) // is capital letter Term
                        {
                            longTerm += term + " ";
                        }
                        else
                        
{
                     //       longTerm = string.Empty;//restart long term
                        }

                        //STOP WORDS
                        if (Indexer.stopWords.ContainsKey(term))
                            continue;

                        //Term

                        //Term Term

                        //Term (connection words) Term  (connection words) Term

                        /*  if (signs.Contains(s[0]))
                          {
                              if (s[0] == '!') { }
                              if (s[0] == '-') { }
                              continue;
                          }*/




                        //CW: and, of, the, of-the?, in the?, 

                        string termToLower = s.ToLower();

                            //delete spaces in start
                          //  termToLower.TrimStart();

                            //NUMBERS

                            //NUMBER-NUMBER

                            //CURRENCY


                            //NUMBER%  

                            //NUMBER percent/percentage -> NUMBER %

                            //30.5% percent

                            //1,023$



                            //(WORD) (WORD) (WORD)
                            //this is it

                            //(WORD) (WORD) (NUMBER)

                            //(WORD) (WORD) (SIGN)



                            //(WORD) (NUMBER) (WORD)

                            //(WORD) (NUMBER) (NUMBER)

                            //(WORD) (NUMBER) (SIGN)


                            //(WORD) (SIGN) (WORD)

                            //(WORD) (SIGN) (NUMBER)

                            //(WORD) (SIGN) (SIGN)


                            //(NUMBER) (WORD) (WORD)

                            //(NUMBER (WORD) (NUMBER)
                            //(NUMBER) (WORD) (SIGN)

                            //(NUMBER) (NUMBER) (WORD)
                            //(NUMBER) (NUMBER) (NUMBER)
                            //(NUMBER) (NUMBER) (SIGN) 

                            //(WORD) (NUMBER) (SIGN)

                            //(NUMBER) (WORD) (SIGN)

                            //

                            //3.05 M Dollars

                            //CHANGE MILLION+
                            //SPARE 1,024 OR 3.14 OR 5 5/6

                            //NUMBER,NUMBER

                            //NUMBER,NUMBER,NUMBER -> 3 M

                            //NUMBER,NUMBER,NUMBER,NUMBER -> 9 B

                            //TERM,TERM -> TERM, TERM

                            //TERMS

                            //TERM-TERM

                            //TERM'S TERMS' T3RMS TERM3 TERM.TERM TERM.TERM.TERM 


                            //debug

                            //CONNECTION WORDS

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
                            ReadFile.wordPosition++;

                            if (myMiniPostingListDict.ContainsKey(stemTerm))
                                myMiniPostingListDict[stemTerm] += "," + ReadFile.wordPosition;
                            else
                                myMiniPostingListDict.Add(stemTerm, "{" + ReadFile.wordPosition);
                        }

                        // Keep the console window open in debug mode.
                    

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
                string METADATA_SECURE = /*"DOCNO:" + */DOCNO /*+ " max_tf (in Document)="*/+ ", " + maxTerm + /*" tf (in Document)="*/ " : " + maxOccurencesInDocument +
                    /*", Language : "*/ ", " + languageDocument + ", uniqueInDoc : " + countAmountOfUniqueInDoc;

                // myMiniPostingListDict.Add("METADATA_SECURE", METADATA_SECURE);
                Indexer.DocumentMetadata.Add(Indexer.docNumber.ToString(), METADATA_SECURE);

                //http://www.csharpstar.com/return-multiple-values-from-function-csharp/

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
