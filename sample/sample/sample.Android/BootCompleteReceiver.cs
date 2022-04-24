using System;
using Android.App;
using Android.Content;

namespace sample.Droid;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { Intent.ActionBootCompleted })]
public class BootCompleteReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        //restart services you want after phone reboot
        
        //todo I'm not sure how each IoC works
        //so I add once again registration of IoC Services
        //you need to check how your IoC Works and where is
        //a proper place to call method registrations
        MainActivity.RegisterIoC();
        
        //todo uncomment that to test restart receiver
        //App.Container.GetInstance<ILocationBackgroundWorker>().StartLocationUpdates(TimeSpan.FromSeconds(5));
    }
}