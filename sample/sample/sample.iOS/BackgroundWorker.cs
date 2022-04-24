using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace sample.iOS;



public class BackgroundWorker : IBackgroundWorker
{
    /// <summary>
    /// Timer Cooldown/Delay
    /// </summary>
    public const double TIMER_COOLDOWN = 5; //5 seconds
                                            //best time according to iOS is from 5 to 15 minutes,
                                            //but with 30 minutes it may also work
    
    public event EventHandler WorkerStopped;
    
    private NSTimer _timer;

    public Func<Task> BackgroundWork { get; private set; }
    
    
    public void StartWorker(Func<Task> backgroundWork)
    {
        BackgroundWork = backgroundWork;

        if (_timer is not null)
        {
            StopWorker();
        }

        _timer = NSTimer.CreateRepeatingScheduledTimer(TIMER_COOLDOWN, async _ =>
        {
            nint taskId = 0;
            taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
            {
                // Time execution limit reached. Stopping the background task
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
            });
 
            await BackgroundWork();
 
            UIApplication.SharedApplication.EndBackgroundTask(taskId);
        });
    }
    
    public void StopWorker()
    {
        if (_timer is not null)
        {
            OnWorkerStopped();
            _timer.Invalidate();
            _timer.Dispose();
            _timer = null;
        }
    }

    public Task<bool> IsDataAvailableToSync()
    {
        //todo check if we need to run our BG Work
        //we need to run if any data to sync or download 
        //is available
        return Task.FromResult(true);//true - data available, false - run BG Work is not required
    }

    protected virtual void OnWorkerStopped()
    {
        WorkerStopped?.Invoke(this, EventArgs.Empty);
    }
}