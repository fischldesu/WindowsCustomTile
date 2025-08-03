using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Fischldesu.WCTCore.Tasks.Background;

public sealed class BackgroundTaskEntry : IBackgroundTask
{

    public async void Run(IBackgroundTaskInstance backgroundTask)
    {
        backgroundTask.Canceled += BackgroundTask_Canceled;

        var deferral = backgroundTask.GetDeferral();
        try
        {
            Log.Info($"backgroundtask Running: {backgroundTask.Task.Name}");
            await BackgroundTileUpdater.Update(this);
        }
        catch (Exception e)
        {
            Log.Fatal($"{e.GetType()}: {e.Message}\r\n {e.StackTrace}");
        }
        deferral.Complete();
    }

    private void BackgroundTask_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
    {

    }
}
