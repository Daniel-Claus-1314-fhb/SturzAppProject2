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
        private static readonly string _measurementGeolocationPath = @"Measurement\Geolocation";
        private static readonly string _evaluationPath = @"Evaluation";

        public static async Task AppendMeasurementDataToFileAsync(TaskArguments taskArguments, MeasurementData measurementData, bool isActiveListChoosen)
        {
            Task accelerometerDataTask = null;
            Task gyrometerDataTask = null;
            Task quaterionDataTask = null;

            if (taskArguments.IsUsedAccelerometer && taskArguments.IsRecordSamplesAccelerometer) 
            { 
                accelerometerDataTask = AppendAccelerometerDataToFileAsync(taskArguments.Filename, measurementData, isActiveListChoosen);
            }
            if (taskArguments.IsUsedGyrometer && taskArguments.IsRecordSamplesGyrometer)
            {
                gyrometerDataTask = AppendGyrometerDataToFileAsync(taskArguments.Filename, measurementData, isActiveListChoosen);
            }
            if (taskArguments.IsUsedQuaternion && taskArguments.IsRecordSamplesQuaternion)
            {
                quaterionDataTask = AppendQuaternionDataToFileAsync(taskArguments.Filename, measurementData, isActiveListChoosen);
            }

            if (accelerometerDataTask != null)
            {
                await accelerometerDataTask;
            }
            if (gyrometerDataTask != null)
            {
                await gyrometerDataTask;
            }
            if (quaterionDataTask != null)
            {
                await quaterionDataTask;
            }
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
                // convert data into byte array
                byte[] bytes = measurementData.ToAccelerometerBytes(isActiveListChoosen);
                if (bytes != null && bytes.Length > 0)
                {
                    // find folder
                    StorageFolder accelerometerFolder = await FindStorageFolder(_measurementAccelerometerPath);
                    // save byte array
                    await SaveBytesToEndOfFileAsync(bytes, accelerometerFolder, filename);
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
                // convert data into byte array
                byte[] bytes = measurementData.ToGyrometerBytes(isActiveListChoosen);
                if (bytes != null && bytes.Length > 0)
                {
                    // find folder
                    StorageFolder gyrometerFolder = await FindStorageFolder(_measurementGyrometerPath);
                    // save byte array
                    await SaveBytesToEndOfFileAsync(bytes, gyrometerFolder, filename);
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
                // convert data into byte array
                byte[] bytes = measurementData.ToQuaternionBytes(isActiveListChoosen);
                if (bytes != null && bytes.Length > 0)
                {
                    // find folder
                    StorageFolder gyrometerFolder = await FindStorageFolder(_measurementQuaternionPath);
                    // save csv string
                    await SaveBytesToEndOfFileAsync(bytes, gyrometerFolder, filename);
                }
            }
            return;
        }

        //##################################################################################################################################
        //################################################## Save Geolocation data #########################################################
        //##################################################################################################################################

        /// <summary>
        /// Saves quaternion tuples form the given quaternion tuples list to the end of file.
        /// Importent: Await Task to be sure all quaternion tuples has been saved.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="accelerometerTupleList"></param>
        /// <returns></returns>
        public static async Task AppendGeolocationDataToFileAsync(String filename, MeasurementData measurementData, bool isActiveListChoosen)
        {
            if (filename != null && filename != String.Empty)
            {
                // convert data into byte array
                byte[] bytes = measurementData.ToLocationBytes(isActiveListChoosen);
                if (bytes != null && bytes.Length > 0)
                {
                    // find folder
                    StorageFolder folder = await FindStorageFolder(_measurementGeolocationPath);
                    // save csv string
                    await SaveBytesToEndOfFileAsync(bytes, folder, filename);
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
        public static async Task AppendEvaluationDataToFileAsync(TaskArguments taskArguments, EvaluationResultModel evaluationResultModel)
        {
            if (taskArguments.IsRecordSamplesEvaluation && 
                taskArguments.Filename != null && taskArguments.Filename != String.Empty && 
                evaluationResultModel.EvaluationResultList.Count > 0)
            {
                // convert data into byte array
                byte[] bytes = evaluationResultModel.ToEvaluationBytes();
                if (bytes != null && bytes.Length > 0)
                {
                    // find folder
                    StorageFolder folder = await FindStorageFolder(_evaluationPath);
                    // save byte array
                    await SaveBytesToEndOfFileAsync(bytes, folder, taskArguments.Filename);
                }
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
        
        private static async Task SaveBytesToEndOfFileAsync(byte[] appendBytes, StorageFolder targetFolder, string filename)
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
    }
}
