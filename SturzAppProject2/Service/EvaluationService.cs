using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BackgroundTask.Service
{
    internal static class EvaluationService
    {
        //##################################################################################################################################
        //################################################## Save Evaluation data ##########################################################
        //##################################################################################################################################

        internal static async Task SaveEvaluationDataToFileAsync(String filename, EvaluationResultModel evaluationResultModel)
        {
            if (filename != null && filename != String.Empty && evaluationResultModel.EvaluationResultList.Count > 0)
            {
                // convert data into byte array
                byte[] bytes = evaluationResultModel.ToEvaluationBytes();
                if (bytes != null && bytes.Length > 0)
                {
                    // find folder
                    StorageFolder folder = await FileService.FindStorageFolder(FileService.GetEvaluationPath());
                    // delete old evaluationData
                    await FileService.DeleteFileAsync(folder, filename);
                    // save byte array
                    await FileService.SaveBytesToEndOfFileAsync(bytes, folder, filename);
                }
            }
            return;
        }

        //##################################################################################################################################
        //################################################## Load Evaluation Data ##########################################################
        //##################################################################################################################################

        #region Load Evaluation Data

        internal static async Task<EvaluationDataModel> LoadSamplesForEvaluationAsync(string filename)
        {
            EvaluationDataModel evaluationData = new EvaluationDataModel();

            if (filename != null && filename != string.Empty)
            {
                Task<List<AccelerometerSample>> loadAccelerometerTask = FileService.LoadAccelerometerSamplesFromFileAsync(filename);
                Task<List<GyrometerSample>> loadGyrometerTask = FileService.LoadGyrometerSamplesFromFileAsync(filename);

                evaluationData.AddAllAccelerometerAnalysisFromSampleList(await loadAccelerometerTask);
                evaluationData.AddAllGyrometerAnalysisFromSampleList(await loadGyrometerTask);
            }
            return evaluationData;
        }

        #endregion

    }
}
