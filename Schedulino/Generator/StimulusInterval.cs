using ProtocolMaster.Model.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulinoDriver.Generator
{
    class StimulusInterval : Interval
    {
        public Stimulus stim;

        public StimulusInterval(Stimulus stim, uint begin, uint end):base(begin,end)
        {
            this.stim = stim;
        }
        public override ProtocolEvent ToProtocolEvent()
        {
            return new ProtocolEvent(stim.handler, (stim.stimulus_id+"\n(Stim)"),
                   new KeyValuePair<string, string>("SignalPin", stim.behavior_pin),
                   new KeyValuePair<string, string>("DurationPin", stim.duration_pin),
                   new KeyValuePair<string, string>("TimeStartMs", begin.ToString()),
                   new KeyValuePair<string, string>("TimeEndMs", end.ToString()));
        }
    }
}
