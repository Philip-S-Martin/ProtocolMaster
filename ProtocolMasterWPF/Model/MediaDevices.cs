using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace ProtocolMasterWPF.Model
{
    internal class MediaDevices : Observable
    {
        private static MediaDevices instance = new MediaDevices();
        public static MediaDevices Instance { get => instance; }
        static MediaDevices() { }
        private MediaDevices()
        {
            RefreshDevices();
        }
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
        public DeviceInformation AudioDeviceByID(string id)
        {
            return AudioDevices.First(a => a.Id == id);
        }
        public DeviceInformation VideoDeviceByID(string id)
        {
            return VideoDevices.First(a => a.Id == id);
        }
    }
}
