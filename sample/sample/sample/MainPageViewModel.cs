using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace sample;

public class MainPageViewModel
{
    private readonly ILocationBackgroundWorker _locationBackgroundWorker;
    private readonly IPermissionHandler _permissionHandler;

    public ICommand StartUpdatesCommand { get; }

    public ObservableCollection<string> LocationUpdates { get; }

    public MainPageViewModel(
        ILocationBackgroundWorker locationBackgroundWorker,
        IPermissionHandler permissionHandler)
    {
        _locationBackgroundWorker = locationBackgroundWorker;
        _permissionHandler = permissionHandler;
        _locationBackgroundWorker.LocationUpdated+= LocationBackgroundWorkerOnLocationUpdated;
        StartUpdatesCommand = new Command(ExecuteStartUpdates);
        LocationUpdates = new ObservableCollection<string>();
    }

    private void LocationBackgroundWorkerOnLocationUpdated(object sender, Location e)
    {
        //Also we may send from Event Time when it was raised, I don't make it in my example 
        //and log it here, but you can change that and send in even immediately ;)
        Device.BeginInvokeOnMainThread(() =>
        {
            LocationUpdates.Add($"{e.Latitude} {e.Longitude} {DateTime.Now.ToString("hh:mm:ss")}");
        });
        //todo Add here your API Request
    }

    private void ExecuteStartUpdates()
    {
        _permissionHandler.RequestPermission<Permissions.LocationAlways>();
        _locationBackgroundWorker.StartLocationUpdates(TimeSpan.FromSeconds(5));
    }
}