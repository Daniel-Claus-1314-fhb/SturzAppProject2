using BackgroundTask.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.ApplicationModel.Activation;

namespace BackgroundTask.Service
{
    internal static class ExportService
    {
        public static async void ExportMeasurementData(Measurement measurement)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            

            StorageFolder resultFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Evaluation");
            StorageFolder resultFolder2 = await resultFolder.GetFolderAsync("Accelerometer");
            StorageFile resultFile = await resultFolder2.GetFileAsync(measurement.Filename);

            //await resultFile.CopyAsync(Windows.Storage.KnownFolders.DocumentsLibrary, "e" + measurement.AccelerometerFilename);

            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("CSV-Datei", new List<string>() { ".csv" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "e" + measurement.Filename;

            savePicker.PickSaveFileAndContinue();
        }

        

    }
}
