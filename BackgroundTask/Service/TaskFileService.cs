using BackgroundTask.DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BackgroundTask.Service
{
    internal static class TaskFileService
    {
        private static readonly string measurementFoldername = "Measurement";
        private static readonly string evaluationFoldername = "Evaluation";

        private static readonly string accelerometerFoldername = "Accelerometer";
        private static readonly string gyrometerFoldername = "Gyrometer";




        public static async void AppendPassivAccelerometerReadingsToFileAsync(AccelerometerData accelerometerData)
        {
            Debug.WriteLine("############## Save Passiv Readings ##################");
            StorageFolder accelerometerFolder = await FindMeasurementStorageFolder(MeasurementType.Accelerometer);

            StringBuilder stringbuilder = new StringBuilder();
            foreach (AccelerometerReading accelerometerReading in accelerometerData.GetPassivReadingsList())
            {
                stringbuilder.Append(String.Format(new CultureInfo("en-US"), "{0:f4},{1:f4},{2:f4},{3}\n",
                    accelerometerReading.AccelerationX, accelerometerReading.AccelerationY, accelerometerReading.AccelerationZ, accelerometerReading.Timestamp.UtcTicks));
            }

            await SaveStringToEndOfFileAsync(stringbuilder.ToString(), accelerometerFolder, accelerometerData.AccelerometerFilename);
        }

        public static async void AppendActivAccelerometerReadingsToFileAsync(AccelerometerData accelerometerData)
        {
            Debug.WriteLine("############## Save Activ Readings ##################");
            StorageFolder accelerometerFolder = await FindMeasurementStorageFolder(MeasurementType.Accelerometer);

            StringBuilder stringbuilder = new StringBuilder();
            foreach (AccelerometerReading accelerometerReading in accelerometerData.GetActivReadingsList())
            {
                stringbuilder.Append(String.Format(new CultureInfo("en-US"), "{0:f4},{1:f4},{2:f4},{3}\n",
                    accelerometerReading.AccelerationX, accelerometerReading.AccelerationY, accelerometerReading.AccelerationZ, accelerometerReading.Timestamp.UtcTicks));
            }

            await SaveStringToEndOfFileAsync(stringbuilder.ToString(), accelerometerFolder, accelerometerData.AccelerometerFilename);
        }



        private static async Task<StorageFolder> FindMeasurementStorageFolder(MeasurementType measurementType)
        {
            String resultFolderName = "unknown";
            StorageFolder resultFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                if (localFolder != null)
                {
                    // first level folder
                    StorageFolder measurementFolder = await localFolder.CreateFolderAsync(measurementFoldername, CreationCollisionOption.OpenIfExists);

                    if (measurementFolder != null)
                    {
                        if (measurementType == MeasurementType.Accelerometer)
                        {
                            resultFolderName = accelerometerFoldername;
                        }
                        else if (measurementType == MeasurementType.Gyrometer)
                        {
                            resultFolderName = gyrometerFoldername;
                        }
                        // second level folder
                        resultFolder = await measurementFolder.CreateFolderAsync(resultFolderName, CreationCollisionOption.OpenIfExists);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[BackgroundTask.Service.FileService.FindMeasurementStorageFolder] Ordner: {0} konnte nicht gefunden werden.", resultFolderName);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[BackgroundTask.Service.FileService.FindMeasurementStorageFolder] Ordner: {0} konnte nicht zugegriffen werden.", resultFolderName);
            }
            return resultFolder;
        }

        private static async Task<StorageFolder> FindEvaluationStorageFolder(MeasurementType measurementType)
        {
            String resultFolderName = "unknown";
            StorageFolder resultFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                if (localFolder != null)
                {
                    // first level folder
                    StorageFolder measurementFolder = await localFolder.CreateFolderAsync(evaluationFoldername, CreationCollisionOption.OpenIfExists);

                    if (measurementFolder != null)
                    {
                        if (measurementType == MeasurementType.Accelerometer)
                        {
                            resultFolderName = accelerometerFoldername;
                        }
                        else if (measurementType == MeasurementType.Gyrometer)
                        {
                            resultFolderName = gyrometerFoldername;
                        }
                        // second level folder
                        resultFolder = await measurementFolder.CreateFolderAsync(resultFolderName, CreationCollisionOption.OpenIfExists);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[BackgroundTask.Service.FileService.FindMeasurementStorageFolder] Ordner: {0} konnte nicht gefunden werden.", resultFolderName);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[BackgroundTask.Service.FileService.FindMeasurementStorageFolder] Ordner: {0} konnte nicht zugegriffen werden.", resultFolderName);
            }
            return resultFolder;
        }


        private static async Task SaveStringToEndOfFileAsync(String appendString, StorageFolder targetFolder, string filename)
        {
            try
            {
                StorageFile file = await targetFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                using (IRandomAccessStream textStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (DataWriter textWriter = new DataWriter(textStream.GetOutputStreamAt(textStream.Size)))
                    {
                        textWriter.WriteString(appendString);
                        await textWriter.StoreAsync();
                    }
                    Debug.WriteLine("############## Current file size: '{0}' ################", textStream.Size);
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[BackgroundTask.Service.FileService.SaveStringToFile] Datei: {0} konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[BackgroundTask.Service.FileService.SaveStringToFile] Datei: {0} konnte nicht zugegriffen werden.", filename);
            }
            return;
        }
    }

    internal enum MeasurementType
    {
        Accelerometer,
        Gyrometer
    }
}
