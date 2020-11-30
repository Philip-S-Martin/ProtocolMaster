using ProtocolMaster.Model.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulino.InterpreterData
{
    internal class SoundData
    {
        private string name;
        private string behaviorPin;
        private string handler;
        private string durationPin;
        private string soundID;
        private int duration;

        public string Name { get => name; set => name = value; }
        public string BehaviorPin { get => behaviorPin; set => behaviorPin = value; }
        public string Handler { get => handler; set => handler = value; }
        public string DurationPin { get => durationPin; set => durationPin = value; }
        public string SoundID { get => soundID; set => soundID = value; }
        public int Duration { get => duration; set => duration = value; }

        public ProtocolEvent Generate(int timeMs)
        {
            return new ProtocolEvent(this.Handler, "Sound",
                   new KeyValuePair<string, string>("SignalPin", this.BehaviorPin),
                   new KeyValuePair<string, string>("DurationPin", this.DurationPin),
                   new KeyValuePair<string, string>("Value", this.SoundID),
                   new KeyValuePair<string, string>("TimeStartMs", timeMs.ToString()),
                   new KeyValuePair<string, string>("TimeEndMs", (timeMs + this.Duration).ToString()));
        }
    }
}
