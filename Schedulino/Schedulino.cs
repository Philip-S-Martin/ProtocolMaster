using ProtocolMaster.Component.Model;
using ProtocolMaster.Component.Debug;
using System.ComponentModel.Composition;
using System.IO.Ports;

namespace SchedulinoDriver
{
    [Export(typeof(ProtocolMaster.Component.Model.IDriver))]
    [ExportMetadata("Symbol", new string[] { "Schedulino" })]
    public class Schedulino : IDriver
    {
        private enum enum_state { SETUP = 0, RUNNING, RESET }
        enum_state _state;
        uint _run_time;
        byte _serial_available;
        private uint _capacity;

        SerialPort serial;
        public SerialPort Serial { get => serial; set => serial = value; }

        public void Setup()
        {
            _state = enum_state.SETUP;
            _run_time = 0;
            _serial_available = 0;
            _capacity = 0;

            Serial.BaudRate = 9600;
            Serial.Open();
        }

        public async void Loop()
        {
            byte[] read = new byte[1];
            await Serial.BaseStream.ReadAsync(read, 0, 1);
            Receive((char)read[0]);
        }

        public void Exit()
        {

        }

        private void Receive(char input)
        {
            switch (input)
            {
                case 'C':
                    ReceiveCapacity();
                    break;
                case 'E':
                    ReceiveError();
                    break;
                case 'D':
                    ReceiveDone();
                    break;
                case 'P':
                    ReceiveReport();
                    break;
                case 'R':
                    ReceiveReply();
                    break;
                default:
                    ReceiveIncorrect();
                    break;
            }
        }
        // Receivers: should probably be broken out into other classes...
        #region
        private uint _RecieveNum(int length)
        {
            byte[] response = new byte[length];
            Serial.Read(response, 0, length);
            uint result = 0;
            uint pow = 1;
            for (int i = 0; i < length; i++)
            {
                result += response[i] * pow;
                pow = pow << 8;
            }
            return result;
        }
        private uint _SendNum(ulong num, int length)
        {
            byte[] response = new byte[length];
            Serial.Read(response, 0, length);
            uint result = 0;
            uint pow = 1;
            for (int i = 0; i < length; i++)
            {
                result += response[i] * pow;
                pow = pow << 8;
            }
            return result;
        }
        private void ReceiveCapacity()
        {
            _capacity = _RecieveNum(2);
            Log.Error("Schedulino CAPACITY\ncapacity:" + _capacity);
        }
        private void ReceiveError()
        {
            byte file, error, ext;
            file = (byte)_RecieveNum(1);
            error = (byte)_RecieveNum(1);
            ext = (byte)_RecieveNum(1);
            Log.Error("Schedulino ERROR\nfile:" + file + "\nerror:" + error + "\next:" + ext); ;

        }
        private void ReceiveDone()
        {
            _run_time = _RecieveNum(4);
            _state = (enum_state)_RecieveNum(1);
            Log.Error("Schedulino DONE\nrun_time:" + _run_time + "\nstate:" + _state);
        }
        private void ReceiveReport()
        {
            ushort index = (ushort)_RecieveNum(2);
            uint time = (uint)_RecieveNum(4);
            byte pin = (byte)_RecieveNum(1);
            byte pinstate = (byte)_RecieveNum(1);
            Log.Error("Schedulino REPORT\nindex:" + index + "\ntime:" + time + "\npin:" + pin + "\npinstate:" + pinstate);

        }
        private void ReceiveReply()
        {
            _serial_available = (byte)_RecieveNum(1);
            Log.Error("Schedulino REPLY\nserial_available:" + _serial_available);
        }
        private void ReceiveIncorrect()
        {
            Log.Error("Schedulino UNEXPECTED BYTE");
        }
        #endregion
    }
}
