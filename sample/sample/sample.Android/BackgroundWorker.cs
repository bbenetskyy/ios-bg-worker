using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;

namespace sample.Droid;

public class BackgroundWorker : IBackgroundWorker
{
    public event EventHandler WorkerStopped;
    public Func<Task> BackgroundWork { get; private set; }
    public void StartWorker(Func<Task> backgroundWork)
    {
        BackgroundWork = backgroundWork;
        var intent = new Intent(Application.Context, typeof(BackgroundService));

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
        WorkerStopped?.Invoke(this, EventArgs.Empty);
    }

    public Task<bool> IsDataAvailableToSync()
    {
        //todo check if we need to run our BG Work
        //we need to run if any data to sync or download 
        //is available
        return Task.FromResult(true);//true - data available, false - run BG Work is not required
    }
}