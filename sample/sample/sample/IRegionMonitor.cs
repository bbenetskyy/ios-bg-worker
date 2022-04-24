using System;

namespace sample;

public interface IRegionMonitor
{
    /// <summary>
    /// Raised when we got any region changes
    /// </summary>
    event EventHandler<string> MonitorNotifications;
    //it's better for you to replace string to something
    //more specific like class with all required fields,
    //I will simplify that to just string because it's ok
    //according to your requirements ;)

    void StartRegionUpdates();
}