using System.Collections.Generic;

namespace ProtocolMasterCore.Protocol.Driver
{
    public enum DriverProgress
    {
        LOADING,
        READY,
        PRERUN,
        RUNNING,
        DONE,
        CANCELLED
    }

    /// <summary>
    /// Primary interface for Drivers
    /// Functions should be executed in order ProcessData() -> Run()
    /// Cancel should be used as a callback to safely stop (such as properly closing IO streams) when driver is running in another thread.
    /// </summary>
    public interface IDriver : IExtension
    {
        /// <summary>
        /// Data processing function, this takes DriverData and converts it into hardware-compatible data
        /// </summary>
        /// <param name="dataList">List of DriverData for the driver to handle, such as events or behaviors</param>
        bool Setup(List<ProtocolEvent> dataList);
        /// <summary>
        /// Driver Run Function, this is called after all Data Processing is complete
        /// </summary>
        void Start();

    }
}
