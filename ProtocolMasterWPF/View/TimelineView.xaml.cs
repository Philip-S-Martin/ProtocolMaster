using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using ProtocolMasterCore.Protocol;
using ProtocolMasterWPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void ResetPlot()
        {
            Line.X = 0;
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
        public void LoadPlotDataInUIThread(List<ProtocolEvent> eventList)=>App.Current.Dispatcher.Invoke(() => LoadPlotData(eventList));
        
        public void LoadPlotData(List<ProtocolEvent> eventList)
        {
            Plot.Model.Series.Clear();

            List<CategoryNode> nodes = CategoryNode.BuildTrees(eventList);
            if (nodes != null)
            {
                List<IntervalBarSeries> allSeries;
                List<string> labels;
                List<double> gridLines;
                CategoryNode.GeneratePlotData(nodes, out allSeries, out labels, out gridLines);

                // GENERATE LABELS, GRIDLINES, ETC. FROM TREE!
                VerticalCategoryAxis.Labels.Clear();
                VerticalCategoryAxis.Labels.AddRange(labels);
                VerticalCategoryAxis.ExtraGridlines = gridLines.ToArray();

                foreach (IntervalBarSeries series in allSeries)
                    Plot.Model.Series.Add(series);
            }
            Plot.ResetAllAxes();
            HorizontalTimeAxis.Minimum = HorizontalTimeAxis.AbsoluteMinimum;
            VerticalCategoryAxis.AbsoluteMaximum = VerticalCategoryAxis.ExtraGridlines.Max() + 0.6f;
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
        public void StartTime() => App.Current.Dispatcher.Invoke(() => StartTimeLocal());
        private void StartTimeLocal()
        {
            Line.Color = OxyColors.Red;
        }
        public void StopTime() => App.Current.Dispatcher.Invoke(() => StopTimeLocal());
        private void StopTimeLocal()
        {
            Line.Color = OxyColors.Green;
            Plot.InvalidatePlot();
        }
        public void UpdateTime(double elapsed, double duration) => App.Current.Dispatcher.Invoke(() => UpdateTimeLocal(elapsed, duration));
        private void UpdateTimeLocal(double elapsed, double duration)
        {
            Line.X = elapsed;
            Plot.InvalidatePlot();
        }
    }
}
