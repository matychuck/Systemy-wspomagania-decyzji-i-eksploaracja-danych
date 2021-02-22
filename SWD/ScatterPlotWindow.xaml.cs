using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
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
    /// Logika interakcji dla klasy ScatterPlotWindow.xaml
    /// </summary>
    public partial class ScatterPlotWindow : Window
    {
        ScatterPlotViewModel viewModel = new ScatterPlotViewModel();
        public ScatterPlotWindow(List<string> valuesHeaders, List<string> stringHeaders, List<string> allHeaders = null)
        {
            InitializeComponent();
            comboBoxX.ItemsSource = valuesHeaders;
            comboBoxY.ItemsSource = valuesHeaders;
            comboBoxC.ItemsSource = allHeaders;
            //comboBoxC.ItemsSource = stringHeaders;
        }

        //private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        int partitionsNumber = 0;
        //        if (int.TryParse(partitions.Text, out partitionsNumber))
        //        {
        //            if (partitionsNumber > 0)
        //            {
        //                var mainWindow = (MainWindow)Application.Current.MainWindow;
        //                string columnName = mainWindow.gridData.Columns[comboBox.SelectedIndex].ColumnName;

        //                var values = (from row in mainWindow.gridData.AsEnumerable()
        //                              select Convert.ToDouble(row.Field<string>(columnName))).ToList();

        //                double min = values.Min(); // wartość minimalna
        //                double max = values.Max(); // wartość maksymalna
        //                double step = (max - min) / partitionsNumber; // krok
        //                List<double> discretizePartitions = new List<double>();
        //                discretizePartitions.Add(min);


        //                // lista discretizePartitions zawiera konce przedziałów
        //                for (int i = 0; i < partitionsNumber - 1; i++)
        //                {
        //                    discretizePartitions.Add(discretizePartitions[i] + step);
        //                }
        //                discretizePartitions.Add(max);

        //                for (int i = 0; i < discretizePartitions.Count(); i++)
        //                {
        //                    Console.WriteLine(discretizePartitions[i].ToString());
        //                }

        //                SortedDictionary<double, int> histogram = new SortedDictionary<double, int>();
        //                foreach (var value in values)
        //                {
        //                    for (int i = 1; i < discretizePartitions.Count(); i++)
        //                    {
        //                        if (value <= discretizePartitions[i]) // przedział prawostronnie domknięty
        //                        {
        //                            if (histogram.ContainsKey(discretizePartitions[i])) histogram[discretizePartitions[i]] += 1;
        //                            else histogram.Add(discretizePartitions[i], 1);
        //                            break;
        //                        }
        //                    }
        //                }
        //                for (int i = 1; i < discretizePartitions.Count(); i++)
        //                {
        //                    if (!histogram.ContainsKey(discretizePartitions[i])) histogram.Add(discretizePartitions[i], 0);
        //                }

        //                Collection<Item> Items = new Collection<Item>();

        //                bool firstElement = true;
        //                string previousKey = "";
        //                foreach (var value in histogram)
        //                {

        //                    if (firstElement)
        //                    {
        //                        Items.Add(new Item { Label = "(" + min + "-" + value.Key.ToString() + ">", Value = value.Value });
        //                        previousKey = value.Key.ToString();
        //                        firstElement = false;
        //                    }
        //                    else
        //                    {
        //                        Items.Add(new Item { Label = "(" + previousKey + "-" + value.Key.ToString() + ">", Value = value.Value });
        //                        previousKey = value.Key.ToString();
        //                    }


        //                }
        //                viewModel.ChangeViewModel(Items, columnName);
        //                this.DataContext = null;
        //                this.DataContext = viewModel;
        //                plot.InvalidatePlot(true);
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("Proszę wybrać poprawną ilość przedziałów");
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        MessageBox.Show("Nie wybrano zmiennej ciągłej");
        //    }
        //}

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            List<int> usedColorIndexes = new List<int>();
            List<double> colorsValues = new List<double>()
            {
                0,0.2,0.4,0.6,0.8,1.0,1.2,1.4,1.6,1.8
            };

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            string columnNameX = comboBoxX.SelectedItem.ToString();
            string columnNameY = comboBoxY.SelectedItem.ToString();
            string columnNameClass = comboBoxC.SelectedItem.ToString();
            var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };

            var values = (from row in mainWindow.gridData.AsEnumerable()
                          select row.Field<string>(columnNameClass)).Distinct().ToList();

            Dictionary<string, double> parentClass = new Dictionary<string, double>();

            if (ColorsTable.getColorsByClassName(columnNameClass) == null && (theSameColors.IsChecked==false || ColorsTable.colors.Count == 0))
            {
                

                Dictionary<string, double> children = new Dictionary<string, double>();
                foreach (var value in values)
                {
                    //children.Add(value, random.NextDouble() * (2 * Math.PI - 0) + 0);
                    children.Add(value, drawColor(usedColorIndexes,colorsValues));
                }

                ClassWithChildren parent = new ClassWithChildren(columnNameClass, children);
                ColorsTable.colors.Add(parent);
            }


            if (theSameColors.IsChecked == true)
            {
                parentClass = ColorsTable.getColorsByClassName(ColorsTable.colors[0].name);
            }
            else
            {
                parentClass = ColorsTable.getColorsByClassName(columnNameClass);
            }
            

            foreach (DataRow row in mainWindow.gridData.Rows)
            {
                scatterSeries.Points.Add(new ScatterPoint(Convert.ToDouble(row[columnNameX]), Convert.ToDouble(row[columnNameY]), 4, parentClass[row[columnNameClass].ToString()]));
            }

            viewModel.ChangeViewModel(scatterSeries);
            Dictionary<double, SolidColorBrush> colorsLegend = new Dictionary<double, SolidColorBrush>()
            {
                {0,Brushes.Red },
                {0.2,Brushes.Yellow },
                {0.4,Brushes.LightGreen },
                {0.6,Brushes.Orange },
                {0.8,Brushes.Blue },
                {1.0,Brushes.DarkOliveGreen},
                {1.2,Brushes.Black },
                {1.4,Brushes.Violet },
                {1.6,Brushes.Brown },
                {1.8,Brushes.Cyan },
            };
            int top = 0;
            legendGrid.Children.Clear();
            foreach(var child in parentClass)
            {
                Rectangle rect = new Rectangle()
                {
                    Width = 10,
                    Height = 10,
                    Fill = colorsLegend[child.Value],
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                    Margin = new Thickness(0, top, 0, 0)
                };

                TextBlock textBlock = new TextBlock()
                {
                    Text = child.Key.ToString(),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                    Margin = new Thickness(15, top-3, 0, 0)
                };
                legendGrid.Children.Add(rect);
                legendGrid.Children.Add(textBlock);
                top += 20;
            }
            
            this.DataContext = null;
            this.DataContext = viewModel;
            plot.InvalidatePlot(true);

        }

        public double drawColor(List<int> usedColorIndexes, List<double> colorsValues)
        {
            Random random = new Random();
            var index = random.Next(0,10);
            bool guard = true;
            while (guard)
            {
                if (usedColorIndexes.Contains(index))
                {
                    index = random.Next(0, 10);
                }
                else
                {
                    guard = false;
                }
            }
            
            usedColorIndexes.Add(index);
            return colorsValues[index];
        }
    }
}

//5.9 ciemny czerwony, 1.87 jasny niebieski, 0.95 ciemniejszy niebieski, 3.65 żółty, 4.95 pomarańczowy, 2.79 jasny zielony, 2.25 ciemny zielony