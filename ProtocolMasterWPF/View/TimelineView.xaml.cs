using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using ProtocolMasterCore.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        DateTime start;
        Task bgWorker;
        Progress<int> animationProgress;
        CancellationTokenSource tokenSource;
        private void ResetPlot()
        {
            Line.X = 0;
            plot.Model.Series.Clear();
            categoryAxis.Labels.Clear();
            dateTimeAxis.AbsoluteMinimum = -0.00001;
            dateTimeAxis.Minimum = dateTimeAxis.AbsoluteMinimum;
            dateTimeAxis.AbsoluteMaximum = 0.9999;

            categoryAxis.AbsoluteMinimum = -0.6;
            categoryAxis.AbsoluteMaximum = 0.6;
            categoryAxis.MaximumRange = categoryAxis.AbsoluteMaximum - categoryAxis.AbsoluteMinimum;
            categoryAxis.MinimumRange = categoryAxis.AbsoluteMaximum - categoryAxis.AbsoluteMinimum;
            plot.Model.InvalidatePlot(true);
        }

        static OxyColor[] pallete = { OxyColors.DarkRed, OxyColors.DodgerBlue, OxyColors.Green };

        internal class CategoryNode
        {
            public string Name { get; private set; }
            public CategoryNode Parent { get; private set; }
            public List<CategoryNode> Children { get; private set; }
            public IntervalBarSeries Series { get; private set; }
            public CategoryNode(string name)
            {
                this.Name = name;
                this.Children = new List<CategoryNode>();
                Series = new IntervalBarSeries() { Title = name, StrokeThickness = 1.5, StrokeColor = OxyColors.Gray, FillColor = OxyColor.FromArgb(255, 16, 16, 16), BarWidth = 1.0, ToolTip = name };
            }
            public CategoryNode(string name, CategoryNode parent) : this(name)
            {
                this.Parent = parent;
                Series.BarWidth = 0.35;
                parent.Children.Add(this);
            }
            public void SetSeriesData(int categoryIndex, OxyColor color)
            {
                Series.StrokeColor = OxyColor.FromArgb(255, (byte)((color.R * 3 + 255) / 4), (byte)((color.G * 3 + 255) / 4), (byte)((color.B * 3 + 255) / 4));
                foreach (IntervalBarItem item in Series.Items)
                {
                    item.CategoryIndex = categoryIndex;
                    item.Color = color;
                }
            }
            public static void GeneratePlotData(List<CategoryNode> rootNodes, out List<IntervalBarSeries> allSeries, out List<string> labels, out List<double> gridLines)
            {
                double linePos = 0.0;
                int fullIndex = 0;
                int palleteIndex = 0;
                allSeries = new List<IntervalBarSeries>();
                labels = new List<string>();
                gridLines = new List<double>();
                for (int i = 0; i < rootNodes.Count; i++)
                {
                    CategoryNode root = rootNodes[i];
                    root.SetSeriesData(fullIndex++, pallete[palleteIndex]);
                    allSeries.Add(root.Series);
                    labels.Add(root.Name);
                    gridLines.Add(linePos++);
                    Stack<CategoryNode> subtree = new Stack<CategoryNode>(root.Children);
                    while (subtree.Count != 0)
                    {
                        CategoryNode subNode = subtree.Pop();
                        subNode.SetSeriesData(fullIndex++, pallete[palleteIndex]);
                        allSeries.Add(subNode.Series);
                        labels.Add(subNode.Name);
                        gridLines.Add(linePos++);
                        foreach (CategoryNode child in subNode.Children)
                            subtree.Push(child);
                    }
                    if (i < rootNodes.Count - 1)
                    {
                        palleteIndex = (palleteIndex + 1) % pallete.Length;
                        linePos++;
                        fullIndex++;
                        labels.Add("");
                    }
                }
            }
            // The biggest ugliest mess in the universe!
            public static List<CategoryNode> BuildTrees(List<ProtocolEvent> eventList)
            {
                Dictionary<string, CategoryNode> nodeDictionary = new Dictionary<string, CategoryNode>();
                List<CategoryNode> rootNodes = new List<CategoryNode>();
                Queue<ProtocolEvent> eventQueue = new Queue<ProtocolEvent>(eventList);
                if (eventList != null)
                {
                    while (eventQueue.Count != 0)
                    {
                        ProtocolEvent plotEvent = eventQueue.Dequeue();
                        if (plotEvent.HasCategory())
                        {
                            CategoryNode targetNode;
                            if (plotEvent.HasParent())
                            {
                                if (nodeDictionary.ContainsKey(plotEvent.ParentLabel))
                                {
                                    if (!nodeDictionary.TryGetValue(plotEvent.FullLabel(), out targetNode))
                                    {
                                        targetNode = new CategoryNode(plotEvent.CategoryLabel, nodeDictionary[plotEvent.ParentLabel]);
                                        nodeDictionary.Add(plotEvent.FullLabel(), targetNode);
                                    }
                                }
                                else
                                {
                                    eventQueue.Enqueue(plotEvent);
                                    continue;
                                }
                            }
                            else
                            {
                                if (!nodeDictionary.TryGetValue(plotEvent.FullLabel(), out targetNode))
                                {
                                    targetNode = new CategoryNode(plotEvent.CategoryLabel);
                                    nodeDictionary.Add(plotEvent.FullLabel(), targetNode);
                                    rootNodes.Add(targetNode);
                                }
                            }
                            targetNode.Series.Items.Add(new IntervalBarItem
                            {
                                Start = new DateTime(Convert.ToInt64(plotEvent.Arguments["TimeStartMs"]) * 10000).ToOADate(),
                                End = new DateTime(Convert.ToInt64(plotEvent.Arguments["TimeEndMs"]) * 10000).ToOADate()
                            });
                        }
                    }
                }
                return rootNodes;
            }
        }

        LineAnnotation Line;
        CategoryAxis categoryAxis;
        DateTimeAxis dateTimeAxis;
        public void LoadPlotData(List<ProtocolEvent> eventList)
        {
            List<IntervalBarSeries> allSeries;
            List<string> labels;
            List<double> gridLines;
            CategoryNode.GeneratePlotData(CategoryNode.BuildTrees(eventList), out allSeries, out labels, out gridLines);

            // GENERATE LABELS, GRIDLINES, ETC. FROM TREE!
            categoryAxis.Labels.Clear();
            categoryAxis.Labels.AddRange(labels);
            gridLines.CopyTo(categoryAxis.ExtraGridlines, 0);

            plot.Model.Series.Clear();
            foreach (IntervalBarSeries series in allSeries)
                plot.Model.Series.Add(series);
            plot.ResetAllAxes();
            dateTimeAxis.Minimum = 0;
            categoryAxis.AbsoluteMaximum = gridLines[gridLines.Count - 1] + 0.6;
            categoryAxis.MaximumRange = categoryAxis.AbsoluteMaximum - categoryAxis.AbsoluteMinimum;
            categoryAxis.MinimumRange = categoryAxis.AbsoluteMaximum - categoryAxis.AbsoluteMinimum;
            plot.Model.InvalidatePlot(true);
        }
        private void SetUpPlot()
        {
            var model = new PlotModel
            {
                IsLegendVisible = false
            };

            dateTimeAxis = new DateTimeAxis()
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
            model.Axes.Add(dateTimeAxis);
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
                GapWidth = 0.0f,
                ExtraGridlines = new double[32],
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
            model.Background = OxyColors.Transparent;

            plot.ActualController.UnbindMouseDown(OxyMouseButton.Left);
            plot.ActualController.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
            plot.ActualController.UnbindMouseDown(OxyMouseButton.Right);
            plot.ActualController.BindMouseDown(OxyMouseButton.Right, PlotCommands.ResetAt);

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
            dateTimeAxis.Minimum = 0;
            plot.Model = model;

            ResetPlot();
        }
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
                while (now < nextFrame)
                {
                    Thread.Sleep(5);
                    now = DateTime.Now;
                }
                progress.Report(1);
                nextFrame = now.AddMilliseconds(200);
            }
            progress.Report(0);
        }
    }
}
