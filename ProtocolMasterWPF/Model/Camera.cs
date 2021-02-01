using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace ProtocolMasterWPF.Model
{
    internal class Camera : INotifyPropertyChanged
    {
        private StorageFolder videoStore;
        private string storagePath;
        public MediaCapture MediaCap { get; private set; }
        public Camera()
        {
            AppEnvironment.TryAddLocationAppData("Video", "Video", out storagePath);
            InitVideoStore();
            MediaCap = new MediaCapture();
            InitializeCap();
        }
        private async void InitVideoStore()
        {
            videoStore = await StorageFolder.GetFolderFromPathAsync(storagePath);
        }
        public async void InitializeCap()
        {
            try
            {
                await MediaCap.InitializeAsync(new MediaCaptureInitializationSettings());
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error($"The app was denied access to the camera:\t{ex}");
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
        public async void StartRecord()
        {
            try
            {
                StorageFile file = await videoStore.CreateFileAsync("Recording (1).wmv", CreationCollisionOption.GenerateUniqueName);
                await MediaCap.StartRecordToStorageFileAsync(MediaEncodingProfile.CreateWmv(VideoEncodingQuality.Auto), file);
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
