using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ProtocolMaster.Model.Protocol.Driver
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
    public interface IDriver
    {
        ConcurrentQueue<VisualData> VisualData { get; }
        Progress<DriverProgress> CurrentProgress { get; set; }
        /// <summary>
        /// Data processing function, this takes DriverData and converts it into hardware-compatible data
        /// </summary>
        /// <param name="dataList">List of DriverData for the driver to handle, such as events or behaviors</param>
        void ProcessData(List<DriveData> dataList);
        /// <summary>
        /// Driver Run Function, this is called after all Data Processing is complete
        /// </summary>
        void Run();
        /// <summary>
        /// Cancellation function, used as a callback in a CancellationToken in case Driver is ended early.
        /// </summary>
        void Cancel();
    }
}
