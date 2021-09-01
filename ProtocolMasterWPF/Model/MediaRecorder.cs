using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace ProtocolMasterWPF.Model
{
    internal class MediaRecorder : INotifyPropertyChanged
    {
        private StorageFolder videoStore;
        private string storagePath;
        public MediaCapture MediaCap { get; private set; }
        private RecordMode mode;
        private enum RecordMode
        {
            NONE,
            VIDEO,
            VIDEOAUDIO,
            AUDIO
        }
        public MediaRecorder(DeviceInformation videoDevice, DeviceInformation audioDevice)
        {
            AppEnvironment.TryAddLocationDocuments("Video", "Video", out storagePath);
            InitVideoStore();
            MediaCap = new MediaCapture();
            InitializeCap(videoDevice, audioDevice);
        }
        private async void InitVideoStore()
        {
            videoStore = await StorageFolder.GetFolderFromPathAsync(storagePath);
        }
        public void InitializeCap(DeviceInformation videoDevice, DeviceInformation audioDevice)
        {
            if (videoDevice == null)
            {
                Log.Error($"Video Device null");
                if (audioDevice != null)
                {
                    MediaCap.InitializeAsync(new MediaCaptureInitializationSettings()
                    {
                        AudioDeviceId = audioDevice.Id
                    }).AsTask().Wait();
                    mode = RecordMode.AUDIO;
                }
                else mode = RecordMode.NONE;
            }
            else
            {
                if (audioDevice == null)
                {
                    try
                    {
                        MediaCap.InitializeAsync(new MediaCaptureInitializationSettings()
                        {
                            VideoDeviceId = videoDevice.Id
                        }).AsTask().Wait();
                        mode = RecordMode.VIDEO;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        // THIS CATCH NEEDS TESTING, NOT SURE IF MODE SET NONE IS GOOD
                        Log.Error($"The app was denied access to the camera: {ex}");
                        mode = RecordMode.NONE;
                    }
                }
                else
                {
                    try
                    {
                        MediaCap.InitializeAsync(new MediaCaptureInitializationSettings()
                        {
                            VideoDeviceId = videoDevice.Id,
                            AudioDeviceId = audioDevice.Id
                        }).AsTask().Wait();
                        
                        var res = MediaCap.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoRecord);
                        
                        mode = RecordMode.VIDEOAUDIO;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        // THIS CATCH NEEDS TESTING, NOT SURE IF MODE SET NONE IS GOOD
                        Log.Error($"The app was denied access to the camera: {ex}");
                        mode = RecordMode.NONE;
                    }
                }
            }
        }
        public async void StartPreview()
        {
            try
            {
                await MediaCap.StartPreviewAsync();
            }
            catch (System.IO.FileLoadException ex)
            {
                Log.Error(ex.ToString());
            }
        }
        public bool IsRecording { get; private set; }
        public async void StartRecord(string label, uint quality)
        {
            try
            {
                if (mode == RecordMode.AUDIO)
                {
                    StorageFile file = await videoStore.CreateFileAsync($"{label}.mp3", CreationCollisionOption.GenerateUniqueName);
                    await MediaCap.StartRecordToStorageFileAsync(MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Auto), file);
                }
                else if (mode == RecordMode.VIDEO || mode == RecordMode.VIDEOAUDIO)
                {
                    // Choose quality based recording
                    MediaCap.SetEncoderProperty(MediaStreamType.VideoRecord, new Guid(0x1c0608e9, 0x370c, 0x4710, 0x8a, 0x58, 0xcb, 0x61, 0x81, 0xc4, 0x24, 0x23), PropertyValue.CreateUInt32(3));
                    // Set quality level
                    MediaCap.SetEncoderProperty(MediaStreamType.VideoRecord, new Guid(0xfcbf57a3, 0x7ea5, 0x4b0c, 0x96, 0x44, 0x69, 0xb4, 0x0c, 0x39, 0xc3, 0x91), PropertyValue.CreateUInt32(quality));

                    StorageFile file = await videoStore.CreateFileAsync($"{label}.mp4", CreationCollisionOption.GenerateUniqueName);
                    await MediaCap.StartRecordToStorageFileAsync(MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto), file);
                }
                else IsRecording = false;
                IsRecording = true;
                Log.Error("Began Recording");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to record:\t{ex}");
            }
        }
        public async void StopRecord()
        {
            if (IsRecording)
            {
                try
                {
                    await MediaCap.StopRecordAsync();
                    IsRecording = false;
                    Log.Error("Finished Recording");
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to save recording:\t{ex}");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
