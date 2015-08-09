using BackgroundTask.DataModel;
using BackgroundTask.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Sensors;

namespace BackgroundTask.Service
{
    public class BackgroundTaskService
    {
        private BackgroundTaskRegistration _backgroundTaskRegistration;


        public bool StartBackgroundTaskForMeasurement(Measurement measurement) 
        {
            bool isStarted = false;

            return isStarted;
        }

        public bool StopBackgroundTaskForMeasurement(Measurement measurement)
        {
            bool isStopped = false;

            return isStopped;
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
                    await RegisterAccelerometerTask(accelerometer.DeviceId, taskName, arguments);
                }
                else
                {
                    //ShowNotifyMessage("App darf keine Background Tasks starten.", NotifyLevel.Error);
                }
            }
        }

        private async Task<bool> RegisterAccelerometerTask(string deviceId, string taskName, string arguments)
        {
            String taskEntryPoint = "BackgroundTask.TaskAction";
            DeviceUseTrigger trigger = new DeviceUseTrigger();

            foreach (var currentTask in BackgroundTaskRegistration.AllTasks)
            {
                if (currentTask.Value.Name == taskName)
                {
                    _backgroundTaskRegistration = (BackgroundTaskRegistration)(currentTask.Value);
                }
            }

            if (_backgroundTaskRegistration == null)
            {
                var builder = new BackgroundTaskBuilder();
                builder.Name = taskName;
                builder.TaskEntryPoint = taskEntryPoint;
                builder.SetTrigger(trigger);

                _backgroundTaskRegistration = builder.Register();

                if (!await RequestDeviceUseTrigger(deviceId, trigger, taskName, arguments))
                {
                    DeregisterAccelerometerTask(taskName);
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> RequestDeviceUseTrigger(string deviceId, DeviceUseTrigger deviceUseTrigger, string taskName, string arguments)
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
                DeregisterAccelerometerTask(taskName);
            }
            return false;
        }

        #endregion

        //#############################################################################
        //########################## Stop Background Task #############################
        //#############################################################################

        #region Stop BackgroundTask

        public void DeregisterAccelerometerTask(string taskName)
        {
            if (taskName != null && taskName.Length > 0)
            {
                foreach (var currentTask in BackgroundTaskRegistration.AllTasks)
                {
                    if (currentTask.Value.Name == taskName)
                    {
                        currentTask.Value.Unregister(true);
                        //ShowNotifyMessage("Background Task wurde beendet.", NotifyLevel.Info);
                    }
                }
                this._backgroundTaskRegistration = null;
            }
        }

        #endregion

    }
}
