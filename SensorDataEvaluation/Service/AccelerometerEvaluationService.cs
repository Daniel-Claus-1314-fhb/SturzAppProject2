using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.Service
{
    public static class AccelerometerEvaluationService
    {
        private static TimeSpan _lastKnownStep;

        public static void ProcessAnalysis(AccelerometerEvaluation accelerometerEvaluationModel)
        {
            PreprocessAccelerometerTuples(accelerometerEvaluationModel);
            AnalysisVectorLength(accelerometerEvaluationModel);
            DetectSteps(accelerometerEvaluationModel);
        }

        private static void PreprocessAccelerometerTuples(AccelerometerEvaluation accelerometerEvaluationModel)
        {
            var accelAnalysisList = accelerometerEvaluationModel.AccelerometerAnalysisList;

            if (accelAnalysisList != null && accelAnalysisList.Count > 1)
            {
                for (int i = 0; i < accelAnalysisList.Count; i++)
                {
                    // Is accelerometer value already analysed?
                    if (!(bool)accelAnalysisList.ElementAt(i)[4] && i > 0)
                    {
                        // if the accelerometer tuple as not analysis yet, the low pass filter the values.
                        // low pass filter algorithm:
                        // On = On-1 + α(In – On-1); O = Output; α = coefficient between 0..1; I = Input; 
                        accelAnalysisList.ElementAt(i)[1] = ((double)accelAnalysisList.ElementAt(i - 1)[1] + (0.8d * ((double)accelAnalysisList.ElementAt(i)[1] - (double)accelAnalysisList.ElementAt(i - 1)[1])));
                        accelAnalysisList.ElementAt(i)[2] = ((double)accelAnalysisList.ElementAt(i - 1)[2] + (0.8d * ((double)accelAnalysisList.ElementAt(i)[2] - (double)accelAnalysisList.ElementAt(i - 1)[2])));
                        accelAnalysisList.ElementAt(i)[3] = ((double)accelAnalysisList.ElementAt(i - 1)[3] + (0.8d * ((double)accelAnalysisList.ElementAt(i)[3] - (double)accelAnalysisList.ElementAt(i - 1)[3])));
                    }
                }
            }
        }

        private static void AnalysisVectorLength(AccelerometerEvaluation accelerometerEvaluationModel)
        {
            var accelAnalysisList = accelerometerEvaluationModel.AccelerometerAnalysisList;

            if (accelAnalysisList != null && accelAnalysisList.Count > 1)
            {
                for (int i = 0; i < accelAnalysisList.Count; i++)
                {
                    // Is accelerometer value already analysed? && is accelerometer value not the first one && is accelerometer value not the last one
                    if (!(bool)accelAnalysisList.ElementAt(i)[4] && i > 0 && i < accelAnalysisList.Count - 1)
                    {
                        TimeSpan currentTimeSpan = (TimeSpan)accelAnalysisList.ElementAt(i)[0];
                        double currentAccelerometerX = (double)accelAnalysisList.ElementAt(i)[1];
                        double currentAccelerometerY = (double)accelAnalysisList.ElementAt(i)[2];
                        double currentAccelerometerZ = (double)accelAnalysisList.ElementAt(i)[3];
                        // In kartesischen Koordinaten kann die Länge von Vektoren nach dem Satz des Pythagoras berechnet werden.
                        double vectorLength = Math.Sqrt(Math.Pow(currentAccelerometerX, 2d) + Math.Pow(currentAccelerometerY, 2) + Math.Pow(currentAccelerometerZ, 2)) - 1;

                        accelerometerEvaluationModel.AccelerometerEvaluationList.Add(new object[3] { currentTimeSpan, vectorLength, false});
                    }
                }
            }
        }

        private static void DetectSteps(AccelerometerEvaluation accelerometerEvaluationModel)
        {
            double threshold = accelerometerEvaluationModel.StepThreshold;
            TimeSpan stepTimeDistence = accelerometerEvaluationModel.StepDistance;
            var accelEvaluationList = accelerometerEvaluationModel.AccelerometerEvaluationList;

            if (accelEvaluationList != null && accelEvaluationList.Count > 2)
            {
                // detect Steps
                for (int i = 0; i < accelEvaluationList.Count; i++)
                {
                    // is evaluation value not the first one && is evaluation value not the last one
                    if (i > 0 && i < accelEvaluationList.Count - 1)
                    { 
                        //if(((data[i] - data[i-1]) * (data[i] - data[i+1])) > 0 ) 
                        if (((double)accelEvaluationList.ElementAt(i)[1] - (double)accelEvaluationList.ElementAt(i - 1)[1]) * ((double)accelEvaluationList.ElementAt(i)[1] - (double)accelEvaluationList.ElementAt(i + 1)[1]) > 0d &&
                            (((double)accelEvaluationList.ElementAt(i)[1]).CompareTo(0d + threshold) > 0 || ((double)accelEvaluationList.ElementAt(i)[1]).CompareTo(0d - threshold) < 0) &&
                            ((TimeSpan)accelEvaluationList.ElementAt(i)[0]).Subtract(_lastKnownStep) > stepTimeDistence)
                        {
                            accelEvaluationList.ElementAt(i)[2] = true;
                            _lastKnownStep = (TimeSpan)accelEvaluationList.ElementAt(i)[0];
                        }
                    }
                }
            }
        }
    }
}
