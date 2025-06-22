using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

namespace LiveTileWinUI3.Utility
{
    public class FilePicker
    {
        public async static Task<string?> ReadText(string[] exts, PickerLocationId pickerLocation = PickerLocationId.DocumentsLibrary)
        {
            var picker = new FileOpenPicker();

            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(App.mainWindow));
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = pickerLocation;

            foreach (var ext in exts)
                picker.FileTypeFilter.Add(ext);
            
            var file = await picker.PickSingleFileAsync();
            if (file != null && file.IsAvailable)
                return await FileIO.ReadTextAsync(file);

            return null;
        }
    }
}
