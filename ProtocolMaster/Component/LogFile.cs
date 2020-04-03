using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProtocolMaster.Component
{
    class LogFile
    {
        private List<String> buffer;
        private long writeAfter;
        private string filePath;
        private string tempPath;
        bool tempFail;

        public LogFile(string filePath)
        {
            this.filePath = filePath;
            buffer = new List<string>();
        }

        

        public void Write(string message, bool deepWrite = false)
        {
            StringBuilder builder = new StringBuilder(DateTime.Now.ToString());
            builder.Append("\t");
            builder.Append(message);
            string output = builder.ToString();

            buffer.Add(output);

            if (deepWrite || DateTime.Now.Ticks > writeAfter)
            {
                WriteBuffer();
            }
        }

        public void WriteBuffer()
        {
            try
            {
                File.AppendAllLines(filePath, buffer);
                if (tempFail)
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                    tempFail = false;
                }
            }
            catch (IOException)
            {
                tempFail = true;
                tempPath = filePath + "[temp]";
                Write("Failed to open log at " + filePath + " attempting " + tempPath + " instead.");
                try
                {
                    File.AppendAllLines(tempPath, buffer);
                }
                catch (IOException)
                {
                    Write("Failed to open temporary log " + tempPath + " will wait to write buffer");
                }
            }


            if (!tempFail) buffer = new List<string>();

            writeAfter = DateTime.Now.Ticks + (5 * 10000000);
        }
    }
}
