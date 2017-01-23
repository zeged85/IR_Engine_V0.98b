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
using IR_Engine_TRD;
using WpfApplication2;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for Language.xaml
    /// </summary>
    public partial class Language : Window
    {
        static List<string> lanList = new List<string>();
        //  string languageChosen;
        public Language()
        {
            InitializeComponent();
        }

        private void l_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            //var checkBox = new CheckBox();
            List<string> ml = new List<string>();
            // ml.Add(listBox.SelectedItem.ToString());
            // }
            if (listBox.IsEnabled)
                ml = listBox.SelectedItems as List<string>;



            //  foreach (string s in ml)
            //   Console.WriteLine(s);           
        }

        public string l { get; set; }
    }
}
