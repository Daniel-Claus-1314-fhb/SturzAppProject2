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
        private static readonly string _evaluationAccelerometerPath = @"Evaluation\Accelerometer";
        private static readonly string _evaluationGyrometerPath = @"Evaluation\Gyrometer";

        //##################################################################################################################################
        //################################################## Save Accelerometer data #######################################################
        //##################################################################################################################################

        /// <summary>
        /// Saves accelerometer tuples form to passiv accelerometer tuples list to the end of file.
        /// </summary>
        /// <param name="accelerometerData"></param>
        public static async void AppendPassivAccelerometerDataToFileAsync(AccelerometerData accelerometerData)
        {
            // find folder
            StorageFolder accelerometerFolder = await FindStorageFolder(_measurementAccelerometerPath);
            // convert data into csv
            string csvString = ConvertAccelerometerDataIntoCSVString(accelerometerData.GetPassivTupleList());
            // save csv string
            await SaveStringToEndOfFileAsync(csvString, accelerometerFolder, accelerometerData.Filename);
        }

        /// <summary>
        /// Saves accelerometer tuples form to active accelerometer tuples list to the end of file.
        /// Note: Is used only to save accelerometer tuples when the background task has been canceled. 
        /// Importent: Await Task to be sure all accelerometer tuples has been saved.
        /// </summary>
        /// <param name="accelerometerData"></param>
        /// <returns></returns>
        public static async Task AppendActivAccelerometerDataToFileAsync(AccelerometerData accelerometerData)
        {
            // find folder
            StorageFolder accelerometerFolder = await FindStorageFolder(_measurementAccelerometerPath);
            // convert data into csv
            string csvString = ConvertAccelerometerDataIntoCSVString(accelerometerData.GetActivTupleList());
            // save csv string
            await SaveStringToEndOfFileAsync(csvString, accelerometerFolder, accelerometerData.Filename);
            return;
        }

        //##################################################################################################################################
        //################################################## Save Evaluation data ##########################################################
        //##################################################################################################################################

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accelerometerEvaluation"></param>
        /// <returns></returns>
        public static async Task AppendEvaluationDataToFileAsync(AccelerometerEvaluation accelerometerEvaluation)
        {
            // find folder
            StorageFolder accelerometerFolder = await FindStorageFolder(_evaluationAccelerometerPath);
            // convert data into csv
            string csvString = ConvertEvaluationDataIntoCSVString(accelerometerEvaluation.AccelerometerEvaluationList);
            // save csv string
            await SaveStringToEndOfFileAsync(csvString, accelerometerFolder, accelerometerEvaluation.Filename);
        }

        //##################################################################################################################################
        //################################################## Convert Accelerometer data ####################################################
        //##################################################################################################################################

        private static String ConvertAccelerometerDataIntoCSVString(IList<Tuple<TimeSpan, double, double, double>> accelerometerTuples)
        {
            StringBuilder stringbuilder = new StringBuilder();
            var enumerator = accelerometerTuples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var accelerometerTuple = enumerator.Current;
                stringbuilder.Append(String.Format(new CultureInfo("en-US"), "{0},{1:f3},{2:f3},{3:f3}\n",
                    accelerometerTuple.Item1.TotalMilliseconds, accelerometerTuple.Item2, accelerometerTuple.Item3, accelerometerTuple.Item4));
            }
            return stringbuilder.ToString();
        }

        //##################################################################################################################################
        //################################################## Convert Evaluation data #######################################################
        //##################################################################################################################################

        private static string ConvertEvaluationDataIntoCSVString(List<object[]> accelerometerEvaluationList)
        {
            StringBuilder stringbuilder = new StringBuilder();
            var enumerator = accelerometerEvaluationList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var currentAccelerometerEvaluation = enumerator.Current;
                stringbuilder.Append(String.Format(new CultureInfo("en-US"), "{0},{1:f3},{2:g}\n",
                    ((TimeSpan)currentAccelerometerEvaluation[0]).TotalMilliseconds, (double)currentAccelerometerEvaluation[1], (bool)currentAccelerometerEvaluation[2] ? 1:0));
            }
            return stringbuilder.ToString();
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
                    Debug.WriteLine("############## Current file size: '{0}' ################", textStream.Size);
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
