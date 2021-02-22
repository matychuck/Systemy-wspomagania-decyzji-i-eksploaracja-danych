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
using Accord.Statistics;
using Accord.Math;

namespace SWD
{
    /// <summary>
    /// Logika interakcji dla klasy KNNMethod.xaml
    /// </summary>
    public partial class KNNMethod : Window
    {
        private List<string> valuesHeaders = null;
        private List<string> metrics = null;
        public KNNMethod(List<string> valuesHeaders)
        {
            InitializeComponent();
            this.valuesHeaders = valuesHeaders;
            metrics = new List<string>()
            {
                "metryka euklidesowa",
                "metryka Manhattan",
                "metryka Czebyszewa",
                "metryka Mahalanobisa"
            };

            metricsComboBox.ItemsSource = metrics;

            int left = 0;
            int leftTextboxMargin = 5;
            foreach(var value in valuesHeaders)
            {
                Label valueLabel = new Label()
                {
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                    Content = value,
                    Margin = new Thickness(left, 0, 0, 0)
                };
                TextBox valueTextbox = new TextBox()
                {
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                    Margin = new Thickness(leftTextboxMargin, 30, 0, 0),
                    Width = 130,
                    Height = 20,
                    Name = "_new" + value
                };
                newValuesGrid.Children.Add(valueLabel);
                newValuesGrid.Children.Add(valueTextbox);
                left += 140;
                leftTextboxMargin += 140;
            }

            //Button classifyButton = new Button()
            //{
            //    Width = 70,
            //    Height = 30,
            //    Content = "Klasyfikuj",
            //    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
            //    VerticalAlignment = System.Windows.VerticalAlignment.Top,
            //    Margin = new Thickness(leftTextboxMargin + 30, 20, 0, 0)
            //};
            //newValuesGrid.Children.Add(classifyButton);
        }

        private Dictionary<int, double> euclidesMethod(List<double> values)
        {
            double sum = 0;

            Dictionary<int,double> distances = new Dictionary<int, double>();
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            for(int i=0; i < mainWindow.gridData.Rows.Count; i++)
            {
                for(int j=0; j < mainWindow.gridData.Columns.Count-1; j++)
                {
                    var value = mainWindow.gridData.Rows[i][j].ToString();
                    if (value.StartsWith(",") || value.StartsWith("."))
                    {
                        value = "0" + value;
                    }
                    sum += Math.Pow(Convert.ToDouble(value) - values[j],2);
                }
                distances.Add(i, Math.Sqrt(sum));
                sum = 0;
            }

            return distances;
        }

        private Dictionary<int, double> manhattanMethod(List<double> values)
        {
            double sum = 0;

            Dictionary<int, double> distances = new Dictionary<int, double>();
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            for (int i = 0; i < mainWindow.gridData.Rows.Count; i++)
            {
                for (int j = 0; j < mainWindow.gridData.Columns.Count-1; j++)
                {
                    var value = mainWindow.gridData.Rows[i][j].ToString();
                    if (value.StartsWith(",") || value.StartsWith("."))
                    {
                        value = "0" + value;
                    }
                    sum += Math.Abs(Convert.ToDouble(value) - values[j]);
                }
                distances.Add(i, sum);
                sum = 0;
            }

            return distances;
        }

        private Dictionary<int, double> czebyszewMethod(List<double> values)
        {
            double sum = 0;

            Dictionary<int, double> distances = new Dictionary<int, double>();
            List<double> vectors = new List<double>();
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            for (int i = 0; i < mainWindow.gridData.Rows.Count; i++)
            {
                for (int j = 0; j < mainWindow.gridData.Columns.Count-1; j++)
                {
                    var value = mainWindow.gridData.Rows[i][j].ToString();
                    if (value.StartsWith(",") || value.StartsWith("."))
                    {
                        value = "0" + value;
                    }
                    vectors.Add(Math.Abs(Convert.ToDouble(value) - values[j]));
                }
                distances.Add(i, vectors.Max());
                vectors.Clear();
                sum = 0;
            }

            return distances;
        }

        private Dictionary<int, double> mahalanobisMethod(List<double> values)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            int i = 0;
            Dictionary<int, double> distances = new Dictionary<int, double>();

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
                    table[i, j] = Convert.ToDouble(value);
                }

                i++;
              }

            double[] d = new double[mainWindow.gridData.Columns.Count - 1];
            double[,] diff = new double[1, mainWindow.gridData.Columns.Count - 1];
            double[,] diffT = null;
            double[,] covarMatrix = null;
            double[,] inverseCovarMatrix = null;
            double[,] tmp = null;
            double[,] squaredDist = null;

            for (int k = 0; k < mainWindow.gridData.Rows.Count; k++)
            {
                for (int j = 0; j < mainWindow.gridData.Columns.Count - 1; j++)
                {
                    var value = mainWindow.gridData.Rows[k][j].ToString();
                    if (value.StartsWith(",") || value.StartsWith("."))
                    {
                        value = "0" + value;
                    }
                    d[j] = Convert.ToDouble(value) - values[j];
                }

                for (int j = 0; j < mainWindow.gridData.Columns.Count - 1; j++)
                {
                    diff[0, j] = d[j];
                }
                diffT = Transpose(diff);
                covarMatrix = table.Covariance();
                inverseCovarMatrix = covarMatrix.Inverse();
                tmp = MultiplyMatrix(diff, inverseCovarMatrix);
                squaredDist = MultiplyMatrix(tmp, diffT);
                double result = Math.Sqrt(squaredDist[0, 0]);
                distances.Add(k, result);
            }

            return distances;
        }

        private void ClassifyClick(object sender, RoutedEventArgs e)
        {
            int choice = -1;
            bool randomized = false;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            List<double> newObjectValues = new List<double>();
            foreach(var child in newValuesGrid.Children)
            {
                if(child is TextBox)
                {
                    newObjectValues.Add(double.Parse(((TextBox)child).Text, CultureInfo.InvariantCulture));
                }
            }
            
            Dictionary<int, double> distances = null;

            switch (metricsComboBox.SelectedItem.ToString())
            {
                case "metryka euklidesowa": distances = euclidesMethod(newObjectValues); break;
                case "metryka Manhattan": distances = manhattanMethod(newObjectValues); break;
                case "metryka Czebyszewa": distances = czebyszewMethod(newObjectValues); break;
                case "metryka Mahalanobisa": distances = mahalanobisMethod(newObjectValues); break;
                default: break;
            }
            foreach(var tmp in distances)
            {
                Console.WriteLine(tmp.Key + " " + tmp.Value);
            }
            

            int k = Convert.ToInt32(kNumber.Text);

            var orderedDistances = distances.OrderBy(key => key.Value); //odległości posortowane od najmniejszej do największej raz z indexem obiektu
            Dictionary<string, int> decisionClasses = new Dictionary<string, int>();
            foreach(KeyValuePair<int, double> distance in orderedDistances.Take(k))
            {
                var decisionClass = mainWindow.gridData.Rows[distance.Key][mainWindow.gridData.Columns.Count-1].ToString();

                if (decisionClasses.ContainsKey(decisionClass)) decisionClasses[decisionClass] += 1;
                else decisionClasses.Add(decisionClass,1);             
            }

             var maxValue = decisionClasses.Values.Max();

            List<string> results = new List<string>();

            foreach (var decision in decisionClasses)
            {
                if(decision.Value == maxValue)
                {
                    results.Add(decision.Key);
                }
            }

            if(results.Count > 1)
            {
                if (k + 1 >= (orderedDistances.Count()))
                {
                    Random rnd = new Random();
                    choice = rnd.Next(0, results.Count);
                    randomized = true;
                }
                else
                {
                    results = addNewNeighbour(k + 1, decisionClasses, orderedDistances);
                }
            }

            if (randomized)
            {
                results =  new List<string>() { results[choice] };
                randomized = false;
                choice = -1;
            }

            if (results.Count == 1)
            {
                var newRow = mainWindow.gridData.NewRow();

                for (var c = 0; c < mainWindow.gridData.Columns.Count-1; c++)
                {
                    var value = newObjectValues[c];
                    newRow[c] = value;
                }

                newRow[mainWindow.gridData.Columns.Count - 1] = results[0];
                mainWindow.gridData.Rows.Add(newRow);
                MessageBox.Show("Obiekt należy do klasy " + results[0] + ". Pomyślnie dodano do tabeli");
            }
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


        private List<string> addNewNeighbour(int k, Dictionary<string,int> decisionClasses, IOrderedEnumerable<KeyValuePair<int,double>> orderedDistances)
        {
            int choice = -1;
            Console.WriteLine("Rekur");
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
                if (k + 1 > (orderedDistances.Count()))
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
    }
}
