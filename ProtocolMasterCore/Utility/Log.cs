using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ProtocolMasterCore.Utility
{
    public delegate void LogPrinter(string message);
    public static class Log
    {
        static TextWriter cout;
        static TextWriter cerr;

        private static readonly string logdata;
        private static readonly string archive;

        private static readonly int maxUnarchived = 20;
        private static readonly int minUnarchived = 16;

        public static LogPrinter ErrorPrinter;
        public static LogPrinter OutputPrinter;

        public static bool PrintErrors { get; set; }
        public static bool PrintOutput { get; set; }
        static Log()
        {
            if (AppEnvironment.TryAddLocationDocuments("Log", "Log", out logdata))
            { }
            if (AppEnvironment.TryAddLocationDocuments("LogArchive", "Log/Archive", out archive))
            { }

            string timePrefix = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            FileStream outStream = new FileStream(Path.Combine(logdata, $"{timePrefix}_Out.log"), FileMode.Create);
            FileStream errStream = new FileStream(Path.Combine(logdata, $"{timePrefix}_Err.log"), FileMode.Create);

            cout = TextWriter.Synchronized(new StreamWriter(outStream));
            cerr = TextWriter.Synchronized(new StreamWriter(errStream));

            PrintErrors = true;
            ArchiveOldest();
        }
        public static void Error(object message, bool writeThrough = false)
        {
            string toWrite;
            if (message == null)
                toWrite = $"{DateTime.Now}:\tNULL";
            else toWrite = $"{DateTime.Now}:\t{message}";

            cerr.WriteLine(toWrite);
            if(writeThrough)
                cerr.Flush();

            ErrorPrinter.Invoke(toWrite);
        }
        public static void Out(object message, bool writeThrough = false)
        {
            string toWrite;
            if (message == null)
                toWrite = $"{DateTime.Now}:\tNULL";
            else toWrite = $"{DateTime.Now}:\t{message}";

            cout.WriteLine(toWrite);
            if (writeThrough)
                cout.Flush();

            OutputPrinter.Invoke(toWrite);
        }
        public static void Flush()
        {
            cout.Flush();
            cerr.Flush();
        }
        public static void OpenFolder()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = logdata,
                UseShellExecute = true,
                Verb = "open"
            });
        }
        private static void ArchiveOldest() => Archiver.ArchiveOldestInDirectory(logdata, archive, ".log", maxUnarchived, minUnarchived);
    }
}
