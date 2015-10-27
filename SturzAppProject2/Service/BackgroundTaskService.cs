using BackgroundTask.DataModel;
using BackgroundTask.DataModel.Setting;
using BackgroundTask.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Sensors;

namespace BackgroundTask.Service
{
    public static class BackgroundTaskService
    {
        internal static void SynchronizeMeasurementsWithActiveBackgroundTasks(List<MeasurementModel> measurements)
        {
            List<String> pairedMeasurementIds = new List<string>();

            // Consider all started measurements of active background tasks.
            // All started measurements without active background tasks will aborted;
            foreach (MeasurementModel measurement in measurements)
            {
                if (measurement.MeasurementState == MeasurementState.Started)
                {
                    if (isBackgroundTaskRegistered(measurement.Id))
                    {
                        pairedMeasurementIds.Add(measurement.Id);
                    }
                    else
                    {
                        Debug.WriteLine("Measurement with Id '{0}' will be aborted.", measurement.Id);
                        measurement.EndTime = DateTime.Now;
                        measurement.MeasurementState = MeasurementState.Stopped;
                    }
                }
            }

            // All background tasks without started measurements will be deregistered.
            foreach (var currentTask in BackgroundTaskRegistration.AllTasks)
            {
                if (!pairedMeasurementIds.Contains(currentTask.Value.Name))
                {
                    Debug.WriteLine("Background Task with name '{0}' will be deregisterd.", currentTask.Value.Name);
                    DeregisterBackgroundTask(currentTask.Value.Name);
                }
            }
        }

        public static async Task<bool> StartBackgroundTaskForMeasurement(MeasurementModel measurement)
        {
            bool isStarted = false;
            if (measurement != null &&
                measurement.Id != null &&
                measurement.Id != String.Empty &&
                canRegisterBackgroundTask() &&
                measurement.Setting != null)
            {
                TaskArguments taskArguments = mapTo(measurement);
                string arguments = JsonConvert.SerializeObject(taskArguments);                
                if (await StartAccelerometerTask(measurement.Id, arguments))
                {
                    isStarted = true;    
                }    
            }
            return isStarted;
        }

        public static bool StopBackgroundTaskForMeasurement(MeasurementModel measurement)
        {
            bool isStopped = false;

            if (measurement != null &&
                measurement.Id != null &&
                measurement.Id.Length > 0)
            {
                isStopped = DeregisterBackgroundTask(measurement.Id);
            }
            return isStopped;
        }

        private static bool canRegisterBackgroundTask()
        {
            return BackgroundTaskRegistration.AllTasks.Count == 0 ? true : false;
        }

        private static bool isBackgroundTaskRegistered(string taskName)
        {
            Debug.WriteLine("Anzahl BackgroundTasks '{0}'", BackgroundTaskRegistration.AllTasks.Count);
            foreach (var currentTask in BackgroundTaskRegistration.AllTasks)
            {
                if (currentTask.Value.Name.Equals(taskName))
                {
                    return true;
                }
            }
            return false;
        }

        internal static void AttachToOnProgressEvent(string taskName, BackgroundTaskProgressEventHandler progressHandler)
        {
            if (taskName != null && taskName != String.Empty && progressHandler != null) 
            {
                foreach (var currentTask in BackgroundTaskRegistration.AllTasks)
                {
                    if (currentTask.Value.Name.Equals(taskName))
                    {
                        Debug.WriteLine("+++ Attach Eventlistner to the OnProgressEvent of the background task.");
                        currentTask.Value.Progress += progressHandler;
                    }
                }
            }
        }

        internal static void DetachToOnProgressEvent(string taskName, BackgroundTaskProgressEventHandler progressHandler)
        {
            if (taskName != null && taskName != String.Empty && progressHandler != null)
            {
                foreach (var currentTask in BackgroundTaskRegistration.AllTasks)
                {
                    if (currentTask.Value.Name.Equals(taskName))
                    {
                        Debug.WriteLine("--- Detach Eventlistner to the OnProgressEvent of the background task.");
                        currentTask.Value.Progress -= progressHandler;
                    }
                }
            }
        }

        private static TaskArguments mapTo(MeasurementModel measurementModel)
        {
            TaskArguments taskArguments = new TaskArguments();
            taskArguments.MeasurementId = measurementModel.Id;
            taskArguments.Filename = measurementModel.Filename;
            taskArguments.TargetDuration = measurementModel.Setting.TargetDuration;
            taskArguments.StartOffsetDuration = measurementModel.Setting.StartOffsetDuration;

            taskArguments.IsUsedEvaluation = measurementModel.Setting.IsUsedEvaluation;
            taskArguments.IsRecordSamplesEvaluation = measurementModel.Setting.IsRecordSamplesEvaluation;
            taskArguments.SampleBufferSize = measurementModel.Setting.SampleBufferSize;
            taskArguments.AccelerometerThreshold = measurementModel.Setting.AccelerometerThreshold;
            taskArguments.GyrometerThreshold = measurementModel.Setting.GyrometerThreshold;
            taskArguments.StepDistance = measurementModel.Setting.StepDistance;
            taskArguments.PeakJoinDistance = measurementModel.Setting.PeakJoinDistance;

            taskArguments.IsUsedAccelerometer = measurementModel.Setting.IsUsedAccelerometer;
            taskArguments.IsRecordSamplesAccelerometer = measurementModel.Setting.IsRecordSamplesAccelerometer;
            taskArguments.ReportIntervalAccelerometer = measurementModel.Setting.ReportIntervalAccelerometer;

            taskArguments.IsUsedGyrometer = measurementModel.Setting.IsUsedGyrometer;
            taskArguments.IsRecordSamplesGyrometer = measurementModel.Setting.IsRecordSamplesGyrometer;
            taskArguments.ReportIntervalGyrometer = measurementModel.Setting.ReportIntervalGyrometer;

            taskArguments.IsUsedQuaternion = measurementModel.Setting.IsUsedQuaternion;
            taskArguments.IsRecordSamplesQuaternion = measurementModel.Setting.IsRecordSamplesQuaternion;
            taskArguments.ReportIntervalQuaternion = measurementModel.Setting.ReportIntervalQuaternion;

            taskArguments.IsUsedGeolocation = measurementModel.Setting.IsUsedGeolocation;
            taskArguments.IsRecordSamplesGeolocation = measurementModel.Setting.IsRecordSamplesGeolocation;
            taskArguments.ReportIntervalGeolocation = measurementModel.Setting.ReportIntervalGeolocation;
            return taskArguments;
        }
        
        //#############################################################################
        //########################## Start Background Task ############################
        //#############################################################################

        #region Start BackgroundTask

        public static async Task<bool> StartAccelerometerTask(string taskName, string arguments)
        {
            bool isBackgroundTaskRegistered = false;

            Accelerometer accelerometer = Accelerometer.GetDefault();
            if (accelerometer != null && taskName != null && taskName.Length > 0)
            {
                BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

                if ((BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity.Equals(backgroundAccessStatus))
                    || (BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity.Equals(backgroundAccessStatus)))
                {
                    isBackgroundTaskRegistered = await RegisterAccelerometerTaskAsync(accelerometer.DeviceId, taskName, arguments);
                }
            }
            return isBackgroundTaskRegistered;
        }

        private static async Task<bool> RegisterAccelerometerTaskAsync(string deviceId, string taskName, string arguments)
        {
            String taskEntryPoint = "BackgroundTask.TaskAction";
            DeviceUseTrigger trigger = new DeviceUseTrigger();

            if (!isBackgroundTaskRegistered(taskName))
            {
                var builder = new BackgroundTaskBuilder();
                builder.Name = taskName;
                builder.TaskEntryPoint = taskEntryPoint;
                builder.SetTrigger(trigger);

                BackgroundTaskRegistration backgroundTaskregistration = builder.Register();

                if (!await RequestDeviceUseTriggerAsync(deviceId, trigger, taskName, arguments))
                {
                    DeregisterBackgroundTask(taskName);
                    return false;
                }
            }
            return true;
        }

        private static async Task<bool> RequestDeviceUseTriggerAsync(string deviceId, DeviceUseTrigger deviceUseTrigger, string taskName, string arguments)
        {
            try
            {
                DeviceTriggerResult deviceTriggerResult = await deviceUseTrigger.RequestAsync(deviceId, arguments);
                switch (deviceTriggerResult)
                {
                    case DeviceTriggerResult.Allowed:
                        //ShowNotifyMessage("Background Task wurde gestartet.", NotifyLevel.Info);
                        return true;
                    case DeviceTriggerResult.DeniedBySystem:
                        //ShowNotifyMessage("Background Task wurde vom System verweigert.", NotifyLevel.Warn);
                        break;
                    case DeviceTriggerResult.DeniedByUser:
                        //ShowNotifyMessage("Background Task wurde vom Nutzer verweigert.", NotifyLevel.Warn);
                        break;
                    case DeviceTriggerResult.LowBattery:
                        //ShowNotifyMessage("Background Task wurde wegen geringer Batterie verweigert.", NotifyLevel.Warn);
                        break;
                }
            }
            catch (InvalidOperationException)
            {
                DeregisterBackgroundTask(taskName);
            }
            return false;
        }

        #endregion

        //#############################################################################
        //########################## Stop Background Task #############################
        //#############################################################################

        #region Stop BackgroundTask

        public static bool DeregisterBackgroundTask(string taskName)
        {
            bool isDeregistered = false;
            if (taskName != null && taskName.Length > 0)
            {
                foreach (var currentTask in BackgroundTaskRegistration.AllTasks)
                {
                    if (currentTask.Value.Name.Equals(taskName))
                    {
                        currentTask.Value.Unregister(true);
                        isDeregistered = true;
                    }
                }
            }
            return isDeregistered;
        }

        #endregion

    }
}
