using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using ProtocolMaster.Component.Debug;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        public void ListVisualizer(string name)
        {
            MenuItem newVis = new MenuItem();
            newVis.Header = name;
            VisualizerDropdown.Items.Add(newVis);
        }


        public void Start_Click(object sender, RoutedEventArgs e)
        {

        }

        public async void Stop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetUpPlot()
        {
            DateTime start = new DateTime(2017, 1, 1, 15, 20, 0);
            DateTime end = new DateTime(2017, 1, 1, 15, 30, 0);

            var model = new PlotModel();
            model.IsLegendVisible = false;

            model.Axes.Add(new OxyPlot.Axes.DateTimeAxis()
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
            var categoryAxis = new OxyPlot.Axes.CategoryAxis()
            {
                Position = AxisPosition.Left,
                TicklineColor = OxyColors.Transparent,
                AxislineColor = OxyColors.Gray,
                MinorTicklineColor = OxyColors.Transparent,
                TextColor = OxyColors.WhiteSmoke,
                TitleColor = OxyColors.WhiteSmoke,
                ExtraGridlineColor = OxyColors.Gray,
                MinorGridlineColor = OxyColors.Gray,
                MajorGridlineColor = OxyColors.Gray,
                ExtraGridlines = new double[] {0,1,2,3}
            };
            categoryAxis.Labels.Add("Sound");
            categoryAxis.Labels.Add("VNS");
            categoryAxis.Labels.Add("Shock");
            categoryAxis.Labels.Add("Opto");

            model.Axes.Add(categoryAxis);
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
            plot.Model.PlotAreaBorderColor = OxyColors.Transparent;

            

            for (int i = 0; i < 10; i++)
            {
                var targetSeries = new OxyPlot.Series.IntervalBarSeries { Title = "Series " + i.ToString(), StrokeThickness = 1 };
                for (int j = 0; j < random.Next(0, i); j++)
                    targetSeries.Items.Add(new IntervalBarItem { CategoryIndex = j, Start = start.AddHours(random.NextDouble() + i).ToOADate(), End = end.AddHours(random.NextDouble() + i).ToOADate() });
                model.Series.Add(targetSeries);
            }

            LineAnnotation Line = new LineAnnotation()
            {
                StrokeThickness = 1,
                Color = OxyColors.Green,
                Type = LineAnnotationType.Vertical,
                X = start.AddHours(6).ToOADate(),
                Y = 0
            };

            plot.Model.Annotations.Add(Line);

        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
