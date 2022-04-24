using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content.Resources;
using Xamarin.Essentials;

namespace sample.Droid;

[Service]
public class LocationBackgroundService : Service
{
    /// <summary>
    /// The Id of the Service
    /// </summary>
    private const int SERVICE_ID = 5060;

    /// <summary>
    /// The string identifier for the service notification
    /// </summary>
    private const string SERVICE_NOTIFICATION_CHANNEL_ID = "5061";
    
    private readonly ILocationBackgroundWorker _locationBackgroundWorker;

    private bool _workerActive;
    private CancellationTokenSource _cancellationTokenSource;

    public LocationBackgroundService()
    {
        _locationBackgroundWorker = App.Container.GetInstance<ILocationBackgroundWorker>();
    }
    
     public override IBinder OnBind(Intent intent)
    {
        //  This service is not bindable by an external app so
        //  return null
        return null;
    }

    /// <summary>
    /// On Start Service Command
    /// </summary>
    /// <returns>Start Command Result</returns>
    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags,
        int startId)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        _locationBackgroundWorker.WorkerStopped += OnWorkerStopped;
        //  Build the notification for the foreground service
        var notification = BuildNotification();
        StartForeground(SERVICE_ID, notification);

        _ = Task.Run(async () =>
        {
            _workerActive = true;
            while (_workerActive)
            {
                await Task.Delay(_locationBackgroundWorker.Interval, _cancellationTokenSource.Token);
                var location = await Geolocation.GetLocationAsync() ?? await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    ((LocationBackgroundWorker) _locationBackgroundWorker).OnLocationUpdated(location);
                }
            }
        });

        //  Return a sticky result so that the service remains running
        return StartCommandResult.Sticky;
    }

    /// <summary>
    /// Worker Stopped
    /// </summary>
    private void OnWorkerStopped(object sender, EventArgs e)
    {
        _workerActive = false;
        _cancellationTokenSource.Cancel();
        _locationBackgroundWorker.WorkerStopped -= OnWorkerStopped;
        StopForeground(removeNotification: true);
        StopSelf();
    }

    /// <summary>
    /// Build Service Notification Tile
    /// </summary>
    /// <returns>Service Notification Tile</returns>
    private Notification BuildNotification()
    {
        // Building intent
        var intent = new Intent(Application.Context, typeof(MainActivity));
        intent.AddFlags(ActivityFlags.NoUserAction);
        intent.PutExtra("Title", "Message");

        var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

        var notificationBuilder = new NotificationCompat.Builder(Application.Context, SERVICE_NOTIFICATION_CHANNEL_ID)
            .SetContentTitle("Name of BG Location Work")
            .SetContentText("User Friendly Description")
            .SetSmallIcon(Resource.Mipmap.icon)
            .SetColor(ResourcesCompat.GetColor(Application.Context.Resources, Resource.Color.colorAccent, null))
            .SetOngoing(true)
            .SetContentIntent(pendingIntent);

        // Building channel if API version is 26 or above
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            NotificationChannel notificationChannel = new NotificationChannel(SERVICE_NOTIFICATION_CHANNEL_ID,
                "AppName", NotificationImportance.Low);
            notificationChannel.Importance = NotificationImportance.Low;
            notificationChannel.EnableLights(true);
            notificationChannel.EnableVibration(false);
            notificationChannel.SetShowBadge(true);

            if (Application.Context.GetSystemService(NotificationService) is NotificationManager notificationManager)
            {
                notificationBuilder.SetChannelId(SERVICE_NOTIFICATION_CHANNEL_ID);
                notificationManager.CreateNotificationChannel(notificationChannel);
            }
        }

        return notificationBuilder.Build();
    }
}