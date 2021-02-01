using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using ProtocolMasterCore.Protocol;
using ProtocolMasterWPF.Helpers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProtocolMasterWPF.View
{
    /// <summary>
    /// Interaction logic for TimelineView.xaml
    /// </summary>
    public partial class TimelineView : UserControl
    {
        public TimelineView()
        {
            InitializeComponent();
            SetUpPlot();
        }
        // Plot properties
        LineAnnotation Line { get; set; }
        CategoryAxis VerticalCategoryAxis { get; set; }
        DateTimeAxis HorizontalTimeAxis { get; set; }
        // Animator Properties
        Task AnimatorTask { get; set; }
        DateTime start;
        Progress<int> animationProgress;
        CancellationTokenSource tokenSource;
        private void SetUpPlot()
        {
            // Generate Model
            PlotModel model = StyledPlotModel();
            // Generate Axes
            HorizontalTimeAxis = StyledDateTimeAxis();
            VerticalCategoryAxis = StyledCategoryAxis();
            // Add Axes
            model.Axes.Add(HorizontalTimeAxis);
            model.Axes.Add(VerticalCategoryAxis);
            // Generate Time Annotation
            Line = StyledLineAnnotation();
            // Add Time Annotation
            model.Annotations.Add(Line);
            Plot.Model = model;
            ResetPlot();
        }
        private void ResetPlot()
        {
            PrepAnimator();
            Plot.Model.Series.Clear();
            VerticalCategoryAxis.Labels.Clear();
            HorizontalTimeAxis.AbsoluteMinimum = -0.00001;
            HorizontalTimeAxis.Minimum = HorizontalTimeAxis.AbsoluteMinimum;
            HorizontalTimeAxis.AbsoluteMaximum = 0.99;

            VerticalCategoryAxis.AbsoluteMinimum = -0.6;
            VerticalCategoryAxis.AbsoluteMaximum = 0.6;
            VerticalCategoryAxis.MaximumRange = VerticalCategoryAxis.AbsoluteMaximum - VerticalCategoryAxis.AbsoluteMinimum;
            VerticalCategoryAxis.MinimumRange = VerticalCategoryAxis.AbsoluteMaximum - VerticalCategoryAxis.AbsoluteMinimum;
            Plot.Model.InvalidatePlot(true);
        }
        public void LoadPlotData(List<ProtocolEvent> eventList)
        {
            List<IntervalBarSeries> allSeries;
            List<string> labels;
            List<double> gridLines;
            CategoryNode.GeneratePlotData(CategoryNode.BuildTrees(eventList), out allSeries, out labels, out gridLines);

            // GENERATE LABELS, GRIDLINES, ETC. FROM TREE!
            VerticalCategoryAxis.Labels.Clear();
            VerticalCategoryAxis.Labels.AddRange(labels);
            gridLines.CopyTo(VerticalCategoryAxis.ExtraGridlines, 0);

            Plot.Model.Series.Clear();
            foreach (IntervalBarSeries series in allSeries)
                Plot.Model.Series.Add(series);
            Plot.ResetAllAxes();
            HorizontalTimeAxis.Minimum = 0;
            VerticalCategoryAxis.AbsoluteMaximum = gridLines[gridLines.Count - 1] + 0.6;
            VerticalCategoryAxis.MaximumRange = VerticalCategoryAxis.AbsoluteMaximum - VerticalCategoryAxis.AbsoluteMinimum;
            VerticalCategoryAxis.MinimumRange = VerticalCategoryAxis.AbsoluteMaximum - VerticalCategoryAxis.AbsoluteMinimum;
            Plot.Model.InvalidatePlot(true);
        }
        private PlotModel StyledPlotModel()
        {
            PlotModel styledModel = new PlotModel
            {
                IsLegendVisible = false
            };
            // Set Model Colors
            styledModel.DefaultColors = new List<OxyColor>
                {
                OxyColors.DarkRed,
                OxyColors.DodgerBlue,
                OxyColors.Green,
                OxyColors.Yellow
                };
            styledModel.TextColor = OxyColors.White;
            styledModel.PlotAreaBorderColor = OxyColors.Transparent;
            styledModel.Background = OxyColors.Transparent;
            // Change Mouse Bindings
            Plot.ActualController.UnbindMouseDown(OxyMouseButton.Left);
            Plot.ActualController.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
            Plot.ActualController.UnbindMouseDown(OxyMouseButton.Right);
            Plot.ActualController.BindMouseDown(OxyMouseButton.Right, PlotCommands.ResetAt);
            return styledModel;
        }
        private DateTimeAxis StyledDateTimeAxis()
        {
            return new DateTimeAxis()
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
                StartPosition = 0,
            };
        }
        private CategoryAxis StyledCategoryAxis()
        {
            return new CategoryAxis()
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
                GapWidth = 0.0f,
                ExtraGridlines = new double[32],
            };
        }
        private LineAnnotation StyledLineAnnotation()
        {
            return new LineAnnotation()
            {
                StrokeThickness = 1.5,
                Color = OxyColors.Green,
                Type = LineAnnotationType.Vertical,
                LineStyle = LineStyle.Solid,
                X = 0,
                Y = 0
            };
        }
        public void PrepAnimator()
        {
            Line.X = 0;
            animationProgress = new Progress<int>();
            tokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = tokenSource.Token;
            animationProgress.ProgressChanged += AnimatorProgress;
            AnimatorTask = new Task(() =>
            {
                AnimatorLoop(animationProgress, cancelToken);
            }, cancelToken);
        }
        public void StartAnimator(object sender, EventArgs e)
        {
            Line.Color = OxyColors.Red;
            AnimatorTask.Start();
            start = DateTime.Now;
        }
        public void EndAnimator(object sender, EventArgs e)
        {
            tokenSource.Cancel();
            Line.Color = OxyColors.Green;
            Plot.InvalidatePlot();
        }
        void AnimatorProgress(object sender, int e)
        {
            if (!tokenSource.IsCancellationRequested)
            {
                Line.X = (DateTime.Now.ToOADate() - start.ToOADate());
                Plot.InvalidatePlot();
            }
        }
        void AnimatorLoop(IProgress<int> progress, CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                Thread.Sleep(192);
                progress.Report(1);
            }
            progress.Report(0);
        }
    }
}
