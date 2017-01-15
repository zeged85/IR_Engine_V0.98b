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
            vm = new ViewModel(new Indexer());
            DataContext = vm;
            vm.VM_Progress = 0;

            vm.PropertyChanged +=
                     delegate(Object sender, PropertyChangedEventArgs e)
                     {
                         NotifyPropertyChanged("MW_" + e.PropertyName);
                     };
        }

        public void NotifyPropertyChanged(string PropName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropName));
        }




        private void Start(object sender, RoutedEventArgs e)
        {
            DateTime m_start = DateTime.Now;

            string error = "";
            Indexer.documentsPath = m_documentsPath + "\\";

            tmpForNoStemming = m_postingFilesPath + "\\";

            if (Indexer.ifStemming == true)
            {
                Indexer.postingFilesPath = m_postingFilesPath + "\\" + "Stemming" + "\\";
            }
            else
            {
                Indexer.postingFilesPath = tmpForNoStemming;
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


            else
            {
                Indexer.docNumber = 0;


                // Thread t1 = new Thread(tc.mmm); ;
                //tc.mmm();
                //t1.Start();


                // s_Engine = new Engine(vm.model);
                // vm.startEngine();
                //  s_Engine.ignite();
                Thread t1 = new Thread(vm.startEngine);
                // t1.Start();
                //  s_Engine.ignite();
                Thread t2 = new Thread(delegate()
                {
                    t1.Start();
                    t1.Join();
                    Indexer.clearAllData();
                    DateTime m_end = DateTime.Now;
                    string m_time = (m_end - m_start).ToString();
                    MessageBoxResult mbr = System.Windows.MessageBox.Show("Running Time : " + m_time + "\n" + "Number of indexed documents: " + Indexer.docNumber + "\n" + "Number of unique terms: " + Indexer.amountOfUnique, "Output", MessageBoxButton.OK, MessageBoxImage.None);


                });
                t2.Start();
               // t2.Join();
                /*
                DateTime m_end = DateTime.Now;
                string m_time = (m_end - m_start).ToString();
                MessageBoxResult mbr = System.Windows.MessageBox.Show("Running Time : " + m_time + "\n" + "Number of indexed documents: " + ReadFile.totalDocs + "\n" + "Number of unique terms: " + Indexer.amountOfUnique, "Output", MessageBoxButton.OK, MessageBoxImage.None);
                */
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
            System.Windows.MessageBox.Show("The selected path for dataset: " + m_documentsPath);
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
                // System.Windows.MessageBox.Show("Please enter path for process with Stemming");
            }
            else
            {
                Indexer.ifStemming = false;
            }

        }


        private void Reset(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(m_postingFilesPath))
            {
                System.Windows.MessageBox.Show("Please choose a path for posting files to erase.");
            }

            //    else if{
            //      System.Windows.MessageBox.Show("Folder is empty.\n" + "There is no files to erase.");
            //}
            else
            {
                if (Directory.GetFiles(m_postingFilesPath).Length == 0 && Directory.GetDirectories(m_postingFilesPath).Length == 0)
                {
                    System.Windows.MessageBox.Show("Folder is empty.\n" + "There is no files to erase.");

                }
                else
                {
                    // m_postingFilesPath = tmpAddress;
                    if (Stemming.IsChecked == true)
                    {

                        DirectoryInfo d = new DirectoryInfo(/*TempClass.postingFilesPath*/ m_postingFilesPath);
                        DirectoryInfo dPost = new DirectoryInfo(/*TempClass.postingFilesPath*/m_postingFilesPath + "\\PostingFiles");
                        if (d.Exists)
                        {
                            foreach (FileInfo f in d.GetFiles())
                                f.Delete();
                            //   foreach (di di in d.GetDirectories())
                            //     di.Delete();
                        }

                        if (dPost.Exists)
                        {
                            foreach (FileInfo f in dPost.GetFiles())
                                f.Delete();
                            Directory.Delete(m_postingFilesPath + "\\PostingFiles");
                        }

                        Indexer.postingFilesPath = tmpAddress;
                        //m_postingFilesPath = tmpAddress;

                        DirectoryInfo stem = new DirectoryInfo(/*TempClass.postingFilesPath*/m_postingFilesPath + "\\Stemming");

                        if (stem.Exists)
                        {
                            foreach (FileInfo f in stem.GetFiles())
                                f.Delete();
                            Directory.Delete(m_postingFilesPath + "\\Stemming");
                        }
                    }

                    DirectoryInfo di = new DirectoryInfo(/*TempClass.postingFilesPath*/ m_postingFilesPath);
                    DirectoryInfo dj = new DirectoryInfo(/*TempClass.postingFilesPath*/ m_postingFilesPath + "\\PostingFiles");
                    DirectoryInfo ds = new DirectoryInfo(m_postingFilesPath + "\\Stemming");

                    if (di.Exists)
                    {
                        foreach (FileInfo f in di.GetFiles())
                            f.Delete();

                        //   foreach (di di in d.GetDirectories())
                        //     di.Delete();
                    }

                    if (dj.Exists)
                    {
                        foreach (FileInfo f in dj.GetFiles())
                            f.Delete();
                        Directory.Delete(m_postingFilesPath + "\\PostingFiles");
                    }

                    if (ds.Exists)
                    {
                        foreach (FileInfo f in ds.GetFiles())
                            f.Delete();

                        foreach (DirectoryInfo dir in ds.GetDirectories())
                            dir.Delete(true);
                        Directory.Delete(m_postingFilesPath + "\\Stemming");
                    }
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
                DictionaryWindow m_Dictionary = new DictionaryWindow();
                string str;
                string[] split;
                StreamReader s = new StreamReader(m_postingFilesPath + "\\Dictionary.txt");
                while ((str = s.ReadLine()) != null)
                {
                    split = str.Split(new string[] { ":", "," }, StringSplitOptions.RemoveEmptyEntries);
                    
                    m_Dictionary.listInverted.Items.Add(/*str*/split[0] + " : " + split[1]);
                }
                s.Close();
                m_Dictionary.Show();
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

            if (m_postingFilesPath == null)
                System.Windows.Forms.MessageBox.Show("Please Choose Posting Files path.");
            else
            {
                //test
                Indexer.postingFilesPath = m_postingFilesPath + "\\";
                vm.loadPostingFiles();
                vm.createDictionary();
            }
        }
    }
}
