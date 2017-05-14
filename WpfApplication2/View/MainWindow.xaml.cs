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
        ViewModel vm;

        bool isValid = true;
        string m_documentsPath, m_postingFilesPath;


       // string tmpAddress;
        string tmpForNoStemming;

       // string languageChosen;
        bool isDictionaryLoaded = false;
        public event PropertyChangedEventHandler PropertyChanged;

       // public List<string> namelist = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            QueryInputTextBox.IsReadOnly = true;
            vm = new ViewModel(new Indexer());
            DataContext = vm;
            vm.VM_Progress = 0;
            // DataContext = this;

            this.PropertyChanged +=
                     delegate(Object sender, PropertyChangedEventArgs e)
                     {
                         NotifyPropertyChanged("MW_" + e.PropertyName);
                     };
        }

        public void NotifyPropertyChanged(string PropName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropName));
            // Console.WriteLine("test");
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            isValid = true;
            QueryInputTextBox.IsReadOnly = true;
            DateTime m_start = DateTime.Now;
            isStemming(this, null);
            string error = "";
      //      Indexer.documentsPath = m_documentsPath + "\\";

       //     tmpForNoStemming = m_postingFilesPath + "\\";

           

            if (!Directory.Exists(m_documentsPath))
            {
                isValid = false;
                error = "Path for dataset is missing. Please choose a folder.\n";
            }

            if (!Directory.EnumerateFileSystemEntries(m_documentsPath).Any())
            {
                isValid = false;
                error = "No Data Files found on folder.\n";
            }

            if (!Directory.Exists(m_postingFilesPath))
            {
                isValid = false;
                error += "Path for posting files is missing. Please choose a folder\n";
            }

            if (!File.Exists(m_documentsPath + @"\movies.csv"))
            {
                isValid = false;
                error += "movies.csv missing in the folder.\n";
            }
            
            if (!isValid)
                System.Windows.Forms.MessageBox.Show(error);

            else
            {
                Indexer.titleNumber = 0;
             //   Indexer.amountOfUnique = 0;
                isDictionaryLoaded = true;

                Thread t1 = new Thread(vm.startEngine);
                // t1.Start();
                //  s_Engine.ignite();
                Thread t2 = new Thread(delegate()
                {
                    t1.Start();
                    t1.Join();
                    ///http://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                    ///Whenever you update your UI elements from a thread other than the main thread, you need to use:
                    this.Dispatcher.Invoke(() =>
                    {
                        QueryInputTextBox.IsReadOnly = false;
                    });

                    //   Indexer.clearAllData();
                    //   Indexer.stopWords.Clear();
                    DateTime m_end = DateTime.Now;
                    string m_time = (m_end - m_start).ToString();
                    MessageBoxResult mbr = System.Windows.MessageBox.Show("Running Time : " + m_time + "\n" + "Number of indexed movies: " + Indexer.titleNumber + "\n" + "Number of unique movie-id ratings: " + Indexer.myRatings.Count, "Output", MessageBoxButton.OK, MessageBoxImage.None);

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
            Indexer.documentsPath = m_documentsPath;
        }


        private void documentsFolderSelected(object sender, RoutedEventArgs e)
        {
            m_documentsPath = documentsFolder_Text.Text;
            Indexer.documentsPath = m_documentsPath;
            // System.Windows.MessageBox.Show("The selected path for dataset: " + m_documentsPath);
        }


        private void postingFiles_Browser(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog Dialog = new FolderBrowserDialog();
            Dialog.ShowDialog();
            m_postingFilesPath = Dialog.SelectedPath;

            postingFilesFolder_Text.Text = m_postingFilesPath;
            isStemming(this, null);
        }

        private void postingFilesFolderSelected(object sender, RoutedEventArgs e)
        {
            m_postingFilesPath = postingFilesFolder_Text.Text;
            isStemming(this, null);
        }

        private void isStemming(object sender, RoutedEventArgs e)
        {
            if (Stemming.IsChecked == true)
            {
                Indexer.ifStemming = true;
                vm.setOutputFolder(m_postingFilesPath + "\\" + "Stemming" + "\\");

            }
            else
            {
                vm.setOutputFolder(  m_postingFilesPath + "\\" + "UnStemming" + "\\");

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
                    QueryInputTextBox.IsReadOnly = true;
                    isDictionaryLoaded = false;
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
                isStemming(this, null);

                if (File.Exists(vm.getOutputFolder() + "\\Dictionary.txt"))
                {
                    DictionaryWindow m_Dictionary = new DictionaryWindow();
                    string str;
                    string[] split;
                    StreamReader s = new StreamReader(vm.getOutputFolder() + "\\Dictionary.txt");
                    while ((str = s.ReadLine()) != null)
                    {
                        split = str.Split(new string[] { "^", "#" }, StringSplitOptions.RemoveEmptyEntries);

                        m_Dictionary.listInverted.Items.Add(/*str*/split[0] + " : " + split[1]);
                    }
                    s.Close();
                    m_Dictionary.Show();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("The requested Dictionary is not exist in the current folder.");
                }
            }
            isStemming(this, null);
        }

        private void languageChoose_Pressed(object sender, RoutedEventArgs e)
        {
            Language m_Language = new Language();
            m_Language.Show();
        }

        private void loadDictionaryPressed(object sender, RoutedEventArgs e)
        {
            // niros - last commit

            if (m_postingFilesPath == null)
                System.Windows.Forms.MessageBox.Show("Please Choose Posting Files path.");
            else
            {
                isStemming(this, null);
                string folder = vm.getOutputFolder();

                if (File.Exists(folder + "Dictionary.txt"))
                {
                    //test

                    Thread newthread = new Thread(vm.loadDictionary);

                    newthread.Start();
                    QueryInputTextBox.IsReadOnly = false;
                    //   vm.loadDictionary();
                    isDictionaryLoaded = true;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Files does not existed in the folder.");
                }
                // NotifyPropertyChanged();
            }

        }

        private void txtAutoSuggestName_TextChanged(object sender, TextChangedEventArgs e)
        {
            listBoxSuggestion.Items.Clear();
            if (QueryInputTextBox.Text != "")
            {
                List<string> namelist = vm.autoComplete(QueryInputTextBox.Text);
                if (namelist.Count > 0)
                {
                    listBoxSuggestion.Visibility = Visibility.Visible;
                    foreach (var obj in namelist)
                    {
                        listBoxSuggestion.Items.Add(obj);
                    }
                }//
               // listBoxSuggestion.Visibility = Visibility.Hidden;
            }
            else
            {
                listBoxSuggestion.Visibility = Visibility.Hidden;
            }
        }

       // http://www.wpf-tutorial.com/dialogs/creating-a-custom-input-dialog/
        private void btnEnterName_Click(object sender, RoutedEventArgs e)
        {
        //    InputDialogSample inputDialog = new InputDialogSample("Please enter your name:", "John Doe");
       //     if (inputDialog.ShowDialog() == true)
        //        lblName.Text = inputDialog.Answer;
        }

        private void Suggest_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.ClickCount == 2)
            listBoxMyMovies.Items.Add(e.OriginalSource);
            string title = string.Copy( listBoxSuggestion.SelectedItem.ToString());


            vm.VM_DocResult = title;

            //http://www.wpf-tutorial.com/dialogs/the-messagebox/
            MessageBoxResult result = System.Windows. MessageBox.Show("Did you like the movie \"" + title + "\"?", "My App", System.Windows.MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    System.Windows.MessageBox.Show("Hello to you too!", "My App");
                    break;
                case MessageBoxResult.No:
                    System.Windows.MessageBox.Show("Oh well, too bad!", "My App");
                    break;
                case MessageBoxResult.Cancel:
                    System.Windows.MessageBox.Show("Nevermind then...", "My App");
                    break;
            }
            vm.VM_selectMovie(title);


          //  System.Windows.Forms.MessageBox.Show("Double Click");
        }

        private void txtAutoSuggestName_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
            if (e.Key == Key.Down)
            {
               
                //  listBoxSuggestion.Focus();
                //   listBoxSuggestion.SelectedIndex = 0;
                ListBoxItem item = (ListBoxItem)listBoxSuggestion.ItemContainerGenerator.ContainerFromIndex(0);
                FocusManager.SetFocusedElement(this, item);
                
                // http://stackoverflow.com/questions/2591259/cancel-key-press-event
                e.Handled = true;


            }
            if (e.Key == Key.Enter)
            {

                listBoxMyMovies.Items.Add(listBoxSuggestion.Items.CurrentPosition);

                if (isDictionaryLoaded == true /* && !string.IsNullOrEmpty(Searcher.pathForResult)*/)
                {

                   // Searcher.singleQueryInput = QueryInputTextBox.Text;
                    //string[] split = Searcher.singleQueryInput.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                    string query = QueryInputTextBox.Text;


                    query = query.Trim();
                    
                    if (query.Last().ToString()=="."){
                        query = query.Substring(0,query.Length-2);
                    }
                    if (query.Contains(','))
                    {
                        query = query.Remove(',');
                    }
  
                    if (query.Contains(' ')){
                    query = query.Replace(' ', '+');
                    }

                    
                    string[] allTerms = query.Split('+');
                    List<string> syn = new List<string>();
                    /*
                    foreach (string term in allTerms)
                    {
                        string[] SYNOms = vm.getSYNONYMS(term);
                        foreach (string str in SYNOms)
                        {
                            if (!syn.Contains(str))
                            {
                                syn.Add(str);
                            }
                        }
                    }

                    */
                    string[] SYNONYMS  = syn.ToArray();



                    if (Indexer.ifStemming == true)
                    {
                        Stemmer stem = new Stemmer();

                        for (int i = 0; i < SYNONYMS.Length - 1; i++)
                        {
                            SYNONYMS[i] = stem.stemTerm(SYNONYMS[i]);
                        }

                            if (query.Contains('+'))
                            {
                                string[] str = query.Split('+');

                                query = stem.stemTerm(str[0]);

                                foreach (string s in str)
                                {
                                    if (s == str[0])
                                        continue;
                                    query += "+" + stem.stemTerm(s);
                                }
                            }
                            else
                            {
                                query = stem.stemTerm(query);
                            }

                    }


                  //  vm.runSingleQuery(query, SYNONYMS);
                    System.Windows.Forms.MessageBox.Show("Query Activated");

                    QueryInputTextBox.IsReadOnly = true;
                    System.Threading.Thread.Sleep(5000);
                    QueryInputTextBox.IsReadOnly = false;
                   // Searcher.languageChosen.Clear();

                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Please load dictionary,\nAnd/Or choose a path for result file.");
                }
            }
         
        }


        private void listBoxSuggestion_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (listBoxSuggestion.SelectedIndex == 0 && e.Key == Key.Up)
            {
                QueryInputTextBox.Focus();
            }
        }

        private void listBoxSuggestion_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (listBoxSuggestion.SelectedIndex > -1)
            {
                if (e.Key == Key.Enter)
                {
                    //  QueryInputTextBox.Text = listBoxSuggestion.SelectedItem.ToString();
                    vm.VM_QueryInput = listBoxSuggestion.SelectedItem.ToString();
                    listBoxSuggestion.Visibility = Visibility.Hidden;
                }
            }
        }

        private void queriesFile_Browser(object sender, RoutedEventArgs e)
        {
            /*
            if (isDictionaryLoaded == true && !string.IsNullOrEmpty(Searcher.pathForResult))
            {
                System.Windows.Forms.OpenFileDialog queriesFile = new System.Windows.Forms.OpenFileDialog();

                queriesFile.ShowDialog();

                //just for testing
                // vm.VMsearchQuery
                
                if (!string.IsNullOrEmpty(queriesFile.FileName))
                {
                    vm.openQueryFile(queriesFile.FileName);
                    System.Windows.Forms.MessageBox.Show("Process ended.\nResult File is in your chosen folder.");
                    Searcher.languageChosen.Clear();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Please Choose A Valid Queries File.");
                }    
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please load dictionary,\nAnd/Or choose a path for result file.");
            }

    */
        }

        private void pathForResult_Browser(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog Dialog = new FolderBrowserDialog();
            Dialog.ShowDialog();
       //     Searcher.pathForResult = Dialog.SelectedPath;
            resultFolder_Text.Text = Dialog.SelectedPath;
        }

        private void QueryInputTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }
    }
}
