using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.ViewModel.Setting
{
    public class BaseSettingViewModel : INotifyPropertyChanged
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public BaseSettingViewModel(String name, TimeSpan targetDuration, TimeSpan startOffsetDuration)
        {
            this.Name = name;
            this.TargetDuration = targetDuration;
            this.StartOffsetDuration = startOffsetDuration;
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        private String _name;
        public String Name
        {
            get { return _name; }
            set { this.SetProperty(ref this._name, value); }
        }

        private TimeSpan _targetDuration;
        public TimeSpan TargetDuration
        {
            get { return _targetDuration; }
            set { this.SetProperty(ref this._targetDuration, value); }
        }

        private TimeSpan _startOffsetDuration;
        public TimeSpan StartOffsetDuration
        {
            get { return _startOffsetDuration; }
            set { this.SetProperty(ref this._startOffsetDuration, value); }
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
