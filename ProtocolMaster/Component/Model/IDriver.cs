using System;

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
