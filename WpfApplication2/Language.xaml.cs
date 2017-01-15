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

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for Language.xaml
    /// </summary>
    public partial class Language : Window
    {
      //  string languageChosen;
        public Language()
        {
            InitializeComponent();
        }

        private void l_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comBox = sender as ComboBox;
           // languageChosen = comBox.Text;
           // languageChosen = comBox.Text;
           // languageChosen = comBox.SelectionBoxItemStringFormat;
           // languageChosen = comBox.SelectedValue.ToString();
            //  languageChosen = comBox.SelectedItem.ToString();
            // = comBox.SelectionBoxItem.ToString();
            //MessageBox.Show(languageChosen);
            //Console.WriteLine(languageChosen);
        }
    }
}
