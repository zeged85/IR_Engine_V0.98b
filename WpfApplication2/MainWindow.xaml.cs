using IR_Engine;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;


namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //essential fields

        //  Indexer tc = new Indexer();
        ViewModel vm;
        //Engine s_Engine;
        bool isValid = true;
        string m_documentsPath, m_postingFilesPath;
        // public static bool stemmingSelected;

        string tmpAddress;
        string tmpForNoStemming;

        string languageChosen;

        public event PropertyChangedEventHandler PropertyChanged;



        public MainWindow()
        {
            InitializeComponent();
            vm = new ViewModel(new Indexer(), new Searcher());
            DataContext = vm;
            vm.VM_Progress = 0;

           // DataContext = this;
            
            this.PropertyChanged +=
                     delegate(Object sender, PropertyChangedEventArgs e)
                     {
                         NotifyPropertyChanged("MW_" + e.PropertyName);
                     };


         //   this.QueryInputTextBox.


             //   AutoCompleteMode = AutoCompleteMode.SuggestAppend;
           // this.textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;

        }

        
            /*
        this.textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
 this.textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;

private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t != null)
            {
                //say you want to do a search when user types 3 or more chars
                if (t.Text.Length >= 3)
                {
                    //SuggestStrings will have the logic to return array of strings either from cache/db
                    string[] arr = SuggestStrings(t.Text);

                    AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                    collection.AddRange(arr);

                    this.textBox1.AutoCompleteCustomSource = collection;
                }
            }
        }


            */




        public void NotifyPropertyChanged(string PropName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropName));
           // Console.WriteLine("test");
        }




        private void Start(object sender, RoutedEventArgs e)
        {
            DateTime m_start = DateTime.Now;

            string error = "";
            Indexer.documentsPath = m_documentsPath + "\\";

            tmpForNoStemming = m_postingFilesPath + "\\";

            if (/*Indexer.ifStemming == true*/ Stemming.IsChecked == true)
            {
                Indexer.postingFilesPath = m_postingFilesPath + "\\" + "Stemming" + "\\";
                Indexer.ifStemming = true;
            }
            else if (/*!Indexer.ifStemming == true*/ Stemming.IsChecked != true)
            {
                Indexer.postingFilesPath = m_postingFilesPath + "\\" + "UnStemming" + "\\";
                Indexer.ifStemming = false;
                // Indexer.postingFilesPath = m_postingFilesPath + "\\";
            }



            if (!Directory.Exists(m_documentsPath))
            {
                isValid = false;
                error = "Path for dataset is missing. Please choose a folder.\n";
            }

            if (!Directory.Exists(m_postingFilesPath))
            {
                isValid = false;
                error += "Path for posting files is missing. Please choose a folder\n";
            }

            if (!File.Exists(m_documentsPath + @"\stop_words.txt"))
            {
                isValid = false;
                error += "Stop words file is missing in the folder.\n";

            }
            if (!isValid)
                System.Windows.Forms.MessageBox.Show(error);


            /*
            if (File.Exists(m_postingFilesPath + "\\UnStemming" + "\\Dictionary.txt") && File.Exists(m_postingFilesPath + "\\UnStemming" + "\\MetaData.txt") && Directory.Exists(m_postingFilesPath + "\\UnStemming" + "\\PostingFiles") && !Stemming.IsChecked==true)
            {
                DialogResult d = System.Windows.Forms.MessageBox.Show("Indexed Files without Stemming already been created. Erase Them and start over?", "Confirm", MessageBoxButtons.YesNo);

                if ( d == System.Windows.Forms.DialogResult.No)
                {
                    System.Windows.Forms.MessageBox.Show("Bye Bye");
                }
                else
                {
                    Reset(this, new RoutedEventArgs());

                    Indexer.docNumber = 0;

                    Thread t1 = new Thread(vm.startEngine);
                    // t1.Start();
                    //  s_Engine.ignite();
                    Thread t2 = new Thread(delegate()
                    {
                        t1.Start();
                        t1.Join();
                        Indexer.clearAllData();
                        Indexer.stopWords.Clear();
                        DateTime m_end = DateTime.Now;
                        string m_time = (m_end - m_start).ToString();
                        MessageBoxResult mbr = System.Windows.MessageBox.Show("Running Time : " + m_time + "\n" + "Number of indexed documents: " + Indexer.docNumber + "\n" + "Number of unique terms: " + Indexer.amountOfUnique, "Output", MessageBoxButton.OK, MessageBoxImage.None);

                    });
                    t2.Start();

                }
            }

            if (File.Exists(m_postingFilesPath + "\\Stemming" + "\\Dictionary.txt") && File.Exists(m_postingFilesPath + "\\Stemming" + "\\MetaData.txt") && Directory.Exists(m_postingFilesPath + "\\Stemming" + "\\PostingFiles") && Stemming.IsChecked==true)
            {
                DialogResult d = System.Windows.Forms.MessageBox.Show("Indexed Files with Stemming already been created. Erase Them and start over?", "Confirm", MessageBoxButtons.YesNo);

                if (d == System.Windows.Forms.DialogResult.No)
                {
                    System.Windows.Forms.MessageBox.Show("Bye Bye");
                }
                else
                {
                    Reset(this, new RoutedEventArgs());

                    Indexer.docNumber = 0;

                    Thread t1 = new Thread(vm.startEngine);
                    // t1.Start();
                    //  s_Engine.ignite();
                    Thread t2 = new Thread(delegate()
                    {
                        t1.Start();
                        t1.Join();
                        Indexer.clearAllData();
                        Indexer.stopWords.Clear();
                        DateTime m_end = DateTime.Now;
                        string m_time = (m_end - m_start).ToString();
                        MessageBoxResult mbr = System.Windows.MessageBox.Show("Running Time : " + m_time + "\n" + "Number of indexed documents: " + Indexer.docNumber + "\n" + "Number of unique terms: " + Indexer.amountOfUnique, "Output", MessageBoxButton.OK, MessageBoxImage.None);

                    });
                    t2.Start();
                }
            }
            */




            
            else
            {
                Indexer.docNumber = 0;

                Thread t1 = new Thread(vm.startEngine);
                // t1.Start();
                //  s_Engine.ignite();
                Thread t2 = new Thread(delegate()
                {
                    t1.Start();
                    t1.Join();
                    Indexer.clearAllData();
                    Indexer.stopWords.Clear();
                    DateTime m_end = DateTime.Now;
                    string m_time = (m_end - m_start).ToString();
                    MessageBoxResult mbr = System.Windows.MessageBox.Show("Running Time : " + m_time + "\n" + "Number of indexed documents: " + Indexer.docNumber + "\n" + "Number of unique terms: " + Indexer.amountOfUnique, "Output", MessageBoxButton.OK, MessageBoxImage.None);

                });
                t2.Start();
            }
            
        }

        private void documents_Browser(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog Dialog = new FolderBrowserDialog();
            Dialog.ShowDialog();
            m_documentsPath = Dialog.SelectedPath;
            documentsFolder_Text.Text = m_documentsPath;
        }


        private void documentsFolderSelected(object sender, RoutedEventArgs e)
        {
            m_documentsPath = documentsFolder_Text.Text;
            // System.Windows.MessageBox.Show("The selected path for dataset: " + m_documentsPath);
        }


        private void postingFiles_Browser(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog Dialog = new FolderBrowserDialog();
            Dialog.ShowDialog();
            m_postingFilesPath = Dialog.SelectedPath;

            postingFilesFolder_Text.Text = m_postingFilesPath;
        }

        private void postingFilesFolderSelected(object sender, RoutedEventArgs e)
        {
            m_postingFilesPath = postingFilesFolder_Text.Text;
        }

        private void isStemming(object sender, RoutedEventArgs e)
        {
            if (Stemming.IsChecked == true)
            {
                Indexer.ifStemming = true;
                Indexer.postingFilesPath = m_postingFilesPath + "\\" + "Stemming" + "\\";
             
                // System.Windows.MessageBox.Show("Please enter path for process with Stemming");
            }
            else
            {
                Indexer.postingFilesPath = m_postingFilesPath + "\\" + "UnStemming" + "\\";
               
                Indexer.ifStemming = false;
            }
        }


        private void Reset(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(m_postingFilesPath))
            {
                System.Windows.MessageBox.Show("Please choose a path for posting files to erase.");
            }

            else
            {
                if (Directory.GetFiles(m_postingFilesPath).Length == 0 && Directory.GetDirectories(m_postingFilesPath).Length == 0)
                {
                    System.Windows.MessageBox.Show("Folder is empty.\n" + "There is no files to erase.");
                }
                else
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(m_postingFilesPath);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
            
                    System.Windows.Forms.MessageBox.Show("All Files Have Been Deleted!");
                }
            }
        }

        private void showDictionaryPressed(object sender, RoutedEventArgs e)
        {
            bool tmp = true;
            string err = string.Empty;
            if (!Directory.Exists(m_postingFilesPath))
            {
                tmp = false;
                err = "Path for posting files is missing.\nCan't find the Dictionary. Please choose a folder\n";
            }

            if (!tmp)
                System.Windows.Forms.MessageBox.Show(err);

            else
            {
            
                    if (File.Exists(Indexer.postingFilesPath + "\\Dictionary.txt"))
                    {
                        DictionaryWindow m_Dictionary = new DictionaryWindow();
                        string str;
                        string[] split;
                        StreamReader s = new StreamReader(Indexer.postingFilesPath + "\\Dictionary.txt");
                        while ((str = s.ReadLine()) != null)
                        {
                            split = str.Split(new string[] { ":", "," }, StringSplitOptions.RemoveEmptyEntries);

                            m_Dictionary.listInverted.Items.Add(/*str*/split[0] + " : " + split[1]);
                        }
                        s.Close();
                        m_Dictionary.Show();
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("The requested Dictionary (without stemming)\n is not exist in the current folder.");
                    }
                
                
            }
        }

        private void languageChoose_Pressed(object sender, RoutedEventArgs e)
        {
            Language m_Language = new Language();

            m_Language.Show();
            // languageChosen = 

        }

        private void loadDictionaryPressed(object sender, RoutedEventArgs e)
        {
            // niros - last commit

            if (m_postingFilesPath == null)
                System.Windows.Forms.MessageBox.Show("Please Choose Posting Files path.");
            else
            {
                
                    if (File.Exists(Indexer.postingFilesPath + "\\Dictionary.txt"))
                    {
                    //test

                    Thread newthread = new Thread(vm.loadDictionary);

                    newthread.Start();
                     //   vm.loadDictionary();



                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Requested dictionary for files created without Stemming,\nIs already exists.");
                    }




               // NotifyPropertyChanged();
            }
        }
    }
}
