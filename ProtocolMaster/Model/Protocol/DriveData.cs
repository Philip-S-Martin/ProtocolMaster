using System.Collections.Generic;

namespace ProtocolMaster.Model.Protocol
{
    public class DriveData
    {
        public string Handler { get; private set; }
        public string CategoryLabel { get; private set; }
        public bool HasCategory { get; private set; }
        public Dictionary<string, string> Arguments { get; private set; }

        public DriveData(string handler, params KeyValuePair<string, string>[] args)
        {
            this.Handler = handler;
            Arguments = new Dictionary<string, string>();
            foreach(KeyValuePair<string, string> arg in args)
            {
                Arguments.Add(arg.Key, arg.Value);
            }
            HasCategory = false;
        }

        public DriveData(string handler, string categoryLabel, params KeyValuePair<string, string>[] args) : this(handler, args)
        {
            this.CategoryLabel = categoryLabel;
            HasCategory = true;
        }
        /*
        private int loadOrder;
        private Dictionary<string, object> properties;

        public int LoadOrder { get => loadOrder; private set => loadOrder = value; }
        public Dictionary<string, object> Properties { get => properties; private set => properties = value; }*/
        // example driver = {"Schedulino"}, properties = {<"time_ms", 1000>, <"pin", 5>, <"state", 1>} 
        // would be interpreted as: 
        // - at 1 second (1000ms) into the protocol, change the state of pin 5 to HIGH
        // in the Schedulino driver.

        // The driver MUST be looking for events with these keys and correct value types, if it recieves
        // unexpected data it should have the ability to find & throw errors.
    }
}
