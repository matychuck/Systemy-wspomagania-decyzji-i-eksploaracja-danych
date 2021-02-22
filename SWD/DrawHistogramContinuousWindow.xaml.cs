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
    /// Logika interakcji dla klasy DrawHistogramContinuousWindow.xaml
    /// </summary>
    public partial class DrawHistogramContinuousWindow : Window
    {
        public List<string> headers = null;
        HistogramViewModel viewModel = new HistogramViewModel();
        public DrawHistogramContinuousWindow(List<string> headers)
        {
            InitializeComponent();
            this.headers = headers;
            comboBox.ItemsSource = headers;


        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int partitionsNumber = 0;
                if (int.TryParse(partitions.Text, out partitionsNumber))
                {
                    if (partitionsNumber > 0)
                    {
                        var mainWindow = (MainWindow)Application.Current.MainWindow;
                        string columnName = mainWindow.gridData.Columns[comboBox.SelectedIndex].ColumnName;

                        var values = (from row in mainWindow.gridData.AsEnumerable()
                                      select Convert.ToDouble(row.Field<string>(columnName))).ToList();

                        double min = values.Min(); // wartość minimalna
                        double max = values.Max(); // wartość maksymalna
                        double step = (max - min) / partitionsNumber; // krok
                        List<double> discretizePartitions = new List<double>();
                        discretizePartitions.Add(min);


                        // lista discretizePartitions zawiera konce przedziałów
                        for (int i = 0; i < partitionsNumber - 1; i++)
                        {
                            discretizePartitions.Add(discretizePartitions[i] + step);
                        }
                        discretizePartitions.Add(max);

                        for (int i = 0; i < discretizePartitions.Count(); i++)
                        {
                            Console.WriteLine(discretizePartitions[i].ToString());
                        }

                        SortedDictionary<double, int> histogram = new SortedDictionary<double, int>();
                        foreach (var value in values)
                        {
                            for (int i = 1; i < discretizePartitions.Count(); i++)
                            {
                                if (value <= discretizePartitions[i]) // przedział prawostronnie domknięty
                                {
                                    if (histogram.ContainsKey(discretizePartitions[i])) histogram[discretizePartitions[i]] += 1;
                                    else histogram.Add(discretizePartitions[i], 1);
                                    break;
                                }
                            }
                        }
                        for (int i = 1; i < discretizePartitions.Count(); i++)
                        {
                            if (!histogram.ContainsKey(discretizePartitions[i])) histogram.Add(discretizePartitions[i], 0);
                        }

                        Collection<Item> Items = new Collection<Item>();

                        bool firstElement = true;
                        string previousKey = "";
                        foreach (var value in histogram)
                        {

                            if (firstElement)
                            {
                                Items.Add(new Item { Label = "(" + min + "-" + value.Key.ToString() + ">", Value = value.Value });
                                previousKey = value.Key.ToString();
                                firstElement = false;
                            }
                            else
                            {
                                Items.Add(new Item { Label = "(" + previousKey + "-" + value.Key.ToString() + ">", Value = value.Value });
                                previousKey = value.Key.ToString();
                            }


                        }
                        viewModel.ChangeViewModel(Items, columnName);
                        this.DataContext = null;
                        this.DataContext = viewModel;
                        plot.InvalidatePlot(true);
                    }
                }
                else
                {
                    MessageBox.Show("Proszę wybrać poprawną ilość przedziałów");
                }
            }catch(Exception exp)
            {
                MessageBox.Show("Nie wybrano zmiennej ciągłej");
            }
        }
    }
}
