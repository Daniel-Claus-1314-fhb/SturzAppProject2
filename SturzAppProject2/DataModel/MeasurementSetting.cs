using BackgroundTask.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public class MeasurementSetting
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region construtors

        /// <summary>
        /// Initilize a default MeasurementSettings model.
        /// </summary>
        public MeasurementSetting()
        {
            this.ReportInterval = 50;
            this.ProcessedSamplesCount = 200;
            this.PeakThreshold = 0.5d;
            this.StepDistance = 300;
            this.UseAccelerometer = true;
        }

        /// <summary>
        /// Constructor to create a new MeasurementSettings model from an exsisting MeasurementSettingsViewModel.
        /// </summary>
        /// <param name="measurementSettingViewModel"></param>
        public MeasurementSetting(MeasurementSettingViewModel measurementSettingViewModel) : this()
        {
            this.ReportInterval = measurementSettingViewModel.ReportInterval;
            this.ProcessedSamplesCount = measurementSettingViewModel.ProcessedSampleCount;
            this.PeakThreshold = measurementSettingViewModel.PeakThreshold;
            this.StepDistance = measurementSettingViewModel.StepDistance;
            this.UseAccelerometer = measurementSettingViewModel.UseAccelerometer;
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        /// <summary>
        /// TimeSpan in milliseconds between to accelerometer readings.
        /// </summary>
        public uint ReportInterval { get; set; }
        /// <summary>
        /// Amount of accelerometer readings which will analysed for step detection.
        /// </summary>
        public uint ProcessedSamplesCount { get; set; }
        /// <summary>
        /// Threshold which must be exceeded to identify a step.
        /// </summary>
        public double PeakThreshold { get; set; }
        /// <summary>
        /// TimeSpan in milliseconds which have to be past between to detected Steps.
        /// </summary>
        public uint StepDistance { get; set; }
        /// <summary>
        /// Is true when the accelerometer is used to detect steps.
        /// </summary>
        public bool UseAccelerometer { get; set; }

        #endregion
    }
}
