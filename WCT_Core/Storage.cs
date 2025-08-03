using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fischldesu.WCTCore;

internal class Storage
{
    public static Windows.Storage.StorageFolder Folder => Windows.Storage.ApplicationData.Current.LocalFolder;
    public static Windows.Storage.ApplicationDataContainer Settings => Windows.Storage.ApplicationData.Current.LocalSettings;

}
