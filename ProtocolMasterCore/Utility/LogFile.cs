using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProtocolMasterCore.Utility
{
    class LogFile
    {
        ConcurrentQueue<string> inputbuffer;
        private string filePath;
        private string tempPath;
        bool tempFail;

        public LogFile(string filePath)
        {
            this.filePath = filePath;
            inputbuffer = new ConcurrentQueue<string>();
            Task fileWriter = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (tempFail || !inputbuffer.IsEmpty) WriteBuffer();
                    else Thread.Sleep(1000);
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Write(string message)
        {
            inputbuffer.Enqueue(message);
        }

        public void WriteBuffer()
        {
            try
            {
                StreamWriter writer = File.AppendText(filePath);
                if (tempFail)
                {
                    using (Stream input = File.OpenRead(tempPath))
                    {
                        input.CopyTo(writer.BaseStream); // Using .NET 4
                    }
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                    tempFail = false;
                }
                while (inputbuffer.TryDequeue(out string append))
                    writer.WriteLine(append);
                writer.Close();
            }
            catch (IOException)
            {
                tempFail = true;
                tempPath = $"[temp]{filePath}";
                Log.Out($"Failed to open log at {filePath} attempting {tempPath} instead.");
                try
                {
                    StreamWriter writer = File.AppendText(tempPath);
                    while (inputbuffer.TryDequeue(out string append))
                        writer.WriteLine(append);
                    writer.Close();
                }
                catch (IOException)
                {
                    Log.Out($"Failed to open temporary log {tempPath} will wait to write buffer");
                }
            }
        }
    }
}
