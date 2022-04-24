using System;
using Xamarin.Essentials;

namespace sample.Droid;

public class RegionMonitor : IRegionMonitor
{
    public event EventHandler<string> MonitorNotifications;
    
    private const double RADIUS = 3;//radius of region circle to monitor

    private Location _regionCenter;
    private bool _enterRegion;
    
    public async void StartRegionUpdates()
    {
        var location = await Geolocation.GetLocationAsync() ?? await Geolocation.GetLastKnownLocationAsync();
        _regionCenter = new(location.Latitude + 0.01, location.Longitude - 0.01);
    }

    public void MonitorRegion(Location location)
    {
        //this is really simple, but good test example of https://developer.android.com/training/location/geofencing
        //under that documentation shown more complex examples but I don't know how
        //far you need to go with that, this solution should cover most base scenarios ;) 
        _regionCenter ??= location;
        if (_enterRegion)
        {
            var distance = Location.CalculateDistance(_regionCenter ,location, DistanceUnits.Kilometers);

            if (distance <= RADIUS)
            {
                OnMonitorNotifications($"Still Inside Region {location.Latitude:N6} {location.Longitude:N6} {DateTime.Now.ToString("hh:mm:ss")}");
            }
            else
            {
                OnMonitorNotifications($"On Region Left {location.Latitude:N6} {location.Longitude:N6} {DateTime.Now.ToString("hh:mm:ss")}");
                _enterRegion = false;
            }
        }
        else
        {
            var distance = Location.CalculateDistance(_regionCenter ,location, DistanceUnits.Kilometers);

            if(distance <= RADIUS)
            {
                OnMonitorNotifications($"On Region Entered {location.Latitude:N6} {location.Longitude:N6} {DateTime.Now.ToString("hh:mm:ss")}");
                _enterRegion = true;
            }
            else
            {
                OnMonitorNotifications($"Still Out of Region {location.Latitude:N6} {location.Longitude:N6} {DateTime.Now.ToString("hh:mm:ss")}");
            }
        }
    }

    protected virtual void OnMonitorNotifications(string e)
    {
        MonitorNotifications?.Invoke(this, e);
    }
}