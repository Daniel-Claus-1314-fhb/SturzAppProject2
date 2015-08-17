﻿using BackgroundTask.DataModel;
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
    internal static class FileService
    {
        private static readonly string _measurementsMetaDataFilename = "Measurements.json";

        private static readonly string _measurementsMetaDataPath = @"";
        private static readonly string _measurementAccelerometerPath = @"Measurement\Accelerometer";
        private static readonly string _measurementGyrometerPath = @"Measurement\Gyrometer";
        private static readonly string _evaluationAccelerometerPath = @"Evaluation\Accelerometer";
        private static readonly string _evaluationGyrometerPath = @"Evaluation\Gyrometer";

        #region Save/Load MeasurementList

        //##################################################################################################################################
        //################################################## Save Measurements #############################################################
        //##################################################################################################################################

        internal static async void SaveMeasurementListAsync(List<Measurement> measurements)
        {
            if (measurements != null)
            {
                string jsonString = JsonConvert.SerializeObject(measurements);
                StorageFolder targetFolder = await FindStorageFolder(_measurementsMetaDataPath);
                await SaveJsonStringToFile(jsonString, targetFolder, _measurementsMetaDataFilename);
            }
        }

        //##################################################################################################################################
        //################################################## Load Measurements #############################################################
        //##################################################################################################################################

        internal static async Task<List<Measurement>> LoadMeasurementListAsync()
        {
            bool isFileCorrupted = false;
            List<Measurement> measurements = new List<Measurement>();
            StorageFolder measurementFolder = await FindStorageFolder(_measurementsMetaDataPath);
            string jsonString = await LoadJsonStringFromFile(measurementFolder, _measurementsMetaDataFilename);

            if (jsonString != null && jsonString.Length > 0)
            {
                try
                {
                    measurements = JsonConvert.DeserializeObject<List<Measurement>>(jsonString);
                }
                catch (JsonReaderException)
                {
                    Debug.WriteLine("[BackgroundTask.Service.FileService] Sersialized list of measurements could not read.");
                    // Delete corrupted file.
                    isFileCorrupted = true;
                }

                if (isFileCorrupted)
                {
                    await DeleteFileAsync(measurementFolder, _measurementsMetaDataFilename);
                }
            }
            return measurements;
        }

        #endregion

        #region Load OxyplotData
        
        //##################################################################################################################################
        //################################################## Load Oxyplot data #############################################################
        //##################################################################################################################################

        internal static async Task<OxyplotData> LoadOxyplotDataAsync(Measurement measurement)
        {
            OxyplotData oxyplotData = new OxyplotData();
            if (measurement != null)
            {
                StorageFolder accelerometerFolder = await FindStorageFolder(_measurementAccelerometerPath);
                StorageFolder gyrometerFolder = await FindStorageFolder(_measurementGyrometerPath);

                Task<List<Tuple<TimeSpan, double, double, double>>> loadAccelerometerTask = LoadAccelerometerReadingsFromFile(accelerometerFolder, measurement.AccelerometerFilename);
                Task<List<Tuple<TimeSpan, double, double, double>>> loadGyrometerTask = LoadGyrometerReadingsFromFile(gyrometerFolder, measurement.GyrometerFilename);

                oxyplotData.AccelerometerReadings = await loadAccelerometerTask;
                oxyplotData.GyrometerReadings = await loadGyrometerTask;
            }
            return oxyplotData;
        }

        #endregion

        //##################################################################################################################################
        //################################################## find folder ###################################################################
        //##################################################################################################################################

        private static async Task<StorageFolder> FindStorageFolder(string folderPath)
        {
            StorageFolder resultFolder = ApplicationData.Current.LocalFolder;
            if (folderPath != null && folderPath.Length > 0)
            {
                try
                {
                    resultFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderPath, CreationCollisionOption.OpenIfExists);
                }
                catch (FileNotFoundException)
                {
                    Debug.WriteLine("[BackgroundTask.Service.FileService.FindMeasurementStorageFolder] Ordner: '{0}' konnte nicht gefunden werden.", folderPath);
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("[BackgroundTask.Service.FileService.FindMeasurementStorageFolder] Ordner: '{0}' konnte nicht zugegriffen werden.", folderPath);
                }
            }
            return resultFolder;
        }

        #region Load accelerometerReadings

        //##################################################################################################################################
        //################################################## load accerlerometerReadings ###################################################
        //##################################################################################################################################

        private static async Task<List<Tuple<TimeSpan, double, double, double>>> LoadAccelerometerReadingsFromFile(StorageFolder targetFolder, string filename)
        {
            List<Tuple<TimeSpan, double, double, double>> accelerometerReadingTuples = new List<Tuple<TimeSpan, double, double, double>>();
            try
            {
                StorageFile file = await targetFolder.GetFileAsync(filename);
                using (StreamReader stream = new StreamReader(await file.OpenStreamForReadAsync()))
                {
                    while (!stream.EndOfStream)
                    {
                        string currentReadLineOfFile = await stream.ReadLineAsync();
                        string[] stringArray = currentReadLineOfFile.Split(new Char[] { ',' });

                        long timeStampTicks;
                        double accerlerometerX;
                        double accerlerometerY; 
                        double accerlerometerZ;

                        NumberStyles styles = NumberStyles.Any;
                        IFormatProvider provider = new CultureInfo("en-US");

                        if (Double.TryParse(stringArray[0], styles, provider, out accerlerometerX) &&
                            Double.TryParse(stringArray[1], styles, provider, out accerlerometerY) &&
                            Double.TryParse(stringArray[2], styles, provider, out accerlerometerZ) &&
                            long.TryParse(stringArray[3], out timeStampTicks))
                        {
                            accelerometerReadingTuples.Add(new Tuple<TimeSpan, double, double, double>(TimeSpan.FromTicks(timeStampTicks), accerlerometerX, accerlerometerY, accerlerometerZ));
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[MensaApp.FileService.LoadAccelerometerReadingsFromFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[MensaApp.FileService.LoadAccelerometerReadingsFromFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return accelerometerReadingTuples;
        }

        private static async Task<List<Tuple<TimeSpan, double, double, double>>> LoadGyrometerReadingsFromFile(StorageFolder targetFolder, string filename)
        {
            List<Tuple<TimeSpan, double, double, double>> gyrometerReadingTuples = new List<Tuple<TimeSpan, double, double, double>>();
            try
            {
                StorageFile file = await targetFolder.GetFileAsync(filename);
                using (StreamReader stream = new StreamReader(await file.OpenStreamForReadAsync()))
                {
                    while (!stream.EndOfStream)
                    {
                        string currentReadLineOfFile = await stream.ReadLineAsync();
                        string[] stringArray = currentReadLineOfFile.Split(new Char[] { ',' });

                        long timeStampTicks;
                        double gyrometerX;
                        double gyrometerY;
                        double gyrometerZ;

                        NumberStyles styles = NumberStyles.Any;
                        IFormatProvider provider = new CultureInfo("en-US");

                        if (Double.TryParse(stringArray[0], styles, provider, out gyrometerX) &&
                            Double.TryParse(stringArray[1], styles, provider, out gyrometerY) &&
                            Double.TryParse(stringArray[2], styles, provider, out gyrometerZ) &&
                            long.TryParse(stringArray[3], out timeStampTicks))
                        {
                            gyrometerReadingTuples.Add(new Tuple<TimeSpan, double, double, double>(TimeSpan.FromTicks(timeStampTicks), gyrometerX, gyrometerY, gyrometerZ));
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[MensaApp.FileService.LoadAccelerometerReadingsFromFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[MensaApp.FileService.LoadAccelerometerReadingsFromFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return gyrometerReadingTuples;
        }
        
        #endregion

        #region Save/Load JSONString

        //##################################################################################################################################
        //################################################## Save into File ################################################################
        //##################################################################################################################################

        private static async Task SaveJsonStringToFile(string jsonString, StorageFolder targetFolder, string filename)
        {
            try
            {
                if (targetFolder != null)
                {
                    StorageFile file = await targetFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                    using (IRandomAccessStream textStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (DataWriter textWriter = new DataWriter(textStream))
                        {
                            textWriter.WriteString(jsonString);
                            await textWriter.StoreAsync();
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[MensaApp.FileService.SaveJsonStringToFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[MensaApp.FileService.SaveJsonStringToFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return;
        }

        //##################################################################################################################################
        //################################################## Load from File ################################################################
        //##################################################################################################################################

        private static async Task<string> LoadJsonStringFromFile(StorageFolder targetFolder, string filename)
        {
            string jsonString = null;
            if (targetFolder != null)
            {
                try
                {
                    IReadOnlyList<StorageFile> files = await targetFolder.GetFilesAsync();
                    if (files != null)
                    {
                        using (IEnumerator<StorageFile> filesIterator = files.GetEnumerator())
                        {
                            while (filesIterator.MoveNext())
                            {
                                StorageFile file = filesIterator.Current;
                                if (file.Name.Equals(filename))
                                {
                                    jsonString = await FileIO.ReadTextAsync(file);
                                }
                            }
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    Debug.WriteLine("[MensaApp.FileService.LoadJsonStringFromFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("[MensaApp.FileService.LoadJsonStringFromFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
                }
            }
            return jsonString;
        }

        #endregion

        //##################################################################################################################################
        //################################################## delete File ###################################################################
        //##################################################################################################################################


        public static async void DeleteAccelerometerMeasurementAsync(string filename)
        {
            if (filename != null && filename.Length > 0)
            {
                StorageFolder accelerometerFolder = await FindStorageFolder(_measurementAccelerometerPath);
                await DeleteFileAsync(accelerometerFolder, filename);
            }
        }

        public static async void DeleteGyrometerMeasurementAsync(string filename)
        {
            if (filename != null && filename.Length > 0)
            {
                StorageFolder gyrometerFolder = await FindStorageFolder(_measurementGyrometerPath);
                await DeleteFileAsync(gyrometerFolder, filename);
            }
        }
        
        private static async Task DeleteFileAsync(StorageFolder targetFolder, string filename)
        {
            try
            {
                if (targetFolder != null)
                {
                    StorageFile file = await targetFolder.GetFileAsync(filename);
                    if (file != null)
                    {
                        await file.DeleteAsync();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[MensaApp.FileService.DeleteFileAsync] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[MensaApp.FileService.DeleteFileAsync] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return;
        }
    }
}
