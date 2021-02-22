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
    /// Logika interakcji dla klasy NormalizeWindow.xaml
    /// </summary>
    public partial class NormalizeWindow : Window
    {
        public string normalizedColumnName { get; set; }
        public bool all { get; set; }
        List<string> headers = null;

        public NormalizeWindow(List<string> headers)
        {
            InitializeComponent();
            this.headers = headers;
            comboBox.ItemsSource = headers;
            normalizedColumnName = "";
        }

        private void NormalizeButtonOk_Click(object sender, RoutedEventArgs e)
        {
            this.normalizedColumnName = comboBox.SelectedItem.ToString();
            this.Close();
        }

        private void NormalizeAllButton_Click(object sender, RoutedEventArgs e)
        {
            all = true;
            this.Close();
        }

        private void NormalizeButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
