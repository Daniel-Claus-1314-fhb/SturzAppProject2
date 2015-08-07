using BackgroundTask.Common;
using BackgroundTask.DataModel;
using BackgroundTask.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=391641 dokumentiert.

namespace BackgroundTask
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        private BackgroundTaskRegistration _backgroundTaskRegistration;
        private NotifyViewModel _notifyViewModel = new NotifyViewModel();

        private MeasurementList _measurementList;

        public MainPage()
        {
            this.InitializeComponent();

            NotifyMessageGrid.DataContext = NotifyViewModel;

            Current = this;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        public MeasurementList MeasurementList
        {
            get { return _measurementList; }
            set { _measurementList = value; }
        }

        #region Notify
          
        public NotifyViewModel NotifyViewModel
        {
            get { return _notifyViewModel; }
        }

        public void ShowNotifyMessage(string message, NotifyLevel level)
        {
            if (message != null && message.Length > 0) 
            {
                this._notifyViewModel.ShowMessage(message, level);
            }
        }

        public void ResetNotifyMessage()
        {
            this._notifyViewModel.ResetMessage();
        }

        #endregion

        /// <summary>
        /// Wird aufgerufen, wenn diese Seite in einem Rahmen angezeigt werden soll.
        /// </summary>
        /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.
        /// Dieser Parameter wird normalerweise zum Konfigurieren der Seite verwendet.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SuspensionManager.RegisterFrame(ContentFrame, "ContentFrame");
            if (ContentFrame.Content == null)
            {
                if (!ContentFrame.Navigate(typeof(OverviewPage)))
                {
                    throw new Exception("Failed to create page");
                }
            }
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();

                //Indicate the back button press is handled so the app does not exit
                e.Handled = true;
            }
        }

        #region Start/Stop BackgroundTask
        
        //#############################################################################
        //########################## Start Background Task ############################
        //#############################################################################

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
                    ShowNotifyMessage("App darf keine Background Tasks starten.", NotifyLevel.Error);
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
                        ShowNotifyMessage("Background Task wurde gestartet.", NotifyLevel.Info);
                        return true;
                    case DeviceTriggerResult.DeniedBySystem:
                        ShowNotifyMessage("Background Task wurde vom System verweigert.", NotifyLevel.Warn);
                        break;
                    case DeviceTriggerResult.DeniedByUser:
                        ShowNotifyMessage("Background Task wurde vom Nutzer verweigert.", NotifyLevel.Warn);
                        break;
                    case DeviceTriggerResult.LowBattery:
                        ShowNotifyMessage("Background Task wurde wegen geringer Batterie verweigert.", NotifyLevel.Warn);
                        break;
                }
            }
            catch (InvalidOperationException)
            {
                DeregisterAccelerometerTask(taskName);
            }
            return false;
        }

        //#############################################################################
        //########################## Stop Background Task #############################
        //#############################################################################

        public void DeregisterAccelerometerTask(string taskName)
        {
            if (taskName != null && taskName.Length > 0)
            {
                foreach (var currentTask in BackgroundTaskRegistration.AllTasks)
                {
                    if (currentTask.Value.Name == taskName)
                    {
                        currentTask.Value.Unregister(true);
                        ShowNotifyMessage("Background Task wurde beendet.", NotifyLevel.Info);
                    }
                }
                this._backgroundTaskRegistration = null;
            }
        }

        #endregion
    }
}
