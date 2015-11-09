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
using Windows.Devices.Geolocation;
using Windows.System.Threading;

namespace BackgroundTask
{
    public sealed class TaskAction : IBackgroundTask
    {
        private Accelerometer _accelerometer;      
        private Gyrometer _gyrometer;
        private OrientationSensor _orientationSensor;
        private Geolocator _geolocator;
        private ThreadPoolTimer _periodicUpdateTimer;
        private uint _totalSteps = 0; 

        TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs> _accelerometerEventHandler;
        TypedEventHandler<Gyrometer, GyrometerReadingChangedEventArgs> _gyrometerEventHandler;
        TypedEventHandler<OrientationSensor, OrientationSensorReadingChangedEventArgs> _orientationSensorEventHandler;
        TypedEventHandler<Geolocator, PositionChangedEventArgs> _geolocationEventHandler;
    
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

            if (_taskArguments != null)
            {
                // init different sensors
                InitAccelerometer(_taskArguments);
                InitGyrometer(_taskArguments);
                InitOrientationSensor(_taskArguments);
                InitGeolocationSensor(_taskArguments);

                taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

                // create new measurement data model
                _measurementData = new MeasurementData(_taskArguments.Filename, _taskArguments.SampleBufferSize);
                _measurementData.ListsHasSwitched += MeasurementData_ReadingsListsHasSwitched;

                //set evaluation setting model
                _evaluationSettingModel = new EvaluationSettingModel(_taskArguments.SampleBufferSize, _taskArguments.AccelerometerThreshold, 
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
            await TaskFileService.AppendMeasurementDataToFileAsync(_taskArguments, _measurementData, true);
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
            if (_geolocator != null)
            {
                if (_periodicUpdateTimer != null)
                {
                    _periodicUpdateTimer.Cancel();
                }
                _geolocator.PositionChanged -= _geolocationEventHandler;
                _geolocator.ReportInterval = 0;
                _geolocator = null;
            }
        }

        //###########################################################################
        //########################### Accelerometer #################################
        //###########################################################################

        private void InitAccelerometer(TaskArguments taskArguments)
        {
            if (taskArguments.IsUsedAccelerometer) 
            { 
                _accelerometer = Accelerometer.GetDefault();
                if (_accelerometer != null)
                {
                    uint targetSensorReportInterval = taskArguments.ReportIntervalAccelerometer;
                    _accelerometer.ReportInterval = targetSensorReportInterval >= _accelerometer.MinimumReportInterval ? targetSensorReportInterval : _accelerometer.MinimumReportInterval; 
                    _accelerometerEventHandler = new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(AccelerometerReadingChanged);
                    _accelerometer.ReadingChanged += _accelerometerEventHandler;
                }
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

        private void InitGyrometer(TaskArguments taskArguments)
        {
            if (taskArguments.IsUsedGyrometer) 
            { 
                _gyrometer = Gyrometer.GetDefault();
                if (_gyrometer != null)
                {
                    uint targetSensorReportInterval = taskArguments.ReportIntervalGyrometer;
                    _gyrometer.ReportInterval = targetSensorReportInterval >= _gyrometer.MinimumReportInterval ? targetSensorReportInterval : _gyrometer.MinimumReportInterval;
                    _gyrometerEventHandler = new TypedEventHandler<Gyrometer, GyrometerReadingChangedEventArgs>(GyrometerReadingChanged);
                    _gyrometer.ReadingChanged += _gyrometerEventHandler;
                }
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

        private void InitOrientationSensor(TaskArguments taskArguments)
        {
            if (taskArguments.IsUsedQuaternion)
            {
                _orientationSensor = OrientationSensor.GetDefault();
                if (_orientationSensor != null)
                {
                    uint targetSensorReportInterval = taskArguments.ReportIntervalQuaternion;
                    _orientationSensor.ReportInterval = targetSensorReportInterval >= _orientationSensor.MinimumReportInterval ? targetSensorReportInterval : _orientationSensor.MinimumReportInterval;
                    _orientationSensorEventHandler = new TypedEventHandler<OrientationSensor, OrientationSensorReadingChangedEventArgs>(OrientationSensorReadingChanged);
                    _orientationSensor.ReadingChanged += _orientationSensorEventHandler;
                }
            }
        }

        private void OrientationSensorReadingChanged(OrientationSensor sender, OrientationSensorReadingChangedEventArgs args)
        {
            if (_measurementData != null)
                _measurementData.AddOrientationSensorReading(args.Reading);
        }

        //###########################################################################
        //########################### GeolocationSensor #############################
        //###########################################################################

        private void InitGeolocationSensor(TaskArguments taskArguments)
        {
            if (taskArguments.IsUsedGeolocation)
            {
                _geolocator = new Geolocator();
                if (_geolocator != null && _geolocator.LocationStatus != PositionStatus.Disabled)
                {
                    // force gps quality readings
                    _geolocator.DesiredAccuracy = PositionAccuracy.High;
                    _geolocator.MovementThreshold = 0;
                    //_geolocator.ReportInterval = taskArguments.ReportIntervalGeolocation;
                    //_geolocationEventHandler = new TypedEventHandler<Geolocator, PositionChangedEventArgs>(GeolocationPositionChanged);
                    //_geolocator.PositionChanged += _geolocationEventHandler;

                    TimeSpan period = TimeSpan.FromSeconds(taskArguments.ReportIntervalGeolocation);

                    _periodicUpdateTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                    {
                        Debug.WriteLine("Position read with status: {0}", _geolocator.LocationStatus);

                        if (_geolocator != null && _geolocator.LocationStatus != PositionStatus.Disabled)
                        {
                            Geoposition currentGeoposition = await _geolocator.GetGeopositionAsync();
                            if (currentGeoposition != null)
                            {
                                Debug.WriteLine("Current coordianates: {0}:{1}", 
                                    currentGeoposition.Coordinate.Point.Position.Latitude, currentGeoposition.Coordinate.Point.Position.Longitude);
                                _measurementData.AddGeolocationReading(currentGeoposition.Coordinate);
                            }
                        }
                        else if (_periodicUpdateTimer != null)
                        {
                            _periodicUpdateTimer.Cancel();
                        }
                    }, period);
                }
            }
        }

        private void GeolocationPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (_measurementData != null)
            {
                _measurementData.AddGeolocationReading(args.Position.Coordinate);
            }
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
                await TaskFileService.AppendMeasurementDataToFileAsync(_taskArguments, measurementData, false);
                await ProcessAnalysis(measurementData, false);
            }
            return;
        }

        private async Task ProcessAnalysis(MeasurementData measurementData, bool isActiveListChoosen)
        {
            if (_taskArguments.IsUsedEvaluation)
            {
                EvaluationDataModel evaluationDataModel = new EvaluationDataModel();
                if (isActiveListChoosen)
                {
                    evaluationDataModel.AddAllAccelerometerAnalysisFromSampleList(measurementData.GetActivAccelerometerList());
                    evaluationDataModel.AddAllGyrometerAnalysisFromSampleList(measurementData.GetActivGyrometerList());
                }
                else
                {
                    evaluationDataModel.AddAllAccelerometerAnalysisFromSampleList(measurementData.GetPassivAccelerometerList());
                    evaluationDataModel.AddAllGyrometerAnalysisFromSampleList(measurementData.GetPassivGyrometerList());
                }
                EvaluationResultModel evaluationResultModel = await _measurementEvaluationService.RunEvaluationDuringMeasurementAsync(evaluationDataModel, _evaluationSettingModel);
                _totalSteps += evaluationResultModel.DetectedSteps;
                await TaskFileService.AppendEvaluationDataToFileAsync(_taskArguments, evaluationResultModel);
                _taskInstance.Progress = _totalSteps;
            }
        }
    }
}
