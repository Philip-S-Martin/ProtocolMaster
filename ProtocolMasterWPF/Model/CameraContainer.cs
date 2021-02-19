using ProtocolMasterCore.Utility;
using ProtocolMasterWPF.Properties;
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
            InitDefaultDevices();
            ResetCam();
        }
        public Camera Cam
        {
            get => _cam;
            private set
            {
                _cam = value;
                NotifyProperty();
            }
        }
        private Camera _cam;
        public DeviceInformation VideoDevice
        {
            get => _videoDevice;
            set
            {
                _videoDevice = value;
                NotifyProperty();
                ResetCam();
                if (value.Id != Settings.Default.CameraID)
                {
                    Settings.Default.CameraID = value.Id;
                    Settings.Default.Save();
                }
            }
        }
        private DeviceInformation _videoDevice;
        public DeviceInformation AudioDevice
        {
            get => _audioDevice;
            set
            {
                _audioDevice = value;
                NotifyProperty();
                ResetCam();
                if(value.Id != Settings.Default.MicrophoneID)
                {
                    Settings.Default.MicrophoneID = value.Id;
                    Settings.Default.Save();
                }
            }
        }
        private DeviceInformation _audioDevice;
        public void StartRecord() => Cam.StartRecord();
        public void StopRecord() => Cam.StopRecord();

        private void ResetCam()
        {
            Cam = new Camera(VideoDevice, AudioDevice);
        }

        public void InitDefaultDevices()
        {
            try
            {
                _videoDevice = MediaDevices.Instance.VideoDeviceByID(Settings.Default.CameraID);
            }
            catch (Exception e)
            {
                Log.Error($"Default Camera could not be seleted, exception: {e}");
                VideoDevice = MediaDevices.Instance.VideoDevices.First();
            }
            try
            {
                _audioDevice = MediaDevices.Instance.AudioDeviceByID(Settings.Default.MicrophoneID);
            }
            catch (Exception e)
            {
                Log.Error($"Default Microphone could not be seleted, exception: {e}");
                AudioDevice = MediaDevices.Instance.AudioDevices.First();
            }
        }
    }
}
