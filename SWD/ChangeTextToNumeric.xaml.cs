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

namespace SWD
{
    /// <summary>
    /// Logika interakcji dla klasy ChangeTextToNumeric.xaml
    /// </summary>
    public partial class ChangeTextToNumeric : Window
    {
        public string selectedColumnName { get; set; }
        public MethodType methodType { get; set; }
       
        List<string> headers = null;
        public ChangeTextToNumeric(List<string> headers)
        {
            InitializeComponent();
            this.headers = headers;
            comboBox.ItemsSource = headers;
            selectedColumnName = "";
            methodType = MethodType.None;
        }

        private void ChangeButtonOk_Click(object sender, RoutedEventArgs e)
        {
            this.selectedColumnName = comboBox.SelectedItem.ToString();
            if (alphabetic.IsChecked == true) methodType = MethodType.Alphabetical;
            else if (orderOfAppearance.IsChecked == true) methodType = MethodType.InOrder;
            this.Close();
        }

        private void ChangeButtonChange_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public enum MethodType
        {
            Alphabetical = 0,
            InOrder,
            None
        }
    }
}
