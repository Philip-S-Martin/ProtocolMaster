using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
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
        public async void StartRecord(string label)
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
                    StorageFile file = await videoStore.CreateFileAsync($"{label}.wmv", CreationCollisionOption.GenerateUniqueName);
                    await MediaCap.StartRecordToStorageFileAsync(MediaEncodingProfile.CreateWmv(VideoEncodingQuality.Auto), file);
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
