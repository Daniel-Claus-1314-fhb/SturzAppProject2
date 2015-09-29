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
        private TimeSpan lastAssumedAcclerometerStepTime;
        private TimeSpan lastAssumedGyrometerStepTime;

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
                lastAssumedAcclerometerStepTime = TimeSpan.Zero;
                lastAssumedGyrometerStepTime = TimeSpan.Zero;
                return Run(evaluationData, evaluationSetting);
            });
        }

        private EvaluationResultModel Run(EvaluationDataModel evaluationData, EvaluationSettingModel evaluationSetting)
        {
            EvaluationResultModel evaluationResult = new EvaluationResultModel();
            AnalysisVectorLength(evaluationData, evaluationResult);
            AssumingAcceleromterSteps(evaluationResult, evaluationSetting);
            AssumingGyrometerSteps(evaluationResult, evaluationSetting);
            DetectSteps(evaluationResult, evaluationSetting);
            return evaluationResult;
        }

        private void AnalysisVectorLength(EvaluationDataModel evaluationData, EvaluationResultModel evaluationResultModel)
        {
            var accelerometerList = evaluationData.AccelerometerSampleAnalysisList;
            var gyrometerList = evaluationData.GyrometerSampleAnalysisList;

            if (accelerometerList != null && accelerometerList.Count > 1 &&
                gyrometerList != null && gyrometerList.Count > 1)
            {
                int minListsCount = Math.Min(accelerometerList.Count, gyrometerList.Count);

                for (int i = 0; i < minListsCount; i++)
                {
                    // Vectorlength Accelerometer
                    TimeSpan currentTimeSpan = accelerometerList.ElementAt(i).MeasurementTime;
                    double currentAccelerometerX = accelerometerList.ElementAt(i).CoordinateX;
                    double currentAccelerometerY = accelerometerList.ElementAt(i).CoordinateY;
                    double currentAccelerometerZ = accelerometerList.ElementAt(i).CoordinateZ;
                    // In kartesischen Koordinaten kann die Länge von Vektoren nach dem Satz des Pythagoras berechnet werden.
                    double accelerometerVectorLength = Math.Sqrt(Math.Pow(currentAccelerometerX, 2d) + Math.Pow(currentAccelerometerY, 2) + Math.Pow(currentAccelerometerZ, 2));

                    // Vectorlength Gyrometer
                    double currentGyrometerVelocityX = gyrometerList.ElementAt(i).VelocityX;
                    double currentGyrometerVelocityY = gyrometerList.ElementAt(i).VelocityY;
                    double currentGyrometerVelocityZ = gyrometerList.ElementAt(i).VelocityZ;
                    // In kartesischen Koordinaten kann die Länge von Vektoren nach dem Satz des Pythagoras berechnet werden.
                    double gyrometerVectorLength = Math.Sqrt(Math.Pow(currentGyrometerVelocityX, 2d) + Math.Pow(currentGyrometerVelocityY, 2) + Math.Pow(currentGyrometerVelocityZ, 2));

                    evaluationResultModel.EvaluationResultList.Add(new EvaluationSample(currentTimeSpan, accelerometerVectorLength, gyrometerVectorLength));
                }
            }
        }

        private void AssumingAcceleromterSteps(EvaluationResultModel evaluationResultModel, EvaluationSettingModel evaluationSetting)
        {
            double accelerometerThreshold = evaluationSetting.AccelermeterVectorLengthThreshold;
            var evaluationList = evaluationResultModel.EvaluationResultList;

            if (evaluationList != null && evaluationList.Count > 2)
            {
                // detect Steps
                for (int i = 1; i < evaluationList.Count - 1; i++)
                {
                    double twicePreviousAccelerometerVectorLength = evaluationList.ElementAt(i).AccelerometerVectorLength;
                    double previousAccelerometerVectorLength = evaluationList.ElementAt(i).AccelerometerVectorLength;
                    double currentAccelerometerVectorLength = evaluationList.ElementAt(i).AccelerometerVectorLength;
                    double nextAccelerometerVectorLength = evaluationList.ElementAt(i).AccelerometerVectorLength;
                    double twiceNextAccelerometerVectorLength = evaluationList.ElementAt(i).AccelerometerVectorLength;

                    // is evaluation value not the first one && is evaluation value not the last one
                    if (i > 0)
                    {
                        previousAccelerometerVectorLength = evaluationList.ElementAt(i - 1).AccelerometerVectorLength;
                    }
                    if (i > 1)
                    {
                        twicePreviousAccelerometerVectorLength = evaluationList.ElementAt(i - 2).AccelerometerVectorLength;
	                }

                    if (i < evaluationList.Count - 1)
                    {
                        nextAccelerometerVectorLength = evaluationList.ElementAt(i + 1).AccelerometerVectorLength;
                    }
                    if (i < evaluationList.Count - 2)
                    {
                        twiceNextAccelerometerVectorLength = evaluationList.ElementAt(i + 2).AccelerometerVectorLength;
                    }

                    // Assume accelerometer step
                    if (// find peak
                        (currentAccelerometerVectorLength - previousAccelerometerVectorLength) * (currentAccelerometerVectorLength - nextAccelerometerVectorLength) > 0d &&
                        (currentAccelerometerVectorLength - twicePreviousAccelerometerVectorLength) * (currentAccelerometerVectorLength - twiceNextAccelerometerVectorLength) > 0d &&
                        // check threshold
                        currentAccelerometerVectorLength.CompareTo(accelerometerThreshold) > 0)
                    {
                        // Set assumed step
                        evaluationList.ElementAt(i).IsAssumedAccelerometerStep = true;
                    }
                }
            }
        }

        private void AssumingGyrometerSteps(EvaluationResultModel evaluationResultModel, EvaluationSettingModel evaluationSetting)
        {
            double gyrometerThreshold = evaluationSetting.GyrometerVectorLengthThreshold;
            var evaluationList = evaluationResultModel.EvaluationResultList;

            if (evaluationList != null && evaluationList.Count > 2)
            {
                // detect Steps
                for (int i = 1; i < evaluationList.Count - 1; i++)
                {
                    double twicePreviousGyrometerVectorLength = evaluationList.ElementAt(i).GyrometerVectorLength;
                    double previousGyrometerVectorLength = evaluationList.ElementAt(i).GyrometerVectorLength;
                    double currentGyrometerVectorLength = evaluationList.ElementAt(i).GyrometerVectorLength;
                    double nextGyrometerVectorLength = evaluationList.ElementAt(i).GyrometerVectorLength;
                    double twiceNextGyrometerVectorLength = evaluationList.ElementAt(i).GyrometerVectorLength;

                    // is evaluation value not the first one && is evaluation value not the last one
                    if (i > 0)
                    {
                        previousGyrometerVectorLength = evaluationList.ElementAt(i - 1).GyrometerVectorLength;
                    }
                    if (i > 1)
                    {
                        twicePreviousGyrometerVectorLength = evaluationList.ElementAt(i - 2).GyrometerVectorLength;
                    }
                    if (i < evaluationList.Count - 1)
                    {
                        nextGyrometerVectorLength = evaluationList.ElementAt(i + 1).GyrometerVectorLength;
                    }
                    if (i < evaluationList.Count - 2)
                    {
                        twiceNextGyrometerVectorLength = evaluationList.ElementAt(i + 2).GyrometerVectorLength;
                    }

                    // Assume gyrometer step
                    if (// find peak
                        (currentGyrometerVectorLength - previousGyrometerVectorLength) * (currentGyrometerVectorLength - nextGyrometerVectorLength) > 0d &&
                        (currentGyrometerVectorLength - twicePreviousGyrometerVectorLength) * (currentGyrometerVectorLength - twiceNextGyrometerVectorLength) > 0d &&
                        // check threshold
                        currentGyrometerVectorLength.CompareTo(gyrometerThreshold) > 0)
                    {
                        // Set assumed step
                        evaluationList.ElementAt(i).IsAssumedGyrometerStep = true;
                    }
                }
            }
        }

        private void DetectSteps(EvaluationResultModel evaluationResultModel, EvaluationSettingModel evaluationSetting)
        {
            TimeSpan stepTimeDistence = evaluationSetting.StepDistance;
            TimeSpan assumedStepsPairingThreshold = evaluationSetting.PeakJoinDistance;
            
            var evaluationList = evaluationResultModel.EvaluationResultList;

            if (evaluationList != null && evaluationList.Count > 0)
            {
                // detect Steps
                for (int i = 0; i < evaluationList.Count; i++)
                {
                    // check timespan between to detected steps
                    if (evaluationList.ElementAt(i).MeasurementTime.Subtract(_lastKnownStep) > stepTimeDistence)
                    {
                        // Set assumed accelerometer step
                        if (evaluationList.ElementAt(i).IsAssumedAccelerometerStep)
                        {
                            lastAssumedAcclerometerStepTime = evaluationList.ElementAt(i).MeasurementTime;
                        }

                        // Set assumed gyrometer step
                        if (evaluationList.ElementAt(i).IsAssumedGyrometerStep)
                        {
                            lastAssumedGyrometerStepTime = evaluationList.ElementAt(i).MeasurementTime;
                        }

                        // Vergleichen der zuletzt gefundenen peaks
                        if (lastAssumedAcclerometerStepTime != TimeSpan.Zero &&
                            lastAssumedGyrometerStepTime != TimeSpan.Zero)
                        {
                            //Tritt ein, wenn der zuletzt vermutete Accelerometerschritt nach dem zuletzt vermuteten Gyrometerschritt liegt und kleiner als der Schwellwert ist.
                            //oder
                            //Tritt ein, wenn der zuletzt vermutete Acceleroemeterschritt vor dem zuletzt vermuteten Gyrometerschritt liegt und kleiner als der Schwellwert ist.
                            if ((lastAssumedAcclerometerStepTime.CompareTo(lastAssumedGyrometerStepTime) >= 0 &&
                                lastAssumedAcclerometerStepTime.Subtract(lastAssumedGyrometerStepTime) < assumedStepsPairingThreshold)
                                ||
                                (lastAssumedAcclerometerStepTime.CompareTo(lastAssumedGyrometerStepTime) < 0 &&
                                    lastAssumedGyrometerStepTime.Subtract(lastAssumedAcclerometerStepTime) < assumedStepsPairingThreshold))
                            {
                                evaluationList.ElementAt(i).IsDetectedStep = true;
                                _lastKnownStep = evaluationList.ElementAt(i).MeasurementTime;
                                lastAssumedGyrometerStepTime = TimeSpan.Zero;
                                lastAssumedAcclerometerStepTime = TimeSpan.Zero;
                            }

                            // Tritt ein, wenn der zuletzt vermutete Acceleroemeterschritt nach dem zuletzt vermuteten Gyrometerschritt liegt und größer als der Schwellwert ist.
                            else if (lastAssumedAcclerometerStepTime.CompareTo(lastAssumedGyrometerStepTime) > 0 &&
                                    lastAssumedAcclerometerStepTime.Subtract(lastAssumedGyrometerStepTime) > assumedStepsPairingThreshold)
                            {
                                // Setzt den vermuteten Gyrometerschritt auf null zurück, weil der pairing Schwellwert überschritten wurde.
                                lastAssumedGyrometerStepTime = TimeSpan.Zero;
                            }

                            //Tritt ein, wenn der zuletzt vermutete Acceleroemeterschritt vor dem zuletzt vermuteten Gyrometerschritt liegt und größer als der Schwellwert ist.
                            else if (lastAssumedAcclerometerStepTime.CompareTo(lastAssumedGyrometerStepTime) < 0 &&
                                    lastAssumedGyrometerStepTime.Subtract(lastAssumedAcclerometerStepTime) < assumedStepsPairingThreshold)
                            {
                                // Setzt den vermuteten Accelerometerschritt auf null zurück, weil der pairing Schwellwert überschritten wurde.
                                lastAssumedAcclerometerStepTime = TimeSpan.Zero;
                            }
                        }
                    }
                }
            }
        }
    }
}
