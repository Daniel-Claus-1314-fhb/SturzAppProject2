using BackgroundTask.Common;
using BackgroundTask.DataModel;
using BackgroundTask.Service;
using BackgroundTask.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Provider;
using BackgroundTask.DataModel.Setting;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=391641 dokumentiert.

namespace BackgroundTask
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        private NotifyViewModel _notifyViewModel = new NotifyViewModel();
        private GlobalMeasurementModel _globalMeasurementModel;

        public MainPage()
        {
            this.InitializeComponent();

            NotifyMessageGrid.DataContext = NotifyViewModel;

            Current = this;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        public GlobalMeasurementModel GlobalMeasurementModel
        {
            get { return _globalMeasurementModel; }
        }

        public SettingModel GlobalSettingModel
        {
            get { return _globalMeasurementModel.GlobalSetting; }
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
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Show loader
            this.ShowLoader();

            if (_globalMeasurementModel == null)
            {
                _globalMeasurementModel = new GlobalMeasurementModel();
                _globalMeasurementModel.Measurements = await FileService.LoadGlobalMeasurementListAsync();
                _globalMeasurementModel.GlobalSetting = await FileService.LoadGlobalSettingAsync();
                _globalMeasurementModel.MeasurementListUpdated += SaveMeasurementList;
                _globalMeasurementModel.GlobalSettingUpdated += SaveGlobalSetting;
            }
            BackgroundTaskService.SynchronizeMeasurementsWithActiveBackgroundTasks(_globalMeasurementModel.Measurements);
            
            SuspensionManager.RegisterFrame(ContentFrame, "ContentFrame");
            if (ContentFrame.Content == null)
            {
                if (!ContentFrame.Navigate(typeof(OverviewPage)))
                {
                    throw new Exception("Failed to create page");
                }
            }

            // hide loader
            this.HideLoader();
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
                //Indicate the back button press is handled so the app does not exit
                e.Handled = true;
                ResetNotifyMessage();
            }
        }

        private void SaveMeasurementList(object sender, EventArgs e)
        {
            Debug.WriteLine("'{0}' Measurement has been saved.", _globalMeasurementModel.Measurements.Count);
            FileService.SaveGlobalMeasurementListAsync(_globalMeasurementModel.Measurements);
        }

        private void SaveGlobalSetting(object sender, EventArgs e)
        {
            Debug.WriteLine("Global setting model has been saved.");
            FileService.SaveGlobalSettingModelAysnc(_globalMeasurementModel.GlobalSetting);
        }

        public void ShowLoader()
        {
            VisualStateManager.GoToState(this, "Load", false);
        }

        public void HideLoader()
        {
            VisualStateManager.GoToState(this, "Loaded", false);
        }

        //############################################################################################################################################
        //################################################### Measurement Methods ####################################################################
        //############################################################################################################################################

        public async Task<bool> StartBackgroundTask(string measurementId)
        {
            bool isStarted = false;
            if (measurementId != null && measurementId.Length > 0 )
            {
                MeasurementModel measurement = this._globalMeasurementModel.GetMeasurementById(measurementId);
                if (measurement != null)
                {
                    isStarted = await BackgroundTaskService.StartBackgroundTaskForMeasurement(measurement);
                }
            }
            return isStarted;
        }

        public bool StopBackgroundTask(string measurementId)
        {
            bool isStopped = false;
            if (measurementId != null && measurementId.Length > 0)
            {
                MeasurementModel measurement = this._globalMeasurementModel.GetMeasurementById(measurementId);
                if (measurement != null)
                {
                    isStopped = BackgroundTaskService.StopBackgroundTaskForMeasurement(measurement);
                }
            }
            return isStopped;
        }

        public async Task<OxyplotData> FindMeasurementGraphData(string measurementId)
        {
            OxyplotData oxyplotData = new OxyplotData();

            if (measurementId != null && measurementId.Length > 0)
            {
                MeasurementModel measurement = this._globalMeasurementModel.GetMeasurementById(measurementId);
                if (measurement != null)
                {
                    oxyplotData = await FileService.LoadOxyplotDataAsync(measurement.Filename);
                }
            }
            return oxyplotData;
        }

    }
}
