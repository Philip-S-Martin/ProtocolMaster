using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;


namespace ProtocolMaster.View
{
    /// <summary>
    /// Interaction logic for TimelinePane.xaml
    /// </summary>
    public partial class TimelinePane : Page
    {
        public TimelinePane()
        {
            InitializeComponent();
            SetUpPlot();
        }

        public void ListDriver(string name)
        {
            MenuItem newDriver = new MenuItem();
            newDriver.Header = name;
            DriverDropdown.Items.Add(newDriver);
        }

        public void ListInterpreter(string name)
        {
            MenuItem newInterpreter = new MenuItem();
            newInterpreter.Header = name;
            InterpreterDropdown.Items.Add(newInterpreter);
        }

        private void SetUpPlot()
        {
            DateTime start = new DateTime(2017, 1, 1, 15, 20, 0);
            DateTime end = new DateTime(2017, 1, 1, 15, 30, 0);

            var model = new PlotModel();
            model.IsLegendVisible = false;

            model.Axes.Add(new OxyPlot.Axes.TimeSpanAxis()
            {
                Position = AxisPosition.Bottom,
                TicklineColor = OxyColors.Gray,
                AxislineColor = OxyColors.Gray,
                MinorTicklineColor = OxyColors.Gray,
                TextColor = OxyColors.WhiteSmoke,
                TitleColor = OxyColors.WhiteSmoke,
                ExtraGridlineColor = OxyColors.Gray,
                MinorGridlineColor = OxyColors.Gray,
                MajorGridlineColor = OxyColors.Gray
            });
            model.Axes.Add(new OxyPlot.Axes.CategoryAxis()
            {
                Position = AxisPosition.Left,
                TicklineColor = OxyColors.Gray,
                AxislineColor = OxyColors.Gray,
                MinorTicklineColor = OxyColors.Gray,
                TextColor = OxyColors.WhiteSmoke,
                TitleColor = OxyColors.WhiteSmoke,
                ExtraGridlineColor = OxyColors.Gray,
                MinorGridlineColor = OxyColors.Gray,
                MajorGridlineColor = OxyColors.Gray
            });
            plot.Model = model;


            var series = new OxyPlot.Series.IntervalBarSeries { Title = "Series 1", StrokeThickness = 1 };
            model.Series.Add(series);

            Random random = new Random();

            plot.Model.DefaultColors = new List<OxyColor>
                {
                OxyColors.Gray,
                OxyColors.Gray,
                OxyColors.Gray,
                OxyColors.Gray
                };
            plot.Model.TextColor = OxyColors.White;
            plot.Model.PlotAreaBorderColor = OxyColors.Gray;

            for (int i = 0; i < 10; i++)
            {
                var targetSeries = new OxyPlot.Series.IntervalBarSeries { Title = "Series " + i.ToString(), StrokeThickness = 1 };
                for (int j = 0; j < random.Next(0, i); j++)
                    targetSeries.Items.Add(new IntervalBarItem { CategoryIndex = j, Start = start.AddHours(i).ToOADate(), End = end.AddHours(i).ToOADate() });
                model.Series.Add(targetSeries);
                targetSeries.LabelField = "Test" + i;
            }

        }
    }
}
