using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using IR_Engine;
using WpfApplication2;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for Language.xaml
    /// </summary>
    public partial class Language : Window
    {
        public Language()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }



        private void l_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;


            string strselecteditems = listBox.SelectedItem.ToString();

            string[] split = strselecteditems.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

          //  Searcher.languageChosen.Add(split[1]);

          //  foreach (string s in Searcher.languageChosen)
          //      Console.WriteLine(s);


        }

        public string l { get; set; }
    }
}
