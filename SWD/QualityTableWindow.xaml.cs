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
    /// Logika interakcji dla klasy QualityTableWindow.xaml
    /// </summary>
    public partial class QualityTableWindow : Window
    {
        public QualityTableWindow(string metricName, double[] quality)
        {
            InitializeComponent();
            DataTable gridData = new DataTable();
            gridData.Columns.Add(new DataColumn("k"));
            gridData.Columns.Add(new DataColumn("Q_"+metricName));

            for(int i = 0; i < quality.Length; i++)
            {
                var newRow = gridData.NewRow();
                newRow[0] = i+1;
                newRow[1] = quality[i];
                gridData.Rows.Add(newRow);
            }

            dataGridView.DataContext = gridData.DefaultView;
        }
    }
}
