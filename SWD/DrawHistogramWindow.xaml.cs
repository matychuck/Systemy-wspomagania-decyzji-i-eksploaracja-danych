using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Logika interakcji dla klasy DrawHistogramWindow.xaml
    /// </summary>
    public partial class DrawHistogramWindow : Window
    {
        public List<string> headers = null;
        HistogramViewModel viewModel = new HistogramViewModel();
        public DrawHistogramWindow(List<string> headers)
        {
            InitializeComponent();
            this.headers = headers;
            comboBox.ItemsSource = headers;
            
            
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool doubleValues = false;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            string columnName = mainWindow.gridData.Columns[comboBox.SelectedIndex].ColumnName;

            var values = (from row in mainWindow.gridData.AsEnumerable()
                          select row.Field<string>(columnName)).ToList();

        
            double result;
            if (double.TryParse(values[0], out result))
            {
                doubleValues = true;
            }

            if (doubleValues)
            {
                SortedDictionary<double, int> histogram = new SortedDictionary<double, int>();
                foreach (var value in values)
                {
                    if (histogram.ContainsKey(Convert.ToDouble(value))) histogram[Convert.ToDouble(value)] += 1;
                    else histogram.Add(Convert.ToDouble(value), 1);
                }
                Collection<Item> Items = new Collection<Item>();
                foreach (var value in histogram)
                {
                    Items.Add(new Item { Label = value.Key.ToString(), Value = value.Value });
                }
                viewModel.ChangeViewModel(Items, columnName);
                this.DataContext = null;
                this.DataContext = viewModel;
                plot.InvalidatePlot(true);
            }
            else
            {
                SortedDictionary<string, int> histogram = new SortedDictionary<string, int>();
                foreach (var value in values)
                {
                    if (histogram.ContainsKey(value)) histogram[value] += 1;
                    else histogram.Add(value, 1);
                }
                Collection<Item> Items = new Collection<Item>();
                foreach (var value in histogram)
                {
                    Items.Add(new Item { Label = value.Key, Value = value.Value });
                }
                viewModel.ChangeViewModel(Items, columnName);
                this.DataContext = null;
                this.DataContext = viewModel;
                plot.InvalidatePlot(true);
            }
            
        }
    }
}
