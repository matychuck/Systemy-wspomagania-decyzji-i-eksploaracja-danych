using Accord.Math;
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
using Accord.Math.Distances;

namespace SWD
{
    /// <summary>
    /// Logika interakcji dla klasy LeaveOneOutWindow.xaml
    /// </summary>
    public partial class LeaveOneOutWindow : Window
    {
        private List<string> metrics = null;
        List<DataColumn> numericColumns = null;
        public LeaveOneOutWindow()
        {
            InitializeComponent();

            metrics = new List<string>()
            {
                "metryka euklidesowa",
                "metryka Manhattan",
                "metryka Czebyszewa",
                "metryka Mahalanobisa"
            };

            metricsComboBox.ItemsSource = metrics;
        }

        private void ClassifyClick(object sender, RoutedEventArgs e)
        {
            int sum = 0;
            int choice = -1;
            bool randomized = false;
            var mainWindow = (MainWindow)Application.Current.MainWindow;

            numericColumns = mainWindow.gridData.Columns.Cast<DataColumn>().Where(r => IsNumeric(r)).ToList();

            double[,] distances = null;
            int[] compatibility = new int[mainWindow.gridData.Rows.Count];
            double[] quality = new double[mainWindow.gridData.Rows.Count-1];

            switch (metricsComboBox.SelectedItem.ToString())
            {
                case "metryka euklidesowa": distances = euclidesMethod(); break;
                case "metryka Manhattan": distances = manhattanMethod(); break;
                case "metryka Czebyszewa": distances = czebyszewMethod(); break;
                case "metryka Mahalanobisa": distances = mahalanobisMethod(); break;
                default: break;
            }

            for (int i = 1; i < mainWindow.gridData.Rows.Count; i++)
            {
                for (int j = 0; j < mainWindow.gridData.Rows.Count; j++)
                {
                    Dictionary<int, double> distancesWithoutCurrentObject = new Dictionary<int, double>();
                    for(int q = 0; q < distances.GetColumn(j).Count(); q++)
                    {
                        if (q == j) continue;
                        else distancesWithoutCurrentObject.Add(q, distances.GetColumn(j)[q]);
                    }
                    var orderedDistances = distancesWithoutCurrentObject.OrderBy(key => key.Value).ToList();


                    Dictionary<string, int> decisionClasses = new Dictionary<string, int>();
                    foreach (KeyValuePair<int, double> distance in orderedDistances.Take(i))
                    {
                        var decisionClass = mainWindow.gridData.Rows[distance.Key][mainWindow.gridData.Columns.Count - 1].ToString();

                        if (decisionClasses.ContainsKey(decisionClass)) decisionClasses[decisionClass] += 1;
                        else decisionClasses.Add(decisionClass, 1);
                    }
                    var maxValue = decisionClasses.Values.Max();
                    List<string> results = new List<string>();

                    foreach (var decision in decisionClasses)
                    {
                        if (decision.Value == maxValue)
                        {
                            results.Add(decision.Key);
                        }
                    }

                    if (results.Count > 1)
                    {
                        if(i+1 >= (orderedDistances.Count()))
                        {
                            Random rnd = new Random();
                            choice = rnd.Next(0, results.Count);
                            randomized = true;
                        }
                        else
                        {
                            results = addNewNeighbour(i + 1, decisionClasses, orderedDistances);
                        }
                        
                    }
                    if (randomized)
                    {
                        var test = mainWindow.gridData.Rows[j][mainWindow.gridData.Columns.Count - 1].ToString();
                        if (mainWindow.gridData.Rows[j][mainWindow.gridData.Columns.Count - 1].ToString() == results[choice])
                        {
                            compatibility[j] = 1;
                        }
                        else
                        {
                            compatibility[j] = 0;
                        }
                    }
                    else
                    {
                        if (results.Count == 1)
                        {
                            if (mainWindow.gridData.Rows[j][mainWindow.gridData.Columns.Count - 1].ToString() == results[0])
                            {
                                compatibility[j] = 1;
                            }
                            else
                            {
                                compatibility[j] = 0;
                            }
                        }
                    }
                    randomized = false;
                    choice = -1;
                }
                for(int z = 0; z < compatibility.Length; z++)
                {
                    if (compatibility[z] == 1) sum++;
                }
                quality[i-1] = (double)((double)sum / (double)mainWindow.gridData.Rows.Count);
                sum = 0;
                compatibility.Clear();
            }

            LinearQualityChartViewModel linearQualityChartViewModel = new LinearQualityChartViewModel();
            linearQualityChartViewModel.ChangeViewModel(quality, metricsComboBox.SelectedItem.ToString());
            this.DataContext = null;
            this.DataContext = linearQualityChartViewModel;
            plot.InvalidatePlot(true);

            QualityTableWindow qualityTable = new QualityTableWindow(metricsComboBox.SelectedItem.ToString(), quality);
            qualityTable.Show();
            
        }

        private double[,] euclidesMethod()
        {
            double sum = 0;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            double[,] distances = new double[mainWindow.gridData.Rows.Count, mainWindow.gridData.Rows.Count];
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";

            for (int i = 0; i < mainWindow.gridData.Rows.Count; i++)
            {
                for (int k = 0; k < mainWindow.gridData.Rows.Count; k++)
                {
                    if (i == k)
                    {
                        distances[i, k] = 0;
                    }
                    else
                    {
                        for (int j = 0; j < mainWindow.gridData.Columns.Count-1; j++)
                        {
                            var valueI = mainWindow.gridData.Rows[i][j].ToString();
                            if (valueI.StartsWith(",") || valueI.StartsWith("."))
                            {
                                valueI = "0" + valueI;
                            }

                            var valueK = mainWindow.gridData.Rows[k][j].ToString();
                            if (valueK.StartsWith(",") || valueK.StartsWith("."))
                            {
                                valueK = "0" + valueK;
                            }
                            sum += Math.Pow(Convert.ToDouble(valueI,format) - Convert.ToDouble(valueK, format), 2);
                        }

                        distances[i,k] = Math.Sqrt(sum);
                        sum = 0;
                    }
                    
                }
                
            }

            return distances;
        }

        private double[,] manhattanMethod()
        {
            double sum = 0;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            double[,] distances = new double[mainWindow.gridData.Rows.Count, mainWindow.gridData.Rows.Count];
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";

            for (int i = 0; i < mainWindow.gridData.Rows.Count; i++)
            {
                for (int k = 0; k < mainWindow.gridData.Rows.Count; k++)
                {
                    if (i == k)
                    {
                        distances[i, k] = 0;
                    }
                    else
                    {
                        for (int j = 0; j < mainWindow.gridData.Columns.Count - 1; j++)
                        {
                            var valueI = mainWindow.gridData.Rows[i][j].ToString();
                            if (valueI.StartsWith(",") || valueI.StartsWith("."))
                            {
                                valueI = "0" + valueI;
                            }

                            var valueK = mainWindow.gridData.Rows[k][j].ToString();
                            if (valueK.StartsWith(",") || valueK.StartsWith("."))
                            {
                                valueK = "0" + valueK;
                            }
                            sum += Math.Abs(Convert.ToDouble(valueI, format) - Convert.ToDouble(valueK, format));
                        }

                        distances[i, k] = sum;
                        sum = 0;
                    }

                }

            }
            return distances;
        }

        private double[,] czebyszewMethod()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            double[,] distances = new double[mainWindow.gridData.Rows.Count, mainWindow.gridData.Rows.Count];
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";
            List<double> vectors = new List<double>();
            for (int i = 0; i < mainWindow.gridData.Rows.Count; i++)
            {
                for (int k = 0; k < mainWindow.gridData.Rows.Count; k++)
                {
                    if (i == k)
                    {
                        distances[i, k] = 0;
                    }
                    else
                    {
                        for (int j = 0; j < mainWindow.gridData.Columns.Count - 1; j++)
                        {
                            var valueI = mainWindow.gridData.Rows[i][j].ToString();
                            if (valueI.StartsWith(",") || valueI.StartsWith("."))
                            {
                                valueI = "0" + valueI;
                            }

                            var valueK = mainWindow.gridData.Rows[k][j].ToString();
                            if (valueK.StartsWith(",") || valueK.StartsWith("."))
                            {
                                valueK = "0" + valueK;
                            }
                            vectors.Add(Math.Abs(Convert.ToDouble(valueI, format) - Convert.ToDouble(valueK, format)));
                        }
                        distances[i, k] = vectors.Max();
                        vectors.Clear();
                    }

                }
            }
            return distances;
        }

        private double[,] mahalanobisMethod()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";
            int i = 0;
            double[,] distances = new double[mainWindow.gridData.Rows.Count, mainWindow.gridData.Rows.Count];

            double[,] table = new double[mainWindow.gridData.Rows.Count, mainWindow.gridData.Columns.Count - 1];
            //double[,] table = new double[5,3];
            //table[0, 0] = 64;
            //table[0, 1] = 580;
            //table[0, 2] = 29;
            //table[1, 0] = 66;
            //table[1, 1] = 570;
            //table[1, 2] = 33;
            //table[2, 0] = 68;
            //table[2, 1] = 590;
            //table[2, 2] = 37;
            //table[3, 0] = 69;
            //table[3, 1] = 660;
            //table[3, 2] = 46;
            //table[4, 0] = 73;
            //table[4, 1] = 600;
            //table[4, 2] = 55;

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
            double[,] diffT = null;
            double[,] covarMatrix = table.Covariance();
            double[,] inverseCovarMatrix = null;
            double[,] tmp = null;
            double[,] squaredDist = null;

            try
            {
                inverseCovarMatrix = covarMatrix.Inverse();
            }
            catch (Exception exp)
            {
                inverseCovarMatrix = covarMatrix.PseudoInverse();
            }

            var mahalanobis = Mahalanobis.FromPrecisionMatrix(inverseCovarMatrix);

            List<double> X = new List<double>();
            List<double> Y = new List<double>();

            for (int k = 0; k < mainWindow.gridData.Rows.Count; k++)
            {
                for (int n = 0; n < mainWindow.gridData.Rows.Count; n++)
                {
                    if (k == n)
                    {
                        distances[k, n] = 0;
                    }
                    else
                    {
                        X.Clear();
                        Y.Clear();
                        for (int j = 0; j < mainWindow.gridData.Columns.Count - 1; j++)
                        {
                            var valueK = mainWindow.gridData.Rows[k][j].ToString();
                            if (valueK.StartsWith(",") || valueK.StartsWith("."))
                            {
                                valueK = "0" + valueK;
                            }

                            var valueN = mainWindow.gridData.Rows[n][j].ToString();
                            if (valueN.StartsWith(",") || valueN.StartsWith("."))
                            {
                                valueN = "0" + valueN;
                            }
                            //d[j] = Convert.ToDouble(valueK, format) - Convert.ToDouble(valueN, format);
                            // diff[0, j] = d[j];
                            X.Add(Convert.ToDouble(valueK, format));
                            Y.Add(Convert.ToDouble(valueN, format));
                        }

                        //diffT = Transpose(diff);
                       // covarMatrix = table.Covariance();
                        //try
                        //{
                        //    inverseCovarMatrix = covarMatrix.Inverse();
                        //}
                        //catch(Exception exp)
                        //{
                        //    inverseCovarMatrix = covarMatrix.PseudoInverse();
                        //}
                        
                        //var mahalanobis = Mahalanobis.FromPrecisionMatrix(inverseCovarMatrix);
                        double result = mahalanobis.Distance(X.ToArray(), Y.ToArray());
                        //tmp = MultiplyMatrix(diff, inverseCovarMatrix);
                        //squaredDist = MultiplyMatrix(tmp, diffT);
                        //double result = Math.Sqrt(squaredDist[0, 0]);
                        if (Double.IsNaN(result))
                        {
                            distances[k, n] = 0;
                        }
                        else
                        {
                            distances[k, n] = result;
                        }

                    }
                }
            }

            return distances;
        }

        public double[,] MultiplyMatrix(double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);
            double temp = 0;
            double[,] kHasil = new double[rA, cB];
            if (cA != rB)
            {
                Console.WriteLine("matrices can't be multiplied !!");
                return null;
            }
            else
            {
                for (int i = 0; i < rA; i++)
                {
                    for (int j = 0; j < cB; j++)
                    {
                        temp = 0;
                        for (int k = 0; k < cA; k++)
                        {
                            temp += A[i, k] * B[k, j];
                        }
                        kHasil[i, j] = temp;
                    }
                }
                return kHasil;
            }
        }

        public double[,] Transpose(double[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            double[,] result = new double[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }


        private List<string> addNewNeighbour(int k, Dictionary<string, int> decisionClasses, List<KeyValuePair<int, double>> orderedDistances)
        {
            int choice = -1;

            //Console.WriteLine("Rekur");
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var newNeighbour = orderedDistances.ToList()[k-1];

            var decisionClass = mainWindow.gridData.Rows[newNeighbour.Key][mainWindow.gridData.Columns.Count - 1].ToString();

            if (decisionClasses.ContainsKey(decisionClass)) decisionClasses[decisionClass] += 1;
            else decisionClasses.Add(decisionClass, 1);

            var maxValue = decisionClasses.Values.Max();
            List<string> results = new List<string>();

            foreach (var decision in decisionClasses)
            {
                if (decision.Value == maxValue)
                {
                    results.Add(decision.Key);
                }
            }


            if (results.Count > 1)
            {
                if (k + 1 >= (orderedDistances.Count()))
                {
                    Random rnd = new Random();
                    choice = rnd.Next(0, results.Count);
                    return new List<string>() { results[choice] };
                }
                else
                {
                    return addNewNeighbour(k + 1, decisionClasses, orderedDistances);
                }
            }
            else
            {
                return results;
            }
        }

        bool IsNumeric(object o)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            double result_ignored;
            return o != null &&
              !(o is DBNull) &&
              double.TryParse(Convert.ToString(mainWindow.gridData.Rows[0][mainWindow.gridData.Columns.IndexOf((DataColumn)o)]), out result_ignored);
        }
    }
}
