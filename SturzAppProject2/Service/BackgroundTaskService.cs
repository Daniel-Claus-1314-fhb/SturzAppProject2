using BackgroundTask.DataModel;
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
    public class BackgroundTaskService
    {

        internal void SynchronizeMeasurementsWithActiveBackgroundTasks(List<Measurement> measurements)
        {
            List<String> pairedMeasurementIds = new List<string>();

            // Consider all started measurements of active background tasks.
            // All started measurements without active background tasks will aborted;
            foreach (Measurement measurement in measurements)
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
                        measurement.MeasurementState = MeasurementState.Aborted;
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

        public bool StartBackgroundTaskForMeasurement(Measurement measurement)
        {
            bool isStarted = false;
            if (measurement != null &&
                measurement.Id != null &&
                measurement.Id.Length > 0 &&
                canRegisterBackgroundTask() &&
                measurement.Setting != null)
            {
                TaskArguments taskArguments = new TaskArguments(measurement.Id, measurement.AccelerometerFilename, measurement.GyrometerFilename,
                    measurement.Setting.ReportInterval, measurement.Setting.ProcessedSamplesCount);

                string arguments = JsonConvert.SerializeObject(taskArguments);
                if (measurement.Setting.UseAccelerometer)
                {
                    StartAccelerometerTask(measurement.Id, arguments);
                }
                isStarted = true;
            }
            return isStarted;
        }

        public bool StopBackgroundTaskForMeasurement(Measurement measurement)
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





        private bool canRegisterBackgroundTask()
        {
            return BackgroundTaskRegistration.AllTasks.Count == 0 ? true : false;
        }

        private bool isBackgroundTaskRegistered(string taskName)
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

        //#############################################################################
        //########################## Start Background Task ############################
        //#############################################################################

        #region Start BackgroundTask

        public async void StartAccelerometerTask(string taskName, string arguments)
        {
            Accelerometer accelerometer = Accelerometer.GetDefault();
            if (accelerometer != null && taskName != null && taskName.Length > 0)
            {
                BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

                if ((BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity.Equals(backgroundAccessStatus))
                    || (BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity.Equals(backgroundAccessStatus)))
                {
                    await RegisterAccelerometerTaskAsync(accelerometer.DeviceId, taskName, arguments);
                }
                else
                {
                    //ShowNotifyMessage("App darf keine Background Tasks starten.", NotifyLevel.Error);
                }
            }
        }

        private async Task<bool> RegisterAccelerometerTaskAsync(string deviceId, string taskName, string arguments)
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

        private async Task<bool> RequestDeviceUseTriggerAsync(string deviceId, DeviceUseTrigger deviceUseTrigger, string taskName, string arguments)
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

        public bool DeregisterBackgroundTask(string taskName)
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
                        //ShowNotifyMessage("Background Task wurde beendet.", NotifyLevel.Info);
                    }
                }
            }
            return isDeregistered;
        }

        #endregion

    }
}
