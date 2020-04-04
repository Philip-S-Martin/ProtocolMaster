using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Model
{
    // Timeline pumps data from interpreters to drivers
    // Timelines are the CORE of the application!
    class Timeline
    {
        IEnumerable<IInterpreter> interpreters;
        IEnumerable<IDriver> drivers;
        Dictionary<Guid, Event> events;
        // events are sent to their drivers (each event may go to more than one),
        // in order, carrying their properties
    }
}
