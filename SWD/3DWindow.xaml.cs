using System;
using System.Collections;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace SWD
{
    /// <summary>
    /// Logika interakcji dla klasy _3DWindow.xaml
    /// </summary>
    public partial class _3DWindow : Window
    {
        public SWD.TransformMatrix m_transformMatrix = new SWD.TransformMatrix();

        // ***************************** 3d chart ***************************
        private SWD.Chart3D m_3dChart;       // data for 3d chart
        public int m_nChartModelIndex = -1;         // model index in the Viewport3d
        public int m_nSurfaceChartGridNo = 100;     // surface chart grid no. in each axis
        public int m_nScatterPlotDataNo = 5000;     // total data number of the scatter plot

        // ***************************** selection rect ***************************
        ViewportRect m_selectRect = new ViewportRect();
        public int m_nRectModelIndex = -1;

        public _3DWindow(List<string> headers)
        {
            InitializeComponent();
            comboBoxX.ItemsSource = headers;
            comboBoxY.ItemsSource = headers;
            comboBoxZ.ItemsSource = headers;
        }

        public void OnViewportMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs args)
        {
            Point pt = args.GetPosition(mainViewport);
            if (args.ChangedButton == MouseButton.Left)         // rotate or drag 3d model
            {
                m_transformMatrix.OnLBtnDown(pt);
            }
            else if (args.ChangedButton == MouseButton.Right)   // select rect
            {
                m_selectRect.OnMouseDown(pt, mainViewport, m_nRectModelIndex);
            }
        }

        public void OnViewportMouseMove(object sender, System.Windows.Input.MouseEventArgs args)
        {
            Point pt = args.GetPosition(mainViewport);

            if (args.LeftButton == MouseButtonState.Pressed)                // rotate or drag 3d model
            {
                m_transformMatrix.OnMouseMove(pt, mainViewport);

                TransformChart();
            }
            else if (args.RightButton == MouseButtonState.Pressed)          // select rect
            {
                m_selectRect.OnMouseMove(pt, mainViewport, m_nRectModelIndex);
            }
            else
            {
                /*
                String s1;
                Point pt2 = m_transformMatrix.VertexToScreenPt(new Point3D(0.5, 0.5, 0.3), mainViewport);
                s1 = string.Format("Screen:({0:d},{1:d}), Predicated: ({2:d}, H:{3:d})", 
                    (int)pt.X, (int)pt.Y, (int)pt2.X, (int)pt2.Y);
                this.statusPane.Text = s1;
                */
            }
        }

        public void OnViewportMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs args)
        {
            Point pt = args.GetPosition(mainViewport);
            if (args.ChangedButton == MouseButton.Left)
            {
                m_transformMatrix.OnLBtnUp();
            }
            else if (args.ChangedButton == MouseButton.Right)
            {
                if (m_nChartModelIndex == -1) return;
                // 1. get the mesh structure related to the selection rect
                MeshGeometry3D meshGeometry = Model3D.GetGeometry(mainViewport, m_nChartModelIndex);
                if (meshGeometry == null) return;

                // 2. set selection in 3d chart
                m_3dChart.Select(m_selectRect, m_transformMatrix, mainViewport);

                // 3. update selection display
                m_3dChart.HighlightSelection(meshGeometry, Color.FromRgb(200, 200, 200));
            }
        }

        public void TestSimpleScatterPlot()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            string columnNameX = comboBoxX.SelectedItem.ToString();
            string columnNameY = comboBoxY.SelectedItem.ToString();
            string columnNameZ = comboBoxZ.SelectedItem.ToString();

            var valuesX = (from row in mainWindow.gridData.AsEnumerable()
                          select Convert.ToDouble(row.Field<string>(columnNameX))).ToList();
            var valuesY = (from row in mainWindow.gridData.AsEnumerable()
                           select Convert.ToDouble(row.Field<string>(columnNameY))).ToList();
            var valuesZ = (from row in mainWindow.gridData.AsEnumerable()
                           select Convert.ToDouble(row.Field<string>(columnNameZ))).ToList();

            int nDotNo = mainWindow.gridData.Rows.Count;
            // 1. set the scatter plot size
            m_3dChart = new ScatterChart3D();
            m_3dChart.SetDataNo(nDotNo);

            // 2. set the properties of each dot
            Random randomObject = new Random();
            for (int i = 0; i < nDotNo; i++)
            {
                ScatterPlotItem plotItem = new ScatterPlotItem();
               
                plotItem.w = (float)0.1;
                plotItem.h = (float)0.1;

                plotItem.x = (float)valuesX[i];
                plotItem.y = (float)valuesY[i];
                plotItem.z = (float)valuesZ[i];

                plotItem.shape = (int)Chart3D.SHAPE.ELLIPSE;
                plotItem.nMaxI = 1;

                Byte nR = (Byte)randomObject.Next(256);
                Byte nG = (Byte)randomObject.Next(256);
                Byte nB = (Byte)randomObject.Next(256);

                plotItem.color = Color.FromRgb(255, 0, 0);
                ((ScatterChart3D)m_3dChart).SetVertex(i, plotItem);
            }
            // 3. set axes
            m_3dChart.GetDataRange();
            m_3dChart.SetAxes();

            // 4. Get Mesh3D array from scatter plot
            ArrayList meshs = ((ScatterChart3D)m_3dChart).GetMeshes();

            // 5. display vertex no and triangle no.
            UpdateModelSizeInfo(meshs);

            // 6. show 3D scatter plot in Viewport3d
            SWD.Model3D model3d = new SWD.Model3D();
            m_nChartModelIndex = model3d.UpdateModel(meshs, null, m_nChartModelIndex, this.mainViewport);

            // 7. set projection matrix
            float viewRange = 5;
            m_transformMatrix.CalculateProjectionMatrix(0, viewRange, 0, viewRange, 0, viewRange, 0.5);
            TransformChart();
        }

        private void UpdateModelSizeInfo(ArrayList meshs)
        {
            int nMeshNo = meshs.Count;
            int nChartVertNo = 0;
            int nChartTriangelNo = 0;
            for (int i = 0; i < nMeshNo; i++)
            {
                nChartVertNo += ((Mesh3D)meshs[i]).GetVertexNo();
                nChartTriangelNo += ((Mesh3D)meshs[i]).GetTriangleNo();
            }
           // labelVertNo.Content = String.Format("Vertex No: {0:d}", nChartVertNo);
           // labelTriNo.Content = String.Format("Triangle No: {0:d}", nChartTriangelNo);
        }

        // this function is used to rotate, drag and zoom the 3d chart
        private void TransformChart()
        {
            if (m_nChartModelIndex == -1) return;
            ModelVisual3D visual3d = (ModelVisual3D)(this.mainViewport.Children[m_nChartModelIndex]);
            if (visual3d.Content == null) return;
            Transform3DGroup group1 = visual3d.Content.Transform as Transform3DGroup;
            group1.Children.Clear();
            group1.Children.Add(new MatrixTransform3D(m_transformMatrix.m_totalMatrix));
        }

        private void Draw3DButton_Click(object sender, RoutedEventArgs e)
        {
            TestSimpleScatterPlot();
        }
    }
}
