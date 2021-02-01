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

        private readonly int maxUnarchived = 20;
        private readonly int minUnarchived = 16;

        private readonly LogFile lfOut;
        private readonly LogFile lfErr;
        public bool PrintErrors { get; set; }
        public bool PrintOutput { get; set; }

        private Log()
        {
            if (AppEnvironment.TryAddLocationAppData("Log", "Log", out logdata))
            { }
            if (AppEnvironment.TryAddLocationAppData("LogArchive", "Log/Archive", out archive))
            { }

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

        public static void Error(object message) => Instance.I_Error(message == null ? "NULL" : message.ToString());
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

        private void ArchiveOldest() => Archiver.ArchiveOldestInDirectory(logdata, archive, ".log", maxUnarchived, minUnarchived);

    }
}
