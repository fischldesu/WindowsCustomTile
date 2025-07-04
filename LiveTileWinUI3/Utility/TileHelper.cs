using LiveTileWinUI3.Utility.Log;
using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace LiveTileWinUI3.Utility
{
    public enum TileSize
    {
        Small,
        Medium,
        Wide,
        Large,
    }
    public class TileHelper
    {
        private readonly TileUpdater tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
        private readonly static TileHelper instance = new();
        private static Action? cancelToken;

        public static void SetXml(XmlDocument xmlDoc)
        {
            if (xmlDoc != null)
            {
                TileNotification tileNotification = new(xmlDoc);
                instance.tileUpdater.Update(tileNotification);
                Logger.Log("TileUpdater.Update");
            }
        }

        public static void SetHTTPServer(Uri uri, PeriodicUpdateRecurrence recurrence = PeriodicUpdateRecurrence.HalfHour)
        {
            instance.tileUpdater.StartPeriodicUpdate(uri, recurrence);
            Logger.Log("TileUpdater.StartPeriodicUpdate");
        }

        public static void SetHTTPServer(Uri[] uris, PeriodicUpdateRecurrence recurrence = PeriodicUpdateRecurrence.HalfHour)
        {
            instance.tileUpdater.StartPeriodicUpdateBatch(uris, recurrence);
            Logger.Log("TileUpdater.StartPeriodicUpdate");
        }

        public static void SetInterval(XmlDocument[] xmlDocuments, TimeSpan timeSpan)
        {
            if (xmlDocuments.Length > 0)
                SetXml(xmlDocuments[0]);
            
            var size = 1;
            cancelToken?.Invoke();
            cancelToken = Timer.SetInterval(() =>
            {
                if (size >= xmlDocuments.Length)
                    size = 0;

                SetXml(xmlDocuments[size]);
                size++;
            }, timeSpan);
        }

    }
}
