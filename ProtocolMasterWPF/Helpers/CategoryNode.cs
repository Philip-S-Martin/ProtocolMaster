using OxyPlot;
using OxyPlot.Series;
using ProtocolMasterCore.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolMasterWPF.Helpers
{
    internal class CategoryNode
    {
        static OxyColor[] pallete = { OxyColors.DarkRed, OxyColors.DodgerBlue, OxyColors.Green };
        public string Name { get; private set; }
        public CategoryNode Parent { get; private set; }
        public List<CategoryNode> Children { get; private set; }
        public IntervalBarSeries Series { get; private set; }
        public CategoryNode(string name)
        {
            this.Name = name;
            this.Children = new List<CategoryNode>();
            Series = new IntervalBarSeries() 
            { 
                Title = name, 
                StrokeThickness = 1.5, 
                StrokeColor = OxyColors.Gray, 
                FillColor = OxyColor.FromArgb(255, 16, 16, 16), 
                BarWidth = 1.0, 
                ToolTip = name };
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
}
