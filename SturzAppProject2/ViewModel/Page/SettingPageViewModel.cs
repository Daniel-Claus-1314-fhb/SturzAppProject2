using BackgroundTask.ViewModel.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.ViewModel.Page
{
    public class SettingPageViewModel : INotifyPropertyChanged
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors
        
        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        private SettingViewModel _settingViewModel;
        public SettingViewModel SettingViewModel
        {
            get { return _settingViewModel; }
            set { this.SetProperty(ref this._settingViewModel, value); }
        }

        public bool isGlobalSetting { get; set; }


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
