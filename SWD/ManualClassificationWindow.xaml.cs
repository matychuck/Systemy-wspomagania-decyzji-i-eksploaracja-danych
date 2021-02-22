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
    /// Logika interakcji dla klasy ManualClassificationWindow.xaml
    /// </summary>
    public partial class ManualClassificationWindow : Window
    {
        MainWindow mainWindow = null;
        public Dictionary<string, int> classesWithNumbers = new Dictionary<string, int>();
        public List<string> classes = null;
        public int index;
        public string columnWithClassesName;
        public ManualClassificationWindow(List<string> classes, string columnWithClassesName)
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
            this.classes = classes;
            index = 1;
            stepLabel.Content = index + "/" + classes.Count;
            classLabel.Content = classes[0];
            this.columnWithClassesName = columnWithClassesName;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            classesWithNumbers.Add(classLabel.Content.ToString(), int.Parse(textBox.Text));
            if (index < classes.Count)
            {
                index++;
                stepLabel.Content = index + "/" + classes.Count;
                classLabel.Content = classes[index - 1];
                textBox.Text = "";
                textBox.Focus();
            }
            else
            {
                if(mainWindow.gridData.Columns.Contains("ręczna klasyfikacja"))
                {
                    foreach (DataRow row in mainWindow.gridData.Rows)
                    {
                        int number = -1;
                        number = classesWithNumbers[row[columnWithClassesName].ToString()];
                        row["ręczna klasyfikacja"] = number;
                    }
                    
                }
                else
                {                    
                    mainWindow.gridData.Columns.Add(new DataColumn("ręczna klasyfikacja"));
                    mainWindow.headers.Add("ręczna klasyfikacja");
                    mainWindow.columns++;
                    foreach (DataRow row in mainWindow.gridData.Rows)
                    {
                        int number = -1;
                        number = classesWithNumbers[row[columnWithClassesName].ToString()];
                        row[mainWindow.columns - 1] = number;
                    }
                    this.Close();
                }
                
                mainWindow.dataGridView.ItemsSource = null;
                mainWindow.dataGridView.ItemsSource = mainWindow.gridData.DefaultView;
                MessageBox.Show("Klasyfikacja zakończona pomyślnie!");
            }
            
        }
    }
}
