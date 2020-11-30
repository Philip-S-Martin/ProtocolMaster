using MahApps.Metro.Controls;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using ProtocolMaster.Model.Debug;
using ProtocolMaster.Model.Protocol;
using ProtocolMaster.Model.Protocol.Driver;
using ProtocolMaster.Model.Protocol.Interpreter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProtocolMaster.View
{
    /// <summary>
    /// Interaction logic for Timeline.xaml
    /// </summary>
    public partial class Timeline : UserControl
    {
        public Timeline()
        {
            InitializeComponent();
            SetUpPlot();
            App.Instance.ExtensionSystem.InterpreterManager.OnOptionsLoaded += LoadInterpreters;
            App.Instance.ExtensionSystem.DriverManager.OnOptionsLoaded += LoadDrivers;

            App.Instance.ExtensionSystem.DriverManager.OnProtocolStart += StartAnimation;
            App.Instance.ExtensionSystem.DriverManager.OnProtocolEnd += EndAnimation;
        }
        public void LoadDrivers(object sender, EventArgs e)
        {
            DriverDropdown.Items.Clear();
            foreach (DriverMeta meta in App.Instance.ExtensionSystem.DriverManager.Options)
            {
                ListDriver(meta);
            }
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

            App.Instance.ExtensionSystem.DriverManager.Selected = data;
            ShowSelectedDriver();
        }

        public void ShowSelectedDriver()
        {
            SelectedDriver.Header = "Selected: " + App.Instance.ExtensionSystem.DriverManager.Selected.ToString();
        }

        public void LoadInterpreters(object sender, EventArgs e)
        {
            InterpreterDropdown.Items.Clear();
            foreach (InterpreterMeta meta in App.Instance.ExtensionSystem.InterpreterManager.Options)
            {
                ListInterpreter(meta);
            }
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
            App.Instance.ExtensionSystem.InterpreterManager.Selected = data;
            ShowSelectedInterpreter();
        }

        public void ShowSelectedInterpreter()
        {
            SelectedInterpreter.Header = "Selected: " + App.Instance.ExtensionSystem.InterpreterManager.Selected.ToString();
        }

        DateTime start;

        public void Load_Click(object sender, RoutedEventArgs e)
        {
            LoadButton.IsEnabled = true;
            StartButton.IsEnabled = true;
            CancelButton.IsEnabled = false;
            ResetButton.IsEnabled = true;
            App.Instance.ExtensionSystem.Interpret(App.Window.DriveView.GetSelectedItemID());
        }
        public void Start_Click(object sender, RoutedEventArgs e)
        {
            LoadButton.IsEnabled = false;
            StartButton.IsEnabled = false;
            CancelButton.IsEnabled = true;
            ResetButton.IsEnabled = false;

            App.Instance.ExtensionSystem.Run();
        }

        public void Cancel_Click(object sender, RoutedEventArgs e)
        {
            LoadButton.IsEnabled = true;
            StartButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            ResetButton.IsEnabled = true;
            
            App.Instance.ExtensionSystem.End();
        }

        public void Reset_Click(object sender, RoutedEventArgs e)
        {
            LoadButton.IsEnabled = true;
            StartButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            ResetButton.IsEnabled = false;

            App.Instance.ExtensionSystem.Reset();
            Line.X = 0;
            plot.Model.Series.Clear();
            categoryAxis.Labels.Clear();
            plot.Model.InvalidatePlot(true);
        }

        private void GeneratePlotModel()
        {

        }

        LineAnnotation Line;
        CategoryAxis categoryAxis;
        public void LoadPlotData(List<ProtocolEvent> driveDataList)
        {
            IntervalBarSeries targetSeries = new IntervalBarSeries { Title = "Preload Series" };

            List<double> gridLines = new List<double>();

            categoryAxis.Labels.Clear();
            if (driveDataList != null)
            {
                foreach (ProtocolEvent data in driveDataList)
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
            }

            gridLines.CopyTo(categoryAxis.ExtraGridlines, 0);
            targetSeries.StrokeThickness = 1.5;
            targetSeries.StrokeColor = OxyColors.Gray;
            targetSeries.FillColor = OxyColor.FromArgb(255, 16, 16, 16);

            plot.Model.Series.Clear();
            plot.Model.Series.Add(targetSeries);



            plot.ResetAllAxes();
            plot.Model.Axes[0].Minimum = 0;
            plot.Model.InvalidatePlot(true);
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
                AxislineThickness = 1.5,
                ExtraGridlineThickness = 1.5,
                MajorGridlineThickness = 1.5,
                MinorGridlineThickness = 1.5,
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
                AxislineThickness = 1.5,
                ExtraGridlineThickness = 1.5,
                MajorGridlineThickness = 1.5,
                MinorGridlineThickness = 1.5,
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
                StrokeThickness = 1.5,
                Color = OxyColors.Green,
                Type = LineAnnotationType.Vertical,
                LineStyle = LineStyle.Solid,
                X = 0,
                Y = 0
            };

            model.Annotations.Add(Line);

            plot.Model = model;
            Animate();
        }

        Task bgWorker;
        private void Animate()
        {
            
        }
        Progress<int> animationProgress;
        CancellationTokenSource tokenSource;
        public void StartAnimation(object sender, EventArgs e)
        {

            animationProgress = new Progress<int>();
            tokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = tokenSource.Token;
            animationProgress.ProgressChanged += bgWorker_ProgressChanged;
            bgWorker = new Task(() =>
            {
                bgWorker_DoWork(animationProgress, cancelToken);
            }, cancelToken);
            bgWorker.Start();
            start = DateTime.Now;
        }
        public void EndAnimation(object sender, EventArgs e)
        {
            tokenSource.Cancel();
        }

        void bgWorker_ProgressChanged(object sender, int e)
        {
            //Log.Error(e.UserState.GetType().ToString());
            Line.X = (DateTime.Now.ToOADate() - start.ToOADate());
            plot.InvalidatePlot();
        }

        void bgWorker_DoWork(IProgress<int> progress, CancellationToken cancelToken)
        {
            DateTime workerstart = DateTime.Now;
            DateTime end = workerstart.AddSeconds(120);
            DateTime now = workerstart;
            DateTime nextFrame = workerstart.AddTicks(250000);
            while (now < end && !cancelToken.IsCancellationRequested)
            {
                /*
                while (now < nextFrame)
                {
                    Thread.Sleep(7);
                    now = DateTime.Now;
                }*/
                Thread.Sleep(40);
                progress.Report(1);
                nextFrame = now.AddMilliseconds(35);
            }
            progress.Report(0);
        }
        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }
    }
}
