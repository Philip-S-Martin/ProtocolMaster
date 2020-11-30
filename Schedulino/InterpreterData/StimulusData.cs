using ProtocolMaster.Model.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulino.InterpreterData
{
    internal delegate bool StimPairingEvaluator(int soundNumber, int totalSounds);
    internal delegate List<ProtocolEvent> StimDeliveryGen(int soundStartMs, int soundDurationMs);
    internal class StimulusData
    {
        private string name;
        private int soundGroup;
        private string stimulusSoundPairing;
        private string intraSoundDelivery;
        private int delayMinimumMs;
        private int delayMaximumMs;
        private int durationMinimumMs;
        private int durationMaximumMs;
        private int numPairedSounds;
        private int stimuliPerSound;
        private int interStimulusIntervalMin;
        private int interStimulusIntervalMax;

        public string Name { get => name; set { if (value != null) name = value; else throw new System.ArgumentException("Name cannot be null"); } }
        public int SoundGroup { get => soundGroup; set => soundGroup = value; }
        public string StimulusSoundPairing { get => stimulusSoundPairing; set { if (value != null) stimulusSoundPairing = value; } }
        public string IntraSoundDelivery { get => intraSoundDelivery; set { if (value != null) intraSoundDelivery = value; } }
        public int DelayMinimumMs { get => delayMinimumMs; set => delayMinimumMs = value; }
        public int DelayMaximumMs { get => delayMaximumMs; set => delayMaximumMs = value; }
        public int DurationMinimumMs { get => durationMinimumMs; set => durationMinimumMs = value; }
        public int DurationMaximumMs { get => durationMaximumMs; set => durationMaximumMs = value; }
        public int NumPairedSounds { get => numPairedSounds; set => numPairedSounds = value; }
        public int StimuliPerSound { get => stimuliPerSound; set => stimuliPerSound = value; }
        public int InterStimulusIntervalMin { get => interStimulusIntervalMin; set => interStimulusIntervalMin = value; }
        public int InterStimulusIntervalMax { get => interStimulusIntervalMax; set => interStimulusIntervalMax = value; }
        StimPairingEvaluator IsPairedToSound { get; set; }
        StimDeliveryGen DeliverInRange { get; set; }
        public StimulatorData StimulatorRef { get; set; }
        public DeliveryData IntraSoundDeliveryRef { get; set; }
        public PairingData StimulusSoundPairingRef { get; set; }
        public void GetRefs(LegacyAFCInterpreter interpreter)
        {
            StimulatorRef = interpreter.Stimulators[Name];
            IntraSoundDeliveryRef = interpreter.Deliveries[IntraSoundDelivery];
            StimulusSoundPairingRef = interpreter.Pairings[StimulusSoundPairing];
        }
        public List<ProtocolEvent> GenerateForSound(int soundStartMs, SoundData sound, int soundNumber, int soundCount)
        {
            /*
            if (IsPairedToSound(soundNumber, totalSounds))
            {
                return DeliverInRange(soundStartMs, soundDurationMs);
            }
            */
            if (BackPairing(soundNumber, soundCount))
            {
                return ThroughoutDelivery(soundStartMs, sound.Duration);
            }
            else return new List<ProtocolEvent>();
        }

        bool FrontPairing(int soundNumber, int totalSounds)
        {
            if (soundNumber < numPairedSounds && soundNumber >= 0 && soundNumber < totalSounds)
                return true;
            else
                return false;
        }
        bool BackPairing(int soundNumber, int totalSounds)
        {
            if ((soundNumber >= totalSounds - numPairedSounds) && soundNumber >= 0 && soundNumber < totalSounds)
                return true;
            else
                return false;
        }
        List<ProtocolEvent> FrontDelivery(int soundStartMs, int soundDurationMs)
        {
            List<ProtocolEvent> events = new List<ProtocolEvent>();
            Random random = new Random();
            int timeMs = soundStartMs;

            timeMs += random.Next(DelayMinimumMs, DelayMaximumMs);
            for (int i = 0; i < stimuliPerSound; i++)
            {
                int duration = random.Next(DurationMinimumMs, DurationMaximumMs);
                events.Add(new ProtocolEvent(StimulatorRef.Handler, Name,
                   new KeyValuePair<string, string>("SignalPin", StimulatorRef.BehaviorPin),
                   new KeyValuePair<string, string>("DurationPin", StimulatorRef.DurationPin),
                   new KeyValuePair<string, string>("TimeStartMs", timeMs.ToString()),
                   new KeyValuePair<string, string>("TimeEndMs", (timeMs + duration).ToString())));
                timeMs += duration;
                timeMs += random.Next(interStimulusIntervalMin, interStimulusIntervalMax);
            }
            return events;
        }
        List<ProtocolEvent> BackDelivery(int soundStartMs, int soundDurationMs)
        {
            List<ProtocolEvent> events = new List<ProtocolEvent>();
            Random random = new Random();
            int timeMs = soundStartMs + soundDurationMs;

            timeMs += random.Next(DelayMinimumMs, DelayMaximumMs);
            for (int i = 0; i < stimuliPerSound; i++)
            {
                int duration = random.Next(DurationMinimumMs, DurationMaximumMs);
                events.Add(new ProtocolEvent(StimulatorRef.Handler, Name,
                   new KeyValuePair<string, string>("SignalPin", StimulatorRef.BehaviorPin),
                   new KeyValuePair<string, string>("DurationPin", StimulatorRef.DurationPin),
                   new KeyValuePair<string, string>("TimeStartMs", (timeMs - duration).ToString()),
                   new KeyValuePair<string, string>("TimeEndMs", timeMs.ToString())));
                timeMs -= duration;
                timeMs -= random.Next(interStimulusIntervalMin, interStimulusIntervalMax);
            }
            return events;
        }
        List<ProtocolEvent> ThroughoutDelivery(int soundStartMs, int soundDurationMs)
        {
            List<ProtocolEvent> events = new List<ProtocolEvent>();

            events.Add(new ProtocolEvent(StimulatorRef.Handler, Name,
               new KeyValuePair<string, string>("SignalPin", StimulatorRef.BehaviorPin),
               new KeyValuePair<string, string>("DurationPin", StimulatorRef.DurationPin),
               new KeyValuePair<string, string>("TimeStartMs", soundStartMs.ToString()),
               new KeyValuePair<string, string>("TimeEndMs", (soundStartMs + soundDurationMs).ToString())));
            return events;
        }
    }
}
