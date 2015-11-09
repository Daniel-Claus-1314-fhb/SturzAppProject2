using BackgroundTask.DataModel;
using BackgroundTask.DataModel.DataSets;
using BackgroundTask.DataModel.Setting;
using Newtonsoft.Json;
using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Sensors;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BackgroundTask.Service
{
    internal static class FileService
    {
        private static ResourceLoader FolderResource = ResourceLoader.GetForCurrentView("FolderResources");

        private static readonly string _measurementsMetaDataFilename = "Measurements.json";
        private static readonly string _settingMetaDataFilename = "Settings.json";

        private static readonly string _metaDataPath = FolderResource.GetString("MetaDataPath");
        private static readonly string _accelerometerPath = FolderResource.GetString("AccelerometerPath");
        private static readonly string _gyrometerPath = FolderResource.GetString("GyrometerPath");
        private static readonly string _quaternionPath = FolderResource.GetString("QuaternionPath");
        private static readonly string _geolocationPath = FolderResource.GetString("GeolocationPath");
        private static readonly string _evaluationPath = FolderResource.GetString("EvaluationPath");

        internal static string GetAccelerometerPath()
        {
            return _accelerometerPath;
        }
        internal static string GetGyrometerPath()
        {
            return _gyrometerPath;
        }
        internal static string GetQuaterionPath()
        {
            return _quaternionPath;
        }
        internal static string GetGeolocationPath()
        {
            return _geolocationPath;
        }
        internal static string GetEvaluationPath()
        {
            return _evaluationPath;
        }

        //##################################################################################################################################
        //################################################## Save Meta Data ################################################################
        //##################################################################################################################################

        #region Save/Load MeasurementList

        internal static async void SaveGlobalMeasurementListAsync(List<MeasurementModel> measurements)
        {
            if (measurements != null)
            {
                string jsonString = JsonConvert.SerializeObject(measurements);
                StorageFolder targetFolder = await FindStorageFolder(_metaDataPath);
                await SaveJsonStringToFile(jsonString, targetFolder, _measurementsMetaDataFilename);
            }
        }

        internal static async void SaveGlobalSettingModelAysnc(SettingModel settingModel)
        {
            if (settingModel != null)
            {
                string jsonString = JsonConvert.SerializeObject(settingModel);
                StorageFolder targetFolder = await FindStorageFolder(_metaDataPath);
                await SaveJsonStringToFile(jsonString, targetFolder, _settingMetaDataFilename);
            }
        }

        //##################################################################################################################################
        //################################################## Load Meta Data ###############################################################
        //##################################################################################################################################

        internal static async Task<List<MeasurementModel>> LoadGlobalMeasurementListAsync()
        {
            bool isFileCorrupted = false;
            List<MeasurementModel> measurements = new List<MeasurementModel>();
            StorageFolder measurementFolder = await FindStorageFolder(_metaDataPath);
            string jsonString = await LoadJsonStringFromFile(measurementFolder, _measurementsMetaDataFilename);

            if (jsonString != null && jsonString.Length > 0)
            {
                try
                {
                    measurements = JsonConvert.DeserializeObject<List<MeasurementModel>>(jsonString);
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

        internal static async Task<SettingModel> LoadGlobalSettingAsync()
        {
            bool isFileCorrupted = false;
            SettingModel settingModel = null;
            StorageFolder measurementFolder = await FindStorageFolder(_metaDataPath);
            string jsonString = await LoadJsonStringFromFile(measurementFolder, _settingMetaDataFilename);

            if (jsonString != null && jsonString.Length > 0)
            {
                try
                {
                    settingModel = JsonConvert.DeserializeObject<SettingModel>(jsonString);
                }
                catch (JsonReaderException)
                {
                    Debug.WriteLine("[BackgroundTask.Service.FileService] Sersialized list of measurements could not read.");
                    // Delete corrupted file.
                    isFileCorrupted = true;
                }

                if (isFileCorrupted)
                {
                    await DeleteFileAsync(measurementFolder, _settingMetaDataFilename);
                }
            }

            if (settingModel == null)
            {
                settingModel = SettingModel.DefaultSettingModel();
            }
            return settingModel;
        }

        #endregion

        //##################################################################################################################################
        //################################################## find folder ###################################################################
        //##################################################################################################################################

        internal static async Task<StorageFolder> FindStorageFolder(string folderPath)
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

        //##################################################################################################################################
        //################################################## load AccerlerometerSamples ####################################################
        //##################################################################################################################################

        #region Load accelerometerReadings

        internal static async Task<bool> IsAccelerometerSamplesAvailable(string filename)
        {
            bool isAvailable = false;
            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetAccelerometerPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                if (file != null)
                {
                    isAvailable = true;
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.IsAccelerometerSamplesAvailable] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.IsAccelerometerSamplesAvailable] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return isAvailable;
        }

        internal static async Task<int> GetAccelerometerSamplesCount(string filename)
        {
            int totalSampleCount = 0;
            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetAccelerometerPath());
                StorageFile file = await targetFolder.GetFileAsync(filename); 
                using (BinaryReader stream = new BinaryReader(await file.OpenStreamForReadAsync()))
                {
                    int bytesOfSample = AccelerometerSample.AmountOfBytes;
                    long totalBytesCount = stream.BaseStream.Length;

                    double result1 = totalBytesCount / bytesOfSample;
                    double roundResult = Math.Round(result1, 0, MidpointRounding.ToEven);
                    totalSampleCount = Convert.ToInt32(roundResult);                    
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.GetAccelerometerSamplesCount] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.GetAccelerometerSamplesCount] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return totalSampleCount;
        }

        internal static async Task<List<AccelerometerSample>> LoadAccelerometerSamplesFromFileAsync(string filename)
        {
            return await LoadAccelerometerSamplesFromFileAsync(filename, 0, 0);
        }

        internal static async Task<List<AccelerometerSample>> LoadAccelerometerSamplesFromFileAsync(string filename, int startOffset, int sampleCount)
        {
            List<AccelerometerSample> resultSamples = new List<AccelerometerSample>();
            int bytesOfSample = AccelerometerSample.AmountOfBytes;

            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetAccelerometerPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                using (BinaryReader stream = new BinaryReader(await file.OpenStreamForReadAsync()))
                {
                    // Set Position to the beginning of the stream.
                    stream.BaseStream.Position = startOffset * bytesOfSample;
                    byte[] byteArray = stream.ReadBytes((int)stream.BaseStream.Length);

                    int readBytes = CalculateBytesToRead(startOffset, sampleCount, bytesOfSample, byteArray.Length);

                    for (int i = 0; i < readBytes; i += bytesOfSample)
                    {
                        byte[] sampleBytes = new byte[bytesOfSample];
                        Array.Copy(byteArray, i, sampleBytes, 0, bytesOfSample);
                        resultSamples.Add(new AccelerometerSample(sampleBytes));
                    }
                } 
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.LoadAccelerometerReadingsFromFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.LoadAccelerometerReadingsFromFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return resultSamples;
        }

        #endregion

        //##################################################################################################################################
        //################################################## load GyrometerSamples #########################################################
        //##################################################################################################################################

        internal static async Task<bool> IsGyrometerSamplesAvailable(string filename)
        {
            bool isAvailable = false;
            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetGyrometerPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                if (file != null)
                {
                    isAvailable = true;
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.IsGyrometerSamplesAvailable] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.IsGyrometerSamplesAvailable] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return isAvailable;
        }

        internal static async Task<int> GetGyrometerSamplesCount(string filename)
        {
            int totalSampleCount = 0;
            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetGyrometerPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                using (BinaryReader stream = new BinaryReader(await file.OpenStreamForReadAsync()))
                {
                    int bytesOfSample = GyrometerSample.AmountOfBytes;
                    long totalBytesCount = stream.BaseStream.Length;

                    double result1 = totalBytesCount / bytesOfSample;
                    double roundResult = Math.Round(result1, 0, MidpointRounding.ToEven);
                    totalSampleCount = Convert.ToInt32(roundResult);
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.GetGyrometerSamplesCount] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.GetGyrometerSamplesCount] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return totalSampleCount;
        }

        internal static async Task<List<GyrometerSample>> LoadGyrometerSamplesFromFileAsync(string filename)
        {
            return await LoadGyrometerSamplesFromFileAsync(filename, 0, 0);
        }

        internal static async Task<List<GyrometerSample>> LoadGyrometerSamplesFromFileAsync(string filename, int startOffset, int sampleCount)
        {
            List<GyrometerSample> resultSamples = new List<GyrometerSample>();
            int bytesOfSample = GyrometerSample.AmountOfBytes;

            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetGyrometerPath());
                StorageFile file = await targetFolder.GetFileAsync(filename); 
                using (BinaryReader stream = new BinaryReader(await file.OpenStreamForReadAsync()))
                {
                    // Set Position to the beginning of the stream.
                    stream.BaseStream.Position = startOffset * bytesOfSample;
                    byte[] byteArray = stream.ReadBytes((int)stream.BaseStream.Length);

                    int readBytes = CalculateBytesToRead(startOffset, sampleCount, bytesOfSample, byteArray.Length);

                    for (int i = 0; i < readBytes; i += bytesOfSample)
                    {
                        byte[] sampleBytes = new byte[bytesOfSample];
                        Array.Copy(byteArray, i, sampleBytes, 0, bytesOfSample);
                        resultSamples.Add(new GyrometerSample(sampleBytes));
                    }
                } 
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.LoadGyrometerSamplesFromFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.LoadGyrometerSamplesFromFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return resultSamples;
        }

        //##################################################################################################################################
        //################################################## load QuaternionSamples ########################################################
        //##################################################################################################################################
        
        internal static async Task<bool> IsQuaterionSamplesAvailable(string filename)
        {
            bool isAvailable = false;
            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetQuaterionPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                if (file != null)
                {
                    isAvailable = true;
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.IsQuaterionSamplesAvailable] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.IsQuaterionSamplesAvailable] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return isAvailable;
        }

        internal static async Task<int> GetQuaterionSamplesCount(string filename)
        {
            int totalSampleCount = 0;
            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetQuaterionPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                using (BinaryReader stream = new BinaryReader(await file.OpenStreamForReadAsync()))
                {
                    int bytesOfSample = QuaternionSample.AmountOfBytes;
                    long totalBytesCount = stream.BaseStream.Length;

                    double result1 = totalBytesCount / bytesOfSample;
                    double roundResult = Math.Round(result1, 0, MidpointRounding.ToEven);
                    totalSampleCount = Convert.ToInt32(roundResult);
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.GetQuaterionSamplesCount] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.GetQuaterionSamplesCount] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return totalSampleCount;
        }

        internal static async Task<List<QuaternionSample>> LoadQuaternionSamplesFromFileAsync(string filename)
        {
            return await LoadQuaternionSamplesFromFileAsync(filename, 0, 0);
        }

        internal static async Task<List<QuaternionSample>> LoadQuaternionSamplesFromFileAsync(string filename, int startOffset, int sampleCount)
        {
            List<QuaternionSample> resultSamples = new List<QuaternionSample>();
            int bytesOfSample = QuaternionSample.AmountOfBytes;

            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetQuaterionPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                using (BinaryReader stream = new BinaryReader(await file.OpenStreamForReadAsync()))
                {
                    // Set Position to the beginning of the stream plus offsett.
                    stream.BaseStream.Position = 0 + startOffset * bytesOfSample;
                    byte[] byteArray = stream.ReadBytes((int)stream.BaseStream.Length);

                    int readBytes = CalculateBytesToRead(startOffset, sampleCount, bytesOfSample, byteArray.Length);

                    for (int i = 0; i < readBytes; i += bytesOfSample)
                    {
                        byte[] sampleBytes = new byte[bytesOfSample];
                        Array.Copy(byteArray, i, sampleBytes, 0, bytesOfSample);
                        resultSamples.Add(new QuaternionSample(sampleBytes));
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.LoadQuaternionSamplesFromFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.LoadQuaternionSamplesFromFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return resultSamples;
        }

        //##################################################################################################################################
        //################################################## load Geolocation ##############################################################
        //##################################################################################################################################
        
        internal static async Task<bool> IsGeolocationSamplesAvailable(string filename)
        {
            bool isAvailable = false;
            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetGeolocationPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                if (file != null)
                {
                    isAvailable = true;
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.IsGeolocationSamplesAvailable] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.IsGeolocationSamplesAvailable] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return isAvailable;
        }

        internal static async Task<int> GetGeolocationSamplesCount(string filename)
        {
            int totalSampleCount = 0;
            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetGeolocationPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                using (BinaryReader stream = new BinaryReader(await file.OpenStreamForReadAsync()))
                {
                    int bytesOfSample = GeolocationSample.AmountOfBytes;
                    long totalBytesCount = stream.BaseStream.Length;

                    double result1 = totalBytesCount / bytesOfSample;
                    double roundResult = Math.Round(result1, 0, MidpointRounding.ToEven);
                    totalSampleCount = Convert.ToInt32(roundResult);
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.GetGeolocationSamplesCount] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.GetGeolocationSamplesCount] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return totalSampleCount;
        }

        internal static async Task<List<GeolocationSample>> LoadGeolocationSamplesFromFileAsync(string filename)
        {
            return await LoadGeolocationSamplesFromFileAsync(filename, 0, 0);
        }

        internal static async Task<List<GeolocationSample>> LoadGeolocationSamplesFromFileAsync(string filename, int startOffset, int sampleCount)
        {
            List<GeolocationSample> resultSamples = new List<GeolocationSample>();
            int bytesOfSample = GeolocationSample.AmountOfBytes;

            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetGeolocationPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                using (BinaryReader stream = new BinaryReader(await file.OpenStreamForReadAsync()))
                {
                    // Set Position to the beginning of the stream plus offsett.
                    stream.BaseStream.Position = 0 + startOffset * bytesOfSample;
                    byte[] byteArray = stream.ReadBytes((int)stream.BaseStream.Length);

                    int readBytes = CalculateBytesToRead(startOffset, sampleCount, bytesOfSample, byteArray.Length);

                    for (int i = 0; i < readBytes; i += bytesOfSample)
                    {
                        byte[] sampleBytes = new byte[bytesOfSample];
                        Array.Copy(byteArray, i, sampleBytes, 0, bytesOfSample);
                        resultSamples.Add(new GeolocationSample(sampleBytes));
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.LoadGeolocationSamplesFromFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.LoadGeolocationSamplesFromFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return resultSamples;
        }

        //##################################################################################################################################
        //################################################## load Evaluations ##############################################################
        //##################################################################################################################################

        internal static async Task<bool> IsEvaluationSamplesAvailable(string filename)
        {
            bool isAvailable = false;
            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetEvaluationPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                if (file != null)
                {
                    isAvailable = true;
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.IsEvaluationSamplesAvailable] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.IsEvaluationSamplesAvailable] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return isAvailable;
        }

        internal static async Task<int> GetEvaluationSamplesCount(string filename)
        {
            int totalSampleCount = 0;
            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetEvaluationPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                using (BinaryReader stream = new BinaryReader(await file.OpenStreamForReadAsync()))
                {
                    int bytesOfSample = EvaluationSample.AmountOfBytes;
                    long totalBytesCount = stream.BaseStream.Length;

                    double result1 = totalBytesCount / bytesOfSample;
                    double roundResult = Math.Round(result1, 0, MidpointRounding.ToEven);
                    totalSampleCount = Convert.ToInt32(roundResult);
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.GetEvaluationSamplesCount] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.GetEvaluationSamplesCount] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return totalSampleCount;
        }

        internal static async Task<List<EvaluationSample>> LoadEvaluationSamplesFromFileAsync(string filename)
        {
            return await LoadEvaluationSamplesFromFileAsync(filename, 0, 0);
        }

        internal static async Task<List<EvaluationSample>> LoadEvaluationSamplesFromFileAsync(string filename, int startOffset, int sampleCount)
        {
            List<EvaluationSample> resultSamples = new List<EvaluationSample>();
            int bytesOfSample = EvaluationSample.AmountOfBytes;

            try
            {
                StorageFolder targetFolder = await FileService.FindStorageFolder(FileService.GetEvaluationPath());
                StorageFile file = await targetFolder.GetFileAsync(filename);
                using (BinaryReader stream = new BinaryReader(await file.OpenStreamForReadAsync()))
                {
                    // Set Position to the beginning of the stream plus offsett.
                    stream.BaseStream.Position = 0 + startOffset * bytesOfSample;
                    byte[] byteArray = stream.ReadBytes((int)stream.BaseStream.Length);

                    int readBytes = CalculateBytesToRead(startOffset, sampleCount, bytesOfSample, byteArray.Length);

                    for (int i = 0; i < readBytes; i += bytesOfSample)
                    {
                        byte[] sampleBytes = new byte[bytesOfSample];
                        Array.Copy(byteArray, i, sampleBytes, 0, bytesOfSample);
                        resultSamples.Add(new EvaluationSample(sampleBytes));
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.LoadEvaluationFromFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.LoadEvaluationFromFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return resultSamples;
        }

        //##################################################################################################################################
        //################################################## Save into File ################################################################
        //##################################################################################################################################

        #region Save/Load JSONString

        internal static async Task SaveJsonStringToFile(string jsonString, StorageFolder targetFolder, string filename)
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
                Debug.WriteLine("[SturzAppProject2.FileService.SaveJsonStringToFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.SaveJsonStringToFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return;
        }

        internal static async Task SaveBytesToEndOfFileAsync(byte[] appendBytes, StorageFolder targetFolder, string filename)
        {
            try
            {
                StorageFile file = await targetFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                using (IRandomAccessStream textStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (DataWriter textWriter = new DataWriter(textStream.GetOutputStreamAt(textStream.Size)))
                    {
                        textWriter.WriteBytes(appendBytes);
                        await textWriter.StoreAsync();
                    }
                    Debug.WriteLine("############## Current file size: '{0}' in '{1}' ################", textStream.Size, filename);
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[BackgroundTask.Service.TaskFileService.SaveStringToFile] Datei: {0} konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[BackgroundTask.Service.TaskFileService.SaveStringToFile] Datei: {0} konnte nicht zugegriffen werden.", filename);
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
                    Debug.WriteLine("[SturzAppProject2.FileService.LoadJsonStringFromFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("[SturzAppProject2.FileService.LoadJsonStringFromFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
                }
            }
            return jsonString;
        }

        #endregion

        //##################################################################################################################################
        //################################################## delete File ###################################################################
        //##################################################################################################################################

        public static async Task DeleteAllMeasurementFilesAsync(MeasurementDataSets dataSets, string filename)
        {
            Task accelerometerDeleteTask = null;
            Task gyrometerDeleteTask = null;
            Task quaternionDeleteTask = null;
            Task geolocationDeleteTask = null;
            Task evaluationDeleteTask = null;

            if (dataSets.accelerometerDataSet.IsAvailable)
            {
                accelerometerDeleteTask = DeleteFileFromFolderAsync(_accelerometerPath, filename);
            }
            if (dataSets.gyrometerDataSet.IsAvailable)
            {
                gyrometerDeleteTask = DeleteFileFromFolderAsync(_gyrometerPath, filename);
            }
            if (dataSets.quaterionDataSet.IsAvailable)
            {
                quaternionDeleteTask = DeleteFileFromFolderAsync(_quaternionPath, filename);
            }
            if (dataSets.geolocationDataSet.IsAvailable)
            {
                geolocationDeleteTask = DeleteFileFromFolderAsync(_geolocationPath, filename);
            }
            if (dataSets.evaluationDataSet.IsAvailable)
            {
                evaluationDeleteTask = DeleteFileFromFolderAsync(_evaluationPath, filename);
            }

            if (accelerometerDeleteTask != null)
            {
                await accelerometerDeleteTask;
            }
            if (gyrometerDeleteTask != null)
            {
                await gyrometerDeleteTask;
            }
            if (quaternionDeleteTask != null)
            {
                await quaternionDeleteTask;
            }
            if (geolocationDeleteTask != null)
            {
                await geolocationDeleteTask;
            }
            if (evaluationDeleteTask != null)
            {
                await evaluationDeleteTask;
            }
            return;
        }

        private static async Task DeleteFileFromFolderAsync(string folderPath, string filename)
        {
            if (filename != null && filename != string.Empty &&
                folderPath != null && folderPath != string.Empty)
            {
                StorageFolder folder = await FindStorageFolder(folderPath);
                await DeleteFileAsync(folder, filename);
            }
            return;
        }

        internal static async Task DeleteFileAsync(StorageFolder targetFolder, string filename)
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
                Debug.WriteLine("[SturzAppProject2.FileService.DeleteFileAsync] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.DeleteFileAsync] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return;
        }

        //##################################################################################################################################
        //################################################## helper method #################################################################
        //##################################################################################################################################

        private static int CalculateBytesToRead(int startOffset, int sampleCount, int bytesOfSample, int totalBytesCount)
        {
            int sampleBytes = sampleCount * bytesOfSample;
            int startOffsettBytes = startOffset * bytesOfSample;

            int resultReadBytes = totalBytesCount - startOffsettBytes;
            if (sampleBytes > 0 &&
                sampleBytes <= totalBytesCount - startOffsettBytes)
            {
                resultReadBytes = sampleBytes;
            }
            return resultReadBytes;
        }
    }
}
