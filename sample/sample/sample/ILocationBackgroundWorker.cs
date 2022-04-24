using System;
using Xamarin.Essentials;

namespace sample;

public interface ILocationBackgroundWorker
{
    /// <summary>
    /// Raised when only Work with Result Completed
    /// </summary>
    event EventHandler<Location> LocationUpdated;
    
    /// <summary>
    /// Event Raised when BG Worker has been Stopped
    /// </summary>
    public event EventHandler WorkerStopped;

    /// <summary>
    /// Update Interval for native platform implementation
    /// </summary>
    TimeSpan Interval { get; }

    void StartLocationUpdates(TimeSpan interval);
    
    /// <summary>
    /// Force Stop Location Execution
    /// </summary>
    void StopWorker();
}