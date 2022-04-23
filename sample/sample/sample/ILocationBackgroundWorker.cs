using System;
using Xamarin.Essentials;

namespace sample;

public interface ILocationBackgroundWorker
{
    /// <summary>
    /// Raised when only Work with Result Completed
    /// </summary>
    event EventHandler<Location> LocationUpdated;

    void StartLocationUpdates(TimeSpan interval);
}