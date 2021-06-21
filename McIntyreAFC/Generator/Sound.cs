using System.Collections.Generic;

namespace Schedulino.Generator
{
    class Sound
    {
        public string name, handler, behavior_pin, duration_pin, sound_id;
        public string log_mode;
        public uint duration;
        public List<SoundInterval> presoundIntervals;
        public List<SoundInterval> expSoundIntervals;
        public List<Stimulus> stimuli;

        public Sound(string name)
        {
            this.name = name;
            this.log_mode = "Yes";
            presoundIntervals = new List<SoundInterval>();
            expSoundIntervals = new List<SoundInterval>();
            stimuli = new List<Stimulus>();
        }

        public Sound CloneSoundWithoutProtocolData()
        {
            Sound clone = new Sound(this.name);
            clone.handler = this.handler;
            clone.behavior_pin = this.behavior_pin;
            clone.duration_pin = this.duration_pin;
            clone.sound_id = this.sound_id;
            clone.duration = this.duration;
            return clone;
        }
    }
}
