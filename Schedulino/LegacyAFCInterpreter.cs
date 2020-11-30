using ProtocolMaster.Model.Debug;
using ProtocolMaster.Model.Protocol;
using ProtocolMaster.Model.Protocol.Interpreter;
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
using Schedulino.InterpreterData;

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
    public class LegacyAFCInterpreter : ExcelDataInterpreter, IInterpreter
    {
        internal Dictionary<string, ProtocolData> Protocols { get; private set; }
        internal Dictionary<string, SoundData> Sounds { get; private set; }
        internal Dictionary<string, StimulatorData> Stimulators { get; private set; }
        internal Dictionary<string, DeliveryData> Deliveries { get; private set; }
        internal Dictionary<string, PairingData> Pairings { get; private set; }

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
            Protocols = new Dictionary<string, ProtocolData>();
            Sounds = new Dictionary<string, SoundData>();
            Stimulators = new Dictionary<string, StimulatorData>();
            Deliveries = new Dictionary<string, DeliveryData>();
            Pairings = new Dictionary<string, PairingData>();
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
                    string colName = DataReader.GetString(i);
                    if (colName != null)
                        sheetMap.Add(colName, i);
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
        public List<ProtocolEvent> Generate(string protocolName)
        {
            Load();
            DumpDictionary(Protocols);
            return Protocols[protocolName].Generate(this);
        }

        // move this function into base class for all helper classes
        private void DumpDictionary(Dictionary<string, ProtocolData> dict)
        {
            foreach (KeyValuePair<string, ProtocolData> item in dict)
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
                //Log.Error(sb.ToString());
            }
        }

        #region Data Filler Functions


        ProtocolData lastProtocol;
        private bool FillProtocol(Dictionary<string, int> map)
        {
            if (DataReader.Read())
            {
                if (DataReader.GetString(map["Protocol"]) != null)
                {
                    ProtocolData protocol = new ProtocolData();
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
                    lastProtocol.AddStimulus(new StimulusData()
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
                    SoundData sound = new SoundData()
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
                    StimulatorData stimulator = new StimulatorData()
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
                    DeliveryData delivery = new DeliveryData()
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
                    PairingData pairing = new PairingData()
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

    }


}
