using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.UI.Xaml.Controls;

namespace ProtocolMasterWPF.ViewModel
{
    internal class CameraViewModel : ViewModelBase
    {
        internal CameraViewModel()
        {
            GetUwpCaptureElement();
        }

        private MediaCapture _mediaCapture;

        public MediaCapture MediaCapture
        {
            get
            {
                if (_mediaCapture == null)
                    _mediaCapture = new MediaCapture();
                return _mediaCapture;
            }
            set
            {
                _mediaCapture = value;
                OnPropertyChanged(nameof(MediaCapture));
            }
        }


        public CaptureElement CapElement { get; set; }

        public WindowsXamlHost XamlHostCaptureElement { get; set; }

        /// <summary>
        /// Create / Host UWP CaptureElement
        /// </summary>
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

                try
                {
                    await StartPreviewAsync();
                }
                catch (Exception)
                {

                }
            }
        }

        private async Task StartPreviewAsync()
        {
            try
            {
                await MediaCapture.InitializeAsync(new MediaCaptureInitializationSettings());
            }
            catch (UnauthorizedAccessException)
            {
                //_logger.Info($"The app was denied access to the camera \n {ex}");
                return;
            }

            try
            {
                CapElement.Source = MediaCapture;
                await MediaCapture.StartPreviewAsync();
            }
            catch (System.IO.FileLoadException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

        }
    }
}

