using System;

namespace sample.Droid;

public class RegionMonitor : IRegionMonitor
{
    public event EventHandler<string> MonitorNotifications;
    public void StartRegionUpdates()
    {
        
    }
}