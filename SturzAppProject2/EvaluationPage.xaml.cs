﻿using BackgroundTask.DataModel;
using BackgroundTask.Service;
using BackgroundTask.ViewModel;
using BackgroundTask.ViewModel.Setting;
using SensorDataEvaluation.DataModel;
using SensorDataEvaluation.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkID=390556 dokumentiert.

namespace BackgroundTask
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class EvaluationPage : Page
    {
        MainPage _mainPage = MainPage.Current;

        MeasurementEvaluationService _measurementEvaluationService = new MeasurementEvaluationService();

        private EvaluationPageViewModel _evaluationPageViewModel;
        public EvaluationPageViewModel EvaluationPageViewModel
        {
            get { return _evaluationPageViewModel; }
        }

        public EvaluationPage()
        {
            _evaluationPageViewModel = new EvaluationPageViewModel();
            this.InitializeComponent();
        }

        /// <summary>
        /// Wird aufgerufen, wenn diese Seite in einem Frame angezeigt werden soll.
        /// </summary>
        /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.
        /// Dieser Parameter wird normalerweise zum Konfigurieren der Seite verwendet.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            string measurementId = e.Parameter as string;
            if (measurementId != null && measurementId != String.Empty)
            {
                MeasurementModel measurement = _mainPage.GlobalMeasurementModel.GetMeasurementById(measurementId);
                if (measurement != null)
                {
                    // Show loader
                    _mainPage.ShowLoader();
                    _evaluationPageViewModel.MeasurementViewModel = new MeasurementViewModel(measurement);
                    _evaluationPageViewModel.EvaluationDataModel = await EvaluationService.LoadSamplesForEvaluationAsync(measurement.Filename);
                    ((StartEvaluationCommand)_evaluationPageViewModel.StartEvaluationCommand).OnCanExecuteChanged();
                    // hide loader
                    _mainPage.HideLoader();
                }
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _evaluationPageViewModel = null;
        }

        private async void StartEvaluationAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            // Show loader
            _mainPage.ShowLoader();

            //prepare timer
            Stopwatch stopwatch = new Stopwatch();

            SettingViewModel setting = _evaluationPageViewModel.MeasurementViewModel.Setting;
            //prepare evaluation settings and evaluation data
            uint processedSampleCount = setting.EvaluationSettingViewModel.SampleBufferSize;
            double accelerometerThreshold = setting.EvaluationSettingViewModel.AccelerometerThreshold;
            double gyrometerThreshold = setting.EvaluationSettingViewModel.GyrometerThreshold;
            uint stepDistance = setting.EvaluationSettingViewModel.StepDistance;
            uint peakJoinDistance = setting.EvaluationSettingViewModel.PeakJoinDistance;
            EvaluationSettingModel evaluationSettingModel = new EvaluationSettingModel(processedSampleCount, accelerometerThreshold, gyrometerThreshold, stepDistance, peakJoinDistance);
            
            EvaluationDataModel evaluationDataModel = _evaluationPageViewModel.EvaluationDataModel;

            //set evaluation state to started
            _evaluationPageViewModel.EvaluationState = EvaluationState.Started;
            ((StartEvaluationCommand)_evaluationPageViewModel.StartEvaluationCommand).OnCanExecuteChanged();

            //process evaluation
            stopwatch.Start();
            _evaluationPageViewModel.EvalautionResultModel = await _measurementEvaluationService.RunEvaluationAfterMeasurementAsync(evaluationDataModel, evaluationSettingModel);
            uint totalDetectedSteps = _evaluationPageViewModel.EvalautionResultModel.DetectedSteps;
            _evaluationPageViewModel.MeasurementViewModel.TotalSteps = totalDetectedSteps;

            //propagate update
            MeasurementModel measurement = _mainPage.GlobalMeasurementModel.GetMeasurementById(_evaluationPageViewModel.MeasurementViewModel.Id);
            _mainPage.GlobalMeasurementModel.UpdateMeasurementInList(_evaluationPageViewModel.MeasurementViewModel);

            // Save evaluation if necessary
            if (setting.EvaluationSettingViewModel.IsRecordSamples)
            {
                await EvaluationService.SaveEvaluationDataToFileAsync(measurement.Filename, _evaluationPageViewModel.EvalautionResultModel);
            }

            //set evaluation state to stopped
            stopwatch.Stop();
            _evaluationPageViewModel.EvaluationState = EvaluationState.Stopped;
            ((StartEvaluationCommand)_evaluationPageViewModel.StartEvaluationCommand).OnCanExecuteChanged();
            _mainPage.ShowNotifyMessage(String.Format("Messung wurde innerhalb von '{0:f4}' Sekunden erneut ausgewertet.", stopwatch.Elapsed.Duration().TotalSeconds), NotifyLevel.Info);

            // hide loader
            _mainPage.HideLoader();
        }
    }
}
