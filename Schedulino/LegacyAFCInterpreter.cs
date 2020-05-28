using ProtocolMaster.Component.Debug;
using ProtocolMaster.Component.Model;
using ProtocolMaster.Component.Model.Interpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Schedulino
{
    [InterpreterMeta("LegacyAFCInterpreter", "1.1")]

    /*
     * 
        "Protocol,Description,Owner,Sound 1,Sound 2,Number of Presounds (each),
            Number of Sounds (each),Inter-sound Interval Minimum (seconds),Inter-sound Interval Maximum (seconds),
            Extra Time (seconds),Stimulus,Sound Group,Stimulus-Sound Pairing,Intra-Sound Stimulus Delivery,
            Stimulus Delay Minimum (ms) (Optional),Stimulus Delay Maximum (ms) (Optional),Stimulus Duration (ms),
            Number of Stimulus,Stimulus Repetitions Per Sound,Inter-Stimulus Interval Minimum,Inter-Stimulus Interval Maximum",
        "Sound,Handler,Behavior_Pin,Duration_Pin,Sound_ID",
        "Stimulator,Handler,Behavior_Pin,Duration_Pin",
        "Delivery,Handler",
        "Pairing,Handler"
    */
    public class LegacyAFCInterpreter : SpreadSheetInterpreter, IInterpreter
    {
        public List<DriveData> Data { get; private set; }
        internal Dictionary<string, Protocol> Protocols { get; private set; }
        internal Dictionary<string, Sound> Sounds { get; private set; }
        internal Dictionary<string, Stimulator> Stimulators { get; private set; }
        internal Dictionary<string, Delivery> Deliveries { get; private set; }
        internal Dictionary<string, Pairing> Pairings { get; private set; }

        private delegate bool Filler(Dictionary<string, int> map);
        Dictionary<string, Filler> fillers;

        public LegacyAFCInterpreter()
        {
            fillers = new Dictionary<string, Filler>()
            {
                { "Protocols", FillProtocol },
                { "Sounds", FillSound },
                { "Stimulators", FillStimulator },
                { "Delivery", FillDelivery },
                { "Pairing", FillPairing }
            };
            Protocols = new Dictionary<string, Protocol>();
            Sounds = new Dictionary<string, Sound>();
            Stimulators = new Dictionary<string, Stimulator>();
            Deliveries = new Dictionary<string, Delivery>();
            Pairings = new Dictionary<string, Pairing>();
        }
        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            do
            {
                // instantiate header map
                Dictionary<string, int> sheetMap = new Dictionary<string, int>();
                // Get header row
                DataReader.Read();
                for (int i = 0; i < DataReader.FieldCount; i++)
                {
                    sheetMap.Add(DataReader.GetString(i), i);
                }
                // Select a filler function (sheetFiller) based on sheet name (DataReader.Name)
                // such as FillProtocols for sheet name "Protocols"
                Filler sheetFiller = fillers[DataReader.Name];
                // Run the selected filler function until end of sheet
                while (sheetFiller(sheetMap)){}
                // Go to next sheet
            } while (DataReader.NextResult());
            DataReader.Close();
        }
        public void Generate(string protocolName)
        {
            Load();
            DumpDictionary(Protocols);
            Data = Protocols[protocolName].Generate(this);
        }



        // move this function into base class for all helper classes
        private void DumpDictionary(Dictionary<string, Protocol> dict)
        {
            foreach (KeyValuePair<string, Protocol> item in dict)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("KEY: ");
                sb.Append(item.Key);
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(item.Value))
                {
                    string name = descriptor.Name;
                    object value = descriptor.GetValue(item.Value);
                    sb.Append("\n");
                    sb.Append(name);
                    sb.Append(": ");
                    sb.Append(value);
                }
                Log.Error(sb.ToString());
            }
        }

        #region Data Filler Functions


        Protocol lastProtocol;
        private bool FillProtocol(Dictionary<string, int> map)
        {
            if (DataReader.Read())
            {
                if (DataReader.GetString(map["Protocol"]) != null)
                {
                    Protocol protocol = new Protocol();
                    protocol.Name = DataReader.GetString(map["Protocol"]);
                    protocol.Description = DataReader.GetString(map["Description"]);
                    protocol.Owner = DataReader.GetString(map["Owner"]);
                    protocol.Sound_1 = DataReader.GetString(map["Sound 1"]);
                    protocol.Sound_2 = DataReader.GetString(map["Sound 2"]);
                    protocol.PresoundCount = Convert.ToInt32(DataReader.GetDouble(map["Number of Presounds (each)"]));
                    protocol.SoundCount = Convert.ToInt32(DataReader.GetDouble(map["Number of Sounds (each)"]));
                    protocol.IntersoundIntervalMin = Convert.ToInt32(1000*DataReader.GetDouble(map["Inter-sound Interval Minimum (seconds)"]));
                    protocol.InterSoundIntervalMax = Convert.ToInt32(1000*DataReader.GetDouble(map["Inter-sound Interval Maximum (seconds)"]));
                    protocol.ExtraTime = Convert.ToInt32(1000*DataReader.GetDouble(map["Extra Time (seconds)"]));
                    lastProtocol = protocol;
                    Protocols.Add(protocol.Name, protocol);
                }
                else if (DataReader.GetString(map["Stimulus"]) != null)
                {
                    lastProtocol.AddStimulus(new Stimulus()
                    {
                        Name = DataReader.GetString(map["Stimulus"]),
                        SoundGroup = Convert.ToInt32(DataReader.GetDouble(map["Sound Group"])),
                        StimulusSoundPairing = DataReader.GetString(map["Stimulus-Sound Pairing"]),
                        IntraSoundDelivery = DataReader.GetString(map["Intra-Sound Stimulus Delivery"]),
                        DelayMinimumMs = Convert.ToInt32(DataReader.GetDouble(map["Stimulus Delay Minimum (ms)"])),
                        DelayMaximumMs = Convert.ToInt32(DataReader.GetDouble(map["Stimulus Delay Maximum (ms)"])),
                        DurationMinimumMs = Convert.ToInt32(DataReader.GetDouble(map["Stimulus Duration Minimum (ms)"])),
                        DurationMaximumMs = Convert.ToInt32(DataReader.GetDouble(map["Stimulus Duration Maximum (ms)"])),
                        NumPairedSounds = Convert.ToInt32(DataReader.GetDouble(map["Number of Paired Sounds"])),
                        StimuliPerSound = Convert.ToInt32(DataReader.GetDouble(map["Stimulus Repetitions Per Sound"])),
                        InterStimulusIntervalMin = Convert.ToInt32(DataReader.GetDouble(map["Inter-Stimulus Interval Minimum"])),
                        InterStimulusIntervalMax = Convert.ToInt32(DataReader.GetDouble(map["Inter-Stimulus Interval Maximum"]))
                    });
                }
                else
                {
                    // whitespace
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool FillSound(Dictionary<string, int> map)
        {
            if (DataReader.Read())
            {
                if (DataReader.GetString(map["Sound"]) != null)
                {
                    Sound sound = new Sound()
                    {
                        Name = DataReader.GetString(map["Sound"]),
                        Handler = DataReader.GetString(map["Handler"]),
                        BehaviorPin = DataReader.GetString(map["Behavior_Pin"]),
                        DurationPin = DataReader.GetString(map["Duration_Pin"]),
                        SoundID = DataReader.GetString(map["Sound_ID"]),
                        Duration = Convert.ToInt32(1000*DataReader.GetDouble(map["Duration (seconds)"]))
                    };
                    Sounds.Add(sound.Name, sound);
                }
                else
                {
                    // whitespace
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool FillStimulator(Dictionary<string, int> map)
        {
            if (DataReader.Read())
            {
                if (DataReader.GetString(map["Stimulator"]) != null)
                {
                    Stimulator stimulator = new Stimulator()
                    {
                        Name = DataReader.GetString(map["Stimulator"]),
                        Handler = DataReader.GetString(map["Handler"]),
                        BehaviorPin = DataReader.GetString(map["Behavior_Pin"]),
                        DurationPin = DataReader.GetString(map["Duration_Pin"]),
                    };
                    Stimulators.Add(stimulator.Name, stimulator);
                }
                else
                {
                    // whitespace
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool FillDelivery(Dictionary<string, int> map)
        {
            if (DataReader.Read())
            {
                if (DataReader.GetString(map["Delivery"]) != null)
                {
                    Delivery delivery = new Delivery()
                    {
                        Name = DataReader.GetString(map["Delivery"]),
                        Handler = DataReader.GetString(map["Handler"]),
                    };
                    Deliveries.Add(delivery.Name, delivery);
                }
                else
                {
                    // whitespace
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool FillPairing(Dictionary<string, int> map)
        {
            if (DataReader.Read())
            {
                if (DataReader.GetString(map["Pairing"]) != null)
                {
                    Pairing pairing = new Pairing()
                    {
                        Name = DataReader.GetString(map["Pairing"]),
                        Handler = DataReader.GetString(map["Handler"]),
                    };
                    Pairings.Add(pairing.Name, pairing);
                }
                else
                {
                    // whitespace
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Helper Classes: Protocol, Stimulus, Sound, Stimulator, Delivery, Pairing


        internal class Protocol
        {
            List<Stimulus> Stimuli;
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
            public Sound SoundRef_1 { get; set; }
            public Sound SoundRef_2 { get; set; }
            public Protocol()
            {
                Stimuli = new List<Stimulus>();
            }
            public void AddStimulus(Stimulus toAdd)
            {
                Stimuli.Add(toAdd);
            }
            public void GetRefs(LegacyAFCInterpreter interpreter)
            {
                SoundRef_1 = interpreter.Sounds[Sound_1];
                if(sound_2 != null)
                    SoundRef_2 = interpreter.Sounds[Sound_2];
                foreach(Stimulus stim in Stimuli)
                {
                    stim.GetRefs(interpreter);
                }
            }
            public List<DriveData> Generate(LegacyAFCInterpreter interpreter)
            {
                GetRefs(interpreter);
                List<DriveData> events = new List<DriveData>();

                Random random = new Random();
                int timeMs = ExtraTime;

                for (int i = 0; i < presoundCount; i++)
                {
                    events.Add(new DriveData(SoundRef_1.Handler,
                       new KeyValuePair<string, string>("SignalPin", SoundRef_1.BehaviorPin),
                       new KeyValuePair<string, string>("DurationPin", SoundRef_1.DurationPin),
                       new KeyValuePair<string, string>("Value", SoundRef_1.SoundID),
                       new KeyValuePair<string, string>("TimeStartMs", timeMs.ToString()),
                       new KeyValuePair<string, string>("TimeEndMs", (timeMs + SoundRef_1.Duration).ToString())));
                    timeMs += SoundRef_1.Duration + random.Next(IntersoundIntervalMin, InterSoundIntervalMax);
                }
                for (int i = 0; i < soundCount; i++)
                {
                    events.Add(new DriveData(SoundRef_1.Handler,
                       new KeyValuePair<string, string>("SignalPin", SoundRef_1.BehaviorPin),
                       new KeyValuePair<string, string>("DurationPin", SoundRef_1.DurationPin),
                       new KeyValuePair<string, string>("Value", SoundRef_1.SoundID),
                       new KeyValuePair<string, string>("TimeStartMs", timeMs.ToString()),
                       new KeyValuePair<string, string>("TimeEndMs", (timeMs + SoundRef_1.Duration).ToString())));
                    foreach(Stimulus stim in Stimuli)
                    {
                        // this only does stimulus for SOUND 1, needs updating for SOUND 2
                        events.AddRange(stim.Generate(timeMs, SoundRef_1.Duration, 1));
                    }
                    timeMs += SoundRef_1.Duration + random.Next(IntersoundIntervalMin, InterSoundIntervalMax);
                }
                return events;
            }
        }

        internal class Stimulus
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
            public Stimulator StimulatorRef { get; set; }
            public Delivery IntraSoundDeliveryRef { get; set; }
            public Pairing StimulusSoundPairingRef { get; set; }
            public void GetRefs(LegacyAFCInterpreter interpreter)
            {
                StimulatorRef = interpreter.Stimulators[Name];
                IntraSoundDeliveryRef = interpreter.Deliveries[IntraSoundDelivery];
                StimulusSoundPairingRef = interpreter.Pairings[StimulusSoundPairing];
            }
            public List<DriveData> Generate(int timeMs, int soundDuration, int sound)
            {
                List<DriveData> events = new List<DriveData>();
                Random random = new Random();
                timeMs += random.Next(DelayMinimumMs, DelayMaximumMs);
                if (sound == SoundGroup)
                {
                    // THIS IS FRONT PAIRING
                    for (int i = 0; i < stimuliPerSound; i++)
                    {
                        int duration = random.Next(DurationMinimumMs, DurationMaximumMs);
                        events.Add(new DriveData(StimulatorRef.Handler,
                           new KeyValuePair<string, string>("SignalPin", StimulatorRef.BehaviorPin),
                           new KeyValuePair<string, string>("DurationPin", StimulatorRef.DurationPin),
                           new KeyValuePair<string, string>("TimeStartMs", timeMs.ToString()),
                           new KeyValuePair<string, string>("TimeEndMs", (timeMs + duration).ToString())));
                        timeMs += duration;
                        timeMs += random.Next(interStimulusIntervalMin, interStimulusIntervalMax);
                    }
                }
                return events;
            }
        }

        internal class Sound
        {
            private string name;
            private string behaviorPin;
            private string handler;
            private string durationPin;
            private string soundID;
            private int duration;

            public string Name { get => name; set => name = value; }
            public string BehaviorPin { get => behaviorPin; set => behaviorPin = value; }
            public string Handler { get => handler; set => handler = value; }
            public string DurationPin { get => durationPin; set => durationPin = value; }
            public string SoundID { get => soundID; set => soundID = value; }
            public int Duration { get => duration; set => duration = value; }
        }

        internal class Stimulator
        {
            private string name;
            private string handler;
            private string behaviorPin;
            private string durationPin;

            public string Name { get => name; set { if (value != null) name = value; else throw new System.ArgumentException("Name cannot be null"); } }
            public string Handler { get => handler; set => handler = value; }
            public string BehaviorPin { get => behaviorPin; set => behaviorPin = value; }
            public string DurationPin { get => durationPin; set => durationPin = value; }
        }


        internal class Delivery
        {
            private string name;
            private string handler;

            public string Name { get => name; set { if (value != null) name = value; else throw new System.ArgumentException("Name cannot be null"); } }
            public string Handler { get => handler; set => handler = value; }
        }

        internal class Pairing
        {
            private string name;
            private string handler;

            public string Name { get => name; set { if (value != null) name = value; else throw new System.ArgumentException("Name cannot be null"); } }
            public string Handler { get => handler; set => handler = value; }
        }
        #endregion
    }


}
