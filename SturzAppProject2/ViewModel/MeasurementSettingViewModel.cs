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

        public MeasurementSettingViewModel()
        {
            this.ReportInterval = 20;
            this.ProcessedSampleCount = 1000;
            this.UseAccelerometer = true;
            this.UseGyrometer = false;
        }

        public MeasurementSettingViewModel(uint reportInterval, uint processedSampleCount, bool useAccelerometer, bool useGyrometer) : this()
        {
            this.ReportInterval = reportInterval;
            this.ProcessedSampleCount = processedSampleCount;
            this.UseAccelerometer = useAccelerometer;
            this.UseGyrometer = useGyrometer;
        }

        public MeasurementSettingViewModel(MeasurementSetting setting)
        {
            this.ReportInterval = setting.ReportInterval;
            this.ProcessedSampleCount = setting.ProcessedSamplesCount;
            this.UseAccelerometer = setting.UseAccelerometer;
            this.UseGyrometer = setting.UseGyrometer;
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        private uint _reportInterval;
        public uint ReportInterval
        {
            get { return _reportInterval; }
            set { this.SetProperty(ref this._reportInterval, value); }
        }

        private uint _processedSampleCount;

        public uint ProcessedSampleCount
        {
            get { return _processedSampleCount; }
            set { this.SetProperty(ref this._processedSampleCount, value); }
        }

        private bool _useAccelerometer;
        public bool UseAccelerometer
        {
            get { return _useAccelerometer; }
            set { this.SetProperty(ref this._useAccelerometer, value); }
        }

        private bool _useGyrometer;
        public bool UseGyrometer
        {
            get { return _useGyrometer; }
            set { this.SetProperty(ref this._useGyrometer, value); }
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
