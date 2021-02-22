using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD
{
    class ScatterPlotViewModel
    {
        public ScatterPlotViewModel()
        {
            var model = new PlotModel { Title = "" };
            var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
            
            this.Items = scatterSeries;
            model.Series.Add(scatterSeries);  
            var customAxis = new RangeColorAxis { Key = "customColors" };
            customAxis.AddRange(0, 0.1, OxyColors.Red);
            customAxis.AddRange(0.2, 0.3, OxyColors.Yellow);
            customAxis.AddRange(0.4, 0.5, OxyColors.Green);
            customAxis.AddRange(0.6, 0.7, OxyColors.Orange);
            customAxis.AddRange(0.8, 0.9, OxyColors.Blue);
            customAxis.AddRange(1.0, 1.1, OxyColors.DarkOliveGreen);
            customAxis.AddRange(1.2, 1.3, OxyColors.Black);
            customAxis.AddRange(1.4, 1.5, OxyColors.Violet);
            customAxis.AddRange(1.6, 1.7, OxyColors.Brown);
            customAxis.AddRange(1.8, 1.9, OxyColors.Cyan);
            model.Axes.Add(customAxis);
            this.MyModel = model;


        }

        public void ChangeViewModel(ScatterSeries items)
        {
            this.Items = items;

            var model = new PlotModel { Title = "" };

            model.Series.Add(items);
            var customAxis = new RangeColorAxis { Key = "customColors" };
            customAxis.AddRange(0, 0.1, OxyColors.Red);
            customAxis.AddRange(0.2, 0.3, OxyColors.Yellow);
            customAxis.AddRange(0.4, 0.5, OxyColors.LightGreen);
            customAxis.AddRange(0.6, 0.7, OxyColors.Orange);
            customAxis.AddRange(0.8, 0.9, OxyColors.Blue);
            customAxis.AddRange(1.0, 1.1, OxyColors.DarkOliveGreen);
            customAxis.AddRange(1.2, 1.3, OxyColors.Black);
            customAxis.AddRange(1.4, 1.5, OxyColors.Violet);
            customAxis.AddRange(1.6, 1.7, OxyColors.Brown);
            customAxis.AddRange(1.8, 1.9, OxyColors.Cyan);
            model.Axes.Add(customAxis);
            //model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Liczba sąsiadów", MajorTickSize = 0.1, Minimum = 0, Maximum = 25 });
            //model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = " Jakość", MajorTickSize = 0.1, Minimum = 0, Maximum = 10 });
            this.MyModel = model;
            this.MyModel.InvalidatePlot(true);
        }

        public PlotModel MyModel { get; private set; }
        public ScatterSeries Items { get; set; }
    }

}
