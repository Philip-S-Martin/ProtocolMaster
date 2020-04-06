using System.Collections.Generic;

namespace ProtocolMaster.Component.Model
{
    class Event
    {
        int loadOrder;
        IEnumerable<string> drivers;
        Dictionary<string, object> properties;
        // example driver = {"Schedulino"}, properties = {<"time_ms", 1000>, <"pin", 5>, <"state", 1>} 
        // would be interpreted as: 
        // - at 1 second (1000ms) into the protocol, change the state of pin 5 to HIGH
        // in the Schedulino driver.

        // The driver MUST be looking for events with these keys and correct value types, if it recieves
        // unexpected data it should have the ability to find & throw errors.
    }
}
