using BackgroundTask.Common;
using BackgroundTask.DataModel;
using BackgroundTask.Service;
using BackgroundTask.ViewModel;
using Newtonsoft.Json;
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
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Standardseite" ist unter "http://go.microsoft.com/fwlink/?LinkID=390556" dokumentiert.

namespace BackgroundTask
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Frames navigiert werden kann.
    /// </summary>
    public sealed partial class MeasurementPage : Page
    {

        MainPage _mainPage = MainPage.Current;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private MeasurementPageViewModel _measurementPageViewModel;


        public MeasurementPage()
        {
            _measurementPageViewModel = new MeasurementPageViewModel(StartMeasurement, StopMeasurement, ExportMeasurement, DeleteMeasurement, ShowMeasurementGraph);
            
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
                }
            }
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);

            if (_measurementPageViewModel.MeasurementViewModel.MeasurementState == MeasurementState.Initialized)
            {
                _mainPage.MainMeasurementListModel.Update(_measurementPageViewModel.MeasurementViewModel);
                RaiseCanExecuteChanged();
                _mainPage.ShowNotifyMessage("Messung wurde gespeichert.", NotifyLevel.Info);
            }
        }

        #endregion



        private bool StartMeasurement(MeasurementViewModel measurementViewModel)
        {
            bool isStarted = false;
            // first update for settings
            _mainPage.MainMeasurementListModel.Update(measurementViewModel);

            //start functionality
            isStarted = _mainPage.StartBackgroundTask(measurementViewModel.Id);

            if (isStarted)
            {
                _measurementPageViewModel.MeasurementViewModel.StartMeasurement();
                //secound update for successfully started measurement.
                _mainPage.MainMeasurementListModel.Update(measurementViewModel);
                // its importent to raise the change of measurementstate to all commands
                RaiseCanExecuteChanged();
                _mainPage.ShowNotifyMessage("Messung wurde gestarted.", NotifyLevel.Info);
            }
            else
            {
                _mainPage.ShowNotifyMessage("Messung konnte nicht gestarted werden.", NotifyLevel.Error);
            }
            return true;
        }

        private bool StopMeasurement(MeasurementViewModel measurementViewModel)
        {
            bool isStopped = false;
            
            // stop functionality
            isStopped = _mainPage.StopBackgroundTask(measurementViewModel.Id);

            if (isStopped)
            {
                _measurementPageViewModel.MeasurementViewModel.StopMeasurement();
                _mainPage.MainMeasurementListModel.Update(measurementViewModel);
                // its importent to raise the change of measurementstate to all commands
                RaiseCanExecuteChanged();
                _mainPage.ShowNotifyMessage("Messung wurde gestoppt.", NotifyLevel.Info);
            }
            else
            {
                _mainPage.ShowNotifyMessage("Messung konnte nicht gestoppt werden.", NotifyLevel.Error);
            }

            return isStopped;
        }

        private bool ExportMeasurement(MeasurementViewModel measurementViewModel)
        {
            bool isExported = false;

            //TODO Insert Export functionality

            if (isExported)
            {
                _mainPage.ShowNotifyMessage("Messung wurde exportiert.", NotifyLevel.Info);
            }
            else
            {
                _mainPage.ShowNotifyMessage("Messung konnte nicht exportiert werden.", NotifyLevel.Warn);
            }
            return isExported;
        }

        private bool DeleteMeasurement(MeasurementViewModel measurementViewModel)
        {
            bool isDeleted = false;

            isDeleted = _mainPage.MainMeasurementListModel.Delete(measurementViewModel.Id);

            if (isDeleted)
            {
                _measurementPageViewModel.MeasurementViewModel.DeleteMeasurement();
                // TODO Delete all Files of the measurement
                // its importent to raise the change of measurementstate to all commands
                RaiseCanExecuteChanged();
                _mainPage.ShowNotifyMessage("Messung wurde gelöscht.", NotifyLevel.Info);
            }
            else
            {
                _mainPage.ShowNotifyMessage("Messung konnte nicht gelöscht werden.", NotifyLevel.Warn);
            }
            return isDeleted;
        }

        private async Task<bool> ShowMeasurementGraph(MeasurementViewModel measurementViewModel)
        {
            bool isGraphDataAvailable = false;

            measurementViewModel.OxyplotData = await _mainPage.FindMeasurementGraphData(measurementViewModel.Id);

            if (measurementViewModel.OxyplotData.HasAccelerometerReadings || measurementViewModel.OxyplotData.HasGyrometerReadings)
            {
                isGraphDataAvailable = true;
                Frame contentFrame = _mainPage.FindName("ContentFrame") as Frame;
                _mainPage.ShowNotifyMessage(String.Format("Graph der Messung mit dem Namen '{0}' wurde geladen.", measurementViewModel.Name), NotifyLevel.Info);
                contentFrame.Navigate(typeof(GraphPage), measurementViewModel.OxyplotData);
            }
            else
            {
                _mainPage.ShowNotifyMessage(String.Format("Graph der Messung mit dem Namen '{0}' konnten nicht geladen werden.", measurementViewModel.Name), NotifyLevel.Error);
            }
            return isGraphDataAvailable;
        }

        private void RaiseCanExecuteChanged()
        {
            ((StartMeasurementCommand)_measurementPageViewModel.StartMeasurementCommand).OnCanExecuteChanged();
            ((StopMeasurementCommand)_measurementPageViewModel.StopMeasurementCommand).OnCanExecuteChanged();
            ((ExportMeasurementCommand)_measurementPageViewModel.ExportMeasurementCommand).OnCanExecuteChanged();
            ((DeleteMeasurementCommand)_measurementPageViewModel.DeleteMeasurementCommand).OnCanExecuteChanged();
            ((ShowMeasurementGraphCommand)_measurementPageViewModel.ShowMeasurementGraphCommand).OnCanExecuteChanged();
        }
    }
}
