using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using ProtocolMaster;

namespace ProtocolMaster.Component
{
    public sealed class Log
    {
        private static readonly Log instance = new Log();
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Log()
        {
        }

        private readonly string appdata;
        public string AppData { get { return appdata; } }
        private readonly string logdata;
        private readonly string archive;

        private readonly int maxUnarchived = 48;
        private readonly int minUnarchived = 16;

        private readonly LogFile lfOut;
        private readonly LogFile lfErr;
        public bool PrintErrors { get; set; }

        private Log()
        {
            appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ProtocolMaster";
            logdata = appdata + "\\Log\\";
            archive = logdata + "\\Archive\\";

            // First two are redundant, but it is prettier this way
            Directory.CreateDirectory(appdata);
            Directory.CreateDirectory(logdata);
            Directory.CreateDirectory(archive);

            string timePrefix = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            lfOut = new LogFile(logdata + timePrefix + "_Out.log");
            lfErr = new LogFile(logdata + timePrefix + "_Err.log");

            PrintErrors = true;

            ArchiveOldest();
        }
        public static Log Instance
        {
            get
            {
                return instance;
            }
        }

        public static void Error(string message) => Log.Instance._Error(message);
        public void _Error(string message)
        {
            lfErr.Write(message);
            if (PrintErrors && App.Window != null && App.Window.Log != null)
            {
                App.Window.Log.Log(message.Replace("\t", "\n"));
            }
        }
        public static void Out(string message) => Log.Instance._Out(message);
        public void _Out(string message)
        {
            lfOut.Write(message);
            if (PrintErrors && App.Window != null && App.Window.Log != null)
            {
                App.Window.Log.Log(message.Replace("\t", "\n"));
            }
        }

        public void WriteFiles()
        {
            lfErr.WriteBuffer();
            lfOut.WriteBuffer();
        }

        public void OpenFolder()
        {
            WriteFiles();
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = logdata,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void ArchiveOldest()
        {
            string[] files = Directory.GetFiles(logdata);
            List<string> logs = new List<string>();
            foreach (string file in files)
            {
                if (file.Contains(".log")) logs.Add(file);
            }

            if (logs.Count < maxUnarchived) return;

            logs = logs.OrderBy(d => d).Take(logs.Count - minUnarchived).ToList();

            //final archive name (I use date / time)
            string zipFileName = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + "[" + logs.Count + "].zip";

            using (MemoryStream zipMS = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(zipMS, ZipArchiveMode.Create, true))
                {
                    //loop through files to add
                    foreach (string zipTarget in logs)
                    {
                        //exclude some files? -I don't want to ZIP other .zips in the folder.
                        if (new FileInfo(zipTarget).Extension == ".zip") continue;

                        //read the file bytes
                        byte[] fileToZipBytes = System.IO.File.ReadAllBytes(zipTarget);

                        //create the entry - this is the zipped filename
                        //change slashes - now it's VALID
                        ZipArchiveEntry zipFileEntry = zipArchive.CreateEntry(zipTarget.Replace(logdata, "").Replace('\\', '/'));

                        //add the file contents
                        using (Stream zipEntryStream = zipFileEntry.Open())
                        using (BinaryWriter zipFileBinary = new BinaryWriter(zipEntryStream))
                        {
                            zipFileBinary.Write(fileToZipBytes);
                        }

                        //lstLog.Items.Add("zipped: " + fileToZip);
                    }
                }

                using (FileStream finalZipFileStream = new FileStream(archive + zipFileName, FileMode.Create))
                {
                    zipMS.Seek(0, SeekOrigin.Begin);
                    zipMS.CopyTo(finalZipFileStream);
                }

                //lstLog.Items.Add("ZIP Archive Created.");

                foreach (string log in logs)
                {
                    File.Delete(log);
                }
            }
        }
    }
}
