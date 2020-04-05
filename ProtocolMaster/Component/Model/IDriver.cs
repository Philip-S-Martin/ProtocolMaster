using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Model
{
    public interface IDriverData
    {
        String Symbol { get; }
    }
    public interface IDriver
    {
        // some old junk, will most likely just require 
        // some communication functions.
        /*
        void Setup();
        void Loop();
        void Exit();
        SerialPort Serial { get; set; }
        */
    }
}
