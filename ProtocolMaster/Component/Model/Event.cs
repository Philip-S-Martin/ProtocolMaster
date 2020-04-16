using System.Collections.Generic;

namespace ProtocolMaster.Component.Model
{
    public class IInterpretedEvent
    {
        string symbol;
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
