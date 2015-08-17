using BackgroundTask.DataModel;
using BackgroundTask.Service;
using Newtonsoft.Json;
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

        private AccelerometerData _accelerometerData;
            //string measurementId = String.Format("{0}_{1:yyyyMMdd}_{1:HHmmss}", _taskArguments.taskName, DateTime.Now);

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

                // CREATE NEW ACCELEROMETERDATA MODEL
                _accelerometerData = new AccelerometerData(_taskArguments.AccelerometerFilename, _taskArguments.ProcessedSampleCount);
                _accelerometerData.ReadingsListsHasSwitched += _accelerometerData_ReadingsListsHasSwitched;

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

        void _accelerometerData_ReadingsListsHasSwitched(object sender, EventArgs e)
        {
            Debug.WriteLine("############# ReadingsList has switched ############");
            if (sender.GetType().Equals(typeof(AccelerometerData)))
            {
                TaskFileService.AppendPassivAccelerometerReadingsToFileAsync((AccelerometerData) sender);
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
            _accelerometerData.ReadingsListsHasSwitched -= _accelerometerData_ReadingsListsHasSwitched;
            await TaskFileService.AppendActivAccelerometerReadingsToFileAsync(_accelerometerData);

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
            if (_accelerometerData != null)
            {
                _accelerometerData.AddAccelerometerReading(args.Reading);
            }
        }
    }
}
