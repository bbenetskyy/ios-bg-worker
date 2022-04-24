using System;
using System.Linq;
using System.Threading.Tasks;
using CoreLocation;
using UIKit;
using Xamarin.Essentials;

namespace sample.iOS;

public class RegionMonitor : IRegionMonitor
{
    public event EventHandler<string> MonitorNotifications;
    
    /// <summary>
    /// Local Location Manager
    /// </summary>
    private CLLocationManager _locMgr;

    private CLCircularRegion _region;
    
    
    public void StartRegionUpdates()
    {
        _locMgr = new CLLocationManager();
        _locMgr.PausesLocationUpdatesAutomatically = false;

        // iOS 8 has additional permissions requirements
        if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
        {
            _locMgr.RequestAlwaysAuthorization(); // works in background
        }

        if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
        {
            _locMgr.AllowsBackgroundLocationUpdates = true;
        }

        if (CLLocationManager.LocationServicesEnabled && CLLocationManager.IsMonitoringAvailable(typeof(CLCircularRegion)))
        {
            _ = Task.Run(() =>
                {
                    //set the desired accuracy, in meters
                    _locMgr.DesiredAccuracy = LocationBackgroundWorker.LOC_MGR_DESIRED_ACCURACY;

                    _locMgr.RegionEntered += OnRegionEntered;
                    _locMgr.RegionLeft += OnRegionLeft;
                    _locMgr.LocationsUpdated += OnLocationsUpdated;
                    
                    _locMgr.StartUpdatingLocation();
                }
            );
        }
    }

    public void MonitorRegion(Location location)
    {
        //this is not the best interface, because
        //ios is quite different from android
        //and for ios we don't need that at all
    }

    protected virtual void OnMonitorNotifications(string e)
    {
        MonitorNotifications?.Invoke(this, e);
    }

    private void OnLocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
    {
        var radius = 100;//radius of region circle to monitor
        var location = e.Locations.First();
        //create region outside our location to trigger enter region event
        location = new CLLocation(location.Coordinate.Latitude + 0.01, location.Coordinate.Longitude - 0.01);
        
        OnMonitorNotifications($"Create Region {location.Coordinate.Latitude:N6} {location.Coordinate.Longitude:N6} {DateTime.Now.ToString("hh:mm:ss")}");
        
        //todo this region should be from your API
        //just to test that it works I create from first location I've get
        _region = new CLCircularRegion(location.Coordinate, radius, "YourIdentifier");
        
        _locMgr.StartMonitoring(_region);
        _locMgr.StopUpdatingLocation();

        //todo to stop monitoring call _locMgr.StopMonitoring(_region);
        //and don't forget to ensure _region is not null before stop 
    }

    private void OnRegionEntered(object sender, CLRegionEventArgs e)
    {
        OnMonitorNotifications($"On Region Entered {e.Region.Identifier} {e.Region.Center.Latitude:N6} {e.Region.Center.Longitude:N6} {DateTime.Now.ToString("hh:mm:ss")}");
    }

    private void OnRegionLeft(object sender, CLRegionEventArgs e)
    {
        OnMonitorNotifications($"On Region Left {e.Region.Identifier} {e.Region.Center.Latitude:N6} {e.Region.Center.Longitude:N6} {DateTime.Now.ToString("hh:mm:ss")}");
    }
}