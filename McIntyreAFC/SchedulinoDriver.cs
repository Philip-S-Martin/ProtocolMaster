using ProtocolMasterCore.Prompt;
using ProtocolMasterCore.Protocol;
using ProtocolMasterCore.Protocol.Driver;
using ProtocolMasterCore.Utility;
using System;
using RJCP.IO.Ports;
using System.Collections.Generic;
using System.Threading;

namespace McIntyreAFC
{
    [DriverMeta("McIntyreAFC", "1.1", "DigitalDuration", "DigitalPulse", "DigitalStringDuration")]
    public class Schedulino : IDriver, IPromptUserSelect
    {
        List<SchedulePinState> schedule;
        int scheduleIndex = 0;
        private enum ScheduleState { SETUP = 0, RUNNING, DONE }
        ScheduleState _state;
        uint _run_time;
        byte _serial_available;
        private uint _capacity;
        
        SerialPortStream serial;
        public SerialPortStream Serial { get => serial; set => serial = value; }
        public UserSelectHandler UserSelectPrompt { private get; set; }

        // Data processing handlers
        delegate void Handler(ProtocolEvent item);
        readonly Dictionary<string, Handler> handlers;
        readonly Handler invalidKeyHander;

        // Arduino Serial receivers
        delegate void Receiver();
        readonly Dictionary<char, Receiver> receivers;
        readonly Receiver invalidKeyReceiver;

        public Schedulino()
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

        private bool isCanceled;
        public bool IsCanceled
        {
            get => isCanceled;
            set
            {
                isCanceled = true;
                Log.Error("McInyreAFC CANCELLED");
                Serial.Write("X");
                ReadSerialBuffer(Serial);
            }
        }

        public bool Setup(List<ProtocolEvent> dataList)
        {
            Log.Error("McIntyreAFC SETUP");
            // CONVERT EVENTS TO SCHEDULE
            // Generate Schedule
            foreach (ProtocolEvent data in dataList) Handle(data);
            // Sort schedule
            schedule.Sort();

            // PORT SELECTION
            string[] portOptions = SerialPortStream.GetPortNames();
            string port;
            // If there are no available ports, exit
            if (portOptions.Length == 0) return false;
            // If there is one available port, use it
            else if (portOptions.Length == 1) port = portOptions[0];
            // If there are many available ports, allow the user to select
            else port = UserSelectPrompt(portOptions);

            // SERIALPORT SETUP
            Serial = new SerialPortStream
            {
                RtsEnable = true,
                DtrEnable = true,
                PortName = port,
                BaudRate = 9600,
                NewLine = "\n"
            };

            Serial.DataReceived += DataReceiver;
            Serial.Open();

            // Handshake
            // Read serial buffer until arduino tells us how much capacity it has
            while (_state != ScheduleState.DONE && _capacity == 0)
            {
                Thread.Sleep(8);
            }
            // Send as many events as possible
            while (_capacity > 0 && scheduleIndex < schedule.Count)
                while (SendNextEvent(Serial)) ;
            return true;
        }

        public void Start()
        {
            Log.Error("McIntyreAFC START");
            // Send start signal
            if (_state != ScheduleState.DONE)
            {
                while (!TrySendStartSignal())
                {
                    Thread.Sleep(2);
                }
            }
            Log.Error($"McIntyreAFC STARTING:[state:{_state}]");
            // Send events and send serial data until Done event is recieved
            while (_state == ScheduleState.RUNNING)
            {
                Thread.Sleep(64);
            }
            // Read remaining buffer
            ReadSerialBuffer(Serial);
            if (serial.IsOpen) Serial.Close();
        }
        void DataReceiver(object sender, SerialDataReceivedEventArgs args)
        {
            if (_state != ScheduleState.DONE)
            {
                ReadSerialBuffer(sender as SerialPortStream);
                while (_state == ScheduleState.RUNNING && SendNextEvent(sender as SerialPortStream)) ;
            }
        }
        private bool SendNextEvent(SerialPortStream port)
        {
            if (port.IsOpen && _capacity > 0 && _serial_available >= 7 && scheduleIndex < schedule.Count)
            {
                Log.Error($"McIntyreAFC SENDEVENT:[scheduleIndex:{scheduleIndex}],[pinState:{schedule[scheduleIndex]}]");
                byte[] bytes = schedule[scheduleIndex].ToBytes();
                port.Write("E");
                port.Write(bytes, 0, bytes.Length);
                _serial_available -= 7;
                _capacity--;
                scheduleIndex++;
                return true;
            }
            else return false;
        }

        private bool TrySendStartSignal()
        {
            if (Serial.IsOpen && _serial_available > 0)
            {
                Serial.Write("S");
                _state = ScheduleState.RUNNING;
                return true;
            }
            return false;
        }

        private void ReadSerialBuffer(SerialPortStream port)
        {
            while (port.IsOpen && port.BytesToRead > 0)
            {
                int read = port.ReadByte();
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
            Log.Error($"McIntyreAFC CAPACITY:[capacity:{_capacity}],[serial_available:{_serial_available}]");
        }
        private void ErrorReceiver()
        {
            byte file, error, ext;
            file = Convert.ToByte(Serial.ReadLine());
            error = Convert.ToByte(Serial.ReadLine());
            ext = Convert.ToByte(Serial.ReadLine());
            Log.Error($"McIntyreAFC ERROR:[file:{file}],[error:{error}],[ext:{ext}]");
        }
        private void DoneReceiver()
        {
            _run_time = Convert.ToUInt32(Serial.ReadLine());
            _state = ScheduleState.DONE;
            Log.Error($"McIntyreAFC DONE:[run_time:{_run_time}],[state:{_state}]");
        }
        private void ReportReceiver()
        {
            ushort index = Convert.ToUInt16(Serial.ReadLine());
            uint time = Convert.ToUInt32(Serial.ReadLine());
            byte pin = Convert.ToByte(Serial.ReadLine());
            byte pinstate = Convert.ToByte(Serial.ReadLine());
            _capacity++;
            Log.Error($"McIntyreAFC REPORT:[index:{index}],[time:{time}],[otime:{schedule[index].Time}],[pin:{pin}],[pinstate:{pinstate}]");
        }
        private void ReplyReceiver()
        {
            _serial_available += 7;
            Log.Error($"McIntyreAFC REPLY:[serial_available:{_serial_available}]");
        }
        private void InvalidKeyReceiver()
        {
            Log.Error("McIntyreAFC UNEXPECTED BYTE");
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
