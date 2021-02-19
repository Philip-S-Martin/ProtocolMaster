using ProtocolMasterCore.Protocol;
using System.Collections.Generic;

namespace Schedulino.Generator
{
    class StimulusInterval : Interval
    {
        public Stimulus stim;
        public StimulusInterval(Stimulus stim, uint begin, uint end) : base(begin, end)
        {
            this.stim = stim;
        }
        public override ProtocolEvent ToProtocolEvent()
        {
            if (stim.sound != null)
                return new ProtocolEvent(stim.handler, (stim.name), (stim.sound.name),
                   new KeyValuePair<string, string>("SignalPin", stim.behavior_pin),
                   new KeyValuePair<string, string>("DurationPin", stim.duration_pin),
                   new KeyValuePair<string, string>("TimeStartMs", begin.ToString()),
                   new KeyValuePair<string, string>("TimeEndMs", end.ToString()));
            else
                return new ProtocolEvent(stim.handler, (stim.name + "\n(Stim)"),
                   new KeyValuePair<string, string>("SignalPin", stim.behavior_pin),
                   new KeyValuePair<string, string>("DurationPin", stim.duration_pin),
                   new KeyValuePair<string, string>("TimeStartMs", begin.ToString()),
                   new KeyValuePair<string, string>("TimeEndMs", end.ToString()));
        }
    }
}
