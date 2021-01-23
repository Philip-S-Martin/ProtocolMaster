using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ProtocolMasterWPF.ViewModel
{
    public class ProtocolSelectViewModel
    {
        public ObservableCollection<Object> DriveOptions { get; private set; }
        public ObservableCollection<Object> LocalOptions { get; private set; }
        public ObservableCollection<Object> PublishedOptions { get; private set; }
        public ProtocolSelectViewModel()
        {
            DriveOptions = new ObservableCollection<object>();
            for (int i = 0; i < 100; i++) DriveOptions.Add($"Drive: {i}");
            LocalOptions = new ObservableCollection<object>();
            for (int i = 0; i < 100; i++) LocalOptions.Add($"Local: {i}");
            PublishedOptions = new ObservableCollection<object>();
            for (int i = 0; i < 100; i++) PublishedOptions.Add($"Published: {i}");
        }
        
    }
}
