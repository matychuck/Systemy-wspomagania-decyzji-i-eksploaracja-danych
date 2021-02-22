using Accord.Math;
using Accord.Math.Distances;
using Accord.Statistics;
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
    /// Logika interakcji dla klasy KAverageWindow.xaml
    /// </summary>
    public partial class KAverageWindow : Window
    {
        Mahalanobis mahalanobis;
        private List<string> metrics = null;
        DataTable copiedTable = null;
        MainWindow mainWindow = null;
        DataTable tableWithNewColumns = new DataTable();
        DataTable resultTable = new DataTable();
        List<string> distinctClasses = new List<string>();
        DataTable bestResults = null;
        double previousRatio = 100000000000;
        int previousK = 1;

        public KAverageWindow()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;

            metrics = new List<string>()
            {
                "metryka euklidesowa",
                "metryka Manhattan",
                "metryka Czebyszewa",
                "metryka Mahalanobisa"
            };

            metricsComboBox.ItemsSource = metrics;
            distinctClasses = (from row in mainWindow.gridData.AsEnumerable()
                            select row.Field<string>(mainWindow.gridData.Columns[mainWindow.gridData.Columns.Count-1].ColumnName)).Distinct().ToList();

            copiedTable = mainWindow.gridData.Copy();
            copiedTable.Columns.RemoveAt(copiedTable.Columns.Count - 1);

            for(int z = 0; z < copiedTable.Rows.Count; z++)
            {
                var newRow = tableWithNewColumns.NewRow();
                tableWithNewColumns.Rows.Add(newRow);
            }




            //var mainWindow = (MainWindow)Application.Current.MainWindow;
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";
            int i = 0;

            double[,] table = new double[mainWindow.gridData.Rows.Count, mainWindow.gridData.Columns.Count - 1];


            foreach (DataRow row in mainWindow.gridData.Rows)
            {
                for (int j = 0; j < mainWindow.gridData.Columns.Count - 1; j++)
                {
                    var value = mainWindow.gridData.Rows[i][j].ToString();
                    if (value.StartsWith(",") || value.StartsWith("."))
                    {
                        value = "0" + value;
                    }
                    table[i, j] = Convert.ToDouble(value, format);
                }

                i++;
            }

            double[] d = new double[mainWindow.gridData.Columns.Count - 1];
            double[,] diff = new double[1, mainWindow.gridData.Columns.Count - 1];
            double[,] covarMatrix = table.Covariance();
            double[,] inverseCovarMatrix = null;

            try
            {
                inverseCovarMatrix = covarMatrix.Inverse();
            }
            catch (Exception exp)
            {
                inverseCovarMatrix = covarMatrix.PseudoInverse();
            }

            mahalanobis = Mahalanobis.FromPrecisionMatrix(inverseCovarMatrix);




        }

        private void ClassifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox.IsChecked == true)
            {
                for(int i = 1; i < 10; i++)
                {
                    AlgorithmKAverage(i);
                }
                if (bestResults != null)
                {
                    MessageBox.Show((previousK).ToString());
                    dataGridView2.ItemsSource = null;
                    dataGridView2.ItemsSource = bestResults.DefaultView;
                }
            }
            else
            {
                AlgorithmKAverage();
            }
        }

        private double euclidesMethod(List<double> midValues, List<double> elementValues)
        {
            double sum = 0;
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";

            for (int i = 0; i < copiedTable.Columns.Count - 1; i++)
            {
                var valueI = midValues[i];
                var valueK = elementValues[i];
                sum += Math.Pow(valueI - valueK, 2);
            }

            return Math.Sqrt(sum);
        }

        private double manhattanMethod(List<double> midValues, List<double> elementValues)
        {
            double sum = 0;
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";

            for (int j = 0; j < copiedTable.Columns.Count - 1; j++)
            {
                var valueI = midValues[j];
                var valueK = elementValues[j];

                sum += Math.Abs(valueI - valueK);
            }

            return sum;
        }

        private double czebyszewMethod(List<double> midValues, List<double> elementValues)
        {
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";
            List<double> vectors = new List<double>();

            for (int j = 0; j < copiedTable.Columns.Count - 1; j++)
            {
                var valueI = midValues[j];
                var valueK = elementValues[j];
                vectors.Add(Math.Abs(valueI - valueK));
            } 

            return vectors.Max();
        }

        private double mahalanobisMethod(List<double> midValues, List<double> elementValues)
        {
            //var mainWindow = (MainWindow)Application.Current.MainWindow;
            //var format = new NumberFormatInfo();
            //format.NegativeSign = "-";
            //int i = 0;

            //double[,] table = new double[mainWindow.gridData.Rows.Count, mainWindow.gridData.Columns.Count - 1];


            //foreach (DataRow row in mainWindow.gridData.Rows)
            //{
            //    for (int j = 0; j < mainWindow.gridData.Columns.Count - 1; j++)
            //    {
            //        var value = mainWindow.gridData.Rows[i][j].ToString();
            //        if (value.StartsWith(",") || value.StartsWith("."))
            //        {
            //            value = "0" + value;
            //        }
            //        table[i, j] = Convert.ToDouble(value, format);
            //    }

            //    i++;
            //}

            //double[] d = new double[mainWindow.gridData.Columns.Count - 1];
            //double[,] diff = new double[1, mainWindow.gridData.Columns.Count - 1];
            //double[,] covarMatrix = table.Covariance();
            //double[,] inverseCovarMatrix = null;

            //try
            //{
            //    inverseCovarMatrix = covarMatrix.Inverse();
            //}
            //catch (Exception exp)
            //{
            //    inverseCovarMatrix = covarMatrix.PseudoInverse();
            //}

            //var mahalanobis = Mahalanobis.FromPrecisionMatrix(inverseCovarMatrix);

            double result = mahalanobis.Distance(midValues.ToArray(), elementValues.ToArray());

            if (Double.IsNaN(result))
            {
                return 0;
            }
            else
            {
                return result;
            }
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

        private void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < tableWithNewColumns.Columns.Count; i++)
            {
                mainWindow.gridData.Columns.Add(new DataColumn(tableWithNewColumns.Columns[i].ColumnName));
                mainWindow.headers.Add(tableWithNewColumns.Columns[i].ColumnName);

                for(int j=0;j<tableWithNewColumns.Rows.Count; j++)
                {
                    mainWindow.gridData.Rows[j][tableWithNewColumns.Columns[i].ColumnName] = tableWithNewColumns.Rows[j][i];
                }
            }

            mainWindow.dataGridView.ItemsSource = null;
            mainWindow.dataGridView.ItemsSource = mainWindow.gridData.DefaultView;
            mainWindow.columns += tableWithNewColumns.Columns.Count;
        }

        private int getStatistics(int index, string className, int[] kvalues)
        {
            int amount = 0;
            for(int i = 0; i < mainWindow.gridData.Rows.Count; i++)
            {
                if(mainWindow.gridData.Rows[i][mainWindow.gridData.Columns.Count-1].ToString() == className && kvalues[i] == index)
                {
                    amount++;
                }
            }
            return amount;
        }

        private void AlgorithmKAverage(int clasters = -1)
        {
            resultTable.Clear();
            resultTable = new DataTable();
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";
            format.NumberDecimalSeparator = ".";
            int k;
            if (clasters == -1)
            {
                k = int.Parse(kNumber.Text);
            }
            else
            {
                k = clasters;
            }
            
            string chosenMetric = metricsComboBox.SelectedItem.ToString();
            int[] kvalues = new int[copiedTable.Rows.Count]; //zmapowane wiersze
            double[] distances = new double[k];
            List<Middle> middlesList = new List<Middle>();
            DataTable temporaryTableForCurrentK = new DataTable();
            Random rand = new Random();
            try
            {
                resultTable.Columns.Add(new DataColumn("Klasa"));
            }
            catch (Exception exp) { }
            for (int i = 0; i < copiedTable.Columns.Count; i++)
            {
                temporaryTableForCurrentK.Columns.Add(new DataColumn(copiedTable.Columns[i].ColumnName));
            }


            for (int i = 0; i < k; i++)
            {
                List<double> midValues = new List<double>();
                for (int j = 0; j < copiedTable.Columns.Count; j++)
                {

                    double min = (from row in copiedTable.AsEnumerable()
                                  select Convert.ToDouble(ConvertString(row.Field<string>(copiedTable.Columns[j].ColumnName)), format)).Min();
                    double max = (from row in copiedTable.AsEnumerable()
                                  select Convert.ToDouble(ConvertString(row.Field<string>(copiedTable.Columns[j].ColumnName)), format)).Max();

                    midValues.Add(rand.NextDouble() * (max - min) + min);
                }
                middlesList.Add(new Middle(i, new List<double>(midValues)));
                try
                {
                    resultTable.Columns.Add(new DataColumn(i.ToString()));
                }
                catch (Exception exp) { }
            }
            bool changedMiddle = true;
            bool changedK = true;
            List<double> elementValues = new List<double>();
            int test = 0;
            while (changedMiddle || changedK)
            {
                test++;
                for (int i = 0; i < copiedTable.Rows.Count; i++)
                {
                    elementValues.Clear();
                    var row = copiedTable.Rows[i];
                    for (int v = 0; v < copiedTable.Columns.Count; v++)
                    {
                        elementValues.Add(Convert.ToDouble(ConvertString(row[v].ToString()), format));
                    }

                    for (int j = 0; j < k; j++)
                    {
                        switch (chosenMetric)
                        {
                            case "metryka euklidesowa": distances[j] = euclidesMethod(middlesList[j].values, elementValues); break;
                            case "metryka Manhattan": distances[j] = manhattanMethod(middlesList[j].values, elementValues); break;
                            case "metryka Czebyszewa": distances[j] = czebyszewMethod(middlesList[j].values, elementValues); break;
                            case "metryka Mahalanobisa": distances[j] = mahalanobisMethod(middlesList[j].values, elementValues); break;
                            default: break;
                        }
                    }
                    changedK = false;

                    if (kvalues[i] != distances.IndexOf(distances.Min()))
                    {
                        kvalues[i] = distances.IndexOf(distances.Min());
                        changedK = true;
                    }
                    distances.Clear();
                }
                changedMiddle = false;

                for (int i = 0; i < k; i++)
                {
                    var indexesOfTypeK = kvalues.Find(x => x == i);

                    if (indexesOfTypeK.Count() == 0)
                    {
                        Console.WriteLine("Dla k = " + i + " było pusto!");
                        continue;
                    }
                    for (int j = 0; j < indexesOfTypeK.Count(); j++)
                    {
                        temporaryTableForCurrentK.Rows.Add(copiedTable.Rows[indexesOfTypeK[j]].ItemArray);
                    }

                    for (int j = 0; j < temporaryTableForCurrentK.Columns.Count; j++)
                    {
                        var values = (from row in temporaryTableForCurrentK.AsEnumerable()
                                      select Convert.ToDouble(ConvertString(row.Field<string>(copiedTable.Columns[j].ColumnName)), format)).ToList();
                        var average = values.Average();
                        if (middlesList[i].values[j] != average)
                        {
                            middlesList[i].values[j] = average;
                            changedMiddle = true;
                        }
                    }

                    temporaryTableForCurrentK.Clear();
                }
            }

            try
            {
                tableWithNewColumns.Columns.Add(new DataColumn(chosenMetric));
            }catch(Exception exp) { }


            for (int i = 0; i < kvalues.Count(); i++)
            {

                tableWithNewColumns.Rows[i][tableWithNewColumns.Columns.Count - 1] = kvalues[i];
                Console.WriteLine(kvalues[i]);
            }
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++" + test);
            for (int i = 0; i < middlesList.Count; i++)
            {
                Console.WriteLine(middlesList[i].values[0] + " " + middlesList[i].values[1]);
            }

            for (int i = 0; i < distinctClasses.Count; i++)
            {
                var newRow = resultTable.NewRow();
                resultTable.Rows.Add(newRow);
                resultTable.Rows[i][0] = distinctClasses[i];
                for (int j = 1; j <= k; j++)
                {
                    resultTable.Rows[i][j] = getStatistics(j - 1, distinctClasses[i], kvalues);
                }
            }

            if(bestResults == null)
            {
                bestResults = resultTable.Copy();
                checkIfBetterRatio();
            }                
            else if (checkIfBetterRatio())
            {
                bestResults = resultTable.Copy();
                previousK = clasters;
            }

            // odświeżenie widoku tablicy
            dataGridView2.ItemsSource = null;
            dataGridView2.ItemsSource = resultTable.DefaultView;
        }

        private bool checkIfBetterRatio()
        {
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";
            format.NumberDecimalSeparator = ".";
            double ratio = 0;
            double sumRatio=0;
            bool validCluster = true;
            for(int i = 1; i < resultTable.Columns.Count; i++)
            {
                var clasterValues = (from row in resultTable.AsEnumerable()
                                     select Convert.ToDouble(ConvertString(row.Field<string>(resultTable.Columns[i].ColumnName)), format)).ToList();
                double maxValue = clasterValues.Max();
                double sum=0;
                foreach(var value in clasterValues) { sum += value; }
                //sum = sum - maxValue;
                if (validCluster)
                {
                    if (((sum - maxValue) / sum) * 100 < 30)
                    {
                        ratio = (sum / maxValue) * 100;
                        sumRatio += ratio;
                    }
                    else
                    {
                        validCluster = false;
                    }
                }
            }
            if (validCluster)
            {
                if (sumRatio < previousRatio)
                {
                    previousRatio = sumRatio;
                    return true;
                }
            }
            
            return false;
        }
    }
}
