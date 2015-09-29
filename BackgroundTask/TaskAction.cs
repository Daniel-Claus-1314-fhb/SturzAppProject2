using BackgroundTask.DataModel;
using SensorDataEvaluation.DataModel;
using BackgroundTask.Service;
using Newtonsoft.Json;
using SensorDataEvaluation.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Background;
using Windows.Devices.Sensors;
using Windows.Foundation;

namespace BackgroundTask
{
    public sealed class TaskAction : IBackgroundTask
    {
        private Accelerometer _accelerometer;      
        private Gyrometer _gyrometer;
        private OrientationSensor _orientationSensor;
        private uint _totalSteps = 0; 

        TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs> _accelerometerEventHandler;
        TypedEventHandler<Gyrometer, GyrometerReadingChangedEventArgs> _gyrometerEventHandler;
        TypedEventHandler<OrientationSensor, OrientationSensorReadingChangedEventArgs> _orientationSensorEventHandler;
    
        private IBackgroundTaskInstance _taskInstance;
        private BackgroundTaskDeferral _deferral;
        private TaskArguments _taskArguments;

        MeasurementEvaluationService _measurementEvaluationService = new MeasurementEvaluationService();

        private MeasurementData _measurementData;
        private EvaluationSettingModel _evaluationSettingModel;

        //###########################################################################
        //######################## Init MeasurementTask #############################
        //###########################################################################

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine(
                "####################################################\n" +
                "############### Run MeasurementTask ################\n" +
                "####################################################");

            _taskInstance = taskInstance;
            // GET TASKARGRUMENTS
            _taskArguments = GetTaskArgumentsFromTriggerDetails((DeviceUseDetails)_taskInstance.TriggerDetails);

            if (_taskArguments != null && 
                    _taskArguments.Filename != null && _taskArguments.Filename != String.Empty &&
                    _taskArguments.ReportInterval > 0 && _taskArguments.ProcessedSampleCount > 0)
            {
                // init different sensors
                InitAccelerometer(_taskArguments.ReportInterval);
                InitGyrometer(_taskArguments.ReportInterval);
                InitOrientationSensor(_taskArguments.ReportInterval);

                taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

                // create new measurement data model
                _measurementData = new MeasurementData(_taskArguments.Filename, _taskArguments.ProcessedSampleCount);
                _measurementData.ListsHasSwitched += MeasurementData_ReadingsListsHasSwitched;

                //set evaluation setting model
                _evaluationSettingModel = new EvaluationSettingModel(_taskArguments.ProcessedSampleCount, _taskArguments.AccelerometerThreshold, 
                    _taskArguments.GyrometerThreshold, _taskArguments.StepDistance, _taskArguments.PeakJoinDistance);

                _deferral = _taskInstance.GetDeferral();
                Debug.WriteLine(
                    "####################################################\n" +
                    "########## MeasurementTask initialisiert ###########\n" +
                    "####################################################");
            }
            else
            {
                Debug.WriteLine(
                    "####################################################\n" +
                    "############# MeasurementTask aborted ##############\n" +
                    "####################################################");
            }
        }

        private TaskArguments GetTaskArgumentsFromTriggerDetails(DeviceUseDetails details)
        {
            TaskArguments taskArguments = null;
            
            string argumentsJsonString = details.Arguments;
            if (argumentsJsonString != null && argumentsJsonString.Length > 0)
            {
                taskArguments = JsonConvert.DeserializeObject<TaskArguments>(argumentsJsonString);
            }
            return taskArguments;
        }

        //###########################################################################
        //######################## Cancel MeasurementTask ###########################
        //###########################################################################

        private async void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine(
                "####################################################\n" +
                "############### Stop MeasurementTask ###############\n" +
                "####################################################");

            // reset all used sensors
            CleanUpSensors();
            
            // detach readings lists has switched event
            _measurementData.ListsHasSwitched -= MeasurementData_ReadingsListsHasSwitched;

            // save the current measurement.
            await TaskFileService.AppendMeasurementDataToFileAsync(_taskArguments.Filename, _measurementData, true);
            // process step analysis
            await ProcessAnalysis(_measurementData, true);

            _deferral.Complete();
        }

        private void CleanUpSensors()
        {
            if (_accelerometer != null)
            {
                _accelerometer.ReadingChanged -= _accelerometerEventHandler;
                _accelerometer.ReportInterval = 0;
                _accelerometer = null;
            }
            if (_gyrometer != null)
            {
                _gyrometer.ReadingChanged -= _gyrometerEventHandler;
                _gyrometer.ReportInterval = 0;
                _gyrometer = null;
            }
            if (_orientationSensor != null)
            {
                _orientationSensor.ReadingChanged -= _orientationSensorEventHandler;
                _orientationSensor.ReportInterval = 0;
                _orientationSensor = null;
            }
        }

        //###########################################################################
        //########################### Accelerometer #################################
        //###########################################################################

        private void InitAccelerometer(uint targetSensorReportInterval)
        {
            _accelerometer = Accelerometer.GetDefault();
            if (_accelerometer != null)
            {
                _accelerometer.ReportInterval = targetSensorReportInterval >= _accelerometer.MinimumReportInterval ? targetSensorReportInterval : _accelerometer.MinimumReportInterval; 
                _accelerometerEventHandler = new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(AccelerometerReadingChanged);
                _accelerometer.ReadingChanged += _accelerometerEventHandler;
            }
        }

        private void AccelerometerReadingChanged(Accelerometer accelerometer, AccelerometerReadingChangedEventArgs args)
        {
            if (_measurementData != null)
                _measurementData.AddAccelerometerReading(args.Reading);
        }

        //###########################################################################
        //########################### Gyrometer #####################################
        //###########################################################################

        private void InitGyrometer(uint targetSensorReportInterval)
        {
            _gyrometer = Gyrometer.GetDefault();
            if (_gyrometer != null)
            {
                _gyrometer.ReportInterval = targetSensorReportInterval >= _gyrometer.MinimumReportInterval ? targetSensorReportInterval : _gyrometer.MinimumReportInterval;
                _gyrometerEventHandler = new TypedEventHandler<Gyrometer, GyrometerReadingChangedEventArgs>(GyrometerReadingChanged);
                _gyrometer.ReadingChanged += _gyrometerEventHandler;
            }
        }

        private void GyrometerReadingChanged(Gyrometer gyrometer, GyrometerReadingChangedEventArgs args)
        {
            if (_measurementData != null)
                _measurementData.AddGyrometerReading(args.Reading);
        }

        //###########################################################################
        //########################### OrientationSensor #############################
        //###########################################################################

        private void InitOrientationSensor(uint targetSensorReportInterval)
        {
            _orientationSensor = OrientationSensor.GetDefault();
            if (_orientationSensor != null)
            {
                _orientationSensor.ReportInterval = targetSensorReportInterval >= _orientationSensor.MinimumReportInterval ? targetSensorReportInterval : _orientationSensor.MinimumReportInterval;
                _orientationSensorEventHandler = new TypedEventHandler<OrientationSensor, OrientationSensorReadingChangedEventArgs>(OrientationSensorReadingChanged);
                _orientationSensor.ReadingChanged += _orientationSensorEventHandler;
            }
        }

        private void OrientationSensorReadingChanged(OrientationSensor sender, OrientationSensorReadingChangedEventArgs args)
        {
            if (_measurementData != null)
                _measurementData.AddOrientationSensorReading(args.Reading);
        }

        //###########################################################################
        //########################### Process MeasurementData #######################
        //###########################################################################

        internal async void MeasurementData_ReadingsListsHasSwitched(object sender, EventArgs e)
        {
            Debug.WriteLine("############# ReadingsList has switched ############");
            if (sender.GetType().Equals(typeof(MeasurementData)))
            {
                MeasurementData measurementData = sender as MeasurementData;
                await TaskFileService.AppendMeasurementDataToFileAsync(_taskArguments.Filename, measurementData, false);
                await ProcessAnalysis(measurementData, false);
            }
            return;
        }

        private async Task ProcessAnalysis(MeasurementData measurementData, bool isActiveListChoosen)
        {
            EvaluationDataModel evaluationDataModel = new EvaluationDataModel();
            if (isActiveListChoosen)
            {
                evaluationDataModel.AddAllAccelerometerAnalysisFromSampleList(measurementData.GetActivAccelerometerList());
                evaluationDataModel.AddAllGyrometerAnalysisFromSampleList(measurementData.GetActivGyrometerList());
                evaluationDataModel.AddAllQuaternionAnalysisFromSampleList(measurementData.GetActivQuaternionList());
            }
            else
            {
                evaluationDataModel.AddAllAccelerometerAnalysisFromSampleList(measurementData.GetPassivAccelerometerList());
                evaluationDataModel.AddAllGyrometerAnalysisFromSampleList(measurementData.GetPassivGyrometerList());
                evaluationDataModel.AddAllQuaternionAnalysisFromSampleList(measurementData.GetPassivQuaternionList());
            }
            EvaluationResultModel evaluationResultModel = await _measurementEvaluationService.RunEvaluationDuringMeasurementAsync(evaluationDataModel, _evaluationSettingModel);
            _totalSteps += evaluationResultModel.DetectedSteps;
            await TaskFileService.AppendEvaluationDataToFileAsync(_taskArguments.Filename, evaluationResultModel);
            _taskInstance.Progress = _totalSteps;
            return;
        }
    }
}
