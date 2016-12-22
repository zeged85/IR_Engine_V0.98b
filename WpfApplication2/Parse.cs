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
        public static Dictionary<string, string> stopWords = ReadFile.fileToDictionary(Indexer.documentsPath + "\\stop_words.txt" /*@"C:\stopWords\stop_words.txt"*/);
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
                    char[] delimiterChars = { ' ', ',', '.', ':', '\t', '(', ')', '"', '/', '-', '\'', '?', '[', ']', '$', '%', ';', '*', '+', '=', '&', '\'', '`', '#', '|', '\"', '{', '}', '!', '<', '>', '_' };
                    string[] words = line.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);
                    //   System.Console.WriteLine("{0} words in text:", words.Length);

                    lineIdx++;
                    foreach (string s in words)
                    {
                        //    System.Console.WriteLine(s);

                        //Term

                        //Term Term

                        //Term (connection words) Term  (connection words) Term


                        //CW: and, of, the, of-the?, in the?, 

                        string term = s.ToLower();

                        //delete spaces in start
                        term.TrimStart();

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

                        //STOP WORDS
                        if (stopWords.ContainsKey(term))
                            continue;

                        //debug

                        //CONNECTION WORDS

                        string newterm = string.Empty;
                        //STEMMER
                        if (Indexer.ifStemming == true)
                        {
                            Stemmer stem = new Stemmer();
                            stem.stemTerm(term);
                            newterm = stem.ToString();
                        }

                        else
                        {
                            newterm = term;
                        }
                        ReadFile.wordPosition++;

                        if (myMiniPostingListDict.ContainsKey(newterm))
                            myMiniPostingListDict[newterm] += "," + ReadFile.wordPosition;
                        else
                            myMiniPostingListDict.Add(newterm, "{" + ReadFile.wordPosition);
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
    }
}
