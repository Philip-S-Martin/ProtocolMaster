using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace ProtocolMasterWPF.Model
{
    internal class LocalProtocolStore : Observable
    {
        private static LocalProtocolStore instance = new LocalProtocolStore();
        public static LocalProtocolStore Instance
        {
            get
            {
                return instance;
            }
        }
        static LocalProtocolStore()
        {
        }
        private string _directory;
        public string Directory { get =>_directory; private set { _directory = value; NotifyProperty(); } }
        public ObservableCollection<LocalFileStreamer> LocalFiles { get; private set;}
        private LocalProtocolStore()
        {
            AppEnvironment.TryAddLocationDocuments("Protocols", "Protocols", out string dirResult);
            Directory = dirResult;
            
            LocalFiles = new ObservableCollection<LocalFileStreamer>();
            RefreshFiles();
        }
        public void RefreshFiles()
        {
            LocalFiles.Clear();
            DirectoryInfo dir = new DirectoryInfo(Directory);
            foreach (FileInfo info in dir.GetFiles())
            {
                LocalFiles.Add(new LocalFileStreamer(info));
            }
        }
    }
}
