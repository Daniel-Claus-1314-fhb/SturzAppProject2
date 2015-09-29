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

// Die Elementvorlage "Standardseite" ist unter "http://go.microsoft.com/fwlink/?LinkID=390556" dokumentiert.

namespace BackgroundTask
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Frames navigiert werden kann.
    /// </summary>
    public sealed partial class MeasurementPage : Page, IFileSavePickerContinuable
    {
        MainPage _mainPage = MainPage.Current;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private MeasurementPageViewModel _measurementPageViewModel = null;
        private ThreadPoolTimer _periodicUpdateTimer = null;
        private BackgroundTaskProgressEventHandler _progressHandler = null;

        public MeasurementPage()
        {
            _measurementPageViewModel = new MeasurementPageViewModel(StartMeasurement, StopMeasurement, ExportMeasurement, ShowMeasurementGraph, RedoEvaluationGraph, DeleteMeasurement);

            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Ruft den <see cref="NavigationHelper"/> ab, der mit dieser <see cref="Page"/> verknüpft ist.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Ruft das Anzeigemodell für diese <see cref="Page"/> ab.
        /// Dies kann in ein stark typisiertes Anzeigemodell geändert werden.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public MeasurementPageViewModel MeasurementPageViewModel
        {
            get { return this._measurementPageViewModel; }
        }

        /// <summary>
        /// Füllt die Seite mit Inhalt auf, der bei der Navigation übergeben wird.  Gespeicherte Zustände werden ebenfalls
        /// bereitgestellt, wenn eine Seite aus einer vorherigen Sitzung neu erstellt wird.
        /// </summary>
        /// <param name="sender">
        /// Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Ereignisdaten, die die Navigationsparameter bereitstellen, die an
        /// <see cref="Frame.Navigate(Type, Object)"/> als diese Seite ursprünglich angefordert wurde und
        /// ein Wörterbuch des Zustands, der von dieser Seite während einer früheren
        /// beibehalten wurde.  Der Zustand ist beim ersten Aufrufen einer Seite NULL.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Behält den dieser Seite zugeordneten Zustand bei, wenn die Anwendung angehalten oder
        /// die Seite im Navigationscache verworfen wird.  Die Werte müssen den Serialisierungsanforderungen
        /// von <see cref="SuspensionManager.SessionState"/> entsprechen.
        /// </summary>
        /// <param name="sender">Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/></param>
        /// <param name="e">Ereignisdaten, die ein leeres Wörterbuch zum Auffüllen bereitstellen
        /// serialisierbarer Zustand.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper-Registrierung

        /// <summary>
        /// Die in diesem Abschnitt bereitgestellten Methoden werden einfach verwendet, um
        /// damit NavigationHelper auf die Navigationsmethoden der Seite reagieren kann.
        /// <para>
        /// Platzieren Sie seitenspezifische Logik in Ereignishandlern für  
        /// <see cref="NavigationHelper.LoadState"/>
        /// und <see cref="NavigationHelper.SaveState"/>.
        /// Der Navigationsparameter ist in der LoadState-Methode verfügbar 
        /// zusätzlich zum Seitenzustand, der während einer früheren Sitzung beibehalten wurde.
        /// </para>
        /// </summary>
        /// <param name="e">Stellt Daten für Navigationsmethoden und -ereignisse bereit.
        /// Handler, bei denen die Navigationsanforderung nicht abgebrochen werden kann.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string measurementId = e.Parameter as string;
            if (measurementId != null)
            {
                Measurement measurement = _mainPage.MainMeasurementListModel.GetById(measurementId);
                if (measurement != null)
                {
                    _measurementPageViewModel.MeasurementViewModel = new MeasurementViewModel(measurement);
                    VisualStateManager.GoToState(this, _measurementPageViewModel.MeasurementViewModel.MeasurementState.ToString(), true);

                    // Add timer event and attach eventlistner
                    StartUpdateTimer();
                    SetOnProgressEventListnerByMeasurementState(_measurementPageViewModel.MeasurementViewModel.Id, _measurementPageViewModel.MeasurementViewModel.MeasurementState);
                }
            }
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);

            // If the measurement has not started yet, then save settings of measurment
            if (_measurementPageViewModel.MeasurementViewModel.MeasurementState == MeasurementState.Initialized)
            {
                _mainPage.MainMeasurementListModel.Update(_measurementPageViewModel.MeasurementViewModel);
                RaiseCanExecuteChanged();
                _mainPage.ShowNotifyMessage("Messung wurde gespeichert.", NotifyLevel.Info);
            }

            // remove timer event and detach eventlistner
            StopUpdateTimer();
            DetachOnProgressEventListner(_measurementPageViewModel.MeasurementViewModel.Id);
        }

        #endregion

        //############################################################################################################################################
        //################################################### AppbarButton Methods ###################################################################
        //############################################################################################################################################
        #region DelegateMethods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementViewModel"></param>
        private async void StartMeasurement(MeasurementViewModel measurementViewModel)
        {
            // Show loader
            _mainPage.ShowLoader();

            bool isStarted = false;
            // first update for settings
            _mainPage.MainMeasurementListModel.Update(measurementViewModel);

            //start functionality
            isStarted = await _mainPage.StartBackgroundTask(measurementViewModel.Id);

            if (isStarted)
            {
                _measurementPageViewModel.MeasurementViewModel.StartMeasurement();
                //second update for successfully started measurement.
                _mainPage.MainMeasurementListModel.Update(measurementViewModel);
                // its importent to raise the change of measurementstate to all commands
                RaiseCanExecuteChanged();
                StartUpdateTimer();
                SetOnProgressEventListnerByMeasurementState(_measurementPageViewModel.MeasurementViewModel.Id, _measurementPageViewModel.MeasurementViewModel.MeasurementState);
                _mainPage.ShowNotifyMessage("Messung wurde gestarted.", NotifyLevel.Info);
            }
            else
            {
                _mainPage.ShowNotifyMessage("Messung konnte nicht gestarted werden.", NotifyLevel.Error);
            }

            // Hide loader
            _mainPage.HideLoader();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementViewModel"></param>
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
                _mainPage.MainMeasurementListModel.Update(measurementViewModel);

                // its importent to raise the change of measurementstate to all commands
                RaiseCanExecuteChanged();
                StopUpdateTimer();
                SetOnProgressEventListnerByMeasurementState(_measurementPageViewModel.MeasurementViewModel.Id, _measurementPageViewModel.MeasurementViewModel.MeasurementState);

                _mainPage.ShowNotifyMessage("Messung wurde gestoppt.", NotifyLevel.Info);
            }
            else
            {
                _mainPage.ShowNotifyMessage("Messung konnte nicht gestoppt werden.", NotifyLevel.Error);
            }

            // Hide loader
            _mainPage.HideLoader();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementViewModel"></param>
        private void ExportMeasurement(MeasurementViewModel measurementViewModel)
        {
            // Show loader
            _mainPage.ShowLoader();

            _mainPage.ExportMeasurementData(measurementViewModel.Id);

            // Hide loader
            _mainPage.HideLoader();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementViewModel"></param>
        private void RedoEvaluationGraph(MeasurementViewModel measurementViewModel)
        {
            if (measurementViewModel != null)
            {
                Frame contentFrame = _mainPage.FindName("ContentFrame") as Frame;
                contentFrame.Navigate(typeof(EvaluationPage), measurementViewModel.Id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementViewModel"></param>
        private void ShowMeasurementGraph(MeasurementViewModel measurementViewModel)
        {
            if (measurementViewModel != null)
            {
                Frame contentFrame = _mainPage.FindName("ContentFrame") as Frame;
                contentFrame.Navigate(typeof(GraphPage), measurementViewModel.Id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementViewModel"></param>
        private async void DeleteMeasurement(MeasurementViewModel measurementViewModel)
        {
            bool isDeleted = false;
            isDeleted = await _mainPage.MainMeasurementListModel.Delete(measurementViewModel.Id);

            if (isDeleted)
            {
                _measurementPageViewModel.MeasurementViewModel.DeleteMeasurement();
                RaiseCanExecuteChanged();
                _mainPage.ShowNotifyMessage("Messung wurde gelöscht.", NotifyLevel.Info);

                Frame contentFrame = MainPage.Current.FindName("ContentFrame") as Frame;
                if (contentFrame != null && contentFrame.CanGoBack)
                    contentFrame.GoBack();
            }
            else
            {
                _mainPage.ShowNotifyMessage("Messung konnte nicht gelöscht werden.", NotifyLevel.Error);
            }
        }

        private void RaiseCanExecuteChanged()
        {
            ((StartMeasurementCommand)_measurementPageViewModel.StartMeasurementCommand).OnCanExecuteChanged();
            ((StopMeasurementCommand)_measurementPageViewModel.StopMeasurementCommand).OnCanExecuteChanged();
            ((ExportMeasurementCommand)_measurementPageViewModel.ExportMeasurementCommand).OnCanExecuteChanged();
            ((ShowMeasurementGraphCommand)_measurementPageViewModel.ShowMeasurementGraphCommand).OnCanExecuteChanged();
            ((RedoEvaluationCommand)_measurementPageViewModel.RedoEvaluationCommand).OnCanExecuteChanged();
            ((DeleteMeasurementCommand)_measurementPageViewModel.DeleteMeasurementCommand).OnCanExecuteChanged();
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
                _mainPage.MainMeasurementListModel.Update(_measurementPageViewModel.MeasurementViewModel);
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
            Measurement measurement = _mainPage.MainMeasurementListModel.GetById(this.MeasurementPageViewModel.MeasurementViewModel.Id);
            StorageFile file = args.File;
            if (file != null && measurement != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // load data for export
                ExportData exportData = await FileService.LoadSamplesForExportAsync(measurement.Filename);
                // write data into file
                await FileIO.WriteTextAsync(file, exportData.ToExportCSVString());
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    //OutputTextBlock.Text = "File " + file.Name + " was saved.";
                    _mainPage.ShowNotifyMessage("Messung wurde exportiert.", NotifyLevel.Info);
                }
                else
                {
                    _mainPage.ShowNotifyMessage("Messung konnte nicht exportiert werden.", NotifyLevel.Warn);
                }
            }
            else
            {
                _mainPage.ShowNotifyMessage("Exportiervorgang abgebrochen.", NotifyLevel.Warn);
            }
        }

        //public async void ContinueFileSavePicker(FileSavePickerContinuationEventArgs args)
        //{
        //    StorageFile file = args.File;
        //    if (file != null)
        //    {
        //        // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
        //        CachedFileManager.DeferUpdates(file);
        //        // write to file

        //        StorageFolder resultFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Measurement");
        //        StorageFolder resultFolderAcc = await resultFolder.GetFolderAsync("Accelerometer");
        //        StorageFolder resultFolderGyr = await resultFolder.GetFolderAsync("Gyrometer");
        //        StorageFolder resultFolderQua = await resultFolder.GetFolderAsync("Quaternion");
        //        StorageFile resultFileAcc = await resultFolderAcc.GetFileAsync("Measurement_" + this.MeasurementPageViewModel.MeasurementViewModel.Id + ".csv");
        //        StorageFile resultFileGyr = await resultFolderGyr.GetFileAsync("Measurement_" + this.MeasurementPageViewModel.MeasurementViewModel.Id + ".csv");
        //        StorageFile resultFileQua = await resultFolderQua.GetFileAsync("Measurement_" + this.MeasurementPageViewModel.MeasurementViewModel.Id + ".csv");

        //        string writetext = "Accelerometer \n";
        //        writetext += await FileIO.ReadTextAsync(resultFileAcc);
        //        writetext += "Gyrometer \n";
        //        writetext += await FileIO.ReadTextAsync(resultFileGyr);
        //        writetext += "Quaternion \n";
        //        writetext += await FileIO.ReadTextAsync(resultFileQua);

        //        await FileIO.WriteTextAsync(file, writetext);
        //        // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
        //        // Completing updates may require Windows to ask for user input.
        //        FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
        //        if (status == FileUpdateStatus.Complete)
        //        {
        //            //OutputTextBlock.Text = "File " + file.Name + " was saved.";
        //            _mainPage.ShowNotifyMessage("Messung wurde exportiert.", NotifyLevel.Info);
        //        }
        //        else
        //        {
        //            _mainPage.ShowNotifyMessage("Messung konnte nicht exportiert werden.", NotifyLevel.Warn);
        //        }
        //    }
        //    else
        //    {
        //        _mainPage.ShowNotifyMessage("Exportiervorgang abgebrochen.", NotifyLevel.Warn);
        //    }
        //}
    }
}
