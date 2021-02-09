using Microsoft.Toolkit.Wpf.UI.XamlHost;
using ProtocolMasterCore.Utility;
using ProtocolMasterWPF.Model;
using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace ProtocolMasterWPF.ViewModel
{
    internal class CameraViewModel : ViewModelBase
    {
        Camera Cam { get; set; }
        internal CameraViewModel()
        {
            Cam = new Camera();
            GetUwpCaptureElement();
        }
        public CaptureElement CapElement { get; set; }
        public WindowsXamlHost XamlHostCaptureElement { get; set; }
        private void GetUwpCaptureElement()
        {
            XamlHostCaptureElement = new WindowsXamlHost
            {
                InitialTypeName = "Windows.UI.Xaml.Controls.CaptureElement"
            };
            XamlHostCaptureElement.ChildChanged += XamlHost_ChildChangedAsync;
        }
        private async void XamlHost_ChildChangedAsync(object sender, EventArgs e)
        {
            var windowsXamlHost = (WindowsXamlHost)sender;

            var captureElement = (CaptureElement)windowsXamlHost.Child;
            if (captureElement != null)
            {
                CapElement = captureElement;
                CapElement.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;
                CapElement.Source = Cam.MediaCap;
                Cam.StartPreview();
            }
        }
    }
}

