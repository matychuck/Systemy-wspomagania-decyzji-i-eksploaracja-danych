using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD
{
    class LinearQualityChartViewModel
    {
        public PlotModel MyModel { get; private set; }
        public double[] Items { get; set; }


        public LinearQualityChartViewModel()
        {
            var model = new PlotModel { Title = "Jakość metryki" };
           
            this.MyModel = model;
        }

        public void ChangeViewModel(double[] data, string metricName)
        {
            this.Items = data;

            var model = new PlotModel { Title = "Jakość " + metricName };

            model.LegendPosition = LegendPosition.RightTop;
            model.LegendPlacement = LegendPlacement.Outside;
            model.PlotMargins = new OxyThickness(50, 0, 0, 40);

            { // kolor czerwony
                var ls = new LineSeries { Title = "Jakość" };
                //ls.MarkerStroke = OxyPlot.OxyColors.Tomato;
                ls.Color = OxyColors.Red;
                for (int j = 0; j < data.Length; j++)
                {
                    double x = j;
                    double y = data[j];
                    ls.Points.Add(new DataPoint(x, y));
                }
                model.Series.Add(ls);
            }
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Liczba sąsiadów"});
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = " Jakość",  MajorTickSize=0.1, Minimum = 0,  Maximum = 1 });
            this.MyModel = model;
            this.MyModel.InvalidatePlot(true);
        }
    }
}
