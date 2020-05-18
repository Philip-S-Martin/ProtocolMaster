using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using ProtocolMaster.Component.Debug;
using ProtocolMaster.Component.Model;
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

        public void ListDriver(DriverMeta data)
        {
            MenuItem newDriver = new MenuItem();
            newDriver.Header = data.Name + " " + data.Version;
            newDriver.Resources.Add("data", data);
            newDriver.Click += new RoutedEventHandler(DriverClickHandler);
            DriverDropdown.Items.Add(newDriver);
        }

        public void DriverClickHandler(object sender, RoutedEventArgs e)
        {
            MenuItem src = e.Source as MenuItem;
            DriverMeta data = src.Resources["data"] as DriverMeta;
            SelectedDriver.Header = "Selected: " + data.Name + " " + data.Version;
            App.Instance.Extensions.Drivers.Select(data);
        }

        public void ListInterpreter(InterpreterMeta data)
        {
            MenuItem newInterpreter = new MenuItem();
            newInterpreter.Header = data.Name + " " + data.Version;
            newInterpreter.Resources.Add("data", data);
            newInterpreter.Click += new RoutedEventHandler(InterpreterClickHandler);
            InterpreterDropdown.Items.Add(newInterpreter);
        }

        public void InterpreterClickHandler(object sender, RoutedEventArgs e)
        {
            MenuItem src = e.Source as MenuItem;
            InterpreterMeta data = src.Resources["data"] as InterpreterMeta;
            SelectedInterpreter.Header = "Selected: " + data.Name + " " + data.Version;
        }

        public void ListVisualizer(VisualizerMeta data)
        {
            MenuItem newVis = new MenuItem();
            newVis.Header = data.Name + " " + data.Version;
            newVis.Resources.Add("data", data);
            newVis.Click += new RoutedEventHandler(VisualizerClickHandler);
            VisualizerDropdown.Items.Add(newVis);
        }

        public void VisualizerClickHandler(object sender, RoutedEventArgs e)
        {
            MenuItem src = e.Source as MenuItem;
            VisualizerMeta data = src.Resources["data"] as VisualizerMeta;
            SelectedVisualizer.Header = "Selected: " + data.Name + " " + data.Version;
        }

        public void Start_Click(object sender, RoutedEventArgs e)
        {
            App.Instance.Extensions.Drivers.Run();
        }

        public void Stop_Click(object sender, RoutedEventArgs e)
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
                StrokeThickness = 2,
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
