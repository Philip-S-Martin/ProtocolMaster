﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulinoDriver.Generator
{
    class Sound
    {
        public string name, handler, behavior_pin, duration_pin, sound_id;
        public uint duration;
        public List<SoundInterval> presoundIntervals;
        public List<SoundInterval> expSoundIntervals;
        public List<Stimulus> stimuli;

        public Sound(string name)
        {
            this.name = name;
            presoundIntervals = new List<SoundInterval>();
            expSoundIntervals = new List<SoundInterval>();
            stimuli = new List<Stimulus>();
        }
    }
}