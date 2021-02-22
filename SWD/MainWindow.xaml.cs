using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static SWD.ChangeTextToNumeric;
using static SWD.SelectPercentageWindow;
using Accord.Math.Distances;

namespace SWD
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string openedFileName = null;
        public int rows, columns = 0; // liczba wierszy (wraz z nagłówkiem) i kolumn po wczytaniu
        public List<string> headers = null; // nagłowki kolumn po wczytaniu
        public DataTable gridData; // dane z pliku bez nagłówka
        List<double> discretizePartitions = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int value = random.Next(0, 10000);
            string newFileName = "Modified_" + openedFileName + "_" + DateTime.Now.Date.Year + "_" + DateTime.Now.Date.Month + "_" + DateTime.Now.Date.Day + "_" + value + ".txt";
            StreamWriter saveFile =  new StreamWriter(newFileName);
            
            for (int i = 0; i < gridData.Columns.Count; i++)
            {
                if (i == 0)
                {
                    saveFile.Write(gridData.Columns[i].ToString());
                }
                else
                {
                    saveFile.Write(" " + gridData.Columns[i].ToString());
                }

                if (i == gridData.Columns.Count - 1)
                {
                    saveFile.WriteLine();
                }
            }

            for(int j = 0; j< gridData.Rows.Count; j++)
            {
                for (int k = 0; k < gridData.Columns.Count; k++)
                {
                    if (k == 0)
                    {
                        saveFile.Write(gridData.Rows[j][k].ToString());
                    }
                    else
                    {
                        saveFile.Write(" " + gridData.Rows[j][k].ToString());
                    }
                }
                saveFile.WriteLine();
            }

            saveFile.Close();

            MessageBox.Show("Plik zapisano pomyślnie");

        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            string filePath = null;
            string line = null; // obecnie wczytana linia
            string[] row = null; // obecnie wczytana linia podzielona spacjami

            // wybór pliku
            OpenFileDialog ChooseFileWindow = new OpenFileDialog();

            ChooseFileWindow.Filter = "Tekstowe|*.txt|CSV|*.csv*";
            ChooseFileWindow.DefaultExt = "*.txt";
            if (ChooseFileWindow.ShowDialog() == true)
            {
                filePath = ChooseFileWindow.FileName;
                openedFileName = ChooseFileWindow.SafeFileName;
            }
            else
            {
                return;
            }

            if (filePath != null)
            {
                StreamReader file = new StreamReader(filePath);
                gridData = new DataTable();
                rows = 0;
                double checkIfNumeric;
                // wczytywanie linia po lini
                while ((line = file.ReadLine()) != null)
                {
                    if (!line.StartsWith("#")) // pomijanie komentarzy
                    {
                        row = line.Split(' ','\t');
                        
                        columns = row.Count();

                        if (rows == 0)
                        {
                            if(double.TryParse(row[0], out checkIfNumeric))
                            {
                                headers = new List<string>();
                                for (int i = 0; i < columns; i++)
                                {
                                    gridData.Columns.Add(new DataColumn("Column"+(i+1)));
                                    headers.Add("Column" + (i + 1));
                                }
                                var newRow = gridData.NewRow();

                                for (var c = 0; c < columns; c++)
                                {
                                    var value = row[c];
                                    newRow[c] = value;
                                }

                                gridData.Rows.Add(newRow);
                            }
                            else
                            {
                                headers = new List<string>(row);
                                foreach (var title in headers)
                                {
                                    gridData.Columns.Add(new DataColumn(title));
                                }
                            }
                            
                        }

                        if (rows > 0)
                        {
                            var newRow = gridData.NewRow();

                            for (var c = 0; c < columns; c++)
                            {
                                var value = row[c];
                                newRow[c] = value;
                            }

                            gridData.Rows.Add(newRow);
                        }

                        rows++;
                    }

                }
                dataGridView.DataContext = gridData.DefaultView;
                changeButton.IsEnabled = true;
                discretizeButton.IsEnabled = true;
                normalizeButton.IsEnabled = true;
                rangedButton.IsEnabled = true;
                percentButton.IsEnabled = true;
                histogramButton.IsEnabled = true;
                chartsButton.IsEnabled = true;
                classificationButton.IsEnabled = true;
                EDButton.IsEnabled = true;
                kAverageButton.IsEnabled = true;
                decisionTreeButton.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Plik nie został wybrany");
            }
        }

        private void ChangeTextToNumeric(object sender, RoutedEventArgs e)
        {
            try
            {
                string changedColumnName = "";
                MethodType methodType = MethodType.None;
                List<string> stringHeaders = new List<string>();
                foreach (var item in headers)
                {
                    double result;
                    var value = (from row in gridData.AsEnumerable()
                                 select row.Field<string>(item)).First();
                    if (!double.TryParse(value, out result))
                    {
                        stringHeaders.Add(item);
                    }
                }
                ChangeTextToNumeric changeTextToNumericWindow = new ChangeTextToNumeric(stringHeaders);
                if (changeTextToNumericWindow.ShowDialog() == false)
                {
                    changedColumnName = changeTextToNumericWindow.selectedColumnName;
                    methodType = changeTextToNumericWindow.methodType;
                }

                if (changedColumnName != "" && methodType != MethodType.None)
                    AddColumnAfterChange(changedColumnName, methodType);

            }catch(Exception exp)
            {
                MessageBox.Show("Wybrano błędne parametry!");
            }
         }

        public void AddColumnAfterChange(string columnName, MethodType methodType) // dodanie kolumny zamiana danych tekstowych na numeryczne
        {

            //pobieranie wartości kolumny bez powtarzania
            var distinctValues = (from row in gridData.AsEnumerable()
                                  select row.Field<string>(columnName)).Distinct().ToList();

            if (methodType == MethodType.Alphabetical)
            {
                string newColumnName = columnName + "__alph";

                distinctValues = distinctValues.OrderBy(x => x).ToList(); //sortowanie alfabetycznie

                gridData.Columns.Add(new DataColumn(newColumnName));
                headers.Add(newColumnName);
            }
            else if (methodType == MethodType.InOrder)
            {
                string newColumnName = columnName + "__order";

                gridData.Columns.Add(new DataColumn(newColumnName));
                headers.Add(newColumnName);
            }

            // odświeżenie widoku tablicy
            dataGridView.ItemsSource = null;
            dataGridView.ItemsSource = gridData.DefaultView;

            columns++;

            // uzupełnianie komórek tablicy wartościami
            foreach (DataRow row in gridData.Rows)
            {
                var orderNumber = distinctValues.FindIndex(x => x == row[columnName].ToString()) + 1;
                row[columns - 1] = orderNumber;
            }
        }

        private void Discretize(object sender, RoutedEventArgs e)
        {
            try { 
            string discretizedColumnName = "";
            int partitionNumber = 0;
            List<string> stringHeaders = new List<string>();
            foreach (var item in headers)
            {
                double result;
                var value = (from row in gridData.AsEnumerable()
                             select row.Field<string>(item)).First();
                if (double.TryParse(value, out result))
                {
                    stringHeaders.Add(item);
                }
            }
            DiscretizeWindow discretizeWindow = new DiscretizeWindow(stringHeaders);
            if (discretizeWindow.ShowDialog() == false)
            {
                discretizedColumnName = discretizeWindow.discretizedColumnName;
                partitionNumber = discretizeWindow.partitionNumber;
            }

            if (discretizedColumnName != "" && partitionNumber != 0)
                AddColumnAfterDiscretization(discretizedColumnName, partitionNumber);
            }
            catch (Exception exp)
            {
                MessageBox.Show("Wybrano błędne parametry!");
            }
        }

        public void AddColumnAfterDiscretization(string columnName, int partitionNumber)
        {
            //pobieranie wartości kolumny bez powtarzania
            var distinctValues = (from row in gridData.AsEnumerable()
                                  select Convert.ToDouble(row.Field<string>(columnName))).Distinct().ToList();

            double min = distinctValues.Min(); // wartość minimalna
            double max = distinctValues.Max(); // wartość maksymalna
            double step = (max - min) / partitionNumber; // krok
            discretizePartitions = new List<double>();
            discretizePartitions.Add(min);

            // lista discretizePartitions zawiera konce przedziałów
            for (int i = 0; i < partitionNumber - 1; i++)
            {
                discretizePartitions.Add(discretizePartitions[i] + step);
            }
            discretizePartitions.Add(max);

            //string newColumnName = columnName + "__d" + partitionNumber;
            string newColumnName = columnName + "__d";
            gridData.Columns.Add(newColumnName,typeof(String)).SetOrdinal(gridData.Columns.Count-2);
            headers.Add(newColumnName);

            // odświeżenie widoku tablicy
            dataGridView.ItemsSource = null;
            dataGridView.ItemsSource = gridData.DefaultView;

            columns++;

            // uzupełnianie komórek tablicy wartościami
            foreach (DataRow row in gridData.Rows)
            {
                var value = double.Parse(row[columnName].ToString());
                for (int i = 0; i < discretizePartitions.Count(); i++)
                {
                    if (value <= discretizePartitions[i]) // przedział prawostronnie domknięty
                    {
                        if (i == 0) row[columns - 2] = i + 1;
                        else row[columns - 2] = i;
                        break;
                    }
                }
            }
        }

        private void Normalize(object sender, RoutedEventArgs e)
        {
            //try { 
            string normalizedColumnName = "";
            bool normalizeAll = false;
            List<string> stringHeaders = new List<string>();
            foreach (var item in headers)
            {
                double result;
                var value = (from row in gridData.AsEnumerable()
                             select row.Field<string>(item)).First();
                if (double.TryParse(value, out result))
                {
                    stringHeaders.Add(item);
                }
            }
            NormalizeWindow normalizeWindow = new NormalizeWindow(stringHeaders);
            if (normalizeWindow.ShowDialog() == false)
            {
                    if (normalizeWindow.all)
                    {
                        normalizeAll = true;
                    }
                    else
                    {
                        normalizedColumnName = normalizeWindow.normalizedColumnName;
                    }
               
            }

            if (normalizedColumnName != "")
                AddColumnAfterNormalize(normalizedColumnName);

                if (normalizeAll == true)
                    AddColumnAfterNormalizeAll();

            //}
            //catch (Exception exp)
            //{
            //    MessageBox.Show("Wybrano błędne parametry!");
            //}
        }

        public void AddColumnAfterNormalize(string columnName)
        {
            //string columnName = gridData.Columns[columnIndex].ColumnName;
            

            //pobieranie wartości kolumny
            var values = (from row in gridData.AsEnumerable()
                                  select Convert.ToDouble(row.Field<string>(columnName))).ToList();

            var average = values.Average();
            double deviation = 0;
            double sum = 0;
            for (int i = 0; i < values.Count(); i++)
            {
                sum += Math.Pow((values[i] - average), 2);
            }

            deviation = Math.Sqrt(sum / values.Count());

            string newColumnName = columnName + "__norm";
            gridData.Columns.Add( new DataColumn(newColumnName));
            headers.Add(newColumnName);

            // odświeżenie widoku tablicy
            dataGridView.ItemsSource = null;
            dataGridView.ItemsSource = gridData.DefaultView;

            columns++;

            // uzupełnianie komórek tablicy wartościami
            foreach (DataRow row in gridData.Rows)
            { 
                row[columns - 1] = ((Convert.ToDouble(row[columnName].ToString()) - average) / deviation);
            }
            
        }

        public void AddColumnAfterNormalizeAll()
        {
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";
            format.NumberDecimalSeparator = ",";
            for (int i = 0; i < gridData.Columns.Count-1; i++)
            {
                var values = (from row in gridData.AsEnumerable()
                              select Convert.ToDouble(ConvertStringValueZero(row.Field<string>(gridData.Columns[i].ColumnName)), format)).ToList();

                var average = values.Average();
                double deviation = 0;
                double sum = 0;
                for (int j = 0; j < values.Count(); j++)
                {
                    sum += Math.Pow((values[j] - average), 2);
                }

                deviation = Math.Sqrt(sum / values.Count());

                foreach (DataRow row in gridData.Rows)
                {
                    var value = ((Convert.ToDouble(row[gridData.Columns[i].ColumnName].ToString(), format) - average) / deviation);
                    if (Double.IsNaN(value))
                    {
                        row[gridData.Columns[i].ColumnName] = 0;
                    }
                    else
                    {
                        row[gridData.Columns[i].ColumnName] = ((Convert.ToDouble(row[gridData.Columns[i].ColumnName].ToString(), format) - average) / deviation);
                    }
                    
                }

            }
        }

        private void ChangeRange(object sender, RoutedEventArgs e)
        {
            try { 
            string rangedColumnName = "";
            double newMin = 0, newMax = 0;
            List<string> stringHeaders = new List<string>();
            foreach (var item in headers)
            {
                double result;
                var value = (from row in gridData.AsEnumerable()
                             select row.Field<string>(item)).First();
                if (double.TryParse(value, out result))
                {
                    stringHeaders.Add(item);
                }
            }
            NewRangeWindow newRangeWindow = new NewRangeWindow(stringHeaders);
            if (newRangeWindow.ShowDialog() == false)
            {
                rangedColumnName = newRangeWindow.rangedColumnName;
                newMin = newRangeWindow.newMin;
                newMax = newRangeWindow.newMax;
            }

            if (rangedColumnName != "")
                AddColumnAfterRangeChange(rangedColumnName, newMin, newMax);
            }
            catch (Exception exp)
            {
                MessageBox.Show("Wybrano błędne parametry!");
            }
        }

        private void AddColumnAfterRangeChange(string columnName, double newMin, double newMax)
        {
            //string columnName = gridData.Columns[rangedColumnIndex].ColumnName;

            //pobieranie wartości kolumny bez powtarzania
            var values = (from row in gridData.AsEnumerable()
                                  select Convert.ToDouble(row.Field<string>(columnName))).ToList();

            var oldMin = values.Min();
            var oldMax = values.Max();

            string newColumnName = columnName + "__range__" + newMin + "__" + newMax;
            gridData.Columns.Add(new DataColumn(newColumnName));
            headers.Add(newColumnName);

            // odświeżenie widoku tablicy
            dataGridView.ItemsSource = null;
            dataGridView.ItemsSource = gridData.DefaultView;

            columns++;

            foreach (DataRow row in gridData.Rows)
            {
                var testy = Convert.ToDouble(row[columnName].ToString());
                var testy2 = Convert.ToDouble(row[columnName].ToString()) - oldMin;
                var newValue = (((Convert.ToDouble(row[columnName].ToString()) - oldMin) * (newMax - newMin)) / (oldMax - oldMin)) + newMin;
                row[columns - 1] = newValue;
            }
                
        }

        private void SelectPercentage(object sender, RoutedEventArgs e)
        {
            try { 
            string selectedColumnName = "";
            double percentage = 0;
            ValuesType valuesType = ValuesType.None;
            List<string> stringHeaders = new List<string>();
            foreach (var item in headers)
            {
                double result;
                var value = (from row in gridData.AsEnumerable()
                             select row.Field<string>(item)).First();
                if (double.TryParse(value, out result))
                {
                    stringHeaders.Add(item);
                }
            }
            SelectPercentageWindow selectPercentageWindow = new SelectPercentageWindow(stringHeaders);
            if (selectPercentageWindow.ShowDialog() == false)
            {
                selectedColumnName = selectPercentageWindow.selectedColumnName;
                percentage = selectPercentageWindow.chosenPercentage;
                valuesType = selectPercentageWindow.valuesType;

            }

            if (selectedColumnName != "" && valuesType != ValuesType.None)
                SelectAfterSelectPercentage(selectedColumnName, percentage, valuesType);
            }
            catch (Exception exp)
            {
                MessageBox.Show("Wybrano błędne parametry!");
            }
        }

        private void SelectAfterSelectPercentage(string columnName, double percentage, ValuesType valuesType)
        {
            //string columnName = gridData.Columns[selectedColumnIndex].ColumnName;
            List<double> chosenValues = new List<double>(); // wartości wchodządze w zakres procentowy
            List<int> rowsIndexes = new List<int>();
            //pobieranie wartości kolumny bez powtarzania
            var values = (from row in gridData.AsEnumerable()
                          select Convert.ToDouble(row.Field<string>(columnName))).ToList();

            var amount = (int)(values.Count() * (percentage/100));
            if (valuesType == ValuesType.Smallest) values = values.OrderBy(x => x).ToList();
            else if (valuesType == ValuesType.Biggest) values = values.OrderByDescending(x => x).ToList();

            for (int i = 0; i < amount; i++)
            {
                chosenValues.Add(values[i]);
            }

            dataGridView.Focus();
            foreach (DataRow row in gridData.Rows)
            {
                if (chosenValues.Contains(Convert.ToDouble(row[columnName].ToString()))){
                    rowsIndexes.Add(gridData.Rows.IndexOf(row));               
                }
            }
            if (rowsIndexes.Count() == 0)
            {
                MessageBox.Show("Ten procent dla wczytanych danych jest za mały");
            }
            else
            {
                SelectRowByIndexes(dataGridView, rowsIndexes);
            }
            
        }

        private void DrawHistogramContinuous(object sender, RoutedEventArgs e)
        {
            try { 
            List<string> stringHeaders = new List<string>();
            foreach (var item in headers)
            {
                double result;
                var value = (from row in gridData.AsEnumerable()
                             select row.Field<string>(item)).First();
                if (double.TryParse(value, out result))
                {
                    stringHeaders.Add(item);
                }
            }
            DrawHistogramContinuousWindow dhcw = new DrawHistogramContinuousWindow(stringHeaders);
            if(dhcw.ShowDialog() == true)
            {

            }
            }
            catch (Exception exp)
            {
                MessageBox.Show("Wybrano błędne parametry!");
            }
        }

        private void DrawHistogram(object sender, RoutedEventArgs e)
        {
            try { 
                DrawHistogramWindow dhw = new DrawHistogramWindow(headers);
                if(dhw.ShowDialog() == true)
                {

                }
            }
            catch (Exception exp)
            {
                MessageBox.Show("Wybrano błędne parametry!");
            }
        }

        private void Draw2DChart(object sender, RoutedEventArgs e)
        {
            try { 
            List<string> stringHeaders = new List<string>();
            List<string> valuesHeaders = new List<string>();
            foreach (var item in headers)
            {
                double result;
                var value = (from row in gridData.AsEnumerable()
                             select row.Field<string>(item)).First();
                if (double.TryParse(value, out result))
                {
                    valuesHeaders.Add(item);
                }
                else
                {
                    stringHeaders.Add(item);
                }
            }
                ScatterPlotWindow spw = new ScatterPlotWindow(valuesHeaders, stringHeaders, headers);
                if (spw.ShowDialog() == true)
                {

                }
            }
            catch (Exception exp)
            {
                MessageBox.Show("Wybrano błędne parametry!");
            }
        }

        private void Draw3DChart(object sender, RoutedEventArgs e)
        {
            try { 
            List<string> valuesHeaders = new List<string>();
            foreach (var item in headers)
            {
                double result;
                var value = (from row in gridData.AsEnumerable()
                             select row.Field<string>(item)).First();
                if (double.TryParse(value, out result))
                {
                    valuesHeaders.Add(item);
                }
            }
            _3DWindow window3D = new _3DWindow(valuesHeaders);
            if (window3D.ShowDialog() == true)
            {

            }
            }
            catch (Exception exp)
            {
                MessageBox.Show("Wybrano błędne parametry!");
            }
        }

        private void KNN(object sender, RoutedEventArgs e)
        {
            List<string> valuesHeaders = new List<string>();
            foreach (var item in headers)
            {
                double result;
                var value = (from row in gridData.AsEnumerable()
                             select row.Field<string>(item)).First();
                if (double.TryParse(value, out result))
                {
                    valuesHeaders.Add(item);
                }
            }
            KNNMethod kNNMethodWindow = new KNNMethod(valuesHeaders);
            if (kNNMethodWindow.ShowDialog() == true)
            {

            }
        }

        private void LeaveOneOut(object sender, RoutedEventArgs e)
        {
            LeaveOneOutWindow leaveOneOutWindow = new LeaveOneOutWindow();
            if(leaveOneOutWindow.ShowDialog() == true)
            {

            }
        }

        private void TwoDimentionsED(object sender, RoutedEventArgs e)
        {
            List<string> stringHeaders = new List<string>();
            List<string> valuesHeaders = new List<string>();
            foreach (var item in headers)
            {
                double result;
                var value = (from row in gridData.AsEnumerable()
                             select row.Field<string>(item)).First();
                if (double.TryParse(value, out result))
                {
                    valuesHeaders.Add(item);
                }
                else
                {
                    stringHeaders.Add(item);
                }
            }
            EDTwoDimentions eDTwoDimentions = new EDTwoDimentions(headers, headers);
            if(eDTwoDimentions.ShowDialog() == true)
            {

            }
        }

        private void MoreDimensionsED(object sender, RoutedEventArgs e)
        {          
            EDMoreDimentions eDTwoDimentions = new EDMoreDimentions(openedFileName);
            if (eDTwoDimentions.ShowDialog() == true)
            {

            }
        }
        
        private void KAverageClassify(object sender, RoutedEventArgs e)
        {
            KAverageWindow kAverageWindow = new KAverageWindow();
            if (kAverageWindow.ShowDialog() == true)
            {

            }
            //var col1 = (from row in gridData.AsEnumerable()
            //            select Convert.ToDouble(ConvertString(row.Field<string>(gridData.Columns[gridData.Columns.Count-1].ColumnName)))).ToArray();
            //var col2 = (from row in gridData.AsEnumerable()
            //            select Convert.ToDouble(ConvertString(row.Field<string>(gridData.Columns[gridData.Columns.Count - 2].ColumnName)))).ToArray();
            //var a = new Jaccard();
            //MessageBox.Show(a.Similarity(col1, col2).ToString());
            //MessageBox.Show(a.Distance(col1, col2).ToString());
        }

        private void ManualClassify(object sender, RoutedEventArgs e)
        {
            List<string> stringHeaders = new List<string>();
            foreach (var item in headers)
            {
                double result;
                var value = (from row in gridData.AsEnumerable()
                             select row.Field<string>(item)).First();
                if (!double.TryParse(value, out result))
                {
                    stringHeaders.Add(item);
                }
            }

            if(stringHeaders.Count == 1)
            {
                var distinctClasses = (from row in gridData.AsEnumerable()
                                       select row.Field<string>(stringHeaders[0])).Distinct().ToList();
                ManualClassificationWindow manualClassificationWindow = new ManualClassificationWindow(distinctClasses, stringHeaders[0]);
                if (manualClassificationWindow.ShowDialog() == true)
                {

                }
            }
            
        }

        private void SimilarityCount(object sender, RoutedEventArgs e)
        {
            List<string> valuesHeaders = new List<string>();
            foreach (var item in headers)
            {
                double result;
                var value = (from row in gridData.AsEnumerable()
                             select row.Field<string>(item)).First();
                if (double.TryParse(value, out result))
                {
                    valuesHeaders.Add(item);
                }
            }
            SimilarityCountWindow similarityCountWindow = new SimilarityCountWindow(valuesHeaders);
            if(similarityCountWindow.ShowDialog() == true)
            {

            }
        }

        private void DecisionTree(object sender, RoutedEventArgs e)
        {

            DecisionTreeWindow decisionTree = new DecisionTreeWindow();
            if (decisionTree.ShowDialog() == true)
            {

            }
        }

        public static void SelectRowByIndexes(DataGrid dataGrid, List<int> rowIndexes)
        {
            if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.FullRow))
                throw new ArgumentException("The SelectionUnit of the DataGrid must be set to FullRow.");

            if (!dataGrid.SelectionMode.Equals(DataGridSelectionMode.Extended))
                throw new ArgumentException("The SelectionMode of the DataGrid must be set to Extended.");

            if (rowIndexes.Count().Equals(0) || rowIndexes.Count() > dataGrid.Items.Count)
                throw new ArgumentException("Invalid number of indexes.");

            dataGrid.SelectedItems.Clear();
            foreach (int rowIndex in rowIndexes)
            {
                if (rowIndex < 0 || rowIndex > (dataGrid.Items.Count - 1))
                    throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));

                object item = dataGrid.Items[rowIndex]; //=Product X
                dataGrid.SelectedItems.Add(item);

                DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                if (row == null)
                {
                    dataGrid.ScrollIntoView(item);
                    row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                }
                if (row != null)
                {
                    DataGridCell cell = GetCell(dataGrid, row, 0);
                    if (cell != null)
                        cell.Focus();
                }
            }
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    /* if the row has been virtualized away, call its ApplyTemplate() method 
                     * to build its visual tree in order for the DataGridCellsPresenter
                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }

        private string ConvertStringValueZero(string value)
        {
            if (value == "0.0") return "0";
            else return value;
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
