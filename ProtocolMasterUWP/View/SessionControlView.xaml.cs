using ProtocolMasterUWP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ProtocolMasterUWP.View
{
    public sealed partial class SessionControlView : UserControl
    {
        internal SessionControlViewModel SessionControl { get; set; }
        public SessionControlView()
        {
            this.InitializeComponent();
        }

        private void SelectOpen_Click(object sender, RoutedEventArgs e) => SessionControl.OpenSelection();
        private void Preview_Click(object sender, RoutedEventArgs e) => SessionControl.Preview();
        private void Start_Click(object sender, RoutedEventArgs e) => SessionControl.Start();
        private void Stop_Click(object sender, RoutedEventArgs e) => SessionControl.Stop();
        private void Reset_Click(object sender, RoutedEventArgs e) => SessionControl.Reset();
    }
}
