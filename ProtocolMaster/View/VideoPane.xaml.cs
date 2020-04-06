using System.Windows.Controls;

namespace ProtocolMaster.View
{
    /// <summary>
    /// Interaction logic for VideoPane.xaml
    /// </summary>
    public partial class VideoPane : Page
    {
        public VideoPane()
        {
            InitializeComponent();
        }
        /*
        #region Private fields
        private FilterInfo _currentDevice;

        private BitmapImage _image;
        private string _ipCameraUrl;

        private bool _isDesktopSource;
        private bool _isIpCameraSource;
        private bool _isWebcamSource;

        private IVideoSource _videoSource;
        private VideoFileWriter _writer;
        private bool _recording;
        private DateTime? _firstFrameTime;
        #endregion

        


        public ObservableCollection<FilterInfo> VideoDevices { get; set; }

        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public bool IsWebcamSource
        {
            get { return _isWebcamSource; }
            set { _isWebcamSource = value; }
        }

        public bool IsIpCameraSource
        {
            get { return _isIpCameraSource; }
            set { _isIpCameraSource = value; }
        }

        public string IpCameraUrl
        {
            get { return _ipCameraUrl; }
            set { _ipCameraUrl = value; }
        }

        public FilterInfo CurrentDevice
        {
            get { return _currentDevice; }
            set { _currentDevice = value; }
        }

        public ICommand StartRecordingCommand { get; private set; }

        public ICommand StopRecordingCommand { get; private set; }

        public ICommand StartSourceCommand { get; private set; }

        public ICommand StopSourceCommand { get; private set; }

        public ICommand SaveSnapshotCommand { get; private set; }

        private void GetVideoDevices()
        {
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in devices)
            {
                VideoDevices.Add(device);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
            else
            {
                MessageBox.Show("No webcam found");
            }
        }
        private void StartCamera()
        {
            if (IsWebcamSource)
            {
                if (CurrentDevice != null)
                {
                    _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                    _videoSource.NewFrame += video_NewFrame;
                    _videoSource.Start();
                }
                else
                {
                    MessageBox.Show("Current device can't be null");
                }
            }
            else if (IsIpCameraSource)
            {
                _videoSource = new MJPEGStream(IpCameraUrl);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();
            }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (_recording)
                {
                    using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                    {
                        if (_firstFrameTime != null)
                        {
                            _writer.WriteVideoFrame(bitmap, DateTime.Now - _firstFrameTime.Value);
                        }
                        else
                        {
                            _writer.WriteVideoFrame(bitmap);
                            _firstFrameTime = DateTime.Now;
                        }
                    }
                }
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    var bi = bitmap.ToBitmapImage();
                    bi.Freeze();
                    Dispatcher.CurrentDispatcher.Invoke(() => Image = bi);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCamera();
            }
        }
        private void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= video_NewFrame;
            }
            Image = null;
        }

        private void StopRecording()
        {
            _recording = false;
            _writer.Close();
            _writer.Dispose();
        }

        private void StartRecording()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "Video1";
            dialog.DefaultExt = ".avi";
            dialog.AddExtension = true;
            var dialogresult = dialog.ShowDialog();
            if (dialogresult != true)
            {
                return;
            }
            _firstFrameTime = null;
            _writer = new VideoFileWriter();
            _writer.Open(dialog.FileName, (int)Math.Round(Image.Width, 0), (int)Math.Round(Image.Height, 0));
            _recording = true;
        }

        private void SaveSnapshot()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "Snapshot1";
            dialog.DefaultExt = ".png";
            var dialogresult = dialog.ShowDialog();
            if (dialogresult != true)
            {
                return;
            }
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(Image));
            using (var filestream = new FileStream(dialog.FileName, FileMode.Create))
            {
                encoder.Save(filestream);
            }
        }

        public void Dispose()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
            }
            _writer?.Dispose();
        }
        public WebCam webCamera { get; set; }
        */
    }
}