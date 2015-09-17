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
    internal static class FileService
    {
        private static readonly string _measurementsMetaDataFilename = "Measurements.json";

        private static readonly string _measurementsMetaDataPath = @"";
        private static readonly string _measurementAccelerometerPath = @"Measurement\Accelerometer";
        private static readonly string _measurementGyrometerPath = @"Measurement\Gyrometer";
        private static readonly string _measurementQuaternionPath = @"Measurement\Quaternion";
        private static readonly string _evaluationPath = @"Evaluation";

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
                StorageFolder accelerometerEvaluationFolder = await FindStorageFolder(_evaluationPath);

                Task<List<Tuple<TimeSpan, double, double, double>>> loadAccelerometerTask = LoadAccelerometerReadingsFromFile(accelerometerFolder, measurement.Filename);
                Task<List<Tuple<TimeSpan, double, int>>> loadAccelerometerEvaluationTask = LoadAccelerometerEvaluationFromFile(accelerometerEvaluationFolder, measurement.Filename);

                oxyplotData.AccelerometerReadings = await loadAccelerometerTask;
                oxyplotData.AccelerometerEvaluationList = await loadAccelerometerEvaluationTask;
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
            NumberStyles styles = NumberStyles.Any;
            IFormatProvider provider = new CultureInfo("en-US");

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

                        double timeStampTicks;
                        double accerlerometerX;
                        double accerlerometerY; 
                        double accerlerometerZ;

                        if (Double.TryParse(stringArray[0], styles, provider, out timeStampTicks) && 
                            Double.TryParse(stringArray[1], styles, provider, out accerlerometerX) &&
                            Double.TryParse(stringArray[2], styles, provider, out accerlerometerY) &&
                            Double.TryParse(stringArray[3], styles, provider, out accerlerometerZ))
                        {
                            accelerometerReadingTuples.Add(new Tuple<TimeSpan, double, double, double>(TimeSpan.FromMilliseconds(timeStampTicks), accerlerometerX, accerlerometerY, accerlerometerZ));
                        }
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
            return accelerometerReadingTuples;
        }
        
        #endregion

        //##################################################################################################################################
        //################################################## load Evaluations ##############################################################
        //##################################################################################################################################

        private static async Task<List<Tuple<TimeSpan, double, int>>> LoadAccelerometerEvaluationFromFile(StorageFolder targetFolder, string filename)
        {
            NumberStyles styles = NumberStyles.Any;
            IFormatProvider provider = new CultureInfo("en-US");

            List<Tuple<TimeSpan, double, int>> accelerometerVectorLengthTuples = new List<Tuple<TimeSpan, double, int>>();

            try
            {
                StorageFile file = await targetFolder.GetFileAsync(filename);
                using (StreamReader stream = new StreamReader(await file.OpenStreamForReadAsync()))
                {
                    while (!stream.EndOfStream)
                    {
                        string currentReadLineOfFile = await stream.ReadLineAsync();
                        string[] stringArray = currentReadLineOfFile.Split(new Char[] { ',' });

                        double timeStampTicks;
                        double accerlerometerVectorLength;
                        int detectedStep;

                        if (Double.TryParse(stringArray[0], styles, provider, out timeStampTicks) &&
                            Double.TryParse(stringArray[1], styles, provider, out accerlerometerVectorLength) &&
                            Int32.TryParse(stringArray[2],out detectedStep))
                        {
                            accelerometerVectorLengthTuples.Add(new Tuple<TimeSpan, double, int>(TimeSpan.FromMilliseconds(timeStampTicks), accerlerometerVectorLength, detectedStep));
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.LoadAccelerometerEvaluationFromFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.LoadAccelerometerEvaluationFromFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return accelerometerVectorLengthTuples;
        }

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
                Debug.WriteLine("[SturzAppProject2.FileService.SaveJsonStringToFile] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.SaveJsonStringToFile] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
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

        public static async Task DeleteAllMeasurementFilesAsync(string filename)
        {
            Task accelerometerDeleteTask = DeleteAccelerometerFileAsync(filename);
            Task gyrometerDeleteTask = DeleteGyrometerFileAsync(filename);
            Task quaternionDeleteTask = DeleteQuaternionFileAsync(filename);
            Task evaluationDeleteTask = DeleteEvaluationFileAsync(filename);

            await accelerometerDeleteTask;
            await gyrometerDeleteTask;
            await quaternionDeleteTask;
            await evaluationDeleteTask;
            return;
        }

        private static async Task DeleteAccelerometerFileAsync(string filename)
        {
            if (filename != null && filename != string.Empty)
            {
                StorageFolder accelerometerFolder = await FindStorageFolder(_measurementAccelerometerPath);
                await DeleteFileAsync(accelerometerFolder, filename);
            }
            return;
        }

        private static async Task DeleteGyrometerFileAsync(string filename)
        {
            if (filename != null && filename != string.Empty)
            {
                StorageFolder accelerometerFolder = await FindStorageFolder(_measurementGyrometerPath);
                await DeleteFileAsync(accelerometerFolder, filename);
            }
            return;
        }

        private static async Task DeleteQuaternionFileAsync(string filename)
        {
            if (filename != null && filename != string.Empty)
            {
                StorageFolder accelerometerFolder = await FindStorageFolder(_measurementQuaternionPath);
                await DeleteFileAsync(accelerometerFolder, filename);
            }
            return;
        }

        private static async Task DeleteEvaluationFileAsync(string filename)
        {
            if (filename != null && filename != string.Empty)
            {
                StorageFolder accelerometerEvaluationFolder = await FindStorageFolder(_evaluationPath);
                await DeleteFileAsync(accelerometerEvaluationFolder, filename);
            }
            return;
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
                Debug.WriteLine("[SturzAppProject2.FileService.DeleteFileAsync] Datei: '{0}' konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[SturzAppProject2.FileService.DeleteFileAsync] Datei: '{0}' konnte nicht zugegriffen werden.", filename);
            }
            return;
        }
    }
}
