using ProtocolMasterCore.Prompt;
using ProtocolMasterCore.Protocol;
using ProtocolMasterCore.Protocol.Interpreter;
using Schedulino.Generator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        HashSet<string> subjects;
        public UserSelectHandler UserSelectPrompt { get; set; }
        public UserNumberHandler UserNumberPrompt { get; set; }
        public SpreadsheetAFC()
        {
            mappedRowReaders = new Dictionary<string, RowReader>(){
                { "Experiment", ReadExperimentRow },
                { "Protocols", ReadProtocolRow },
                { "Stims", ReadStimsRow },
                { "Subjects", ReadSubjectsRow },
                { "SoundConfig", ReadSoundConfigRow },
                { "StimConfig", ReadStimConfigRow }
            };
            protocols = new Dictionary<string, Protocol>();
            subjects = new HashSet<string>();
        }
        public bool IsCanceled { get; set; }

        string protocolName = null;
        string experimentName = null;
        string subjectID = "";
        public string ProtocolLabel => $"{(experimentName ?? "NoExperiment")}_{(protocolName ?? "NoProtocol")}_{subjectID:D2}";

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
            experimentName = new Regex("\\(\\d+\\)|_| ").Replace(argument, "");
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
                        if (subjects.Count >= 1) ;
                        subjectID = UserSelectPrompt(subjects.ToArray(), "Select a Subject:");
                        return protocols[protocolName].Generate();
                    }
                    else return null;
                }
                else
                {
                    return protocols[protocolName].Generate();
                }
            }
            else return null;
        }
        private string ReadStringAt(Dictionary<string, int> headerMap, string headerKey, bool acceptNull)
        {
            if (headerMap.ContainsKey(headerKey))
            {
                int key = headerMap[headerKey];
                if (DataReader.IsDBNull(key))
                    if (acceptNull) return null;
                    else throw new FormatException($"Value at \"{headerKey}\" in sheet \"{DataReader.Name}\" not present. A value must be entered.");
                else return DataReader.GetValue(key).ToString();
            }
            else throw new FormatException($"Column header \"{headerKey}\" not in sheet \"{DataReader.Name}\". Column header and sheet names must match exactly.");
        }
        private uint ReadIntAt(Dictionary<string, int> headerMap, string headerKey, bool acceptNull)
        {
            string resultString = ReadStringAt(headerMap, headerKey, acceptNull);
            if (resultString == null)
                if (acceptNull) return 0;
                else throw new FormatException($"Value at \"{headerKey}\" in sheet \"{DataReader.Name}\" not present. A value must be entered.");
            else
            {
                if (uint.TryParse(resultString, out uint n))
                    return n;
                else throw new FormatException($"Value \"{resultString}\" at \"{headerKey}\" in sheet \"{DataReader.Name}\" could not be converted to an integer. A number must be entered.");
            }
        }
        private bool ReadExperimentRow(Dictionary<string, int> headerMap)
        {
            if (DataReader.Read())
            {
                baseData = new Protocol();
                baseData.owner = ReadStringAt(headerMap, "Owner", true);
                baseData.sound_order = ReadStringAt(headerMap, "Sound Order", true);
                baseData.Sounds.Add(new Sound(ReadStringAt(headerMap, "Sound A", false)));
                if (!DataReader.IsDBNull(headerMap["Sound B"]))
                    baseData.Sounds.Add(new Sound(ReadStringAt(headerMap, "Sound B", false)));
                baseData.interval_min = ReadIntAt(headerMap, "Inter-sound Interval Minimum (ms)", false);
                baseData.interval_max = ReadIntAt(headerMap, "Inter-sound Interval Maximum (ms)", false);
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
                    Protocol protocol = GetOrCreateProtocol(ReadStringAt(headerMap, "Protocol", false));
                    protocol.num_presounds = ReadIntAt(headerMap, "Number of Presounds (each)", true);
                    protocol.num_exp_sounds = ReadIntAt(headerMap, "Number of Sounds (each)", false);
                    protocol.pre_time = ReadIntAt(headerMap, "Pre-Time (ms)", true);
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
                    Protocol protocol = GetOrCreateProtocol(ReadStringAt(headerMap, "Protocol", false));
                    if (DataReader.IsDBNull(0)) return true;
                    Stimulus stim = new Stimulus();
                    stim.name = ReadStringAt(headerMap, "Stimulus", false);
                    stim.sound_group = ReadStringAt(headerMap, "Sound Group", false);
                    stim.stim_sound_pairing = ReadStringAt(headerMap, "Stim-Sound Pairing", false);
                    stim.stim_sound_window = ReadStringAt(headerMap, "Stim-Sound Window", false);
                    stim.stim_delivery = ReadStringAt(headerMap, "Intra-Window Stim Delivery", false);

                    stim.num_paired_sounds = ReadIntAt(headerMap, "Number of Paired Sounds", false);
                    stim.stims_per_sound = ReadIntAt(headerMap, "Stim Repetitions Per Window", false);
                    stim.dur_min = ReadIntAt(headerMap, "Stim Min Duration (ms)", false);
                    stim.dur_max = ReadIntAt(headerMap, "Stim Max Duration (ms)", false);
                    stim.interval_min = ReadIntAt(headerMap, "Time Between Stims Min", true);
                    stim.interval_max = ReadIntAt(headerMap, "Time Between Stims Max", true);
                    stim.delay_min = ReadIntAt(headerMap, "Stimulus Delay Min (ms)", true);
                    stim.delay_max = ReadIntAt(headerMap, "Stimulus Delay Max (ms)", true);

                    stim.log_mode = ReadStringAt(headerMap, "Log", false);

                    stim.sound = protocol.Sounds.Find(a => a.name == stim.sound_group);
                    if (stim.sound == null) throw new Exception($"Stimulus \"{stim.name}\" requires sound \"{stim.sound}\", but that sound is not included in the experiment.");
                    stim.sound.stimuli.Add(stim);

                    protocol.Stimuli.Add(stim);
                }
                return true;
            }
            else return false;
        }
        private bool ReadSubjectsRow(Dictionary<string, int> headerMap)
        {
            if (DataReader.Read())
            {
                if (!DataReader.IsDBNull(headerMap["Subject"]))
                {
                    string name = ReadStringAt(headerMap, "Subject", false);
                    if (subjects.Contains(name))
                        throw new Exception($"Subject name \"{name}\" is not unique in sheet \"{DataReader.Name}\". All subject names must be unique.");
                    else subjects.Add(name);
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
                string name = ReadStringAt(headerMap, "Sound", false);
                foreach (KeyValuePair<string, Protocol> kvp in protocols)
                {
                    Sound sound = kvp.Value.Sounds.Find(a => a.name == name);
                    if (sound != null)
                    {
                        sound.handler = ReadStringAt(headerMap, "Handler", false);
                        sound.behavior_pin = ReadStringAt(headerMap, "Behavior_Pin", false);
                        sound.duration_pin = ReadStringAt(headerMap, "Duration_Pin", false);

                        sound.duration = ReadIntAt(headerMap, "Duration (ms)", true);
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
                string name = ReadStringAt(headerMap, "Stimulator", false);
                foreach (KeyValuePair<string, Protocol> kvp in protocols)
                {
                    IEnumerable<Stimulus> stims = kvp.Value.Stimuli.Where(a => a.name == name);
                    if (stims != null)
                    {
                        string handler = ReadStringAt(headerMap, "Handler", false);
                        string behavior_pin = ReadStringAt(headerMap, "Behavior_Pin", false);
                        string duration_pin = ReadStringAt(headerMap, "Duration_Pin", false);
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
