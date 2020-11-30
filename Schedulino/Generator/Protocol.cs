using ProtocolMaster.Model.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulinoDriver.Generator
{
    // Pairing Handler: calls the delivery handler for 
    delegate void PairingHandler(Stimulus stim);
    delegate Interval WindowHandler(SoundInterval si);
    delegate void DeliveryHandler(Stimulus stim, UInt32 begin, UInt32 end);
    class Protocol
    {
        public string description, owner;
        public uint extra_time, num_presounds, num_exp_sounds, interval_min, interval_max, exp_start, exp_end;
        public List<Sound> Sounds { get; set; }
        public List<Stimulus> Stimuli { get; set; }

        private Dictionary<string, DeliveryHandler> deliveryDictionary;
        private Dictionary<string, PairingHandler> pairingDictionary;
        private Dictionary<string, WindowHandler> windowDictionary;

        Random random;
        public Protocol()
        {
            Sounds = new List<Sound>();
            Stimuli = new List<Stimulus>();
            deliveryDictionary = new Dictionary<string, DeliveryHandler>()
            {
                { "Front", FrontDelivery },
                { "Back", BackDelivery },
                { "Center", CenteredDelivery },
                { "Throughout", ThroughoutDelivery },
                { "Random", RandomDelivery }
            };
            pairingDictionary = new Dictionary<string, PairingHandler>()
            {
                { "First", FrontPairing },
                { "Last", BackPairing },
                { "Random", RandomPairing },
                { "Unpaired", UnpairedPairing }
            };
            windowDictionary = new Dictionary<string, WindowHandler>()
            {
                {"Before", BeforeWindow },
                {"After", AfterWindow },
                {"During", DuringWindow }
            };
        }
        public List<ProtocolEvent> Generate()
        {
            UInt32 timeMs = extra_time;
            SoundInterval prevSoundInterval = null;
            random = new Random();
            GeneratePreSounds(ref timeMs, ref prevSoundInterval);
            GenerateExperimentalSounds(ref timeMs, ref prevSoundInterval);
            GenerateStimuli();
            List<Interval> allIntervals = new List<Interval>();
            foreach(Sound sound in Sounds)
            {
                allIntervals.AddRange(sound.presoundIntervals);
                allIntervals.AddRange(sound.expSoundIntervals);
            }
            foreach (Stimulus stim in Stimuli)
            {
                allIntervals.AddRange(stim.stimIntervals);
            }
            allIntervals.Sort();
            List<ProtocolEvent> allEvents = new List<ProtocolEvent>(allIntervals.Count);
            foreach (Interval i in allIntervals)
                allEvents.Add(i.ToProtocolEvent());
            return allEvents;
        }
        private void GeneratePreSounds(ref UInt32 timeMs, ref SoundInterval prevSoundInterval)
        {
            for (int i = 0; i < num_presounds; i++)
            {
                foreach (Sound s in Sounds)
                {
                    SoundInterval si = new SoundInterval(s, prevSoundInterval, timeMs, timeMs + s.duration);
                    s.presoundIntervals.Add(si);
                    timeMs += s.duration;
                    timeMs += (UInt32)random.Next((int)interval_min, (int)interval_max);
                    if (prevSoundInterval != null) prevSoundInterval.next = si;
                    prevSoundInterval = si;
                }
            }
        }
        private void GenerateExperimentalSounds(ref UInt32 timeMs, ref SoundInterval prevSoundInterval)
        {
            exp_start = timeMs;
            uint potential_end = timeMs;
            for (int i = 0; i < num_exp_sounds; i++)
            {
                foreach (Sound s in Sounds)
                {
                    SoundInterval si = new SoundInterval(s, prevSoundInterval, timeMs, timeMs + s.duration);
                    s.expSoundIntervals.Add(si);
                    timeMs += s.duration;
                    potential_end = timeMs;
                    timeMs += (UInt32)random.Next((int)interval_min, (int)interval_max);
                    if (prevSoundInterval != null) prevSoundInterval.next = si;
                    prevSoundInterval = si;
                }
            }
            exp_end = potential_end;
        }
        private void GenerateStimuli()
        {
            foreach (Sound s in Sounds)
            {
                foreach (Stimulus stim in s.stimuli)
                {
                    pairingDictionary[stim.stim_sound_pairing](stim);
                }
            }
        }
        private void FrontPairing(Stimulus stim)
        {
            int iterations = (int)Math.Min(stim.num_paired_sounds, stim.sound.expSoundIntervals.Count);
            for (int i = 0; i < iterations; i++)
            {
                Interval interval = windowDictionary[stim.stim_sound_window](stim.sound.expSoundIntervals[i]);
                deliveryDictionary[stim.stim_delivery](stim, interval.begin, interval.end);
            }
        }
        private void BackPairing(Stimulus stim)
        {
            int iterations = (int)Math.Min(stim.num_paired_sounds, stim.sound.expSoundIntervals.Count);
            int max = stim.sound.expSoundIntervals.Count - 1;
            for (int i = 0; i < iterations; i++)
            {
                Interval interval = windowDictionary[stim.stim_sound_window](stim.sound.expSoundIntervals[max - i]);
                deliveryDictionary[stim.stim_delivery](stim, interval.begin, interval.end);
            }
        }
        private void RandomPairing(Stimulus stim)
        {
            int iterations = (int)Math.Min(stim.num_paired_sounds, stim.sound.expSoundIntervals.Count);
            int max = stim.sound.expSoundIntervals.Count;
            List<int> randomList = new List<int>(iterations);
            for (int i = 0; i < iterations; i++)
            {
                int randomIndex;
                do
                {
                    randomIndex = random.Next(max);
                }
                while (randomList.Contains(randomIndex));
                randomList.Add(randomIndex);
                Interval interval = windowDictionary[stim.stim_sound_window](stim.sound.expSoundIntervals[i]);
                deliveryDictionary[stim.stim_delivery](stim, interval.begin, interval.end);
            }
        }
        private void UnpairedPairing(Stimulus stim)
        {
            deliveryDictionary[stim.stim_delivery](stim, exp_start, exp_end);
        }
        private Interval BeforeWindow(SoundInterval si)
        {
            if (si.previous != null)
                return new Interval(si.previous.end, si.begin);
            else
            {
                return new Interval(si.begin - (UInt32)random.Next((int)interval_min, (int)interval_max), si.end);
            }
        }
        private Interval AfterWindow(SoundInterval si)
        {
            if (si.next != null)
                return new Interval(si.end, si.next.begin);
            else
            {
                return new Interval(si.end, si.end + (UInt32)random.Next((int)interval_min, (int)interval_max));
            }
        }
        private Interval DuringWindow(SoundInterval si)
        {
            return (Interval)si;
        }
        private void FrontDelivery(Stimulus stim, UInt32 begin, UInt32 end)
        {
            uint timeMs = begin;
            timeMs += (uint)random.Next((int)stim.delay_min, (int)stim.delay_max);
            for (int i = 0; i < stim.stims_per_sound; i++)
            {
                uint duration = (uint)random.Next((int)stim.dur_min, (int)stim.dur_max);
                stim.stimIntervals.Add(new StimulusInterval(stim, timeMs, timeMs + duration));
                timeMs += duration;
                timeMs += (uint)random.Next((int)stim.interval_min, (int)stim.interval_max);
            }
        }
        private void BackDelivery(Stimulus stim, UInt32 begin, UInt32 end)
        {
            uint timeMs = end;
            timeMs -= (uint)random.Next((int)stim.delay_min, (int)stim.delay_max);
            for (int i = 0; i < stim.stims_per_sound; i++)
            {
                uint duration = (uint)random.Next((int)stim.dur_min, (int)stim.dur_max);
                stim.stimIntervals.Add(new StimulusInterval(stim, timeMs - duration, timeMs));
                timeMs -= duration;
                timeMs -= (uint)random.Next((int)stim.interval_min, (int)stim.interval_max);
            }
        }
        private void ThroughoutDelivery(Stimulus stim, UInt32 begin, UInt32 end)
        {
            stim.stimIntervals.Add(new StimulusInterval(stim, begin, end));
        }
        private void RandomDelivery(Stimulus stim, UInt32 begin, UInt32 end)
        {
            for (int i = 0; i < stim.stims_per_sound; i++)
            {
                StimulusInterval interval;
                do
                {
                    uint duration = (uint)random.Next((int)stim.dur_min, (int)stim.dur_max);
                    uint randomStim = (uint)random.Next((int)begin, (int)(end - duration));
                    interval = new StimulusInterval(stim, randomStim, randomStim + duration);
                } while (stim.stimIntervals.Exists(a => a.Overlap(interval)));
                stim.stimIntervals.Add(interval);
            }
        }
        private void CenteredDelivery(Stimulus stim, UInt32 begin, UInt32 end)
        {
            uint[] duration = new uint[stim.stims_per_sound];
            uint[] between = new uint[stim.stims_per_sound];
            uint totalTime = 0;
            for (int i = 0; i < stim.stims_per_sound; i++)
            {
                duration[i] = (uint)random.Next((int)stim.dur_min, (int)stim.dur_max);
                between[i] = (uint)random.Next((int)stim.interval_min, (int)stim.interval_max);
                totalTime += duration[i] + between[i];
            }
            uint timeMs = begin + ((end - begin) - totalTime) / 2;
            for (int i = 0; i < stim.stims_per_sound; i++)
            {
                stim.stimIntervals.Add(new StimulusInterval(stim, timeMs, timeMs + duration[i]));
                timeMs += duration[i];
                timeMs += between[i];
            }
        }
    }
}
