using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    /// Logika interakcji dla klasy EDMoreDimentions.xaml
    /// </summary>
    public partial class EDMoreDimentions : Window
    {
        MainWindow mainWindow = null;
        List<List<double>> axesValues = null;
        List<List<int>> vectors = null;
        List<List<W>> axisValues = null;
        int steps = 0;
        int lines = 0;
        string openedFileName = "";
        public EDMoreDimentions(string fileName)
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
            this.openedFileName = fileName;
        }

        private void ClassifyAll_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int value = random.Next(0, 10000);
            string newFileName = "Modified_" + openedFileName + "_" + DateTime.Now.Date.Year + "_" + DateTime.Now.Date.Month + "_" + DateTime.Now.Date.Day + "_" + value + ".txt";
            StreamWriter saveFile = new StreamWriter(newFileName);

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


            List<Element> elements = new List<Element>();
            for (int i = 0; i < mainWindow.gridData.Columns.Count - 1; i++)
            {
                //var values = (from row in mainWindow.gridData.AsEnumerable()
                //              select Convert.ToDouble(ConvertString(row.Field<string>(mainWindow.gridData.Columns[i].ColumnName)), format)).OrderBy(x=>x).ToList();

                for (int k = 0; k < mainWindow.gridData.Rows.Count; k++)
                {
                    elements.Add(new Element(mainWindow.gridData.Rows[k][mainWindow.gridData.Columns.Count - 1].ToString(), Convert.ToDouble(ConvertString(mainWindow.gridData.Rows[k][i].ToString()), format), k));
                }

                sortedRowsByColumns.Add(new List<Element>(elements.OrderBy(x => x.value).ToList()));
                elements.Clear();
            }
            int test = 0;
            int correctStartIndex = 0;
            int correctEndIndex = 0;
            int start = 0, end = 0;

            vectors = CreateList<int>(mainWindow.gridData.Rows.Count);
            int[,] coordinates = new int[mainWindow.gridData.Columns.Count - 1, 2];
            List<List<double>> axesValues = CreateList<double>(mainWindow.gridData.Columns.Count - 1);
            string currentClassFromStart = "";
            string currentClassFromEnd = "";
            bool classAssignedStart = false;
            bool classAssignedEnd = false;
            int countIterationsOfNothing = 0;
            List<string> classesOfElementsInZeroVectors = new List<string>();
            axisValues = CreateList<W>(mainWindow.gridData.Columns.Count - 1);
            while (workAlgorithm)
            {
                test++;
                for (int i = 0; i < sortedRowsByColumns.Count; i++)
                {
                    correctStartIndex = start = coordinates[i, 0];
                    correctEndIndex = end = coordinates[i, 1];
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
                                && vectors[sortedRowsByColumns[i][end].originalTableIndex].Count > 0)
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
                        foreach (var vector in vectors)
                        {
                            if (!vector.Contains(1))
                            {
                                string elementClass = sortedRowsByColumns[i].Where(x => x.originalTableIndex == vectors.IndexOf(vector)).First().className;
                                if (!classesOfElementsInZeroVectors.Contains(elementClass))
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

            for (int i = 0; i < axesValues.Count; i++)
            {
                for (int j = 0; j < axesValues[i].Count; j++)
                {
                    lines++;
                }
            }

            for (int i = 0; i < vectors.Count; i++)
            {
                for(int j=0; j < vectors[i].Count; j++)
                {
                   saveFile.Write(vectors[i][j] + " ");
                    if(j== vectors[i].Count-1)
                        saveFile.Write(mainWindow.gridData.Rows[i][mainWindow.gridData.Columns.Count - 1].ToString());
                }
                saveFile.WriteLine();
            }

            saveFile.Close();
            button.IsEnabled = true;
            MessageBox.Show("Liczba cięć: "+ lines +". Plik zapisano pomyślnie");
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<int> vector = new List<int>();
            List<double> newObject = textBox.Text.Split(',').Select(Double.Parse).ToList();
            bool classify = true;
            int count = 0;
            int rowNumber = 0;
            int itemNumber = 0;

            for(int i=0;i< axisValues.Count; i++)
            {
                for(int j = 0; j < axisValues[i].Count; j++)
                {
                    count++;
                }
            }
            while (classify)
            {
                for(int i = 0; i < axisValues.Count; i++)
                {
                    try
                    {
                        if (axisValues[i][rowNumber].Value > newObject[i])
                        {
                            if (axisValues[i][rowNumber].Direction == "left")
                            {
                                vector.Add(1);
                            }
                            else
                            {
                                vector.Add(0);
                            }
                        }
                        else
                        {
                            if (axisValues[i][rowNumber].Direction == "left")
                            {
                                vector.Add(0);
                            }
                            else
                            {
                                vector.Add(1);
                            }
                        }
                    }catch(Exception exp) { }
                    itemNumber++;
                }
                rowNumber++;
                if (itemNumber >= count) classify = false;
            }

            foreach (var d in vector)
            {
                Console.WriteLine(d);
            }

            bool found = false;
            for(int i=0; i< vectors.Count; i++)
            {
                found = true;
                for(int j=0; j < vectors[i].Count; j++)
                {
                    if(vectors[i][j] != vector[j])
                    {
                        found = false;
                        break;    
                    }
                }
                if (found)
                {
                    MessageBox.Show("Należy do klasy: " + mainWindow.gridData.Rows[i][mainWindow.gridData.Columns.Count - 1].ToString());
                    break;
                }
                
            }

            if (!found)
            {
                MessageBox.Show("Obiekt udało się sklasyfikować :(");
            }


            //for (int i = 0; i < newObject.Count(); i++)
            //{
            //    for(int j = 0; j < axisValues.Count; j++)
            //    {
            //        if (i == j)
            //        {
            //            for(int k = 0; k < axisValues[i].Count; k++)
            //            {
            //                if(axisValues[i][k].Value < newObject[i])
            //                {
            //                    if (axisValues[i][k].Direction == "left")
            //                    {
            //                        vector.Add(1);
            //                    }
            //                    else
            //                    {
            //                        vector.Add(0);
            //                    }
            //                }
            //                else
            //                {
            //                    if (axisValues[i][k].Direction == "left")
            //                    {
            //                        vector.Add(0);
            //                    }
            //                    else
            //                    {
            //                        vector.Add(1);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }
    }
}
