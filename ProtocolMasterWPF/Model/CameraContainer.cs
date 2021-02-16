using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace ProtocolMasterWPF.Model
{
    internal class CameraContainer : Observable
    {
        public CameraContainer()
        {
            RefreshDevices();
            _videoDevice = VideoDevices.First();
            _audioDevice = AudioDevices.First();
            ResetCam();
        }
        public Camera Cam { get => _cam; private set { _cam = value; NotifyProperty(); } }
        private Camera _cam;
        public DeviceInformation VideoDevice { get => _videoDevice; set { _videoDevice = value; NotifyProperty(); ResetCam(); } }
        private DeviceInformation _videoDevice;
        public DeviceInformation AudioDevice { get => _audioDevice; set { _audioDevice = value; NotifyProperty(); ResetCam(); } }
        private DeviceInformation _audioDevice;
        public void StartRecord() => Cam.StartRecord();
        public void StopRecord() => Cam.StopRecord();
        private DeviceInformationCollection _videoDevices;
        public DeviceInformationCollection VideoDevices
        {
            get => _videoDevices;
            set
            {
                _videoDevices = value;
                NotifyProperty();
            }
        }
        public void RefreshDevices()
        {
            Task<DeviceInformationCollection> task = DeviceInformation.FindAllAsync(DeviceClass.VideoCapture).AsTask();
            task.Wait();
            VideoDevices = task.Result;
            task.Dispose();

            task = DeviceInformation.FindAllAsync(DeviceClass.AudioCapture).AsTask();
            task.Wait();
            AudioDevices = task.Result;
        }
        private DeviceInformationCollection _audioDevices;
        public DeviceInformationCollection AudioDevices
        {
            get => _audioDevices;
            set
            {
                _audioDevices = value;
                NotifyProperty();
            }
        }
        private void ResetCam()
        {
            Cam = new Camera(VideoDevice, AudioDevice);
        }
    }
}
