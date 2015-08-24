using BackgroundTask.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.ViewModel
{
    public class MeasurementSettingViewModel : INotifyPropertyChanged
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        /// <summary>
        /// Create a default MeasurementSettingViewModel
        /// </summary>
        public MeasurementSettingViewModel()
        {
            this.ReportInterval = 50;
            this.ProcessedSampleCount = 200;
            this.PeakThreshold = 0.5d;
            this.StepDistance = 300;
            this.UseAccelerometer = true;
        }

        /// <summary>
        /// Create a new MeasurementSettingsViewModel by a given MeasurementSetting Model.
        /// </summary>
        /// <param name="setting"></param>
        public MeasurementSettingViewModel(MeasurementSetting setting) : this()
        {
            this.ReportInterval = setting.ReportInterval;
            this.ProcessedSampleCount = setting.ProcessedSamplesCount;
            this.PeakThreshold = setting.PeakThreshold;
            this.StepDistance = setting.StepDistance;
            this.UseAccelerometer = setting.UseAccelerometer;
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        /// <summary>
        /// TimeSpan in milliseconds between to accelerometer readings.
        /// </summary>
        private uint _reportInterval;
        public uint ReportInterval
        {
            get { return _reportInterval; }
            set { this.SetProperty(ref this._reportInterval, value); }
        }
        /// <summary>
        /// Amount of accelerometer readings which will analysed for step detection.
        /// </summary>
        private uint _processedSampleCount;
        public uint ProcessedSampleCount
        {
            get { return _processedSampleCount; }
            set { this.SetProperty(ref this._processedSampleCount, value); }
        }
        /// <summary>
        /// Threshold which must be exceeded to identify a step.
        /// </summary>
        private double _peakThreshold;
        public double PeakThreshold
        {
            get { return _peakThreshold; }
            set { this.SetProperty(ref this._peakThreshold, value); }
        }
        /// <summary>
        /// TimeSpan in milliseconds which have to be past between to detected Steps.
        /// </summary>
        private uint _stepDistance;
        public uint StepDistance
        {
            get { return _stepDistance; }
            set { this.SetProperty(ref this._stepDistance, value); }
        }
        /// <summary>
        /// Is true when the accelerometer is used to detect steps.
        /// </summary>
        private bool _useAccelerometer;
        public bool UseAccelerometer
        {
            get { return _useAccelerometer; }
            set { this.SetProperty(ref this._useAccelerometer, value); }
        }

        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        #region Methods
        
        // property changed logic by jump start
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
