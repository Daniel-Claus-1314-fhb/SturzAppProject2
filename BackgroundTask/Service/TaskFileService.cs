using BackgroundTask.DataModel;
using SensorDataEvaluation.DataModel;
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
        private static readonly string _measurementAccelerometerPath = @"Measurement\Accelerometer";
        private static readonly string _measurementGyrometerPath = @"Measurement\Gyrometer";
        private static readonly string _measurementQuaternionPath = @"Measurement\Quaternion";
        private static readonly string _evaluationPath = @"Evaluation";

        public static async Task AppendMeasurementDataToFileAsync(String filename, MeasurementData measurementData, bool isActiveListChoosen)
        {
            Task accelerometerDataTask = AppendAccelerometerDataToFileAsync(filename, measurementData, isActiveListChoosen);
            Task gyrometerDataTask = AppendGyrometerDataToFileAsync(filename, measurementData, isActiveListChoosen);
            Task quaterionDataTask = AppendQuaternionDataToFileAsync(filename, measurementData, isActiveListChoosen);

            await accelerometerDataTask;
            await gyrometerDataTask;
            await quaterionDataTask;
        }

        //##################################################################################################################################
        //################################################## Save Accelerometer data #######################################################
        //##################################################################################################################################
        
        /// <summary>
        /// Saves accelerometer tuples form the given accelerometer tuples list to the end of file.
        /// Importent: Await Task to be sure all accelerometer tuples has been saved.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="measurementData"></param>
        /// <param name="isActiveListChoosen"></param>
        /// <returns></returns>
        public static async Task AppendAccelerometerDataToFileAsync(String filename, MeasurementData measurementData, bool isActiveListChoosen)
        {
            if (filename != null && filename != String.Empty)
            {
                // convert data into csv
                string csvString = measurementData.ToAccelerometerCSVString(isActiveListChoosen);
                if (csvString != null && csvString != String.Empty)
                {
                    // find folder
                    StorageFolder accelerometerFolder = await FindStorageFolder(_measurementAccelerometerPath);
                    // save csv string
                    await SaveStringToEndOfFileAsync(csvString, accelerometerFolder, filename);
                }
            }
            return;
        }

        //##################################################################################################################################
        //################################################## Save Gyrometer data ###########################################################
        //##################################################################################################################################

        /// <summary>
        /// Saves gyrometer tuples form the given gyrometer tuples list to the end of file.
        /// Importent: Await Task to be sure all gyrometer tuples has been saved.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="measurementData"></param>
        /// <param name="isActiveListChoosen"></param>
        /// <returns></returns>
        public static async Task AppendGyrometerDataToFileAsync(String filename, MeasurementData measurementData, bool isActiveListChoosen)
        {
            if (filename != null && filename != String.Empty)
            {
                // convert data into csv
                string csvString = measurementData.ToGyrometerCSVString(isActiveListChoosen);
                if (csvString != null && csvString != String.Empty)
                {
                    // find folder
                    StorageFolder gyrometerFolder = await FindStorageFolder(_measurementGyrometerPath);
                    // save csv string
                    await SaveStringToEndOfFileAsync(csvString, gyrometerFolder, filename);
                }
            }
            return;
        }

        //##################################################################################################################################
        //################################################## Save Quaternion data ##########################################################
        //##################################################################################################################################

        /// <summary>
        /// Saves quaternion tuples form the given quaternion tuples list to the end of file.
        /// Importent: Await Task to be sure all quaternion tuples has been saved.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="accelerometerTupleList"></param>
        /// <returns></returns>
        public static async Task AppendQuaternionDataToFileAsync(String filename, MeasurementData measurementData, bool isActiveListChoosen)
        {
            if (filename != null && filename != String.Empty)
            {
                // convert data into csv
                string csvString = measurementData.ToQuaternionCSVString(isActiveListChoosen);
                if (csvString != null && csvString != String.Empty)
                {
                    // find folder
                    StorageFolder gyrometerFolder = await FindStorageFolder(_measurementQuaternionPath);
                    // save csv string
                    await SaveStringToEndOfFileAsync(csvString, gyrometerFolder, filename);
                }
            }
            return;
        }

        //##################################################################################################################################
        //################################################## Save Evaluation data ##########################################################
        //##################################################################################################################################

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evaluationResultModel"></param>
        /// <returns></returns>
        public static async Task AppendEvaluationDataToFileAsync(String filename, EvaluationResultModel evaluationResultModel)
        {
            if (filename != null && filename != String.Empty && evaluationResultModel.EvaluationResultList.Count > 0)
            {
                // find folder
                StorageFolder folder = await FindStorageFolder(_evaluationPath);
                // convert data into csv
                string csvString = evaluationResultModel.ToEvaluationResultCSVString();
                // save csv string
                await SaveStringToEndOfFileAsync(csvString, folder, filename);
            }
            return;
        }

        //##################################################################################################################################
        //################################################## find folder ###################################################################
        //##################################################################################################################################

        private static async Task<StorageFolder> FindStorageFolder(string folderPath)
        {
            StorageFolder resultFolder = ApplicationData.Current.LocalFolder;
            if (folderPath != null && folderPath != String.Empty)
            {
                try
                {
                    resultFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderPath, CreationCollisionOption.OpenIfExists);
                }
                catch (FileNotFoundException)
                {
                    Debug.WriteLine("[BackgroundTask.Service.TaskFileService.FindMeasurementStorageFolder] Ordner: '{0}' konnte nicht gefunden werden.", folderPath);
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("[BackgroundTask.Service.TaskFileService.FindMeasurementStorageFolder] Ordner: '{0}' konnte nicht zugegriffen werden.", folderPath);
                }
            }
            return resultFolder;
        }

        //##################################################################################################################################
        //################################################## save sting into folder ########################################################
        //##################################################################################################################################

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
    }
}
