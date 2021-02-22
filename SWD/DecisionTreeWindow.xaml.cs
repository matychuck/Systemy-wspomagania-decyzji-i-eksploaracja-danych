using System;
using System.Collections.Generic;
using System.Data;
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
    /// Logika interakcji dla klasy DecisionTreeWindow.xaml
    /// </summary>
    public partial class DecisionTreeWindow : Window
    {
        MainWindow mainWindow = null;

        public DecisionTreeWindow()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
        }

        private void CreateTree(object sender, RoutedEventArgs e)
        {
            DataTable discretizedData = mainWindow.gridData.Copy();
            
            int index = int.Parse(indexes.Text);
            for(int i = 0; i < index; i++)
            {
                discretizedData.Columns.RemoveAt(0);
            }
            //StreamWriter writer = new StreamWriter("C:\\Magisterka sem2\\SWD\\iris5.txt");
            //for (int i = 0; i < discretizedData.Rows.Count; i++)
            //{
            //    for (int j = 0; j < discretizedData.Columns.Count; j++)
            //    {
            //        if (j == discretizedData.Columns.Count - 1)
            //            writer.Write(discretizedData.Rows[i][j].ToString());
            //        else
            //            writer.Write(discretizedData.Rows[i][j].ToString() + " ");
            //    }
            //    writer.WriteLine();
            //}
            //writer.Close();
            var treeRules = CreateTreeAndHandleUserOperation(discretizedData);
            foreach (var rule in treeRules)
            {
                foreach (var item in rule)
                {
                    rules.Text += item;
                }

                rules.Text += "\n";
            }

            MessageBox.Show("Jakość klasyfikacji: " + DecisionTreeLeaveOneOut(discretizedData).ToString());
        }

        private static List<List<string>> CreateTreeAndHandleUserOperation(DataTable data)
        {
            var decisionTree = new Tree();
            decisionTree.ClearRules();
            decisionTree.Root = Tree.Learn(data, "");
            return Tree.Print(decisionTree.Root, decisionTree.Root.Name.ToUpper());
           
        }

        private static double DecisionTreeLeaveOneOut(DataTable data)
        {
            int[] compatibility = new int[data.Rows.Count];
            DataTable dataReduced = new DataTable();
            var decisionTree = new Tree();
            int sum = 0;
            double quality = 0;
            for(int i = 0; i < data.Rows.Count; i++)
            {
                dataReduced = data.Copy();
                dataReduced.Rows.RemoveAt(i);
                decisionTree = new Tree();
                Dictionary<string, string> element = new Dictionary<string, string>();
                for(int j = 0; j < data.Columns.Count - 1; j++)
                {
                    element.Add(data.Columns[j].ColumnName, data.Rows[i][j].ToString());
                }
                decisionTree.Root = Tree.Learn(dataReduced, "");
                var resultClass = Tree.DecideResultClass(decisionTree.Root, element);
                if (resultClass == data.Rows[i][data.Columns.Count - 1].ToString())
                    compatibility[i] = 1;
                else
                    compatibility[i] = 0;
            }

            for (int z = 0; z < compatibility.Length; z++)
            {
                if (compatibility[z] == 1) sum++;
            }

            quality = (double)((double)sum / (double)data.Rows.Count);

            return quality;
        }
    }
}
