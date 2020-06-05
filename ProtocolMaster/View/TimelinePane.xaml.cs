using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using ProtocolMaster.Component.Debug;
using ProtocolMaster.Component.Model;
using ProtocolMaster.Component.Model.Driver;
using ProtocolMaster.Component.Model.Interpreter;
using ProtocolMaster.Component.Model.Visualizer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            MenuItem newDriver = new MenuItem
            {
                Header = data.ToString()
            };
            newDriver.Resources.Add("data", data);
            newDriver.Click += new RoutedEventHandler(DriverClickHandler);
            DriverDropdown.Items.Add(newDriver);
        }

        public void DriverClickHandler(object sender, RoutedEventArgs e)
        {
            MenuItem src = e.Source as MenuItem;
            DriverMeta data = src.Resources["data"] as DriverMeta;

            App.Instance.Extensions.Drivers.Select(data);
            ShowSelectedDriver();
        }

        public void ShowSelectedDriver()
        {
            SelectedDriver.Header = "Selected: " + App.Instance.Extensions.Drivers.Selected.ToString();
        }

        public void ListInterpreter(InterpreterMeta data)
        {
            MenuItem newInterpreter = new MenuItem
            {
                Header = data.ToString()
            };
            newInterpreter.Resources.Add("data", data);
            newInterpreter.Click += new RoutedEventHandler(InterpreterClickHandler);
            InterpreterDropdown.Items.Add(newInterpreter);
        }

        public void InterpreterClickHandler(object sender, RoutedEventArgs e)
        {
            MenuItem src = e.Source as MenuItem;
            InterpreterMeta data = src.Resources["data"] as InterpreterMeta;
            App.Instance.Extensions.Interpreters.Select(data);
            ShowSelectedInterpreter();
        }

        public void ShowSelectedInterpreter()
        {
            SelectedInterpreter.Header = "Selected: " + App.Instance.Extensions.Interpreters.Selected.ToString();
        }

        public void ListVisualizer(VisualizerMeta data)
        {
            MenuItem newVis = new MenuItem
            {
                Header = data.Name + " " + data.Version
            };
            newVis.Resources.Add("data", data);
            newVis.Click += new RoutedEventHandler(VisualizerClickHandler);
            VisualizerDropdown.Items.Add(newVis);
        }

        public void VisualizerClickHandler(object sender, RoutedEventArgs e)
        {
            MenuItem src = e.Source as MenuItem;
            VisualizerMeta data = src.Resources["data"] as VisualizerMeta;
            App.Instance.Extensions.Visualizers.Select(data);
            ShowSelectedInterpreter();
        }

        public void ShowSelectedVisualizer()
        {
            SelectedVisualizer.Header = "Selected: " + App.Instance.Extensions.Visualizers.Selected.ToString();
        }

        DateTime start;
        public void Start_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
            ResetButton.IsEnabled = false;

            start = DateTime.Now;

            
            App.Instance.Extensions.Run();
        }

        public void Cancel_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            ResetButton.IsEnabled = true;

            bgWorker.CancelAsync();
            App.Instance.Extensions.Cancel();
        }

        public void Reset_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = true;
            CancelButton.IsEnabled = false;
            ResetButton.IsEnabled = false;

            Line.X = 0;
            plot.Model.InvalidatePlot(true);
        }

        private void GeneratePlotModel()
        {

        }

        LineAnnotation Line;
        CategoryAxis categoryAxis;
        public void LoadPlotData(List<DriveData> driveDataList)
        {
            IntervalBarSeries targetSeries = new IntervalBarSeries { Title = "Preload Series" };

            List<double> gridLines = new List<double>();

            foreach (DriveData data in driveDataList)
            {
                if (data.HasCategory)
                {
                    if (!categoryAxis.Labels.Contains(data.CategoryLabel))
                    {
                        categoryAxis.Labels.Add(data.CategoryLabel);
                        gridLines.Add(gridLines.Count);
                    }
                    targetSeries.Items.Add(new IntervalBarItem
                    {
                        CategoryIndex = categoryAxis.Labels.IndexOf(data.CategoryLabel),
                        Start = new DateTime(Convert.ToInt64(data.Arguments["TimeStartMs"]) * 10000).ToOADate(),
                        End = new DateTime(Convert.ToInt64(data.Arguments["TimeEndMs"]) * 10000).ToOADate()
                    });
                }
            }

            gridLines.CopyTo(categoryAxis.ExtraGridlines, 0);
            plot.Model.Series.Clear();
            plot.Model.Series.Add(targetSeries);

            plot.Model.InvalidatePlot(true);
            plot.ResetAllAxes();
        }
        private void SetUpPlot()
        {
            var model = new PlotModel
            {
                IsLegendVisible = false
            };

            model.Axes.Add(new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,
                TicklineColor = OxyColors.Gray,
                AxislineColor = OxyColors.Gray,
                MinorTicklineColor = OxyColors.Gray,
                TextColor = OxyColors.WhiteSmoke,
                TitleColor = OxyColors.WhiteSmoke,
                ExtraGridlineColor = OxyColors.Gray,
                MinorGridlineColor = OxyColors.Gray,
                MajorGridlineColor = OxyColors.Gray,
                StartPosition = 0
            });
            categoryAxis = new CategoryAxis()
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
                ExtraGridlines = new double[32]
            };
            model.Axes.Add(categoryAxis);

            

            model.DefaultColors = new List<OxyColor>
                {
                OxyColors.Gray,
                OxyColors.Gray,
                OxyColors.Gray,
                OxyColors.Gray
                };

            model.TextColor = OxyColors.White;
            model.PlotAreaBorderColor = OxyColors.Transparent;


            Line = new LineAnnotation()
            {
                StrokeThickness = 2,
                Color = OxyColors.Green,
                Type = LineAnnotationType.Vertical,
                X = 0,
                Y = 0
            };

            model.Annotations.Add(Line);

            plot.Model = model;
            Animate();
        }

        BackgroundWorker bgWorker = new BackgroundWorker();
        private void Animate()
        {
            bgWorker.DoWork +=
                new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.ProgressChanged +=
                new ProgressChangedEventHandler(bgWorker_ProgressChanged);
            bgWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
        }

        public void StartAnimation()
        {
            bgWorker.RunWorkerAsync();
        }
        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Log.Error(e.UserState.GetType().ToString());
            Line.X = (DateTime.Now.ToOADate() - start.ToOADate());
            plot.Model.InvalidatePlot(true);
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime workerstart = DateTime.Now;
            DateTime end = workerstart.AddSeconds(120);
            DateTime now = workerstart;
            DateTime nextFrame = workerstart.AddTicks(30000);
            while (now < end)
            {
                while (now < nextFrame)
                {
                    Thread.Sleep(25);
                    now = DateTime.Now;
                }

                bgWorker.ReportProgress(1);
                nextFrame = now.AddTicks(30000);

                if(this.bgWorker.CancellationPending == true)
                {
                    break;
                }
            }
        }
        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
