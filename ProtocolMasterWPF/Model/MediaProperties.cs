using ProtocolMasterCore.Protocol;
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
    internal class MediaProperties : Observable
    {
        public MediaProperties()
        {
            InitDefaultDevices();
            ResetCam();
        }
        public MediaRecorder Recorder
        {
            get => _recorder;
            private set
            {
                _recorder = value;
                NotifyProperty();
            }
        }
        private MediaRecorder _recorder;
        public DeviceInformation VideoDevice
        {
            get => _videoDevice;
            set
            {
                _videoDevice = value;
                ResetCam();
                NotifyProperty();
                if (value != null)
                {
                    if (value.Id != Settings.Default.CameraID)
                    {
                        Settings.Default.CameraID = value.Id;
                        Settings.Default.Save();
                    }
                }
                else
                {
                    Settings.Default.CameraID = null;
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
                ResetCam();
                NotifyProperty();
                if (value != null)
                {
                    if (value.Id != Settings.Default.MicrophoneID)
                    {
                        Settings.Default.MicrophoneID = value.Id;
                        Settings.Default.Save();
                    }
                }
                else
                {
                    Settings.Default.MicrophoneID = null;
                    Settings.Default.Save();
                }
            }
        }
        private DeviceInformation _audioDevice;
        private uint _quality;
        public uint Quality
        {
            get => _quality;
            set
            {
                _quality = value;
                NotifyProperty();
                Settings.Default.CameraQuality = value;
                Settings.Default.Save();
            }
        }
        

        string label = "Recording";
        public void SetLabel(List<ProtocolEvent> events, string label)
        {
            if (label != null)
                this.label = label;
            else this.label = "Recording";
        }
        public void StartRecord()
        {
            Recorder.StartRecord(label, Quality);
        }
        public void StopRecord() => Recorder.StopRecord();

        private void ResetCam()
        {
            Recorder = new MediaRecorder(VideoDevice, AudioDevice);
        }

        public void InitDefaultDevices()
        {
            try
            {
                if (Settings.Default.CameraID == "") _videoDevice = null;
                else
                    _videoDevice = MediaDevices.Instance.VideoDeviceByID(Settings.Default.CameraID);
            }
            catch (Exception e)
            {
                Log.Error($"Default Camera could not be seleted, exception: {e}");
                if (MediaDevices.Instance.VideoDevices.Count != 0)
                    VideoDevice = MediaDevices.Instance.VideoDevices.First();
                else VideoDevice = null;
            }
            try
            {
                if (Settings.Default.MicrophoneID == "") _audioDevice = null;
                else _audioDevice = MediaDevices.Instance.AudioDeviceByID(Settings.Default.MicrophoneID);
            }
            catch (Exception e)
            {
                Log.Error($"Default Microphone could not be seleted, exception: {e}");
                if (MediaDevices.Instance.AudioDevices.Count != 0)
                    AudioDevice = MediaDevices.Instance.AudioDevices.First();
                else AudioDevice = null;
            }
            Quality = Settings.Default.CameraQuality;
        }
    }
}
