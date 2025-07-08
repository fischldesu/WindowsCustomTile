﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using LiveTileWinUI3.Utility.Log;

namespace LiveTileWinUI3.Utility
{
    public class FilePicker
    {
        private static FileOpenPicker InitFilePicker()
        {
            
            // When App run as Administrator and PickSingleFileAsync causes Exception
            var picker = new FileOpenPicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(App.mainWindow));

            return picker;
        }

        public async static Task<string?> ReadText(string[] exts, PickerLocationId pickerLocation = PickerLocationId.DocumentsLibrary)
        {
            var picker = InitFilePicker();

            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = pickerLocation;

            try
            {
                foreach (var ext in exts)
                    picker.FileTypeFilter.Add(ext);

                var file = await picker.PickSingleFileAsync();
                if (file != null && file.IsAvailable)
                    return await FileIO.ReadTextAsync(file);
            }
            catch (Exception)
            {
                Logger.Log("Excetion when picking a file", LogMessage.LogLevel.ERROR);
            }
            return null;
        }

        public async static Task<string?> PickSingleFile(string[] exts, bool thumbThumbnailViewMode, PickerLocationId startLocation = PickerLocationId.DocumentsLibrary)
        {
            var picker = InitFilePicker();

            if (thumbThumbnailViewMode) picker.ViewMode = PickerViewMode.Thumbnail;
            else picker.ViewMode = PickerViewMode.List;
            
            picker.SuggestedStartLocation = startLocation;

            try
            {
                foreach (var ext in exts)
                    picker.FileTypeFilter.Add(ext);

                var f = await picker.PickSingleFileAsync();
                return f?.Path;
            }
            catch (Exception)
            {
                Logger.Log("Excetion when picking a file", LogMessage.LogLevel.ERROR);
            }
            return null;
        }
    }
}
