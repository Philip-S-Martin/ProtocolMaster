using ProtocolMasterUWP.Observable;
using ProtocolMasterUWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public sealed partial class SessionView : UserControl
    {
        SessionControlViewModel SessionControl;
        
        public SessionView()
        {
            this.InitializeComponent();
            SessionControl = new SessionControlViewModel();
            PrimaryControls.SessionControl = SessionControl;
            SelectionPane.SessionControl = SessionControl;
        }

        private void SplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            if (SessionControl.IsSelecting)
            {
                args.Cancel = true;
            }
        }
    }
}
