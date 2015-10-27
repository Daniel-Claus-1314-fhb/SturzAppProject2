using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.ViewModel.Setting
{
    public class GyrometerSettingViewModel : INotifyPropertyChanged
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public GyrometerSettingViewModel(bool isUsed, bool isRecordSamples, uint reportInterval)
        {
            this.IsUsed = isUsed;
            this.IsRecordSamples = isRecordSamples;
            this.ReportInterval = reportInterval;
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

        private uint _reportInterval;
        public uint ReportInterval
        {
            get { return _reportInterval; }
            set { this.SetProperty(ref this._reportInterval, value); }
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
