using BackgroundTask.DataModel;
using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BackgroundTask.Service
{
    internal static class GraphService
    {

        #region Load OxyplotData

        internal static async Task<OxyplotData> LoadGraphDataAsync(string filename)
        {
            OxyplotData oxyplotData = new OxyplotData();
            if (filename != null & filename != String.Empty)
            {
                Task<List<AccelerometerSample>> loadAccelerometerTask = FileService.LoadAccelerometerSamplesFromFileAsync(filename);
                Task<List<GyrometerSample>> loadGyrometerTask = FileService.LoadGyrometerSamplesFromFileAsync(filename);
                Task<List<QuaternionSample>> loadQuaternionTask = FileService.LoadQuaternionSamplesFromFileAsync(filename);
                Task<List<EvaluationSample>> loadEvaluationSamplesTask = FileService.LoadEvaluationSamplesFromFileAsync(filename);

                oxyplotData.AccelerometerSamples = await loadAccelerometerTask;
                oxyplotData.GyrometerSamples = await loadGyrometerTask;
                oxyplotData.QuaternionSamples = await loadQuaternionTask;
                oxyplotData.EvaluationSamples = await loadEvaluationSamplesTask;
            }
            return oxyplotData;
        }

        #endregion
    }
}
