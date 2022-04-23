using System;
using System.Linq;
using System.Threading.Tasks;
using CoreLocation;
using UIKit;
using Xamarin.Essentials;

namespace sample.iOS;

public class LocationBackgroundWorker : ILocationBackgroundWorker
{
    /// <summary>
    /// Location Manager Desired Accuracy in meters
    /// </summary>
    private const int LOC_MGR_DESIRED_ACCURACY = 100;

    /// <summary>
    /// Last Known Location from iOS
    /// </summary>
    private CLLocation _lastKnownLocation;

    /// <summary>
    /// Last Updated Time
    /// </summary>
    private DateTime _lastUpdatedTime;

    /// <summary>
    /// Local Location Manager
    /// </summary>
    private CLLocationManager _locMgr;

    /// <summary>
    /// Raised when only Work with Result Completed
    /// </summary>
    public event EventHandler<Location> LocationUpdated;

    public void StartLocationUpdates(TimeSpan interval)
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

        if (CLLocationManager.LocationServicesEnabled)
        {
            _ = Task.Run(() =>
            {
                //set the desired accuracy, in meters
                _locMgr.DesiredAccuracy = LOC_MGR_DESIRED_ACCURACY;
                _locMgr.LocationsUpdated += (_, args) =>
                {
                    _lastKnownLocation = args.Locations.Last();
                    if (DateTime.UtcNow - _lastUpdatedTime > interval)
                    {
                        _lastUpdatedTime = DateTime.UtcNow;
                        OnLocationUpdated(new(
                            _lastKnownLocation.Coordinate.Latitude,
                            _lastKnownLocation.Coordinate.Longitude));
                    }
                };
                _locMgr.StartUpdatingLocation();
            });
        }
    }

    protected virtual void OnLocationUpdated(Location e)
    {
        LocationUpdated?.Invoke(this, e);
    }
}