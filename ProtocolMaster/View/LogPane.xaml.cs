﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ProtocolMaster.View
{
    /// <summary>
    /// Interaction logic for LogPane.xaml
    /// </summary>
    public partial class LogPane : Page
    {
        public LogPane()
        {
            InitializeComponent();
        }

        // Log Functions
        public void Log(string logString)
        {
            Paragraph addLog = new Paragraph();
            addLog.Inlines.Add(logString);
            LogDocument.Blocks.Add(addLog);
        }

        public void Log_Folder_Click(object sender, RoutedEventArgs e)
        {
            ProtocolMaster.Component.Debug.Log.Instance.OpenFolder();
        }
        public void Log_Test(object sender, RoutedEventArgs e)
        {
            ProtocolMaster.Component.Debug.Log.Error("Test");
        }
    }
}
