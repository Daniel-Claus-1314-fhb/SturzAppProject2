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
            this.ReportInterval = 20;
            this.ProcessedSamplesCount = 500;
            this.AccelerometerThreshold = 1.5d;
            this.GyrometerThreshold = 150d;
            this.StepDistance = 300;
            this.PeakJoinDistance = 150;
        }

        /// <summary>
        /// Constructor to create a new MeasurementSettings model from an exsisting MeasurementSettingsViewModel.
        /// </summary>
        /// <param name="measurementSettingViewModel"></param>
        public MeasurementSetting(MeasurementSettingViewModel measurementSettingViewModel) : this()
        {
            this.ReportInterval = measurementSettingViewModel.ReportInterval;
            this.ProcessedSamplesCount = measurementSettingViewModel.ProcessedSampleCount;
            this.AccelerometerThreshold = measurementSettingViewModel.AccelerometerThreshold;
            this.GyrometerThreshold = measurementSettingViewModel.GyrometerThreshold;
            this.StepDistance = measurementSettingViewModel.StepDistance;
            this.PeakJoinDistance = measurementSettingViewModel.PeakJoinDistance;
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
        /// Threshold which must be exceeded to become relevant for a step.
        /// </summary>
        public double AccelerometerThreshold { get; set; }
        /// <summary>
        /// Threshold which must be exceeded to become relevant for a step.
        /// </summary>
        public double GyrometerThreshold { get; set; }
        /// <summary>
        /// TimeSpan in milliseconds which have to be past between to detected steps.
        /// </summary>
        public uint StepDistance { get; set; }
        /// <summary>
        /// TimeSpan in milliseconds within a accelerometer peak and a gyrometer peak will join to a detected step.
        /// </summary>
        public uint PeakJoinDistance { get; set; }

        #endregion
    }
}
