using ProtocolMaster.Component.Model;
using ProtocolMaster.Component.Debug;
using System.ComponentModel.Composition;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http.Headers;
using System.Collections.Concurrent;

namespace Schedulino
{
    [DriverMeta("Schedulino", "1.1", "DigitalDuration", "DigitalPulse", "DigitalStringDuration")]
    public class SchedulinoDriver : IDriver
    {
        private enum ScheduleState { SETUP = 0, RUNNING, RESET }
        ScheduleState _state;
        uint _run_time;
        byte _serial_available;
        private uint _capacity;

        SerialPort serial;
        public SerialPort Serial { get => serial; set => serial = value; }

        public ConcurrentQueue<VisualData> VisualData { get; private set; }

        // Data processing handlers
        delegate void Handler(DriveData item);
        readonly Dictionary<string, Handler> handlers;
        readonly Handler invalidKeyHander;

        // Arduino Serial receivers
        delegate void Receiver();
        readonly Dictionary<char, Receiver> receivers;
        readonly Receiver invalidKeyReceiver;

        public SchedulinoDriver()
        {
            _state = ScheduleState.SETUP;
            _run_time = 0;
            _serial_available = 0;
            _capacity = 0;

            // Handlers for processing data
            handlers = new Dictionary<string, Handler>
            {
                { "DigitalDuration", DigitalDurationHandler },
                { "DigitalPulse", DigitalPulseHandler },
                { "DigitalStringDuration", DigitalStringDurationHandler }
            };
            invalidKeyHander = InvalidKeyHandler;

            // Reveivers for receiving data from Arduino
            receivers = new Dictionary<char, Receiver>
            {
                { 'C', CapacityReceiver },
                { 'E', ErrorReceiver },
                { 'D', DoneReceiver },
                { 'P', ReportReceiver },
                { 'R', ReplyReceiver }
            };
            invalidKeyReceiver = InvalidKeyReceiver;
        }

        public void Cancel()
        {
            Log.Error("Cancelling Schedulino");
        }

        public void ProcessData(List<DriveData> dataList)
        {
            foreach(DriveData data in dataList)
            {
                Handle(data);
            }
        }

        public void Run()
        {
            // SerialPort setup
            Serial = new SerialPort
            {
                RtsEnable = true,
                DtrEnable = true,
                PortName = "COM3",
                BaudRate = 9600
            };
            Serial.Open();

            while (true)
            {
                int read = Serial.ReadByte();
                if (read != -1)
                    Receive((char)read);
            }
        }

        private void Handle(DriveData data)
        {
            Log.Error("Handling: " + data.Handler);
            if (handlers.TryGetValue(data.Handler, out Handler thisKeyHandler))
            {
                thisKeyHandler(data);
            }
            else
            {
                invalidKeyHander(data);
            }
        }

        #region Handler Functions
        private void DigitalDurationHandler(DriveData item)
        {

        }
        private void DigitalPulseHandler(DriveData item)
        {

        }
        private void DigitalStringDurationHandler(DriveData item)
        {

        }
        private void InvalidKeyHandler(DriveData item)
        {

        }
        #endregion

        private void Receive(char input)
        {
            Log.Error("Receiving: " + input);
            if (receivers.TryGetValue(input, out Receiver thisKeyReceiver))
            {
                thisKeyReceiver();
            }
            else
            {
                invalidKeyReceiver();
            }
        }

        #region Receiver Functions
        private void CapacityReceiver()
        {
            _capacity = NumReadSerial(2);
            Log.Error("Schedulino CAPACITY\ncapacity:" + _capacity);
        }
        private void ErrorReceiver()
        {
            byte file, error, ext;
            file = (byte)NumReadSerial(1);
            error = (byte)NumReadSerial(1);
            ext = (byte)NumReadSerial(1);
            Log.Error("Schedulino ERROR\nfile:" + file + "\nerror:" + error + "\next:" + ext); ;

        }
        private void DoneReceiver()
        {
            _run_time = NumReadSerial(4);
            _state = (ScheduleState)NumReadSerial(1);
            Log.Error("Schedulino DONE\nrun_time:" + _run_time + "\nstate:" + _state);
        }
        private void ReportReceiver()
        {
            ushort index = (ushort)NumReadSerial(2);
            uint time = (uint)NumReadSerial(4);
            byte pin = (byte)NumReadSerial(1);
            byte pinstate = (byte)NumReadSerial(1);
            Log.Error("Schedulino REPORT\nindex:" + index + "\ntime:" + time + "\npin:" + pin + "\npinstate:" + pinstate);
        }
        private void ReplyReceiver()
        {
            _serial_available = (byte)NumReadSerial(1);
            Log.Error("Schedulino REPLY\nserial_available:" + _serial_available);
        }
        private void InvalidKeyReceiver()
        {
            Log.Error("Schedulino UNEXPECTED BYTE");
        }


        #endregion

        #region Read/Write Functions
        private uint NumReadSerial(int length)
        {
            byte[] readBytes = new byte[length];
            Serial.Read(readBytes, 0, length);
            return NumRead(readBytes);
        }
        private static uint NumRead(byte[] input)
        {
            uint result = 0;
            uint pow = 1;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                result += input[i] * pow;
                pow <<= 8;
            }
            return result;
        }
        private void NumWriteSerial(uint num, int length)
        {
            Serial.Write(NumWrite(num, length), 0, length);
        }
        private static byte[] NumWrite(uint num, int length)
        {
            byte[] response = new byte[length];

            for (int i = 0; i < length; i++)
            {
                num >>= (i * 8);
                response[i] = (byte)num;
            }
            return response;
        }
        #endregion


    }
}
