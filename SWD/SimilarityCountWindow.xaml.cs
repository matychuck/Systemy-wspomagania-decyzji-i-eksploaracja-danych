using Accord.Math.Distances;
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
    /// Logika interakcji dla klasy SimilarityCountWindow.xaml
    /// </summary>
    public partial class SimilarityCountWindow : Window
    {
        MainWindow mainWindow = null;
        List<string> metrics = null;
        public SimilarityCountWindow(List<string> columns)
        {
            InitializeComponent();
            comboBoxCol1.ItemsSource = columns;
            comboBoxCol2.ItemsSource = columns;
            metrics = new List<string>()
            {
                "Korelacja Pearsona",
                "Cosinus",
                "Jaccard"
            };
            similarityMeasures.ItemsSource = metrics;
            mainWindow = (MainWindow)Application.Current.MainWindow;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";
            var valuesCol1 = (from row in mainWindow.gridData.AsEnumerable()
                          select Convert.ToDouble(ConvertString(row.Field<string>(mainWindow.gridData.Columns[comboBoxCol1.SelectedItem.ToString()].ColumnName)), format)).ToArray();

            var valuesCol2 = (from row in mainWindow.gridData.AsEnumerable()
                              select Convert.ToDouble(ConvertString(row.Field<string>(mainWindow.gridData.Columns[comboBoxCol2.SelectedItem.ToString()].ColumnName)), format)).ToArray();

            if (similarityMeasures.SelectedItem.ToString() == "Jaccard")
            {
                var jaccard = new Jaccard();
                textBlock.Text += "\nPodobieństwo miary Jaccard pomiędzy kolumnami " + comboBoxCol1.SelectedItem.ToString() + " i " + comboBoxCol2.SelectedItem.ToString() + " wynosi: " + jaccard.Similarity(valuesCol1, valuesCol2).ToString();
            }
            else if (similarityMeasures.SelectedItem.ToString() == "Korelacja Pearsona")
            {
                var pearsonCorrelation = new PearsonCorrelation();
                textBlock.Text += "\nPodobieństwo miary Korelacja Pearsona pomiędzy kolumnami " + comboBoxCol1.SelectedItem.ToString() + " i " + comboBoxCol2.SelectedItem.ToString() + " wynosi: " + pearsonCorrelation.Similarity(valuesCol1, valuesCol2).ToString();
            }
            else if (similarityMeasures.SelectedItem.ToString() == "Cosinus")
            {
                var cosine = new Cosine();
                textBlock.Text += "\nPodobieństwo miary Cosinus pomiędzy kolumnami " + comboBoxCol1.SelectedItem.ToString() + " i " + comboBoxCol2.SelectedItem.ToString() + " wynosi: " + cosine.Similarity(valuesCol1, valuesCol2).ToString();

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
    }
}
