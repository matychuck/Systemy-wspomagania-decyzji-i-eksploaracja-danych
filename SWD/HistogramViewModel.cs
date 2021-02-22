using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SWD.DrawHistogramWindow;

namespace SWD
{
    class HistogramViewModel
    {

        public HistogramViewModel()
        {
            this.Items = new Collection<Item>();
            var model = new PlotModel { Title = "Column Series", LegendPlacement = LegendPlacement.Outside, LegendPosition = OxyPlot.LegendPosition.RightTop, LegendOrientation = LegendOrientation.Vertical };

            model.Axes.Add(new CategoryAxis { Position = AxisPosition.Bottom, ItemsSource = this.Items, LabelField = "Label" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, MinimumPadding = 0, AbsoluteMinimum = 0 });

            model.Series.Add(new ColumnSeries { Title = "2009", ItemsSource = this.Items, ValueField = "Value" });

            this.MyModel = model;


        }

        public void ChangeViewModel(Collection<Item> items, string columnName)
        {
            this.Items = items;
            
            var model = new PlotModel { Title = columnName, LegendPlacement = LegendPlacement.Outside, LegendPosition = OxyPlot.LegendPosition.RightTop, LegendOrientation = LegendOrientation.Vertical };

            model.Axes.Add(new CategoryAxis { Position = AxisPosition.Bottom, ItemsSource = this.Items, LabelField = "Label" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, MinimumPadding = 0, AbsoluteMinimum = 0 });

            model.Series.Add(new ColumnSeries { Title = columnName, ItemsSource = this.Items, ValueField = "Value" });

            this.MyModel = model;
            this.MyModel.InvalidatePlot(true);
        }

        public PlotModel MyModel { get; private set; }
        public Collection<Item> Items { get; set; }        

    }

    public class Item
    {
        public string Label { get; set; }
        public double Value { get; set; }
    }
}
