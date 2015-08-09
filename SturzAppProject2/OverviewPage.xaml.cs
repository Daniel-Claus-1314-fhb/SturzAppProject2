using BackgroundTask.Common;
using BackgroundTask.DataModel;
using BackgroundTask.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class OverviewPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private OverviewPageViewModel _overViewPageViewModel = new OverviewPageViewModel();

        private MainPage _mainPage;

        public OverviewPage()
        {
            this.InitializeComponent();

            this._mainPage = MainPage.Current;

            CreateMeasurementClick createMeasurementCommend = new CreateMeasurementClick();
            createMeasurementCommend.FuncPointer = AddNewMeasurement;
            _overViewPageViewModel.CreateMeasurementCommand = createMeasurementCommend;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public OverviewPageViewModel OverviewPageViewModel
        {
            get { return _overViewPageViewModel; }
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
            _overViewPageViewModel.MeasurementViewModels = _mainPage.mapping.mapTo(_mainPage.MainMeasurementListModel.Measurements);
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
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        public bool AddNewMeasurement()
        {
            // create new measurementModel
            Measurement createdMeasurement = new Measurement();

            // add measurementModel to mainpage modelList
            _mainPage.MainMeasurementListModel.Insert(createdMeasurement);

            // map created measurementModel to viewModel and add measuermentViewmodel to viewmodelList
            this._overViewPageViewModel.InsertMeasurement(new MeasurementViewModel(createdMeasurement));

            _mainPage.ShowNotifyMessage("Messung wurde erstellt.", NotifyLevel.Info);
            return true;
        }

        private void MeasurementList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            Frame contentFrame = _mainPage.FindName("ContentFrame") as Frame;
            if (listView != null && contentFrame != null)
            {
                MeasurementViewModel selectedMeasurementViewModel = listView.SelectedItem as MeasurementViewModel;
                if (selectedMeasurementViewModel != null)
                {
                    _mainPage.ShowNotifyMessage(String.Format("Messung mit dem Namen '{0}' wurde ausgewählt.", selectedMeasurementViewModel.Name), NotifyLevel.Info);
                    contentFrame.Navigate(typeof(MeasurementPage), selectedMeasurementViewModel.Id);
                }
            }
        }
    }
}
