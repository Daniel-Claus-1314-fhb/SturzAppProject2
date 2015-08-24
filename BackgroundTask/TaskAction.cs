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
        private IBackgroundTaskInstance _taskInstance;
        private BackgroundTaskDeferral _deferral;
        private TaskArguments _taskArguments;

        private AccelerometerData _accelerometerDataModel;
        private AccelerometerEvaluation _accelerometerEvaluationModel;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine(
                "####################################################\n" +
                "############### Run AccelerometerTask ##############\n" +
                "####################################################");
            _taskInstance = taskInstance;

            // GET TASKARGRUMENTS
            _taskArguments = getTaskArgumentsFromTriggerDetails((DeviceUseDetails)_taskInstance.TriggerDetails);

            if (_taskArguments != null && 
                _taskArguments.AccelerometerFilename != null &&
                _taskArguments.AccelerometerFilename.Length > 0 &&
                _taskArguments.ReportInterval > 0 && 
                _taskArguments.ProcessedSampleCount > 0)
            {
                _deferral = _taskInstance.GetDeferral();
                InitAccelerometer(_taskArguments.ReportInterval);
                taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

                // create new accelerometer data model
                _accelerometerDataModel = new AccelerometerData(_taskArguments.AccelerometerFilename, _taskArguments.ProcessedSampleCount);
                _accelerometerDataModel.TupleListsHasSwitched += AccelerometerData_ReadingsListsHasSwitched;

                //create new accelerometer evaluation model
                _accelerometerEvaluationModel = new AccelerometerEvaluation(_taskArguments.AccelerometerFilename, _taskArguments.ProcessedSampleCount, 
                    _taskArguments.PeakThreshold, _taskArguments.StepDistance);

                Debug.WriteLine(
                    "####################################################\n" +
                    "########## AccelerometerTask initialisiert #########\n" +
                    "####################################################");
            }
            else
            {
                Debug.WriteLine(
                    "####################################################\n" +
                    "############# AccelerometerTask aborted ############\n" +
                    "####################################################");
            }
        }

        private TaskArguments getTaskArgumentsFromTriggerDetails(DeviceUseDetails details)
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
        //######################## Cancel AccelerometerTask #########################
        //###########################################################################


        private async void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine(
                "####################################################\n" +
                "############### Stop AccelerometerTask #############\n" +
                "####################################################");
            // clean up accelerometer
            if (_accelerometer != null)
            {
                _accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
                _accelerometer.ReportInterval = 0;
                _accelerometer = null;
            }

            // save the current measurement.
            _accelerometerDataModel.TupleListsHasSwitched -= AccelerometerData_ReadingsListsHasSwitched;
            await TaskFileService.AppendActivAccelerometerDataToFileAsync(_accelerometerDataModel);
            await ProcessAnalysis(_accelerometerDataModel.GetActivTupleList());
            _taskInstance.Progress = this._accelerometerEvaluationModel.TotalSteps;

            _deferral.Complete();
        }

        //###########################################################################
        //########################### Accelerometer Init ############################
        //###########################################################################

        private void InitAccelerometer(uint sensorReportInterval)
        {
            _accelerometer = Accelerometer.GetDefault();
            if (_accelerometer != null)
            {
                uint minSensorReportInterval = _accelerometer.MinimumReportInterval;
                uint customReportInterval = 0;

                if (sensorReportInterval > 0)
                {
                    customReportInterval = sensorReportInterval;
                }
                _accelerometer.ReportInterval = customReportInterval > minSensorReportInterval ? customReportInterval : minSensorReportInterval;
                _accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
            }
        }

        private void ReadingChanged(Accelerometer accelerometer, AccelerometerReadingChangedEventArgs args)
        {
            if (_accelerometerDataModel != null)
            {
                _accelerometerDataModel.AddAccelerometerReading(args.Reading);
            }
        }

        //###########################################################################
        //########################### Process AccelerometerData #####################
        //###########################################################################

        internal async void AccelerometerData_ReadingsListsHasSwitched(object sender, EventArgs e)
        {
            Debug.WriteLine("############# ReadingsList has switched ############");
            if (sender.GetType().Equals(typeof(AccelerometerData)))
            {
                AccelerometerData accelerometerData = sender as AccelerometerData;

                TaskFileService.AppendPassivAccelerometerDataToFileAsync(accelerometerData);
                await ProcessAnalysis(accelerometerData.GetPassivTupleList());

                _taskInstance.Progress = this._accelerometerEvaluationModel.TotalSteps;
            }
        }

        internal async Task ProcessAnalysis(List<Tuple<TimeSpan, double, double, double>> accelerometerTuples)
        {
            _accelerometerEvaluationModel.AddAccelerometerDataForAnalysis(accelerometerTuples);
            AccelerometerEvaluationService.ProcessAnalysis(_accelerometerEvaluationModel);
            await TaskFileService.AppendEvaluationDataToFileAsync(_accelerometerEvaluationModel);
        }
    }
}
