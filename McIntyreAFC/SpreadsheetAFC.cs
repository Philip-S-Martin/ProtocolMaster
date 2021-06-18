using ProtocolMasterCore.Prompt;
using ProtocolMasterCore.Protocol;
using ProtocolMasterCore.Protocol.Interpreter;
using Schedulino.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace McIntyreAFC
{
    [InterpreterMeta("SpreadsheetAFC", "1.1")]

    public class SpreadsheetAFC : ExcelDataInterpreter, IInterpreter, IPromptUserSelect, IPromptUserNumber
    {
        private delegate bool RowReader(Dictionary<string, int> map);
        Dictionary<string, RowReader> mappedRowReaders;
        Dictionary<string, Protocol> protocols;
        Protocol baseData;
        public UserSelectHandler UserSelectPrompt { get; set; }
        public UserNumberHandler UserNumberPrompt { get; set; }
        public SpreadsheetAFC()
        {
            mappedRowReaders = new Dictionary<string, RowReader>(){
                { "Experiment", ReadExperimentRow },
                { "Protocols", ReadProtocolRow },
                { "Stims", ReadStimsRow },
                { "SoundConfig", ReadSoundConfigRow },
                { "StimConfig", ReadStimConfigRow }
            };
            protocols = new Dictionary<string, Protocol>();
        }
        public bool IsCanceled { get; set; }

        string protocolName = null;
        string experimentName = null;
        int subjectNumber = 0;
        public string ProtocolLabel => $"{(experimentName != null ? experimentName : "NoExperiment")}_{(protocolName != null ? protocolName : "NoProtocol")}_{subjectNumber:D2}";

        private Protocol GetOrCreateProtocol(string protocolName)
        {
            Protocol result;
            if (protocols.TryGetValue(protocolName, out result))
                return result;
            else
            {
                result = baseData.CloneProtocolWithoutEvents();
                result.name = protocolName;
                protocols.Add(protocolName, result);
                return result;
            }
        }
        public List<ProtocolEvent> Generate(string argument)
        {
            experimentName = new Regex("\\(\\d+\\)|_| ").Replace( argument, "");
            if (DataReader == null) return null;
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
                RowReader readRow;
                if (mappedRowReaders.ContainsKey(DataReader.Name))
                    readRow = mappedRowReaders[DataReader.Name];
                else readRow = ReadNothing;
                // Run the selected filler function until end of sheet
                while (readRow(sheetMap)) { }
                // Go to next sheet
            } while (DataReader.NextResult());
            DataReader.Close();

            if (!IsCanceled)
            {
                if (protocolName == null)
                {
                    protocolName = UserSelectPrompt(protocols.Keys.ToArray(), "Select a Protocol:");
                    if (protocolName != null)
                    {
                        subjectNumber = UserNumberPrompt(1, 99, "Select a Subject:");
                        return protocols[protocolName].Generate();
                    }
                    else return null;
                }
                else
                {
                    {
                        return protocols[protocolName].Generate();
                    }
                }
            }
            else return null;
        }
        private bool ReadExperimentRow(Dictionary<string, int> headerMap)
        {
            if (DataReader.Read())
            {
                baseData = new Protocol();
                baseData.owner = DataReader.GetValue(headerMap["Owner"]).ToString();
                baseData.sound_order = DataReader.GetValue(headerMap["Sound Order"]).ToString();
                
                string extra_time_str = DataReader.GetValue(headerMap["Extra Time (ms)"]).ToString();
                string interval_min_str = DataReader.GetValue(headerMap["Inter-sound Interval Minimum (ms)"]).ToString();
                string interval_max_str = DataReader.GetValue(headerMap["Inter-sound Interval Maximum (ms)"]).ToString();

                baseData.Sounds.Add(new Sound(DataReader.GetValue(headerMap["Sound A"]).ToString()));
                if (!DataReader.IsDBNull(headerMap["Sound B"]))
                    baseData.Sounds.Add(new Sound(DataReader.GetValue(headerMap["Sound B"]).ToString()));

                baseData.extra_time = Convert.ToUInt32(extra_time_str);
                baseData.interval_min = Convert.ToUInt32(interval_min_str);
                baseData.interval_max = Convert.ToUInt32(interval_max_str);

                return true;
            }
            else return false;
        }
        private bool ReadProtocolRow(Dictionary<string, int> headerMap)
        {
            if (DataReader.Read())
            {
                if (!DataReader.IsDBNull(headerMap["Protocol"]))
                {
                    string name = DataReader.GetValue(headerMap["Protocol"]).ToString();
                    Protocol protocol = GetOrCreateProtocol(name);

                    string presounds_str = DataReader.GetValue(headerMap["Number of Presounds (each)"]).ToString();
                    string sounds_str = DataReader.GetValue(headerMap["Number of Sounds (each)"]).ToString();


                    protocol.num_presounds = Convert.ToUInt32(presounds_str);
                    protocol.num_exp_sounds = Convert.ToUInt32(sounds_str);
                }
                return true;
            }
            else return false;
        }
        private bool ReadStimsRow(Dictionary<string, int> headerMap)
        {
            if (DataReader.Read())
            {
                if (!DataReader.IsDBNull(headerMap["Protocol"]))
                {
                    string name = DataReader.GetValue(headerMap["Protocol"]).ToString();
                    Protocol protocol = GetOrCreateProtocol(name);

                    if (DataReader.IsDBNull(0)) return true;
                    Stimulus stim = new Stimulus();
                    stim.name = DataReader.GetValue(headerMap["Stimulus"]).ToString();
                    stim.sound_group = DataReader.GetValue(headerMap["Sound Group"]).ToString();
                    stim.stim_sound_pairing = DataReader.GetValue(headerMap["Stim-Sound Pairing"]).ToString();
                    stim.stim_sound_window = DataReader.GetValue(headerMap["Stim-Sound Window"]).ToString();
                    stim.stim_delivery = DataReader.GetValue(headerMap["Intra-Window Stim Delivery"]).ToString();

                    // This is a very hacky way to handle timeline integer underflow, but it works
                    // It should probably be replaced by allowing intervals to have negative values
                    // and then increasing all intervals by the most negative value
                    if (stim.stim_sound_window == "Before")
                    {
                        protocol.GrowExtraTimeIfNeeded(stim.dur_max);
                    }

                    string paired_sounds_str = DataReader.GetValue(headerMap["Number of Paired Sounds"]).ToString();
                    string stims_per_sound_str = DataReader.GetValue(headerMap["Stim Repetitions Per Window"]).ToString();
                    string dur_min_str = DataReader.GetValue(headerMap["Stim Min Duration (ms)"]).ToString();
                    string dur_max_str = DataReader.GetValue(headerMap["Stim Max Duration (ms)"]).ToString();
                    string interval_min_str = DataReader.GetValue(headerMap["Time Between Stims Min"]).ToString();
                    string interval_max_str = DataReader.GetValue(headerMap["Time Between Stims Max"]).ToString();
                    string delay_min_str = DataReader.GetValue(headerMap["Stimulus Delay Min (ms)"]).ToString();
                    string delay_max_str = DataReader.GetValue(headerMap["Stimulus Delay Max (ms)"]).ToString();

                    stim.num_paired_sounds = Convert.ToUInt32(paired_sounds_str);
                    stim.stims_per_sound = Convert.ToUInt32(stims_per_sound_str);
                    stim.dur_min = Convert.ToUInt32(dur_min_str);
                    stim.dur_max = Convert.ToUInt32(dur_max_str);
                    stim.interval_min = Convert.ToUInt32(interval_min_str);
                    stim.interval_max = Convert.ToUInt32(interval_max_str);
                    stim.delay_min = Convert.ToUInt32(delay_min_str);
                    stim.delay_max = Convert.ToUInt32(delay_max_str);

                    stim.sound = protocol.Sounds.Find(a => a.name == stim.sound_group);
                    if (stim.sound == null) throw new NullReferenceException("Stimulus sound is invalid");
                    stim.sound.stimuli.Add(stim);

                    protocol.Stimuli.Add(stim);
                }
                return true;
            }
            else return false;
        }
        private bool ReadSoundConfigRow(Dictionary<string, int> headerMap)
        {
            if (DataReader.Read())
            {
                if (DataReader.IsDBNull(0)) return true;
                string name = DataReader.GetValue(headerMap["Sound"]).ToString();
                foreach (KeyValuePair<string, Protocol> kvp in protocols)
                {
                    Sound sound = kvp.Value.Sounds.Find(a => a.name == name);
                    if (sound != null)
                    {
                        sound.handler = DataReader.GetValue(headerMap["Handler"]).ToString();
                        sound.behavior_pin = DataReader.GetValue(headerMap["Behavior_Pin"]).ToString();
                        sound.duration_pin = DataReader.GetValue(headerMap["Duration_Pin"]).ToString();
                        sound.sound_id = DataReader.GetValue(headerMap["Sound_ID"]).ToString();

                        string duration_str = DataReader.GetValue(headerMap["Duration (ms)"]).ToString();

                        sound.duration = Convert.ToUInt32(duration_str);
                    }
                }
                return true;
            }
            return false;
        }
        private bool ReadStimConfigRow(Dictionary<string, int> headerMap)
        {
            if (DataReader.Read())
            {
                if (DataReader.IsDBNull(0)) return true;
                string name = DataReader.GetValue(headerMap["Stimulator"]).ToString();
                foreach (KeyValuePair<string, Protocol> kvp in protocols)
                {
                    IEnumerable<Stimulus> stims = kvp.Value.Stimuli.Where(a => a.name == name);
                    if (stims != null)
                    {
                        string handler = DataReader.GetValue(headerMap["Handler"]).ToString();
                        string behavior_pin = DataReader.GetValue(headerMap["Behavior_Pin"]).ToString();
                        string duration_pin = DataReader.GetValue(headerMap["Duration_Pin"]).ToString();
                        foreach (Stimulus stim in stims)
                        {
                            stim.handler = handler;
                            stim.behavior_pin = behavior_pin;
                            stim.duration_pin = duration_pin;
                        }
                    }
                }
                return true;
            }
            return false;
        }
        private bool ReadNothing(Dictionary<string, int> headerMap)
        {
            return false;
        }

    }


}
