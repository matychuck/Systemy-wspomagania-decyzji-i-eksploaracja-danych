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
    /// Logika interakcji dla klasy SelectPercentageWindow.xaml
    /// </summary>
    public partial class SelectPercentageWindow : Window
    {
        public string selectedColumnName { get; set; }
        public double chosenPercentage { get; set; }
        public ValuesType valuesType { get; set; }
        List<string> headers = null;

        public SelectPercentageWindow(List<string> headers)
        {
            InitializeComponent();
            this.headers = headers;
            comboBox.ItemsSource = headers;
            selectedColumnName = "";
            valuesType = ValuesType.None;
        }

        private void SelectPercentageButtonOk_Click(object sender, RoutedEventArgs e)
        {
            double percent;
            this.selectedColumnName = comboBox.SelectedItem.ToString();
            if (smallestValues.IsChecked == true) valuesType = ValuesType.Smallest;
            else if (biggestValues.IsChecked == true) valuesType = ValuesType.Biggest;
            
            if(double.TryParse(percentage.Text, out percent))
            {
                chosenPercentage = percent;
            }
            this.Close();
        }

        private void SelectPercentageButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public enum ValuesType
        {
            Smallest = 0,
            Biggest,
            None
        }
    }
}
