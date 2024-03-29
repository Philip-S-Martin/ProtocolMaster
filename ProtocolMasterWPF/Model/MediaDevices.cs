﻿using System;
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
            _qualityOptions = new List<int>(_qualityArray);
            RefreshDevices();
        }
        private List<DeviceInformation> _videoDevices;

        public List<DeviceInformation> VideoDevices
        {
            get => _videoDevices;
            private set
            {
                _videoDevices = value;
                NotifyProperty();
            }
        }
        public void RefreshDevices()
        {
            Task<DeviceInformationCollection> task = DeviceInformation.FindAllAsync(DeviceClass.VideoCapture).AsTask();
            task.Wait();
            VideoDevices = task.Result.ToList<DeviceInformation>();
            task.Dispose();
            task = DeviceInformation.FindAllAsync(DeviceClass.AudioCapture).AsTask();
            task.Wait();
            AudioDevices = task.Result.ToList<DeviceInformation>();
        }
        private List<DeviceInformation> _audioDevices;
        public List<DeviceInformation> AudioDevices
        {
            get => _audioDevices;
            private set
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
        private int[] _qualityArray = { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        private List<int> _qualityOptions;
        public List<int> QualityOptions
        {
            get => _qualityOptions;
        }
    }
}
