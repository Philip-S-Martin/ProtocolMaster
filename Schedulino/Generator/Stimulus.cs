using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulinoDriver.Generator
{
    class Stimulus
    {
        public string stimulus_id, sound_group, stim_sound_pairing, stim_sound_window, stim_delivery, handler, behavior_pin, duration_pin;
        public uint num_paired_sounds, stims_per_sound, dur_min, dur_max, interval_min, interval_max, delay_min, delay_max;
        public Sound sound;
        public List<StimulusInterval> stimIntervals;

        public Stimulus()
        {
            stimIntervals = new List<StimulusInterval>();
        }
    }
}
