using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace Fischldesu.WCTCore.Tile;

public enum TileSize
{
    Small,
    Medium,
    Wide,
    Large,
}

internal class TileHelper
{
    public sealed class TileXmlDocumentInfo(XmlDocument xmlDocument, int index)
    {
        public readonly XmlDocument XmlDocument = xmlDocument;
        public readonly int XmlFileIndex = index;
    }

    private const string AutoUpdateTileXmlFolderName = "AutoUpdateTileXml";

    public static void SetTileXml(XmlDocument xmlDocument)
    {
        var updater = TileUpdateManager.CreateTileUpdaterForApplication();
        updater.Update(new TileNotification(xmlDocument));
    }

    public async static Task SetAutoUpdateTileXmls(ReadOnlyCollection<XmlDocument> xmlDocuments)
    {
        var tileXmlFolder = await Storage.Folder.CreateFolderAsync(AutoUpdateTileXmlFolderName, CreationCollisionOption.ReplaceExisting);
        for (int i = 0; i < xmlDocuments.Count; i++)
        {
            try
            {
                var xmlString = xmlDocuments[i].GetXml();
                var xmlFile = await tileXmlFolder.CreateFileAsync($"TileXml-{i:D2}.xml", CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(xmlFile, xmlString);
            }
            catch (Exception e)
            {
                Log.Error($"SetAutoUpdateTileXmls: Write Xml file failed. {e.Message}");
            }
        }
    }
    
    public static void SetPeriodicUpdateHTTPServer(Uri uri, PeriodicUpdateRecurrence recurrence)
    {
        var updater = TileUpdateManager.CreateTileUpdaterForApplication();
        updater.StartPeriodicUpdate(uri, recurrence);
    }

    public static void SetPeriodicUpdateHTTPServer(ReadOnlyCollection<Uri> uris, PeriodicUpdateRecurrence recurrence)
    {
        var updater = TileUpdateManager.CreateTileUpdaterForApplication();
        updater.StartPeriodicUpdateBatch(uris.AsEnumerable(), recurrence);
    }

    public async static Task<ReadOnlyCollection<XmlDocument>?> GetAutoUpdateAllTileXmls()
    {
        StorageFolder? autoUpdateTileXmlFolder = null;
        try
        {
            autoUpdateTileXmlFolder = await Storage.Folder.GetFolderAsync(AutoUpdateTileXmlFolderName);
        }
        catch { }
        if (autoUpdateTileXmlFolder == null)
        {
            Log.Error("GetAutoUpdateTileXml:Storage.LocalData.GetFolderAsync No such a directory");
            return null;
        }

        var xmlFiles = await autoUpdateTileXmlFolder.GetFilesAsync();
        var xmlDocuments = new List<XmlDocument>();
        foreach (var file in xmlFiles)
        {
            try
            {
                var xmlString = await FileIO.ReadTextAsync(file);
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);
                xmlDocuments.Add(xmlDoc);
            }
            catch (Exception e)
            {
                Log.Error($"GetAutoUpdateAllTileXmls: Read Xml file failed. {e.Message}");
            }
        }
        return xmlDocuments.AsReadOnly();
    }

    public async static Task<TileXmlDocumentInfo?> GetAutoUpdateTileXml(int index)
    {
        StorageFolder? autoUpdateTileXmlFolder = null;
        try
        {
            autoUpdateTileXmlFolder = await Storage.Folder.GetFolderAsync(AutoUpdateTileXmlFolderName);
        }
        catch { }
        if (autoUpdateTileXmlFolder == null)
        {
            Log.Error("GetAutoUpdateTileXml:Storage.LocalData.GetFolderAsync No such a directory");
            return null;
        }

        var xmlFiles = await autoUpdateTileXmlFolder.GetFilesAsync();
        if (index < 0 || index >= xmlFiles.Count) index = 0;
        var file = xmlFiles[index];

        if (file == null)
        {
            Log.Error($"GetAutoUpdateTileXml: Cannot find file {index}");
            return null;
        }
        
        try
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(await FileIO.ReadTextAsync(file));
            Log.Info($"GetAutoUpdateTileXml: read file [{file.Name}] of index {index}");
            return new(xmlDocument, index);
        }
        catch (Exception e)
        {
            Log.Error($"GetAutoUpdateTileXml: read file [{file.Name}] of index {index} failed\r\n Exception {e.Message}");
        }
        return null;
    }

    public static void Reset()
    {
        TileUpdateManager.CreateTileUpdaterForApplication().Clear();
    }

}
