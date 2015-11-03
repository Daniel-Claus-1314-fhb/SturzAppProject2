using BackgroundTask.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.ApplicationModel.Activation;
using Windows.Storage.Streams;
using SensorDataEvaluation.DataModel;
using BackgroundTask.DataModel.Export;

namespace BackgroundTask.Service
{
    internal static class ExportService
    {
        public static async void ExportMeasurementData(MeasurementModel measurement)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            
            StorageFolder resultFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Evaluation");
            StorageFolder resultFolder2 = await resultFolder.GetFolderAsync("Accelerometer");
            StorageFile resultFile = await resultFolder2.GetFileAsync(measurement.Filename);

            //await resultFile.CopyAsync(Windows.Storage.KnownFolders.DocumentsLibrary, "e" + measurement.AccelerometerFilename);

            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Binary-Datei", new List<string>() { ".bin" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "e" + measurement.Filename;
            savePicker.PickSaveFileAndContinue();
        }

        //##################################################################################################################################
        //################################################## Export Samples ################################################################
        //##################################################################################################################################

        internal static async Task<bool> ExportSamplesAsync(StorageFile exportTargetFile, string filenameOfSourceFile, ExportSettingModel exportSetting)
        {
            bool isSuccessfulExport = false;

            using (IRandomAccessStream textStream = await exportTargetFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (DataWriter textWriter = new DataWriter(textStream.GetOutputStreamAt(0)))
                {
                    if (exportSetting.IsAccelerometer)
                    {
                        await ExportAccelerometerSamples(filenameOfSourceFile, textWriter);
                        isSuccessfulExport = true;
                    }
                    if (exportSetting.IsGyrometer)
                    {
                        await ExportGyrometerSamples(filenameOfSourceFile, textWriter);
                        isSuccessfulExport = true;
                    }
                    if (exportSetting.IsQuaternion)
                    {
                        await ExportQuaterionSamples(filenameOfSourceFile, textWriter);
                        isSuccessfulExport = true;
                    }
                    if (exportSetting.IsGeolocation)
                    {
                        await ExportGeolocationSamples(filenameOfSourceFile, textWriter);
                        isSuccessfulExport = true;
                    }
                    if (exportSetting.IsEvaluation)
                    {
                        await ExportEvaluationSamples(filenameOfSourceFile, textWriter);
                        isSuccessfulExport = true;
                    }
                }
            }
            return isSuccessfulExport;
        }

        //###################################################################################
        //################################### Accelerometer #################################
        //###################################################################################

        private static async Task ExportAccelerometerSamples(string filenameOfSourceFile, DataWriter textWriter)
        {
            List<AccelerometerSample> exportSamples = await FileService.LoadAccelerometerSamplesFromFileAsync(filenameOfSourceFile);

            if (exportSamples != null && exportSamples.Count > 0)
            {
                // insert header for accelerometer data
                int sampleCount = exportSamples.Count;
                textWriter.WriteBytes(AccelerometerSample.GetExportDataDescription(sampleCount));
                textWriter.WriteString(AccelerometerSample.GetExportHeader());
                var enumerator = exportSamples.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    textWriter.WriteBytes(enumerator.Current.ToByteArray());
                }
                await textWriter.StoreAsync();
            }
        }

        //###################################################################################
        //################################### Gyrometer #####################################
        //###################################################################################

        private static async Task ExportGyrometerSamples(string filenameOfSourceFile, DataWriter textWriter)
        {
            List<GyrometerSample> exportSamples = await FileService.LoadGyrometerSamplesFromFileAsync(filenameOfSourceFile);

            // Append gyrometer header and gyrometer samples
            if (exportSamples != null && exportSamples.Count > 0)
            {
                // insert header for gyrometer data
                int sampleCount = exportSamples.Count;
                textWriter.WriteBytes(GyrometerSample.GetExportDataDescription(sampleCount));
                textWriter.WriteString(GyrometerSample.GetExportHeader());
                var enumerator = exportSamples.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    textWriter.WriteBytes(enumerator.Current.ToByteArray());
                }
                await textWriter.StoreAsync();
            }
        }

        //###################################################################################
        //################################### Quaternion ####################################
        //###################################################################################

        private static async Task ExportQuaterionSamples(string filenameOfSourceFile, DataWriter textWriter)
        {
            List<QuaternionSample> exportSamples = await FileService.LoadQuaternionSamplesFromFileAsync(filenameOfSourceFile);

            // Append quaternion header and quaternion samples
            if (exportSamples != null && exportSamples.Count > 0)
            {
                // insert header for quaternion data
                int sampleCount = exportSamples.Count;
                textWriter.WriteBytes(QuaternionSample.GetExportDataDescription(sampleCount));
                textWriter.WriteString(QuaternionSample.GetExportHeader());
                var enumerator = exportSamples.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    textWriter.WriteBytes(enumerator.Current.ToByteArray());
                }
                await textWriter.StoreAsync();
            }
        }

        //###################################################################################
        //################################### Geolocation ###################################
        //###################################################################################

        private static async Task ExportGeolocationSamples(string filenameOfSourceFile, DataWriter textWriter)
        {
            List<GeolocationSample> exportSamples = await FileService.LoadGeolocationSamplesFromFileAsync(filenameOfSourceFile);

            // Append evaluation header and evaluation samples
            if (exportSamples != null && exportSamples.Count > 0)
            {
                // insert header for evaluation data
                int sampleCount = exportSamples.Count;
                textWriter.WriteBytes(GeolocationSample.GetExportDataDescription(sampleCount));
                textWriter.WriteString(GeolocationSample.GetExportHeader());
                var enumerator = exportSamples.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    textWriter.WriteBytes(enumerator.Current.ToByteArray());
                }
                await textWriter.StoreAsync();
            }
        }

        //###################################################################################
        //################################### Evaluation ####################################
        //###################################################################################

        private static async Task ExportEvaluationSamples(string filenameOfSourceFile, DataWriter textWriter)
        {
            List<EvaluationSample> exportSamples = await FileService.LoadEvaluationSamplesFromFileAsync(filenameOfSourceFile);

            // Append evaluation header and evaluation samples
            if (exportSamples != null && exportSamples.Count > 0)
            {
                // insert header for evaluation data
                int sampleCount = exportSamples.Count;
                textWriter.WriteBytes(EvaluationSample.GetExportDataDescription(sampleCount));
                textWriter.WriteString(EvaluationSample.GetExportHeader());
                var enumerator = exportSamples.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    textWriter.WriteBytes(enumerator.Current.ToByteArray());
                }
                await textWriter.StoreAsync();
            }
        }
    }
}
