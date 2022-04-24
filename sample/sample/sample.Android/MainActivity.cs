using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

namespace sample.Droid;

[Activity(Label = "sample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
        
        RegisterIoC();

        LoadApplication(new App());
    }

    public static void RegisterIoC()
    {
        App.Container.RegisterSingleton<MainPageViewModel>();
        App.Container.RegisterSingleton<IPermissionHandler, PermissionHandler>();
        App.Container.RegisterSingleton<ILocationBackgroundWorker, LocationBackgroundWorker>();
        App.Container.RegisterSingleton<IBackgroundWorker, BackgroundWorker>();
        App.Container.RegisterSingleton<IRegionMonitor, RegionMonitor>();
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
    {
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}