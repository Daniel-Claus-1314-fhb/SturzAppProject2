using BackgroundTask.DataModel;
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

        private static NumberStyles _styles = NumberStyles.Any;
        private static IFormatProvider _provider = new CultureInfo("en-US");

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
        //################################################## Save Evaluation data ##########################################################
        //##################################################################################################################################

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationResultModel"></param>
        /// <returns></returns>
        public static async Task SaveEvaluationDataToFileAsync(String filename, EvaluationResultModel evaluationResultModel)
        {
            if (filename != null && filename != String.Empty && evaluationResultModel.EvaluationResultList.Count > 0)
            {
                // find folder
                StorageFolder folder = await FindStorageFolder(_evaluationPath);
                // convert data into csv
                string csvString = evaluationResultModel.ToEvaluationResultCSVString();
                // delete old evaluationData
                await DeleteFileAsync(folder, filename);
                // save csv string
                await SaveJsonStringToFile(csvString, folder, filename);
            }
            return;
        }

        //##################################################################################################################################
        //################################################## Save export data ##############################################################
        //##################################################################################################################################

        public static async Task SaveExportDataToFileAsync(StorageFile file, ExportData exportData)
        {
            using (IRandomAccessStream textStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (DataWriter textWriter = new DataWriter(textStream.GetOutputStreamAt(textStream.Size)))
                {
                    // Append accelerometer header and accelerometer samples
                    if (exportData.AccelerometerSamples != null && exportData.AccelerometerSamples.Count > 0)
                    {
                        // insert header for accelerometer data
                        textWriter.WriteString(exportData.AccelerometerSamples.ElementAt(0).GetExportHeader());
                        var enumerator = exportData.AccelerometerSamples.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            textWriter.WriteString(enumerator.Current.ToExportCSVString());
                        }
                    }
                    // Append gyrometer header and gyrometer samples
                    if (exportData.GyrometerSamples != null && exportData.GyrometerSamples.Count > 0)
                    {
                        // insert header for gyrometer data
                        textWriter.WriteString(exportData.GyrometerSamples.ElementAt(0).GetExportHeader());
                        var enumerator = exportData.GyrometerSamples.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            textWriter.WriteString(enumerator.Current.ToExportCSVString());
                        }
                    }
                    // Append quaternion header and quaternion samples
                    if (exportData.QuaternionSamples != null && exportData.QuaternionSamples.Count > 0)
                    {
                        // insert header for quaternion data
                        textWriter.WriteString(exportData.QuaternionSamples.ElementAt(0).GetExportHeader());
                        var enumerator = exportData.QuaternionSamples.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            textWriter.WriteString(enumerator.Current.ToExportCSVString());
                        }
                    }
                    // Append evaluation header and evaluation samples
                    if (exportData.EvaluationSamples != null && exportData.EvaluationSamples.Count > 0)
                    {
                        // insert header for evaluation data
                        textWriter.WriteString(exportData.EvaluationSamples.ElementAt(0).GetExportHeader());
                        var enumerator = exportData.EvaluationSamples.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            textWriter.WriteString(enumerator.Current.ToExportCSVString());
                        }
                    }
                    await textWriter.StoreAsync();
                }
            }
            return;
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


        //##################################################################################################################################
        //################################################## Load Oxyplot data #############################################################
        //##################################################################################################################################

        #region Load OxyplotData

        internal static async Task<OxyplotData> LoadOxyplotDataAsync(string filename)
        {
            OxyplotData oxyplotData = new OxyplotData();
            if (filename != null & filename != String.Empty)
            {
                StorageFolder accelerometerFolder = await FindStorageFolder(_measurementAccelerometerPath);
                StorageFolder gyrometerFolder = await FindStorageFolder(_measurementGyrometerPath);
                StorageFolder quaternionFolder = await FindStorageFolder(_measurementQuaternionPath);
                StorageFolder evaluationFolder = await FindStorageFolder(_evaluationPath);

                Task<List<AccelerometerSample>> loadAccelerometerTask = LoadAccelerometerSamplesFromFile(accelerometerFolder, filename);
                Task<List<GyrometerSample>> loadGyrometerTask = LoadGyrometerSamplesFromFile(gyrometerFolder, filename);
                Task<List<QuaternionSample>> loadQuaternionTask = LoadQuaternionSamplesFromFile(quaternionFolder, filename);
                Task<List<EvaluationSample>> loadEvaluationSamplesTask = LoadEvaluationSamplesFromFile(evaluationFolder, filename);

                oxyplotData.AccelerometerSamples = await loadAccelerometerTask;
                oxyplotData.GyrometerSamples = await loadGyrometerTask;
                oxyplotData.QuaternionSamples = await loadQuaternionTask;
                oxyplotData.EvaluationSamples = await loadEvaluationSamplesTask;
            }
            return oxyplotData;
        }

        #endregion

        //##################################################################################################################################
        //################################################## Load Export Data ##############################################################
        //##################################################################################################################################

        #region Load Evaluation Data

        internal static async Task<ExportData> LoadSamplesForExportAsync(string filename)
        {
            ExportData exportData = new ExportData();

            if (filename != null && filename != string.Empty)
            {
                StorageFolder accelerometerFolder = await FindStorageFolder(_measurementAccelerometerPath);
                StorageFolder gyrometerFolder = await FindStorageFolder(_measurementGyrometerPath);
                StorageFolder quaternionFolder = await FindStorageFolder(_measurementQuaternionPath);
                StorageFolder evaluationFolder = await FindStorageFolder(_evaluationPath);

                Task<List<AccelerometerSample>> loadAccelerometerTask = LoadAccelerometerSamplesFromFile(accelerometerFolder, filename);
                Task<List<GyrometerSample>> loadGyrometerTask = LoadGyrometerSamplesFromFile(gyrometerFolder, filename);
                Task<List<QuaternionSample>> loadQuaternionTask = LoadQuaternionSamplesFromFile(quaternionFolder, filename);
                Task<List<EvaluationSample>> loadEvaluationSamplesTask = LoadEvaluationSamplesFromFile(evaluationFolder, filename);

                exportData.AccelerometerSamples = await loadAccelerometerTask;
                exportData.GyrometerSamples = await loadGyrometerTask;
                exportData.QuaternionSamples = await loadQuaternionTask;
                exportData.EvaluationSamples = await loadEvaluationSamplesTask;
            }
            return exportData;
        }

        #endregion

        //##################################################################################################################################
        //################################################## Load Evaluation Data ##########################################################
        //##################################################################################################################################

        #region Load Evaluation Data

        internal static async Task<EvaluationDataModel> LoadSamplesForEvaluationAsync(string filename)
        {
            EvaluationDataModel evaluationData = new EvaluationDataModel();

            if (filename != null && filename != string.Empty)
            {
                StorageFolder accelerometerFolder = await FindStorageFolder(_measurementAccelerometerPath);
                StorageFolder gyrometerFolder = await FindStorageFolder(_measurementGyrometerPath);
                StorageFolder quaternionFolder = await FindStorageFolder(_measurementQuaternionPath);

                Task<List<AccelerometerSample>> loadAccelerometerTask = LoadAccelerometerSamplesFromFile(accelerometerFolder, filename);
                Task<List<GyrometerSample>> loadGyrometerTask = LoadGyrometerSamplesFromFile(gyrometerFolder, filename);
                Task<List<QuaternionSample>> loadQuaternionTask = LoadQuaternionSamplesFromFile(quaternionFolder, filename);

                evaluationData.AddAllAccelerometerAnalysisFromSampleList(await loadAccelerometerTask);
                evaluationData.AddAllGyrometerAnalysisFromSampleList(await loadGyrometerTask);
                evaluationData.AddAllQuaternionAnalysisFromSampleList(await loadQuaternionTask);
            }
            return evaluationData;
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
        //################################################## load AccerlerometerSamples ####################################################
        //##################################################################################################################################
        
        private static async Task<List<AccelerometerSample>> LoadAccelerometerSamplesFromFile(StorageFolder targetFolder, string filename)
        {
            List<AccelerometerSample> accelerometerSampleList = new List<AccelerometerSample>();

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

                        if (Double.TryParse(stringArray[0], _styles, _provider, out timeStampTicks) &&
                            Double.TryParse(stringArray[1], _styles, _provider, out accerlerometerX) &&
                            Double.TryParse(stringArray[2], _styles, _provider, out accerlerometerY) &&
                            Double.TryParse(stringArray[3], _styles, _provider, out accerlerometerZ))
                        {
                            accelerometerSampleList.Add(new AccelerometerSample(TimeSpan.FromMilliseconds(timeStampTicks), accerlerometerX, accerlerometerY, accerlerometerZ));
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
            return accelerometerSampleList;
        }

        #endregion

        //##################################################################################################################################
        //################################################## load GyrometerSamples #########################################################
        //##################################################################################################################################

        private static async Task<List<GyrometerSample>> LoadGyrometerSamplesFromFile(StorageFolder targetFolder, string filename)
        {
            List<GyrometerSample> gyrometerSampleList = new List<GyrometerSample>();

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
                        double velocityX;
                        double velocityY;
                        double velocityZ;

                        if (Double.TryParse(stringArray[0], _styles, _provider, out timeStampTicks) &&
                            Double.TryParse(stringArray[1], _styles, _provider, out velocityX) &&
                            Double.TryParse(stringArray[2], _styles, _provider, out velocityY) &&
                            Double.TryParse(stringArray[3], _styles, _provider, out velocityZ))
                        {
                            gyrometerSampleList.Add(new GyrometerSample(TimeSpan.FromMilliseconds(timeStampTicks), velocityX, velocityY, velocityZ));
                        }
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
            return gyrometerSampleList;
        }

        //##################################################################################################################################
        //################################################## load QuaternionSamples ########################################################
        //##################################################################################################################################

        private static async Task<List<QuaternionSample>> LoadQuaternionSamplesFromFile(StorageFolder targetFolder, string filename)
        {
            List<QuaternionSample> quaternionSampleList = new List<QuaternionSample>();

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
                        double angleW;
                        double coordinateX;
                        double coordinateY;
                        double coordinateZ;

                        if (Double.TryParse(stringArray[0], _styles, _provider, out timeStampTicks) &&
                            Double.TryParse(stringArray[1], _styles, _provider, out angleW) &&
                            Double.TryParse(stringArray[2], _styles, _provider, out coordinateX) &&
                            Double.TryParse(stringArray[3], _styles, _provider, out coordinateY) &&
                            Double.TryParse(stringArray[4], _styles, _provider, out coordinateZ))
                        {
                            quaternionSampleList.Add(new QuaternionSample(TimeSpan.FromMilliseconds(timeStampTicks), angleW, coordinateX, coordinateY, coordinateZ));
                        }
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
            return quaternionSampleList;
        }

        //##################################################################################################################################
        //################################################## load Evaluations ##############################################################
        //##################################################################################################################################

        private static async Task<List<EvaluationSample>> LoadEvaluationSamplesFromFile(StorageFolder targetFolder, string filename)
        {
            List<EvaluationSample> evaluationData = new List<EvaluationSample>();

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
                        double gyrometerVectorLength;
                        int assumedAccelerometerStep;
                        int assumedGyrometerStep;
                        int detectedStep;

                        if (Double.TryParse(stringArray[0], _styles, _provider, out timeStampTicks) &&
                            Double.TryParse(stringArray[1], _styles, _provider, out accerlerometerVectorLength) &&
                            Double.TryParse(stringArray[2], _styles, _provider, out gyrometerVectorLength) &&
                            Int32.TryParse(stringArray[3], out assumedAccelerometerStep) &&
                            Int32.TryParse(stringArray[4], out assumedGyrometerStep) &&
                            Int32.TryParse(stringArray[5], out detectedStep))
                        {
                            evaluationData.Add(new EvaluationSample(TimeSpan.FromMilliseconds(timeStampTicks), accerlerometerVectorLength, gyrometerVectorLength,
                                assumedAccelerometerStep == 0 ? false : true, assumedGyrometerStep == 0 ? false : true, detectedStep == 0 ? false : true));
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
            return evaluationData;
        }

        //##################################################################################################################################
        //################################################## Save into File ################################################################
        //##################################################################################################################################

        #region Save/Load JSONString

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
            Task accelerometerDeleteTask = DeleteFileFromFolderAsync(_measurementAccelerometerPath, filename);
            Task gyrometerDeleteTask = DeleteFileFromFolderAsync(_measurementGyrometerPath, filename);
            Task quaternionDeleteTask = DeleteFileFromFolderAsync(_measurementQuaternionPath, filename);
            Task evaluationDeleteTask = DeleteFileFromFolderAsync(_evaluationPath, filename);

            await accelerometerDeleteTask;
            await gyrometerDeleteTask;
            await quaternionDeleteTask;
            await evaluationDeleteTask;
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
