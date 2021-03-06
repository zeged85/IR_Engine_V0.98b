﻿using IR_Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IR_Engine
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private IModel model_indexer;
        private ISearcher search;
        public ViewModel(IModel index, ISearcher searcher)
        {
            this.model_indexer = index;
            this.search = searcher;

            search.PropertyChanged +=
                          delegate (Object sender, PropertyChangedEventArgs e)
                          {
                              this.NotifyPropertyChanged("VM_" + e.PropertyName);
                          };

            model_indexer.PropertyChanged +=
                          delegate(Object sender, PropertyChangedEventArgs e)
                          {
                              this.NotifyPropertyChanged("VM_" + e.PropertyName);
                          };

           // docResult = new string.DefaultIfEmpty();
        }

        public void NotifyPropertyChanged(string PropName)
        {


          //  progress = model_indexer.Progress;


            if (PropName == "VM_Searcher_DocResult")
            {
                docResult = search.DocResult;
                PropName = "VM_DocResult";
            }



            if (PropName == "VM_Progress")
            {
                progress = model_indexer.Progress;
                //PropName = "VM_DocResult";
            }



            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropName));



            
        }

        private int progress;
        public int VM_Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                this.NotifyPropertyChanged("VM_Progress");
            }
        }

        private int docSum;
        public int VM_DocSum
        {
            get { return docSum; }
            set
            {
                docSum = value;
                this.NotifyPropertyChanged("VM_DocSum");
            }
        }


        private string docResult;
        public string VM_DocResult
        {
            get { return docResult; }
            set
            {
                docResult = value;
                this.NotifyPropertyChanged("VM_DocResult");
            }
        }

        private string queryInput;
        public string VM_QueryInput
        {
            get { return queryInput; }
            set
            {
                //check if dic loaded


                //fire query
              
                
                /*
                if (value[value.Length-1] == ' ')// if user entered ' ' char
                {//check term1-term*

             
                    //show most populer term*


                }
                */

                
                queryInput = value;


                   

        //        SortedDictionary<string,double> docRankRes = VMsearchQuery(queryInput);


          //      string[] SYNONYMS = search.getSYNONYMS(queryInput);
                //sort

                //show results on screen

                //queryoutput



                this.NotifyPropertyChanged("VM_QueryInput");
                


             //   triger searchResult
            }
        }






        /*
        public void loadPostingFiles()
        {
            model.loadPostingFiles();

        }

        public void createDictionary()
        {
            model.createDictionary();
        }
        */
      

        public void startEngine()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            model_indexer.clearAllData();

            VM_DocResult = "loading files...";

            model_indexer.loadMonths();

            model_indexer.initiate(); //

           // model_indexer.Progress = 55;

            model_indexer.freeMemory(); // create last folder

            model_indexer.Progress = 50;
            VM_DocResult = "merging...";
            model_indexer.MergeAllToSingleUnSorted();

            model_indexer.Progress = 55;
            VM_DocResult = "sorting...";
            model_indexer.sort();

            model_indexer.Progress = 60;

            model_indexer.deleteGarbage();

            model_indexer.Progress = 60;

          //  model_indexer.dumpDocumentMetadata(); //create meta.txt

            model_indexer.Progress = 65;
            VM_DocResult = "loading Posting Files...";
            model_indexer.loadPostingFiles();

            model_indexer.Progress = 70;

         //   model_indexer.loadMetadata();

            model_indexer.Progress = 75;
            VM_DocResult = "createing Dictionary...";
            model_indexer.createDictionary(); // and save to file dict.txt

            //

            model_indexer.Progress = 80;

            //

            //   model_indexer. loadDictionary(); //from dict.txt

            //


            model_indexer.UniqueWordsQuery(); //CREATE METADATA

            model_indexer.Progress = 85;

            //model_indexer.PrintfreqInAllCorpusList(); //TOP 10 BOTTOM 10 

            model_indexer.Progress = 90;


            

            model_indexer.mmm();


            //
            //check is indexer full

            Indexer.myPostings.Clear();

            loadDictionary();

            model_indexer.Progress = 95;
            VM_DocResult = "updating pointers...";

            model_indexer.addPointers();

            loadDictionary(); //from dict.txt

            model_indexer.Progress = 99;

            VM_DocResult = "building cache...";

            model_indexer.createCache();



            model_indexer.Progress = 100;
            //
            VM_DocResult = "done.";
        }

        public string[] getSYNONYMS(string queryInput){

           return search.getSYNONYMS(queryInput);
        }

        public void openQueryFile(string path)
        {
            search.openQueryFile(path);
        }

        public void runSingleQuery(string query, string[] SYNONYMS, int query_id)
        {
            search.runSingleQuery(query, SYNONYMS, query_id);
        }

        public SortedDictionary<string,double> VMsearchQuery(string query)
        {
            SortedDictionary<string, double> DocRankRes = search.processFullTermQuery(query);
            if (DocRankRes.Count > 1)
                Console.WriteLine("Word exists in memory");
            else
                Console.WriteLine("Word does not exist in memory");

            return DocRankRes;
        }

        public List<string> autoComplete(string querySingleTerm)
        {
            if (queryInput.Substring(querySingleTerm.Length-1)==" ")
            {
                string res = querySingleTerm.TrimEnd();

                string plus = res.Replace(' ', '+');
                return search.autoComplete(plus);
            }

            return new List<string>();
        }

        public string getOutputFolder()
        {
            return model_indexer.getOutputFolder();
        }

        public string getDocument(int docID)
        {
            return search.getDocument(docID);
        }

        public void setOutputFolder(string path)
        {

            model_indexer.setOutputFolder(path);
            search.setOutputFolder(path);
        }

        public string[] getWiki(string term)
        {
            return search.getWiki(term);
        }

        public void loadDictionary()
        {

            //make dict singleton
            VM_DocResult = "Loading Dictionary...";
            model_indexer.loadDictionary();

            //load metadata also

            model_indexer.loadMetadata();
            VM_DocResult = "Ready.";
            //get dictionary size

            //get metadata size

            //is this needed?
            //  Indexer.clearAllData();

        }

        public void clearAllData()
        {
            model_indexer.clearAllData();
        }
    }
}
