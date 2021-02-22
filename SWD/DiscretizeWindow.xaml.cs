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
    /// Logika interakcji dla klasy DiscretizeWindow.xaml
    /// </summary>
    public partial class DiscretizeWindow : Window
    {
        public string discretizedColumnName { get; set; }
        public int partitionNumber { get; set; }
        List<string> headers = null;

        public DiscretizeWindow(List<string> headers)
        {
            InitializeComponent();
            this.headers = headers;
            comboBox.ItemsSource = headers;
            discretizedColumnName = "";
            partitionNumber = 0;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            partitionLabel.Content = (int)slider.Value;
            
        }

        private void DiscretizeButtonOk_Click(object sender, RoutedEventArgs e)
        {
            this.partitionNumber = (int)partitionLabel.Content;
            this.discretizedColumnName = comboBox.SelectedItem.ToString();
            this.Close();
        }
        private void DiscretizeButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
