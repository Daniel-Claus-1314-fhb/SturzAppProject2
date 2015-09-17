using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.Service
{
    public static class MeasurementEvaluationService
    {
        private static TimeSpan _lastKnownStep;

        public static EvaluationResultModel Run(EvaluationDataModel evaluationData, EvaluationSettingModel evaluationSetting)
        {
            EvaluationResultModel evaluationResult = new EvaluationResultModel();
            PreprocessEvaluationDataTuples(evaluationData);
            AnalysisVectorLength(evaluationData, evaluationResult);
            DetectSteps(evaluationResult, evaluationSetting);
            return evaluationResult;
        }

        private static void PreprocessEvaluationDataTuples(EvaluationDataModel evaluationData)
        {
            var accelerometerList = evaluationData.AccelerometerAnalysisList;
            if (accelerometerList != null && accelerometerList.Count > 1)
            {
                for (int i = 0; i < accelerometerList.Count; i++)
                {
                    // Is accelerometer value already analysed?
                    if (!(bool)accelerometerList.ElementAt(i)[4] && i > 0)
                    {
                        // if the accelerometer tuple as not analysis yet, the low pass filter the values.
                        // low pass filter algorithm:
                        // On = On-1 + α(In – On-1); O = Output; α = coefficient between 0..1; I = Input; 
                        accelerometerList.ElementAt(i)[1] = ((double)accelerometerList.ElementAt(i - 1)[1] + (0.8d * ((double)accelerometerList.ElementAt(i)[1] - (double)accelerometerList.ElementAt(i - 1)[1])));
                        accelerometerList.ElementAt(i)[2] = ((double)accelerometerList.ElementAt(i - 1)[2] + (0.8d * ((double)accelerometerList.ElementAt(i)[2] - (double)accelerometerList.ElementAt(i - 1)[2])));
                        accelerometerList.ElementAt(i)[3] = ((double)accelerometerList.ElementAt(i - 1)[3] + (0.8d * ((double)accelerometerList.ElementAt(i)[3] - (double)accelerometerList.ElementAt(i - 1)[3])));
                    }
                }
            }

            var gyrometerList = evaluationData.GyrometerAnalysisList;
            if (gyrometerList != null && gyrometerList.Count > 1)
            {
                for (int i = 0; i < gyrometerList.Count; i++)
                {
                    // Is gyrometer value already analysed?
                    if (!(bool)gyrometerList.ElementAt(i)[4] && i > 0)
                    {
                        // if the accelerometer tuple as not analysis yet, the low pass filter the values.
                        // low pass filter algorithm:
                        // On = On-1 + α(In – On-1); O = Output; α = coefficient between 0..1; I = Input; 
                        gyrometerList.ElementAt(i)[1] = ((double)gyrometerList.ElementAt(i - 1)[1] + (0.8d * ((double)gyrometerList.ElementAt(i)[1] - (double)gyrometerList.ElementAt(i - 1)[1])));
                        gyrometerList.ElementAt(i)[2] = ((double)gyrometerList.ElementAt(i - 1)[2] + (0.8d * ((double)gyrometerList.ElementAt(i)[2] - (double)gyrometerList.ElementAt(i - 1)[2])));
                        gyrometerList.ElementAt(i)[3] = ((double)gyrometerList.ElementAt(i - 1)[3] + (0.8d * ((double)gyrometerList.ElementAt(i)[3] - (double)gyrometerList.ElementAt(i - 1)[3])));
                    }
                }
            }
            // CRITICAL!!! Do not try to low pass filter quaternion values, because "sqrt(w^2 + x^2 + y^2 + z^2) = 1".
        }

        private static void AnalysisVectorLength(EvaluationDataModel evaluationData, EvaluationResultModel evaluationResultModel)
        {
            var accelerometerList = evaluationData.AccelerometerAnalysisList;

            if (accelerometerList != null && accelerometerList.Count > 1)
            {
                for (int i = 0; i < accelerometerList.Count; i++)
                {
                    // Is accelerometer value already analysed? && is accelerometer value not the first one && is accelerometer value not the last one
                    if (!(bool)accelerometerList.ElementAt(i)[4] && i > 0 && i < accelerometerList.Count - 1)
                    {
                        TimeSpan currentTimeSpan = (TimeSpan)accelerometerList.ElementAt(i)[0];
                        double currentAccelerometerX = (double)accelerometerList.ElementAt(i)[1];
                        double currentAccelerometerY = (double)accelerometerList.ElementAt(i)[2];
                        double currentAccelerometerZ = (double)accelerometerList.ElementAt(i)[3];
                        // In kartesischen Koordinaten kann die Länge von Vektoren nach dem Satz des Pythagoras berechnet werden.
                        double vectorLength = Math.Sqrt(Math.Pow(currentAccelerometerX, 2d) + Math.Pow(currentAccelerometerY, 2) + Math.Pow(currentAccelerometerZ, 2)) - 1;

                        evaluationResultModel.EvaluationResultList.Add(new object[3] { currentTimeSpan, vectorLength, false });
                    }
                }
            }
        }

        private static void DetectSteps(EvaluationResultModel evaluationResultModel, EvaluationSettingModel evaluationSetting)
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
                        if (// find peak
                            ((double)accelEvaluationList.ElementAt(i)[1] - (double)accelEvaluationList.ElementAtOrDefault(i - 1)[1]) * ((double)accelEvaluationList.ElementAt(i)[1] - (double)accelEvaluationList.ElementAt(i + 1)[1]) > 0d &&
                            // check threshold
                            (((double)accelEvaluationList.ElementAt(i)[1]).CompareTo(0d + threshold) > 0 || ((double)accelEvaluationList.ElementAt(i)[1]).CompareTo(0d - threshold) < 0) &&
                            // check timespan between to detected steps
                            ((TimeSpan)accelEvaluationList.ElementAt(i)[0]).Subtract(_lastKnownStep) > stepTimeDistence)
                        {
                            // Set detected step
                            accelEvaluationList.ElementAt(i)[2] = true;
                            // set time of last known step
                            _lastKnownStep = (TimeSpan)accelEvaluationList.ElementAt(i)[0];
                        }
                    }
                }
            }
        }
    }
}
