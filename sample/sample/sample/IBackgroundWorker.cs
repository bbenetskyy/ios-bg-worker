using System;
using System.Threading.Tasks;

namespace sample;

/// <summary>
/// Background Worker with delayed Execution
/// </summary>
public interface IBackgroundWorker
{
    /// <summary>
    /// Event Raised when BG Worker has been Stopped
    /// </summary>
    event EventHandler WorkerStopped;
    
    /// <summary>
    /// Represent BG Work
    /// </summary>
    Func<Task> BackgroundWork { get; }
    
    /// <summary>
    /// Start Specific BG Work
    /// </summary>
    void StartWorker(Func<Task> backgroundWork);
    
    /// <summary>
    /// Force Stop BG Work Execution
    /// </summary>
    void StopWorker();

    /// <summary>
    /// Identify if BG Work has any data to upload or download
    /// </summary>
    Task<bool> IsDataAvailableToSync();
}