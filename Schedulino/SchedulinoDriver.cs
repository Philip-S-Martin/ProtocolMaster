using ProtocolMaster.Model.Protocol.Driver;
using ProtocolMaster.Model.Protocol;
using ProtocolMaster.Model.Debug;
using System.ComponentModel.Composition;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http.Headers;
using System.Collections.Concurrent;
using System;
using System.Xml;
using System.Diagnostics;

namespace Schedulino
{
    [DriverMeta("Schedulino", "1.1", "DigitalDuration", "DigitalPulse", "DigitalStringDuration")]
    public class SchedulinoDriver : IDriver
    {
        List<SchedulePinState> schedule;
        int scheduleIndex = 0;
        private enum ScheduleState { SETUP = 0, RUNNING, RESET, CANCEL }
        ScheduleState _state;
        uint _run_time;
        byte _serial_available;
        private uint _capacity;

        SerialPort serial;
        public SerialPort Serial { get => serial; set => serial = value; }

        // Data processing handlers
        delegate void Handler(ProtocolEvent item);
        readonly Dictionary<string, Handler> handlers;
        readonly Handler invalidKeyHander;

        // Arduino Serial receivers
        delegate void Receiver();
        readonly Dictionary<char, Receiver> receivers;
        readonly Receiver invalidKeyReceiver;

        public SchedulinoDriver()
        {
            schedule = new List<SchedulePinState>();
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
                { 'D', DoneReceiver },
                { 'E', ErrorReceiver },
                { 'P', ReportReceiver },
                { 'R', ReplyReceiver }
            };
            invalidKeyReceiver = InvalidKeyReceiver;
        }

        public void Cancel()
        {
            Log.Error("Cancelling Schedulino");
            //scheduleIndex = schedule.Count;
            Serial.Write("X");
            while (serial.BytesToRead >= 1)
            {
                int read = Serial.ReadByte();
                if (read != -1)
                    Receive((char)read);
            }
            _state = ScheduleState.CANCEL;
        }

        public void Setup(List<ProtocolEvent> dataList)
        {
            foreach (ProtocolEvent data in dataList)
            {
                Handle(data);
            }
            schedule.Sort();

            // SerialPort setup
            Serial = new SerialPort
            {
                RtsEnable = true,
                DtrEnable = true,
                PortName = "COM3",
                BaudRate = 9600,
                NewLine = "\n"
            };
            Serial.Open();

            // Handshake
            // Read serial buffer until arduino tells us how much capacity it has
            while (_state != ScheduleState.CANCEL && _capacity == 0)
            {
                ReadSerialBuffer();
            }
            // Pre-Load as many events as possible
            while (_state != ScheduleState.CANCEL && _capacity > 0 && scheduleIndex < schedule.Count)
            {
                SendNextEvent();
                ReadSerialBuffer();
            }
        }

        public void Start()
        {
            // Send start signal
            if (_state != ScheduleState.CANCEL && _state != ScheduleState.RESET)
            {
                while (serial.IsOpen && !TrySendStartSignal())
                {
                    ReadSerialBuffer();
                }
            }
            // Send events and send serial data until Done event is recieved
            while (_state != ScheduleState.CANCEL && _state == ScheduleState.RUNNING)
            {
                SendNextEvent();
                ReadSerialBuffer();
            }
            Serial.Close();
        }
        private void SendNextEvent()
        {
            if (serial.IsOpen && _capacity > 0 && _serial_available >= 7 && scheduleIndex < schedule.Count)
            {
                byte[] bytes = schedule[scheduleIndex].ToBytes();
                Serial.Write("E");
                Serial.Write(bytes, 0, bytes.Length);
                _serial_available -= 7;
                _capacity--;
                scheduleIndex++;
            }
        }

        private bool TrySendStartSignal()
        {
            if (serial.IsOpen && _serial_available > 0)
            {
                Serial.Write("S");
                _state = ScheduleState.RUNNING;
                return true;
            }
            return false;
        }

        private void ReadSerialBuffer()
        {
            while (serial.IsOpen && serial.BytesToRead >= 1)
            {
                int read = Serial.ReadByte();
                if (read != -1)
                    Receive((char)read);
            }
        }

        private void Handle(ProtocolEvent data)
        {
            //Log.Error("Handling: " + data.Handler);
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

        private byte PinMnemonicToNumeric(string pinstring)
        {
            try
            {
                byte value = Convert.ToByte(pinstring);
                return value;
            }
            catch (FormatException)
            {
                if (pinstring == "A0") return 14;
                if (pinstring == "A1") return 15;
                if (pinstring == "A2") return 16;
                if (pinstring == "A3") return 17;
                if (pinstring == "A4") return 18;
                if (pinstring == "A5") return 19;
                throw new FormatException("Invalid Pinstring");
            }
        }

        private byte[] PinSetParseHelper(string pinstring)
        {
            if (pinstring.Length == 0) return null;

            string[] pinstrings = pinstring.Split(',');
            byte[] pins = new byte[pinstrings.Length];
            for (int i = 0; i < pinstrings.Length; i++)
            {
                pins[i] = PinMnemonicToNumeric(pinstrings[i]);
            }
            return pins;
        }

        private byte[] PinRangeParseHelper(string pinstring)
        {
            // there should only be 2 strings, add error checking for that
            string[] pinstrings = pinstring.Split(':');
            byte[] pins = new byte[pinstrings.Length];
            for (int i = 0; i < pinstrings.Length; i++)
            {
                pins[i] = PinMnemonicToNumeric(pinstrings[i]);
            }
            Array.Sort(pins);
            return pins;
        }

        private byte PinStateStringHelper(byte value, byte place)
        {
            return (byte)((value >> place) & 1);
        }
        private void DigitalDurationHandler(ProtocolEvent item)
        {
            item.Arguments.TryGetValue("SignalPin", out string commStr);
            byte[] pins_signal = PinSetParseHelper(commStr);
            item.Arguments.TryGetValue("DurationPin", out commStr);
            byte[] pins_dur = PinSetParseHelper(commStr);
            item.Arguments.TryGetValue("TimeStartMs", out commStr);
            uint time_start = Convert.ToUInt32(commStr);
            item.Arguments.TryGetValue("TimeEndMs", out commStr);
            uint time_end = Convert.ToUInt32(commStr);

            if (pins_signal != null)
                for (int i = 0; i < pins_signal.Length; i++)
                {
                    schedule.Add(new SchedulePinState(pins_signal[i], (byte)1, time_start));
                    schedule.Add(new SchedulePinState(pins_signal[i], (byte)0, time_end));
                }
            if (pins_dur != null)
                for (int i = 0; i < pins_dur.Length; i++)
                {
                    schedule.Add(new SchedulePinState(pins_dur[i], (byte)1, time_start));
                    schedule.Add(new SchedulePinState(pins_dur[i], (byte)0, time_end));
                }
        }
        private void DigitalPulseHandler(ProtocolEvent item)
        {
            item.Arguments.TryGetValue("SignalPin", out string commStr);
            byte[] pins_signal = PinSetParseHelper(commStr);
            item.Arguments.TryGetValue("DurationPin", out commStr);
            byte[] pins_dur = PinSetParseHelper(commStr);
            item.Arguments.TryGetValue("TimeStartMs", out commStr);
            uint time_start = Convert.ToUInt32(commStr);
            item.Arguments.TryGetValue("TimeEndMs", out commStr);
            uint time_end = Convert.ToUInt32(commStr);

            if (pins_signal != null)
                for (int i = 0; i < pins_signal.Length; i++)
                {
                    schedule.Add(new SchedulePinState(pins_signal[i], (byte)1, time_start));
                    schedule.Add(new SchedulePinState(pins_signal[i], (byte)0, time_start + 5));
                }
            if (pins_dur != null)
                for (int i = 0; i < pins_dur.Length; i++)
                {
                    schedule.Add(new SchedulePinState(pins_dur[i], (byte)1, time_start));
                    schedule.Add(new SchedulePinState(pins_dur[i], (byte)0, time_end));
                }
        }
        private void DigitalStringDurationHandler(ProtocolEvent item)
        {
            // This uses a pin range instead of a pin swr
            item.Arguments.TryGetValue("SignalPin", out string commStr);
            byte[] pins_signal = PinRangeParseHelper(commStr);
            item.Arguments.TryGetValue("DurationPin", out commStr);
            byte[] pins_dur = PinSetParseHelper(commStr);
            item.Arguments.TryGetValue("Value", out commStr);
            byte value = Convert.ToByte(commStr);
            item.Arguments.TryGetValue("TimeStartMs", out commStr);
            uint time_start = Convert.ToUInt32(commStr);
            item.Arguments.TryGetValue("TimeEndMs", out commStr);
            uint time_end = Convert.ToUInt32(commStr);


            if (pins_signal != null)
                for (int i = pins_signal[0]; i <= pins_signal[1]; i++)
                {
                    schedule.Add(new SchedulePinState((byte)i, PinStateStringHelper(value, (byte)(i - pins_signal[0])), time_start));
                    schedule.Add(new SchedulePinState((byte)i, (byte)0, time_end));
                }
            if (pins_dur != null)
                for (int i = 0; i < pins_dur.Length; i++)
                {
                    schedule.Add(new SchedulePinState(pins_dur[i], (byte)1, time_start));
                    schedule.Add(new SchedulePinState(pins_dur[i], (byte)0, time_end));
                }

        }
        private void InvalidKeyHandler(ProtocolEvent item)
        {

        }
        #endregion

        private void Receive(char input)
        {
            //Log.Error("Receiving: " + input);
            if (receivers.TryGetValue(input, out Receiver thisKeyReceiver))
            {
                thisKeyReceiver();
            }
            else
            {
                //Log.Error("Byte:" + (byte)input);
                invalidKeyReceiver();
            }
        }

        #region Receiver Functions
        // These functions may be broken out into their own classes
        private void CapacityReceiver()
        {
            _capacity = Convert.ToUInt16(Serial.ReadLine());
            _serial_available = Convert.ToByte(Serial.ReadLine());
            //Log.Error("Schedulino CAPACITY\ncapacity:" + _capacity + "\nserial_available:" + _serial_available);
        }
        private void ErrorReceiver()
        {
            byte file, error, ext;
            file = Convert.ToByte(Serial.ReadLine());
            error = Convert.ToByte(Serial.ReadLine());
            ext = Convert.ToByte(Serial.ReadLine());
            //Log.Error("Schedulino ERROR\nfile:" + file + "\nerror:" + error + "\next:" + ext); ;

        }
        private void DoneReceiver()
        {
            _run_time = Convert.ToUInt32(Serial.ReadLine());
            _state = (ScheduleState)Convert.ToByte(Serial.ReadLine());
            //Log.Error("Schedulino DONE\nrun_time:" + _run_time + "\nstate:" + _state);
        }
        private void ReportReceiver()
        {
            ushort index = Convert.ToUInt16(Serial.ReadLine());
            uint time = Convert.ToUInt32(Serial.ReadLine());
            byte pin = Convert.ToByte(Serial.ReadLine());
            byte pinstate = Convert.ToByte(Serial.ReadLine());
            _capacity++;
            //Log.Error("Schedulino REPORT\nindex:" + index + "\ntime/otime:" + time + "/" + schedule[index].Time + "\npin:" + pin + "\npinstate:" + pinstate);
        }
        private void ReplyReceiver()
        {
            _serial_available += 7;
            //Log.Error("Schedulino REPLY\nserial_available:" + _serial_available);
        }
        private void InvalidKeyReceiver()
        {
            Log.Error("Schedulino UNEXPECTED BYTE");
        }


        #endregion

        #region Read Helper Functions
        /*
        private uint NumReadSerial(int length)
        {
            byte[] readBytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                readBytes[i] = (byte)Serial.ReadByte();
            }
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
        */

        #endregion

        struct SchedulePinState : IComparable<SchedulePinState>
        {
            public readonly byte Pin;
            public readonly byte State;
            public readonly uint Time;
            public SchedulePinState(byte pin, byte state, uint time)
            {
                this.Pin = pin;
                this.State = state;
                this.Time = time;
            }
            public byte[] ToBytes()
            {
                byte[] response = new byte[6];
                response[4] = Pin;
                response[5] = State;

                uint tempTime = Time;
                for (int i = 0; i < 4; i++)
                {
                    response[i] = (byte)tempTime;
                    tempTime >>= 8;
                }
                return response;
            }
            public override int GetHashCode()
            {
                long lTime = Time + int.MinValue;
                return (int)lTime;
            }

            public int CompareTo(SchedulePinState other)
            {
                return (int)(this.Time - other.Time + this.State - other.State);
            }
        }
    }
}
