using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackgroundTask.ViewModel.Setting
{
    public class EvaluationSettingViewModel : INotifyPropertyChanged
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public EvaluationSettingViewModel(bool isUsed, bool isRecordSamples, uint sampleBufferSize, 
            double accelerometerThreshold, double gyrometerThreshold, uint stepDistance, uint peakJoinDistance)
        {
            this.IsUsed = isUsed;
            this.IsRecordSamples = isRecordSamples;
            this.SampleBufferSize = sampleBufferSize;
            this.AccelerometerThreshold = accelerometerThreshold;
            this.GyrometerThreshold = gyrometerThreshold;
            this.StepDistance = stepDistance;
            this.PeakJoinDistance = peakJoinDistance;
        }
        
        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        private bool _isUsed;
        public bool IsUsed
        {
            get { return _isUsed; }
            set { this.SetProperty(ref this._isUsed, value); }
        }

        private bool _isRecordSamples;
        public bool IsRecordSamples
        {
            get { return _isRecordSamples; }
            set { this.SetProperty(ref this._isRecordSamples, value); }
        }

        private uint _sampleBufferSize;
        public uint SampleBufferSize
        {
            get { return _sampleBufferSize; }
            set { this.SetProperty(ref this._sampleBufferSize, value); }
        }

        /// <summary>
        /// Threshold which must be exceeded to identify a step.
        /// </summary>
        private double _accelerometerThreshold;
        public double AccelerometerThreshold
        {
            get { return _accelerometerThreshold; }
            set { this.SetProperty(ref this._accelerometerThreshold, value); }
        }
        /// <summary>
        /// Threshold which must be exceeded to identify a step.
        /// </summary>
        private double _gyrometerThreshold;
        public double GyrometerThreshold
        {
            get { return _gyrometerThreshold; }
            set { this.SetProperty(ref this._gyrometerThreshold, value); }
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
        /// TimeSpan in milliseconds within a accelerometer peak and a gyrometer peak will join to a detected step.
        /// </summary>
        private uint _peakJoinDistance;
        public uint PeakJoinDistance
        {
            get { return _peakJoinDistance; }
            set { this.SetProperty(ref this._peakJoinDistance, value); }
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
