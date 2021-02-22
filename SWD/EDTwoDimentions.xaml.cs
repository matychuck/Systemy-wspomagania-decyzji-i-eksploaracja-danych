using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
    /// Logika interakcji dla klasy EDTwoDimentions.xaml
    /// </summary>
    public partial class EDTwoDimentions : Window
    {
        ScatterPlotViewModel viewModel = new ScatterPlotViewModel();
        MainWindow mainWindow = null;
        List<List<double>> axesValues = null;
        int steps = 0;
        int lines = 0;
        public EDTwoDimentions(List<string> valuesHeaders, List<string> stringHeaders)
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
            this.DataContext = viewModel;
            comboBoxX.ItemsSource = valuesHeaders;
            comboBoxY.ItemsSource = valuesHeaders;
            comboBoxC.ItemsSource = stringHeaders;
        }

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

            if (ColorsTable.getColorsByClassName(columnNameClass) == null)
            {


                Dictionary<string, double> children = new Dictionary<string, double>();
                foreach (var value in values)
                {
                    //children.Add(value, random.NextDouble() * (2 * Math.PI - 0) + 0);
                    children.Add(value, drawColor(usedColorIndexes, colorsValues));
                }

                ClassWithChildren parent = new ClassWithChildren(columnNameClass, children);
                ColorsTable.colors.Add(parent);
            }


            parentClass = ColorsTable.getColorsByClassName(columnNameClass);

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
            foreach (var child in parentClass)
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
                    Margin = new Thickness(15, top - 3, 0, 0)
                };
                legendGrid.Children.Add(rect);
                legendGrid.Children.Add(textBlock);
                top += 20;
            }

            

            this.DataContext = null;
            this.DataContext = viewModel;
            plot.InvalidatePlot(true);
            drawAxesButton.IsEnabled = true;
        }

        public double drawColor(List<int> usedColorIndexes, List<double> colorsValues)
        {
            Random random = new Random();
            var index = random.Next(0, 10);
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

        private void DrawAxesButton_Click(object sender, RoutedEventArgs e)
        {
            var format = new NumberFormatInfo();
            bool workStart = true;
            bool workEnd = true;
            int countFromStart = 0;
            int countFromEnd = 0;
            bool workAlgorithm = true;
            format.NegativeSign = "-";
            format.NumberDecimalSeparator = ",";
            //klucz to nazwa kolumny, value to lista posortowanych wartośći w wierszach dla danej kolumny
            List<List<Element>> sortedRowsByColumns = new List<List<Element>>();
            //DataTable copiedTable = mainWindow.gridData.Copy();

            var ValuesX = (from row in mainWindow.gridData.AsEnumerable()
                                  select row.Field<string>(comboBoxX.SelectedItem.ToString())).ToList();

            var ValuesY = (from row in mainWindow.gridData.AsEnumerable()
                           select row.Field<string>(comboBoxY.SelectedItem.ToString())).ToList();

            List<List<string>> twoDimentionalArray = new List<List<string>>()
            {
                ValuesX,
                ValuesY
            };


            List<Element> elements = new List<Element>();
            for (int i = 0; i < twoDimentionalArray.Count; i++)
            {
                //var values = (from row in mainWindow.gridData.AsEnumerable()
                //              select Convert.ToDouble(ConvertString(row.Field<string>(mainWindow.gridData.Columns[i].ColumnName)), format)).OrderBy(x=>x).ToList();

                for (int k = 0; k < twoDimentionalArray[i].Count; k++)
                {
                    elements.Add(new Element(mainWindow.gridData.Rows[k][mainWindow.gridData.Columns.Count - 1].ToString(), Convert.ToDouble(ConvertString(twoDimentionalArray[i][k].ToString()), format), k));
                }

                sortedRowsByColumns.Add(new List<Element>(elements.OrderBy(x => x.value).ToList()));
                elements.Clear();
            }

            //List<Element> elements = new List<Element>();
            //for (int i = 0; i < mainWindow.gridData.Columns.Count - 1; i++)
            //{
            //    //var values = (from row in mainWindow.gridData.AsEnumerable()
            //    //              select Convert.ToDouble(ConvertString(row.Field<string>(mainWindow.gridData.Columns[i].ColumnName)), format)).OrderBy(x=>x).ToList();

            //    for (int k = 0; k < mainWindow.gridData.Rows.Count; k++)
            //    {
            //        elements.Add(new Element(mainWindow.gridData.Rows[k][mainWindow.gridData.Columns.Count - 1].ToString(), Convert.ToDouble(ConvertString(mainWindow.gridData.Rows[k][i].ToString()), format), k));
            //    }

            //    sortedRowsByColumns.Add(new List<Element>(elements.OrderBy(x => x.value).ToList()));
            //    elements.Clear();
            //}
            int test = 0;
            int correctStartIndex = 0;
            int correctEndIndex = 0;
            int start = 0, end = 0;
            List<List<int>> vectors = CreateList<int>(mainWindow.gridData.Rows.Count);
            int[,] coordinates = new int[twoDimentionalArray.Count, 2];
            axesValues = CreateList<double>(twoDimentionalArray.Count);
            //List<List<int>> vectors = CreateList<int>(mainWindow.gridData.Rows.Count);   
            //int[,] coordinates = new int[mainWindow.gridData.Columns.Count - 1, 2];
            //List<List<double>> axesValues = CreateList<double>(mainWindow.gridData.Columns.Count - 1);
            string currentClassFromStart = "";
            string currentClassFromEnd= "";
            bool classAssignedStart = false;
            bool classAssignedEnd = false;
            int countIterationsOfNothing = 0;
            List<string> classesOfElementsInZeroVectors = new List<string>();
            List<List<W>> axisValues = CreateList<W>(twoDimentionalArray.Count);
            while (workAlgorithm)
            {
                test++;
                for(int i=0; i < sortedRowsByColumns.Count; i++)
                {
                    correctStartIndex = start = coordinates[i,0];
                    correctEndIndex = end = coordinates[i,1];
                    if (end == 0)
                    {
                        coordinates[i, 1] = sortedRowsByColumns[i].Count - 1;
                        correctEndIndex = end = sortedRowsByColumns[i].Count - 1;
                    }

                    //
                    currentClassFromStart = "";
                    currentClassFromEnd = "";

                    while (workStart || workEnd)
                    {
                        if (workStart)
                        {
                            if (vectors[sortedRowsByColumns[i][start].originalTableIndex].Contains(1) 
                                && vectors[sortedRowsByColumns[i][start].originalTableIndex].Count > 0)
                            {
                                start++;
                            }
                            else
                            {
                                if (!classAssignedStart)
                                {
                                    currentClassFromStart = sortedRowsByColumns[i][start].className;
                                    classAssignedStart = true;
                                }
                                    

                                if (currentClassFromStart != sortedRowsByColumns[i][start].className)
                                {
                                    workStart = false;
                                }
                                else
                                {
                                    var listOfElementsWithTheSameValue = sortedRowsByColumns[i].Any(x => x.value == sortedRowsByColumns[i][start].value && x.className != currentClassFromStart && !vectors[x.originalTableIndex].Contains(1));
                                    if (listOfElementsWithTheSameValue)
                                    {
                                        workStart = false;
                                    }
                                    else
                                    {                                     
                                        start++;
                                        correctStartIndex = start;
                                        countFromStart++;
                                    }

                                }
                            }
                        }
                        if (workEnd)
                        {
                            if (vectors[sortedRowsByColumns[i][end].originalTableIndex].Contains(1) 
                                && vectors[sortedRowsByColumns[i][end].originalTableIndex].Count>0)
                            {
                                end--;
                            }
                            else
                            {
                                if (!classAssignedEnd)
                                {
                                    currentClassFromEnd = sortedRowsByColumns[i][end].className;
                                    classAssignedEnd = true;
                                }
                                
                                if (currentClassFromEnd != sortedRowsByColumns[i][end].className)
                                {
                                    workEnd = false;
                                }
                                else
                                {
                                    var listOfElementsWithTheSameValue = sortedRowsByColumns[i].Any(x => x.value == sortedRowsByColumns[i][end].value && x.className != currentClassFromEnd && !vectors[x.originalTableIndex].Contains(1));
                                    if (listOfElementsWithTheSameValue)
                                    {
                                        workEnd = false;
                                    }
                                    else
                                    {
                                        
                                        end--;
                                        correctEndIndex = end;
                                        countFromEnd++;
                                    }
                                }
                            }
                        }
                        if (end <= start)
                        {
                            workStart = false;
                            workEnd = false;
                            workAlgorithm = false;
                        }
                    }
                    if ((countFromEnd == countFromStart) && (currentClassFromEnd == currentClassFromStart))
                    {
                        foreach(var vector in vectors)
                        {
                            if (!vector.Contains(1))
                            {
                                string elementClass = sortedRowsByColumns[i].Where(x => x.originalTableIndex == vectors.IndexOf(vector)).First().className;
                                if(!classesOfElementsInZeroVectors.Contains(elementClass))
                                    classesOfElementsInZeroVectors.Add(elementClass);
                            }     
                        }

                        if (classesOfElementsInZeroVectors.Count == 1)
                        {
                            workAlgorithm = false;
                        }
                        else
                        {
                            classesOfElementsInZeroVectors.Clear();
                        }
                            
                    }
                   
                        if (countFromStart == 0 && countFromEnd == 0)
                        {
                            countIterationsOfNothing++;
                            if (countIterationsOfNothing == sortedRowsByColumns.Count)
                            {
                                workAlgorithm = false;
                            }
                        }
                        else
                        {
                            if (countFromStart > countFromEnd)
                            {
                                int expectedSize = 0;
                                for (int j = 0; j < start; j++)
                                {
                                    int index = sortedRowsByColumns[i][j].originalTableIndex;
                                    vectors[index].Add(1);
                                    expectedSize = vectors[index].Count;
                                }
                                foreach (var vec in vectors)
                                {
                                    if (vec.Count < expectedSize)
                                    {
                                        vec.Add(0);
                                    }
                                }
                                coordinates[i, 0] = correctStartIndex;
                                axesValues[i].Add(sortedRowsByColumns[i][correctStartIndex - 1].value);
                                axisValues[i].Add(new W("left", sortedRowsByColumns[i][correctStartIndex - 1].value));
                            }
                            else
                            {
                                int expectedSize = 0;
                                for (int j = sortedRowsByColumns[i].Count - 1; j > end; j--)
                                {
                                    int index = sortedRowsByColumns[i][j].originalTableIndex;
                                    vectors[index].Add(1);
                                    expectedSize = vectors[index].Count;
                                }
                                foreach (var vec in vectors)
                                {
                                    if (vec.Count < expectedSize)
                                    {
                                        vec.Add(0);
                                    }
                                }
                                coordinates[i, 1] = correctEndIndex;
                                axesValues[i].Add(sortedRowsByColumns[i][correctEndIndex + 1].value);
                                axisValues[i].Add(new W("right", sortedRowsByColumns[i][correctEndIndex + 1].value));
                            }
                        }
                        countFromEnd = 0;
                        countFromStart = 0;
                        workStart = true;
                        workEnd = true;
                        classAssignedEnd = false;
                        classAssignedStart = false;
                    
                    if (!workAlgorithm)
                    {
                        workStart = false;
                        workEnd = false;
                        break;
                    }
                        
                    //}  
                }
            }
            int y = 0;
            everythingButton.IsEnabled = true;
            stepButton.IsEnabled = true;

            for (int i = 0; i < axesValues.Count; i++)
            {
                for (int j = 0; j < axesValues[i].Count; j++)
                {
                    lines++;
                }
            }
                    //foreach( var vec in vectors)
                    //{
                    //    Console.Write("[");
                    //    for (int i = 0; i < vec.Count; i++)
                    //    {
                    //        Console.Write(vec[i] + ",");
                    //    }
                    //    Console.Write("]\n" + " " + sortedRowsByColumns[0].Where(x => x.originalTableIndex == y).First().value);
                    //    Console.WriteLine();
                    //    y++;
                    //}

                    //for(int i = 0; i < axesValues.Count; i++)
                    //{
                    //    for(int j=0; j < axesValues[i].Count; j++)
                    //    {
                    //        if (i == 0)
                    //        {
                    //            var annotation = new LineAnnotation();
                    //            annotation.Color = OxyColors.Blue;
                    //            annotation.MinimumY = 0;
                    //            annotation.MaximumY = 40;
                    //            annotation.X = axesValues[i][j];
                    //            annotation.LineStyle = LineStyle.Solid;
                    //            annotation.Type = LineAnnotationType.Vertical;
                    //            viewModel.MyModel.Annotations.Add(annotation);
                    //        }
                    //        if (i == 1)
                    //        {
                    //            var annotation = new LineAnnotation();
                    //            annotation.Color = OxyColors.Blue;
                    //            annotation.MinimumX = 0;
                    //            annotation.MaximumX = 40;
                    //            annotation.Y = axesValues[i][j];
                    //            annotation.LineStyle = LineStyle.Solid;
                    //            annotation.Type = LineAnnotationType.Horizontal;
                    //            viewModel.MyModel.Annotations.Add(annotation);
                    //        }
                    //        Console.Write(axesValues[i][j] + " ");
                    //    }
                    //    Console.WriteLine();
                    //}
                    //this.DataContext = null;
                    //this.DataContext = viewModel;
                    //plot.InvalidatePlot(true);
                }

        private string ConvertString(string value)
        {
            if (value == "0.0" || value == "0,0") return "0";
            else if (value.StartsWith(",") || value.StartsWith("."))
            {
                value = "0" + value;
                return value;
            }
            else return value;
        }

        private static List<List<T>> CreateList<T>(int capacity)
        {
            List<List<T>> list = new List<List<T>>();

            for (int i = 0; i < capacity; i++)
            {
                List<T> coll = new List<T>();
                list.Add(coll);
            }

            return list;
        }

        private void DrawEverything(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < axesValues.Count; i++)
            {
                for (int j = 0; j < axesValues[i].Count; j++)
                {
                    if (i == 0)
                    {
                        var annotation = new LineAnnotation();
                        annotation.Color = OxyColors.Blue;
                        annotation.MinimumY = -100;
                        annotation.MaximumY = 100;
                        annotation.X = axesValues[i][j];
                        annotation.LineStyle = LineStyle.Solid;
                        annotation.Type = LineAnnotationType.Vertical;
                        viewModel.MyModel.Annotations.Add(annotation);
                    }
                    if (i == 1)
                    {
                        var annotation = new LineAnnotation();
                        annotation.Color = OxyColors.Blue;
                        annotation.MinimumX = -100;
                        annotation.MaximumX = 100;
                        annotation.Y = axesValues[i][j];
                        annotation.LineStyle = LineStyle.Solid;
                        annotation.Type = LineAnnotationType.Horizontal;
                        viewModel.MyModel.Annotations.Add(annotation);
                    }
                    Console.Write(axesValues[i][j] + " ");
                }
                Console.WriteLine();
            }
            this.DataContext = null;
            this.DataContext = viewModel;
            plot.InvalidatePlot(true);
        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            if (steps % 2 == 0)
            {
                var annotation = new LineAnnotation();
                annotation.Color = OxyColors.Blue;
                annotation.MinimumY = 0;
                annotation.MaximumY = 40;
                annotation.X = axesValues[0][steps/2];
                annotation.LineStyle = LineStyle.Solid;
                annotation.Type = LineAnnotationType.Vertical;
                viewModel.MyModel.Annotations.Add(annotation);
            }
            if (steps % 2 == 1)
            {
                var annotation = new LineAnnotation();
                annotation.Color = OxyColors.Blue;
                annotation.MinimumX = 0;
                annotation.MaximumX = 40;
                annotation.Y = axesValues[1][steps/2];
                annotation.LineStyle = LineStyle.Solid;
                annotation.Type = LineAnnotationType.Horizontal;
                viewModel.MyModel.Annotations.Add(annotation);
            }

            steps++;
            if (steps >= lines)
            {
                stepButton.IsEnabled = false;
                steps = 0;
                lines = 0;
            }
            this.DataContext = null;
            this.DataContext = viewModel;
            plot.InvalidatePlot(true);
        }
    }
}
