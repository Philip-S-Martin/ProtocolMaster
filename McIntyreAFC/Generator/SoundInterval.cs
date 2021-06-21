using ProtocolMasterCore.Protocol;
using System.Collections.Generic;

namespace Schedulino.Generator
{
    class SoundInterval : Interval
    {
        public Sound sound;
        public SoundInterval previous;
        public SoundInterval next;
        public SoundInterval(Sound sound, SoundInterval previous, uint begin, uint end) : base(begin, end)
        {
            this.sound = sound;
            this.previous = previous;
        }


        public override ProtocolEvent ToProtocolEvent()
        {
            return new ProtocolEvent(sound.handler, (sound.name),
                    new KeyValuePair<string, string>("LogMode", sound.log_mode),
                   new KeyValuePair<string, string>("SignalPin", sound.behavior_pin),
                   new KeyValuePair<string, string>("DurationPin", sound.duration_pin),
                   new KeyValuePair<string, string>("Value", sound.sound_id),
                   new KeyValuePair<string, string>("TimeStartMs", begin.ToString()),
                   new KeyValuePair<string, string>("TimeEndMs", end.ToString()));
        }
    }
}
