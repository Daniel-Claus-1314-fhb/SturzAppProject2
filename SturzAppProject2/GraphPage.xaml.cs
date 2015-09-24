using BackgroundTask.Common;
using BackgroundTask.DataModel;
using BackgroundTask.ViewModel;
using OxyPlot;
using OxyPlot.Series;
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
    public sealed partial class GraphPage : Page
    {
        MainPage _mainPage = MainPage.Current;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private GraphPageViewModel _graphPageViewModel;

        public GraphPage()
        {
            _graphPageViewModel = new GraphPageViewModel(PlotShownAccerlerometerGraphs);

            PlotModel newPlotModel = new PlotModel();
            newPlotModel.Title = "Accelerometer Graph";
            _graphPageViewModel.PlotModel = newPlotModel;

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

        public GraphPageViewModel GraphPageViewModel
        {
            get { return _graphPageViewModel; }
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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // show loader
            _mainPage.ShowLoader();

            string measurementId = e.Parameter as string;
            if (measurementId != null && measurementId != String.Empty)
            {
                Measurement measurement = _mainPage.MainMeasurementListModel.GetById(measurementId);
                if (measurement != null)
                {
                    _mainPage.ShowNotifyMessage(String.Format("Graph der Messung mit dem Namen '{0}' wird geladen.", measurement.Name), NotifyLevel.Info);
                    OxyplotData oxyplotData = await _mainPage.FindMeasurementGraphData(measurement.Id);

                    if (oxyplotData != null)
                    {
                        _mainPage.ShowNotifyMessage(String.Format("Graph der Messung mit dem Namen '{0}' wurde geladen.", measurement.Name), NotifyLevel.Info);

                        if (oxyplotData.HasAccelerometerSamples)
                        {
                            // Gruppe 1
                            _graphPageViewModel.AccelerometerXLineSeries = oxyplotData.GetAccelerometerXLineSeries();
                            _graphPageViewModel.AccelerometerYLineSeries = oxyplotData.GetAccelerometerYLineSeries();
                            _graphPageViewModel.AccelerometerZLineSeries = oxyplotData.GetAccelerometerZLineSeries();
                            ((ShowGroup1Command)_graphPageViewModel.ShowGroup1Command).OnCanExecuteChanged();
                        }

                        if (oxyplotData.HasGyrometerSamples)
                        {
                            // Gruppe 2
                            _graphPageViewModel.GyrometerXLineSeries = oxyplotData.GetGyrometerXLineSeries();
                            _graphPageViewModel.GyrometerYLineSeries = oxyplotData.GetGyrometerYLineSeries();
                            _graphPageViewModel.GyrometerZLineSeries = oxyplotData.GetGyrometerZLineSeries();
                            ((ShowGroup2Command)_graphPageViewModel.ShowGroup2Command).OnCanExecuteChanged();
                        }

                        if (oxyplotData.HasQuaternionSamples)
                        {
                            // Gruppe 3
                            _graphPageViewModel.QuaterionWLineSeries = oxyplotData.GetQuaterionWLineSeries();
                            _graphPageViewModel.QuaterionXLineSeries = oxyplotData.GetQuaterionXLineSeries();
                            _graphPageViewModel.QuaterionYLineSeries = oxyplotData.GetQuaterionYLineSeries();
                            _graphPageViewModel.QuaterionZLineSeries = oxyplotData.GetQuaterionZLineSeries();
                            ((ShowGroup3Command)_graphPageViewModel.ShowGroup3Command).OnCanExecuteChanged();
                        }

                        if (oxyplotData.HasEvaluationSamples)
                        {
                            // Gruppe4
                            _graphPageViewModel.VectorLengthLineSeries = oxyplotData.GetAccelerometerVectorLengthLineSeries();
                            ((ShowGroup4Command)_graphPageViewModel.ShowGroup4Command).OnCanExecuteChanged();

                            // Gruppe5
                            _graphPageViewModel.StepLineSeries = oxyplotData.GetAccelerometerStepLineSeries();
                            ((ShowGroup5Command)_graphPageViewModel.ShowGroup5Command).OnCanExecuteChanged();
                        }

                        PlotShownAccerlerometerGraphs(_graphPageViewModel);
                    }
                    else
                        _mainPage.ShowNotifyMessage(String.Format("Graph der Messung mit dem Namen '{0}' konnten nicht geladen werden.", measurement.Name), NotifyLevel.Error);
                }
                else
                    _mainPage.ShowNotifyMessage(String.Format("Messung mit der ID '{0}' konnten nicht gefunden werden.", measurementId), NotifyLevel.Error);

            }
            // hide loader
            _mainPage.HideLoader();
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void PlotShownAccerlerometerGraphs(GraphPageViewModel currentGrapPageViewModel)
        {
            currentGrapPageViewModel.PlotModel.Series.Clear();
            if (currentGrapPageViewModel.ShowGroup1)
            {
                // Show group 1
                currentGrapPageViewModel.PlotModel.Series.Add(currentGrapPageViewModel.AccelerometerXLineSeries);
                currentGrapPageViewModel.PlotModel.Series.Add(currentGrapPageViewModel.AccelerometerYLineSeries);
                currentGrapPageViewModel.PlotModel.Series.Add(currentGrapPageViewModel.AccelerometerZLineSeries);
            } 
            if (currentGrapPageViewModel.ShowGroup2)
            {
                // Show group 2
                currentGrapPageViewModel.PlotModel.Series.Add(currentGrapPageViewModel.GyrometerXLineSeries);
                currentGrapPageViewModel.PlotModel.Series.Add(currentGrapPageViewModel.GyrometerYLineSeries);
                currentGrapPageViewModel.PlotModel.Series.Add(currentGrapPageViewModel.GyrometerZLineSeries);
            }
            if (currentGrapPageViewModel.ShowGroup3)
            {
                // Show group 3
                currentGrapPageViewModel.PlotModel.Series.Add(currentGrapPageViewModel.QuaterionWLineSeries);
                currentGrapPageViewModel.PlotModel.Series.Add(currentGrapPageViewModel.QuaterionXLineSeries);
                currentGrapPageViewModel.PlotModel.Series.Add(currentGrapPageViewModel.QuaterionYLineSeries);
                currentGrapPageViewModel.PlotModel.Series.Add(currentGrapPageViewModel.QuaterionZLineSeries);
            }
            if (currentGrapPageViewModel.ShowGroup4)
            {
                // Show group 4
                currentGrapPageViewModel.PlotModel.Series.Add(currentGrapPageViewModel.VectorLengthLineSeries);
            }
            if (currentGrapPageViewModel.ShowGroup5)
            {
                // Show group 5
                currentGrapPageViewModel.PlotModel.Series.Add(currentGrapPageViewModel.StepLineSeries);
            }
            // call InvalidatePlot(true) to update the graph data.
            currentGrapPageViewModel.PlotModel.InvalidatePlot(true);
        }

        private void ZoomInAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            // beim clicken des Buttons "+" soll sich die Auflösung der X-Achse (Zeit-Achse) vergrößern. Der Zoom der Y-Achse soll unverändert beleiben.
            _graphPageViewModel.PlotModel.DefaultXAxis.ZoomAtCenter(1.25);
            // call InvalidatePlot(true) to update the graph data.
            _graphPageViewModel.PlotModel.InvalidatePlot(true);
        }

        private void ZoomOutAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            // beim clicken des Buttons "-" soll sich die Auflösung der X-Achse (Zeit-Achse) verkleinern. Der Zoom der Y-Achse soll unverändert beleiben.
            _graphPageViewModel.PlotModel.DefaultXAxis.ZoomAtCenter(0.8);
            // call InvalidatePlot(true) to update the graph data.
            _graphPageViewModel.PlotModel.InvalidatePlot(true);
        }

        private void ResetViewAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            _graphPageViewModel.PlotModel.ResetAllAxes();
            // call InvalidatePlot(true) to update the graph data.
            _graphPageViewModel.PlotModel.InvalidatePlot(true);
        }
    }
}
