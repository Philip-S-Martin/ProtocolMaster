using ProtocolMaster.Model.Protocol;
using Schedulino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulino.InterpreterData
{
    internal class ProtocolData
    {
        List<StimulusData> Stimuli;
        private string name;
        private string description;
        private string owner;
        private string sound_1;
        private string sound_2;
        private int presoundCount;
        private int soundCount;
        private int intersoundIntervalMin;
        private int interSoundIntervalMax;
        private int extraTime;
        public string Name { get => name; set { if (value != null) name = value; else throw new System.ArgumentException("Name cannot be null"); } }
        public string Description { get => description; set { if (value != null) description = value; else description = ""; } }
        public string Owner { get => owner; set { if (value != null) owner = value; else owner = ""; } }
        public string Sound_1 { get => sound_1; set { if (value != null) sound_1 = value; else throw new System.ArgumentException("Sound_1 cannot be null"); } }
        public string Sound_2 { get => sound_2; set => sound_2 = value; }
        public int PresoundCount { get => presoundCount; set => presoundCount = value; }
        public int SoundCount { get => soundCount; set => soundCount = value; }
        public int IntersoundIntervalMin { get => intersoundIntervalMin; set => intersoundIntervalMin = value; }
        public int InterSoundIntervalMax { get => interSoundIntervalMax; set => interSoundIntervalMax = value; }
        public int ExtraTime { get => extraTime; set => extraTime = value; }
        public SoundData SoundRef_1 { get; set; }
        public SoundData SoundRef_2 { get; set; }
        public ProtocolData()
        {
            Stimuli = new List<StimulusData>();
        }
        public void AddStimulus(StimulusData toAdd)
        {
            Stimuli.Add(toAdd);
        }
        public void GetRefs(LegacyAFCInterpreter interpreter)
        {
            SoundRef_1 = interpreter.Sounds[Sound_1];
            if (sound_2 != null)
                SoundRef_2 = interpreter.Sounds[Sound_2];
            else
                SoundRef_2 = null;
            foreach (StimulusData stim in Stimuli)
            {
                stim.GetRefs(interpreter);
            }
        }
        public List<ProtocolEvent> Generate(LegacyAFCInterpreter interpreter)
        {
            GetRefs(interpreter);
            List<ProtocolEvent> events = new List<ProtocolEvent>();

            Random random = new Random();
            int timeMs = ExtraTime;

            events.AddRange(GeneratePreSounds(ref timeMs, ref random));
            events.AddRange(GenerateSoundsAndStims(ref timeMs, ref random));

            return events;
        }
        private List<ProtocolEvent> GeneratePreSounds(ref int timeMs, ref Random random)
        {
            List<ProtocolEvent> events = new List<ProtocolEvent>();
            for (int i = 0; i < presoundCount; i++)
            {
                events.Add(SoundRef_1.Generate(timeMs));
                timeMs += SoundRef_1.Duration + random.Next(IntersoundIntervalMin, InterSoundIntervalMax);
                // if there is a second sound
                if (SoundRef_2 != null)
                {
                    events.Add(SoundRef_2.Generate(timeMs));
                    timeMs += SoundRef_2.Duration + random.Next(IntersoundIntervalMin, InterSoundIntervalMax);
                }
            }
            return events;
        }
        private List<ProtocolEvent> GenerateSoundsAndStims(ref int timeMs, ref Random random)
        {
            List<ProtocolEvent> events = new List<ProtocolEvent>();
            for (int i = 0; i < soundCount; i++)
            {
                events.Add(SoundRef_1.Generate(timeMs));
                foreach (StimulusData stim in Stimuli)
                {
                    if (stim.SoundGroup == 1)
                        events.AddRange(stim.GenerateForSound(timeMs, SoundRef_1, i, soundCount));
                }
                timeMs += SoundRef_1.Duration + random.Next(IntersoundIntervalMin, InterSoundIntervalMax);
                // if there is a second sound
                if (SoundRef_2 != null)
                {
                    events.Add(SoundRef_2.Generate(timeMs));
                    foreach (StimulusData stim in Stimuli)
                    {
                        if (stim.SoundGroup == 2)
                            events.AddRange(stim.GenerateForSound(timeMs, SoundRef_2, i, soundCount));
                    }
                    timeMs += SoundRef_2.Duration + random.Next(IntersoundIntervalMin, InterSoundIntervalMax);
                }
            }
            return events;
        }
    }
}
