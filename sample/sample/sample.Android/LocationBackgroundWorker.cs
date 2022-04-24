using System;
using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.Essentials;

namespace sample.Droid;

public class LocationBackgroundWorker : ILocationBackgroundWorker
{
    public event EventHandler<Location> LocationUpdated;
    public event EventHandler WorkerStopped;

    public TimeSpan Interval { get; private set; }
    
    public void StartLocationUpdates(TimeSpan interval)
    {
        Interval = interval;
        var intent = new Intent(Application.Context, typeof(LocationBackgroundService));

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            Application.Context.StartForegroundService(intent);
        }
        else
        {
            Application.Context.StartService(intent);
        }
    }

    public void StopWorker()
    {
        OnWorkerStopped();
    }

    public void OnLocationUpdated(Location e)
    {
        LocationUpdated?.Invoke(this, e);
    }

    protected virtual void OnWorkerStopped()
    {
        WorkerStopped?.Invoke(this, EventArgs.Empty);
    }
}