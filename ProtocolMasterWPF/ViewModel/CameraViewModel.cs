using Microsoft.Toolkit.Wpf.UI.XamlHost;
using ProtocolMasterCore.Utility;
using ProtocolMasterWPF.Model;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace ProtocolMasterWPF.ViewModel
{
    internal class CameraViewModel : ViewModelBase
    {
        CameraContainer CamContainer { get; set; }
        internal CameraViewModel(CameraContainer cam)
        {
            CamContainer = cam;
            CamContainer.PropertyChanged += CameraChangedEvent;
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
                if (CamContainer.VideoDevice != null)
                {
                    CapElement.Source = CamContainer.Cam.MediaCap;
                    CamContainer.Cam.StartPreview();
                }
            }
        }
        private async void CameraChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Cam")
            {
                var cameraContainer = (CameraContainer)sender;
                if (CapElement != null && cameraContainer != null)
                {
                    if (CapElement.Source != null) await CapElement.Source.StopPreviewAsync();
                    if (CamContainer.VideoDevice != null)
                    {
                        CapElement.Source = CamContainer.Cam.MediaCap;
                        CamContainer.Cam.StartPreview();
                    }
                }
            }
        }
    }
}

