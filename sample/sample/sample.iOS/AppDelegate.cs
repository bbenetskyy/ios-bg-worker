using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundTasks;
using CoreFoundation;
using Foundation;
using UIKit;

namespace sample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private const string REFRESH_IDENTIFIER = "com.companyname.sample.refresh";
        
        private IBackgroundWorker _synchronisationWorker;
        /// <summary>
        /// Background Worker to Run Background Updates during Perform Fetch
        /// </summary>
        private IBackgroundWorker BackgroundWorker =>
            _synchronisationWorker ??= App.Container.GetInstance<IBackgroundWorker>();
        
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            
            App.Container.Register<MainPageViewModel>();
            App.Container.RegisterSingleton<IPermissionHandler, PermissionHandler>();
            App.Container.RegisterSingleton<ILocationBackgroundWorker, LocationBackgroundWorker>();
            App.Container.RegisterSingleton<IBackgroundWorker, BackgroundWorker>();
            
            LoadApplication(new App());
            
            BGTaskScheduler.Shared.Register(REFRESH_IDENTIFIER, DispatchQueue.CurrentQueue, task =>
            {
                var queue = NSOperationQueue.CurrentQueue;
                task.ExpirationHandler = () =>
                {
                    queue.CancelAllOperations();
                };
                queue.AddOperation(() => _ = StartSynchronisationWork());
            });
            
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            return base.FinishedLaunching(app, options);
        }
        
        /// <summary>
        /// Perform App Background Fetch
        /// </summary>
        public override async void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            var dataAvailableToSync = await StartSynchronisationWork();
            completionHandler?.Invoke(dataAvailableToSync
                ? UIBackgroundFetchResult.NewData
                : UIBackgroundFetchResult.NoData);
        }

        /// <summary>
        /// App Just Enter Background
        /// </summary>
        public override void DidEnterBackground(UIApplication uiApplication)
        {
            base.DidEnterBackground(uiApplication);
           
            var request = new BGAppRefreshTaskRequest(REFRESH_IDENTIFIER);
            request.EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(sample.iOS.BackgroundWorker.TIMER_COOLDOWN);
            
            try
            {
                BGTaskScheduler.Shared.Submit(request, out var error);
                //if you try this on simulator you will get net error:
                //Error Domain=BGTaskSchedulerErrorDomain Code=1 "(null)"
                //but in real device with proper certificates you should get no error!!!
                if (error is not null)
                {
                    //todo Logger.LogError(new Exception(error.Description));
                }
            }
            catch (Exception ex)
            {
                //todo Logger.LogError(ex);
            }
        }
        

        /// <summary>
        /// Check if we need to start BG Work and start if any data is available
        /// </summary>
        private async Task<bool> StartSynchronisationWork()
        {
            var dataAvailableToSync  = await BackgroundWorker.IsDataAvailableToSync();
            if (dataAvailableToSync)
            {
                _ = BackgroundWorker.BackgroundWork?.Invoke();
            }

            return dataAvailableToSync;
        }
    }
}

