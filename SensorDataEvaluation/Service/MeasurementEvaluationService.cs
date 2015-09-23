using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.Service
{
    public class MeasurementEvaluationService
    {
        private TimeSpan _lastKnownStep;

        public async Task<EvaluationResultModel> RunEvaluationDuringMeasurementAsync(EvaluationDataModel evaluationData, EvaluationSettingModel evaluationSetting)
        {
            return await Task.Run<EvaluationResultModel>(() =>
            {
                return Run(evaluationData, evaluationSetting);
            });
        }

        public async Task<EvaluationResultModel> RunEvaluationAfterMeasurementAsync(EvaluationDataModel evaluationData, EvaluationSettingModel evaluationSetting)
        {

            return await Task.Run<EvaluationResultModel>(() =>
            {
                _lastKnownStep = TimeSpan.Zero;
                return Run(evaluationData, evaluationSetting);
            });
        }

        private EvaluationResultModel Run(EvaluationDataModel evaluationData, EvaluationSettingModel evaluationSetting)
        {
            EvaluationResultModel evaluationResult = new EvaluationResultModel();
            AnalysisVectorLength(evaluationData, evaluationResult);
            DetectSteps(evaluationResult, evaluationSetting);
            return evaluationResult;
        }

        private void AnalysisVectorLength(EvaluationDataModel evaluationData, EvaluationResultModel evaluationResultModel)
        {
            var accelerometerList = evaluationData.AccelerometerSampleAnalysisList;

            if (accelerometerList != null && accelerometerList.Count > 1)
            {
                for (int i = 0; i < accelerometerList.Count; i++)
                {
                    // Is accelerometer value already analysed? && is accelerometer value not the first one && is accelerometer value not the last one
                    if (i > 0 && i < accelerometerList.Count - 1)
                    {
                        TimeSpan currentTimeSpan = (TimeSpan)accelerometerList.ElementAt(i).MeasurementTime;
                        double currentAccelerometerX = (double)accelerometerList.ElementAt(i).CoordinateX;
                        double currentAccelerometerY = (double)accelerometerList.ElementAt(i).CoordinateY;
                        double currentAccelerometerZ = (double)accelerometerList.ElementAt(i).CoordinateZ;
                        // In kartesischen Koordinaten kann die Länge von Vektoren nach dem Satz des Pythagoras berechnet werden.
                        double vectorLength = Math.Sqrt(Math.Pow(currentAccelerometerX, 2d) + Math.Pow(currentAccelerometerY, 2) + Math.Pow(currentAccelerometerZ, 2)) - 1;

                        evaluationResultModel.EvaluationResultList.Add(new EvaluationSample(currentTimeSpan, vectorLength));
                    }
                }
            }
        }

        private void DetectSteps(EvaluationResultModel evaluationResultModel, EvaluationSettingModel evaluationSetting)
        {
            double threshold = evaluationSetting.StepThreshold;
            TimeSpan stepTimeDistence = evaluationSetting.StepDistance;

            var accelEvaluationList = evaluationResultModel.EvaluationResultList;

            if (accelEvaluationList != null && accelEvaluationList.Count > 2)
            {
                // detect Steps
                for (int i = 0; i < accelEvaluationList.Count; i++)
                {
                    // is evaluation value not the first one && is evaluation value not the last one
                    if (i > 0 && i < accelEvaluationList.Count - 1)
                    {
                        TimeSpan currentMeasurementTime = accelEvaluationList.ElementAt(i).MeasurementTime;
                        double currentVectorLength = accelEvaluationList.ElementAt(i).AccelerometerVectorLength;
                        double previousVectorLength = accelEvaluationList.ElementAt(i - 1).AccelerometerVectorLength;
                        double nextVectorLength = accelEvaluationList.ElementAt(i + 1).AccelerometerVectorLength;

                        if (// find peak
                            (currentVectorLength - previousVectorLength) * (currentVectorLength - nextVectorLength) > 0d &&
                            // check threshold
                            (currentVectorLength.CompareTo(0d + threshold) > 0 || currentVectorLength.CompareTo(0d - threshold) < 0) &&
                            // check timespan between to detected steps
                            currentMeasurementTime.Subtract(_lastKnownStep) > stepTimeDistence)
                        {
                            // Set detected step
                            accelEvaluationList.ElementAt(i).IsStepDetected = true;
                            // set time of last known step
                            _lastKnownStep = currentMeasurementTime;
                        }
                    }
                }
            }
        }

        #region Deprecated

        //private void PreprocessEvaluationDataTuples(EvaluationDataModel evaluationData)
        //{
        //    var accelerometerList = evaluationData.AccelerometerAnalysisList;
        //    if (accelerometerList != null && accelerometerList.Count > 1)
        //    {
        //        for (int i = 0; i < accelerometerList.Count; i++)
        //        {
        //            // Is accelerometer value already analysed?
        //            if (!(bool)accelerometerList.ElementAt(i)[4] && i > 0)
        //            {
        //                // if the accelerometer tuple as not analysis yet, the low pass filter the values.
        //                // low pass filter algorithm:
        //                // On = On-1 + α(In – On-1); O = Output; α = coefficient between 0..1; I = Input; 
        //                accelerometerList.ElementAt(i)[1] = ((double)accelerometerList.ElementAt(i - 1)[1] + (0.8d * ((double)accelerometerList.ElementAt(i)[1] - (double)accelerometerList.ElementAt(i - 1)[1])));
        //                accelerometerList.ElementAt(i)[2] = ((double)accelerometerList.ElementAt(i - 1)[2] + (0.8d * ((double)accelerometerList.ElementAt(i)[2] - (double)accelerometerList.ElementAt(i - 1)[2])));
        //                accelerometerList.ElementAt(i)[3] = ((double)accelerometerList.ElementAt(i - 1)[3] + (0.8d * ((double)accelerometerList.ElementAt(i)[3] - (double)accelerometerList.ElementAt(i - 1)[3])));
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}
