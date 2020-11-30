using ProtocolMaster.Model.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulinoDriver.Generator
{
    class SoundInterval : Interval
    {
        public Sound sound;
        public SoundInterval previous;
        public SoundInterval next;
        public SoundInterval(Sound sound, SoundInterval previous, uint begin, uint end):base(begin, end)
        {
            this.sound = sound;
            this.previous = previous;
        }


        public override ProtocolEvent ToProtocolEvent()
        {
            return new ProtocolEvent(sound.handler, (sound.name + "\n(Sound)"),
                   new KeyValuePair<string, string>("SignalPin", sound.behavior_pin),
                   new KeyValuePair<string, string>("DurationPin", sound.duration_pin),
                   new KeyValuePair<string, string>("Value", sound.sound_id),
                   new KeyValuePair<string, string>("TimeStartMs", begin.ToString()),
                   new KeyValuePair<string, string>("TimeEndMs", end.ToString()));
        }
    }
}
