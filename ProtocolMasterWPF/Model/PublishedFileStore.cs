using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace ProtocolMasterWPF.Model
{
    internal class PublishedFileStore
    {
        public static PublishedFileStore Instance { get => instance; }
        private static PublishedFileStore instance = new PublishedFileStore();
        static PublishedFileStore() { }
        public ObservableCollection<PublishedFileStreamer> PublishedFiles { get; private set; }
        public string Directory { get; private set; }
        private PublishedFileStore()
        {
            AppEnvironment.TryAddLocationAppData("Published", "Published", out string outDir);
            Directory = outDir;
            string filepath = Path.Combine(Directory, "PubStore.json");
            FileInfo file = new FileInfo(filepath);
            if (file.Exists)
            {
                try
                {
                    FileStream filestream = new FileInfo(filepath).Open(FileMode.Open);
                    Task<ObservableCollection<PublishedFileStreamer>> serialTask = JsonSerializer.DeserializeAsync<ObservableCollection<PublishedFileStreamer>>(filestream).AsTask();
                    serialTask.Wait();
                    filestream.Close();
                    PublishedFiles = serialTask.Result;
                }
                catch (JsonException e)
                {
                    Log.Error($"Could not read published file store, exception: {e}");
                    PublishedFiles = new ObservableCollection<PublishedFileStreamer>();
                }
                catch (IOException e)
                {
                    Log.Error($"Could not open published file store {filepath}, exception: {e}");
                    PublishedFiles = new ObservableCollection<PublishedFileStreamer>();
                }
            }
            else PublishedFiles = new ObservableCollection<PublishedFileStreamer>();
        }
        public void Add(string name, string url)
        {
            PublishedFiles.Add(new PublishedFileStreamer() { Name=name, URL=url });
            string filepath = Path.Combine(Directory, "PubStore.json");
            FileStream filestream = new FileInfo(filepath).Open(FileMode.Create);
            JsonSerializer.SerializeAsync(filestream, PublishedFiles).Wait();
            filestream.Close();
        }
        public void Remove(PublishedFileStreamer target)
        {
            PublishedFiles.Remove(target);
            string filepath = Path.Combine(Directory, "PubStore.json");
            FileStream filestream = new FileInfo(filepath).Open(FileMode.Create);
            JsonSerializer.SerializeAsync(filestream, PublishedFiles).Wait();
            filestream.Close();
        }
    }
}
