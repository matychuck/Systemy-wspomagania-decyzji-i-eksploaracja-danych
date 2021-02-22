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
    /// Logika interakcji dla klasy NewRangeWindow.xaml
    /// </summary>
    public partial class NewRangeWindow : Window
    {
        public string rangedColumnName { get; set; }
        public double newMin { get; set; }
        public double newMax { get; set; }

        List<string> headers = null;

        public NewRangeWindow(List<string> headers)
        {
            InitializeComponent();
            this.headers = headers;
            comboBox.ItemsSource = headers;
            rangedColumnName = "";
        }

        private void RangedButtonOk_Click(object sender, RoutedEventArgs e)
        {
            this.rangedColumnName = comboBox.SelectedItem.ToString();
            double newMinTmp, newMaxTmp;

            if(double.TryParse(newMinTxt.Text,out newMinTmp) && double.TryParse(newMaxTxt.Text, out newMaxTmp))
            {
                newMin = newMinTmp;
                newMax = newMaxTmp;
            }
            this.Close();
        }

        private void RangedButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
