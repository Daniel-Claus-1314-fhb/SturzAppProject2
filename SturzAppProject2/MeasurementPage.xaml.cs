using BackgroundTask.Common;
using BackgroundTask.DataModel;
using BackgroundTask.Service;
using BackgroundTask.ViewModel;
using Newtonsoft.Json;
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
using Windows.Graphics.Display;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
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
using Windows.Storage.Streams;
using SensorDataEvaluation.DataModel;
using Windows.UI.Popups;

// Die Elementvorlage "Standardseite" ist unter "http://go.microsoft.com/fwlink/?LinkID=390556" dokumentiert.

namespace BackgroundTask
{
    public sealed partial class MeasurementPage : Page, IFileSavePickerContinuable
    {
        MainPage _mainPage = MainPage.Current;

        private MeasurementPageViewModel _measurementPageViewModel = null;
        private ThreadPoolTimer _periodicUpdateTimer = null;
        private BackgroundTaskProgressEventHandler _progressHandler = null;

        public MeasurementPage()
        {
            _measurementPageViewModel = new MeasurementPageViewModel(StartMeasurement, StopMeasurement, ExportMeasurement, ShowMeasurementGraph, RedoEvaluationGraph, DeleteMeasurement);
            this.InitializeComponent();
        }
        
        public MeasurementPageViewModel MeasurementPageViewModel
        {
            get { return this._measurementPageViewModel; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string measurementId = e.Parameter as string;
            if (measurementId != null)
            {
                MeasurementModel measurement = _mainPage.GlobalMeasurementModel.GetMeasurementById(measurementId);
                if (measurement != null)
                {
                    _measurementPageViewModel.MeasurementViewModel = new MeasurementViewModel(measurement);
                    VisualStateManager.GoToState(this, _measurementPageViewModel.MeasurementViewModel.MeasurementState.ToString(), true);
                    // Add timer event and attach eventlistner
                    StartUpdateTimer();
                    SetOnProgressEventListnerByMeasurementState(_measurementPageViewModel.MeasurementViewModel.Id, _measurementPageViewModel.MeasurementViewModel.MeasurementState);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // If the measurement has not started yet, then save settings of measurment
            if (_measurementPageViewModel.MeasurementViewModel.MeasurementState == MeasurementState.Initialized)
            {
                _mainPage.GlobalMeasurementModel.UpdateMeasurementInList(_measurementPageViewModel.MeasurementViewModel);
                RaiseCanExecuteChanged();
                _mainPage.ShowNotifyMessage("Messung wurde gespeichert.", NotifyLevel.Info);
            }

            // remove timer event and detach eventlistner
            StopUpdateTimer();
            DetachOnProgressEventListner(_measurementPageViewModel.MeasurementViewModel.Id);
        }

        //############################################################################################################################################
        //################################################### AppbarButton Methods ###################################################################
        //############################################################################################################################################

        #region DelegateMethods

        private async void StartMeasurement(MeasurementViewModel measurementViewModel)
        {
            // Show loader
            _mainPage.ShowLoader();

            bool isStarted = false;
            // first update for settings
            _mainPage.GlobalMeasurementModel.UpdateMeasurementInList(measurementViewModel);
            //start functionality
            isStarted = await _mainPage.StartBackgroundTask(measurementViewModel.Id);
            if (isStarted)
            {
                _measurementPageViewModel.MeasurementViewModel.StartMeasurement();
                //second update for successfully started measurement.
                _mainPage.GlobalMeasurementModel.UpdateMeasurementInList(measurementViewModel);
                // its importent to raise the change of measurementstate to all commands
                RaiseCanExecuteChanged();
                StartUpdateTimer();
                SetOnProgressEventListnerByMeasurementState(_measurementPageViewModel.MeasurementViewModel.Id, _measurementPageViewModel.MeasurementViewModel.MeasurementState);
                _mainPage.ShowNotifyMessage("Messung wurde gestarted.", NotifyLevel.Info);
            }
            else { _mainPage.ShowNotifyMessage("Messung konnte nicht gestarted werden.", NotifyLevel.Error); }

            // Hide loader
            _mainPage.HideLoader();
        }

        private void StopMeasurement(MeasurementViewModel measurementViewModel)
        {
            // Show loader
            _mainPage.ShowLoader();

            bool isStopped = false;
            // stop functionality
            isStopped = _mainPage.StopBackgroundTask(measurementViewModel.Id);
            if (isStopped)
            {
                _measurementPageViewModel.MeasurementViewModel.StopMeasurement();
                _mainPage.GlobalMeasurementModel.UpdateMeasurementInList(measurementViewModel);
                // its importent to raise the change of measurementstate to all commands
                RaiseCanExecuteChanged();
                StopUpdateTimer();
                SetOnProgressEventListnerByMeasurementState(_measurementPageViewModel.MeasurementViewModel.Id, _measurementPageViewModel.MeasurementViewModel.MeasurementState);
                _mainPage.ShowNotifyMessage("Messung wurde gestoppt.", NotifyLevel.Info);
            }
            else { _mainPage.ShowNotifyMessage("Messung konnte nicht gestoppt werden.", NotifyLevel.Error); }

            // Hide loader
            _mainPage.HideLoader();
        }

        private void ExportMeasurement(MeasurementViewModel measurementViewModel)
        {
            // Show loader
            _mainPage.ShowLoader();
            //_mainPage.ExportMeasurementData(measurementViewMdel.Id);

            if (measurementViewModel.Id != null && measurementViewModel.Id.Length > 0)
            {
                MeasurementModel measurement = _mainPage.GlobalMeasurementModel.GetMeasurementById(measurementViewModel.Id);
                if (measurement != null)
                {
                    var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                    savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                    // Dropdown of file types the user can save the file as
                    savePicker.FileTypeChoices.Add("Binary-File", new List<string>() { ".bin" });
                    // Default file name if the user does not type one in or select a file to replace
                    savePicker.SuggestedFileName = String.Format("{0}_{1:yyyy-MM-dd_HH-mm-ss}", measurement.Name, measurement.StartTime);
                    // Open the file picker and call the method "ContinueFileSavePicker" when the user select a file.
                    savePicker.PickSaveFileAndContinue();
                }
            }
            // Hide loader
            _mainPage.HideLoader();
        }

        private void RedoEvaluationGraph(MeasurementViewModel measurementViewModel)
        {
            if (measurementViewModel != null)
            {
                Frame contentFrame = _mainPage.FindName("ContentFrame") as Frame;
                contentFrame.Navigate(typeof(EvaluationPage), measurementViewModel.Id);
            }
        }

        public void EditSetting(object sender, RoutedEventArgs e)
        {
            if (_measurementPageViewModel != null && _measurementPageViewModel.MeasurementViewModel != null)
            {
                Frame contentFrame = _mainPage.FindName("ContentFrame") as Frame;
                contentFrame.Navigate(typeof(SettingPage), _measurementPageViewModel.MeasurementViewModel.Id);
            }
        }

        private void ShowMeasurementGraph(MeasurementViewModel measurementViewModel)
        {
            if (measurementViewModel != null)
            {
                Frame contentFrame = _mainPage.FindName("ContentFrame") as Frame;
                contentFrame.Navigate(typeof(GraphPage), measurementViewModel.Id);
            }
        }

        private async void DeleteMeasurement(MeasurementViewModel measurementViewModel)
        {
            MessageDialog dialog = new MessageDialog("Messung löschen?");
            dialog.Commands.Add(new UICommand("OK"));
            dialog.Commands.Add(new UICommand("Abbrechen"));

            var dialogResult = await dialog.ShowAsync();

            if (dialogResult.Label.Equals("OK"))
            {
                bool isDeleted = false;
                isDeleted = await _mainPage.GlobalMeasurementModel.Delete(measurementViewModel.Id);

                if (isDeleted)
                {
                    _measurementPageViewModel.MeasurementViewModel.DeleteMeasurement();
                    RaiseCanExecuteChanged();
                    _mainPage.ShowNotifyMessage("Messung wurde gelöscht.", NotifyLevel.Info);

                    Frame contentFrame = MainPage.Current.FindName("ContentFrame") as Frame;
                    if (contentFrame != null && contentFrame.CanGoBack)
                        contentFrame.GoBack();
                }
                else { _mainPage.ShowNotifyMessage("Messung konnte nicht gelöscht werden.", NotifyLevel.Error); }
            }
            if (dialogResult.Label.Equals("Abbrechen")) { _mainPage.ShowNotifyMessage("Messung wurden nicht gelöscht.", NotifyLevel.Info); }
        }

        private void RaiseCanExecuteChanged()
        {
            _measurementPageViewModel.StartMeasurementCommand.OnCanExecuteChanged();
            _measurementPageViewModel.StopMeasurementCommand.OnCanExecuteChanged();
            _measurementPageViewModel.ExportMeasurementCommand.OnCanExecuteChanged();
            _measurementPageViewModel.ShowMeasurementGraphCommand.OnCanExecuteChanged();
            _measurementPageViewModel.RedoEvaluationCommand.OnCanExecuteChanged();
            _measurementPageViewModel.EditSettingCommand.OnCanExecuteChanged();
            _measurementPageViewModel.DeleteMeasurementCommand.OnCanExecuteChanged();
            VisualStateManager.GoToState(this, _measurementPageViewModel.MeasurementViewModel.MeasurementState.ToString(), true);
        }

        #endregion

        //############################################################################################################################################
        //################################################### OnProgress Event Handler ###############################################################
        //############################################################################################################################################
        #region OnProgress event

        internal void SetOnProgressEventListnerByMeasurementState(string measurementId, MeasurementState measurementState)
        {
            switch (measurementState)
            {
                case MeasurementState.Started:
                    AttachOnProgressEventListner(measurementId);
                    break;
                default:
                    DetachOnProgressEventListner(measurementId);
                    break;
            }
        }

        internal void AttachOnProgressEventListner(string measurementId)
        {
            if (_progressHandler == null)
            {
                _progressHandler = new BackgroundTaskProgressEventHandler(OnProgress);
            }
            BackgroundTaskService.AttachToOnProgressEvent(measurementId, _progressHandler);
        }

        internal void DetachOnProgressEventListner(string measurementId)
        {
            BackgroundTaskService.DetachToOnProgressEvent(measurementId, _progressHandler);
        }

        public void OnProgress(IBackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High,() => 
            {
                _measurementPageViewModel.MeasurementViewModel.TotalSteps = args.Progress;
                _mainPage.GlobalMeasurementModel.UpdateMeasurementInList(_measurementPageViewModel.MeasurementViewModel);
            });
        }

        #endregion

        //############################################################################################################################################
        //################################################### Update Timer ###########################################################################
        //############################################################################################################################################
        #region Update Timer

        private void StartUpdateTimer()
        {
            if (_periodicUpdateTimer == null && this._measurementPageViewModel.MeasurementViewModel.MeasurementState == MeasurementState.Started)
            {
                TimeSpan period = TimeSpan.FromSeconds(1);

                _periodicUpdateTimer = ThreadPoolTimer.CreatePeriodicTimer((source) =>
                {
                    TimeSpan currentTimeSpan = new TimeSpan(0L);

                    DateTime startTime = _measurementPageViewModel.MeasurementViewModel.StartTime;
                    DateTime endTime = _measurementPageViewModel.MeasurementViewModel.EndTime;

                    if (startTime.CompareTo(DateTime.MinValue) != 0 &&
                        endTime.CompareTo(DateTime.MinValue) != 0)
                    {
                        currentTimeSpan = endTime.Subtract(startTime);
                    }
                    else if (startTime.CompareTo(DateTime.MinValue) != 0 &&
                        endTime.CompareTo(DateTime.MinValue) == 0)
                    {
                        currentTimeSpan = DateTime.Now.Subtract(startTime);
                    }

                    Dispatcher.RunAsync(CoreDispatcherPriority.High, () => { this._measurementPageViewModel.MeasurementViewModel.Duration = currentTimeSpan; });

                }, period);
            }
        }

        private void StopUpdateTimer()
        {
            if (_periodicUpdateTimer != null)
            {
                _periodicUpdateTimer.Cancel();
            }
        }

        #endregion

        public async void ContinueFileSavePicker(FileSavePickerContinuationEventArgs args)
        {
            // Show loader
            _mainPage.ShowLoader();

            MeasurementModel measurement = _mainPage.GlobalMeasurementModel.GetMeasurementById(this.MeasurementPageViewModel.MeasurementViewModel.Id);
            StorageFile file = args.File;
            if (file != null && measurement != null)
            {
                _mainPage.ShowNotifyMessage(String.Format("Messung wird exportiert! Dies kann einige Zeit dauern."), NotifyLevel.Info);

                Stopwatch stopwatch = new Stopwatch();
                Stopwatch stopwatch2 = new Stopwatch();
                
                stopwatch.Start();
                // load data for export
                ExportData exportData = await FileService.LoadSamplesForExportAsync(measurement.Filename);
                stopwatch.Stop();
                Debug.WriteLine("Load export data from file: {0}", stopwatch.Elapsed.Duration());

                stopwatch2.Start();
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write data into file
                await FileService.SaveExportDataToFileAsync(file, exportData);
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                stopwatch2.Stop();
                Debug.WriteLine("Write export data to file: {0}", stopwatch2.Elapsed.Duration());

                if (status == FileUpdateStatus.Complete)
                {
                    _mainPage.ShowNotifyMessage(String.Format("Messung wurde innerhalb von '{0:f4}' Sekunden exportiert.", 
                        stopwatch.Elapsed.Duration().TotalSeconds + stopwatch2.Elapsed.Duration().TotalSeconds), NotifyLevel.Info);
                }
                else { _mainPage.ShowNotifyMessage("Messung konnte nicht exportiert werden.", NotifyLevel.Warn); }
            }
            else { _mainPage.ShowNotifyMessage("Exportiervorgang abgebrochen.", NotifyLevel.Warn); }
            // Hide loader
            _mainPage.HideLoader();
        }

    }
}
