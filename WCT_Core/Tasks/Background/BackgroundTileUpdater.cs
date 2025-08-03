using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace Fischldesu.WCTCore.Tasks.Background;
internal class BackgroundTileUpdater
{

    public async static Task<BackgroundTaskRegistration?> Register(string name, IBackgroundTrigger trigger, bool updateImmediately = false)
    {
        Unregister(name);

        switch (await BackgroundExecutionManager.RequestAccessAsync())
        {
            case BackgroundAccessStatus.Denied:
            case BackgroundAccessStatus.DeniedBySystemPolicy:
            case BackgroundAccessStatus.DeniedByUser:
                return null;
        }

        var backgroundTaskBuilder = new BackgroundTaskBuilder()
        {
            Name = name,
            TaskEntryPoint = "Fischldesu.WCTCore.Tasks.Background.BackgroundTaskEntry",
        };

        backgroundTaskBuilder.SetTrigger(trigger);
        var registration = backgroundTaskBuilder.Register();

        if (updateImmediately) await Update();

        return registration;
    }

    public static void Unregister(string name)
    {
        foreach (var item in BackgroundTaskRegistration.AllTasks)
            if (item.Value.Name == name)
                item.Value.Unregister(true);
        
    }

    public static async Task Update(BackgroundTaskEntry? backgroundUpdater = null)
    {

        var settings = new Settings();
        var nextIndex = settings.AutoUpdateNextTileIndex;
        var result = await Tile.TileHelper.GetAutoUpdateTileXml(nextIndex);

        if (result == null)
        {
            Log.Error("No Tile Xml file found for Update.");
            return;
        }

        Tile.TileHelper.SetTileXml(result.XmlDocument);

        if (result.XmlFileIndex != nextIndex) settings.AutoUpdateNextTileIndex = 0;
        settings.AutoUpdateNextTileIndex++;

    }

}
