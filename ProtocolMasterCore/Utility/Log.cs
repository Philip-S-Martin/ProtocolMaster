using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ProtocolMasterCore.Utility
{
    public delegate void LogPrinter(string message);
    public sealed class Log
    {
        private static readonly Log instance = new Log();
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Log()
        {
        }

        public static LogPrinter OutputPrinter;
        public static LogPrinter ErrorPrinter;

        private readonly string logdata;
        private readonly string archive;

        private readonly int maxUnarchived = 48;
        private readonly int minUnarchived = 16;

        private readonly LogFile lfOut;
        private readonly LogFile lfErr;
        public bool PrintErrors { get; set; }
        public bool PrintOutput { get; set; }

        private Log()
        {
            if (AppEnvironment.TryAddLocationAppData("Log", "Log", out logdata))
            {}
            if (AppEnvironment.TryAddLocationAppData("LogArchive", "Log/Archive", out archive))
            {}

            string timePrefix = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            lfOut = new LogFile(Path.Combine(logdata, $"{timePrefix}_Out.log"));
            lfErr = new LogFile(Path.Combine(logdata, $"{timePrefix}_Err.log"));

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

        public static void Error(object message) => Instance.I_Error(message == null? "NULL" : message.ToString());
        public void I_Error(string message)
        {
            string toWrite = $"E:{DateTime.Now}:\t{message}";
            lfErr.Write(toWrite);
            if (PrintErrors)
            {
                ErrorPrinter(toWrite);
            }
        }
        public static void Out(object message) => Log.Instance.I_Out(message == null ? "NULL" : message.ToString());
        public void I_Out(string message)
        {
            string toWrite = $"O:{DateTime.Now}:\t{message}";
            lfOut.Write(toWrite);
            if (PrintErrors)
            {
                OutputPrinter(toWrite);
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
            string zipFileName = $"{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")}[{logs.Count}].zip";

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
                    }
                }
                using (FileStream finalZipFileStream = new FileStream(Path.Combine(archive, zipFileName), FileMode.Create))
                {
                    zipMS.Seek(0, SeekOrigin.Begin);
                    zipMS.CopyTo(finalZipFileStream);
                }
                foreach (string log in logs)
                {
                    File.Delete(log);
                }
            }
        }
    }
}
